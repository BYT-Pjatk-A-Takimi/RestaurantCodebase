using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace RestaurantApp.Models;

public class LineChef : Chef
{
    [JsonInclude]
    private readonly List<string> _tasksAssigned;

    [JsonConstructor]
    public LineChef(
        string firstName,
        string lastName,
        DateOnly birthDate,
        string phoneNumber,
        WorkDetails workDetails,
        EmployeeExperienceProfile experienceProfile,
        string cuisineType,
        string specialization,
        IReadOnlyCollection<string> tasksAssigned)
        : base(firstName, lastName, birthDate, phoneNumber, workDetails, experienceProfile, cuisineType)
    {
        Specialization = specialization;
        _tasksAssigned = new List<string>(tasksAssigned);
    }

    public string Specialization { get; }

    public IReadOnlyCollection<string> TasksAssigned => _tasksAssigned.AsReadOnly();

    public void CookSpecialtyDish(Dish dish) { }

    public void FollowSousChefInstructions() { }
}
