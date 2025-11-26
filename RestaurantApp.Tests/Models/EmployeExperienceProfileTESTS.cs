using NUnit.Framework;
using RestaurantApp.Models;
using System;

namespace RestaurantApp.Tests.Models
{
    [TestFixture]
    public class EmployeeExperienceProfileTests
    {
        // ----------------------------------------------------
        // TRAINEE PROFILE TESTS
        // ----------------------------------------------------

        [Test]
        public void TraineeProfile_ShouldSetTrainingDuration()
        {
            var trainee = new TraineeProfile(8);

            Assert.That(trainee.TrainingDuration, Is.EqualTo(8));
        }

        [Test]
        public void TraineeProfile_ShouldThrow_WhenDurationIsInvalid()
        {
            Assert.Throws<ArgumentException>(() =>
                new TraineeProfile(0)
            );

            Assert.Throws<ArgumentException>(() =>
                new TraineeProfile(-5)
            );
        }

        [Test]
        public void TraineeProfile_CompleteTraining_ShouldNotThrow()
        {
            var trainee = new TraineeProfile(5);

            Assert.DoesNotThrow(() => trainee.CompleteTraining());
        }

        // ----------------------------------------------------
        // EXPERIENCED PROFILE TESTS
        // ----------------------------------------------------

        [Test]
        public void ExperiencedProfile_ShouldSetAllProperties()
        {
            var profile = new ExperiencedProfile(4, "John Doe");

            Assert.That(profile.YearsOfExperience, Is.EqualTo(4));
            Assert.That(profile.MentorName, Is.EqualTo("John Doe"));
        }

        [Test]
        public void ExperiencedProfile_ShouldThrow_WhenYearsOfExperienceInvalid()
        {
            Assert.Throws<ArgumentException>(() =>
                new ExperiencedProfile(-1, "John")
            );
        }

        [Test]
        public void ExperiencedProfile_ShouldThrow_WhenMentorNameEmpty()
        {
            Assert.Throws<ArgumentException>(() =>
                new ExperiencedProfile(2, "")
            );

            Assert.Throws<ArgumentException>(() =>
                new ExperiencedProfile(2, "   ")
            );

            Assert.Throws<ArgumentException>(() =>
                new ExperiencedProfile(2, null!)
            );
        }

        [Test]
        public void ExperiencedProfile_MentorTrainee_ShouldNotThrow()
        {
            var profile = new ExperiencedProfile(3, "Alice");

            Assert.DoesNotThrow(() => profile.MentorTrainee());
        }

        // ----------------------------------------------------
        // SPECIALIST PROFILE TESTS
        // ----------------------------------------------------

        [Test]
        public void SpecialistProfile_ShouldSetFieldOfExpertise()
        {
            var specialist = new SpecialistProfile("Desserts");

            Assert.That(specialist.FieldOfExpertise, Is.EqualTo("Desserts"));
        }

        [Test]
        public void SpecialistProfile_ShouldThrow_WhenFieldOfExpertiseInvalid()
        {
            Assert.Throws<ArgumentException>(() =>
                new SpecialistProfile("")
            );

            Assert.Throws<ArgumentException>(() =>
                new SpecialistProfile("    ")
            );

            Assert.Throws<ArgumentException>(() =>
                new SpecialistProfile(null!)
            );
        }

        [Test]
        public void SpecialistProfile_DesignNewRecipes_ShouldReturnCorrectString()
        {
            var specialist = new SpecialistProfile("Soups");

            var result = specialist.DesignNewRecipes();

            Assert.That(result, Is.EqualTo("New recipe designed in Soups"));
        }
    }
}
