using System;
using NUnit.Framework;
using RestaurantApp.Models;

namespace RestaurantApp.Tests.Models;

[TestFixture]
public class CustomerTests
{
    [Test]
    public void Member_UseCredits_AppliesDiscountWhenCreditsAvailable()
    {
        var member = new Member("Berkay", "Bayar", DateOnly.Parse("1999-03-12"), "111-1111", "berkay@example.com", credits: 5, creditPointsRate: 2.5m);
        var originalAmount = 100.00m;
        var expectedDiscount = 5 * 2.5m;
        var expectedFinalAmount = originalAmount - expectedDiscount;

        var result = member.UseCredits(originalAmount);

        Assert.That(result, Is.EqualTo(expectedFinalAmount));
        Assert.That(member.Credits, Is.EqualTo(0));
    }

    [Test]
    public void NonMember_PromoteToMember_CreatesNewMember()
    {
        var firstName = "Berkay";
        var lastName = "Bayar";
        var birthDate = DateOnly.Parse("1999-03-12");
        var phoneNumber = "111-1111";
        var email = "berkay@example.com";
        var nonMember = new NonMember(firstName, lastName, birthDate, phoneNumber, email);
        var initialCreditRate = 3.0m;

        var member = nonMember.beMember(initialCreditRate);

        Assert.That(member, Is.Not.Null);
        Assert.That(member, Is.InstanceOf<Member>());
        Assert.That(member.FirstName, Is.EqualTo(firstName));
        Assert.That(member.LastName, Is.EqualTo(lastName));
        Assert.That(member.BirthDate, Is.EqualTo(birthDate));
        Assert.That(member.PhoneNumber, Is.EqualTo(phoneNumber));
        Assert.That(member.Email, Is.EqualTo(email));
        Assert.That(member.Credits, Is.EqualTo(0));
        Assert.That(member.CreditPointsRate, Is.EqualTo(initialCreditRate));
    }
}

