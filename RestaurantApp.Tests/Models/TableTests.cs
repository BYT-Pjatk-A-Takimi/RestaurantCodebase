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

    private static Restaurant CreateRestaurant()
    {
        return new Restaurant("Test Restaurant", 100);
    }

    [Test]
    public void Constructor_ShouldValidateArguments()
    {
        var restaurant = CreateRestaurant();
        Assert.Throws<ArgumentException>(() => new Table(0, 4, "Indoor", restaurant));
        Assert.Throws<ArgumentException>(() => new Table(1, 0, "Indoor", restaurant));
        Assert.Throws<ArgumentException>(() => new Table(1, 4, "", restaurant));
        Assert.Throws<ArgumentNullException>(() => new Table(1, 4, "Indoor", null!));
    }

    [Test]
    public void Reserve_AddsReservation_WhenDateFree()
    {
        var restaurant = CreateRestaurant();
        var table = new Table(1, 4, "Indoor", restaurant);
        var customer = CreateCustomer();
        var reservation = new Reservation(Guid.NewGuid(), new DateOnly(2025, 1, 1), new TimeOnly(19, 0), 2, table);

        var result = table.Reserve(customer, reservation);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.True);
            Assert.That(table.Reservations.Count, Is.EqualTo(1));
        });
    }

}