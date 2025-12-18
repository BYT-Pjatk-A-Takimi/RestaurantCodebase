using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace RestaurantApp.Models;

public class Order
{
    [JsonInclude]
    internal readonly List<OrderDish> _orderDishes = new();
    [JsonInclude]
    private readonly List<Payment> _payments = new();

    public Order(Person customer, Table table, Guid? id = null, DateTime? timeStamp = null, OrderStatus? status = null)
    {
        Customer = customer ?? throw new ArgumentNullException(nameof(customer));
        Table = table ?? throw new ArgumentNullException(nameof(table));

        Id = id ?? Guid.NewGuid();
        Status = status ?? OrderStatus.Pending;
        TimeStamp = timeStamp ?? DateTime.UtcNow;

        table.AddOrder(this);
    }

    [JsonConstructor]
    private Order()
    {
        _id = Guid.NewGuid();
        Status = OrderStatus.Pending;
        _timeStamp = DateTime.UtcNow;
    }

    // BASIC ATTRIBUTES
    private Guid _id;
    public Guid Id
    {
        get => _id;
        private set
        {
            if (value == Guid.Empty)
                throw new ArgumentException("Order id cannot be empty.", nameof(Id));
            _id = value;
        }
    }

    public Person Customer { get; private set; } = null!;

    public Table Table { get; private set; } = null!;

    internal void SetTable(Table table)
    {
        if (table is null)
            throw new ArgumentNullException(nameof(table));
        Table = table;
    }

    private DateTime _timeStamp;
    public DateTime TimeStamp
    {
        get => _timeStamp;
        private set
        {
            if (value == default)
                throw new ArgumentException("Timestamp must be a valid date.", nameof(TimeStamp));
            _timeStamp = value;
        }
    }

    public OrderStatus Status { get; private set; }

    // MULTI-VALUE (liste) – sistem için lazım
    public IReadOnlyCollection<OrderDish> Dishes => _orderDishes;

    public IReadOnlyCollection<Payment> Payments => _payments;

    // DERIVED ATTRIBUTE: /totalAmount
    public decimal TotalAmount => _orderDishes.Sum(d => d.TotalPrice);

    public void AddDish(OrderDish orderDish)
    {
        if (orderDish is null)
            throw new ArgumentNullException(nameof(orderDish));

        if (orderDish.Order != this)
            throw new ArgumentException("OrderDish belongs to a different order.", nameof(orderDish));

        if (_orderDishes.Contains(orderDish))
            throw new ArgumentException("This OrderDish is already in the order.", nameof(orderDish));

        _orderDishes.Add(orderDish);
    }

    public void AddDishes(IEnumerable<OrderDish> dishes)
    {
        if (dishes is null)
            throw new ArgumentNullException(nameof(dishes));

        foreach (var dish in dishes)
        {
            if (dish is null)
                throw new ArgumentException("Dish collection cannot contain null elements.", nameof(dishes));
        }

        _orderDishes.AddRange(dishes);
    }

    // Factory method to create OrderDish
    public OrderDish CreateOrderDish(string name, Dish dish, int quantity)
    {
        return new OrderDish(this, name, dish, quantity);
    }

    // Program.cs'deki eski çağrıları desteklemek için:
    public decimal CalculateTotal() => TotalAmount;

    public void CompleteOrder()
    {
        if (Status == OrderStatus.Completed)
            throw new InvalidOperationException("Order is already completed.");

        Status = OrderStatus.Completed;
    }

    // Qualified Association: Get Dish by Dish Name
    public Dish? GetDishByName(string dishName)
    {
        if (string.IsNullOrWhiteSpace(dishName))
            throw new ArgumentException("Dish name cannot be null or empty.", nameof(dishName));

        var orderDish = _orderDishes.FirstOrDefault(od => od.Dish.Name.Equals(dishName, StringComparison.OrdinalIgnoreCase));
        return orderDish?.Dish;
    }

    public bool RemoveDish(OrderDish orderDish)
    {
        if (orderDish is null)
            throw new ArgumentNullException(nameof(orderDish));

        return _orderDishes.Remove(orderDish);
    }

    internal void AddPayment(Payment payment)
    {
        if (payment is null)
            throw new ArgumentNullException(nameof(payment));

        if (payment.Order != this && payment.Order != null)
            throw new ArgumentException("Payment belongs to a different order.", nameof(payment));

        if (_payments.Contains(payment))
            return;

        _payments.Add(payment);
        payment.SetOrder(this);
    }

    public bool RemovePayment(Payment payment)
    {
        if (payment is null)
            throw new ArgumentNullException(nameof(payment));

        if (_payments.Count <= 1)
            throw new InvalidOperationException("Cannot remove the last payment. An order must have at least one payment.");

        return _payments.Remove(payment);
    }
}
