namespace FrenchExDev.Net.CSharp.Object.Builder.Abstractions;

/// <summary>
/// Abstract base that holds a reference to an object of type <typeparamref name="TClass"/>
/// and allows deferring work (actions) until the reference is set or updated.
/// </summary>
/// <typeparam name="TClass">The type of the referenced object.</typeparam>
/// <remarks>
/// - This type is not thread-safe.
/// - Queued actions are executed only when <see cref="SetReference(TClass)"/> is called.
///   Constructing the instance with a non-null reference does not execute queued actions.
/// - If a reference is already set and you call <see cref="AddAction(System.Action{TClass})"/>,
///   the action will not run until the next call to <see cref="SetReference(TClass)"/>.
/// - Actions are executed in the order they were added (FIFO). If an action throws, execution stops
///   and the exception bubbles up; remaining actions are not executed in that invocation.
/// </remarks>
public abstract class AbstractBuildReference<TClass>
{
    /// <summary>
    /// Actions queued to be executed the next time <see cref="SetReference(TClass)"/> is invoked
    /// with a non-null reference.
    /// </summary>
    /// <remarks>
    /// The list is cleared after a successful execution cycle. No synchronization is performed.
    /// </remarks>
    private List<Action<TClass>> _actions = new();

    /// <summary>
    /// The current referenced object. May be <see langword="null"/> until a value is assigned.
    /// </summary>
    /// <remarks>
    /// Assigning directly to this property (from a derived type) does not trigger execution of queued actions.
    /// Use <see cref="SetReference(TClass)"/> to assign and execute queued actions.
    /// </remarks>
    public TClass? Reference { get; protected set; }

    /// <summary>
    /// Indicates whether <see cref="Reference"/> currently holds a non-null value.
    /// </summary>
    public bool HasReference => Reference is not null;

    /// <summary>
    /// Initializes a new instance with an optional initial reference.
    /// </summary>
    /// <param name="reference">The initial reference value; may be <see langword="null"/>.</param>
    /// <remarks>
    /// Passing a non-null <paramref name="reference"/> does not execute queued actions. To execute them,
    /// call <see cref="SetReference(TClass)"/> explicitly.
    /// </remarks>
    public AbstractBuildReference(TClass? reference)
    {
        Reference = reference;
    }

    /// <summary>
    /// Sets the reference to the specified non-null value and executes all queued actions.
    /// </summary>
    /// <param name="reference">The new reference value. Must not be <see langword="null"/>.</param>
    /// <remarks>
    /// - Executes actions in FIFO order and clears the queue afterwards.
    /// - If an action throws, the exception is propagated and remaining actions in the current cycle are not executed.
    /// </remarks>
    public void SetReference(TClass reference)
    {
        Reference = reference;
        ExecuteActions();
    }

    /// <summary>
    /// Queues an action to be executed the next time <see cref="SetReference(TClass)"/> is called
    /// with a non-null reference.
    /// </summary>
    /// <param name="action">The action to execute. Must not be <see langword="null"/>.</param>
    /// <remarks>
    /// If a reference is already set, the action will not run immediately; it will run on the next
    /// <see cref="SetReference(TClass)"/> call. Duplicate registrations are allowed and will be executed in order.
    /// </remarks>
    public void AddAction(Action<TClass> action)
    {
        _actions.Add(action);
    }

    /// <summary>
    /// Executes all queued actions against the current non-null <see cref="Reference"/> and clears the queue.
    /// </summary>
    /// <remarks>
    /// No work is performed if <see cref="Reference"/> is <see langword="null"/>.
    /// </remarks>
    private void ExecuteActions()
    {
        if (Reference is null) return;

        foreach (var action in _actions)
        {
            action(Reference);
        }
        _actions.Clear();
    }
}
