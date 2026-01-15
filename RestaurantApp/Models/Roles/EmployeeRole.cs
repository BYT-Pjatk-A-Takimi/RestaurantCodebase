using System;
using System.Text.Json.Serialization;
using RestaurantApp.Models.Roles.EmployeeTypes;

namespace RestaurantApp.Models.Roles;

public class EmployeeRole : PersonRole
{
    private WorkDetails _workDetails = null!;
    private EmployeeExperienceProfile _experienceProfile = null!;
    private EmployeeType? _employeeType;

    [JsonConstructor]
    public EmployeeRole(
        WorkDetails workDetails,
        EmployeeExperienceProfile experienceProfile,
        EmployeeType? employeeType = null)
    {
        WorkDetails = workDetails;
        ExperienceProfile = experienceProfile;
        EmployeeType = employeeType;
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
            
            if (_experienceProfile == value)
                return;

            if (_experienceProfile != null)
            {
                _experienceProfile.ClearEmployeeRole();
            }

            _experienceProfile = value;
            
            if (_experienceProfile.EmployeeRole != this)
            {
                _experienceProfile.SetEmployeeRole(this);
            }
        }
    }

    public EmployeeType? EmployeeType
    {
        get => _employeeType;
        private set
        {
            if (_employeeType != null)
            {
                _employeeType.ClearEmployeeRole();
            }
            _employeeType = value;
            if (_employeeType != null)
            {
                _employeeType.SetEmployeeRole(this);
            }
        }
    }

    public void UpdateExperienceProfile(EmployeeExperienceProfile profile)
    {
        ExperienceProfile = profile;
    }

    public void SetEmployeeType(EmployeeType type)
    {
        EmployeeType = type;
    }

    public void ClearEmployeeType()
    {
        EmployeeType = null;
    }
}
