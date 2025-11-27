using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using RestaurantApp.Models;

try
{
    var workDetails = new WorkDetails("Dining", "Evening", DateOnly.FromDateTime(DateTime.Today.AddYears(-2)));
    var experienceProfile = new ExperiencedProfile(5, "Chef Gomez");
    var manager = new Manager("Mustafa", "Atalan", DateOnly.Parse("2002-05-14"), "555-0001", workDetails, experienceProfile, level: 2);

    var restaurant = new Restaurant("BYT Bistro", 120);
    var mainMenu = new Menu("Main Menu", "Dinner", new[] { "English", "Turkish" });
    var margherita = new Dish("Margherita Pizza", "Italian", true, false, 14.50m, new[] { "Dough", "Tomato", "Mozzarella", "Basil" });
    var steak = new Dish("Grilled Steak", "American", false, false, 28.00m, new[] { "Beef", "Salt", "Pepper" });
    mainMenu.AddDish(margherita);
    mainMenu.AddDish(steak);
    restaurant.AddMenu(mainMenu);

    var table1 = new Table(1, 4, "Standard");
    var table2 = new Table(2, 2, "Window");
    restaurant.AddTable(table1);
    restaurant.AddTable(table2);

    var customer = new Member("Berkay", "Bayar", DateOnly.Parse("1999-03-12"), "555-2222", "berkay@example.com", 5, 2.5m);
    var reservation = new Reservation(Guid.NewGuid(), DateOnly.FromDateTime(DateTime.Today.AddDays(1)), 2, table1);
    table1.Reserve(customer, reservation);
    reservation.Confirm();

    var orderDishes = new List<OrderDish>
    {
        new("Margherita Order", margherita, 2),
        new("Steak Order", steak, 1)
    };

    var order = customer.PlaceOrder(table1, orderDishes);
    order.CompleteOrder();
    customer.AddCredits();

    var total = order.CalculateTotal();
    var discountedTotal = customer.UseCredits(total);
    var payment = customer.MakePayment(order, PaymentMethod.Card, discountedTotal);

    Console.WriteLine($"Manager on duty: {manager.GetFullName()}");
    Console.WriteLine($"Processed payment amount: {payment.Amount:C}");

    var filePath = "restaurant_data.json";
    try
    {
        var options = new JsonSerializerOptions { WriteIndented = true, Converters = { new DateOnlyJsonConverter() } };
        File.WriteAllText(filePath, JsonSerializer.Serialize(restaurant, options));
        Console.WriteLine($"Restaurant data saved to {filePath}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error saving restaurant: {ex.Message}");
    }

    try
    {
        var options = new JsonSerializerOptions { Converters = { new DateOnlyJsonConverter() } };
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

public class DateOnlyJsonConverter : JsonConverter<DateOnly>
{
    public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        DateOnly.Parse(reader.GetString()!);

    public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options) =>
        writer.WriteStringValue(value.ToString("yyyy-MM-dd"));
}