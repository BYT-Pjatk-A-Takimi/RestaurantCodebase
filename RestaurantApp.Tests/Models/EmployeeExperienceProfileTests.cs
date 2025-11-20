using NUnit.Framework;
using RestaurantApp.Models;

namespace RestaurantApp.Tests.Models;

[TestFixture]
public class EmployeeExperienceProfileTests
{
    [Test]
    public void TraineeProfile_ShouldStoreTrainingDuration()
    {
        var trainee = new TraineeProfile(8);

        Assert.That(trainee.Description, Is.EqualTo("Trainee"));
        Assert.That(trainee.TrainingDurationWeeks, Is.EqualTo(8));
        Assert.That(trainee.HasCompletedTraining(8), Is.True);
        Assert.That(trainee.HasCompletedTraining(5), Is.False);
    }

    [Test]
    public void ExperiencedProfile_ShouldStoreExperienceAndMentor()
    {
        var profile = new ExperiencedProfile(5, "John Doe");

        Assert.That(profile.Description, Is.EqualTo("Experienced"));
        Assert.That(profile.YearsOfExperience, Is.EqualTo(5));
        Assert.That(profile.MentorName, Is.EqualTo("John Doe"));
    }

    [Test]
    public void SpecialistProfile_ShouldStoreFieldOfExpertiseAndDesignRecipe()
    {
        var profile = new SpecialistProfile("Desserts");

        Assert.That(profile.Description, Is.EqualTo("Specialist"));
        Assert.That(profile.FieldOfExpertise, Is.EqualTo("Desserts"));

        var result = profile.DesignNewRecipe("Cheesecake", "strawberry sauce");
        Assert.That(result, Is.EqualTo("Cheesecake with strawberry sauce"));
    }
}