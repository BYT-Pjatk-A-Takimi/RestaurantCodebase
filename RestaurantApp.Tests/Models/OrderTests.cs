using System;
using NUnit.Framework;
using RestaurantApp.Models;

namespace TestProject1.Models
{
    public class OrderTests
    {
        private Dish CreateDish(decimal price)
        {
            return new Dish(
                "TestDish",
                "International",
                false,
                price,
                new[] { "salt", "oil" }
            );
        }

        private OrderDish CreateOrderDish(decimal price, int quantity)
        {
            return new OrderDish("TestDish", CreateDish(price), quantity);
        }

        [Test]
        public void AddDish_AddsSingleDish()
        {
            var customer = new NonMember("Beril", "Yavuz", new DateOnly(2000, 1, 1), "555", "test@mail.com");
            var table = new Table(1, 4, "Indoor");
            var order = new Order(customer, table);

            var dish = CreateOrderDish(10m, 2);

            order.AddDish(dish);

            Assert.AreEqual(1, order.Dishes.Count);
        }

        [Test]
        public void AddDishes_AddsMultipleDishes()
        {
            var customer = new NonMember("Beril", "Yavuz", new DateOnly(2000, 1, 1), "555", "test@mail.com");
            var table = new Table(1, 4, "Indoor");
            var order = new Order(customer, table);

            var dishes = new[]
            {
                CreateOrderDish(10m, 1),
                CreateOrderDish(12m, 2)
            };

            order.AddDishes(dishes);

            Assert.AreEqual(2, order.Dishes.Count);
        }

        [Test]
        public void CalculateTotal_ReturnsCorrectSum()
        {
            var customer = new NonMember("Beril", "Yavuz", new DateOnly(2000, 1, 1), "555", "test@mail.com");
            var table = new Table(1, 4, "Indoor");
            var order = new Order(customer, table);

            order.AddDish(CreateOrderDish(10m, 2)); // 20
            order.AddDish(CreateOrderDish(5m, 3));  // 15

            var total = order.CalculateTotal();

            Assert.AreEqual(35m, total);
        }

        [Test]
        public void CompleteOrder_ChangesStatusToCompleted()
        {
            var customer = new NonMember("Beril", "Yavuz", new DateOnly(2000, 1, 1), "555", "test@mail.com");
            var table = new Table(1, 4, "Indoor");
            var order = new Order(customer, table);

            order.CompleteOrder();

            Assert.AreEqual(OrderStatus.Completed, order.Status);
        }
    }
}