using System.Collections;

namespace FrenchExDev.Net.CSharp.ManagedList;

/// <summary>
/// Fluent builder used to configure callback actions that are invoked when items are added, removed
/// or cleared from a managed list. The builder can materialize either an <see cref="OpenManagedList{T}"/>
/// (callbacks can still be extended after creation) or a <see cref="ClosedManagedList{T}"/> (callbacks are frozen).
/// </summary>
/// <typeparam name="T">Element type.</typeparam>
/// <remarks>
/// The callbacks registered through this builder are accumulated and reused for every list instance
/// produced (they share the same underlying collections). Creating multiple lists from the same builder
/// will therefore make them share callbacks and the internal storage list.
/// <para>Thread safety: This builder and the produced lists are <b>not</b> thread-safe.</para>
/// </remarks>
/// <example>
/// Basic usage:
/// <code>
/// var builder = new ManagedListBuilder<string>()
///     .OnAdd(s => Console.WriteLine($"Added: {s}"))
///     .OnRemove(s => Console.WriteLine($"Removed: {s}"))
///     .OnClear(s => Console.WriteLine($"Clearing: {s}"));
///
/// var closed = builder.Closed(); // Cannot add more callbacks afterwards
/// var open   = builder.Open();   // Can add more callbacks via open.OnAdd/OnRemove/OnClear
///
/// closed.Add("A");
/// closed.Remove("A");
/// open.Add("B");
/// open.Clear();
/// </code>
/// </example>
public class ManagedListBuilder<T>
{
    // Internal shared state (lists + callbacks) reused by produced managed lists.
    private readonly List<T> _internals = new();
    private readonly OnAddList<T> _onAdd = new();
    private readonly OnRemoveList<T> _onRemove = new();
    private readonly onClearList<T> _onClear = new();

    /// <summary>
    /// Registers an action invoked for every item <em>before</em> it is added through <see cref="ManagedList{T}.Add"/>.
    /// </summary>
    /// <param name="action">Callback receiving the item.</param>
    /// <returns>This builder (for fluent chaining).</returns>
    public ManagedListBuilder<T> OnAdd(Action<T> action)
    {
        _onAdd.Add(action);
        return this;
    }

    /// <summary>
    /// Registers an action invoked for every item <em>before</em> it is removed through <see cref="ManagedList{T}.Remove(T)"/>
    /// or <see cref="ManagedList{T}.RemoveAt(int)"/>.
    /// </summary>
    /// <param name="action">Callback receiving the item being removed.</param>
    /// <returns>This builder.</returns>
    public ManagedListBuilder<T> OnRemove(Action<T> action)
    {
        _onRemove.Add(action);
        return this;
    }

    /// <summary>
    /// Registers an action invoked once per existing item when <see cref="ManagedList{T}.Clear"/> is called,
    /// prior to the internal list being emptied.
    /// </summary>
    /// <param name="action">Callback receiving each item about to be cleared.</param>
    /// <returns>This builder.</returns>
    public ManagedListBuilder<T> OnClear(Action<T> action)
    {
        _onClear.Add(action);
        return this;
    }

    /// <summary>
    /// Produces a <see cref="ClosedManagedList{T}"/> whose callback collections cannot be extended after creation.
    /// </summary>
    public ClosedManagedList<T> Closed()
    {
        return new ClosedManagedList<T>(_internals, _onAdd, _onRemove, _onClear);
    }

    /// <summary>
    /// Produces an <see cref="OpenManagedList{T}"/> that still allows adding callbacks (OnAdd/OnRemove/OnClear).
    /// </summary>
    public OpenManagedList<T> Open()
    {
        return new OpenManagedList<T>(_internals, _onAdd, _onRemove, _onClear);
    }
}

/// <summary>
/// Internal list of Add callbacks (public only so derived lists can reference; not intended for external use directly).
/// </summary>
public class OnAddList<T> : List<Action<T>> { }

/// <summary>
/// Internal list of Remove callbacks.
/// </summary>
public class OnRemoveList<T> : List<Action<T>> { }

/// <summary>
/// Internal list of Clear callbacks (named with lowercase leading 'o' to preserve legacy usage).
/// </summary>
public class onClearList<t> : List<Action<t>> { }

/// <summary>
/// Managed list variant that permits adding more callbacks after construction.
/// </summary>
/// <typeparam name="T">Element type.</typeparam>
/// <remarks>
/// Additional callbacks registered through the fluent methods modify the shared callback collections and therefore
/// affect every list (open or closed) created from the same builder/source if they share those collections.
/// </remarks>
public class OpenManagedList<T> : ManagedList<T>
{
    /// <summary>Creates a fresh open managed list with isolated callbacks.</summary>
    public OpenManagedList() : base() { }

    /// <summary>
    /// Internal/advanced constructor reusing an existing internal storage and callback collections.
    /// </summary>
    public OpenManagedList(List<T> internals, OnAddList<T> onAdd, OnRemoveList<T> onRemove, onClearList<T> onClear)
        : base(internals, onAdd, onRemove, onClear) { }

    /// <summary>Registers an Add callback (same semantics as builder).</summary>
    public OpenManagedList<T> OnAdd(Action<T> action)
    {
        _onAdd.Add(action);
        return this;
    }

    /// <summary>Registers a Remove callback (same semantics as builder).</summary>
    public OpenManagedList<T> OnRemove(Action<T> action)
    {
        _onRemove.Add(action);
        return this;
    }

    /// <summary>Registers a Clear callback (same semantics as builder).</summary>
    public OpenManagedList<T> OnClear(Action<T> action)
    {
        _onClear.Add(action);
        return this;
    }
}

/// <summary>
/// Managed list variant whose callback collections are not exposed for further extension.
/// </summary>
/// <typeparam name="T">Element type.</typeparam>
public class ClosedManagedList<T> : ManagedList<T>
{
    /// <summary>Creates a fresh closed managed list with isolated callbacks.</summary>
    public ClosedManagedList() : base() { }

    /// <summary>
    /// Internal constructor reusing existing internal storage & callback collections.
    /// </summary>
    public ClosedManagedList(List<T> internals, OnAddList<T> onAdd, OnRemoveList<T> onRemove, onClearList<T> onClear)
        : base(internals, onAdd, onRemove, onClear) { }
}

/// <summary>
/// Abstract base implementation of <see cref="IList{T}"/> that introduces lifecycle callbacks for Add, Remove and Clear operations.
/// </summary>
/// <typeparam name="T">Element type.</typeparam>
/// <remarks>
/// Callback invocation order:
/// <list type="bullet">
/// <item><description><b>Add:</b> Callbacks are invoked before the item is appended (See <see cref="Add"/>). For <see cref="Insert"/>, callbacks are invoked after insertion (inconsistent behavior; consider normalizing if needed).</description></item>
/// <item><description><b>Remove / RemoveAt:</b> Callbacks are invoked before the item is removed.</description></item>
/// <item><description><b>Clear:</b> For each registered clear callback, every existing item is passed then the list is emptied.</description></item>
/// </list>
/// Exceptions thrown inside callbacks are propagated and will abort the underlying list mutation if thrown before it occurs.
/// The implementation is not thread-safe.
/// </remarks>
/// <example>
/// <code>
/// var list = new OpenManagedList<int>()
///     .OnAdd(i => Console.WriteLine($"Adding {i}"))
///     .OnRemove(i => Console.WriteLine($"Removing {i}"))
///     .OnClear(i => Console.WriteLine($"Clearing {i}"));
///
/// list.Add(42);        // Triggers OnAdd callbacks
/// list.Remove(42);     // Triggers OnRemove callbacks
/// list.Add(1); list.Add(2);
/// list.Clear();        // Triggers OnClear for 1 then 2
/// </code>
/// </example>
public abstract class ManagedList<T> : IList<T>
{
    /// <summary>Backing storage for list items.</summary>
    protected List<T> _internals = new();

    /// <summary>Registered Add callbacks.</summary>
    protected readonly OnAddList<T> _onAdd;
    /// <summary>Registered Remove callbacks.</summary>
    protected readonly OnRemoveList<T> _onRemove;
    /// <summary>Registered Clear callbacks.</summary>
    protected readonly onClearList<T> _onClear;

    /// <summary>Creates a new managed list with isolated callback collections.</summary>
    protected ManagedList()
    {
        _onAdd = new OnAddList<T>();
        _onRemove = new OnRemoveList<T>();
        _onClear = new onClearList<T>();
    }

    /// <summary>
    /// Creates a managed list reusing existing internals and callbacks (enables shared state across instances).
    /// </summary>
    protected ManagedList(List<T> internals, OnAddList<T> onAdd, OnRemoveList<T> onRemove, onClearList<T> onClear)
    {
        _internals = internals;
        _onAdd = onAdd;
        _onRemove = onRemove;
        _onClear = onClear;
    }

    /// <inheritdoc />
    public T this[int index] { get => _internals[index]; set => _internals[index] = value; }

    /// <inheritdoc />
    public int Count => _internals.Count;

    /// <inheritdoc />
    public bool IsReadOnly => false;

    /// <summary>
    /// Adds an item to the list invoking all registered Add callbacks <em>before</em> insertion.
    /// </summary>
    public void Add(T item)
    {
        foreach (var action in _onAdd)
        {
            action(item);
        }

        _internals.Add(item);
    }

    /// <summary>
    /// Clears the list invoking each registered Clear callback once for every existing item prior to removing them all.
    /// </summary>
    public void Clear()
    {
        foreach (var action in _onClear)
        {
            foreach (var item in _internals)
                action(item);
        }

        _internals.Clear();
    }

    /// <inheritdoc />
    public bool Contains(T item)
    {
        return _internals.Contains(item);
    }

    /// <inheritdoc />
    public void CopyTo(T[] array, int arrayIndex)
    {
        _internals.CopyTo(array, arrayIndex);
    }

    /// <inheritdoc />
    public IEnumerator<T> GetEnumerator()
    {
        return _internals.GetEnumerator();
    }

    /// <inheritdoc />
    public int IndexOf(T item)
    {
        return _internals.IndexOf(item);
    }

    /// <summary>
    /// Inserts an item at a specific index. Note: callbacks are invoked <em>after</em> the insertion (unlike <see cref="Add"/>).
    /// </summary>
    public void Insert(int index, T item)
    {
        _internals.Insert(index, item);

        foreach (var action in _onAdd)
        {
            action(item);
        }
    }

    /// <summary>
    /// Removes the first occurrence of an item, invoking Remove callbacks <em>before</em> the underlying removal.
    /// </summary>
    public bool Remove(T item)
    {
        foreach (var action in _onRemove)
        {
            action(item);
        }

        return _internals.Remove(item);
    }

    /// <summary>
    /// Removes the item at the specified index invoking Remove callbacks <em>before</em> the removal.
    /// </summary>
    public void RemoveAt(int index)
    {
        var item = _internals[index];

        foreach (var action in _onRemove)
        {
            action(item);
        }
        _internals.RemoveAt(index);
    }

    /// <summary>
    /// Returns an enumerator that iterates through the collection.
    /// </summary>
    /// <returns>An <see cref="IEnumerator"/> object that can be used to iterate through the collection.</returns>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
