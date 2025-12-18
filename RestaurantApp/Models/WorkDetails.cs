using System;
using System.Text.Json.Serialization;

namespace RestaurantApp.Models;

public sealed class WorkDetails
{
    private string _department = string.Empty;
    private string _shiftSchedule = string.Empty;
    private DateOnly _dateOfHiring;

    [JsonConstructor]
    public WorkDetails(string department, string shiftSchedule, DateOnly dateOfHiring)
    {
        Department = department;
        ShiftSchedule = shiftSchedule;
        DateOfHiring = dateOfHiring;
    }

    public string Department
    {
        get => _department;
        private set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Department cannot be empty.", nameof(Department));
            _department = value;
        }
    }

    public string ShiftSchedule
    {
        get => _shiftSchedule;
        private set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Shift schedule cannot be empty.", nameof(ShiftSchedule));
            _shiftSchedule = value;
        }
    }

    public DateOnly DateOfHiring
    {
        get => _dateOfHiring;
        private set
        {
            if (value == default)
                throw new ArgumentException("Date of hiring must be a valid date.", nameof(DateOfHiring));
            _dateOfHiring = value;
        }
    }
}
