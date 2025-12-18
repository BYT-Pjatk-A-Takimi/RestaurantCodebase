using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace RestaurantApp.Models;

public class SousChef : Chef
{
    [JsonInclude]
    private readonly List<string> _supervisedSections = new();

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
        SetSupervisedSections(supervisedSections);
    }

    public bool DayShift { get; private set; }

    public IReadOnlyCollection<string> SupervisedSections => _supervisedSections.AsReadOnly();

    private void SetSupervisedSections(IEnumerable<string> sections)
    {
        if (sections == null)
            throw new ArgumentNullException(nameof(sections), "Supervised sections cannot be null.");

        var list = new List<string>(sections);
        if (list.Count == 0)
            throw new ArgumentException("Supervised sections must contain at least one item.", nameof(sections));

        foreach (var section in list)
        {
            if (string.IsNullOrWhiteSpace(section))
                throw new ArgumentException("Supervised section cannot be empty.", nameof(sections));
        }

        _supervisedSections.Clear();
        _supervisedSections.AddRange(list);
    }

    public void prepareSpecialists(Dish dish) { }

    public void AssistExecutiveChef() { }
}
