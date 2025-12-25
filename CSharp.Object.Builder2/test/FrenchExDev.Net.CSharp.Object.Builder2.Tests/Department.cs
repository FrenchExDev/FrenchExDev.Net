namespace FrenchExDev.Net.CSharp.Object.Builder2.Tests;

public class Department
{
    public string Name { get; set; } = string.Empty;
    public Employee? Manager { get; set; }
    public List<Employee> Employees { get; set; } = [];
}
