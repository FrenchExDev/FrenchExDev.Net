namespace FrenchExDev.Net.CSharp.Object.Builder2.Tests;

public class SlowBuilder : AbstractBuilder<SimpleObject>
{
    private readonly ThreadSafeCounter _counter;
    public string? Value { get; set; }

    public SlowBuilder() { _counter = new ThreadSafeCounter(); }
    public SlowBuilder(ThreadSafeCounter counter) { _counter = counter; }

    protected override SimpleObject Instantiate()
    {
        Interlocked.Increment(ref _counter.InstantiateCount);
        Thread.Sleep(10);
        return new() { Value = Value ?? string.Empty };
    }

    public SlowBuilder WithValue(string value) { Value = value; return this; }
    public int GetInstantiateCount() => _counter.InstantiateCount;
}
