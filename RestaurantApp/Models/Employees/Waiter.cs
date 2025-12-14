using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace RestaurantApp.Models;

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
