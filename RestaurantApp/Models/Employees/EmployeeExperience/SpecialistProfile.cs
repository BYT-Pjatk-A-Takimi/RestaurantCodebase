using System;
using System.Text.Json.Serialization;

namespace RestaurantApp.Models;

public sealed class SpecialistProfile : EmployeeExperienceProfile
{
    private string _fieldOfExpertise = string.Empty;

    public string FieldOfExpertise
    {
        get => _fieldOfExpertise;
        private set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Field of expertise cannot be empty.", nameof(FieldOfExpertise));
            _fieldOfExpertise = value;
        }
    }

    [JsonConstructor]
    public SpecialistProfile(string fieldOfExpertise)
    {
        FieldOfExpertise = fieldOfExpertise;
    }

    public string DesignNewRecipes()
    {
        // Disiyagram method, davranış belirtilmemiş
        return $"New recipe designed in {FieldOfExpertise}";
    }
}
