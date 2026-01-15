using System.Text.Json.Serialization;

using RestaurantApp.Models.Roles;

namespace RestaurantApp.Models;

[JsonPolymorphic]
[JsonDerivedType(typeof(TraineeProfile), typeDiscriminator: "TraineeProfile")]
[JsonDerivedType(typeof(ExperiencedProfile), typeDiscriminator: "ExperiencedProfile")]
[JsonDerivedType(typeof(SpecialistProfile), typeDiscriminator: "SpecialistProfile")]
public abstract class EmployeeExperienceProfile
{
    [JsonIgnore]
    public EmployeeRole? EmployeeRole { get; private set; }

    [JsonConstructor]
    protected EmployeeExperienceProfile() { }

    internal void SetEmployeeRole(EmployeeRole role)
    {
        if (EmployeeRole == role)
            return;

        EmployeeRole = role;
        
        if (role != null && role.ExperienceProfile != this)
        {
            role.UpdateExperienceProfile(this);
        }
    }

    internal void ClearEmployeeRole()
    {
        EmployeeRole = null;
    }
}
