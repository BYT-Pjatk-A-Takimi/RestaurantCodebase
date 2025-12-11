using System;
using NUnit.Framework;
using RestaurantApp.Models;

namespace RestaurantApp.Tests.Models;

[TestFixture]
public class ReservationTests
{
    private static Restaurant CreateRestaurant()
    {
        return new Restaurant("Test Restaurant", 100);
    }

    private static Table CreateTable(int number, int capacity)
    {
        var restaurant = CreateRestaurant();
        return new Table(number, capacity, "DefaultArea", restaurant);
    }

    [Test]
    public void Reservation_Constructor_InitializesPropertiesCorrectly()
    {
        var id = Guid.NewGuid();
        var date = new DateOnly(2025, 5, 10);
        var time = new TimeOnly(19, 0);
        var partySize = 4;
        var table = CreateTable(1, 4);

        var reservation = new Reservation(id, date, time, partySize, table);

        Assert.Multiple(() =>
        {
            Assert.That(reservation.Id, Is.EqualTo(id));
            Assert.That(reservation.DateOfReservation, Is.EqualTo(date));
            Assert.That(reservation.TimeOfReservation, Is.EqualTo(time));
            Assert.That(reservation.PartySize, Is.EqualTo(partySize));
            Assert.That(reservation.Table, Is.EqualTo(table));
            Assert.That(reservation.Status, Is.EqualTo(ReservationStatus.Pending));
        });
    }

    [Test]
    public void Reservation_Constructor_Throws_WhenIdIsEmpty()
    {
        var date = new DateOnly(2025, 5, 10);
        var table = CreateTable(1, 4);

        Assert.Throws<ArgumentException>(() =>
            new Reservation(Guid.Empty, date, new TimeOnly(19, 0), 3, table));
    }

    [Test]
    public void Reservation_Constructor_Throws_WhenDateIsDefault()
    {
        var table = CreateTable(1, 4);

        Assert.Throws<ArgumentException>(() =>
            new Reservation(Guid.NewGuid(), default, new TimeOnly(19, 0), 3, table));
    }

    [Test]
    public void Reservation_Constructor_Throws_WhenTimeIsDefault()
    {
        var date = new DateOnly(2025, 5, 10);
        var table = CreateTable(1, 4);

        Assert.Throws<ArgumentException>(() =>
            new Reservation(Guid.NewGuid(), date, default, 3, table));
    }

    [Test]
    public void Reservation_Constructor_Throws_WhenPartySizeIsNotPositive()
    {
        var date = new DateOnly(2025, 5, 10);
        var table = CreateTable(1, 4);

        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new Reservation(Guid.NewGuid(), date, new TimeOnly(19, 0), 0, table));

        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new Reservation(Guid.NewGuid(), date, new TimeOnly(19, 0), -1, table));
    }

    [Test]
    public void Reservation_Constructor_AllowsNullTable()
    {
        var date = new DateOnly(2025, 5, 10);

        var reservation = new Reservation(Guid.NewGuid(), date, new TimeOnly(19, 0), 3, null);

        Assert.Multiple(() =>
        {
            Assert.That(reservation.Table, Is.Null);
            Assert.That(reservation.DateOfReservation, Is.EqualTo(date));
            Assert.That(reservation.PartySize, Is.EqualTo(3));
        });
    }

    [Test]
    public void Reservation_Confirm_SetsStatusToConfirmed()
    {
        var reservation = new Reservation(
            Guid.NewGuid(),
            dateOfReservation: new DateOnly(2025, 5, 10),
            timeOfReservation: new TimeOnly(19, 0),
            partySize: 2,
            table: CreateTable(1, 4));

        reservation.Confirm();

        Assert.That(reservation.Status, Is.EqualTo(ReservationStatus.Confirmed));
    }

    [Test]
    public void Reservation_Cancel_SetsStatusToCancelled()
    {
        var reservation = new Reservation(
            Guid.NewGuid(),
            new DateOnly(2025, 5, 10),
            new TimeOnly(20, 0),
            2,
            CreateTable(1, 4));

        reservation.Cancel();

        Assert.That(reservation.Status, Is.EqualTo(ReservationStatus.Cancelled));
    }

    [Test]
    public void Reservation_StatusChanges_DoNotAffectOtherProperties()
    {
        var id = Guid.NewGuid();
        var date = new DateOnly(2025, 5, 10);
        var table = CreateTable(1, 4);
        var reservation = new Reservation(id, date, new TimeOnly(19, 0), 4, table);

        reservation.Confirm();
        reservation.Cancel();

        Assert.That(reservation.Id, Is.EqualTo(id));
        Assert.That(reservation.DateOfReservation, Is.EqualTo(date));
        Assert.That(reservation.Table, Is.EqualTo(table));
        Assert.That(reservation.PartySize, Is.EqualTo(4));
        Assert.That(reservation.Status, Is.EqualTo(ReservationStatus.Cancelled));
    }

}
