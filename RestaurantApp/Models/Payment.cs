using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using RestaurantApp;

namespace RestaurantApp.Models;

public class Payment
{
    [JsonConstructor]
    public Payment(Guid orderId, decimal amount, PaymentMethod method)
    {
        if (orderId == Guid.Empty)
            throw new ArgumentException("Order id cannot be empty.", nameof(orderId));

        if (amount <= 0)
            throw new ArgumentException("Amount must be positive.", nameof(amount));

        Id = Guid.NewGuid();
        OrderId = orderId;
        Amount = amount;
        Method = method;
        Status = PaymentStatus.Pending;
        ProcessedOn = null;
    }

    // BASIC ATTRIBUTES
    public Guid Id { get; }

    public Guid OrderId { get; }

    public decimal Amount { get; private set; }

    public PaymentMethod Method { get; }

    public PaymentStatus Status { get; private set; }

    // OPTIONAL ATTRIBUTE (iyi bir örnek): ödeme işlenene kadar null
    public DateTime? ProcessedOn { get; private set; }

    // CLASS / STATIC ATTRIBUTE (basic type): taxRate
    public static decimal TaxRate { get; private set; } = 0.23m; // 23%

    public static void ChangeTaxRate(decimal newTaxRate)
    {
        if (newTaxRate < 0m || newTaxRate > 0.30m)
            throw new ArgumentException("Tax rate must be between 0 and 0.30.", nameof(newTaxRate));

        TaxRate = newTaxRate;
    }

    public void ProcessPayment()
    {
        if (Status == PaymentStatus.Completed)
            throw new InvalidOperationException("Payment is already completed.");

        Status = PaymentStatus.Completed;
        ProcessedOn = DateTime.UtcNow;
    }

    public void RefundPayment()
    {
        if (Status != PaymentStatus.Completed)
            throw new InvalidOperationException("Only completed payments can be refunded.");

        Status = PaymentStatus.Refunded;
    }

    public void AdjustAmount(decimal newAmount)
    {
        if (newAmount <= 0)
            throw new ArgumentException("Amount must be positive.", nameof(newAmount));

        Amount = newAmount;
    }

    // ---------- CLASS EXTENT & PERSISTENCE (hocanın istediği kısım) ----------

    // Extent: sistemdeki tüm Payment nesnelerini tutuyor
    public static List<Payment> Extent { get; } = new();

    public static void AddToExtent(Payment payment)
    {
        if (payment is null)
            throw new ArgumentNullException(nameof(payment));

        Extent.Add(payment);
    }

    public static void SaveAll(string filePath)
    {
        var options = JsonSerialization.GetDefaultOptions();
        var json = JsonSerializer.Serialize(Extent, options);
        File.WriteAllText(filePath, json);
    }

    public static void LoadAll(string filePath)
    {
        Extent.Clear();

        if (!File.Exists(filePath))
            return;

        var json = File.ReadAllText(filePath);
        var options = JsonSerialization.GetDefaultOptions();
        var loaded = JsonSerializer.Deserialize<List<Payment>>(json, options);

        if (loaded != null)
            Extent.AddRange(loaded);
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