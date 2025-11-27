using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using NUnit.Framework;
using RestaurantApp;
using RestaurantApp.Models;

namespace RestaurantApp.Tests;

[TestFixture]
public class JsonSerializationTests
{
    [Test]
    public void GetDefaultOptions_ShouldReturnOptionsWithCorrectSettings()
    {
        var options = JsonSerialization.GetDefaultOptions();

        Assert.Multiple(() =>
        {
            Assert.That(options.WriteIndented, Is.True);
            Assert.That(options.ReferenceHandler, Is.EqualTo(ReferenceHandler.IgnoreCycles));
            Assert.That(options.Converters, Is.Not.Empty);
            Assert.That(options.Converters, Has.Some.InstanceOf<DateOnlyJsonConverter>());
        });
    }

    [Test]
    public void GetDefaultOptions_ShouldReturnNewInstanceEachTime()
    {
        var options1 = JsonSerialization.GetDefaultOptions();
        var options2 = JsonSerialization.GetDefaultOptions();

        Assert.That(options1, Is.Not.SameAs(options2));
    }
}

[TestFixture]
public class DateOnlyJsonConverterTests
{
    private DateOnlyJsonConverter _converter = null!;

    [SetUp]
    public void SetUp()
    {
        _converter = new DateOnlyJsonConverter();
    }

    [Test]
    public void Write_ShouldSerializeDateOnlyToIsoFormat()
    {
        var date = new DateOnly(2024, 3, 15);
        using var stream = new System.IO.MemoryStream();
        using var writer = new Utf8JsonWriter(stream);

        _converter.Write(writer, date, new JsonSerializerOptions());
        writer.Flush();

        stream.Position = 0;
        var reader = new System.IO.StreamReader(stream);
        var json = reader.ReadToEnd();

        Assert.That(json, Is.EqualTo("\"2024-03-15\""));
    }

    [Test]
    public void Write_ShouldSerializeDifferentDatesCorrectly()
    {
        var testCases = new[]
        {
            new DateOnly(2024, 1, 1),
            new DateOnly(2024, 12, 31),
            new DateOnly(2000, 2, 29),
            new DateOnly(2023, 6, 15)
        };

        foreach (var date in testCases)
        {
            using var stream = new System.IO.MemoryStream();
            using var writer = new Utf8JsonWriter(stream);

            _converter.Write(writer, date, new JsonSerializerOptions());
            writer.Flush();

            stream.Position = 0;
            var reader = new System.IO.StreamReader(stream);
            var json = reader.ReadToEnd();
            var expected = $"\"{date:yyyy-MM-dd}\"";

            Assert.That(json, Is.EqualTo(expected), $"Failed for date {date}");
        }
    }

    [Test]
    public void Read_ShouldDeserializeIsoFormatStringToDateOnly()
    {
        var json = "\"2024-03-15\"";
        var reader = new Utf8JsonReader(System.Text.Encoding.UTF8.GetBytes(json));

        reader.Read();
        var result = _converter.Read(ref reader, typeof(DateOnly), new JsonSerializerOptions());

        Assert.That(result, Is.EqualTo(new DateOnly(2024, 3, 15)));
    }

    [Test]
    public void Read_ShouldDeserializeDifferentDateFormats()
    {
        var testCases = new[]
        {
            ("\"2024-01-01\"", new DateOnly(2024, 1, 1)),
            ("\"2024-12-31\"", new DateOnly(2024, 12, 31)),
            ("\"2000-02-29\"", new DateOnly(2000, 2, 29)),
            ("\"2023-06-15\"", new DateOnly(2023, 6, 15))
        };

        foreach (var (json, expectedDate) in testCases)
        {
            var reader = new Utf8JsonReader(System.Text.Encoding.UTF8.GetBytes(json));
            reader.Read();
            var result = _converter.Read(ref reader, typeof(DateOnly), new JsonSerializerOptions());

            Assert.That(result, Is.EqualTo(expectedDate), $"Failed for JSON {json}");
        }
    }

    [Test]
    public void Read_ShouldThrowExceptionForInvalidDateString()
    {
        var json = "\"invalid-date\"";
        var bytes = System.Text.Encoding.UTF8.GetBytes(json);
        var reader = new Utf8JsonReader(bytes);
        reader.Read();

        FormatException? exception = null;
        try
        {
            _converter.Read(ref reader, typeof(DateOnly), new JsonSerializerOptions());
        }
        catch (FormatException ex)
        {
            exception = ex;
        }

        Assert.That(exception, Is.Not.Null);
    }

    [Test]
    public void Read_ShouldThrowExceptionForNullString()
    {
        var json = "null";
        var bytes = System.Text.Encoding.UTF8.GetBytes(json);
        var reader = new Utf8JsonReader(bytes);
        reader.Read();

        ArgumentNullException? exception = null;
        try
        {
            _converter.Read(ref reader, typeof(DateOnly), new JsonSerializerOptions());
        }
        catch (ArgumentNullException ex)
        {
            exception = ex;
        }
        catch (NullReferenceException)
        {
            // DateOnly.Parse might throw NullReferenceException for null
            exception = new ArgumentNullException();
        }

        Assert.That(exception, Is.Not.Null);
    }

    [Test]
    public void RoundTrip_ShouldPreserveDateOnlyValue()
    {
        var originalDate = new DateOnly(2024, 7, 20);
        var options = JsonSerialization.GetDefaultOptions();

        var json = JsonSerializer.Serialize(originalDate, options);
        var deserializedDate = JsonSerializer.Deserialize<DateOnly>(json, options);

        Assert.That(deserializedDate, Is.EqualTo(originalDate));
    }
}

[TestFixture]
public class JsonSerializationIntegrationTests
{
    [Test]
    public void SerializeDeserialize_PersonWithDateOnly_ShouldWork()
    {
        var options = JsonSerialization.GetDefaultOptions();
        var person = new Member("John", "Doe", new DateOnly(1990, 5, 15), "555-1234", "john@example.com", 10, 2.5m);

        var json = JsonSerializer.Serialize(person, options);
        var deserialized = JsonSerializer.Deserialize<Member>(json, options);

        Assert.Multiple(() =>
        {
            Assert.That(deserialized, Is.Not.Null);
            Assert.That(deserialized!.FirstName, Is.EqualTo("John"));
            Assert.That(deserialized.LastName, Is.EqualTo("Doe"));
            Assert.That(deserialized.BirthDate, Is.EqualTo(new DateOnly(1990, 5, 15)));
        });
    }

    [Test]
    public void SerializeDeserialize_RestaurantWithDateOnlyFields_ShouldWork()
    {
        var options = JsonSerialization.GetDefaultOptions();
        var restaurant = new Restaurant("Test Restaurant", 100);
        var table = new Table(1, 4, "Standard");
        restaurant.AddTable(table);

        var customer = new Member("Jane", "Smith", new DateOnly(1985, 8, 20), "555-5678", "jane@example.com", 5, 1.5m);
        var reservation = new Reservation(Guid.NewGuid(), new DateOnly(2024, 12, 25), 2, table);
        table.Reserve(customer, reservation);

        // Test that serialization doesn't throw and produces valid JSON
        string json;
        Assert.DoesNotThrow(() => json = JsonSerializer.Serialize(restaurant, options));
        json = JsonSerializer.Serialize(restaurant, options);
        
        Assert.That(json, Does.Contain("Test Restaurant"));
        Assert.That(json, Does.Contain("2024-12-25"));
        
        // Deserialization should not throw
        Restaurant? deserialized = null;
        Assert.DoesNotThrow(() => deserialized = JsonSerializer.Deserialize<Restaurant>(json, options));
    }

    [Test]
    public void SerializeDeserialize_EmployeeWithWorkDetails_ShouldPreserveDateOnly()
    {
        var options = JsonSerialization.GetDefaultOptions();
        var workDetails = new WorkDetails("Kitchen", "Day", new DateOnly(2020, 1, 10));
        var experienceProfile = new ExperiencedProfile(5, "Mentor Name");
        var manager = new Manager("Alice", "Johnson", new DateOnly(1988, 3, 12), "555-9999", workDetails, experienceProfile, 3);

        var json = JsonSerializer.Serialize(manager, options);
        var deserialized = JsonSerializer.Deserialize<Manager>(json, options);

        Assert.Multiple(() =>
        {
            Assert.That(deserialized, Is.Not.Null);
            Assert.That(deserialized!.FirstName, Is.EqualTo("Alice"));
            Assert.That(deserialized.BirthDate, Is.EqualTo(new DateOnly(1988, 3, 12)));
            Assert.That(deserialized.WorkDetails.DateOfHiring, Is.EqualTo(new DateOnly(2020, 1, 10)));
        });
    }

    [Test]
    public void SerializeDeserialize_WithCircularReferences_ShouldNotThrow()
    {
        var options = JsonSerialization.GetDefaultOptions();
        var restaurant = new Restaurant("Circular Test", 50);
        var table = new Table(1, 4, "Standard");
        restaurant.AddTable(table);

        var customer = new Member("Bob", "Wilson", new DateOnly(1992, 11, 5), "555-0000", "bob@example.com", 0, 1.0m);
        var reservation = new Reservation(Guid.NewGuid(), new DateOnly(2024, 6, 1), 2, table);
        table.Reserve(customer, reservation);

        // This should not throw due to circular references
        Assert.DoesNotThrow(() =>
        {
            var json = JsonSerializer.Serialize(restaurant, options);
            var deserialized = JsonSerializer.Deserialize<Restaurant>(json, options);
            Assert.That(deserialized, Is.Not.Null);
        });
    }

    [Test]
    public void SerializeDeserialize_PolymorphicTypes_ShouldPreserveTypeInformation()
    {
        var options = JsonSerialization.GetDefaultOptions();
        var member = new Member("Polly", "Morphic", new DateOnly(1995, 4, 10), "555-1111", "polly@example.com", 3, 2.0m);

        var json = JsonSerializer.Serialize((Customer)member, options);
        var deserialized = JsonSerializer.Deserialize<Customer>(json, options);

        Assert.Multiple(() =>
        {
            Assert.That(deserialized, Is.Not.Null);
            Assert.That(deserialized, Is.InstanceOf<Member>());
            Assert.That(((Member)deserialized!).Credits, Is.EqualTo(3));
        });
    }

    [Test]
    public void SerializeDeserialize_ListOfPayments_ShouldWork()
    {
        var options = JsonSerialization.GetDefaultOptions();
        var payments = new List<Payment>
        {
            new Payment(Guid.NewGuid(), 100m, PaymentMethod.Card),
            new Payment(Guid.NewGuid(), 50m, PaymentMethod.Cash)
        };

        var json = JsonSerializer.Serialize(payments, options);
        var deserialized = JsonSerializer.Deserialize<List<Payment>>(json, options);

        Assert.Multiple(() =>
        {
            Assert.That(deserialized, Is.Not.Null);
            Assert.That(deserialized!.Count, Is.EqualTo(2));
            Assert.That(deserialized[0].Amount, Is.EqualTo(100m));
            Assert.That(deserialized[1].Amount, Is.EqualTo(50m));
        });
    }

    [Test]
    public void Serialize_ShouldProduceIndentedJson()
    {
        var options = JsonSerialization.GetDefaultOptions();
        var person = new Member("Test", "User", new DateOnly(2000, 1, 1), "555-0000", "test@example.com", 0, 1.0m);

        var json = JsonSerializer.Serialize(person, options);

        Assert.That(json, Does.Contain("\n"));
        Assert.That(json, Does.Contain("  "));
    }
}

