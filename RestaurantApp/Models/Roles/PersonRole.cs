using System.Text.Json.Serialization;

namespace RestaurantApp.Models.Roles;

[JsonPolymorphic]
[JsonDerivedType(typeof(EmployeeRole), typeDiscriminator: "EmployeeRole")]
[JsonDerivedType(typeof(CustomerRole), typeDiscriminator: "CustomerRole")]
public abstract class PersonRole
{
    [JsonIgnore]
    public Person? Person { get; private set; }

    protected PersonRole() { }

    internal void SetPerson(Person person)
    {
        Person = person;
    }

    internal void ClearPerson()
    {
        Person = null;
    }
}
