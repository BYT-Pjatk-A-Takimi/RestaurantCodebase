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
        if (CustomerRole == role)
            return;

        CustomerRole = role;
        
        if (role != null && role.MembershipStatus != this)
        {
            role.SetMembershipStatus(this);
        }
    }

    internal void ClearCustomerRole()
    {
        CustomerRole = null;
    }
}
