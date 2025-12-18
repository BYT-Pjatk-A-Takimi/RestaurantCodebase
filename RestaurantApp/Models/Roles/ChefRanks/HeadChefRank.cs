using System;
using System.Text.Json.Serialization;

namespace RestaurantApp.Models.Roles.ChefRanks;

public class HeadChefRank : ChefRank
{
    private int _kitchenExperienceYears;

    [JsonConstructor]
    public HeadChefRank(int kitchenExperienceYears)
    {
        KitchenExperienceYears = kitchenExperienceYears;
    }

    public int KitchenExperienceYears
    {
        get => _kitchenExperienceYears;
        private set
        {
            if (value < 0)
                throw new ArgumentException("Kitchen experience years cannot be negative.", nameof(KitchenExperienceYears));
            _kitchenExperienceYears = value;
        }
    }

    public void OverseeKitchen() { }

    public void ApproveMenuChanges(Menu menu) { }
}
