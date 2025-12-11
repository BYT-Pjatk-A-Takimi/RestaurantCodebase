using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace RestaurantApp.Models;

[JsonPolymorphic]
[JsonDerivedType(typeof(Member), typeDiscriminator: "Member")]
[JsonDerivedType(typeof(NonMember), typeDiscriminator: "NonMember")]
public abstract class Customer : Person
{
    [JsonConstructor]
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
        if (reservation is null)
            throw new ArgumentNullException(nameof(reservation));

        if (_reservations.Contains(reservation))
            throw new ArgumentException("This reservation already exists for this customer.", nameof(reservation));

        _reservations.Add(reservation);
        reservation.SetCustomer(this);
        return reservation;
    }

    public Reservation? GetReservation(DateOnly dateOfReservation)
    {
        return _reservations.FirstOrDefault(r => r.DateOfReservation == dateOfReservation);
    }

    public bool RemoveReservation(Reservation reservation)
    {
        if (reservation is null)
            throw new ArgumentNullException(nameof(reservation));

        var removed = _reservations.Remove(reservation);
        if (removed && reservation.Customer == this)
        {
            reservation.SetCustomer(null);
        }
        return removed;
    }

    public virtual Order PlaceOrder(Table table, IEnumerable<(string name, Dish dish, int quantity)> dishItems)
    {
        if (table is null)
            throw new ArgumentNullException(nameof(table));

        if (dishItems is null)
            throw new ArgumentNullException(nameof(dishItems));

        var order = new Order(this, table);
        
        foreach (var item in dishItems)
        {
            if (item.dish is null)
                throw new ArgumentException("Dish cannot be null.", nameof(dishItems));

            new OrderDish(order, item.name, item.dish, item.quantity);
        }

        return order;
    }

    public virtual Order PlaceOrder(Table table, IEnumerable<OrderDish> dishes)
    {
        if (table is null)
            throw new ArgumentNullException(nameof(table));

        if (dishes is null)
            throw new ArgumentNullException(nameof(dishes));

        var order = new Order(this, table);
        
        foreach (var dish in dishes)
        {
            if (dish is null)
                throw new ArgumentException("OrderDish cannot be null.", nameof(dishes));

            if (dish.Order != order)
                throw new ArgumentException("OrderDish belongs to a different order.", nameof(dishes));

            order.AddDish(dish);
        }

        return order;
    }

    public Payment MakePayment(Order order, PaymentMethod method, decimal amount)
    {
        if (order is null)
            throw new ArgumentNullException(nameof(order));

        var payment = new Payment(order, amount, method);
        payment.ProcessPayment();
        return payment;
    }

    public IReadOnlyCollection<Dish> ViewMenu(Menu menu) => menu.Dishes;
}

public sealed class Member : Customer
{
    private const int CreditsPerOrder = 1;

    [JsonConstructor]
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
    [JsonConstructor]
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

