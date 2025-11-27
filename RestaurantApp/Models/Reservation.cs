using System;
using System.Text.Json.Serialization;

namespace RestaurantApp.Models;

public class Reservation
{
    [JsonConstructor]
    public Reservation(Guid id, DateOnly dateOfReservation, int partySize, Table table)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("Reservation id cannot be empty.", nameof(id));

        if (dateOfReservation == default)
            throw new ArgumentException("Date of reservation must be a valid date.", nameof(dateOfReservation));

        if (partySize <= 0)
            throw new ArgumentOutOfRangeException(nameof(partySize), "Party size must be greater than 0.");

        if (table is null)
            throw new ArgumentNullException(nameof(table), "Table cannot be null.");

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
