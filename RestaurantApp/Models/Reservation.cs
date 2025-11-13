using System;

namespace RestaurantApp.Models;

public class Reservation
{
    public Reservation(Guid id, DateOnly dateOfReservation, int partySize, Table table)
    {
        Id = id;
        DateOfReservation = dateOfReservation;
        PartySize = partySize;
        Table = table;
        Status = ReservationStatus.Pending;
    }

    public Guid Id { get; }

    public DateOnly DateOfReservation { get; }

    public int PartySize { get; }

    public Table Table { get; }

    public ReservationStatus Status { get; private set; }

    public void Confirm() => Status = ReservationStatus.Confirmed;

    public void Cancel() => Status = ReservationStatus.Cancelled;
}

public enum ReservationStatus
{
    Pending,
    Confirmed,
    Cancelled
}

