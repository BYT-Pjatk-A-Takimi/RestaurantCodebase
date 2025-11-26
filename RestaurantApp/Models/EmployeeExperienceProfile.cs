using System;

namespace RestaurantApp.Models
{
    public abstract class EmployeeExperienceProfile
    {
        protected EmployeeExperienceProfile() { }
    }

    public sealed class TraineeProfile : EmployeeExperienceProfile
    {
        public int TrainingDuration { get; private set; }

        public TraineeProfile(int trainingDuration)
        {
            if (trainingDuration <= 0)
                throw new ArgumentException("Training duration must be positive.");

            TrainingDuration = trainingDuration;
        }

        public void CompleteTraining()
        {
            // UML’de davranış belirtilmemiş — boş bırakılabilir
        }
    }

    public sealed class ExperiencedProfile : EmployeeExperienceProfile
    {
        public int YearsOfExperience { get; private set; }
        public string MentorName { get; private set; }

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

    public sealed class SpecialistProfile : EmployeeExperienceProfile
    {
        public string FieldOfExpertise { get; private set; }

        public SpecialistProfile(string fieldOfExpertise)
        {
            if (string.IsNullOrWhiteSpace(fieldOfExpertise))
                throw new ArgumentException("Field of expertise cannot be empty.");

            FieldOfExpertise = fieldOfExpertise;
        }

        public string DesignNewRecipes()
        {
            // Disiyagram method, davranış belirtilmemiş
            return $"New recipe designed in {FieldOfExpertise}";
        }
    }
}
