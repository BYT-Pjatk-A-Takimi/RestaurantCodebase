using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace RestaurantApp.Models;

public class Table
{
    [JsonInclude]
    private readonly List<Reservation> _reservations = new();
    [JsonInclude]
    private readonly List<Order> _orders = new();
    [JsonInclude]
    private Restaurant _restaurant;
    [JsonInclude]
    private Waiter? _waiter;

    [JsonConstructor]
    public Table(int tableNumber, int numberOfChairs, string tableType, Restaurant restaurant, bool skipAutoAdd = false)
    {
        if (restaurant is null)
            throw new ArgumentNullException(nameof(restaurant), "Table cannot exist without a Restaurant (composition).");

        TableNumber = tableNumber;
        NumberOfChairs = numberOfChairs;
        TableType = tableType;
        _restaurant = restaurant;

        if (!skipAutoAdd)
        {
            restaurant.AddTable(this);
        }
    }

    // BASIC ATTRIBUTES
    private int _tableNumber;
    public int TableNumber
    {
        get => _tableNumber;
        private set
        {
            if (value <= 0)
                throw new ArgumentException("Table number must be positive.", nameof(TableNumber));
            _tableNumber = value;
        }
    }

    private int _numberOfChairs;
    public int NumberOfChairs
    {
        get => _numberOfChairs;
        private set
        {
            if (value <= 0)
                throw new ArgumentException("Number of chairs must be positive.", nameof(NumberOfChairs));
            _numberOfChairs = value;
        }
    }

    private string _tableType = string.Empty;
    public string TableType
    {
        get => _tableType;
        private set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Table type cannot be empty.", nameof(TableType));
            _tableType = value;
        }
    }

    public Restaurant Restaurant => _restaurant;

    public Waiter? Waiter => _waiter;

    public IReadOnlyCollection<Reservation> Reservations => _reservations;

    public IReadOnlyCollection<Order> Orders => _orders;

    public Reservation? GetReservation(DateOnly date, TimeOnly time) =>
        _reservations.Find(r => r.DateOfReservation == date && r.TimeOfReservation == time);

    public Reservation? GetReservation(DateOnly date) =>
        _reservations.Find(r => r.DateOfReservation == date);

    public bool Reserve(Customer customer, Reservation reservation)
    {
        if (customer is null)
            throw new ArgumentNullException(nameof(customer));

        if (reservation is null)
            throw new ArgumentNullException(nameof(reservation));

        if (_reservations.Contains(reservation))
        {
            try
            {
                customer.MakeReservation(reservation);
            }
            catch (ArgumentException)
            {
            }
            return true;
        }

        if (GetReservation(reservation.DateOfReservation, reservation.TimeOfReservation) is not null)
        {
            // aynı tarih ve saatte zaten rezervasyon varsa reddet
            return false;
        }

        AddReservation(reservation);
        customer.MakeReservation(reservation);
        return true;
    }

    internal void AddReservation(Reservation reservation)
    {
        if (reservation is null)
            throw new ArgumentNullException(nameof(reservation));

        if (reservation.Table != this && reservation.Table != null)
            throw new ArgumentException("Reservation belongs to a different table.", nameof(reservation));

        if (_reservations.Contains(reservation))
            return;

        _reservations.Add(reservation);
        reservation.SetTable(this);
    }

    internal void RemoveReservation(Reservation reservation)
    {
        if (reservation is null)
            throw new ArgumentNullException(nameof(reservation));

        _reservations.Remove(reservation);
        reservation.SetTable(null);
    }

    internal void SetRestaurant(Restaurant restaurant)
    {
        if (restaurant is null)
            throw new ArgumentNullException(nameof(restaurant), "Table cannot exist without a Restaurant (composition).");
        _restaurant = restaurant;
    }

    internal void SetWaiter(Waiter? waiter)
    {
        _waiter = waiter;
    }

    internal void AddOrder(Order order)
    {
        if (order is null)
            throw new ArgumentNullException(nameof(order));

        if (order.Table != this && order.Table != null)
            throw new ArgumentException("Order belongs to a different table.", nameof(order));

        if (_orders.Contains(order))
            return;

        _orders.Add(order);
        order.SetTable(this);
    }

    internal void RemoveOrder(Order order)
    {
        if (order is null)
            throw new ArgumentNullException(nameof(order));

        _orders.Remove(order);
    }
}