using System;
using NUnit.Framework;
using RestaurantApp.Models;

namespace RestaurantApp.Tests.Models;

[TestFixture]
public class TableTests
{
    private static NonMember CreateCustomer()
    {
        return new NonMember("Test", "User", new DateOnly(2000, 1, 1), "123456789", "test@example.com");
    }

    [Test]
    public void Constructor_ShouldValidateArguments()
    {
        Assert.Throws<ArgumentException>(() => new Table(0, 4, "Indoor"));
        Assert.Throws<ArgumentException>(() => new Table(1, 0, "Indoor"));
        Assert.Throws<ArgumentException>(() => new Table(1, 4, ""));
    }

    [Test]
    public void Reserve_AddsReservation_WhenDateFree()
    {
        var table = new Table(1, 4, "Indoor");
        var customer = CreateCustomer();
        var reservation = new Reservation(Guid.NewGuid(), new DateOnly(2025, 1, 1), 2, table);

        var result = table.Reserve(customer, reservation);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.True);
            Assert.That(table.Reservations.Count, Is.EqualTo(1));
        });
    }

    [Test]
    public void Reserve_ReturnsFalse_WhenDateAlreadyReserved()
    {
        var table = new Table(1, 4, "Indoor");
        var customer = CreateCustomer();

        var date = new DateOnly(2025, 1, 1);

        var r1 = new Reservation(Guid.NewGuid(), date, 2, table);
        var r2 = new Reservation(Guid.NewGuid(), date, 3, table);

        table.Reserve(customer, r1);
        var result = table.Reserve(customer, r2);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.False);
            Assert.That(table.Reservations.Count, Is.EqualTo(1));
        });
    }
}