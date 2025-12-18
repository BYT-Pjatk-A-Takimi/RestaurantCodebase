using System;
using NUnit.Framework;
using RestaurantApp.Models;
using RestaurantApp.Models.Roles;
using RestaurantApp.Models.Roles.EmployeeTypes;

namespace RestaurantApp.Tests.Models;

[TestFixture]
public class EmployeeTests
{
    [Test]
    public void EmployeeRole_WorkDetails_IsSetCorrectly()
    {
        var department = "Kitchen";
        var shiftSchedule = "Day";
        var dateOfHiring = DateOnly.FromDateTime(DateTime.Today.AddYears(-2));
        var workDetails = new WorkDetails(department, shiftSchedule, dateOfHiring);
        var experienceProfile = new ExperiencedProfile(5, "Chef Mentor");
        var chefType = new ChefType("Italian");
        var employeeRole = new EmployeeRole(workDetails, experienceProfile, chefType);

        Assert.That(employeeRole.WorkDetails, Is.Not.Null);
        Assert.That(employeeRole.WorkDetails.Department, Is.EqualTo(department));
        Assert.That(employeeRole.WorkDetails.ShiftSchedule, Is.EqualTo(shiftSchedule));
        Assert.That(employeeRole.WorkDetails.DateOfHiring, Is.EqualTo(dateOfHiring));
    }

    [Test]
    public void EmployeeRole_UpdateExperienceProfile_UpdatesProfile()
    {
        var workDetails = new WorkDetails("Service", "Evening", DateOnly.FromDateTime(DateTime.Today));
        var initialProfile = new TraineeProfile(8);
        var waiterType = new WaiterType();
        var employeeRole = new EmployeeRole(workDetails, initialProfile, waiterType);
        var newProfile = new ExperiencedProfile(2, "Senior Waiter");

        employeeRole.UpdateExperienceProfile(newProfile);

        Assert.That(employeeRole.ExperienceProfile, Is.EqualTo(newProfile));
        Assert.That(employeeRole.ExperienceProfile, Is.InstanceOf<ExperiencedProfile>());
        Assert.That(((ExperiencedProfile)employeeRole.ExperienceProfile).YearsOfExperience, Is.EqualTo(2));
    }
}
