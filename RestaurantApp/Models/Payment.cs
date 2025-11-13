using System;

namespace RestaurantApp.Models;

public class Payment
{
    public Payment(Guid orderId, decimal amount, PaymentMethod method)
    {
        Id = Guid.NewGuid();
        OrderId = orderId;
        Amount = amount;
        Method = method;
        Status = PaymentStatus.Pending;
        ProcessedOn = null;
    }

    public Guid Id { get; }

    public Guid OrderId { get; }

    public decimal Amount { get; private set; }

    public PaymentMethod Method { get; }

    public PaymentStatus Status { get; private set; }

    public DateTime? ProcessedOn { get; private set; }

    public void ProcessPayment()
    {
        Status = PaymentStatus.Completed;
        ProcessedOn = DateTime.UtcNow;
    }

    public void RefundPayment()
    {
        Status = PaymentStatus.Refunded;
    }

    public void AdjustAmount(decimal newAmount)
    {
        Amount = newAmount;
    }
}

public enum PaymentMethod
{
    Card,
    Cash
}

public enum PaymentStatus
{
    Pending,
    Completed,
    Refunded
}

