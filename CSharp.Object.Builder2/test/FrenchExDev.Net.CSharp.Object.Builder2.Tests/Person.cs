namespace FrenchExDev.Net.CSharp.Object.Builder2.Tests;

public class Person
{
    public string Name { get; set; } = string.Empty;
    public int Age { get; set; }
    public string? Email { get; set; }
    public Address? Address { get; set; }
    public List<Person> Friends { get; set; } = [];
}
