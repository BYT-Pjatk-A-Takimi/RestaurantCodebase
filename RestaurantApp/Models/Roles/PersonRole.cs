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
        if (Person == person)
            return;
            
        Person = person;
        
        if (this is EmployeeRole employeeRole)
        {
            if (person.EmployeeRole != employeeRole)
            {
                person.BecomeEmployee(employeeRole);
            }
        }
        else if (this is CustomerRole customerRole)
        {
            if (person.CustomerRole != customerRole)
            {
                person.BecomeCustomer(customerRole);
            }
        }
    }

    internal void ClearPerson()
    {
        Person = null;
    }
}
