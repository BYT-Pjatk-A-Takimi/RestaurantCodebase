using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace RestaurantApp.Models;

public class Manager : Employee
{
    [JsonConstructor]
    public Manager(
        string firstName,
        string lastName,
        DateOnly birthDate,
        string phoneNumber,
        WorkDetails workDetails,
        EmployeeExperienceProfile experienceProfile,
        int level)
        : base(firstName, lastName, birthDate, phoneNumber, workDetails, experienceProfile)
    {
        Level = level;
    }

    public int Level { get; }

    public bool AssignTable(Waiter waiter, Table table) => waiter.AssignTable(table);
}

[JsonPolymorphic]
[JsonDerivedType(typeof(HeadChef), typeDiscriminator: "HeadChef")]
[JsonDerivedType(typeof(SousChef), typeDiscriminator: "SousChef")]
[JsonDerivedType(typeof(LineChef), typeDiscriminator: "LineChef")]
public class Chef : Employee
{
    [JsonConstructor]
    public Chef(
        string firstName,
        string lastName,
        DateOnly birthDate,
        string phoneNumber,
        WorkDetails workDetails,
        EmployeeExperienceProfile experienceProfile,
        string cuisineType)
        : base(firstName, lastName, birthDate, phoneNumber, workDetails, experienceProfile)
    {
        CuisineType = cuisineType;
    }

    public string CuisineType { get; }

    public void assignTask() {}

    public void AddDish(Menu menu, Dish dish) => menu.AddDish(dish);

    public void viewOrders() {}

    public IReadOnlyCollection<Dish> ViewMenu(Menu menu) => menu.Dishes;

    public void UpdateMenu(Menu menu, Dish existingDish, Dish updatedDish) => menu.UpdateDish(existingDish, updatedDish);
}

public class HeadChef : Chef
{
    [JsonConstructor]
    public HeadChef(
        string firstName,
        string lastName,
        DateOnly birthDate,
        string phoneNumber,
        WorkDetails workDetails,
        EmployeeExperienceProfile experienceProfile,
        string cuisineType,
        int kitchenExperienceYears)
        : base(firstName, lastName, birthDate, phoneNumber, workDetails, experienceProfile, cuisineType)
    {
        KitchenExperienceYears = kitchenExperienceYears;
    }

    public int KitchenExperienceYears { get; }

    public void OverseeKitchen() { }

    public void ApproveMenuChanges(Menu menu) { }
}

public class SousChef : Chef
{
    [JsonInclude]
    private readonly List<string> _supervisedSections;

    [JsonConstructor]
    public SousChef(
        string firstName,
        string lastName,
        DateOnly birthDate,
        string phoneNumber,
        WorkDetails workDetails,
        EmployeeExperienceProfile experienceProfile,
        string cuisineType,
        bool dayShift,
        IReadOnlyCollection<string> supervisedSections)
        : base(firstName, lastName, birthDate, phoneNumber, workDetails, experienceProfile, cuisineType)
    {
        DayShift = dayShift;
        _supervisedSections = new List<string>(supervisedSections);
    }

    public bool DayShift { get; }

    public IReadOnlyCollection<string> SupervisedSections => _supervisedSections.AsReadOnly();

    public void prepareSpecialists(Dish dish) { }

    public void AssistExecutiveChef() { }
}

public class LineChef : Chef
{
    [JsonInclude]
    private readonly List<string> _tasksAssigned;

    [JsonConstructor]
    public LineChef(
        string firstName,
        string lastName,
        DateOnly birthDate,
        string phoneNumber,
        WorkDetails workDetails,
        EmployeeExperienceProfile experienceProfile,
        string cuisineType,
        string specialization,
        IReadOnlyCollection<string> tasksAssigned)
        : base(firstName, lastName, birthDate, phoneNumber, workDetails, experienceProfile, cuisineType)
    {
        Specialization = specialization;
        _tasksAssigned = new List<string>(tasksAssigned);
    }

    public string Specialization { get; }

    public IReadOnlyCollection<string> TasksAssigned => _tasksAssigned.AsReadOnly();

    public void CookSpecialtyDish(Dish dish) { }

    public void FollowSousChefInstructions() { }
}

public class Waiter : Employee
{
    [JsonInclude]
    private readonly List<Table> _assignedTables = new();

    [JsonConstructor]
    public Waiter(
        string firstName,
        string lastName,
        DateOnly birthDate,
        string phoneNumber,
        WorkDetails workDetails,
        EmployeeExperienceProfile experienceProfile)
        : base(firstName, lastName, birthDate, phoneNumber, workDetails, experienceProfile)
    {
    }

    public IReadOnlyCollection<Table> AssignedTables => _assignedTables;

    public bool AssignTable(Table table)
    {
        if (_assignedTables.Contains(table))
        {
            return false;
        }

        _assignedTables.Add(table);
        return true;
    }
}

public class Valet : Employee
{
    [JsonConstructor]
    public Valet(
        string firstName,
        string lastName,
        DateOnly birthDate,
        string phoneNumber,
        WorkDetails workDetails,
        EmployeeExperienceProfile experienceProfile,
        string assignedLocation)
        : base(firstName, lastName, birthDate, phoneNumber, workDetails, experienceProfile)
    {
        AssignedLocation = assignedLocation;
    }

    public string AssignedLocation { get; }
}

