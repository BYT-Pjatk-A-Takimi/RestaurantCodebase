using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace RestaurantApp.Models;

public class Table
{
    [JsonInclude]
    private readonly List<Reservation> _reservations = new();

    [JsonConstructor]
    public Table(int tableNumber, int numberOfChairs, string tableType)
    {
        if (tableNumber <= 0)
            throw new ArgumentException("Table number must be positive.", nameof(tableNumber));

        if (numberOfChairs <= 0)
            throw new ArgumentException("Number of chairs must be positive.", nameof(numberOfChairs));

        if (string.IsNullOrWhiteSpace(tableType))
            throw new ArgumentException("Table type cannot be empty.", nameof(tableType));

        TableNumber = tableNumber;
        NumberOfChairs = numberOfChairs;
        TableType = tableType;
    }

    // BASIC ATTRIBUTES
    public int TableNumber { get; }

    public int NumberOfChairs { get; }   // BASIC: numberOfChairs

    public string TableType { get; }     // BASIC: tableType

    public IReadOnlyCollection<Reservation> Reservations => _reservations;

    public Reservation? GetReservation(DateOnly date) =>
        _reservations.Find(r => r.DateOfReservation == date);

    public bool Reserve(Customer customer, Reservation reservation)
    {
        if (customer is null)
            throw new ArgumentNullException(nameof(customer));

        if (reservation is null)
            throw new ArgumentNullException(nameof(reservation));

        if (GetReservation(reservation.DateOfReservation) is not null)
        {
            // aynı tarihte zaten rezervasyon varsa reddet
            return false;
        }

        _reservations.Add(reservation);
        customer.MakeReservation(reservation);
        return true;
    }

    internal void AddReservation(Reservation reservation)
    {
        if (reservation is null)
            throw new ArgumentNullException(nameof(reservation));

        if (GetReservation(reservation.DateOfReservation) is null)
        {
            _reservations.Add(reservation);
        }
    }
}