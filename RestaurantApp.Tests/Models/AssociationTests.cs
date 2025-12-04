using System;
using NUnit.Framework;
using RestaurantApp.Models;

namespace RestaurantApp.Tests.Models;

[TestFixture]
public class AssociationTests
{
    // Mocking test objects
    private static NonMember CreateCustomer(string email = "test@example.com")
    {
        return new NonMember("Test", "User", new DateOnly(2000, 1, 1), "123456789", email);
    }

    private static Table CreateTable(int number = 1, int capacity = 4)
    {
        return new Table(number, capacity, "Standard");
    }

    private static Dish CreateDish(string name = "Pizza", decimal price = 20m)
    {
        return new Dish(name, "Italian", false, false, price, new[] { "Cheese", "Dough" });
    }

    private static Menu CreateMenu(string name = "Main Menu")
    {
        return new Menu(name, "Dinner", new[] { "EN", "PL" });
    }

    private static Restaurant CreateRestaurant(string name = "Test Restaurant")
    {
        return new Restaurant(name, 100);
    }

    private static WorkDetails CreateWorkDetails()
    {
        return new WorkDetails("Kitchen", "Day Shift", new DateOnly(2020, 1, 1));
    }

    private static EmployeeExperienceProfile CreateExperienceProfile()
    {
        return new ExperiencedProfile(5, "John Mentor");
    }

    // Customer ↔ Reservation Association (Bidirectional)
    [Test]
    public void Customer_MakeReservation_AddsToBothCustomerAndTableReservations()
    {
        var customer = CreateCustomer();
        var table = CreateTable();
        var reservation = new Reservation(Guid.NewGuid(), new DateOnly(2025, 6, 15), 4, table);

        table.Reserve(customer, reservation);

        Assert.Multiple(() =>
        {
            Assert.That(customer.Reservations, Contains.Item(reservation), 
                "Reservation should be in customer's reservations");
            Assert.That(table.Reservations, Contains.Item(reservation), 
                "Reservation should be in table's reservations");
            Assert.That(reservation.Table, Is.EqualTo(table), 
                "Reservation should reference the correct table");
            Assert.That(customer.Reservations.Count, Is.EqualTo(1));
            Assert.That(table.Reservations.Count, Is.EqualTo(1));
        });
    }

    // Customer ↔ Order Association (Bidirectional)
    [Test]
    public void Customer_PlaceOrder_CreatesOrderWithCorrectCustomerReference()
    {
        var customer = CreateCustomer();
        var table = CreateTable();
        var dish = CreateDish();
        var orderDishes = new[] { new OrderDish("Pizza", dish, 2) };

        var order = customer.PlaceOrder(table, orderDishes);

        Assert.Multiple(() =>
        {
            Assert.That(order.Customer, Is.EqualTo(customer), 
                "Order should reference the correct customer");
            Assert.That(order.Table, Is.EqualTo(table), 
                "Order should reference the correct table");
            Assert.That(order.Dishes.Count, Is.EqualTo(1), 
                "Order should contain the dishes");
        });
    }

    [Test]
    public void Order_Constructor_ThrowsWhenCustomerIsNull()
    {
        var table = CreateTable();

        Assert.Throws<ArgumentNullException>(() => new Order(null!, table));
    }

    [Test]
    public void Order_Constructor_ThrowsWhenTableIsNull()
    {
        var customer = CreateCustomer();

        Assert.Throws<ArgumentNullException>(() => new Order(customer, null!));
    }

    // Table ↔ Reservation Association (Bidirectional)
    [Test]
    public void Table_Reserve_AddsReservationToBothTableAndCustomer()
    {
        var customer = CreateCustomer();
        var table = CreateTable();
        var reservation = new Reservation(Guid.NewGuid(), new DateOnly(2025, 7, 20), 2, table);

        var result = table.Reserve(customer, reservation);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.True, "Reservation should succeed");
            Assert.That(table.Reservations, Contains.Item(reservation), 
                "Reservation should be in table's reservations");
            Assert.That(customer.Reservations, Contains.Item(reservation), 
                "Reservation should be in customer's reservations");
            Assert.That(reservation.Table, Is.EqualTo(table), 
                "Reservation should reference the correct table");
        });
    }

    [Test]
    public void Table_Reserve_ReturnsFalseWhenDateAlreadyReserved()
    {
        var customer = CreateCustomer();
        var table = CreateTable();
        var date = new DateOnly(2025, 8, 10);
        var reservation1 = new Reservation(Guid.NewGuid(), date, 2, table);
        var reservation2 = new Reservation(Guid.NewGuid(), date, 3, table);

        table.Reserve(customer, reservation1);
        var result = table.Reserve(customer, reservation2);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.False, "Duplicate reservation should return false");
            Assert.That(table.Reservations.Count, Is.EqualTo(1), 
                "Only one reservation should exist");
        });
    }

    [Test]
    public void Table_Reserve_ThrowsWhenCustomerIsNull()
    {
        var table = CreateTable();
        var reservation = new Reservation(Guid.NewGuid(), new DateOnly(2025, 6, 15), 2, table);

        Assert.Throws<ArgumentNullException>(() => table.Reserve(null!, reservation));
    }

    [Test]
    public void Table_Reserve_ThrowsWhenReservationIsNull()
    {
        var customer = CreateCustomer();
        var table = CreateTable();

        Assert.Throws<ArgumentNullException>(() => table.Reserve(customer, null!));
    }
}

