using System;
using NUnit.Framework;
using RestaurantApp.Models;

namespace TestProject1.Models
{
    public class PaymentTests
    {
        [Test]
        public void ProcessPayment_ChangesStatusToCompleted_AndSetsProcessedOn()
        {
            var payment = new Payment(Guid.NewGuid(), 100m, PaymentMethod.Card);

            payment.ProcessPayment();

            Assert.AreEqual(PaymentStatus.Completed, payment.Status);
            Assert.IsNotNull(payment.ProcessedOn);
        }

        [Test]
        public void RefundPayment_ChangesStatusToRefunded()
        {
            var payment = new Payment(Guid.NewGuid(), 50m, PaymentMethod.Cash);

            payment.RefundPayment();

            Assert.AreEqual(PaymentStatus.Refunded, payment.Status);
        }

        [Test]
        public void AdjustAmount_UpdatesPaymentAmount()
        {
            var payment = new Payment(Guid.NewGuid(), 70m, PaymentMethod.Card);

            payment.AdjustAmount(120m);

            Assert.AreEqual(120m, payment.Amount);
        }
    }
}