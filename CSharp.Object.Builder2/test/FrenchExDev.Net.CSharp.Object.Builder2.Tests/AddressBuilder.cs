namespace FrenchExDev.Net.CSharp.Object.Builder2.Tests;

public class AddressBuilder : AbstractBuilder<Address>
{
    public string? Street { get; set; }
    public string? City { get; set; }
    public string? ZipCode { get; set; }

    protected override Address Instantiate() => new() { Street = Street!, City = City!, ZipCode = ZipCode };

    protected override void ValidateInternal(VisitedObjectDictionary visitedCollector, IFailureCollector failures)
    {
        AssertNotNullOrEmptyOrWhitespace(Street, nameof(Street), failures, n => new StringIsEmptyOrWhitespaceException(n));
        AssertNotNullOrEmptyOrWhitespace(City, nameof(City), failures, n => new StringIsEmptyOrWhitespaceException(n));
    }

    public AddressBuilder WithStreet(string street) { Street = street; return this; }
    public AddressBuilder WithCity(string city) { City = city; return this; }
    public AddressBuilder WithZipCode(string zipCode) { ZipCode = zipCode; return this; }
}
