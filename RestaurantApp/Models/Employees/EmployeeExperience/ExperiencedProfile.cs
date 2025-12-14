using System;
using System.Text.Json.Serialization;

namespace RestaurantApp.Models;

public sealed class ExperiencedProfile : EmployeeExperienceProfile
{
    public int YearsOfExperience { get; private set; }
    public string MentorName { get; private set; }

    [JsonConstructor]
    public ExperiencedProfile(int yearsOfExperience, string mentorName)
    {
        if (yearsOfExperience < 0)
            throw new ArgumentException("Years of experience cannot be negative.");

        if (string.IsNullOrWhiteSpace(mentorName))
            throw new ArgumentException("Mentor name cannot be empty.");

        YearsOfExperience = yearsOfExperience;
        MentorName = mentorName;
    }

    public void MentorTrainee()
    {
        // UML method, içerik verilmemiş
    }
}
