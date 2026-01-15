using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using RestaurantApp.Models.Roles.ChefRanks;

namespace RestaurantApp.Models.Roles.EmployeeTypes;

public class ChefType : EmployeeType
{
    [JsonInclude]
    private Restaurant? _restaurant;

    private string _cuisineType = string.Empty;
    private ChefRank? _chefRank;

    [JsonConstructor]
    public ChefType(string cuisineType, ChefRank? chefRank = null)
    {
        CuisineType = cuisineType;
        ChefRank = chefRank;
    }

    public string CuisineType
    {
        get => _cuisineType;
        private set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Cuisine type cannot be empty.", nameof(CuisineType));
            _cuisineType = value;
        }
    }

    public ChefRank? ChefRank
    {
        get => _chefRank;
        private set
        {
            if (_chefRank != null)
            {
                _chefRank.ClearChefType();
            }
            _chefRank = value;
            if (_chefRank != null)
            {
                _chefRank.SetChefType(this);
            }
        }
    }

    public Restaurant? Restaurant => _restaurant;

    public void AssignTask() { }

    public void AddDish(Menu menu, Dish dish) => menu.AddDish(dish);

    public void ViewOrders() { }

    public IReadOnlyCollection<Dish> ViewMenu(Menu menu) => menu.Dishes;

    public void UpdateMenu(Menu menu, Dish existingDish, Dish updatedDish) => menu.UpdateDish(existingDish, updatedDish);

    public void SetRestaurant(Restaurant restaurant)
    {
        if (restaurant is null)
            throw new ArgumentNullException(nameof(restaurant));

        if (_restaurant == restaurant)
            return;

        if (_restaurant != null)
            throw new InvalidOperationException("Chef already works at a restaurant. Remove the current restaurant first.");

        _restaurant = restaurant;
        if (!restaurant.Chefs.Contains(this))
        {
            restaurant.AddChef(this);
        }
    }

    internal void SetRestaurantInternal(Restaurant restaurant)
    {
        if (restaurant is null)
            throw new ArgumentNullException(nameof(restaurant));
            
        if (_restaurant == restaurant)
            return;

        _restaurant = restaurant;
        
        if (!restaurant.Chefs.Contains(this))
        {
            restaurant.AddChef(this);
        }
    }

    public void RemoveRestaurant()
    {
        _restaurant = null;
    }

    public void SetChefRank(ChefRank rank)
    {
        ChefRank = rank;
    }

    public void ClearChefRank()
    {
        ChefRank = null;
    }
}
