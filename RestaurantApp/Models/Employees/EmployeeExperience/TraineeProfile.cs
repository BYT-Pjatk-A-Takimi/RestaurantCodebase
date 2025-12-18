using System;
using System.Text.Json.Serialization;

namespace RestaurantApp.Models;

public sealed class TraineeProfile : EmployeeExperienceProfile
{
    private int _trainingDuration;

    public int TrainingDuration
    {
        get => _trainingDuration;
        private set
        {
            if (value <= 0)
                throw new ArgumentException("Training duration must be positive.", nameof(TrainingDuration));
            _trainingDuration = value;
        }
    }

    [JsonConstructor]
    public TraineeProfile(int trainingDuration)
    {
        TrainingDuration = trainingDuration;
    }

    public void CompleteTraining()
    {
        // UML'de davranış belirtilmemiş — boş bırakılabilir
    }
}
