using System;
using System.Text.Json.Serialization;

namespace RestaurantApp.Models;

[JsonPolymorphic]
[JsonDerivedType(typeof(Customer), typeDiscriminator: "Customer")]
[JsonDerivedType(typeof(Employee), typeDiscriminator: "Employee")]
public abstract class Person
{
    private string _firstName = string.Empty;
    private string _lastName = string.Empty;
    private DateOnly _birthDate;
    private string _phoneNumber = string.Empty;

    [JsonConstructor]
    protected Person(string firstName, string lastName, DateOnly birthDate, string phoneNumber)
    {
        FirstName = firstName;
        LastName = lastName;
        BirthDate = birthDate;
        PhoneNumber = phoneNumber;
    }

    public string FirstName
    {
        get => _firstName;
        private set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("First name cannot be empty.", nameof(FirstName));
            _firstName = value;
        }
    }

    public string LastName
    {
        get => _lastName;
        private set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Last name cannot be empty.", nameof(LastName));
            _lastName = value;
        }
    }

    public DateOnly BirthDate
    {
        get => _birthDate;
        private set
        {
            if (value == default)
                throw new ArgumentException("Birth date must be a valid date.", nameof(BirthDate));
            _birthDate = value;
        }
    }

    public string PhoneNumber
    {
        get => _phoneNumber;
        private set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Phone number cannot be empty.", nameof(PhoneNumber));
            _phoneNumber = value;
        }
    }

    public virtual string GetFullName() => $"{FirstName} {LastName}";
}

