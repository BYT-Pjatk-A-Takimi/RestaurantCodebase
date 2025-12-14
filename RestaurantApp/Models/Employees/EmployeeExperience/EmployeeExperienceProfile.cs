using System.Text.Json.Serialization;

namespace RestaurantApp.Models;

[JsonPolymorphic]
[JsonDerivedType(typeof(TraineeProfile), typeDiscriminator: "TraineeProfile")]
[JsonDerivedType(typeof(ExperiencedProfile), typeDiscriminator: "ExperiencedProfile")]
[JsonDerivedType(typeof(SpecialistProfile), typeDiscriminator: "SpecialistProfile")]
public abstract class EmployeeExperienceProfile
{
    [JsonConstructor]
    protected EmployeeExperienceProfile() { }
}
