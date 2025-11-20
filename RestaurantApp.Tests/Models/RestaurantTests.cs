using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using NUnit.Framework;
using RestaurantApp.Models;

namespace RestaurantApp.Tests.Models;

[TestFixture]
public class RestaurantTests
{
    [Test]
    public void Restaurant_Constructor_InitializesPropertiesCorrectly()
    {
        var restaurant = new Restaurant("Test Restaurant", 80);
        Assert.That(restaurant.Name, Is.EqualTo("Test Restaurant"));
        Assert.That(restaurant.MaxCapacity, Is.EqualTo(80));
        Assert.That(restaurant.Tables, Is.Empty);
        Assert.That(restaurant.Menus, Is.Empty);
    }

    [Test]
    public void Restaurant_AddTable_AddsTableToExtent()
    {
        var restaurant = new Restaurant("Test", 50);
        var table = new Table(1, 4);
        restaurant.AddTable(table);
        Assert.That(restaurant.Tables, Contains.Item(table));
        Assert.That(restaurant.GetNumberOfTables(), Is.EqualTo(1));
        Assert.That(restaurant.Tables.All(t => t is Table), Is.True);
    }

    [Test]
    public void Restaurant_AddTable_DoesNotAddDuplicateTableNumber()
    {
        var restaurant = new Restaurant("Test", 50);
        var t1 = new Table(1, 4);
        var t2 = new Table(1, 2);
        restaurant.AddTable(t1);
        restaurant.AddTable(t2);
        Assert.That(restaurant.GetNumberOfTables(), Is.EqualTo(1));
    }

    [Test]
    public void Restaurant_RemoveTable_RemovesTableFromExtent()
    {
        var restaurant = new Restaurant("Test", 50);
        var table = new Table(1, 4);
        restaurant.AddTable(table);
        restaurant.RemoveTable(table);
        Assert.That(restaurant.Tables, Does.Not.Contain(table));
        Assert.That(restaurant.GetNumberOfTables(), Is.EqualTo(0));
    }

    [Test]
    public void Restaurant_AddMenu_AddsMenuToExtent()
    {
        var restaurant = new Restaurant("Test", 50);
        var menu = new Menu("Lunch");
        restaurant.AddMenu(menu);
        Assert.That(restaurant.Menus, Contains.Item(menu));
        Assert.That(restaurant.Menus.All(m => m is Menu), Is.True);
    }

    [Test]
    public void Restaurant_AddMenu_DoesNotAddDuplicateMenuName()
    {
        var restaurant = new Restaurant("Test", 50);
        var m1 = new Menu("Lunch");
        var m2 = new Menu("Lunch");
        restaurant.AddMenu(m1);
        restaurant.AddMenu(m2);
        Assert.That(restaurant.Menus.Count, Is.EqualTo(1));
    }

    [Test]
    public void Restaurant_RemoveMenu_RemovesMenuFromExtent()
    {
        var restaurant = new Restaurant("Test", 50);
        var menu = new Menu("Lunch");
        restaurant.AddMenu(menu);
        var removed = restaurant.RemoveMenu(menu);
        Assert.That(removed, Is.True);
        Assert.That(restaurant.Menus, Does.Not.Contain(menu));
    }

    [Test]
    public void Restaurant_ModifyingExternalCopyOfTables_DoesNotAffectExtent()
    {
        var restaurant = new Restaurant("Test", 50);
        restaurant.AddTable(new Table(1, 4));
        restaurant.AddTable(new Table(2, 2));

        var externalCopy = restaurant.Tables.ToList();
        externalCopy.Add(new Table(3, 6));

        Assert.That(restaurant.GetNumberOfTables(), Is.EqualTo(2));
    }

    [Test]
    public void Restaurant_ModifyingExternalCopyOfMenus_DoesNotAffectExtent()
    {
        var restaurant = new Restaurant("Test", 50);
        restaurant.AddMenu(new Menu("Lunch"));
        restaurant.AddMenu(new Menu("Dinner"));

        var externalCopy = restaurant.Menus.ToList();
        externalCopy.Add(new Menu("Breakfast"));

        Assert.That(restaurant.Menus.Count, Is.EqualTo(2));
    }

    [Test]
    public void Restaurant_Extent_IsPersistedAndRestoredFromFile()
    {
        var restaurants = new List<Restaurant>
        {
            new Restaurant("R1", 50),
            new Restaurant("R2", 100)
        };

        restaurants[0].AddTable(new Table(1, 4));
        restaurants[0].AddMenu(new Menu("Lunch"));

        restaurants[1].AddTable(new Table(2, 2));
        restaurants[1].AddMenu(new Menu("Dinner"));

        var filePath = Path.Combine(Path.GetTempPath(), "restaurants_extent.json");

        try
        {
            var json = JsonSerializer.Serialize(restaurants);
            File.WriteAllText(filePath, json);

            var loadedJson = File.ReadAllText(filePath);
            var loaded = JsonSerializer.Deserialize<List<Restaurant>>(loadedJson);

            Assert.IsNotNull(loaded);
            Assert.That(loaded!.Count, Is.EqualTo(restaurants.Count));

            Assert.That(loaded[0].Name, Is.EqualTo("R1"));
            Assert.That(loaded[0].MaxCapacity, Is.EqualTo(50));
            Assert.That(loaded[0].Tables.Count, Is.EqualTo(1));
            Assert.That(loaded[0].Menus.Count, Is.EqualTo(1));

            Assert.That(loaded[1].Name, Is.EqualTo("R2"));
            Assert.That(loaded[1].MaxCapacity, Is.EqualTo(100));
            Assert.That(loaded[1].Tables.Count, Is.EqualTo(1));
            Assert.That(loaded[1].Menus.Count, Is.EqualTo(1));
        }
        finally
        {
            if (File.Exists(filePath))
                File.Delete(filePath);
        }
    }

    [Test]
    public void Restaurant_AddingValidTableAndMenu_DoesNotThrow()
    {
        var restaurant = new Restaurant("Test", 50);
        var table = new Table(1, 4);
        var menu = new Menu("Lunch");

        Assert.DoesNotThrow(() =>
        {
            restaurant.AddTable(table);
            restaurant.AddMenu(menu);
        });
    }
}
