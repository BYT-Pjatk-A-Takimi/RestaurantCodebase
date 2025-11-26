using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace RestaurantApp.Models;

public class Order
{
    [JsonInclude]
    private readonly List<OrderDish> _orderDishes = new();

    public Order(Customer customer, Table table)
    {
        Customer = customer ?? throw new ArgumentNullException(nameof(customer));
        Table = table ?? throw new ArgumentNullException(nameof(table));

        Id = Guid.NewGuid();
        Status = OrderStatus.Pending;
        TimeStamp = DateTime.UtcNow; // BASIC (timeStamp)
    }

    // BASIC ATTRIBUTES
    public Guid Id { get; }

    public Customer Customer { get; }

    public Table Table { get; }

    public DateTime TimeStamp { get; }   // BASIC: timeStamp

    public OrderStatus Status { get; private set; }  // BASIC: status

    // MULTI-VALUE (liste) – sistem için lazım
    public IReadOnlyCollection<OrderDish> Dishes => _orderDishes;

    // DERIVED ATTRIBUTE: /totalAmount
    public decimal TotalAmount => _orderDishes.Sum(d => d.TotalPrice);

    public void AddDish(OrderDish orderDish)
    {
        if (orderDish is null)
            throw new ArgumentNullException(nameof(orderDish));

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

    // Program.cs'deki eski çağrıları desteklemek için:
    public decimal CalculateTotal() => TotalAmount;

    public void CompleteOrder()
    {
        if (Status == OrderStatus.Completed)
            throw new InvalidOperationException("Order is already completed.");

        Status = OrderStatus.Completed;
    }
}

public class OrderDish
{
    public OrderDish(string name, Dish dish, int quantity)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty.", nameof(name));

        if (quantity <= 0)
            throw new ArgumentException("Quantity must be positive.", nameof(quantity));

        Dish = dish ?? throw new ArgumentNullException(nameof(dish));

        Name = name;
        Quantity = quantity;
    }

    public string Name { get; }

    public Dish Dish { get; }

    public int Quantity { get; }

    // DERIVED (OrderDish seviyesinde de derived örneği)
    public decimal TotalPrice => Dish.Price * Quantity;
}

public enum OrderStatus
{
    Pending,
    Completed
}