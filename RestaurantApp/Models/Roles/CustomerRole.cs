using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using RestaurantApp.Models.Roles.MembershipStatus;

namespace RestaurantApp.Models.Roles;

public class CustomerRole : PersonRole
{
    [JsonInclude]
    private readonly List<Reservation> _reservations;

    [JsonInclude]
    private string? _email;

    private MembershipStatusBase _membershipStatus = null!;

    [JsonConstructor]
    public CustomerRole(string? email, MembershipStatusBase membershipStatus)
    {
        _email = email;
        _reservations = new List<Reservation>();
        MembershipStatus = membershipStatus;
    }

    public string? Email
    {
        get => _email;
        private set
        {
            if (value != null && string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Email cannot be empty if provided.", nameof(Email));
            _email = value;
        }
    }

    public IReadOnlyCollection<Reservation> Reservations => _reservations;

    public MembershipStatusBase MembershipStatus
    {
        get => _membershipStatus;
        private set
        {
            if (value is null)
                throw new ArgumentNullException(nameof(MembershipStatus), "Membership status cannot be null.");
            if (_membershipStatus != null)
            {
                _membershipStatus.ClearCustomerRole();
            }
            _membershipStatus = value;
            _membershipStatus.SetCustomerRole(this);
        }
    }

    public Restaurant ViewRestaurant(Restaurant restaurant) => restaurant;

    public Reservation MakeReservation(Reservation reservation)
    {
        if (reservation is null)
            throw new ArgumentNullException(nameof(reservation));

        if (_reservations.Contains(reservation))
            throw new ArgumentException("This reservation already exists for this customer.", nameof(reservation));

        _reservations.Add(reservation);
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

        return _reservations.Remove(reservation);
    }

    public Order PlaceOrder(Table table, IEnumerable<(string name, Dish dish, int quantity)> dishItems)
    {
        if (table is null)
            throw new ArgumentNullException(nameof(table));

        if (dishItems is null)
            throw new ArgumentNullException(nameof(dishItems));

        if (Person is null)
            throw new InvalidOperationException("CustomerRole must be associated with a Person to place an order.");

        var order = new Order(Person, table);

        foreach (var item in dishItems)
        {
            if (item.dish is null)
                throw new ArgumentException("Dish cannot be null.", nameof(dishItems));

            new OrderDish(order, item.name, item.dish, item.quantity);
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

    public void SetMembershipStatus(MembershipStatusBase status)
    {
        MembershipStatus = status;
    }
}
