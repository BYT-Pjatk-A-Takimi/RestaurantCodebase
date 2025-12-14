using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace RestaurantApp.Models;

public class SousChef : Chef
{
    [JsonInclude]
    private readonly List<string> _supervisedSections;

    [JsonConstructor]
    public SousChef(
        string firstName,
        string lastName,
        DateOnly birthDate,
        string phoneNumber,
        WorkDetails workDetails,
        EmployeeExperienceProfile experienceProfile,
        string cuisineType,
        bool dayShift,
        IReadOnlyCollection<string> supervisedSections)
        : base(firstName, lastName, birthDate, phoneNumber, workDetails, experienceProfile, cuisineType)
    {
        DayShift = dayShift;
        _supervisedSections = new List<string>(supervisedSections);
    }

    public bool DayShift { get; }

    public IReadOnlyCollection<string> SupervisedSections => _supervisedSections.AsReadOnly();

    public void prepareSpecialists(Dish dish) { }

    public void AssistExecutiveChef() { }
}
