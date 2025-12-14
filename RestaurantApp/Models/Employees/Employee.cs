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

    public WorkDetails WorkDetails { get; }

    public EmployeeExperienceProfile ExperienceProfile { get; private set; }

    public void UpdateExperienceProfile(EmployeeExperienceProfile profile)
    {
        ExperienceProfile = profile;
    }
}
