namespace FrenchExDev.Net.CSharp.Object.Builder2;

/// <summary>
/// Represents a thread-safe, lazily-resolved reference to an instance of type <typeparamref name="TClass"/>.
/// Supports deferred resolution, enabling circular reference handling in object graphs.
/// </summary>
/// <typeparam name="TClass">The type of object being referenced. Must be a reference type.</typeparam>
/// <remarks>
/// <para>
/// <see cref="Reference{TClass}"/> is a core component of the Builder pattern implementation that enables:
/// </para>
/// <list type="bullet">
///   <item><description>Deferred object resolution - reference objects before they're built</description></item>
///   <item><description>Circular reference handling - objects can reference each other</description></item>
///   <item><description>Thread-safe resolution - multiple threads can safely resolve the same reference</description></item>
///   <item><description>Callback registration - execute actions when the reference is resolved</description></item>
/// </list>
/// <para>
/// The reference can only be resolved once. Subsequent calls to <see cref="Resolve"/> with a different
/// instance are ignored, ensuring consistency in concurrent scenarios.
/// </para>
/// </remarks>
/// <example>
/// <code>
/// // Create an unresolved reference
/// var reference = new Reference&lt;Person&gt;();
/// Console.WriteLine(reference.IsResolved); // false
/// 
/// // Resolve the reference
/// var person = new Person { Name = "Alice" };
/// reference.Resolve(person);
/// Console.WriteLine(reference.IsResolved); // true
/// Console.WriteLine(reference.Resolved().Name); // "Alice"
/// 
/// // Use with builders for circular references
/// var deptBuilder = new DepartmentBuilder().WithName("Engineering");
/// deptBuilder.WithEmployee(e => e
///     .WithName("Alice")
///     .WithDepartment(deptBuilder.Reference())); // Reference before build
/// </code>
/// </example>
/// <seealso cref="ReferenceList{TClass}"/>
/// <seealso cref="ReferenceNotResolvedException"/>
/// <seealso cref="IReferenceable{TClass}"/>
public record Reference<TClass> where TClass : class
{
    private volatile TClass? _instance;
    private object _resolveLock = new();
    private List<Action<TClass>> _onResolve = new();

    /// <summary>
    /// Gets the current instance of type <typeparamref name="TClass"/> if the reference has been resolved.
    /// </summary>
    /// <value>
    /// The resolved instance, or <see langword="null"/> if the reference has not been resolved yet.
    /// </value>
    /// <remarks>
    /// This property provides direct access to the underlying instance without throwing exceptions.
    /// For safer access patterns, consider using <see cref="Resolved"/> or <see cref="ResolvedOrNull"/>.
    /// </remarks>
    public TClass? Instance => _instance;

    /// <summary>
    /// Gets a value indicating whether the reference has been resolved with a valid instance.
    /// </summary>
    /// <value>
    /// <see langword="true"/> if <see cref="Resolve"/> has been called with a non-null instance;
    /// otherwise, <see langword="false"/>.
    /// </value>
    public bool IsResolved => _instance is not null;

    /// <summary>
    /// Resolves the reference by associating it with the specified instance and triggers any registered
    /// <see cref="OnResolve"/> callbacks.
    /// </summary>
    /// <param name="instance">The instance to associate with this reference. Must not be <see langword="null"/>.</param>
    /// <returns>The current <see cref="Reference{TClass}"/> instance for method chaining.</returns>
    /// <remarks>
    /// <para>
    /// This method is thread-safe. If called concurrently, only the first call will set the instance.
    /// Subsequent calls with different instances are silently ignored to ensure consistency.
    /// </para>
    /// <para>
    /// Registered <see cref="OnResolve"/> callbacks are executed exactly once, immediately after 
    /// the first successful resolution. The callback list is cleared after execution.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// var reference = new Reference&lt;Person&gt;();
    /// reference.OnResolve(p => Console.WriteLine($"Resolved: {p.Name}"));
    /// 
    /// var person = new Person { Name = "Alice" };
    /// reference.Resolve(person); // Prints: "Resolved: Alice"
    /// reference.Resolve(new Person { Name = "Bob" }); // Ignored, already resolved
    /// </code>
    /// </example>
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
    /// Returns the resolved instance, throwing an exception if the reference has not been resolved.
    /// </summary>
    /// <returns>The resolved <typeparamref name="TClass"/> instance.</returns>
    /// <exception cref="ReferenceNotResolvedException">
    /// Thrown when the reference has not been resolved yet.
    /// </exception>
    /// <remarks>
    /// Use this method when you expect the reference to be resolved and want to fail fast if it isn't.
    /// For scenarios where an unresolved reference is acceptable, use <see cref="ResolvedOrNull"/> instead.
    /// </remarks>
    /// <example>
    /// <code>
    /// var reference = new Reference&lt;Person&gt;(new Person { Name = "Alice" });
    /// var person = reference.Resolved(); // Returns the person
    /// 
    /// var unresolvedRef = new Reference&lt;Person&gt;();
    /// var p = unresolvedRef.Resolved(); // Throws ReferenceNotResolvedException
    /// </code>
    /// </example>
    public TClass Resolved() => _instance ?? throw new ReferenceNotResolvedException("Reference is not resolved yet.");

    /// <summary>
    /// Returns the resolved instance, or <see langword="null"/> if the reference has not been resolved.
    /// </summary>
    /// <returns>
    /// The resolved <typeparamref name="TClass"/> instance if available; otherwise, <see langword="null"/>.
    /// </returns>
    /// <remarks>
    /// This method is useful in <c>Instantiate()</c> implementations where a referenced object
    /// may not exist (optional relationship) or may be resolved later (circular reference).
    /// </remarks>
    /// <example>
    /// <code>
    /// protected override Employee Instantiate() => new()
    /// {
    ///     Name = Name!,
    ///     Department = DepartmentRef?.ResolvedOrNull() // May be null if not resolved yet
    /// };
    /// </code>
    /// </example>
    public TClass? ResolvedOrNull() => _instance;

    /// <summary>
    /// Initializes a new instance of the <see cref="Reference{TClass}"/> class with no initial value.
    /// </summary>
    /// <remarks>
    /// The reference will be in an unresolved state until <see cref="Resolve"/> is called.
    /// </remarks>
    public Reference() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="Reference{TClass}"/> class with an existing instance.
    /// </summary>
    /// <param name="existing">
    /// The existing instance to wrap. Can be <see langword="null"/> to create an unresolved reference.
    /// </param>
    /// <remarks>
    /// If <paramref name="existing"/> is not <see langword="null"/>, the reference is immediately resolved.
    /// This constructor is useful when wrapping existing objects in the reference system.
    /// </remarks>
    /// <example>
    /// <code>
    /// var existingPerson = new Person { Name = "Alice" };
    /// var reference = new Reference&lt;Person&gt;(existingPerson);
    /// Console.WriteLine(reference.IsResolved); // true
    /// </code>
    /// </example>
    public Reference(TClass? existing) { _instance = existing; }

    /// <summary>
    /// Registers a callback action to be executed when the reference is resolved.
    /// </summary>
    /// <param name="action">
    /// An action that receives the resolved instance. Will be called exactly once when <see cref="Resolve"/> succeeds.
    /// </param>
    /// <returns>The current <see cref="Reference{TClass}"/> instance for method chaining.</returns>
    /// <remarks>
    /// <para>
    /// Callbacks are executed in the order they were registered, immediately after the first successful 
    /// <see cref="Resolve"/> call. If the reference is already resolved when <see cref="OnResolve"/> is called,
    /// the callback will not be executed (it's intended for future resolution).
    /// </para>
    /// <para>
    /// This is useful for setting up relationships that depend on the resolved instance.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// var reference = new Reference&lt;Department&gt;();
    /// reference
    ///     .OnResolve(dept => Console.WriteLine($"Department created: {dept.Name}"))
    ///     .OnResolve(dept => dept.Initialize());
    /// </code>
    /// </example>
    public Reference<TClass> OnResolve(Action<TClass> action)
    {
        _onResolve.Add(action);
        return this;
    }
}
