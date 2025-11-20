using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace RestaurantApp.Models;

public class Menu
{
    [JsonInclude]
    private readonly List<Dish> _dishes = new();

    [JsonConstructor]
    public Menu(string name, string menuType, IEnumerable<string> availableLanguages)
    {
        Name = name;
        MenuType = menuType;
        AvailableLanguages = new List<string>(availableLanguages);
    }

    public string Name { get; }

    public string MenuType { get; }

    public IReadOnlyCollection<string> AvailableLanguages { get; }

    public IReadOnlyCollection<Dish> Dishes => _dishes;

    public void AddDish(Dish dish)
    {
        if (_dishes.All(d => d.Name != dish.Name))
        {
            _dishes.Add(dish);
        }
    }

    public bool RemoveDish(Dish dish) => _dishes.Remove(dish);

    public void UpdateDish(Dish existingDish, Dish updatedDish)
    {
        var index = _dishes.IndexOf(existingDish);
        if (index >= 0)
        {
            _dishes[index] = updatedDish;
        }
    }
}

