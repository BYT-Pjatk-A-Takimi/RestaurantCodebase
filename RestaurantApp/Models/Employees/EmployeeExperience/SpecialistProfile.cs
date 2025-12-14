using System;
using System.Text.Json.Serialization;

namespace RestaurantApp.Models;

public sealed class SpecialistProfile : EmployeeExperienceProfile
{
    public string FieldOfExpertise { get; private set; }

    [JsonConstructor]
    public SpecialistProfile(string fieldOfExpertise)
    {
        if (string.IsNullOrWhiteSpace(fieldOfExpertise))
            throw new ArgumentException("Field of expertise cannot be empty.");

        FieldOfExpertise = fieldOfExpertise;
    }

    public string DesignNewRecipes()
    {
        // Disiyagram method, davranış belirtilmemiş
        return $"New recipe designed in {FieldOfExpertise}";
    }
}
