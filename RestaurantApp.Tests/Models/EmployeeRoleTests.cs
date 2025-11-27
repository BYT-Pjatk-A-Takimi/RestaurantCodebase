using System;
using System.Collections.Generic;
using NUnit.Framework;
using RestaurantApp.Models;

namespace RestaurantApp.Tests.Models;

[TestFixture]
public class EmployeeRoleTests
{
    [Test]
    public void Manager_AssignTable_ReturnsTrueWhenSuccessful()
    {
        var workDetails = new WorkDetails("Management", "Day", DateOnly.FromDateTime(DateTime.Today));
        var experienceProfile = new ExperiencedProfile(10, "Senior Manager");
        var manager = new Manager("Berkay", "Bayar", DateOnly.Parse("1999-03-12"), "111-1111", workDetails, experienceProfile, level: 3);
        
        var waiterWorkDetails = new WorkDetails("Service", "Evening", DateOnly.FromDateTime(DateTime.Today));
        var waiterExperienceProfile = new TraineeProfile(6);
        var waiter = new Waiter("Berkay", "Bayar", DateOnly.Parse("1999-03-12"), "111-1111", waiterWorkDetails, waiterExperienceProfile);
        
        var table = new Table(1, 4, "Standard");

        var result = manager.AssignTable(waiter, table);

        Assert.That(result, Is.True);
        Assert.That(waiter.AssignedTables, Contains.Item(table));
    }

    [Test]
    public void Chef_AddDish_AddsDishToMenu()
    {
        var workDetails = new WorkDetails("Kitchen", "Day", DateOnly.FromDateTime(DateTime.Today));
        var experienceProfile = new SpecialistProfile("Italian Cuisine");
        var chef = new Chef("Berkay", "Bayar", DateOnly.Parse("1999-03-12"), "111-1111", workDetails, experienceProfile, "Italian");
        
        var menu = new Menu("Dinner Menu", "Dinner", new[] { "English", "Spanish" });
        var dish = new Dish("Pasta Carbonara", "Italian", false, false, 18.50m, new[] { "Pasta", "Eggs", "Bacon", "Parmesan" });

        chef.AddDish(menu, dish);

        Assert.That(menu.Dishes, Contains.Item(dish));
        Assert.That(menu.Dishes.Count, Is.EqualTo(1));
    }

    [Test]
    public void Waiter_AssignTable_AddsTableToAssignedTables()
    {
        var workDetails = new WorkDetails("Service", "Evening", DateOnly.FromDateTime(DateTime.Today));
        var experienceProfile = new ExperiencedProfile(3, "Head Waiter");
        var waiter = new Waiter("Berkay", "Bayar", DateOnly.Parse("1999-03-12"), "111-1111", workDetails, experienceProfile);
        var table = new Table(1, 4, "Standard");

        var result = waiter.AssignTable(table);

        Assert.That(result, Is.True);
        Assert.That(waiter.AssignedTables, Contains.Item(table));
        Assert.That(waiter.AssignedTables.Count, Is.EqualTo(1));
    }

    [Test]
    public void Valet_AssignedLocation_IsSetCorrectly()
    {
        var assignedLocation = "Entrance";
        var workDetails = new WorkDetails("Valet", "Day", DateOnly.FromDateTime(DateTime.Today));
        var experienceProfile = new TraineeProfile(2);
        var valet = new Valet("Berkay", "Bayar", DateOnly.Parse("1999-03-12"), "111-1111", workDetails, experienceProfile, assignedLocation);

        Assert.That(valet.AssignedLocation, Is.EqualTo(assignedLocation));
    }
}

