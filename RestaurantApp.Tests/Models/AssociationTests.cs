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


    //Table ↔ Order Association
    [Test]
    public void Order_ReferencesTable_TableCanAccessOrders()
    {
        var customer = CreateCustomer();
        var table = CreateTable();
        var dish = CreateDish();
        var orderDishes = new[] { new OrderDish("Pizza", dish, 1) };

        var order = customer.PlaceOrder(table, orderDishes);

        Assert.Multiple(() =>
        {
            Assert.That(order.Table, Is.EqualTo(table),
                "Order should reference the correct table");
            Assert.That(order.Table.TableNumber, Is.EqualTo(table.TableNumber),
                "Order's table should have correct table number");
        });
    }

    //Restaurant ↔ Table Association
    [Test]
    public void Restaurant_AddTable_TableBelongsToRestaurant()
    {
        var restaurant = CreateRestaurant();
        var table = CreateTable(1, 4);

        restaurant.AddTable(table);

        Assert.Multiple(() =>
        {
            Assert.That(restaurant.Tables, Contains.Item(table),
                "Table should be in restaurant's tables");
            Assert.That(restaurant.Tables.Count, Is.EqualTo(1),
                "Restaurant should have one table");
            Assert.That(restaurant.GetNumberOfTables(), Is.EqualTo(1),
                "GetNumberOfTables should return correct count");
        });
    }

    [Test]
    public void Restaurant_AddTable_ThrowsWhenTableIsNull()
    {
        var restaurant = CreateRestaurant();

        Assert.Throws<ArgumentNullException>(() => restaurant.AddTable(null!));
    }

    [Test]
    public void Restaurant_AddTable_ThrowsWhenDuplicateTableNumber()
    {
        var restaurant = CreateRestaurant();
        var table1 = CreateTable(1, 4);
        var table2 = CreateTable(1, 6); // Same table number

        restaurant.AddTable(table1);

        Assert.Throws<ArgumentException>(() => restaurant.AddTable(table2),
            "Adding duplicate table number should throw");
    }

    [Test]
    public void Restaurant_RemoveTable_RemovesTableFromRestaurant()
    {
        var restaurant = CreateRestaurant();
        var table = CreateTable();
        restaurant.AddTable(table);

        restaurant.RemoveTable(table);

        Assert.That(restaurant.Tables, Does.Not.Contain(table),
            "Table should be removed from restaurant");
        Assert.That(restaurant.Tables.Count, Is.EqualTo(0));
    }

    //Restaurant ↔ Menu Association
    [Test]
    public void Restaurant_AddMenu_MenuBelongsToRestaurant()
    {
        var restaurant = CreateRestaurant();
        var menu = CreateMenu("Lunch Menu");

        restaurant.AddMenu(menu);

        Assert.Multiple(() =>
        {
            Assert.That(restaurant.Menus, Contains.Item(menu),
                "Menu should be in restaurant's menus");
            Assert.That(restaurant.Menus.Count, Is.EqualTo(1),
                "Restaurant should have one menu");
        });
    }

    [Test]
    public void Restaurant_AddMenu_ThrowsWhenMenuIsNull()
    {
        var restaurant = CreateRestaurant();

        Assert.Throws<ArgumentNullException>(() => restaurant.AddMenu(null!));
    }

    [Test]
    public void Restaurant_AddMenu_ThrowsWhenDuplicateMenuName()
    {
        var restaurant = CreateRestaurant();
        var menu1 = CreateMenu("Main Menu");
        var menu2 = CreateMenu("Main Menu"); // Same name

        restaurant.AddMenu(menu1);

        Assert.Throws<ArgumentException>(() => restaurant.AddMenu(menu2),
            "Adding duplicate menu name should throw");
    }

    [Test]
    public void Restaurant_RemoveMenu_RemovesMenuFromRestaurant()
    {
        var restaurant = CreateRestaurant();
        var menu = CreateMenu();
        restaurant.AddMenu(menu);

        var result = restaurant.RemoveMenu(menu);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.True, "RemoveMenu should return true");
            Assert.That(restaurant.Menus, Does.Not.Contain(menu),
                "Menu should be removed from restaurant");
        });
    }

    //Menu ↔ Dish Association 
    [Test]
    public void Menu_AddDish_DishIsInMenuCollection()
    {
        var menu = CreateMenu();
        var dish = CreateDish("Pasta", 25m);

        menu.AddDish(dish);

        Assert.Multiple(() =>
        {
            Assert.That(menu.Dishes, Contains.Item(dish),
                "Dish should be in menu's dishes");
            Assert.That(menu.Dishes.Count, Is.EqualTo(1),
                "Menu should have one dish");
            Assert.That(menu.GetNumberOfPositions(), Is.EqualTo(1),
                "GetNumberOfPositions should return correct count");
        });
    }

    [Test]
    public void Menu_AddDish_ThrowsWhenDishIsNull()
    {
        var menu = CreateMenu();

        Assert.Throws<ArgumentException>(() => menu.AddDish(null!));
    }

    [Test]
    public void Menu_RemoveDish_RemovesDishFromMenu()
    {
        var menu = CreateMenu();
        var dish = CreateDish();
        menu.AddDish(dish);

        var result = menu.RemoveDish(dish);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.True, "RemoveDish should return true");
            Assert.That(menu.Dishes, Does.Not.Contain(dish),
                "Dish should be removed from menu");
            Assert.That(menu.Dishes.Count, Is.EqualTo(0));
        });
    }
}

