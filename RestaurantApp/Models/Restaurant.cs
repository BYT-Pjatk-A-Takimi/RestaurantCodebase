using System.Collections.Generic;
using System.Linq;

namespace RestaurantApp.Models;

public class Restaurant
{
    private readonly List<Table> _tables = new();
    private readonly List<Menu> _menus = new();

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

