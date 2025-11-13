using System;

namespace RestaurantApp.Models;

public abstract class Person
{
    protected Person(string firstName, string lastName, DateOnly birthDate, string phoneNumber)
    {
        FirstName = firstName;
        LastName = lastName;
        BirthDate = birthDate;
        PhoneNumber = phoneNumber;
    }

    public string FirstName { get; }

    public string LastName { get; }

    public DateOnly BirthDate { get; }

    public string PhoneNumber { get; }

    public virtual string GetFullName() => $"{FirstName} {LastName}";
}

