using System;
using NUnit.Framework;
using RestaurantApp.Models;

namespace TestProject1.Models
{
    public class TableTests
    {
        private NonMember CreateCustomer()
        {
            return new NonMember("Test", "User", new DateOnly(2000, 1, 1), "123", "a@a.com");
        }

        [Test]
        public void Reserve_AddsReservation_WhenDateFree()
        {
            var table = new Table(1, 4, "Indoor");
            var customer = CreateCustomer();
            var reservation = new Reservation(Guid.NewGuid(), new DateOnly(2025, 1, 1), 2, table);

            var result = table.Reserve(customer, reservation);

            Assert.IsTrue(result);
            Assert.AreEqual(1, table.Reservations.Count);
        }

        [Test]
        public void Reserve_ReturnsFalse_WhenDateAlreadyReserved()
        {
            var table = new Table(1, 4, "Indoor");
            var customer = CreateCustomer();

            var date = new DateOnly(2025, 1, 1);

            var r1 = new Reservation(Guid.NewGuid(), date, 2, table);
            var r2 = new Reservation(Guid.NewGuid(), date, 3, table);

            table.Reserve(customer, r1);
            var result = table.Reserve(customer, r2);

            Assert.IsFalse(result);
            Assert.AreEqual(1, table.Reservations.Count);
        }
    }
}