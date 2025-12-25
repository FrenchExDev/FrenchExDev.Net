namespace FrenchExDev.Net.CSharp.Object.Builder2;

/// <summary>
/// Thread-safe reference to an instance supporting deferred resolution.
/// </summary>
public record Reference<TClass> where TClass : class
{
    private volatile TClass? _instance;
    public TClass? Instance => _instance;
    public bool IsResolved => _instance is not null;

    public Reference<TClass> Resolve(TClass instance)
    {
        Interlocked.CompareExchange(ref _instance, instance, null);
        return this;
    }

    public TClass Resolved() => _instance ?? throw new ReferenceNotResolvedException("Reference is not resolved yet.");
    public TClass? ResolvedOrNull() => _instance;

    public Reference() { }
    public Reference(TClass? existing) { _instance = existing; }
}
