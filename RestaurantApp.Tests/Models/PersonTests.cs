using System;
using NUnit.Framework;
using RestaurantApp.Models;

namespace RestaurantApp.Tests.Models;

[TestFixture]
public class PersonTests
{
    [Test]
    public void Person_GetFullName_ReturnsCorrectFullName()
    {
        var workDetails = new WorkDetails("Kitchen", "Day", DateOnly.FromDateTime(DateTime.Today));
        var experienceProfile = new ExperiencedProfile(3, "Mentor");
        var employee = new Manager("Berkay", "Bayar", DateOnly.Parse("1999-03-12"), "111-1111", workDetails, experienceProfile, level: 1);

        var fullName = employee.GetFullName();

        Assert.That(fullName, Is.EqualTo("Berkay Bayar"));
    }

    [Test]
    public void Person_Properties_AreInitializedCorrectly()
    {
        var firstName = "Berkay";
        var lastName = "Bayar";
        var birthDate = DateOnly.Parse("1999-03-12");
        var phoneNumber = "111-1111";
        var workDetails = new WorkDetails("Service", "Evening", DateOnly.FromDateTime(DateTime.Today));
        var experienceProfile = new TraineeProfile(4);
        var employee = new Waiter(firstName, lastName, birthDate, phoneNumber, workDetails, experienceProfile);

        Assert.That(employee.FirstName, Is.EqualTo(firstName));
        Assert.That(employee.LastName, Is.EqualTo(lastName));
        Assert.That(employee.BirthDate, Is.EqualTo(birthDate));
        Assert.That(employee.PhoneNumber, Is.EqualTo(phoneNumber));
    }
}

