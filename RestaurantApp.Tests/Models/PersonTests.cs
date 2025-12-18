using System;
using NUnit.Framework;
using RestaurantApp.Models;
using RestaurantApp.Models.Roles;
using RestaurantApp.Models.Roles.EmployeeTypes;
using RestaurantApp.Models.Roles.MembershipStatus;

namespace RestaurantApp.Tests.Models;

[TestFixture]
public class PersonTests
{
    [Test]
    public void Person_GetFullName_ReturnsCorrectFullName()
    {
        var workDetails = new WorkDetails("Kitchen", "Day", DateOnly.FromDateTime(DateTime.Today));
        var experienceProfile = new ExperiencedProfile(3, "Mentor");
        var managerType = new ManagerType(level: 1);
        var employeeRole = new EmployeeRole(workDetails, experienceProfile, managerType);
        var person = new Person("Berkay", "Bayar", DateOnly.Parse("1999-03-12"), "111-1111", employeeRole: employeeRole);

        var fullName = person.GetFullName();

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
        var waiterType = new WaiterType();
        var employeeRole = new EmployeeRole(workDetails, experienceProfile, waiterType);
        var person = new Person(firstName, lastName, birthDate, phoneNumber, employeeRole: employeeRole);

        Assert.That(person.FirstName, Is.EqualTo(firstName));
        Assert.That(person.LastName, Is.EqualTo(lastName));
        Assert.That(person.BirthDate, Is.EqualTo(birthDate));
        Assert.That(person.PhoneNumber, Is.EqualTo(phoneNumber));
    }

    [Test]
    public void Person_CanBeBothEmployeeAndCustomer()
    {
        var workDetails = new WorkDetails("Service", "Evening", DateOnly.FromDateTime(DateTime.Today));
        var experienceProfile = new TraineeProfile(4);
        var waiterType = new WaiterType();
        var employeeRole = new EmployeeRole(workDetails, experienceProfile, waiterType);

        var memberStatus = new MemberStatus(5, 2.5m);
        var customerRole = new CustomerRole("test@example.com", memberStatus);

        var person = new Person("Berkay", "Bayar", DateOnly.Parse("1999-03-12"), "111-1111", employeeRole, customerRole);

        Assert.That(person.IsEmployee, Is.True);
        Assert.That(person.IsCustomer, Is.True);
    }

    [Test]
    public void Person_CanStopBeingEmployee()
    {
        var workDetails = new WorkDetails("Service", "Evening", DateOnly.FromDateTime(DateTime.Today));
        var experienceProfile = new TraineeProfile(4);
        var employeeRole = new EmployeeRole(workDetails, experienceProfile);
        var person = new Person("Berkay", "Bayar", DateOnly.Parse("1999-03-12"), "111-1111", employeeRole: employeeRole);

        Assert.That(person.IsEmployee, Is.True);

        person.StopBeingEmployee();

        Assert.That(person.IsEmployee, Is.False);
    }
}
