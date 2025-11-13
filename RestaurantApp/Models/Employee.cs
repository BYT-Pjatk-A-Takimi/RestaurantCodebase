using System;

namespace RestaurantApp.Models;

public abstract class Employee : Person
{
    protected Employee(
        string firstName,
        string lastName,
        DateOnly birthDate,
        string phoneNumber,
        WorkDetails workDetails,
        EmployeeExperienceProfile experienceProfile)
        : base(firstName, lastName, birthDate, phoneNumber)
    {
        WorkDetails = workDetails;
        ExperienceProfile = experienceProfile;
    }

    public WorkDetails WorkDetails { get; }

    public EmployeeExperienceProfile ExperienceProfile { get; private set; }

    public void UpdateExperienceProfile(EmployeeExperienceProfile profile)
    {
        ExperienceProfile = profile;
    }
}

public sealed class WorkDetails
{
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

