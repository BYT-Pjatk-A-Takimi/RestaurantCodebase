using System;
using System.Text.Json.Serialization;

namespace RestaurantApp.Models;

public class Valet : Employee
{
    [JsonConstructor]
    public Valet(
        string firstName,
        string lastName,
        DateOnly birthDate,
        string phoneNumber,
        WorkDetails workDetails,
        EmployeeExperienceProfile experienceProfile,
        string assignedLocation)
        : base(firstName, lastName, birthDate, phoneNumber, workDetails, experienceProfile)
    {
        AssignedLocation = assignedLocation;
    }

    private string _assignedLocation = string.Empty;
    public string AssignedLocation
    {
        get => _assignedLocation;
        private set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Assigned location cannot be empty.", nameof(AssignedLocation));
            _assignedLocation = value;
        }
    }
}
