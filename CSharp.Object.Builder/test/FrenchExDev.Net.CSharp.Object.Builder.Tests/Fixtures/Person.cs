namespace FrenchExDev.Net.CSharp.Object.Builder.Tests.Fixtures;

/// <summary>
/// Represents a person with a name, age, and a collection of addresses.
/// </summary>
public class Person
{
    public string Name { get; }

    public int Age { get; }

    public IEnumerable<Address> Addresses { get; }

    public Person(string name, int age, IEnumerable<Address> addresses)
    {
        Name = name;
        Age = age;
        Addresses = addresses;
    }
}
