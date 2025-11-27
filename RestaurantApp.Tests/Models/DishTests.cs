using NUnit.Framework;
using RestaurantApp.Models;
using System;
using System.Collections.Generic;

namespace RestaurantApp.Tests.Models
{
    [TestFixture]
    public class DishTests
    {
        // ----------------------------------------------------
        // CONSTRUCTOR TESTS
        // ----------------------------------------------------

        [Test]
        public void Constructor_ShouldSetAllPropertiesCorrectly()
        {
            var ing = new List<string> { "Salt", "Pepper" };

            var dish = new Dish("Pasta", "Italian", true, false, 25.5m, ing);

            Assert.That(dish.Name, Is.EqualTo("Pasta"));
            Assert.That(dish.Cuisine, Is.EqualTo("Italian"));
            Assert.That(dish.IsVegetarian, Is.True);
            Assert.That(dish.IsVegan, Is.False);
            Assert.That(dish.Price, Is.EqualTo(25.5m));
            Assert.That(dish.Ingredients, Is.EqualTo(ing));
        }

        [Test]
        public void Constructor_ShouldThrow_WhenNameIsEmpty()
        {
            Assert.Throws<ArgumentException>(() =>
                new Dish("", "Italian", true, false, 10m, new List<string> { "Salt" })
            );
        }

        [Test]
        public void Constructor_ShouldThrow_WhenCuisineIsEmpty()
        {
            Assert.Throws<ArgumentException>(() =>
                new Dish("Pasta", "", true, false, 10m, new List<string> { "Salt" })
            );
        }

        [Test]
        public void Constructor_ShouldThrow_WhenPriceIsInvalid()
        {
            Assert.Throws<ArgumentException>(() =>
                new Dish("Pasta", "Italian", true, false, 0m, new List<string> { "Salt" })
            );
        }

        [Test]
        public void Constructor_ShouldThrow_WhenIngredientListIsEmpty()
        {
            Assert.Throws<ArgumentException>(() =>
                new Dish("Pasta", "Italian", true, false, 10m, new List<string>())
            );
        }

        [Test]
        public void Constructor_ShouldThrow_WhenIngredientContainsEmptyItem()
        {
            Assert.Throws<ArgumentException>(() =>
                new Dish("Pasta", "Italian", true, false, 10m, new List<string> { "Salt", "" })
            );
        }

        // ----------------------------------------------------
        // CREATE DISH
        // ----------------------------------------------------

        [Test]
        public void CreateDish_ShouldReturnValidDish()
        {
            var ing = new List<string> { "Tomato" };

            var dish = Dish.CreateDish("Soup", "Turkish", true, false, 15m, ing);

            Assert.That(dish.Name, Is.EqualTo("Soup"));
            Assert.That(dish.Cuisine, Is.EqualTo("Turkish"));
            Assert.That(dish.IsVegetarian, Is.True);
            Assert.That(dish.IsVegan, Is.False);
            Assert.That(dish.Price, Is.EqualTo(15m));
            Assert.That(dish.Ingredients, Is.EqualTo(ing));
        }

        // ----------------------------------------------------
        // CHANGE DISH
        // ----------------------------------------------------

        [Test]
        public void ChangeDish_ShouldUpdateOnlyProvidedFields()
        {
            var dish = new Dish("Burger", "American", false, false, 30m, new List<string> { "Meat" });

            dish.ChangeDish(newName: "Vegan Burger", newIsVegan: true);

            Assert.That(dish.Name, Is.EqualTo("Vegan Burger"));
            Assert.That(dish.IsVegan, Is.True);

            // unchanged fields
            Assert.That(dish.Cuisine, Is.EqualTo("American"));
            Assert.That(dish.IsVegetarian, Is.False);
            Assert.That(dish.Price, Is.EqualTo(30m));
        }

        [Test]
        public void ChangeDish_ShouldUpdateAllFields()
        {
            var dish = new Dish("Burger", "American", false, false, 30m, new List<string> { "Meat" });

            var newIngredients = new List<string> { "Beans", "Lettuce" };

            dish.ChangeDish(
                newName: "Vegan Burger",
                newCuisine: "Vegan",
                newIsVegetarian: true,
                newIsVegan: true,
                newPrice: 40m,
                newIngredients: newIngredients
            );

            Assert.That(dish.Name, Is.EqualTo("Vegan Burger"));
            Assert.That(dish.Cuisine, Is.EqualTo("Vegan"));
            Assert.That(dish.IsVegetarian, Is.True);
            Assert.That(dish.IsVegan, Is.True);
            Assert.That(dish.Price, Is.EqualTo(40m));
            Assert.That(dish.Ingredients, Is.EqualTo(newIngredients));
        }

        // ----------------------------------------------------
        // REMOVE DISH
        // ----------------------------------------------------

        [Test]
        public void RemoveDish_ShouldRemoveItFromCollection()
        {
            var list = new List<Dish>();
            var dish = new Dish("Salad", "Greek", true, false, 18m, new List<string> { "Lettuce" });

            list.Add(dish);

            dish.RemoveDish(ref list);

            Assert.That(list.Contains(dish), Is.False);
        }
    }
}
