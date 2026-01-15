using System;
using System.IO;
using NUnit.Framework;
using RestaurantApp.Models;
using RestaurantApp.Models.Roles;
using RestaurantApp.Models.Roles.MembershipStatus;

namespace RestaurantApp.Tests.Models;

[TestFixture]
public class PaymentTests
{
    private Restaurant CreateRestaurant()
    {
        return new Restaurant("Test Restaurant", 100);
    }

    private Order CreateOrder()
    {
        var nonMemberStatus = new NonMemberStatus();
        var customerRole = new CustomerRole("test@example.com", nonMemberStatus);
        var customer = new Person("Test", "User", new DateOnly(2000, 1, 1), "123456789", customerRole: customerRole);
        var restaurant = CreateRestaurant();
        var table = new Table(1, 4, "Standard", restaurant);
        return new Order(customer, table);
    }

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
        var order = CreateOrder();
        var payment = new Payment(order, 100m, PaymentMethod.Card);

        Assert.Multiple(() =>
        {
            Assert.That(payment.Id, Is.Not.EqualTo(Guid.Empty));
            Assert.That(payment.OrderId, Is.EqualTo(order.Id));
            Assert.That(payment.Order, Is.EqualTo(order));
            Assert.That(payment.Amount, Is.EqualTo(100m));
            Assert.That(payment.Method, Is.EqualTo(PaymentMethod.Card));
            Assert.That(payment.Status, Is.EqualTo(PaymentStatus.Pending));
            Assert.That(payment.ProcessedOn, Is.Null);
        });
    }

    [Test]
    public void ProcessPayment_ShouldSetStatusToCompleted_AndSetProcessedOn()
    {
        var order = CreateOrder();
        var payment = new Payment(order, 50m, PaymentMethod.Cash);

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
        var order = CreateOrder();
        var payment = new Payment(order, 75m, PaymentMethod.Card);

        payment.ProcessPayment();
        payment.RefundPayment();

        Assert.That(payment.Status, Is.EqualTo(PaymentStatus.Refunded));
    }

    [Test]
    public void RefundPayment_ShouldThrow_IfNotCompleted()
    {
        var order = CreateOrder();
        var payment = new Payment(order, 75m, PaymentMethod.Card);

        Assert.Throws<InvalidOperationException>(() => payment.RefundPayment());
    }

    [Test]
    public void AdjustAmount_ShouldChangeAmount_WhenPositive()
    {
        var order = CreateOrder();
        var payment = new Payment(order, 80m, PaymentMethod.Cash);

        payment.AdjustAmount(120m);

        Assert.That(payment.Amount, Is.EqualTo(120m));
    }

    [Test]
    public void AdjustAmount_ShouldThrow_WhenNonPositive()
    {
        var order = CreateOrder();
        var payment = new Payment(order, 80m, PaymentMethod.Cash);

        Assert.Throws<ArgumentException>(() => payment.AdjustAmount(0m));
        Assert.Throws<ArgumentException>(() => payment.AdjustAmount(-10m));
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
        var order = CreateOrder();
        var payment = new Payment(order, 60m, PaymentMethod.Card);

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

        var order1 = CreateOrder();
        var order2 = CreateOrder();
        var p1 = new Payment(order1, 40m, PaymentMethod.Cash);
        var p2 = new Payment(order2, 55m, PaymentMethod.Card);

        Payment.AddToExtent(p1);
        Payment.AddToExtent(p2);

        Payment.SaveAll(filePath);

        Assert.That(File.Exists(filePath), Is.True);

        Payment.Extent.Clear();

        Payment.LoadAll(filePath);

        Assert.That(Payment.Extent, Has.Count.EqualTo(2));
    }

    [Test]
    public void Constructor_WithNullOrder_ShouldThrowArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new Payment(null!, 50m, PaymentMethod.Card));
    }

    [Test]
    public void Constructor_WithZeroAmount_ShouldThrowArgumentException()
    {
        var order = CreateOrder();

        Assert.Throws<ArgumentException>(() => new Payment(order, 0m, PaymentMethod.Card));
    }

    [Test]
    public void Constructor_WithNegativeAmount_ShouldThrowArgumentException()
    {
        var order = CreateOrder();

        Assert.Throws<ArgumentException>(() => new Payment(order, -10m, PaymentMethod.Cash));
    }

    [Test]
    public void ProcessPayment_WhenAlreadyCompleted_ShouldThrowInvalidOperationException()
    {
        var order = CreateOrder();
        var payment = new Payment(order, 100m, PaymentMethod.Card);

        payment.ProcessPayment();

        Assert.Throws<InvalidOperationException>(() => payment.ProcessPayment());
    }

    [Test]
    public void ChangeTaxRate_ShouldUpdateStaticTaxRate()
    {
        Payment.ChangeTaxRate(0.20m);

        Assert.That(Payment.TaxRate, Is.EqualTo(0.20m));

        Payment.ChangeTaxRate(0.23m); // Reset to default
    }

    [Test]
    public void Payment_ShouldBeAddedToOrder()
    {
        var order = CreateOrder();
        var payment = new Payment(order, 100m, PaymentMethod.Card);

        Assert.That(order.Payments, Has.Count.EqualTo(1));
        Assert.That(order.Payments, Contains.Item(payment));
    }
}
