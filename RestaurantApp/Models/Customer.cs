using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace RestaurantApp.Models;

public abstract class Customer : Person
{
    protected Customer(
        string firstName,
        string lastName,
        DateOnly birthDate,
        string phoneNumber,
        string? email)
        : base(firstName, lastName, birthDate, phoneNumber)
    {
        Email = email;
        _reservations = new List<Reservation>();
    }

    [JsonInclude]
    private readonly List<Reservation> _reservations;

    public string? Email { get; }

    public IReadOnlyCollection<Reservation> Reservations => _reservations;

    public Restaurant ViewRestaurant(Restaurant restaurant) => restaurant;

    public Reservation MakeReservation(Reservation reservation)
    {
        _reservations.Add(reservation);
        return reservation;
    }

    public virtual Order PlaceOrder(Table table, IEnumerable<OrderDish> dishes)
    {
        var order = new Order(this, table);
        order.AddDishes(dishes);
        return order;
    }

    public Payment MakePayment(Order order, PaymentMethod method, decimal amount)
    {
        var payment = new Payment(order.Id, amount, method);
        payment.ProcessPayment();
        return payment;
    }

    public IReadOnlyCollection<Dish> ViewMenu(Menu menu) => menu.Dishes;
}

public sealed class Member : Customer
{
    private const int CreditsPerOrder = 1;

    public Member(
        string firstName,
        string lastName,
        DateOnly birthDate,
        string phoneNumber,
        string email,
        int credits,
        decimal creditPointsRate)
        : base(firstName, lastName, birthDate, phoneNumber, email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException("Member email cannot be null or empty.", nameof(email));
        }

        Credits = credits;
        CreditPointsRate = creditPointsRate;
    }

    public int Credits { get; private set; }

    public decimal CreditPointsRate { get; }

    public decimal UseCredits(decimal amount)
    {
        if (Credits <= 0)
        {
            return amount;
        }

        var discount = Credits * CreditPointsRate;
        Credits = 0;
        return amount - discount;
    }

    public void AddCredits() => Credits += CreditsPerOrder;
}

public sealed class NonMember : Customer
{
    public NonMember(
        string firstName,
        string lastName,
        DateOnly birthDate,
        string phoneNumber,
        string email)
        : base(firstName, lastName, birthDate, phoneNumber, email)
    {
    }

    public Member beMember(decimal initialCreditRate)
    {
        if (string.IsNullOrWhiteSpace(Email))
        {
            throw new InvalidOperationException("Cannot promote to member without an email address.");
        }

        return new Member(FirstName, LastName, BirthDate, PhoneNumber, Email, 0, initialCreditRate);
    }
}

