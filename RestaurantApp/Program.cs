using System;
using System.Collections.Generic;
using System.Text.Json;
using RestaurantApp;
using RestaurantApp.Models;
using RestaurantApp.Models.Roles;
using RestaurantApp.Models.Roles.EmployeeTypes;
using RestaurantApp.Models.Roles.MembershipStatus;

try
{
    var workDetails = new WorkDetails("Dining", "Evening", DateOnly.FromDateTime(DateTime.Today.AddYears(-2)));
    var experienceProfile = new ExperiencedProfile(5, "Chef Gomez");
    var managerType = new ManagerType(level: 2);
    var employeeRole = new EmployeeRole(workDetails, experienceProfile, managerType);
    var manager = new Person("Mustafa", "Atalan", DateOnly.Parse("2002-05-14"), "555-0001", employeeRole: employeeRole);

    var restaurant = new Restaurant("BYT Bistro", 120);
    var mainMenu = new Menu("Main Menu", "Dinner", new[] { "English", "Turkish" });
    var margherita = new Dish("Margherita Pizza", "Italian", true, false, 14.50m, new[] { "Dough", "Tomato", "Mozzarella", "Basil" });
    var steak = new Dish("Grilled Steak", "American", false, false, 28.00m, new[] { "Beef", "Salt", "Pepper" });
    mainMenu.AddDish(margherita);
    mainMenu.AddDish(steak);
    restaurant.AddMenu(mainMenu);

    var table1 = new Table(1, 4, "Standard", restaurant);
    var table2 = new Table(2, 2, "Window", restaurant);

    var memberStatus = new MemberStatus(5, 2.5m);
    var customerRole = new CustomerRole("berkay@example.com", memberStatus);
    var customer = new Person("Berkay", "Bayar", DateOnly.Parse("1999-03-12"), "555-2222", customerRole: customerRole);
    
    var reservation = new Reservation(Guid.NewGuid(), DateOnly.FromDateTime(DateTime.Today.AddDays(1)), new TimeOnly(19, 0), 2, table1);
    table1.Reserve(customer, reservation);
    reservation.Confirm();

    var orderDishes = new List<(string name, Dish dish, int quantity)>
    {
        ("Margherita Order", margherita, 2),
        ("Steak Order", steak, 1)
    };

    var order = customerRole.PlaceOrder(table1, orderDishes);
    order.CompleteOrder();
    memberStatus.AddCredits();

    var total = order.CalculateTotal();
    var discountedTotal = memberStatus.UseCredits(total);
    var payment = customerRole.MakePayment(order, PaymentMethod.Card, discountedTotal);

    Console.WriteLine($"Manager on duty: {manager.GetFullName()}");
    Console.WriteLine($"Processed payment amount: {payment.Amount:C}");

    var filePath = "restaurant_data.json";
    try
    {
        var options = JsonSerialization.GetDefaultOptions();
        File.WriteAllText(filePath, JsonSerializer.Serialize(restaurant, options));
        Console.WriteLine($"Restaurant data saved to {filePath}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error saving restaurant: {ex.Message}");
    }

    try
    {
        var options = JsonSerialization.GetDefaultOptions();
        var json = File.ReadAllText(filePath);
        var loadedRestaurant = JsonSerializer.Deserialize<Restaurant>(json, options);
        Console.WriteLine($"Restaurant loaded: {loadedRestaurant?.Name} with {loadedRestaurant?.GetNumberOfTables()} tables");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error loading restaurant: {ex.Message}");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex.Message}");
    Environment.ExitCode = 1;
}
