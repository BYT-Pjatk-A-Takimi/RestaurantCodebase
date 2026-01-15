using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace RestaurantApp.Models.Roles.EmployeeTypes;

public class ManagerType : EmployeeType
{
    [JsonInclude]
    private readonly List<ManagerType> _subordinates = new();
    [JsonInclude]
    private Restaurant? _restaurant;

    private int _level;

    [JsonConstructor]
    public ManagerType(int level, ManagerType? supervisor = null)
    {
        Level = level;
        if (supervisor != null)
        {
            SetSupervisor(supervisor);
        }
    }

    public int Level
    {
        get => _level;
        private set
        {
            if (value <= 0)
                throw new ArgumentException("Level must be positive.", nameof(Level));
            _level = value;
        }
    }

    [JsonIgnore]
    public ManagerType? Supervisor { get; private set; }

    public IReadOnlyCollection<ManagerType> Subordinates => _subordinates;

    public Restaurant? Restaurant => _restaurant;

    public bool AssignTable(WaiterType waiter, Table table) => waiter.AssignTable(table);

    public void SetSupervisor(ManagerType supervisor)
    {
        if (supervisor is null)
            throw new ArgumentNullException(nameof(supervisor));

        if (supervisor == this)
            throw new ArgumentException("A manager cannot supervise themselves.", nameof(supervisor));

        if (supervisor == Supervisor)
        {
            // Even if it's already the supervisor, ensure the reverse link exists
            if (!supervisor.Subordinates.Contains(this))
            {
                supervisor.AddSubordinate(this);
            }
            return;
        }

        if (supervisor == Supervisor)
            throw new ArgumentException("This manager is already supervised by the specified supervisor.", nameof(supervisor));

        if (WouldCreateCircularReference(supervisor))
            throw new InvalidOperationException("Setting this supervisor would create a circular reference.");

        if (Supervisor != null)
        {
            Supervisor.RemoveSubordinate(this);
        }

        Supervisor = supervisor;
        if (!supervisor.Subordinates.Contains(this))
        {
            supervisor.AddSubordinate(this);
        }
    }

    public void AddSubordinate(ManagerType subordinate)
    {
        if (subordinate is null)
            throw new ArgumentNullException(nameof(subordinate));

        if (subordinate == this)
            throw new ArgumentException("A manager cannot be their own subordinate.", nameof(subordinate));

        if (_subordinates.Contains(subordinate))
            return;

        if (subordinate.Supervisor != this)
        {
            subordinate.SetSupervisor(this);
            return;
        }

        _subordinates.Add(subordinate);
    }

    public bool RemoveSubordinate(ManagerType subordinate)
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

    private bool WouldCreateCircularReference(ManagerType potentialSupervisor)
    {
        var current = this.Supervisor;
        var visited = new HashSet<ManagerType> { this };

        while (current != null && !visited.Contains(current))
        {
            if (current == potentialSupervisor)
                return true;
            visited.Add(current);
            current = current.Supervisor;
        }

        return IsInSubordinateChain(potentialSupervisor, this, new HashSet<ManagerType>());
    }

    private static bool IsInSubordinateChain(ManagerType manager, ManagerType target, HashSet<ManagerType> visited)
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
            return;
            
        if (_restaurant != null)
            throw new InvalidOperationException("Manager already works at a restaurant. Remove the current restaurant first.");

        _restaurant = restaurant;
        if (!restaurant.Managers.Contains(this))
        {
            restaurant.AddManager(this);
        }
    }

    internal void SetRestaurantInternal(Restaurant restaurant)
    {
        if (restaurant is null)
            throw new ArgumentNullException(nameof(restaurant));
            
        if (_restaurant == restaurant)
            return;

        _restaurant = restaurant;
        if (!restaurant.Managers.Contains(this))
        {
            restaurant.AddManager(this);
        }
    }

    public void RemoveRestaurant()
    {
        _restaurant = null;
    }
}
