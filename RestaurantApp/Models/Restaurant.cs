using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace RestaurantApp.Models;

public class Restaurant
{
    [JsonInclude]
    private readonly List<Table> _tables = new();
    
    [JsonInclude]
    private readonly List<Menu> _menus = new();

    [JsonConstructor]
    public Restaurant(string name, int maxCapacity)
    {
        Name = name;
        MaxCapacity = maxCapacity;
    }

    public string Name { get; }

    public int MaxCapacity { get; }

    public IReadOnlyCollection<Table> Tables => _tables;

    public IReadOnlyCollection<Menu> Menus => _menus;

    public void AddTable(Table table)
    {
        if (_tables.All(t => t.TableNumber != table.TableNumber))
        {
            _tables.Add(table);
        }
    }

    public void RemoveTable(Table table) => _tables.Remove(table);

    public int GetNumberOfTables() => _tables.Count;

    public void AddMenu(Menu menu)
    {
        if (_menus.All(m => m.Name != menu.Name))
        {
            _menus.Add(menu);
        }
    }

    public bool RemoveMenu(Menu menu) => _menus.Remove(menu);
}

