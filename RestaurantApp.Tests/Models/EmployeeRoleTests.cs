using System;
using System.Collections.Generic;
using NUnit.Framework;
using RestaurantApp.Models;
using RestaurantApp.Models.Roles;
using RestaurantApp.Models.Roles.EmployeeTypes;

namespace RestaurantApp.Tests.Models;

[TestFixture]
public class EmployeeRoleTests
{
    [Test]
    public void ManagerType_AssignTable_ReturnsTrueWhenSuccessful()
    {
        var managerType = new ManagerType(level: 3);
        var waiterType = new WaiterType();

        var restaurant = new Restaurant("Test Restaurant", 100);
        var table = new Table(1, 4, "Standard", restaurant);

        var result = managerType.AssignTable(waiterType, table);

        Assert.That(result, Is.True);
        Assert.That(waiterType.AssignedTables, Contains.Item(table));
    }

    [Test]
    public void ChefType_AddDish_AddsDishToMenu()
    {
        var chefType = new ChefType("Italian");

        var menu = new Menu("Dinner Menu", "Dinner", new[] { "English", "Spanish" });
        var dish = new Dish("Pasta Carbonara", "Italian", false, false, 18.50m, new[] { "Pasta", "Eggs", "Bacon", "Parmesan" });

        chefType.AddDish(menu, dish);

        Assert.That(menu.Dishes, Contains.Item(dish));
        Assert.That(menu.Dishes.Count, Is.EqualTo(1));
    }

    [Test]
    public void WaiterType_AssignTable_AddsTableToAssignedTables()
    {
        var waiterType = new WaiterType();
        var restaurant = new Restaurant("Test Restaurant", 100);
        var table = new Table(1, 4, "Standard", restaurant);

        var result = waiterType.AssignTable(table);

        Assert.That(result, Is.True);
        Assert.That(waiterType.AssignedTables, Contains.Item(table));
        Assert.That(waiterType.AssignedTables.Count, Is.EqualTo(1));
    }

    [Test]
    public void ValetType_AssignedLocation_IsSetCorrectly()
    {
        var assignedLocation = "Entrance";
        var valetType = new ValetType(assignedLocation);

        Assert.That(valetType.AssignedLocation, Is.EqualTo(assignedLocation));
    }
}
