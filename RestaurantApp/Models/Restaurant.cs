using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RestaurantApp.Models;

public class Restaurant
{
    private static List<Restaurant> _extent = new();
    public static IReadOnlyCollection<Restaurant> Extent => _extent.AsReadOnly();

    private readonly List<Table> _tables = new();
    private readonly List<Menu> _menus = new();

    private Restaurant()
    {
        _tables = new List<Table>();
        _menus = new List<Menu>();
    }

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

    private static void AddToExtent(Restaurant restaurant)
    {
        if (restaurant is null)
            throw new ArgumentNullException(nameof(restaurant));

        _extent.Add(restaurant);
    }

    public static void Save(string path = "restaurants_extent.json")
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles
        };

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
            var loaded = JsonSerializer.Deserialize<List<Restaurant>>(json);

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
    }

    public bool RemoveMenu(Menu menu)
    {
        if (menu is null)
            throw new ArgumentNullException(nameof(menu));

        return _menus.Remove(menu);
    }
}
