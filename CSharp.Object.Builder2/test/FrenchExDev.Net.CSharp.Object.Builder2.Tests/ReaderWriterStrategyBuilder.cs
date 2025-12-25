namespace FrenchExDev.Net.CSharp.Object.Builder2.Tests;

public class ReaderWriterStrategyBuilder : AbstractBuilder<SimpleObject>
{
    private static readonly ReaderWriterSynchronizationStrategy _strategy = new();

    public string? Value { get; set; }

    public ReaderWriterStrategyBuilder()
        : base(DefaultReferenceFactory.Instance, _strategy) { }

    protected override SimpleObject Instantiate() => new() { Value = Value ?? string.Empty };

    public ReaderWriterStrategyBuilder WithValue(string value) { Value = value; return this; }
}
