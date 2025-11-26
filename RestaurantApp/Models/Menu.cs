using System;
using System.Collections.Generic;
using System.Linq;

namespace RestaurantApp.Models
{
    public class Menu
    {
        // -------------------------
        // BASIC ATTRIBUTES (UML)
        // -------------------------
        private string _name;
        public string Name
        {
            get => _name;
            private set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Menu name cannot be empty.");
                _name = value;
            }
        }

        private string _menuType;
        public string MenuType
        {
            get => _menuType;
            private set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Menu type cannot be empty.");
                _menuType = value;
            }
        }

        // -------------------------
        // MULTI-VALUE ATTRIBUTE (UML)
        // -------------------------
        private List<string> _availableLanguages = new List<string>();
        public IReadOnlyCollection<string> AvailableLanguages => _availableLanguages.AsReadOnly();

        private void SetLanguages(IEnumerable<string> langs)
        {
            if (langs == null)
                throw new ArgumentException("Language list cannot be null.");

            var list = langs.ToList();

            if (list.Count == 0)
                throw new ArgumentException("There must be at least one language.");

            foreach (var l in list)
            {
                if (string.IsNullOrWhiteSpace(l))
                    throw new ArgumentException("Language cannot be empty.");
            }

            _availableLanguages = list;
        }

        // -------------------------
        // ASSOCIATION (Menu → Dish)
        // -------------------------
        private List<Dish> _dishes = new List<Dish>();
        public IReadOnlyCollection<Dish> Dishes => _dishes.AsReadOnly();

        public void AddDish(Dish dish)
        {
            if (dish == null)
                throw new ArgumentException("Dish cannot be null.");

            _dishes.Add(dish);
        }

        public bool RemoveDish(Dish dish)
        {
            if (dish == null)
                throw new ArgumentException("Dish cannot be null.");

            return _dishes.Remove(dish);
        }

        // -------------------------
        // UML METHODS
        // -------------------------
        public void ChangeMenu(string? newName = null, string? newType = null)
        {
            if (newName != null) Name = newName;
            if (newType != null) MenuType = newType;
        }

        public int GetNumberOfPositions()
        {
            return _dishes.Count;
        }

        // -------------------------
        // CONSTRUCTOR
        // -------------------------
        public Menu(string name, string menuType, IEnumerable<string> availableLanguages)
        {
            Name = name;
            MenuType = menuType;
            SetLanguages(availableLanguages);
        }
    }
}
