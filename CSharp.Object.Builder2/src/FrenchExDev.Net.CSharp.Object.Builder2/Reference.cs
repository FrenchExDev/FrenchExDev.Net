namespace FrenchExDev.Net.CSharp.Object.Builder2;

/// <summary>
/// Thread-safe reference to an instance supporting deferred resolution.
/// </summary>
public record Reference<TClass> where TClass : class
{
    private volatile TClass? _instance;
    private object _resolveLock = new();
    private List<Action<TClass>> _onResolve = new();

    /// <summary>
    /// Gets the current instance of type <typeparamref name="TClass"/> if available.
    /// </summary>
    public TClass? Instance => _instance;

    /// <summary>
    /// Gets a value indicating whether the instance has been resolved and is available for use.
    /// </summary>
    public bool IsResolved => _instance is not null;

    /// <summary>
    /// Resolves the reference by associating it with the specified instance and triggers any registered resolve
    /// actions.
    /// </summary>
    /// <remarks>If the reference has already been resolved, subsequent calls with the same instance will not
    /// re-trigger resolve actions. This method is thread-safe.</remarks>
    /// <param name="instance">The instance of type TClass to associate with this reference. Cannot be null.</param>
    /// <returns>The current Reference<TClass> object, now associated with the specified instance.</returns>
    public Reference<TClass> Resolve(TClass instance)
    {
        Interlocked.CompareExchange(ref _instance, instance, null);

        lock (_resolveLock)
        {
            if (_instance == instance)
            {
                foreach (var action in _onResolve)
                {
                    action(instance);
                }
                _onResolve.Clear();
            }
        }

        return this;
    }

    /// <summary>
    /// Returns the resolved instance of type <typeparamref name="TClass"/> associated with this reference.
    /// </summary>
    /// <returns>The resolved <typeparamref name="TClass"/> instance. If the reference has not been resolved, an exception is
    /// thrown.</returns>
    /// <exception cref="ReferenceNotResolvedException">Thrown if the reference has not been resolved and no instance is available.</exception>
    public TClass Resolved() => _instance ?? throw new ReferenceNotResolvedException("Reference is not resolved yet.");

    /// <summary>
    /// Returns the resolved instance of type <typeparamref name="TClass"/>, or <see langword="null"/> if no instance is
    /// available.
    /// </summary>
    /// <returns>The resolved <typeparamref name="TClass"/> instance if available; otherwise, <see langword="null"/>.</returns>
    public TClass? ResolvedOrNull() => _instance;

    /// <summary>
    /// Initializes a new instance of the Reference class.
    /// </summary>
    public Reference() { }

    /// <summary>
    /// Initializes a new instance of the Reference class that wraps the specified object.
    /// </summary>
    /// <param name="existing">The object to be referenced. Can be null to indicate no initial value.</param>
    public Reference(TClass? existing) { _instance = existing; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="action"></param>
    /// <returns></returns>
    public Reference<TClass> OnResolve(Action<TClass> action)
    {
        _onResolve.Add(action);
        return this;
    }
}
