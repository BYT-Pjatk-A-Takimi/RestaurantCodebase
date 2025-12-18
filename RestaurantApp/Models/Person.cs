using System;
using System.Text.Json.Serialization;
using RestaurantApp.Models.Roles;

namespace RestaurantApp.Models;

public class Person
{
    private string _firstName = string.Empty;
    private string _lastName = string.Empty;
    private DateOnly _birthDate;
    private string _phoneNumber = string.Empty;

    [JsonInclude]
    private EmployeeRole? _employeeRole;
    [JsonInclude]
    private CustomerRole? _customerRole;

    [JsonConstructor]
    public Person(
        string firstName,
        string lastName,
        DateOnly birthDate,
        string phoneNumber,
        EmployeeRole? employeeRole = null,
        CustomerRole? customerRole = null)
    {
        FirstName = firstName;
        LastName = lastName;
        BirthDate = birthDate;
        PhoneNumber = phoneNumber;

        if (employeeRole != null)
        {
            BecomeEmployee(employeeRole);
        }

        if (customerRole != null)
        {
            BecomeCustomer(customerRole);
        }
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

    public EmployeeRole? EmployeeRole => _employeeRole;
    public CustomerRole? CustomerRole => _customerRole;

    [JsonIgnore]
    public bool IsEmployee => _employeeRole != null;

    [JsonIgnore]
    public bool IsCustomer => _customerRole != null;

    public string GetFullName() => $"{FirstName} {LastName}";

    public void BecomeEmployee(EmployeeRole role)
    {
        if (role is null)
            throw new ArgumentNullException(nameof(role));

        if (_employeeRole != null)
        {
            _employeeRole.ClearPerson();
        }

        _employeeRole = role;
        _employeeRole.SetPerson(this);
    }

    public void BecomeCustomer(CustomerRole role)
    {
        if (role is null)
            throw new ArgumentNullException(nameof(role));

        if (_customerRole != null)
        {
            _customerRole.ClearPerson();
        }

        _customerRole = role;
        _customerRole.SetPerson(this);
    }

    public void StopBeingEmployee()
    {
        if (_employeeRole != null)
        {
            _employeeRole.ClearPerson();
            _employeeRole = null;
        }
    }

    public void StopBeingCustomer()
    {
        if (_customerRole != null)
        {
            _customerRole.ClearPerson();
            _customerRole = null;
        }
    }
}
