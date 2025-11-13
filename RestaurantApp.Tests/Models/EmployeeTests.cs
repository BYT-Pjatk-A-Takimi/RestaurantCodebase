using System;
using NUnit.Framework;
using RestaurantApp.Models;

namespace RestaurantApp.Tests.Models;

[TestFixture]
public class EmployeeTests
{
    [Test]
    public void Employee_WorkDetails_IsSetCorrectly()
    {
        var department = "Kitchen";
        var shiftSchedule = "Day";
        var dateOfHiring = DateOnly.FromDateTime(DateTime.Today.AddYears(-2));
        var workDetails = new WorkDetails(department, shiftSchedule, dateOfHiring);
        var experienceProfile = new ExperiencedProfile(5, "Chef Mentor");
        var employee = new Chef("Berkay", "Bayar", DateOnly.Parse("1999-03-12"), "111-1111", workDetails, experienceProfile, "Italian");

        Assert.That(employee.WorkDetails, Is.Not.Null);
        Assert.That(employee.WorkDetails.Department, Is.EqualTo(department));
        Assert.That(employee.WorkDetails.ShiftSchedule, Is.EqualTo(shiftSchedule));
        Assert.That(employee.WorkDetails.DateOfHiring, Is.EqualTo(dateOfHiring));
    }

    [Test]
    public void Employee_UpdateExperienceProfile_UpdatesProfile()
    {
        var workDetails = new WorkDetails("Service", "Evening", DateOnly.FromDateTime(DateTime.Today));
        var initialProfile = new TraineeProfile(8);
        var employee = new Waiter("Berkay", "Bayar", DateOnly.Parse("1999-03-12"), "111-1111", workDetails, initialProfile);
        var newProfile = new ExperiencedProfile(2, "Senior Waiter");

        employee.UpdateExperienceProfile(newProfile);

        Assert.That(employee.ExperienceProfile, Is.EqualTo(newProfile));
        Assert.That(employee.ExperienceProfile, Is.InstanceOf<ExperiencedProfile>());
        Assert.That(((ExperiencedProfile)employee.ExperienceProfile).YearsOfExperience, Is.EqualTo(2));
    }
}

