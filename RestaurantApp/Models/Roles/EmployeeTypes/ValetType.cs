using System;
using System.Text.Json.Serialization;

namespace RestaurantApp.Models.Roles.EmployeeTypes;

public class ValetType : EmployeeType
{
    private string _assignedLocation = string.Empty;

    [JsonConstructor]
    public ValetType(string assignedLocation)
    {
        AssignedLocation = assignedLocation;
    }

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
