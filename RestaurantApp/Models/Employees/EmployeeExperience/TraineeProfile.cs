using System;
using System.Text.Json.Serialization;

namespace RestaurantApp.Models;

public sealed class TraineeProfile : EmployeeExperienceProfile
{
    public int TrainingDuration { get; private set; }

    [JsonConstructor]
    public TraineeProfile(int trainingDuration)
    {
        if (trainingDuration <= 0)
            throw new ArgumentException("Training duration must be positive.");

        TrainingDuration = trainingDuration;
    }

    public void CompleteTraining()
    {
        // UML'de davranış belirtilmemiş — boş bırakılabilir
    }
}
