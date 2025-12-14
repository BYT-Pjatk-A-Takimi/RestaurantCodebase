using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace RestaurantApp.Models;

public class Manager : Employee
{
    [JsonInclude]
    private readonly List<Manager> _subordinates = new();
    [JsonInclude]
    private Restaurant? _restaurant;

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

    public Restaurant? Restaurant => _restaurant;

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

    public void SetRestaurant(Restaurant restaurant)
    {
        if (restaurant is null)
            throw new ArgumentNullException(nameof(restaurant));
        if (_restaurant == restaurant)
            throw new ArgumentException("This manager already works at this restaurant.", nameof(restaurant));
        if (_restaurant != null)
            throw new InvalidOperationException("Manager already works at a restaurant. Remove the current restaurant first.");

        _restaurant = restaurant;
        restaurant.AddManager(this);
    }

    internal void SetRestaurantInternal(Restaurant restaurant)
    {
        if (restaurant is null)
            throw new ArgumentNullException(nameof(restaurant));
        _restaurant = restaurant;
    }

    public void RemoveRestaurant()
    {
        if (_restaurant is null)
            return;
        var restaurant = _restaurant;
        _restaurant = null;
        restaurant.RemoveManager(this);
    }
}
