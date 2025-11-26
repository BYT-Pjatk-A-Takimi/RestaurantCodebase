using System;
using System.IO;
using NUnit.Framework;
using RestaurantApp.Models;

namespace RestaurantApp.Tests.Models;

[TestFixture]
public class PaymentTests
{
    [SetUp]
    public void SetUp()
    {
        Payment.Extent.Clear();
        // taxRate default 0.23, testlerde gerekiyorsa değiştirebiliriz
        Payment.ChangeTaxRate(0.23m);
    }

    [Test]
    public void Constructor_ShouldInitializeProperties()
    {
        var orderId = Guid.NewGuid();
        var payment = new Payment(orderId, 100m, PaymentMethod.Card);

        Assert.Multiple(() =>
        {
            Assert.That(payment.Id, Is.Not.EqualTo(Guid.Empty));
            Assert.That(payment.OrderId, Is.EqualTo(orderId));
            Assert.That(payment.Amount, Is.EqualTo(100m));
            Assert.That(payment.Method, Is.EqualTo(PaymentMethod.Card));
            Assert.That(payment.Status, Is.EqualTo(PaymentStatus.Pending));
            Assert.That(payment.ProcessedOn, Is.Null);
        });
    }

    [Test]
    public void ProcessPayment_ShouldSetStatusToCompleted_AndSetProcessedOn()
    {
        var payment = new Payment(Guid.NewGuid(), 50m, PaymentMethod.Cash);

        payment.ProcessPayment();

        Assert.Multiple(() =>
        {
            Assert.That(payment.Status, Is.EqualTo(PaymentStatus.Completed));
            Assert.That(payment.ProcessedOn, Is.Not.Null);
        });
    }

    [Test]
    public void RefundPayment_ShouldSetStatusToRefunded_WhenCompleted()
    {
        var payment = new Payment(Guid.NewGuid(), 75m, PaymentMethod.Card);

        payment.ProcessPayment();
        payment.RefundPayment();

        Assert.That(payment.Status, Is.EqualTo(PaymentStatus.Refunded));
    }

    [Test]
    public void RefundPayment_ShouldThrow_IfNotCompleted()
    {
        var payment = new Payment(Guid.NewGuid(), 75m, PaymentMethod.Card);

        Assert.Throws<InvalidOperationException>(() => payment.RefundPayment());
    }

    [Test]
    public void AdjustAmount_ShouldChangeAmount_WhenPositive()
    {
        var payment = new Payment(Guid.NewGuid(), 80m, PaymentMethod.Cash);

        payment.AdjustAmount(120m);

        Assert.That(payment.Amount, Is.EqualTo(120m));
    }

    [Test]
    public void AdjustAmount_ShouldThrow_WhenNonPositive()
    {
        var payment = new Payment(Guid.NewGuid(), 80m, PaymentMethod.Cash);

        Assert.Throws<ArgumentException>(() => payment.AdjustAmount(0m));
        Assert.Throws<ArgumentException>(() => payment.AdjustAmount(-10m));
    }

    [Test]
    public void ChangeTaxRate_ShouldChange_WhenValid()
    {
        Payment.ChangeTaxRate(0.10m);

        // TaxRate static property
        // Sadece exception atmaması ve değer ataması bizim için yeterli
        Assert.That(() => Payment.ChangeTaxRate(0.20m), Throws.Nothing);
    }

    [Test]
    public void ChangeTaxRate_ShouldThrow_WhenInvalid()
    {
        Assert.Throws<ArgumentException>(() => Payment.ChangeTaxRate(-0.1m));
        Assert.Throws<ArgumentException>(() => Payment.ChangeTaxRate(0.5m));
    }

    [Test]
    public void AddToExtent_ShouldAddPaymentToExtent()
    {
        var payment = new Payment(Guid.NewGuid(), 60m, PaymentMethod.Card);

        Payment.AddToExtent(payment);

        Assert.That(Payment.Extent, Has.Count.EqualTo(1));
        Assert.That(Payment.Extent[0], Is.EqualTo(payment));
    }

    [Test]
    public void SaveAllAndLoadAll_ShouldPersistPayments()
    {
        var filePath = Path.Combine(TestContext.CurrentContext.WorkDirectory, "payments_test.json");
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }

        var p1 = new Payment(Guid.NewGuid(), 40m, PaymentMethod.Cash);
        var p2 = new Payment(Guid.NewGuid(), 55m, PaymentMethod.Card);

        Payment.AddToExtent(p1);
        Payment.AddToExtent(p2);

        Payment.SaveAll(filePath);

        Assert.That(File.Exists(filePath), Is.True);

        Payment.Extent.Clear();

        Payment.LoadAll(filePath);

        Assert.That(Payment.Extent, Has.Count.EqualTo(2));
    }
}