using System;
using System.Text.Json.Serialization;

namespace RestaurantApp.Models;

public class Reservation
{
    private Guid _id;
    private DateOnly _dateOfReservation;
    private TimeOnly _timeOfReservation;
    private int _partySize;

    [JsonConstructor]
    public Reservation(Guid id, DateOnly dateOfReservation, TimeOnly timeOfReservation, int partySize, Table? table = null)
    {
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
    private Person? _customer;

    public Guid Id
    {
        get => _id;
        private set
        {
            if (value == Guid.Empty)
                throw new ArgumentException("Reservation id cannot be empty.", nameof(Id));
            _id = value;
        }
    }

    public DateOnly DateOfReservation
    {
        get => _dateOfReservation;
        private set
        {
            if (value == default)
                throw new ArgumentException("Date of reservation must be a valid date.", nameof(DateOfReservation));
            _dateOfReservation = value;
        }
    }

    public TimeOnly TimeOfReservation
    {
        get => _timeOfReservation;
        private set
        {
            if (value == default)
                throw new ArgumentException("Time of reservation must be a valid time.", nameof(TimeOfReservation));
            _timeOfReservation = value;
        }
    }

    public int PartySize
    {
        get => _partySize;
        private set
        {
            if (value <= 0)
                throw new ArgumentOutOfRangeException(nameof(PartySize), "Party size must be greater than 0.");
            _partySize = value;
        }
    }
    public Table? Table => _table;
    public Person? Customer => _customer;
    public ReservationStatus Status { get; private set; }

    internal void SetTable(Table? table)
    {
        if (_table == table)
            return;

        _table = table;

        if (table != null && !table.Reservations.Contains(this))
        {
            table.AddReservation(this);
        }
    }

    internal void SetCustomer(Person? customer)
    {
        if (_customer == customer)
            return;

        _customer = customer;
        
        if (customer != null && customer.CustomerRole != null)
        {
            if (!customer.CustomerRole.Reservations.Contains(this))
            {
                customer.CustomerRole.MakeReservation(this);
            }
        }
    }

    public void Confirm() => Status = ReservationStatus.Confirmed;

    public void Cancel() => Status = ReservationStatus.Cancelled;
}
