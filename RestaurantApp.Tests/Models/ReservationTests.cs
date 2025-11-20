using System;
using NUnit.Framework;
using RestaurantApp.Models;

namespace RestaurantApp.Tests.Models;

[TestFixture]
public class ReservationTests
{
    [Test]
    public void Reservation_Constructor_InitializesPropertiesCorrectly()
    {
        var id = Guid.NewGuid();
        var date = new DateOnly(2025, 5, 10);
        var partySize = 4;
        var table = new Table(1, 4);

        var reservation = new Reservation(id, date, partySize, table);

        Assert.That(reservation.Id, Is.EqualTo(id));
        Assert.That(reservation.DateOfReservation, Is.EqualTo(date));
        Assert.That(reservation.PartySize, Is.EqualTo(partySize));
        Assert.That(reservation.Table, Is.EqualTo(table));
        Assert.That(reservation.Status, Is.EqualTo(ReservationStatus.Pending));
    }

    [Test]
    public void Reservation_Confirm_SetsStatusToConfirmed()
    {
        var reservation = new Reservation(Guid.NewGuid(), new DateOnly(2025, 5, 10), 2, new Table(1, 4));
        reservation.Confirm();
        Assert.That(reservation.Status, Is.EqualTo(ReservationStatus.Confirmed));
    }

    [Test]
    public void Reservation_Cancel_SetsStatusToCancelled()
    {
        var reservation = new Reservation(Guid.NewGuid(), new DateOnly(2025, 5, 10), 2, new Table(1, 4));
        reservation.Cancel();
        Assert.That(reservation.Status, Is.EqualTo(ReservationStatus.Cancelled));
    }

    [Test]
    public void Reservation_StatusChanges_DoNotAffectOtherProperties()
    {
        var id = Guid.NewGuid();
        var date = new DateOnly(2025, 5, 10);
        var table = new Table(1, 4);
        var reservation = new Reservation(id, date, 4, table);

        reservation.Confirm();
        reservation.Cancel();

        Assert.That(reservation.Id, Is.EqualTo(id));
        Assert.That(reservation.DateOfReservation, Is.EqualTo(date));
        Assert.That(reservation.Table, Is.EqualTo(table));
        Assert.That(reservation.PartySize, Is.EqualTo(4));
        Assert.That(reservation.Status, Is.EqualTo(ReservationStatus.Cancelled));
    }

    [Test]
    public void Reservation_ValidData_DoesNotThrowException()
    {
        Assert.DoesNotThrow(() =>
        {
            var _ = new Reservation(Guid.NewGuid(), new DateOnly(2025, 5, 10), 3, new Table(1, 4));
        });
    }
}
