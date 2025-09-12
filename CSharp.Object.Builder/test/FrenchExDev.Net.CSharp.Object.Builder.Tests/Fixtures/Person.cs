namespace FrenchExDev.Net.CSharp.Object.Builder.Tests.Fixtures;

/// <summary>
/// Represents a person with a name, age, and a collection of addresses.
/// </summary>
public class Person
{
    public string Name { get; }

    public int Age { get; }

    public IEnumerable<Address> Addresses { get; }

    public IEnumerable<Person> Knows { get; }

    public Person(string name, int age, IEnumerable<Address> addresses, IEnumerable<Person> knows)
    {
        Name = name;
        Age = age;
        Addresses = addresses;
        Knows = knows;
    }
}
