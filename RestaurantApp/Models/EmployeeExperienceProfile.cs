namespace RestaurantApp.Models;

public abstract class EmployeeExperienceProfile
{
    protected EmployeeExperienceProfile(string description)
    {
        Description = description;
    }

    public string Description { get; }
}

public sealed class TraineeProfile : EmployeeExperienceProfile
{
    public TraineeProfile(int trainingDurationWeeks)
        : base("Trainee")
    {
        TrainingDurationWeeks = trainingDurationWeeks;
    }

    public int TrainingDurationWeeks { get; }

    public bool HasCompletedTraining(int completedWeeks) => completedWeeks >= TrainingDurationWeeks;
}

public sealed class ExperiencedProfile : EmployeeExperienceProfile
{
    public ExperiencedProfile(int yearsOfExperience, string mentorName)
        : base("Experienced")
    {
        YearsOfExperience = yearsOfExperience;
        MentorName = mentorName;
    }

    public int YearsOfExperience { get; }

    public string MentorName { get; }
}

public sealed class SpecialistProfile : EmployeeExperienceProfile
{
    public SpecialistProfile(string fieldOfExpertise)
        : base("Specialist")
    {
        FieldOfExpertise = fieldOfExpertise;
    }

    public string FieldOfExpertise { get; }

    public string DesignNewRecipe(string baseDish, string twist) => $"{baseDish} with {twist}";
}

