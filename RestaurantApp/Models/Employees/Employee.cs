using System;
using System.Text.Json.Serialization;

namespace RestaurantApp.Models;

[JsonPolymorphic]
[JsonDerivedType(typeof(Manager), typeDiscriminator: "Manager")]
[JsonDerivedType(typeof(Chef), typeDiscriminator: "Chef")]
[JsonDerivedType(typeof(Waiter), typeDiscriminator: "Waiter")]
[JsonDerivedType(typeof(Valet), typeDiscriminator: "Valet")]
public abstract class Employee : Person
{
    private WorkDetails _workDetails = null!;
    private EmployeeExperienceProfile _experienceProfile = null!;

    [JsonConstructor]
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

    public WorkDetails WorkDetails
    {
        get => _workDetails;
        private set
        {
            if (value is null)
                throw new ArgumentNullException(nameof(WorkDetails), "Work details cannot be null.");
            _workDetails = value;
        }
    }

    public EmployeeExperienceProfile ExperienceProfile
    {
        get => _experienceProfile;
        private set
        {
            if (value is null)
                throw new ArgumentNullException(nameof(ExperienceProfile), "Experience profile cannot be null.");
            _experienceProfile = value;
        }
    }

    public void UpdateExperienceProfile(EmployeeExperienceProfile profile)
    {
        ExperienceProfile = profile;
    }
}
