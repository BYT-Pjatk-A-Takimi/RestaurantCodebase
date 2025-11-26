using NUnit.Framework;
using RestaurantApp.Models;
using System;
using System.Collections.Generic;

namespace RestaurantApp.Tests.Models
{
    [TestFixture]
    public class MenuTests
    {
        // ----------------------------------------------------
        // CONSTRUCTOR TESTS
        // ----------------------------------------------------

        [Test]
        public void Constructor_ShouldSetAllPropertiesCorrectly()
        {
            var langs = new List<string> { "EN", "PL" };

            var menu = new Menu("Main Menu", "Dinner", langs);

            Assert.That(menu.Name, Is.EqualTo("Main Menu"));
            Assert.That(menu.MenuType, Is.EqualTo("Dinner"));
            CollectionAssert.AreEqual(langs, menu.AvailableLanguages);
            Assert.That(menu.Dishes.Count, Is.EqualTo(0));
        }

        [Test]
        public void Constructor_ShouldThrow_WhenNameInvalid()
        {
            Assert.Throws<ArgumentException>(() =>
                new Menu("", "Dinner", new List<string> { "EN" })
            );
        }

        [Test]
        public void Constructor_ShouldThrow_WhenMenuTypeInvalid()
        {
            Assert.Throws<ArgumentException>(() =>
                new Menu("Main Menu", "", new List<string> { "EN" })
            );
        }

        [Test]
        public void Constructor_ShouldThrow_WhenLanguagesNull()
        {
            Assert.Throws<ArgumentException>(() =>
                new Menu("Menu", "Lunch", null!)
            );
        }

        [Test]
        public void Constructor_ShouldThrow_WhenLanguagesEmpty()
        {
            Assert.Throws<ArgumentException>(() =>
                new Menu("Menu", "Lunch", new List<string>())
            );
        }

        [Test]
        public void Constructor_ShouldThrow_WhenLanguageContainsEmptyString()
        {
            Assert.Throws<ArgumentException>(() =>
                new Menu("Menu", "Lunch", new List<string> { "EN", "" })
            );
        }

        // ----------------------------------------------------
        // ADD DISH
        // ----------------------------------------------------

        [Test]
        public void AddDish_ShouldAddDishToList()
        {
            var menu = new Menu("Main", "Dinner", new List<string> { "EN" });
            var dish = new Dish("Pasta", "Italian", true, false, 20m, new List<string> { "Salt" });

            menu.AddDish(dish);

            Assert.That(menu.Dishes.Count, Is.EqualTo(1));
            Assert.That(menu.Dishes, Contains.Item(dish));
        }

        [Test]
        public void AddDish_ShouldThrow_WhenDishIsNull()
        {
            var menu = new Menu("Main", "Dinner", new List<string> { "EN" });

            Assert.Throws<ArgumentException>(() =>
                menu.AddDish(null!)
            );
        }

        // ----------------------------------------------------
        // REMOVE DISH
        // ----------------------------------------------------

        [Test]
        public void RemoveDish_ShouldRemoveDishFromList()
        {
            var menu = new Menu("Main", "Dinner", new List<string> { "EN" });
            var dish = new Dish("Soup", "Turkish", true, false, 10m, new List<string> { "Water" });

            menu.AddDish(dish);
            var result = menu.RemoveDish(dish);

            Assert.That(result, Is.True);
            Assert.That(menu.Dishes.Count, Is.EqualTo(0));
        }

        [Test]
        public void RemoveDish_ShouldThrow_WhenDishIsNull()
        {
            var menu = new Menu("Main", "Dinner", new List<string> { "EN" });

            Assert.Throws<ArgumentException>(() =>
                menu.RemoveDish(null!)
            );
        }

        // ----------------------------------------------------
        // CHANGE MENU
        // ----------------------------------------------------

        [Test]
        public void ChangeMenu_ShouldUpdateOnlyProvidedFields()
        {
            var menu = new Menu("Main", "Dinner", new List<string> { "EN" });

            menu.ChangeMenu(newName: "Lunch Menu");

            Assert.That(menu.Name, Is.EqualTo("Lunch Menu"));
            Assert.That(menu.MenuType, Is.EqualTo("Dinner")); // unchanged
        }

        [Test]
        public void ChangeMenu_ShouldUpdateBothFields()
        {
            var menu = new Menu("Main", "Dinner", new List<string> { "EN" });

            menu.ChangeMenu(newName: "Special Menu", newType: "Brunch");

            Assert.That(menu.Name, Is.EqualTo("Special Menu"));
            Assert.That(menu.MenuType, Is.EqualTo("Brunch"));
        }

        // ----------------------------------------------------
        // GET NUMBER OF POSITIONS
        // ----------------------------------------------------

        [Test]
        public void GetNumberOfPositions_ShouldReturnDishCount()
        {
            var menu = new Menu("Main", "Dinner", new List<string> { "EN" });

            menu.AddDish(new Dish("A", "Type", true, false, 10m, new List<string> { "X" }));
            menu.AddDish(new Dish("B", "Type", false, false, 15m, new List<string> { "Y" }));

            Assert.That(menu.GetNumberOfPositions(), Is.EqualTo(2));
        }
    }
}
