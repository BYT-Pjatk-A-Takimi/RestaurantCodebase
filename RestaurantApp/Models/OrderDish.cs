using System;
using System.Text.Json.Serialization;

namespace RestaurantApp.Models;

public class OrderDish
{
    private string _name = string.Empty;
    private int _quantity;

    [JsonConstructor]
    public OrderDish(Order order, string name, Dish dish, int quantity, bool skipAutoAdd = false)
    {
        if (order is null)
            throw new ArgumentNullException(nameof(order), "OrderDish cannot exist without an Order (composition).");

        Dish = dish ?? throw new ArgumentNullException(nameof(dish));

        Order = order;
        Name = name;
        Quantity = quantity;

        if (!skipAutoAdd)
        {
            // Add to order's dish collection (composition)
            if (!order._orderDishes.Contains(this))
            {
                order._orderDishes.Add(this);
            }

            // Add to dish's orderDish collection (reverse connection)
            dish.AddOrderDish(this);
        }
    }

    public Order Order { get; }

    public string Name
    {
        get => _name;
        private set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Name cannot be empty.", nameof(Name));
            _name = value;
        }
    }

    public Dish Dish { get; }

    public int Quantity
    {
        get => _quantity;
        private set
        {
            if (value <= 0)
                throw new ArgumentException("Quantity must be positive.", nameof(Quantity));
            _quantity = value;
        }
    }

    // DERIVED (OrderDish seviyesinde de derived örneği)
    public decimal TotalPrice => Dish.Price * Quantity;
}
