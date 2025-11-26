using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using RestaurantApp.Models;

namespace RestaurantApp.Tests.Models;

[TestFixture]
public class RestaurantTests
{
    [Test]
    public void Restaurant_Extent_StoresAllCreatedRestaurants()
    {
        var tempPath = Path.Combine(Path.GetTempPath(), "restaurants_extent_test_reset.json");
        if (File.Exists(tempPath))
            File.Delete(tempPath);

        Restaurant.Load(tempPath);

        var r1 = new Restaurant("R1", 50);
        var r2 = new Restaurant("R2", 100);

        Assert.That(Restaurant.Extent, Has.Count.EqualTo(2));
        Assert.That(Restaurant.Extent.Any(r => r.Name == "R1" && r.MaxCapacity == 50), Is.True);
        Assert.That(Restaurant.Extent.Any(r => r.Name == "R2" && r.MaxCapacity == 100), Is.True);
    }

    [Test]
    public void Restaurant_Extent_IsPersistedAndRestoredFromFile()
    {
        var filePath = Path.Combine(Path.GetTempPath(), "restaurants_extent_test.json");
        if (File.Exists(filePath))
            File.Delete(filePath);

        Restaurant.Load(filePath);

        var r1 = new Restaurant("R1", 50);
        var r2 = new Restaurant("R2", 100);

        Restaurant.Save(filePath);

        var extra = new Restaurant("Extra", 10);
        Assert.That(Restaurant.Extent.Count, Is.EqualTo(3));

        var loaded = Restaurant.Load(filePath);

        Assert.That(loaded, Is.True);
        Assert.That(Restaurant.Extent.Count, Is.EqualTo(2));
        Assert.That(Restaurant.Extent.Any(r => r.Name == "R1" && r.MaxCapacity == 50), Is.True);
        Assert.That(Restaurant.Extent.Any(r => r.Name == "R2" && r.MaxCapacity == 100), Is.True);
        Assert.That(Restaurant.Extent.Any(r => r.Name == "Extra"), Is.False);
    }
}
