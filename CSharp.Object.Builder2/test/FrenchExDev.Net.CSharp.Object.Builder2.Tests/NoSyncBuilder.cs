namespace FrenchExDev.Net.CSharp.Object.Builder2.Tests;

public class NoSyncBuilder : AbstractBuilder<SimpleObject>
{
    public string? Value { get; set; }

    public NoSyncBuilder()
        : base(DefaultReferenceFactory.Instance, NoSynchronizationStrategy.Instance) { }

    protected override SimpleObject Instantiate() => new() { Value = Value ?? string.Empty };

    public NoSyncBuilder WithValue(string value) { Value = value; return this; }
}
