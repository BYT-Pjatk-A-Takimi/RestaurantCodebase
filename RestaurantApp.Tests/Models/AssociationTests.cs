using System;
using NUnit.Framework;
using RestaurantApp.Models;
using RestaurantApp.Models.Roles;
using RestaurantApp.Models.Roles.EmployeeTypes;
using RestaurantApp.Models.Roles.MembershipStatus;

namespace RestaurantApp.Tests.Models;

[TestFixture]
public class AssociationTests
{
    // setup methods
    private static Person CreateCustomer(string email = "test@example.com")
    {
        var nonMemberStatus = new NonMemberStatus();
        var customerRole = new CustomerRole(email, nonMemberStatus);
        return new Person("Test", "User", new DateOnly(2000, 1, 1), "123456789", customerRole: customerRole);
    }

    private static Table CreateTable(int number = 1, int capacity = 4, Restaurant? restaurant = null)
    {
        restaurant ??= CreateRestaurant();
        return new Table(number, capacity, "Standard", restaurant);
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

    private static ManagerType CreateManagerType(int level = 1)
    {
        return new ManagerType(level);
    }

    private static ChefType CreateChefType()
    {
        return new ChefType("Italian");
    }

    private static WaiterType CreateWaiterType()
    {
        return new WaiterType();
    }

    // Customer <-> Reservation Association Validations

    [Test]
    public void CustomerRole_MakeReservation_ThrowsWhenReservationIsNull()
    {
        var customer = CreateCustomer();
        Assert.Throws<ArgumentNullException>(() => customer.CustomerRole!.MakeReservation(null!));
    }

    [Test]
    public void CustomerRole_MakeReservation_ThrowsWhenDuplicate()
    {
        var customer = CreateCustomer();
        var table = CreateTable();
        var reservation = new Reservation(Guid.NewGuid(), new DateOnly(2025, 6, 15), new TimeOnly(19, 0), 2, table);
        table.Reserve(customer, reservation);
        Assert.Throws<ArgumentException>(() => customer.CustomerRole!.MakeReservation(reservation));
    }

    [Test]
    public void Table_Reserve_ThrowsWhenCustomerIsNull()
    {
        var table = CreateTable();
        var reservation = new Reservation(Guid.NewGuid(), new DateOnly(2025, 6, 15), new TimeOnly(19, 0), 2, table);
        Assert.Throws<ArgumentNullException>(() => table.Reserve(null!, reservation));
    }

    [Test]
    public void Table_Reserve_ThrowsWhenReservationIsNull()
    {
        var customer = CreateCustomer();
        var table = CreateTable();
        Assert.Throws<ArgumentNullException>(() => table.Reserve(customer, null!));
    }

    [Test]
    public void Table_Reserve_ReturnsFalseWhenDuplicateDateTime()
    {
        var customer = CreateCustomer();
        var table = CreateTable();
        var date = new DateOnly(2025, 8, 10);
        var time = new TimeOnly(19, 0);
        var reservation1 = new Reservation(Guid.NewGuid(), date, time, 2, table);
        var reservation2 = new Reservation(Guid.NewGuid(), date, time, 3, table);
        table.Reserve(customer, reservation1);
        var result = table.Reserve(customer, reservation2);
        Assert.That(result, Is.False);
        Assert.That(table.Reservations.Count, Is.EqualTo(1));
    }

    // Customer <-> Order Association Validations

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

    // Order <-> OrderDish Association Validations

    [Test]
    public void Order_AddDish_ThrowsWhenOrderDishIsNull()
    {
        var customer = CreateCustomer();
        var table = CreateTable();
        var order = new Order(customer, table);
        Assert.Throws<ArgumentNullException>(() => order.AddDish(null!));
    }

    [Test]
    public void Order_AddDish_ThrowsWhenOrderDishFromDifferentOrder()
    {
        var customer = CreateCustomer();
        var table = CreateTable();
        var order1 = new Order(customer, table);
        var order2 = new Order(customer, table);
        var dish = CreateDish();
        var orderDish = new OrderDish(order1, "Pizza", dish, 1, skipAutoAdd: true);
        Assert.Throws<ArgumentException>(() => order2.AddDish(orderDish));
    }

    // OrderDish Validations

    [Test]
    public void OrderDish_Constructor_ThrowsWhenOrderIsNull()
    {
        var dish = CreateDish();
        Assert.Throws<ArgumentNullException>(() => new OrderDish(null!, "Pizza", dish, 2));
    }

    [Test]
    public void OrderDish_Constructor_ThrowsWhenDishIsNull()
    {
        var customer = CreateCustomer();
        var table = CreateTable();
        var order = new Order(customer, table);
        Assert.Throws<ArgumentNullException>(() => new OrderDish(order, "Pizza", null!, 1));
    }

    [Test]
    public void OrderDish_Constructor_ThrowsWhenQuantityIsNotPositive()
    {
        var customer = CreateCustomer();
        var table = CreateTable();
        var order = new Order(customer, table);
        var dish = CreateDish();
        Assert.Throws<ArgumentException>(() => new OrderDish(order, "Pizza", dish, 0));
        Assert.Throws<ArgumentException>(() => new OrderDish(order, "Pizza", dish, -1));
    }

    // Order <-> Payment Association Validations

    [Test]
    public void Payment_Constructor_ThrowsWhenOrderIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => new Payment(null!, 50m, PaymentMethod.Card));
    }

    [Test]
    public void Payment_Constructor_ThrowsWhenAmountIsNotPositive()
    {
        var customer = CreateCustomer();
        var table = CreateTable();
        var order = new Order(customer, table);
        Assert.Throws<ArgumentException>(() => new Payment(order, 0m, PaymentMethod.Card));
        Assert.Throws<ArgumentException>(() => new Payment(order, -10m, PaymentMethod.Card));
    }

    [Test]
    public void Order_RemovePayment_ThrowsWhenLastPayment()
    {
        var customer = CreateCustomer();
        var table = CreateTable();
        var order = new Order(customer, table);
        var payment = new Payment(order, 100m, PaymentMethod.Card);
        Assert.Throws<InvalidOperationException>(() => order.RemovePayment(payment));
    }

    // ManagerType Reflex Association Validations

    [Test]
    public void ManagerType_SetSupervisor_ThrowsWhenSelf()
    {
        var manager = CreateManagerType();
        Assert.Throws<ArgumentException>(() => manager.SetSupervisor(manager));
    }

    [Test]
    public void ManagerType_SetSupervisor_ThrowsWhenCircularReference()
    {
        var manager1 = CreateManagerType(1);
        var manager2 = CreateManagerType(2);
        var manager3 = CreateManagerType(3);
        manager1.AddSubordinate(manager2);
        manager2.AddSubordinate(manager3);
        Assert.Throws<InvalidOperationException>(() => manager3.SetSupervisor(manager1));
    }

    // ManagerType <-> Restaurant Association Validations

    [Test]
    public void ManagerType_SetRestaurant_ThrowsWhenAlreadyAssigned()
    {
        var manager = CreateManagerType();
        var restaurant1 = CreateRestaurant("Restaurant 1");
        var restaurant2 = CreateRestaurant("Restaurant 2");
        manager.SetRestaurant(restaurant1);
        Assert.Throws<InvalidOperationException>(() => manager.SetRestaurant(restaurant2));
    }

    // ChefType <-> Restaurant Association Validations

    [Test]
    public void ChefType_SetRestaurant_ThrowsWhenAlreadyAssigned()
    {
        var chef = CreateChefType();
        var restaurant1 = CreateRestaurant("Restaurant 1");
        var restaurant2 = CreateRestaurant("Restaurant 2");
        chef.SetRestaurant(restaurant1);
        Assert.Throws<InvalidOperationException>(() => chef.SetRestaurant(restaurant2));
    }

    // Restaurant <-> Table Association Validations

    [Test]
    public void Restaurant_AddTable_ThrowsWhenDuplicateTableNumber()
    {
        var restaurant = CreateRestaurant();
        var table1 = CreateTable(1, 4, restaurant);
        var table2 = new Table(1, 6, "Standard", restaurant, skipAutoAdd: true);
        Assert.Throws<ArgumentException>(() => restaurant.AddTable(table2));
    }

    // Restaurant <-> Menu Association Validations

    [Test]
    public void Restaurant_AddMenu_ThrowsWhenDuplicateMenuName()
    {
        var restaurant = CreateRestaurant();
        var menu1 = CreateMenu("Main Menu");
        var menu2 = CreateMenu("Main Menu");
        restaurant.AddMenu(menu1);
        Assert.Throws<ArgumentException>(() => restaurant.AddMenu(menu2));
    }

    [Test]
    public void Restaurant_RemoveMenu_ThrowsWhenLastMenu()
    {
        var restaurant = CreateRestaurant();
        var menu = CreateMenu();
        restaurant.AddMenu(menu);
        Assert.Throws<InvalidOperationException>(() => restaurant.RemoveMenu(menu));
    }

    // Menu <-> Dish Association Validations

    [Test]
    public void Menu_AddDish_ThrowsWhenDishIsNull()
    {
        var menu = CreateMenu();
        Assert.Throws<ArgumentException>(() => menu.AddDish(null!));
    }

    [Test]
    public void Menu_RemoveDish_ThrowsWhenLastDish()
    {
        var menu = CreateMenu();
        var dish = CreateDish();
        menu.AddDish(dish);
        Assert.Throws<InvalidOperationException>(() => menu.RemoveDish(dish));
    }

    // WaiterType <-> Table Association Validations

    [Test]
    public void WaiterType_AssignTable_ThrowsWhenTableIsNull()
    {
        var waiter = CreateWaiterType();
        Assert.Throws<ArgumentNullException>(() => waiter.AssignTable(null!));
    }
}
