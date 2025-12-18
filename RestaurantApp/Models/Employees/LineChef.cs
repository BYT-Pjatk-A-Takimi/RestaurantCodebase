using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace RestaurantApp.Models;

public class LineChef : Chef
{
    [JsonInclude]
    private readonly List<string> _tasksAssigned = new();

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
        SetTasksAssigned(tasksAssigned);
    }

    private string _specialization = string.Empty;
    public string Specialization
    {
        get => _specialization;
        private set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Specialization cannot be empty.", nameof(Specialization));
            _specialization = value;
        }
    }

    public IReadOnlyCollection<string> TasksAssigned => _tasksAssigned.AsReadOnly();

    private void SetTasksAssigned(IEnumerable<string> tasks)
    {
        if (tasks == null)
            throw new ArgumentNullException(nameof(tasks), "Tasks assigned cannot be null.");

        var list = new List<string>(tasks);
        if (list.Count == 0)
            throw new ArgumentException("Tasks assigned must contain at least one item.", nameof(tasks));

        foreach (var task in list)
        {
            if (string.IsNullOrWhiteSpace(task))
                throw new ArgumentException("Task cannot be empty.", nameof(tasks));
        }

        _tasksAssigned.Clear();
        _tasksAssigned.AddRange(list);
    }

    public void CookSpecialtyDish(Dish dish) { }

    public void FollowSousChefInstructions() { }
}
