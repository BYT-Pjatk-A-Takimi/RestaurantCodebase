using System.Text.Json.Serialization;

namespace RestaurantApp.Models.Roles.EmployeeTypes;

[JsonPolymorphic]
[JsonDerivedType(typeof(ChefType), typeDiscriminator: "ChefType")]
[JsonDerivedType(typeof(WaiterType), typeDiscriminator: "WaiterType")]
[JsonDerivedType(typeof(ManagerType), typeDiscriminator: "ManagerType")]
[JsonDerivedType(typeof(ValetType), typeDiscriminator: "ValetType")]
public abstract class EmployeeType
{
    [JsonIgnore]
    public EmployeeRole? EmployeeRole { get; private set; }

    protected EmployeeType() { }

    internal void SetEmployeeRole(EmployeeRole role)
    {
        EmployeeRole = role;
    }

    internal void ClearEmployeeRole()
    {
        EmployeeRole = null;
    }
}
