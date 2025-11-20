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
        Id = Guid.NewGuid();
        Customer = customer;
        Table = table;
        Status = OrderStatus.Pending;
    }

    public Guid Id { get; }

    public Customer Customer { get; }

    public Table Table { get; }

    public OrderStatus Status { get; private set; }

    public IReadOnlyCollection<OrderDish> Dishes => _orderDishes;

    public void AddDish(OrderDish orderDish) => _orderDishes.Add(orderDish);

    public void AddDishes(IEnumerable<OrderDish> dishes) => _orderDishes.AddRange(dishes);

    public decimal CalculateTotal() => _orderDishes.Sum(d => d.TotalPrice);

    public void CompleteOrder() => Status = OrderStatus.Completed;
}

public class OrderDish
{
    public OrderDish(string name, Dish dish, int quantity)
    {
        Name = name;
        Dish = dish;
        Quantity = quantity;
    }

    public string Name { get; }

    public Dish Dish { get; }

    public int Quantity { get; }

    public decimal TotalPrice => Dish.Price * Quantity;
}

public enum OrderStatus
{
    Pending,
    Completed
}

