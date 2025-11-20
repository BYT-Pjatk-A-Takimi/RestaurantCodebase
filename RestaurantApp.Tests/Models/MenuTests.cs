using NUnit.Framework;
using RestaurantApp.Models;
using System.Collections.Generic;
using System.Linq;

namespace RestaurantApp.Tests.Models
{
    public class MenuTests
    {
        [Test]
        public void Constructor_ShouldSetPropertiesCorrectly()
        {
            // Arrange
            var languages = new List<string> { "EN", "PL" };

            // Act
            var menu = new Menu("Main Menu", "Dinner", languages);

            // Assert
            Assert.AreEqual("Main Menu", menu.Name);
            Assert.AreEqual("Dinner", menu.MenuType);
            CollectionAssert.AreEquivalent(languages, menu.AvailableLanguages);
            Assert.AreEqual(0, menu.Dishes.Count);
        }

        [Test]
        public void AddDish_ShouldAddDish_WhenNameIsUnique()
        {
            // Arrange
            var menu = new Menu("Main", "Lunch", new List<string> { "EN" });
            var dish = new Dish("Pasta", "Italian", true, 25m, new List<string> { "Salt" });

            // Act
            menu.AddDish(dish);

            // Assert
            Assert.AreEqual(1, menu.Dishes.Count);
            Assert.AreEqual("Pasta", menu.Dishes.First().Name);
        }

        [Test]
        public void AddDish_ShouldNotAdd_WhenNameAlreadyExists()
        {
            // Arrange
            var menu = new Menu("Main", "Lunch", new List<string> { "EN" });
            var dish1 = new Dish("Pasta", "Italian", true, 25m, new List<string> { "Salt" });
            var dish2 = new Dish("Pasta", "Italian", false, 30m, new List<string> { "Cheese" });

            // Act
            menu.AddDish(dish1);
            menu.AddDish(dish2);

            // Assert
            Assert.AreEqual(1, menu.Dishes.Count);
        }

        [Test]
        public void RemoveDish_ShouldReturnTrue_WhenDishExists()
        {
            // Arrange
            var menu = new Menu("Main", "Lunch", new List<string> { "EN" });
            var dish = new Dish("Pasta", "Italian", true, 25m, new List<string> { "Salt" });
            menu.AddDish(dish);

            // Act
            var result = menu.RemoveDish(dish);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(0, menu.Dishes.Count);
        }

        [Test]
        public void UpdateDish_ShouldReplaceExistingDish()
        {
            // Arrange
            var menu = new Menu("Main", "Lunch", new List<string> { "EN" });
            var oldDish = new Dish("Pasta", "Italian", true, 25m, new List<string> { "Salt" });
            var newDish = new Dish("Pasta", "Italian", false, 30m, new List<string> { "Salt", "Cheese" });

            menu.AddDish(oldDish);

            // Act
            menu.UpdateDish(oldDish, newDish);

            // Assert
            Assert.AreEqual(1, menu.Dishes.Count);
            Assert.AreEqual(30m, menu.Dishes.First().Price);
            Assert.IsFalse(menu.Dishes.First().IsVegetarian);
        }
    }
}