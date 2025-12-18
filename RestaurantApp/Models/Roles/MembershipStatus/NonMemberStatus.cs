using System;
using System.Text.Json.Serialization;

namespace RestaurantApp.Models.Roles.MembershipStatus;

public class NonMemberStatus : MembershipStatusBase
{
    [JsonConstructor]
    public NonMemberStatus()
    {
    }

    public MemberStatus PromoteToMember(decimal initialCreditRate)
    {
        if (initialCreditRate <= 0)
            throw new ArgumentException("Initial credit rate must be positive.", nameof(initialCreditRate));

        return new MemberStatus(0, initialCreditRate);
    }
}
