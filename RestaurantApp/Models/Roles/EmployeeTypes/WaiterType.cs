using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace RestaurantApp.Models.Roles.EmployeeTypes;

public class WaiterType : EmployeeType
{
    [JsonInclude]
    private readonly List<Table> _assignedTables = new();

    [JsonConstructor]
    public WaiterType()
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
        if (table.Waiter != this)
        {
            table.SetWaiter(this);
        }
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
