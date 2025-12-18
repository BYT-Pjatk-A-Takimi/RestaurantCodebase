using System.Text.Json.Serialization;
using RestaurantApp.Models.Roles.EmployeeTypes;

namespace RestaurantApp.Models.Roles.ChefRanks;

[JsonPolymorphic]
[JsonDerivedType(typeof(HeadChefRank), typeDiscriminator: "HeadChefRank")]
[JsonDerivedType(typeof(SousChefRank), typeDiscriminator: "SousChefRank")]
[JsonDerivedType(typeof(LineChefRank), typeDiscriminator: "LineChefRank")]
public abstract class ChefRank
{
    [JsonIgnore]
    public ChefType? ChefType { get; private set; }

    protected ChefRank() { }

    internal void SetChefType(ChefType chefType)
    {
        ChefType = chefType;
    }

    internal void ClearChefType()
    {
        ChefType = null;
    }
}
