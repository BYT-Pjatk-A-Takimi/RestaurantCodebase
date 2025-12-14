using System;
using System.Text.Json.Serialization;

namespace RestaurantApp.Models;

public class Reservation
{
    [JsonConstructor]
    public Reservation(Guid id, DateOnly dateOfReservation, TimeOnly timeOfReservation, int partySize, Table? table = null)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("Reservation id cannot be empty.", nameof(id));

        if (dateOfReservation == default)
            throw new ArgumentException("Date of reservation must be a valid date.", nameof(dateOfReservation));

        if (timeOfReservation == default)
            throw new ArgumentException("Time of reservation must be a valid time.", nameof(timeOfReservation));

        if (partySize <= 0)
            throw new ArgumentOutOfRangeException(nameof(partySize), "Party size must be greater than 0.");

        Id = id;
        DateOfReservation = dateOfReservation;
        TimeOfReservation = timeOfReservation;
        PartySize = partySize;
        _table = table;
        Status = ReservationStatus.Pending;

    }

    [JsonInclude]
    private Table? _table;
    [JsonInclude]
    private Customer? _customer;

    public Guid Id { get; }
    public DateOnly DateOfReservation { get; }
    public TimeOnly TimeOfReservation { get; }
    public int PartySize { get; }
    public Table? Table => _table;
    public Customer? Customer => _customer;
    public ReservationStatus Status { get; private set; }

    internal void SetTable(Table? table)
    {
        _table = table;
    }

    internal void SetCustomer(Customer? customer)
    {
        _customer = customer;
    }

    public void Confirm() => Status = ReservationStatus.Confirmed;

    public void Cancel() => Status = ReservationStatus.Cancelled;
}
