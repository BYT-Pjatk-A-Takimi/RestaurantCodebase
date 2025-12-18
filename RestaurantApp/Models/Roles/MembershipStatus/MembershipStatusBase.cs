using System.Text.Json.Serialization;

namespace RestaurantApp.Models.Roles.MembershipStatus;

[JsonPolymorphic]
[JsonDerivedType(typeof(MemberStatus), typeDiscriminator: "MemberStatus")]
[JsonDerivedType(typeof(NonMemberStatus), typeDiscriminator: "NonMemberStatus")]
public abstract class MembershipStatusBase
{
    [JsonIgnore]
    public CustomerRole? CustomerRole { get; private set; }

    protected MembershipStatusBase() { }

    internal void SetCustomerRole(CustomerRole role)
    {
        CustomerRole = role;
    }

    internal void ClearCustomerRole()
    {
        CustomerRole = null;
    }
}
