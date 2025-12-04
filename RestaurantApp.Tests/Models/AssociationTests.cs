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
     // Test 8: Order ↔ OrderDish Association
    [Test]
    public void Order_AddDish_OrderDishBelongsToOrder()
    {
        // Arrange
        var customer = CreateCustomer();
        var table = CreateTable();
        var order = new Order(customer, table);
        var dish = CreateDish();
        var orderDish = new OrderDish("Pizza", dish, 2);

        // Act
        order.AddDish(orderDish);

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
        var orderDishes = new[]
        {
            new OrderDish("Pizza", dish1, 1),
            new OrderDish("Burger", dish2, 2)
        };

        // Act
        order.AddDishes(orderDishes);

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
        var orderDishes = new OrderDish[]
        {
            new OrderDish("Pizza", dish, 1),
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
        var dish = CreateDish("Pasta", 30m);
        var quantity = 3;

        // Act
        var orderDish = new OrderDish("Pasta", dish, quantity);

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
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new OrderDish("Pizza", null!, 1));
    }

    [Test]
    public void OrderDish_Constructor_ThrowsWhenNameIsEmpty()
    {
        // Arrange
        var dish = CreateDish();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new OrderDish("", dish, 1));
    }

    [Test]
    public void OrderDish_Constructor_ThrowsWhenQuantityIsNotPositive()
    {
        // Arrange
        var dish = CreateDish();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new OrderDish("Pizza", dish, 0));
        Assert.Throws<ArgumentException>(() => new OrderDish("Pizza", dish, -1));
    }

    // Test 10: Customer ↔ Payment Association (Through Order)
    [Test]
    public void Customer_MakePayment_CreatesPaymentWithOrderReference()
    {
        // Arrange
        var customer = CreateCustomer();
        var table = CreateTable();
        var dish = CreateDish();
        var orderDishes = new[] { new OrderDish("Pizza", dish, 2) };
        var order = customer.PlaceOrder(table, orderDishes);
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
    public void Payment_Constructor_ThrowsWhenOrderIdIsEmpty()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new Payment(Guid.Empty, 50m, PaymentMethod.Card));
    }

    [Test]
    public void Payment_Constructor_ThrowsWhenAmountIsNotPositive()
    {
        // Arrange
        var orderId = Guid.NewGuid();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new Payment(orderId, 0m, PaymentMethod.Card));
        Assert.Throws<ArgumentException>(() => new Payment(orderId, -10m, PaymentMethod.Card));
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
     [Test]
    public void Manager_AssignTableToWaiter_TableAppearsInWaiterAssignedTables()
    {
        // Arrange
        var workDetails = CreateWorkDetails();
        var experienceProfile = CreateExperienceProfile();
        var manager = new Manager("Manager", "Boss", new DateOnly(1980, 1, 1), "111111111", 
            workDetails, experienceProfile, level: 5);
        var waiter = new Waiter("John", "Doe", new DateOnly(1990, 1, 1), "123456789", 
            workDetails, experienceProfile);
        var table = CreateTable(10, 8);

        // Act
        var result = manager.AssignTable(waiter, table);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.True, "Manager.AssignTable should return true");
            Assert.That(waiter.AssignedTables, Contains.Item(table), 
                "Table should be in waiter's assigned tables");
            Assert.That(waiter.AssignedTables.Count, Is.EqualTo(1), 
                "Waiter should have one assigned table");
        });
    }

    [Test]
    public void Manager_AssignTableToWaiter_ThrowsWhenWaiterIsNull()
    {
        // Arrange
        var workDetails = CreateWorkDetails();
        var experienceProfile = CreateExperienceProfile();
        var manager = new Manager("Manager", "Boss", new DateOnly(1980, 1, 1), "111111111", 
            workDetails, experienceProfile, level: 5);
        var table = CreateTable();

        // Act & Assert
        // Manager.AssignTable calls waiter.AssignTable directly, so null waiter causes NullReferenceException
        Assert.Throws<NullReferenceException>(() => manager.AssignTable(null!, table));
    }

    // Test 13: Chef ↔ Menu Association (Through Dish Operations)
    [Test]
    public void Chef_AddDishToMenu_DishAppearsInMenu()
    {
        // Arrange
        var workDetails = CreateWorkDetails();
        var experienceProfile = CreateExperienceProfile();
        var chef = new HeadChef("Chef", "Cook", new DateOnly(1985, 1, 1), "222222222", 
            workDetails, experienceProfile, "Italian", kitchenExperienceYears: 10);
        var menu = CreateMenu();
        var dish = CreateDish("Risotto", 35m);

        // Act
        chef.AddDish(menu, dish);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(menu.Dishes, Contains.Item(dish), 
                "Dish should be in menu after chef adds it");
            Assert.That(menu.Dishes.Count, Is.EqualTo(1), 
                "Menu should have one dish");
        });
    }

    [Test]
    public void Chef_ViewMenu_ReturnsMenuDishes()
    {
        // Arrange
        var workDetails = CreateWorkDetails();
        var experienceProfile = CreateExperienceProfile();
        var chef = new HeadChef("Chef", "Cook", new DateOnly(1985, 1, 1), "222222222", 
            workDetails, experienceProfile, "Italian", kitchenExperienceYears: 10);
        var menu = CreateMenu();
        var dish1 = CreateDish("Pasta", 25m);
        var dish2 = CreateDish("Pizza", 20m);
        menu.AddDish(dish1);
        menu.AddDish(dish2);

        // Act
        var dishes = chef.ViewMenu(menu);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(dishes, Contains.Item(dish1), 
                "Chef should be able to view dish1");
            Assert.That(dishes, Contains.Item(dish2), 
                "Chef should be able to view dish2");
            Assert.That(dishes.Count, Is.EqualTo(2), 
                "Chef should see both dishes");
        });
    }

    [Test]
    public void Chef_UpdateMenu_UpdatesDishInMenu()
    {
        // Arrange
        var workDetails = CreateWorkDetails();
        var experienceProfile = CreateExperienceProfile();
        var chef = new HeadChef("Chef", "Cook", new DateOnly(1985, 1, 1), "222222222", 
            workDetails, experienceProfile, "Italian", kitchenExperienceYears: 10);
        var menu = CreateMenu();
        var existingDish = CreateDish("Old Dish", 20m);
        var updatedDish = CreateDish("New Dish", 25m);
        menu.AddDish(existingDish);

        // Act
        chef.UpdateMenu(menu, existingDish, updatedDish);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(menu.Dishes, Contains.Item(updatedDish), 
                "Menu should contain the updated dish");
            Assert.That(menu.Dishes, Does.Not.Contain(existingDish), 
                "Menu should not contain the old dish");
            Assert.That(menu.Dishes.Count, Is.EqualTo(1), 
                "Menu should still have one dish");
        });
    }

    // Test 14: Employee ↔ WorkDetails Association (Composition)
    [Test]
    public void Employee_HasWorkDetails_WorkDetailsAreCorrectlyAssociated()
    {
        // Arrange
        var workDetails = CreateWorkDetails();
        var experienceProfile = CreateExperienceProfile();
        var waiter = new Waiter("John", "Doe", new DateOnly(1990, 1, 1), "123456789", 
            workDetails, experienceProfile);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(waiter.WorkDetails, Is.EqualTo(workDetails), 
                "Employee should reference the correct WorkDetails");
            Assert.That(waiter.WorkDetails.Department, Is.EqualTo("Kitchen"), 
                "WorkDetails department should be accessible");
            Assert.That(waiter.WorkDetails.ShiftSchedule, Is.EqualTo("Day Shift"), 
                "WorkDetails shift schedule should be accessible");
            Assert.That(waiter.WorkDetails.DateOfHiring, Is.EqualTo(new DateOnly(2020, 1, 1)), 
                "WorkDetails date of hiring should be accessible");
        });
    }

    // Test 15: Employee ↔ EmployeeExperienceProfile Association
    [Test]
    public void Employee_UpdateExperienceProfile_ProfileIsUpdated()
    {
        // Arrange
        var workDetails = CreateWorkDetails();
        var initialProfile = CreateExperienceProfile();
        var waiter = new Waiter("John", "Doe", new DateOnly(1990, 1, 1), "123456789", 
            workDetails, initialProfile);
        var newProfile = new SpecialistProfile("Fine Dining");

        // Act
        waiter.UpdateExperienceProfile(newProfile);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(waiter.ExperienceProfile, Is.EqualTo(newProfile), 
                "Employee experience profile should be updated");
            Assert.That(waiter.ExperienceProfile, Is.InstanceOf<SpecialistProfile>(), 
                "Profile should be of type SpecialistProfile");
            Assert.That(((SpecialistProfile)waiter.ExperienceProfile).FieldOfExpertise, 
                Is.EqualTo("Fine Dining"), 
                "Specialist profile should have correct field of expertise");
        });
    }

    [Test]
    public void Employee_UpdateExperienceProfile_AllowsMultipleUpdates()
    {
        // Arrange
        var workDetails = CreateWorkDetails();
        var initialProfile = new TraineeProfile(6);
        var waiter = new Waiter("John", "Doe", new DateOnly(1990, 1, 1), "123456789", 
            workDetails, initialProfile);
        var experiencedProfile = new ExperiencedProfile(3, "Mentor Name");
        var specialistProfile = new SpecialistProfile("Beverage Service");

        // Act
        waiter.UpdateExperienceProfile(experiencedProfile);
        waiter.UpdateExperienceProfile(specialistProfile);

        // Assert
        Assert.That(waiter.ExperienceProfile, Is.EqualTo(specialistProfile), 
            "Final profile should be the last one set");
    }
}
 }




