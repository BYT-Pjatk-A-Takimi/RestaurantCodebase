using System;
using System.Text.Json.Serialization;

namespace RestaurantApp.Models;

public sealed class WorkDetails
{
    [JsonConstructor]
    public WorkDetails(string department, string shiftSchedule, DateOnly dateOfHiring)
    {
        Department = department;
        ShiftSchedule = shiftSchedule;
        DateOfHiring = dateOfHiring;
    }

    public string Department { get; }

    public string ShiftSchedule { get; }

    public DateOnly DateOfHiring { get; }
}
