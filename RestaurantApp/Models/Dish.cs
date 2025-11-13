using System.Collections.Generic;

namespace RestaurantApp.Models;

public class Dish
{
    public Dish(string name, string cuisine, bool isVegetarian, decimal price, IEnumerable<string> ingredients)
    {
        Name = name;
        Cuisine = cuisine;
        IsVegetarian = isVegetarian;
        Price = price;
        Ingredients = new List<string>(ingredients);
    }

    public string Name { get; }

    public string Cuisine { get; }

    public bool IsVegetarian { get; }

    public decimal Price { get; private set; }

    public IReadOnlyCollection<string> Ingredients { get; }

    public Dish WithUpdatedPrice(decimal newPrice)
    {
        return new Dish(Name, Cuisine, IsVegetarian, newPrice, Ingredients);
    }
}

