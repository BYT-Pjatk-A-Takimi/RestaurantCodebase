using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace RestaurantApp.Models;

[JsonPolymorphic]
[JsonDerivedType(typeof(HeadChef), typeDiscriminator: "HeadChef")]
[JsonDerivedType(typeof(SousChef), typeDiscriminator: "SousChef")]
[JsonDerivedType(typeof(LineChef), typeDiscriminator: "LineChef")]
public class Chef : Employee
{
    [JsonInclude]
    private Restaurant? _restaurant;

    [JsonConstructor]
    public Chef(
        string firstName,
        string lastName,
        DateOnly birthDate,
        string phoneNumber,
        WorkDetails workDetails,
        EmployeeExperienceProfile experienceProfile,
        string cuisineType)
        : base(firstName, lastName, birthDate, phoneNumber, workDetails, experienceProfile)
    {
        CuisineType = cuisineType;
    }

    public string CuisineType { get; }

    public Restaurant? Restaurant => _restaurant;

    public void assignTask() {}

    public void AddDish(Menu menu, Dish dish) => menu.AddDish(dish);

    public void viewOrders() {}

    public IReadOnlyCollection<Dish> ViewMenu(Menu menu) => menu.Dishes;

    public void UpdateMenu(Menu menu, Dish existingDish, Dish updatedDish) => menu.UpdateDish(existingDish, updatedDish);

    public void SetRestaurant(Restaurant restaurant)
    {
        if (restaurant is null)
            throw new ArgumentNullException(nameof(restaurant));

        if (_restaurant == restaurant)
            throw new ArgumentException("This chef already works at this restaurant.", nameof(restaurant));

        if (_restaurant != null)
            throw new InvalidOperationException("Chef already works at a restaurant. Remove the current restaurant first.");

        _restaurant = restaurant;
        restaurant.AddChef(this);
    }

    internal void SetRestaurantInternal(Restaurant restaurant)
    {
        if (restaurant is null)
            throw new ArgumentNullException(nameof(restaurant));
        _restaurant = restaurant;
    }

    public void RemoveRestaurant()
    {
        if (_restaurant is null)
            return;

        var restaurant = _restaurant;
        _restaurant = null;
        restaurant.RemoveChef(this);
    }
}
