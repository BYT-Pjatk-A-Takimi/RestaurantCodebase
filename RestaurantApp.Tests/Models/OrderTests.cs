using System;
using NUnit.Framework;
using RestaurantApp.Models;
using RestaurantApp.Models.Roles;
using RestaurantApp.Models.Roles.MembershipStatus;

namespace RestaurantApp.Tests.Models;

[TestFixture]
public class OrderTests
{
    private static Person CreateCustomer()
    {
        var nonMemberStatus = new NonMemberStatus();
        var customerRole = new CustomerRole("test@example.com", nonMemberStatus);
        return new Person("Test", "User", new DateOnly(2000, 1, 1), "123456789", customerRole: customerRole);
    }

    private static Restaurant CreateRestaurant()
    {
        return new Restaurant("Test Restaurant", 100);
    }

    [Test]
    public void Constructor_ShouldSetInitialStatusAndTimestamp()
    {
        var customer = CreateCustomer();
        var restaurant = CreateRestaurant();
        var table = new Table(1, 4, "Standard", restaurant);

        var before = DateTime.UtcNow.AddSeconds(-1);
        var order = new Order(customer, table);
        var after = DateTime.UtcNow.AddSeconds(1);

        Assert.Multiple(() =>
        {
            Assert.That(order.Status, Is.EqualTo(OrderStatus.Pending));
            Assert.That(order.TimeStamp, Is.InRange(before, after));
        });
    }

    [Test]
    public void AddDish_ShouldIncreaseDishCount()
    {
        var customer = CreateCustomer();
        var restaurant = CreateRestaurant();
        var table = new Table(1, 4, "Standard", restaurant);
        var order = new Order(customer, table);

        var dish = new Dish("Pizza", "Italian", false, false, 20m, new[] { "Cheese" });
        var orderDish = new OrderDish(order, "Pizza", dish, 2);
        // OrderDish constructor automatically adds to order, so count should be 1

        Assert.That(order.Dishes.Count, Is.EqualTo(1));
    }

    [Test]
    public void AddDishes_ShouldIncreaseDishCount_ByCollectionSize()
    {
        var customer = CreateCustomer();
        var restaurant = CreateRestaurant();
        var table = new Table(1, 4, "Standard", restaurant);
        var order = new Order(customer, table);

        var dish = new Dish("Burger", "FastFood", false, false, 15m, new[] { "Bun" });

        // OrderDish constructor automatically adds to order
        var dish1 = new OrderDish(order, "Burger", dish, 1);
        var dish2 = new OrderDish(order, "Burger", dish, 2);

        Assert.That(order.Dishes.Count, Is.EqualTo(2));
    }

    [Test]
    public void CalculateTotal_ShouldReturnSumOfOrderDishPrices()
    {
        var customer = CreateCustomer();
        var restaurant = CreateRestaurant();
        var table = new Table(1, 4, "Standard", restaurant);
        var order = new Order(customer, table);

        var dish = new Dish("Burger", "FastFood", false, false, 15m, new[] { "Bun" });
        var orderDish = new OrderDish(order, "Burger", dish, 3); // 3 * 15 = 45
        // OrderDish constructor automatically adds to order

        Assert.That(order.CalculateTotal(), Is.EqualTo(45m));
        Assert.That(order.TotalAmount, Is.EqualTo(45m));
    }

    [Test]
    public void CompleteOrder_ShouldChangeStatusToCompleted()
    {
        var customer = CreateCustomer();
        var restaurant = CreateRestaurant();
        var table = new Table(1, 4, "Standard", restaurant);
        var order = new Order(customer, table);

        order.CompleteOrder();

        Assert.That(order.Status, Is.EqualTo(OrderStatus.Completed));
    }

    [Test]
    public void OrderDish_ShouldValidateInputs()
    {
        var customer = CreateCustomer();
        var restaurant = CreateRestaurant();
        var table = new Table(1, 4, "Standard", restaurant);
        var order = new Order(customer, table);
        var dish = new Dish("Soup", "Starter", true, false, 10m, new[] { "Water" });

        Assert.Throws<ArgumentException>(() => new OrderDish(order, "", dish, 1));
        Assert.Throws<ArgumentException>(() => new OrderDish(order, "Soup", dish, 0));
        Assert.Throws<ArgumentNullException>(() => new OrderDish(order, "Soup", null!, 1));
        Assert.Throws<ArgumentNullException>(() => new OrderDish(null!, "Soup", dish, 1));
    }

    [Test]
    public void CompleteOrder_WhenAlreadyCompleted_ShouldThrowInvalidOperationException()
    {
        var customer = CreateCustomer();
        var restaurant = CreateRestaurant();
        var table = new Table(1, 4, "Standard", restaurant);
        var order = new Order(customer, table);

        order.CompleteOrder();

        Assert.Throws<InvalidOperationException>(() => order.CompleteOrder());
    }

    [Test]
    public void Constructor_WithNullCustomer_ShouldThrowArgumentNullException()
    {
        var restaurant = CreateRestaurant();
        var table = new Table(1, 4, "Standard", restaurant);

        Assert.Throws<ArgumentNullException>(() => new Order(null!, table));
    }

    [Test]
    public void Constructor_WithNullTable_ShouldThrowArgumentNullException()
    {
        var customer = CreateCustomer();

        Assert.Throws<ArgumentNullException>(() => new Order(customer, null!));
    }

    [Test]
    public void RemoveDish_ShouldDecreaseDishCount()
    {
        var customer = CreateCustomer();
        var restaurant = CreateRestaurant();
        var table = new Table(1, 4, "Standard", restaurant);
        var order = new Order(customer, table);

        var dish = new Dish("Pizza", "Italian", false, false, 20m, new[] { "Cheese" });
        var orderDish = new OrderDish(order, "Pizza", dish, 2);

        Assert.That(order.Dishes.Count, Is.EqualTo(1));

        var removed = order.RemoveDish(orderDish);

        Assert.Multiple(() =>
        {
            Assert.That(removed, Is.True);
            Assert.That(order.Dishes.Count, Is.EqualTo(0));
        });
    }

    [Test]
    public void GetDishByName_ShouldReturnDish_WhenExists()
    {
        var customer = CreateCustomer();
        var restaurant = CreateRestaurant();
        var table = new Table(1, 4, "Standard", restaurant);
        var order = new Order(customer, table);

        var dish = new Dish("Margherita Pizza", "Italian", false, false, 20m, new[] { "Cheese" });
        var orderDish = new OrderDish(order, "Margherita", dish, 1);

        var result = order.GetDishByName("Margherita Pizza");

        Assert.That(result, Is.EqualTo(dish));
    }

    [Test]
    public void GetDishByName_ShouldReturnNull_WhenNotExists()
    {
        var customer = CreateCustomer();
        var restaurant = CreateRestaurant();
        var table = new Table(1, 4, "Standard", restaurant);
        var order = new Order(customer, table);

        var dish = new Dish("Pizza", "Italian", false, false, 20m, new[] { "Cheese" });
        var orderDish = new OrderDish(order, "Pizza", dish, 1);

        var result = order.GetDishByName("NonExistent Dish");

        Assert.That(result, Is.Null);
    }

    [Test]
    public void GetDishByName_WithEmptyName_ShouldThrowArgumentException()
    {
        var customer = CreateCustomer();
        var restaurant = CreateRestaurant();
        var table = new Table(1, 4, "Standard", restaurant);
        var order = new Order(customer, table);

        Assert.Throws<ArgumentException>(() => order.GetDishByName(""));
        Assert.Throws<ArgumentException>(() => order.GetDishByName("   "));
    }
}
