using System;
using System.Collections.Generic;

namespace RestaurantApp.Models;

public class Table
{
    private readonly List<Reservation> _reservations = new();

    public Table(int tableNumber, int numberOfChairs, string tableType)
    {
        TableNumber = tableNumber;
        NumberOfChairs = numberOfChairs;
        TableType = tableType;
    }

    public int TableNumber { get; }

    public int NumberOfChairs { get; }

    public string TableType { get; }

    public IReadOnlyCollection<Reservation> Reservations => _reservations;

    public Reservation? GetReservation(DateOnly date) =>
        _reservations.Find(r => r.DateOfReservation == date);

    public bool Reserve(Customer customer, Reservation reservation)
    {
        if (GetReservation(reservation.DateOfReservation) is not null)
        {
            return false;
        }

        _reservations.Add(reservation);
        customer.MakeReservation(reservation);
        return true;
    }

    internal void AddReservation(Reservation reservation)
    {
        if (GetReservation(reservation.DateOfReservation) is null)
        {
            _reservations.Add(reservation);
        }
    }
}

