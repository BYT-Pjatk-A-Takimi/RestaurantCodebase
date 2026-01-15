using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using RestaurantApp.Models.Roles.EmployeeTypes;

namespace RestaurantApp.Models;

public class Restaurant
{
    private static List<Restaurant> _extent = new();
    public static IReadOnlyCollection<Restaurant> Extent => _extent.AsReadOnly();

    [JsonInclude]
    private readonly List<Table> _tables = new();
    [JsonInclude]
    private readonly List<Menu> _menus = new();
    [JsonInclude]
    private readonly List<ChefType> _chefs = new();
    [JsonInclude]
    private readonly List<ManagerType> _managers = new();

    public Restaurant()
    {
        _tables = new List<Table>();
        _menus = new List<Menu>();
        _chefs = new List<ChefType>();
        _managers = new List<ManagerType>();
    }

    [JsonConstructor]
    public Restaurant(string name, int maxCapacity)
    {
        Name = name;
        MaxCapacity = maxCapacity;

        AddToExtent(this);
    }

    private string _name = string.Empty;
    public string Name
    {
        get => _name;
        private set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Restaurant name cannot be empty.", nameof(Name));
            _name = value;
        }
    }

    private int _maxCapacity;
    public int MaxCapacity
    {
        get => _maxCapacity;
        private set
        {
            if (value <= 0)
                throw new ArgumentOutOfRangeException(nameof(MaxCapacity), "Max capacity must be greater than 0.");
            _maxCapacity = value;
        }
    }

    [JsonIgnore]
    public IReadOnlyCollection<Table> Tables => _tables;

    [JsonIgnore]
    public IReadOnlyCollection<Menu> Menus => _menus;

    [JsonIgnore]
    public IReadOnlyCollection<ChefType> Chefs => _chefs;

    [JsonIgnore]
    public IReadOnlyCollection<ManagerType> Managers => _managers;

    private static void AddToExtent(Restaurant restaurant)
    {
        if (restaurant is null)
            throw new ArgumentNullException(nameof(restaurant));

        _extent.Add(restaurant);
    }

    public static void Save(string path = "restaurants_extent.json")
    {
        var options = JsonSerialization.GetDefaultOptions();
        var json = JsonSerializer.Serialize(_extent, options);
        File.WriteAllText(path, json);
    }

    public static bool Load(string path = "restaurants_extent.json")
    {
        if (!File.Exists(path))
        {
            _extent.Clear();
            return false;
        }

        try
        {
            var json = File.ReadAllText(path);
            var options = JsonSerialization.GetDefaultOptions();
            var loaded = JsonSerializer.Deserialize<List<Restaurant>>(json, options);

            if (loaded is null)
            {
                _extent.Clear();
                return false;
            }

            _extent = loaded;
            return true;
        }
        catch
        {
            _extent.Clear();
            return false;
        }
    }

    public void AddTable(Table table)
    {
        if (table is null)
            throw new ArgumentNullException(nameof(table));

        if (_tables.Contains(table))
            return;

        if (_tables.Any(t => t.TableNumber == table.TableNumber))
            throw new ArgumentException(
                $"Table with number {table.TableNumber} already exists in this restaurant.",
                nameof(table));

        _tables.Add(table);
        if (table.Restaurant != this)
        {
            table.SetRestaurant(this);
        }
    }

    public void RemoveTable(Table table)
    {
        if (table is null)
            throw new ArgumentNullException(nameof(table));

        if (!_tables.Contains(table))
            throw new ArgumentException("Table does not belong to this restaurant.", nameof(table));

        _tables.Remove(table);
    }

    public int GetNumberOfTables() => _tables.Count;

    public void AddMenu(Menu menu)
    {
        if (menu is null)
            throw new ArgumentNullException(nameof(menu));

        if (_menus.Contains(menu))
            return;

        if (_menus.Any(m => m.Name == menu.Name))
            throw new ArgumentException(
                $"Menu with name '{menu.Name}' already exists in this restaurant.",
                nameof(menu));

        _menus.Add(menu);
        if (menu.Restaurant != this)
        {
            menu.SetRestaurant(this);
        }
    }

    public bool RemoveMenu(Menu menu)
    {
        if (menu is null)
            throw new ArgumentNullException(nameof(menu));

        if (!_menus.Contains(menu))
            return false;

        if (_menus.Count <= 1)
            throw new InvalidOperationException("Cannot remove the last menu. A restaurant must have at least one menu.");

        var removed = _menus.Remove(menu);
        if (removed && menu.Restaurant == this)
        {
            menu.SetRestaurant(null);
        }

        return removed;
    }

    public void AddChef(ChefType chef)
    {
        if (chef is null)
            throw new ArgumentNullException(nameof(chef));

        if (_chefs.Contains(chef))
            return;

        if (chef.Restaurant != this && chef.Restaurant != null)
            throw new ArgumentException("Chef already belongs to another restaurant.", nameof(chef));

        _chefs.Add(chef);
        if (chef.Restaurant != this)
        {
            chef.SetRestaurantInternal(this);
        }
    }

    public void RemoveChef(ChefType chef)
    {
        if (chef is null)
            throw new ArgumentNullException(nameof(chef));

        if (_chefs.Remove(chef))
        {
            if (chef.Restaurant == this)
            {
                chef.RemoveRestaurant();
            }
        }
    }

    public void AddManager(ManagerType manager)
    {
        if (manager is null)
            throw new ArgumentNullException(nameof(manager));

        if (_managers.Contains(manager))
            return;

        if (manager.Restaurant != this && manager.Restaurant != null)
            throw new ArgumentException("Manager already belongs to another restaurant.", nameof(manager));

        _managers.Add(manager);
        if (manager.Restaurant != this)
        {
            manager.SetRestaurantInternal(this);
        }
    }

    public void RemoveManager(ManagerType manager)
    {
        if (manager is null)
            throw new ArgumentNullException(nameof(manager));

        if (_managers.Remove(manager))
        {
            if (manager.Restaurant == this)
            {
                manager.RemoveRestaurant();
            }
        }
    }
}
