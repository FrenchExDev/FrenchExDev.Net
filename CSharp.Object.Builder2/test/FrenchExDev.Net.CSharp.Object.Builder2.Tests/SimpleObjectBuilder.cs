namespace FrenchExDev.Net.CSharp.Object.Builder2.Tests;

public class SimpleObjectBuilder : AbstractBuilder<SimpleObject>
{
    public string? Value { get; set; }
    protected override SimpleObject Instantiate() => new() { Value = Value ?? string.Empty };
    public SimpleObjectBuilder WithValue(string value) { Value = value; return this; }
}
