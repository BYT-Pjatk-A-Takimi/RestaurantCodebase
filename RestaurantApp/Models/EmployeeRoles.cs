using System;
using System.Collections.Generic;

namespace RestaurantApp.Models;

public class Manager : Employee
{
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

public class Chef : Employee
{
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
        SupervisedSections = new List<string>(supervisedSections);
    }

    public bool DayShift { get; }

    public IReadOnlyCollection<string> SupervisedSections { get; }

    public void prepareSpecialists(Dish dish) { }

    public void AssistExecutiveChef() { }
}

public class LineChef : Chef
{
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
        TasksAssigned = new List<string>(tasksAssigned);
    }

    public string Specialization { get; }

    public IReadOnlyCollection<string> TasksAssigned { get; }

    public void CookSpecialtyDish(Dish dish) { }

    public void FollowSousChefInstructions() { }
}

public class Waiter : Employee
{
    private readonly List<Table> _assignedTables = new();

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

