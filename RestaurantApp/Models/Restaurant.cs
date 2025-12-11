using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using RestaurantApp;

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
    private readonly List<Chef> _chefs = new();
    [JsonInclude]
    private readonly List<Manager> _managers = new();

    public Restaurant()
    {
        _tables = new List<Table>();
        _menus = new List<Menu>();
        _chefs = new List<Chef>();
        _managers = new List<Manager>();
    }

    [JsonConstructor]
    public Restaurant(string name, int maxCapacity)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Restaurant name cannot be empty.", nameof(name));

        if (maxCapacity <= 0)
            throw new ArgumentOutOfRangeException(nameof(maxCapacity), "Max capacity must be greater than 0.");

        Name = name;
        MaxCapacity = maxCapacity;

        AddToExtent(this);
    }

    public string Name { get; private set; } = string.Empty;
    public int MaxCapacity { get; private set; }

    [JsonIgnore]
    public IReadOnlyCollection<Table> Tables => _tables;

    [JsonIgnore]
    public IReadOnlyCollection<Menu> Menus => _menus;

    [JsonIgnore]
    public IReadOnlyCollection<Chef> Chefs => _chefs;

    [JsonIgnore]
    public IReadOnlyCollection<Manager> Managers => _managers;

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

        if (table.Restaurant != this)
            throw new ArgumentException("Table already belongs to another restaurant.", nameof(table));

        if (_tables.Contains(table))
            return;

        if (_tables.Any(t => t.TableNumber == table.TableNumber))
            throw new ArgumentException(
                $"Table with number {table.TableNumber} already exists in this restaurant.",
                nameof(table));

        _tables.Add(table);
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

        if (_menus.Any(m => m.Name == menu.Name))
            throw new ArgumentException(
                $"Menu with name '{menu.Name}' already exists in this restaurant.",
                nameof(menu));

        _menus.Add(menu);
        menu.SetRestaurant(this);
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

    internal void AddChef(Chef chef)
    {
        if (chef is null)
            throw new ArgumentNullException(nameof(chef));

        if (chef.Restaurant != this && chef.Restaurant != null)
            throw new ArgumentException("Chef already belongs to another restaurant.", nameof(chef));

        if (!_chefs.Contains(chef))
        {
            _chefs.Add(chef);
            chef.SetRestaurantInternal(this);
        }
    }

    internal void RemoveChef(Chef chef)
    {
        if (chef is null)
            throw new ArgumentNullException(nameof(chef));

        _chefs.Remove(chef);
    }

    internal void AddManager(Manager manager)
    {
        if (manager is null)
            throw new ArgumentNullException(nameof(manager));

        if (manager.Restaurant != this && manager.Restaurant != null)
            throw new ArgumentException("Manager already belongs to another restaurant.", nameof(manager));

        if (!_managers.Contains(manager))
        {
            _managers.Add(manager);
            manager.SetRestaurantInternal(this);
        }
    }

    internal void RemoveManager(Manager manager)
    {
        if (manager is null)
            throw new ArgumentNullException(nameof(manager));

        _managers.Remove(manager);
    }
}
