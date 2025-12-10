using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace RestaurantApp.Models;

public class Manager : Employee
{
    [JsonInclude]
    private readonly List<Manager> _subordinates = new();

    [JsonConstructor]
    public Manager(
        string firstName,
        string lastName,
        DateOnly birthDate,
        string phoneNumber,
        WorkDetails workDetails,
        EmployeeExperienceProfile experienceProfile,
        int level,
        Manager? supervisor = null)
        : base(firstName, lastName, birthDate, phoneNumber, workDetails, experienceProfile)
    {
        Level = level;
        if (supervisor != null)
        {
            SetSupervisor(supervisor);
        }
    }

    public int Level { get; }

    public Manager? Supervisor { get; private set; }

    public IReadOnlyCollection<Manager> Subordinates => _subordinates;

    public bool AssignTable(Waiter waiter, Table table) => waiter.AssignTable(table);

    public void SetSupervisor(Manager supervisor)
    {
        if (supervisor is null)
            throw new ArgumentNullException(nameof(supervisor));

        if (supervisor == this)
            throw new ArgumentException("A manager cannot supervise themselves.", nameof(supervisor));

        if (supervisor == Supervisor)
            throw new ArgumentException("This manager is already supervised by the specified supervisor.", nameof(supervisor));

        // Check for circular reference
        if (WouldCreateCircularReference(supervisor))
            throw new InvalidOperationException("Setting this supervisor would create a circular reference.");

        if (Supervisor != null)
        {
            Supervisor._subordinates.Remove(this);
        }

        Supervisor = supervisor;
        if (!supervisor._subordinates.Contains(this))
        {
            supervisor._subordinates.Add(this);
        }
    }

    public void AddSubordinate(Manager subordinate)
    {
        if (subordinate is null)
            throw new ArgumentNullException(nameof(subordinate));

        if (subordinate == this)
            throw new ArgumentException("A manager cannot be their own subordinate.", nameof(subordinate));

        if (_subordinates.Contains(subordinate))
            throw new ArgumentException("This manager is already a subordinate.", nameof(subordinate));

        // Check for circular reference
        if (subordinate.WouldCreateCircularReference(this))
            throw new InvalidOperationException("Adding this subordinate would create a circular reference.");

        _subordinates.Add(subordinate);
        if (subordinate.Supervisor != this)
        {
            subordinate.Supervisor = this;
        }
    }

    public bool RemoveSubordinate(Manager subordinate)
    {
        if (subordinate is null)
            throw new ArgumentNullException(nameof(subordinate));

        if (!_subordinates.Contains(subordinate))
            return false;

        _subordinates.Remove(subordinate);
        if (subordinate.Supervisor == this)
        {
            subordinate.Supervisor = null;
        }

        return true;
    }

    private bool WouldCreateCircularReference(Manager potentialSupervisor)
    {        
        var current = this.Supervisor;
        var visited = new HashSet<Manager> { this };
        
        while (current != null && !visited.Contains(current))
        {
            if (current == potentialSupervisor)
                return true; 
            visited.Add(current);
            current = current.Supervisor;
        }
        
        return IsInSubordinateChain(potentialSupervisor, this, new HashSet<Manager>());
    }
    
    private static bool IsInSubordinateChain(Manager manager, Manager target, HashSet<Manager> visited)
    {
        if (manager == target)
            return true;
            
        if (visited.Contains(manager))
            return false;
            
        visited.Add(manager);
        
        foreach (var subordinate in manager._subordinates)
        {
            if (IsInSubordinateChain(subordinate, target, visited))
                return true;
        }
        
        return false;
    }
}

[JsonPolymorphic]
[JsonDerivedType(typeof(HeadChef), typeDiscriminator: "HeadChef")]
[JsonDerivedType(typeof(SousChef), typeDiscriminator: "SousChef")]
[JsonDerivedType(typeof(LineChef), typeDiscriminator: "LineChef")]
public class Chef : Employee
{
    [JsonInclude]
    private readonly List<Restaurant> _restaurants = new();

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

    public IReadOnlyCollection<Restaurant> Restaurants => _restaurants;

    public void assignTask() {}

    public void AddDish(Menu menu, Dish dish) => menu.AddDish(dish);

    public void viewOrders() {}

    public IReadOnlyCollection<Dish> ViewMenu(Menu menu) => menu.Dishes;

    public void UpdateMenu(Menu menu, Dish existingDish, Dish updatedDish) => menu.UpdateDish(existingDish, updatedDish);

    public void AddRestaurant(Restaurant restaurant)
    {
        if (restaurant is null)
            throw new ArgumentNullException(nameof(restaurant));

        if (_restaurants.Contains(restaurant))
            throw new ArgumentException("This chef already works at this restaurant.", nameof(restaurant));

        _restaurants.Add(restaurant);
        restaurant.AddChef(this);
    }

    public bool RemoveRestaurant(Restaurant restaurant)
    {
        if (restaurant is null)
            throw new ArgumentNullException(nameof(restaurant));

        if (!_restaurants.Contains(restaurant))
            return false;

        _restaurants.Remove(restaurant);
        restaurant.RemoveChef(this);
        return true;
    }
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
        if (table is null)
            throw new ArgumentNullException(nameof(table));

        if (_assignedTables.Contains(table))
        {
            return false;
        }

        _assignedTables.Add(table);
        table.SetWaiter(this);
        return true;
    }

    public bool RemoveTable(Table table)
    {
        if (table is null)
            throw new ArgumentNullException(nameof(table));

        if (!_assignedTables.Contains(table))
            return false;

        _assignedTables.Remove(table);
        if (table.Waiter == this)
        {
            table.SetWaiter(null);
        }

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

