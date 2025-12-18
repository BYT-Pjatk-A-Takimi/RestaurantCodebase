using System;
using System.Text.Json.Serialization;

namespace RestaurantApp.Models;

public sealed class ExperiencedProfile : EmployeeExperienceProfile
{
    private int _yearsOfExperience;
    private string _mentorName = string.Empty;

    public int YearsOfExperience
    {
        get => _yearsOfExperience;
        private set
        {
            if (value < 0)
                throw new ArgumentException("Years of experience cannot be negative.", nameof(YearsOfExperience));
            _yearsOfExperience = value;
        }
    }

    public string MentorName
    {
        get => _mentorName;
        private set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Mentor name cannot be empty.", nameof(MentorName));
            _mentorName = value;
        }
    }

    [JsonConstructor]
    public ExperiencedProfile(int yearsOfExperience, string mentorName)
    {
        YearsOfExperience = yearsOfExperience;
        MentorName = mentorName;
    }

    public void MentorTrainee()
    {
        // UML method, içerik verilmemiş
    }
}
