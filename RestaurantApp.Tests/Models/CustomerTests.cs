using System;
using NUnit.Framework;
using RestaurantApp.Models;
using RestaurantApp.Models.Roles;
using RestaurantApp.Models.Roles.MembershipStatus;

namespace RestaurantApp.Tests.Models;

[TestFixture]
public class CustomerTests
{
    [Test]
    public void MemberStatus_UseCredits_AppliesDiscountWhenCreditsAvailable()
    {
        var memberStatus = new MemberStatus(credits: 10, creditPointsRate: 2.5m);
        var originalAmount = 100.00m;
        var expectedDiscount = 10 * 2.5m;
        var expectedFinalAmount = originalAmount - expectedDiscount;

        var result = memberStatus.UseCredits(originalAmount);

        Assert.That(result, Is.EqualTo(expectedFinalAmount));
        Assert.That(memberStatus.Credits, Is.EqualTo(0));
    }

    [Test]
    public void MemberStatus_UseCredits_DoesNotApplyDiscountWhenCreditsBelowThreshold()
    {
        var memberStatus = new MemberStatus(credits: 9, creditPointsRate: 2.5m);
        var originalAmount = 100.00m;

        var result = memberStatus.UseCredits(originalAmount);

        Assert.That(result, Is.EqualTo(originalAmount));
        Assert.That(memberStatus.Credits, Is.EqualTo(9));
    }

    [Test]
    public void NonMemberStatus_PromoteToMember_CreatesNewMemberStatus()
    {
        var nonMemberStatus = new NonMemberStatus();
        var initialCreditRate = 3.0m;

        var memberStatus = nonMemberStatus.PromoteToMember(initialCreditRate);

        Assert.That(memberStatus, Is.Not.Null);
        Assert.That(memberStatus, Is.InstanceOf<MemberStatus>());
        Assert.That(memberStatus.Credits, Is.EqualTo(0));
        Assert.That(memberStatus.CreditPointsRate, Is.EqualTo(initialCreditRate));
    }

    [Test]
    public void CustomerRole_CanChangeMembershipStatus()
    {
        var nonMemberStatus = new NonMemberStatus();
        var customerRole = new CustomerRole("test@example.com", nonMemberStatus);
        var person = new Person("Berkay", "Bayar", DateOnly.Parse("1999-03-12"), "111-1111", customerRole: customerRole);

        var memberStatus = nonMemberStatus.PromoteToMember(2.5m);
        customerRole.SetMembershipStatus(memberStatus);

        Assert.That(customerRole.MembershipStatus, Is.InstanceOf<MemberStatus>());
    }
}
