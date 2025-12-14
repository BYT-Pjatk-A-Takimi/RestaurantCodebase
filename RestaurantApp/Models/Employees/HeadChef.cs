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

    public int KitchenExperienceYears { get; }

    public void OverseeKitchen() { }

    public void ApproveMenuChanges(Menu menu) { }
}
