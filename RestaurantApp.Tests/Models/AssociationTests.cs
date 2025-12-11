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

    private static Manager CreateManager(string firstName = "John", int level = 1)
    {
        var workDetails = CreateWorkDetails();
        var experienceProfile = CreateExperienceProfile();
        return new Manager(firstName, "Doe", new DateOnly(1980, 1, 1), "555-0001", workDetails, experienceProfile, level);
    }

    private static Chef CreateChef(string firstName = "Chef")
    {
        var workDetails = CreateWorkDetails();
        var experienceProfile = CreateExperienceProfile();
        return new Chef(firstName, "Cook", new DateOnly(1985, 1, 1), "555-0002", workDetails, experienceProfile, "Italian");
    }

    private static Waiter CreateWaiter()
    {
        var workDetails = CreateWorkDetails();
        var experienceProfile = CreateExperienceProfile();
        return new Waiter("Waiter", "Service", new DateOnly(1990, 1, 1), "555-0003", workDetails, experienceProfile);
    }

    // Customer ↔ Reservation Association (Bidirectional)
    [Test]
    public void Customer_MakeReservation_AddsToBothCustomerAndTableReservations()
    {
        var customer = CreateCustomer();
        var table = CreateTable();
        var reservation = new Reservation(Guid.NewGuid(), new DateOnly(2025, 6, 15), new TimeOnly(19, 0), 4, table);

        table.Reserve(customer, reservation);

        Assert.Multiple(() =>
        {
            Assert.That(customer.Reservations, Contains.Item(reservation),
                "Reservation should be in customer's reservations");
            Assert.That(table.Reservations, Contains.Item(reservation),
                "Reservation should be in table's reservations");
            Assert.That(reservation.Table, Is.EqualTo(table),
                "Reservation should reference the correct table");
            Assert.That(reservation.Customer, Is.EqualTo(customer),
                "Reservation should reference the correct customer");
            Assert.That(customer.Reservations.Count, Is.EqualTo(1));
            Assert.That(table.Reservations.Count, Is.EqualTo(1));
        });
    }

    [Test]
    public void Customer_MakeReservation_EstablishesBidirectionalRelationship()
    {
        var customer = CreateCustomer();
        var table = CreateTable();
        var reservation = new Reservation(Guid.NewGuid(), new DateOnly(2025, 6, 15), new TimeOnly(19, 0), 4, table);

        customer.MakeReservation(reservation);

        Assert.Multiple(() =>
        {
            Assert.That(customer.Reservations, Contains.Item(reservation));
            Assert.That(reservation.Customer, Is.EqualTo(customer));
        });
    }

    [Test]
    public void Customer_RemoveReservation_RemovesBidirectionalRelationship()
    {
        var customer = CreateCustomer();
        var table = CreateTable();
        var reservation = new Reservation(Guid.NewGuid(), new DateOnly(2025, 6, 15), new TimeOnly(19, 0), 4, table);

        customer.MakeReservation(reservation);
        var removed = customer.RemoveReservation(reservation);

        Assert.Multiple(() =>
        {
            Assert.That(removed, Is.True);
            Assert.That(customer.Reservations, Does.Not.Contain(reservation));
            Assert.That(reservation.Customer, Is.Null);
        });
    }

    // Customer ↔ Order Association (Bidirectional)
    [Test]
    public void Customer_PlaceOrder_CreatesOrderWithCorrectCustomerReference()
    {
        var customer = CreateCustomer();
        var table = CreateTable();
        var dish = CreateDish();

        var order = customer.PlaceOrder(table, new[] { ("Pizza Order", dish, 2) });

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
        var reservation = new Reservation(Guid.NewGuid(), new DateOnly(2025, 7, 20), new TimeOnly(20, 0), 2, table);

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
    public void Table_Reserve_ReturnsFalseWhenDateAndTimeAlreadyReserved()
    {
        var customer = CreateCustomer();
        var table = CreateTable();
        var date = new DateOnly(2025, 8, 10);
        var time = new TimeOnly(19, 0);
        var reservation1 = new Reservation(Guid.NewGuid(), date, time, 2, table);
        var reservation2 = new Reservation(Guid.NewGuid(), date, time, 3, table); // Same date AND time

        table.Reserve(customer, reservation1);
        var result = table.Reserve(customer, reservation2);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.False, "Duplicate reservation (same date and time) should return false");
            Assert.That(table.Reservations.Count, Is.EqualTo(1),
                "Only one reservation should exist");
        });
    }

    [Test]
    public void Table_Reserve_AllowsMultipleReservationsOnSameDateWithDifferentTimes()
    {
        var customer = CreateCustomer();
        var table = CreateTable();
        var date = new DateOnly(2025, 8, 10);
        var reservation1 = new Reservation(Guid.NewGuid(), date, new TimeOnly(18, 0), 2, table);
        var reservation2 = new Reservation(Guid.NewGuid(), date, new TimeOnly(20, 0), 3, table);

        var result1 = table.Reserve(customer, reservation1);
        var result2 = table.Reserve(customer, reservation2);

        Assert.Multiple(() =>
        {
            Assert.That(result1, Is.True, "First reservation should succeed");
            Assert.That(result2, Is.True, "Second reservation with different time should succeed");
            Assert.That(table.Reservations.Count, Is.EqualTo(2),
                "Both reservations should exist");
            Assert.That(table.Reservations, Contains.Item(reservation1));
            Assert.That(table.Reservations, Contains.Item(reservation2));
        });
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

    // Manager Reflex Association Tests
    [Test]
    public void Manager_SetSupervisor_EstablishesBidirectionalRelationship()
    {
        var manager1 = CreateManager("Manager1", 1);
        var manager2 = CreateManager("Manager2", 2);

        manager2.SetSupervisor(manager1);

        Assert.Multiple(() =>
        {
            Assert.That(manager2.Supervisor, Is.EqualTo(manager1), "Manager2 should have Manager1 as supervisor");
            Assert.That(manager1.Subordinates, Contains.Item(manager2), "Manager1 should have Manager2 as subordinate");
        });
    }

    [Test]
    public void Manager_SetSupervisor_ThrowsWhenSelfSupervisor()
    {
        var manager = CreateManager();

        Assert.Throws<ArgumentException>(() => manager.SetSupervisor(manager), "Manager cannot supervise themselves");
    }

    [Test]
    public void Manager_AddSubordinate_EstablishesBidirectionalRelationship()
    {
        var manager1 = CreateManager("Manager1", 1);
        var manager2 = CreateManager("Manager2", 2);

        manager1.AddSubordinate(manager2);

        Assert.Multiple(() =>
        {
            Assert.That(manager1.Subordinates, Contains.Item(manager2));
            Assert.That(manager2.Supervisor, Is.EqualTo(manager1));
        });
    }

    [Test]
    public void Manager_RemoveSubordinate_RemovesBidirectionalRelationship()
    {
        var manager1 = CreateManager("Manager1", 1);
        var manager2 = CreateManager("Manager2", 2);

        manager1.AddSubordinate(manager2);
        var removed = manager1.RemoveSubordinate(manager2);

        Assert.Multiple(() =>
        {
            Assert.That(removed, Is.True);
            Assert.That(manager1.Subordinates, Does.Not.Contain(manager2));
            Assert.That(manager2.Supervisor, Is.Null);
        });
    }

    [Test]
    public void Manager_SetSupervisor_ThrowsWhenCircularReference()
    {
        var manager1 = CreateManager("Manager1", 1);
        var manager2 = CreateManager("Manager2", 2);
        var manager3 = CreateManager("Manager3", 3);

        manager1.AddSubordinate(manager2);
        manager2.AddSubordinate(manager3);

        Assert.Throws<InvalidOperationException>(() => manager3.SetSupervisor(manager1), "Should prevent circular reference");
    }

    // Manager ↔ Restaurant Association Tests (0..1 multiplicity)
    [Test]
    public void Manager_SetRestaurant_EstablishesBidirectionalRelationship()
    {
        var manager = CreateManager();
        var restaurant = CreateRestaurant();

        manager.SetRestaurant(restaurant);

        Assert.Multiple(() =>
        {
            Assert.That(manager.Restaurant, Is.EqualTo(restaurant));
            Assert.That(restaurant.Managers, Contains.Item(manager));
        });
    }

    [Test]
    public void Manager_RemoveRestaurant_RemovesBidirectionalRelationship()
    {
        var manager = CreateManager();
        var restaurant = CreateRestaurant();

        manager.SetRestaurant(restaurant);
        manager.RemoveRestaurant();

        Assert.Multiple(() =>
        {
            Assert.That(manager.Restaurant, Is.Null);
            Assert.That(restaurant.Managers, Does.Not.Contain(manager));
        });
    }

    [Test]
    public void Manager_SetRestaurant_ThrowsWhenDuplicate()
    {
        var manager = CreateManager();
        var restaurant = CreateRestaurant();

        manager.SetRestaurant(restaurant);
        Assert.Throws<ArgumentException>(() => manager.SetRestaurant(restaurant), "Should prevent duplicate association");
    }

    [Test]
    public void Manager_SetRestaurant_ThrowsWhenAlreadyAssigned()
    {
        var manager = CreateManager();
        var restaurant1 = CreateRestaurant("Restaurant 1");
        var restaurant2 = CreateRestaurant("Restaurant 2");

        manager.SetRestaurant(restaurant1);
        Assert.Throws<InvalidOperationException>(() => manager.SetRestaurant(restaurant2), "Should prevent assigning to second restaurant");
    }

    // Chef ↔ Restaurant Association Tests (0..1 multiplicity)
    [Test]
    public void Chef_SetRestaurant_EstablishesBidirectionalRelationship()
    {
        var chef = CreateChef();
        var restaurant = CreateRestaurant();

        chef.SetRestaurant(restaurant);

        Assert.Multiple(() =>
        {
            Assert.That(chef.Restaurant, Is.EqualTo(restaurant));
            Assert.That(restaurant.Chefs, Contains.Item(chef));
        });
    }

    [Test]
    public void Chef_RemoveRestaurant_RemovesBidirectionalRelationship()
    {
        var chef = CreateChef();
        var restaurant = CreateRestaurant();

        chef.SetRestaurant(restaurant);
        chef.RemoveRestaurant();

        Assert.Multiple(() =>
        {
            Assert.That(chef.Restaurant, Is.Null);
            Assert.That(restaurant.Chefs, Does.Not.Contain(chef));
        });
    }

    [Test]
    public void Chef_SetRestaurant_ThrowsWhenDuplicate()
    {
        var chef = CreateChef();
        var restaurant = CreateRestaurant();

        chef.SetRestaurant(restaurant);
        Assert.Throws<ArgumentException>(() => chef.SetRestaurant(restaurant), "Should prevent duplicate association");
    }

    [Test]
    public void Chef_SetRestaurant_ThrowsWhenAlreadyAssigned()
    {
        var chef = CreateChef();
        var restaurant1 = CreateRestaurant("Restaurant 1");
        var restaurant2 = CreateRestaurant("Restaurant 2");

        chef.SetRestaurant(restaurant1);
        Assert.Throws<InvalidOperationException>(() => chef.SetRestaurant(restaurant2), "Should prevent assigning to second restaurant");
    }

    // Qualified Association Tests
    [Test]
    public void Customer_GetReservation_ReturnsReservationByDate()
    {
        var customer = CreateCustomer();
        var table = CreateTable();
        var date1 = new DateOnly(2025, 6, 15);
        var date2 = new DateOnly(2025, 6, 16);
        var reservation1 = new Reservation(Guid.NewGuid(), date1, new TimeOnly(18, 0), 2, table);
        var reservation2 = new Reservation(Guid.NewGuid(), date2, new TimeOnly(20, 0), 3, table);

        table.Reserve(customer, reservation1);
        table.Reserve(customer, reservation2);

        var found = customer.GetReservation(date1);

        Assert.Multiple(() =>
        {
            Assert.That(found, Is.EqualTo(reservation1));
            Assert.That(customer.GetReservation(date2), Is.EqualTo(reservation2));
            Assert.That(customer.GetReservation(new DateOnly(2025, 6, 17)), Is.Null);
        });
    }

    [Test]
    public void Order_GetDishByName_ReturnsDishByQualifiedName()
    {
        var customer = CreateCustomer();
        var table = CreateTable();
        var dish1 = CreateDish("Pizza", 20m);
        var dish2 = CreateDish("Burger", 15m);

        var order = customer.PlaceOrder(table, new[]
        {
            ("Pizza Order", dish1, 2),
            ("Burger Order", dish2, 1)
        });

        var foundDish = order.GetDishByName("Pizza");

        Assert.Multiple(() =>
        {
            Assert.That(foundDish, Is.EqualTo(dish1));
            Assert.That(order.GetDishByName("Burger"), Is.EqualTo(dish2));
            Assert.That(order.GetDishByName("Soup"), Is.Null);
        });
    }

    // Restaurant ↔ Table Association Tests
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
        var table1 = CreateTable(1, 4, restaurant);
        var table2 = new Table(1, 6, "Standard", restaurant, skipAutoAdd: true); // Same table number

        Assert.Throws<ArgumentException>(() => restaurant.AddTable(table2),
            "Adding duplicate table number should throw");
    }

    [Test]
    public void Restaurant_RemoveTable_RemovesTableFromRestaurant()
    {
        var restaurant = CreateRestaurant();
        var table = CreateTable(1, 4, restaurant);

        restaurant.RemoveTable(table);

        Assert.Multiple(() =>
        {
            Assert.That(restaurant.Tables, Does.Not.Contain(table),
                "Table should be removed from restaurant");
            Assert.That(restaurant.Tables.Count, Is.EqualTo(0));
            // In composition, table still references its restaurant
            Assert.That(table.Restaurant, Is.EqualTo(restaurant));
        });
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
        var menu1 = CreateMenu("Lunch Menu");
        var menu2 = CreateMenu("Dinner Menu");
        restaurant.AddMenu(menu1);
        restaurant.AddMenu(menu2);

        var result = restaurant.RemoveMenu(menu1);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.True, "RemoveMenu should return true");
            Assert.That(restaurant.Menus, Does.Not.Contain(menu1),
                "Menu should be removed from restaurant");
            Assert.That(restaurant.Menus, Contains.Item(menu2),
                "Other menu should remain");
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
        var dish1 = CreateDish("Pasta", 25m);
        var dish2 = CreateDish("Pizza", 20m);
        menu.AddDish(dish1);
        menu.AddDish(dish2);

        var result = menu.RemoveDish(dish1);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.True, "RemoveDish should return true");
            Assert.That(menu.Dishes, Does.Not.Contain(dish1),
                "Dish should be removed from menu");
            Assert.That(menu.Dishes, Contains.Item(dish2),
                "Other dish should remain");
            Assert.That(menu.Dishes.Count, Is.EqualTo(1));
        });
    }
     // Test 8: Order ↔ OrderDish Association
    [Test]
    public void Order_AddDish_OrderDishBelongsToOrder()
    {
        // Arrange
        var customer = CreateCustomer();
        var table = CreateTable();
        var order = new Order(customer, table);
        var dish = CreateDish();
        var orderDish = new OrderDish(order, "Pizza", dish, 2);

        // Act - OrderDish constructor automatically adds to order, but we can still test AddDish
        // order.AddDish(orderDish);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(order.Dishes, Contains.Item(orderDish), 
                "OrderDish should be in order's dishes");
            Assert.That(order.Dishes.Count, Is.EqualTo(1), 
                "Order should have one dish");
            Assert.That(order.TotalAmount, Is.EqualTo(40m), 
                "Total amount should be calculated correctly (2 * 20)");
        });
    }

    [Test]
    public void Order_AddDish_ThrowsWhenOrderDishIsNull()
    {
        // Arrange
        var customer = CreateCustomer();
        var table = CreateTable();
        var order = new Order(customer, table);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => order.AddDish(null!));
    }

    [Test]
    public void Order_AddDishes_AddsMultipleOrderDishes()
    {
        // Arrange
        var customer = CreateCustomer();
        var table = CreateTable();
        var order = new Order(customer, table);
        var dish1 = CreateDish("Pizza", 20m);
        var dish2 = CreateDish("Burger", 15m);
        var orderDish1 = new OrderDish(order, "Pizza", dish1, 1);
        var orderDish2 = new OrderDish(order, "Burger", dish2, 2);
        var orderDishes = new[] { orderDish1, orderDish2 };

        // Act - OrderDish constructors automatically add to order
        // order.AddDishes(orderDishes);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(order.Dishes.Count, Is.EqualTo(2), 
                "Order should have two dishes");
            Assert.That(order.TotalAmount, Is.EqualTo(50m), 
                "Total should be 20 + (15 * 2) = 50");
        });
    }

    [Test]
    public void Order_AddDishes_ThrowsWhenCollectionIsNull()
    {
        // Arrange
        var customer = CreateCustomer();
        var table = CreateTable();
        var order = new Order(customer, table);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => order.AddDishes(null!));
    }

    [Test]
    public void Order_AddDishes_ThrowsWhenCollectionContainsNull()
    {
        // Arrange
        var customer = CreateCustomer();
        var table = CreateTable();
        var order = new Order(customer, table);
        var dish = CreateDish();
        var orderDish1 = new OrderDish(order, "Pizza", dish, 1);
        var orderDishes = new OrderDish[]
        {
            orderDish1,
            null!
        };

        // Act & Assert
        Assert.Throws<ArgumentException>(() => order.AddDishes(orderDishes));
    }

    // Test 9: OrderDish ↔ Dish Association
    [Test]
    public void OrderDish_ReferencesDish_DishPriceUsedInCalculation()
    {
        // Arrange
        var customer = CreateCustomer();
        var table = CreateTable();
        var order = new Order(customer, table);
        var dish = CreateDish("Pasta", 30m);
        var quantity = 3;

        // Act
        var orderDish = new OrderDish(order, "Pasta", dish, quantity);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(orderDish.Dish, Is.EqualTo(dish), 
                "OrderDish should reference the correct dish");
            Assert.That(orderDish.Quantity, Is.EqualTo(quantity), 
                "OrderDish should have correct quantity");
            Assert.That(orderDish.TotalPrice, Is.EqualTo(90m), 
                "TotalPrice should be dish price * quantity (30 * 3)");
            Assert.That(orderDish.Dish.Price, Is.EqualTo(30m), 
                "Dish price should be accessible through OrderDish");
        });
    }

    [Test]
    public void OrderDish_Constructor_ThrowsWhenDishIsNull()
    {
        // Arrange
        var customer = CreateCustomer();
        var table = CreateTable();
        var order = new Order(customer, table);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new OrderDish(order, "Pizza", null!, 1));
    }

    [Test]
    public void OrderDish_Constructor_ThrowsWhenNameIsEmpty()
    {
        // Arrange
        var customer = CreateCustomer();
        var table = CreateTable();
        var order = new Order(customer, table);
        var dish = CreateDish();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new OrderDish(order, "", dish, 1));
    }

    [Test]
    public void OrderDish_Constructor_ThrowsWhenQuantityIsNotPositive()
    {
        // Arrange
        var customer = CreateCustomer();
        var table = CreateTable();
        var order = new Order(customer, table);
        var dish = CreateDish();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new OrderDish(order, "Pizza", dish, 0));
        Assert.Throws<ArgumentException>(() => new OrderDish(order, "Pizza", dish, -1));
    }

    // Test 10: Customer ↔ Payment Association (Through Order)
    [Test]
    public void Customer_MakePayment_CreatesPaymentWithOrderReference()
    {
        // Arrange
        var customer = CreateCustomer();
        var table = CreateTable();
        var dish = CreateDish();
        var order = customer.PlaceOrder(table, new[] { ("Pizza Order", dish, 2) });
        var paymentMethod = PaymentMethod.Card;
        var amount = 50m;

        // Act
        var payment = customer.MakePayment(order, paymentMethod, amount);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(payment.OrderId, Is.EqualTo(order.Id), 
                "Payment should reference the correct order ID");
            Assert.That(payment.Amount, Is.EqualTo(amount), 
                "Payment amount should be correct");
            Assert.That(payment.Method, Is.EqualTo(paymentMethod), 
                "Payment method should be correct");
        });
    }

    [Test]
    public void Payment_Constructor_ThrowsWhenOrderIsNull()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new Payment(null!, 50m, PaymentMethod.Card));
    }

    [Test]
    public void Payment_Constructor_ThrowsWhenAmountIsNotPositive()
    {
        // Arrange
        var customer = CreateCustomer();
        var table = CreateTable();
        var order = new Order(customer, table);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new Payment(order, 0m, PaymentMethod.Card));
        Assert.Throws<ArgumentException>(() => new Payment(order, -10m, PaymentMethod.Card));
    }

    // Test 11: Waiter ↔ Table Association (Bidirectional)
    [Test]
    public void Waiter_AssignTable_TableIsInWaiterAssignedTables()
    {
        // Arrange
        var workDetails = CreateWorkDetails();
        var experienceProfile = CreateExperienceProfile();
        var waiter = new Waiter("John", "Doe", new DateOnly(1990, 1, 1), "123456789", 
            workDetails, experienceProfile);
        var table = CreateTable(5, 6);

        // Act
        var result = waiter.AssignTable(table);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.True, "AssignTable should return true");
            Assert.That(waiter.AssignedTables, Contains.Item(table), 
                "Table should be in waiter's assigned tables");
            Assert.That(waiter.AssignedTables.Count, Is.EqualTo(1), 
                "Waiter should have one assigned table");
        });
    }

    [Test]
    public void Waiter_AssignTable_ReturnsFalseWhenTableAlreadyAssigned()
    {
        // Arrange
        var workDetails = CreateWorkDetails();
        var experienceProfile = CreateExperienceProfile();
        var waiter = new Waiter("John", "Doe", new DateOnly(1990, 1, 1), "123456789", 
            workDetails, experienceProfile);
        var table = CreateTable();

        // Act
        waiter.AssignTable(table);
        var result = waiter.AssignTable(table); // Try to assign same table again

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.False, "Assigning same table twice should return false");
            Assert.That(waiter.AssignedTables.Count, Is.EqualTo(1), 
                "Table should only be assigned once");
        });
    }

    [Test]
    public void Waiter_RemoveTable_RemovesBidirectionalRelationship()
    {
        var waiter = CreateWaiter();
        var table = CreateTable();

        waiter.AssignTable(table);
        var removed = waiter.RemoveTable(table);

        Assert.Multiple(() =>
        {
            Assert.That(removed, Is.True);
            Assert.That(waiter.AssignedTables, Does.Not.Contain(table));
            Assert.That(table.Waiter, Is.Null);
        });
    }

    // Order ↔ Payment Composition Tests
    [Test]
    public void Payment_Constructor_RequiresOrder()
    {
        var customer = CreateCustomer();
        var table = CreateTable();
        var order = new Order(customer, table);

        var payment = new Payment(order, 100m, PaymentMethod.Card);

        Assert.Multiple(() =>
        {
            Assert.That(payment.Order, Is.EqualTo(order));
            Assert.That(order.Payments, Contains.Item(payment));
        });
    }

    [Test]
    public void Order_RemovePayment_ThrowsWhenLastPayment()
    {
        var customer = CreateCustomer();
        var table = CreateTable();
        var order = new Order(customer, table);
        var payment = new Payment(order, 100m, PaymentMethod.Card);

        Assert.Throws<InvalidOperationException>(() => order.RemovePayment(payment), "Cannot remove last payment");
    }

    // Restaurant ↔ Table Composition Tests
    [Test]
    public void Table_Constructor_CanAcceptRestaurant()
    {
        var restaurant = CreateRestaurant();
        var table = new Table(1, 4, "Standard", restaurant);

        Assert.Multiple(() =>
        {
            Assert.That(table.Restaurant, Is.EqualTo(restaurant));
            Assert.That(restaurant.Tables, Contains.Item(table));
        });
    }

    [Test]
    public void Restaurant_AddTable_EstablishesBidirectionalRelationship()
    {
        var restaurant = CreateRestaurant();
        var table = CreateTable(1, 4, restaurant);

        Assert.Multiple(() =>
        {
            Assert.That(restaurant.Tables, Contains.Item(table));
            Assert.That(table.Restaurant, Is.EqualTo(restaurant));
        });
    }

    [Test]
    public void Restaurant_RemoveTable_RemovesBidirectionalRelationship()
    {
        var restaurant = CreateRestaurant();
        var table = CreateTable(1, 4, restaurant);

        restaurant.RemoveTable(table);

        Assert.Multiple(() =>
        {
            Assert.That(restaurant.Tables, Does.Not.Contain(table));
            Assert.That(table.Restaurant, Is.EqualTo(restaurant));
        });
    }

    // Restaurant ↔ Menu Aggregation Tests
    [Test]
    public void Restaurant_AddMenu_EstablishesBidirectionalRelationship()
    {
        var restaurant = CreateRestaurant();
        var menu = CreateMenu();

        restaurant.AddMenu(menu);

        Assert.Multiple(() =>
        {
            Assert.That(restaurant.Menus, Contains.Item(menu));
            Assert.That(menu.Restaurant, Is.EqualTo(restaurant));
        });
    }

    [Test]
    public void Restaurant_RemoveMenu_ThrowsWhenLastMenu()
    {
        var restaurant = CreateRestaurant();
        var menu = CreateMenu();

        restaurant.AddMenu(menu);

        Assert.Throws<InvalidOperationException>(() => restaurant.RemoveMenu(menu), "Cannot remove last menu");
    }

    // Menu ↔ Dish Aggregation Tests
    [Test]
    public void Menu_AddDish_EstablishesBidirectionalRelationship()
    {
        var menu = CreateMenu();
        var dish = CreateDish();

        menu.AddDish(dish);

        Assert.Multiple(() =>
        {
            Assert.That(menu.Dishes, Contains.Item(dish));
            Assert.That(dish.Menu, Is.EqualTo(menu));
        });
    }

    [Test]
    public void Menu_RemoveDish_ThrowsWhenLastDish()
    {
        var menu = CreateMenu();
        var dish = CreateDish();

        menu.AddDish(dish);

        Assert.Throws<InvalidOperationException>(() => menu.RemoveDish(dish), "Cannot remove last dish");
    }

    // Order ↔ OrderDish Composition Tests
    [Test]
    public void OrderDish_Constructor_RequiresOrder()
    {
        var customer = CreateCustomer();
        var table = CreateTable();
        var order = new Order(customer, table);
        var dish = CreateDish();

        var orderDish = new OrderDish(order, "Pizza Order", dish, 2);

        Assert.Multiple(() =>
        {
            Assert.That(orderDish.Order, Is.EqualTo(order));
            Assert.That(order.Dishes, Contains.Item(orderDish));
            Assert.That(dish.OrderDishes, Contains.Item(orderDish));
        });
    }

    [Test]
    public void OrderDish_Constructor_ThrowsWhenOrderIsNull()
    {
        var dish = CreateDish();

        Assert.Throws<ArgumentNullException>(() => new OrderDish(null!, "Pizza", dish, 2), "OrderDish requires Order");
    }

    // Duplication Prevention Tests
    [Test]
    public void Customer_MakeReservation_ThrowsWhenDuplicate()
    {
        var customer = CreateCustomer();
        var table = CreateTable();
        var reservation = new Reservation(Guid.NewGuid(), new DateOnly(2025, 6, 15), new TimeOnly(19, 0), 2, table);

        table.Reserve(customer, reservation);
        Assert.Throws<ArgumentException>(() => customer.MakeReservation(reservation), "Should prevent duplicate reservation");
    }

    [Test]
    public void Waiter_AssignTable_ReturnsFalseWhenDuplicate()
    {
        var waiter = CreateWaiter();
        var table = CreateTable();

        waiter.AssignTable(table);
        var result = waiter.AssignTable(table);

        Assert.That(result, Is.False, "Should return false for duplicate assignment");
    }
}

