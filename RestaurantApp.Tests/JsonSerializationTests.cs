using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using NUnit.Framework;
using RestaurantApp;
using RestaurantApp.Models;
using RestaurantApp.Models.Roles;
using RestaurantApp.Models.Roles.EmployeeTypes;
using RestaurantApp.Models.Roles.MembershipStatus;

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
    public void Read_ShouldDeserializeIsoFormatStringToDateOnly()
    {
        var json = "\"2024-03-15\"";
        var reader = new Utf8JsonReader(System.Text.Encoding.UTF8.GetBytes(json));

        reader.Read();
        var result = _converter.Read(ref reader, typeof(DateOnly), new JsonSerializerOptions());

        Assert.That(result, Is.EqualTo(new DateOnly(2024, 3, 15)));
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
        var memberStatus = new MemberStatus(10, 2.5m);
        var customerRole = new CustomerRole("john@example.com", memberStatus);
        var person = new Person("John", "Doe", new DateOnly(1990, 5, 15), "555-1234", customerRole: customerRole);

        var json = JsonSerializer.Serialize(person, options);
        var deserialized = JsonSerializer.Deserialize<Person>(json, options);

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
        var table = new Table(1, 4, "Standard", restaurant);

        var memberStatus = new MemberStatus(5, 1.5m);
        var customerRole = new CustomerRole("jane@example.com", memberStatus);
        var customer = new Person("Jane", "Smith", new DateOnly(1985, 8, 20), "555-5678", customerRole: customerRole);
        var reservation = new Reservation(Guid.NewGuid(), new DateOnly(2024, 12, 25), new TimeOnly(19, 0), 2, table);
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
        var managerType = new ManagerType(3);
        var employeeRole = new EmployeeRole(workDetails, experienceProfile, managerType);
        var person = new Person("Alice", "Johnson", new DateOnly(1988, 3, 12), "555-9999", employeeRole: employeeRole);

        var json = JsonSerializer.Serialize(person, options);
        var deserialized = JsonSerializer.Deserialize<Person>(json, options);

        Assert.Multiple(() =>
        {
            Assert.That(deserialized, Is.Not.Null);
            Assert.That(deserialized!.FirstName, Is.EqualTo("Alice"));
            Assert.That(deserialized.BirthDate, Is.EqualTo(new DateOnly(1988, 3, 12)));
            Assert.That(deserialized.EmployeeRole?.WorkDetails.DateOfHiring, Is.EqualTo(new DateOnly(2020, 1, 10)));
        });
    }

    [Test]
    public void SerializeDeserialize_WithCircularReferences_ShouldNotThrow()
    {
        var options = JsonSerialization.GetDefaultOptions();
        var restaurant = new Restaurant("Circular Test", 50);
        var table = new Table(1, 4, "Standard", restaurant);

        var memberStatus = new MemberStatus(0, 1.0m);
        var customerRole = new CustomerRole("bob@example.com", memberStatus);
        var customer = new Person("Bob", "Wilson", new DateOnly(1992, 11, 5), "555-0000", customerRole: customerRole);
        var reservation = new Reservation(Guid.NewGuid(), new DateOnly(2024, 6, 1), new TimeOnly(20, 0), 2, table);
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
        var memberStatus = new MemberStatus(3, 2.0m);
        var customerRole = new CustomerRole("polly@example.com", memberStatus);
        var person = new Person("Polly", "Morphic", new DateOnly(1995, 4, 10), "555-1111", customerRole: customerRole);

        var json = JsonSerializer.Serialize(person, options);
        var deserialized = JsonSerializer.Deserialize<Person>(json, options);

        Assert.Multiple(() =>
        {
            Assert.That(deserialized, Is.Not.Null);
            Assert.That(deserialized!.IsCustomer, Is.True);
            Assert.That(deserialized.CustomerRole?.MembershipStatus, Is.InstanceOf<MemberStatus>());
            Assert.That(((MemberStatus)deserialized.CustomerRole!.MembershipStatus).Credits, Is.EqualTo(3));
        });
    }

    [Test]
    public void SerializeDeserialize_ListOfPayments_ShouldWork()
    {
        var options = JsonSerialization.GetDefaultOptions();

        var nonMemberStatus1 = new NonMemberStatus();
        var customerRole1 = new CustomerRole("test@example.com", nonMemberStatus1);
        var customer1 = new Person("Test", "User", new DateOnly(2000, 1, 1), "123", customerRole: customerRole1);

        var nonMemberStatus2 = new NonMemberStatus();
        var customerRole2 = new CustomerRole("test2@example.com", nonMemberStatus2);
        var customer2 = new Person("Test2", "User2", new DateOnly(2000, 1, 1), "124", customerRole: customerRole2);

        var payments = new List<Payment>
        {
            new Payment(new Order(customer1, new Table(1, 4, "Standard", new Restaurant("Restaurant 1", 100))), 100m, PaymentMethod.Card),
            new Payment(new Order(customer2, new Table(2, 4, "Standard", new Restaurant("Restaurant 2", 100))), 50m, PaymentMethod.Cash)
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

}
