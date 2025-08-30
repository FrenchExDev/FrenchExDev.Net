namespace FernchExDev.Net.CSharp.Object.Builder.Tests.Fixtures;

/// <summary>
/// Represents a physical address, including a street name and a ZIP code.
/// </summary>
/// <remarks>This class is used to store and manage address information. Both the street name and ZIP code
/// are required when creating an instance of the <see cref="Address"/> class.</remarks>
internal class Address
{
    public string? Street { get; set; }
    public string? ZipCode { get; set; }

    public Address(string street, string zipCode)
    {
        Street = street;
        ZipCode = zipCode;
    }
}