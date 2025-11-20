using NUnit.Framework;
using RestaurantApp.Models;
using System.Collections.Generic;

namespace RestaurantApp.Tests.Models
{
    public class DishTests
    {
        [Test]
        public void Constructor_ShouldSetPropertiesCorrectly()
        {
            // Arrange
            var ingredients = new List<string> { "Salt", "Pepper" };

            // Act
            var dish = new Dish("Pasta", "Italian", true, 25.5m, ingredients);

            // Assert
            Assert.AreEqual("Pasta", dish.Name);
            Assert.AreEqual("Italian", dish.Cuisine);
            Assert.IsTrue(dish.IsVegetarian);
            Assert.AreEqual(25.5m, dish.Price);
            CollectionAssert.AreEquivalent(ingredients, dish.Ingredients);
        }

        [Test]
        public void Ingredients_ShouldBeCopied_NotReference()
        {
            // Arrange
            var ingredients = new List<string> { "Sugar" };
            var dish = new Dish("Cake", "Dessert", false, 12m, ingredients);

            // Act
            ingredients.Add("Vanilla");

            // Assert
            Assert.AreEqual(1, dish.Ingredients.Count); // dışarıdaki listeden etkilenmemeli
        }

        [Test]
        public void WithUpdatedPrice_ShouldReturnNewDishWithNewPrice()
        {
            // Arrange
            var dish = new Dish("Soup", "Turkish", true, 15m, new List<string> { "Water" });

            // Act
            var updated = dish.WithUpdatedPrice(20m);

            // Assert
            Assert.AreEqual(20m, updated.Price);
            Assert.AreEqual(15m, dish.Price);          // orijinal değişmemeli
            Assert.AreEqual(dish.Name, updated.Name);
            Assert.AreEqual(dish.Cuisine, updated.Cuisine);
            Assert.AreEqual(dish.IsVegetarian, updated.IsVegetarian);
        }
    }
}