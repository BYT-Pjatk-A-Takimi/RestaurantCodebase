using System;
using System.Text.Json.Serialization;

namespace RestaurantApp.Models;

public class OrderDish
{
    [JsonConstructor]
    public OrderDish(Order order, string name, Dish dish, int quantity, bool skipAutoAdd = false)
    {
        if (order is null)
            throw new ArgumentNullException(nameof(order), "OrderDish cannot exist without an Order (composition).");

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty.", nameof(name));

        if (quantity <= 0)
            throw new ArgumentException("Quantity must be positive.", nameof(quantity));

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

    public string Name { get; }

    public Dish Dish { get; }

    public int Quantity { get; }

    // DERIVED (OrderDish seviyesinde de derived örneği)
    public decimal TotalPrice => Dish.Price * Quantity;
}
