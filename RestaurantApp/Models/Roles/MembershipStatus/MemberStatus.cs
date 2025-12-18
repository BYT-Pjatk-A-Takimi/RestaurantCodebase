using System;
using System.Text.Json.Serialization;

namespace RestaurantApp.Models.Roles.MembershipStatus;

public class MemberStatus : MembershipStatusBase
{
    private const int CreditsPerOrder = 1;

    private int _credits;
    private decimal _creditPointsRate;

    [JsonConstructor]
    public MemberStatus(int credits, decimal creditPointsRate)
    {
        Credits = credits;
        CreditPointsRate = creditPointsRate;
    }

    public int Credits
    {
        get => _credits;
        private set
        {
            if (value < 0)
                throw new ArgumentException("Credits cannot be negative.", nameof(Credits));
            _credits = value;
        }
    }

    public decimal CreditPointsRate
    {
        get => _creditPointsRate;
        private set
        {
            if (value <= 0)
                throw new ArgumentException("Credit points rate must be positive.", nameof(CreditPointsRate));
            _creditPointsRate = value;
        }
    }

    public decimal UseCredits(decimal amount)
    {
        if (Credits <= 0)
        {
            return amount;
        }

        var discount = Credits * CreditPointsRate;
        Credits = 0;
        return amount - discount;
    }

    public void AddCredits() => Credits += CreditsPerOrder;
}
