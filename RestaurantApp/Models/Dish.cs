using System;
using System.Collections.Generic;

namespace RestaurantApp.Models
{
    public class Dish
    {
        // -------------------------
        // BASIC UML ATTRIBUTES
        // -------------------------
        private string _name;
        public string Name
        {
            get => _name;
            private set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Name cannot be empty.");
                _name = value;
            }
        }

        private string _cuisine;
        public string Cuisine
        {
            get => _cuisine;
            private set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Cuisine cannot be empty.");
                _cuisine = value;
            }
        }

        public bool IsVegetarian { get; private set; }

        public bool IsVegan { get; private set; }

        private decimal _price;
        public decimal Price
        {
            get => _price;
            private set
            {
                if (value <= 0)
                    throw new ArgumentException("Price must be positive.");
                _price = value;
            }
        }

        // ingredients [1..*]
        private List<string> _ingredients = new List<string>();
        public IReadOnlyCollection<string> Ingredients => _ingredients.AsReadOnly();

        private void SetIngredients(IEnumerable<string> list)
        {
            if (list == null)
                throw new ArgumentException("Ingredients list cannot be null.");

            var temp = new List<string>();

            foreach (var item in list)
            {
                if (string.IsNullOrWhiteSpace(item))
                    throw new ArgumentException("Ingredient cannot be empty.");
                temp.Add(item);
            }

            if (temp.Count == 0)
                throw new ArgumentException("Ingredients must contain at least one item.");

            _ingredients = temp;
        }

        // -------------------------
        // CONSTRUCTOR
        // -------------------------
        public Dish(string name, string cuisine, bool isVegetarian, bool isVegan, decimal price, IEnumerable<string> ingredients)
        {
            Name = name;
            Cuisine = cuisine;
            IsVegetarian = isVegetarian;
            IsVegan = isVegan;
            Price = price;
            SetIngredients(ingredients);
        }

        // -------------------------
        // UML METHODS
        // -------------------------

        // createDish()
        public static Dish CreateDish(string name, string cuisine, bool isVegetarian, bool isVegan, decimal price, IEnumerable<string> ingredients)
        {
            return new Dish(name, cuisine, isVegetarian, isVegan, price, ingredients);
        }

        // changeDish()
        public void ChangeDish(string? newName = null,
                               string? newCuisine = null,
                               bool? newIsVegetarian = null,
                               bool? newIsVegan = null,
                               decimal? newPrice = null,
                               IEnumerable<string>? newIngredients = null)
        {
            if (newName != null) Name = newName;
            if (newCuisine != null) Cuisine = newCuisine;
            if (newIsVegetarian != null) IsVegetarian = newIsVegetarian.Value;
            if (newIsVegan != null) IsVegan = newIsVegan.Value;
            if (newPrice != null) Price = newPrice.Value;
            if (newIngredients != null) SetIngredients(newIngredients);
        }

        // removeDish()
        public void RemoveDish(ref List<Dish> collection)
        {
            collection?.Remove(this);
        }
    }
}
