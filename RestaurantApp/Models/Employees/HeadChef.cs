using System;
using System.Text.Json.Serialization;

namespace RestaurantApp.Models;

public class HeadChef : Chef
{
    [JsonConstructor]
    public HeadChef(
        string firstName,
        string lastName,
        DateOnly birthDate,
        string phoneNumber,
        WorkDetails workDetails,
        EmployeeExperienceProfile experienceProfile,
        string cuisineType,
        int kitchenExperienceYears)
        : base(firstName, lastName, birthDate, phoneNumber, workDetails, experienceProfile, cuisineType)
    {
        KitchenExperienceYears = kitchenExperienceYears;
    }

    private int _kitchenExperienceYears;
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
