using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace FrenchExDev.Net.CSharp.ManagedDictionary;

/// <summary>
/// Represents a list of actions to be invoked when an item is added, where each action receives a key and a value as
/// parameters.
/// </summary>
/// <remarks>This class can be used to register multiple callbacks that are executed when an item is added,
/// allowing custom logic to be performed for each key-value pair. Actions are invoked in the order they are added to
/// the list.</remarks>
/// <typeparam name="TKey">The type of the key parameter passed to each action.</typeparam>
/// <typeparam name="TValue">The type of the value parameter passed to each action.</typeparam>
public class OnAddList<TKey, TValue> : List<Action<TKey, TValue>> { }

/// <summary>
/// Represents a collection of actions to be invoked when an item is removed, with each action receiving the item's key
/// and value as parameters.
/// </summary>
/// <remarks>Use this list to register callbacks that should be executed when an item is removed from a
/// collection. Each action in the list is called with the key and value of the removed item, allowing custom logic such
/// as cleanup or notification.</remarks>
/// <typeparam name="TKey">The type of the key associated with the removed item passed to each action.</typeparam>
/// <typeparam name="TValue">The type of the value associated with the removed item passed to each action.</typeparam>
public class OnRemoveList<TKey, TValue> : List<Action<TKey, TValue>> { }

/// <summary>
/// Represents a collection of actions to be invoked when a clear operation is performed, with each action receiving no
/// </summary>
public class OnClearList : List<Action> { }

/// <summary>
/// Provides a builder for configuring and creating managed dictionary instances with customizable behavior for add,
/// remove, and clear operations.
/// </summary>
/// <remarks>Use this builder to specify initial key-value pairs and to register callbacks that are invoked when
/// items are added, removed, or when the dictionary is cleared. After configuration, call either Open or Close to
/// create a managed dictionary instance with the specified behaviors.</remarks>
/// <typeparam name="TKey">The type of keys in the dictionary. Must be non-nullable.</typeparam>
/// <typeparam name="TValue">The type of values in the dictionary.</typeparam>
public class ManagedDictionaryBuilder<TKey, TValue> where TKey : notnull
{
    private readonly Dictionary<TKey, TValue> _initial = new();
    private readonly OnAddList<TKey, TValue> _onAdd = new();
    private readonly OnRemoveList<TKey, TValue> _onRemove = new();
    private readonly OnClearList _onClear = new();

    /// <summary>
    /// Sets an initial key-value pair in the dictionary being built.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public void Set(TKey key, TValue value)
    {
        _initial[key] = value;
    }

    /// <summary>
    /// Replaces the current key-value pairs with those provided in the specified dictionary.
    /// </summary>
    /// <remarks>All existing entries with matching keys are updated to the values from <paramref
    /// name="basis"/>. Keys not present in <paramref name="basis"/> remain unchanged.</remarks>
    /// <param name="basis">A dictionary containing the key-value pairs to set. Each entry will overwrite the corresponding value for its
    /// key.</param>
    public void Set(IDictionary<TKey, TValue> basis)
    {
        foreach (var kv in basis)
        {
            _initial[kv.Key] = kv.Value;
        }
    }

    /// <summary>
    /// Registers a callback to be invoked whenever a new key-value pair is added to the dictionary being built.
    /// </summary>
    /// <remarks>Multiple callbacks can be registered by calling this method multiple times. Callbacks are
    /// invoked in the order they are added.</remarks>
    /// <param name="func">An action to execute when a key-value pair is added. The callback receives the key and value that were added.</param>
    /// <returns>The current instance of <see cref="ManagedDictionaryBuilder{TKey, TValue}"/> to allow method chaining.</returns>
    public ManagedDictionaryBuilder<TKey, TValue> OnAdd(Action<TKey, TValue> func)
    {
        _onAdd.Add(func);
        return this;
    }

    /// <summary>
    /// Registers a callback to be invoked when an item is removed from the dictionary.
    /// </summary>
    /// <remarks>Multiple callbacks can be registered; all will be invoked in the order they were added when
    /// an item is removed.</remarks>
    /// <param name="func">The action to execute when an item is removed. The callback receives the key and value of the removed item as
    /// parameters.</param>
    /// <returns>The current instance of <see cref="ManagedDictionaryBuilder{TKey, TValue}"/> to allow method chaining.</returns>
    public ManagedDictionaryBuilder<TKey, TValue> OnRemove(Action<TKey, TValue> func)
    {
        _onRemove.Add(func);
        return this;
    }

    /// <summary>
    /// Registers an action to be invoked whenever the dictionary is cleared.
    /// </summary>
    /// <remarks>Multiple actions can be registered; all will be invoked in the order they were added when the
    /// dictionary is cleared.</remarks>
    /// <param name="func">The action to execute when the dictionary is cleared. Cannot be null.</param>
    /// <returns>The current instance of <see cref="ManagedDictionaryBuilder{TKey, TValue}"/> to allow method chaining.</returns>
    public ManagedDictionaryBuilder<TKey, TValue> OnClear(Action func)
    {
        _onClear.Add(func);
        return this;
    }

    /// <summary>
    /// Opens and returns a managed dictionary instance that supports add, remove, and clear operations with custom
    /// handlers.
    /// </summary>
    /// <remarks>Use the returned dictionary to perform managed operations with the handlers specified during
    /// construction. Each call to this method returns a separate instance.</remarks>
    /// <returns>A new instance of <see cref="ManagedDictionary{TKey, TValue}"/> configured with the current add, remove, and
    /// clear handlers.</returns>
    public ManagedDictionary<TKey, TValue> Open()
    {
        var n = new OpenManagedDictionary<TKey, TValue>(_onAdd, _onRemove, _onClear, _initial);

        return n;
    }

    /// <summary>
    /// Closes the current managed dictionary and returns a read-only version that prevents further modifications.
    /// </summary>
    /// <remarks>Use this method to finalize the dictionary when no further changes are required. After
    /// calling <c>Close</c>, attempts to modify the returned dictionary will result in an exception.</remarks>
    /// <returns>A <see cref="ClosedManagedDictionary{TKey, TValue}"/> instance containing the current entries. The returned
    /// dictionary is read-only and cannot be modified.</returns>
    public ClosedManagedDictionary<TKey, TValue> Close()
    {
        var c = new ClosedManagedDictionary<TKey, TValue>(_onAdd, _onRemove, _onClear, _initial);

        return c;
    }
}

/// <summary>
/// A managed dictionary that allows dynamic registration of event handlers for add, remove, and clear operations.
/// </summary>
/// <typeparam name="TKey"></typeparam>
/// <typeparam name="TValue"></typeparam>
public class OpenManagedDictionary<TKey, TValue> : ManagedDictionary<TKey, TValue> where TKey : notnull
{
    /// <summary>
    /// Constructs an empty OpenManagedDictionary with default event handler lists.
    /// </summary>
    /// <param name="basis"></param>
    public OpenManagedDictionary(Dictionary<TKey, TValue> basis) : base(new OnAddList<TKey, TValue>(), new OnRemoveList<TKey, TValue>(), new OnClearList())
    {

    }

    /// <summary>
    /// Initializes a new instance of the OpenManagedDictionary class with the specified event handlers and initial
    /// dictionary contents.
    /// </summary>
    /// <remarks>Event handlers provided for add, remove, and clear operations will be triggered on the
    /// corresponding dictionary actions. This allows custom logic to be executed in response to changes in the
    /// dictionary's contents.</remarks>
    /// <param name="onAdd">The delegate or list of actions to invoke when a new key-value pair is added to the dictionary. Cannot be null.</param>
    /// <param name="onRemove">The delegate or list of actions to invoke when a key-value pair is removed from the dictionary. Cannot be null.</param>
    /// <param name="onClear">The delegate or list of actions to invoke when the dictionary is cleared. Cannot be null.</param>
    /// <param name="initial">The initial set of key-value pairs to populate the dictionary. If null, the dictionary is initialized empty.</param>
    public OpenManagedDictionary(OnAddList<TKey, TValue> onAdd, OnRemoveList<TKey, TValue> onRemove, OnClearList onClear, Dictionary<TKey, TValue> initial) : base(onAdd, onRemove, onClear, initial)
    {
    }

    /// <summary>
    /// Registers a callback to be invoked whenever a new key-value pair is added to the dictionary.
    /// </summary>
    /// <remarks>If multiple callbacks are registered, all will be invoked in the order they were added. The
    /// callback is triggered only for additions, not for updates to existing entries.</remarks>
    /// <param name="func">The action to execute when a new entry is added. The callback receives the key and value of the added item as
    /// parameters. Cannot be null.</param>
    /// <returns>The current instance of <see cref="OpenManagedDictionary{TKey, TValue}"/> to allow method chaining.</returns>
    public OpenManagedDictionary<TKey, TValue> OnAdd(Action<TKey, TValue> func)
    {
        InternalOnAdd(func);
        return this;
    }

    /// <summary>
    /// Registers a callback to be invoked when the dictionary is cleared.
    /// </summary>
    /// <remarks>If multiple callbacks are registered, all will be invoked in the order they were added. The
    /// callback is not invoked if the dictionary is already empty when cleared.</remarks>
    /// <param name="func">An action to execute when the dictionary is cleared. The callback is invoked after the clear operation
    /// completes.</param>
    /// <returns>The current instance of <see cref="OpenManagedDictionary{TKey, TValue}"/> to allow method chaining.</returns>
    public OpenManagedDictionary<TKey, TValue> OnClear(Action func)
    {
        InternalOnClear(func);
        return this;
    }

    /// <summary>
    /// Registers a callback to be invoked whenever an item is removed from the dictionary.
    /// </summary>
    /// <remarks>If multiple callbacks are registered, all will be invoked in the order they were added. The
    /// callback is triggered for both explicit removals and items removed due to internal operations, such as
    /// eviction.</remarks>
    /// <param name="func">The action to execute when an item is removed. The callback receives the key and value of the removed item as
    /// parameters. Cannot be null.</param>
    /// <returns>The current instance of <see cref="OpenManagedDictionary{TKey, TValue}"/> to allow method chaining.</returns>
    public OpenManagedDictionary<TKey, TValue> OnRemove(Action<TKey, TValue> func)
    {
        InternalOnRemove(func);
        return this;
    }
}

/// <summary>
/// Represents a managed dictionary with a fixed set of keys, supporting custom actions on add, remove, and clear
/// operations.
/// </summary>
/// <remarks>This dictionary does not allow modification of its key set after construction. Custom delegates can
/// be provided to handle add, remove, and clear events. Use this type when you require a managed dictionary with
/// controlled key membership and event-driven behavior.</remarks>
/// <typeparam name="TKey">The type of keys in the dictionary. Must be non-nullable.</typeparam>
/// <typeparam name="TValue">The type of values associated with the keys in the dictionary.</typeparam>
public class ClosedManagedDictionary<TKey, TValue> : ManagedDictionary<TKey, TValue> where TKey : notnull
{
    /// <summary>
    /// Initializes a new instance of the ClosedManagedDictionary class with the specified event handlers and underlying
    /// dictionary.
    /// </summary>
    /// <param name="onAdd">The delegate or list of delegates to invoke when an item is added to the dictionary. Cannot be null.</param>
    /// <param name="onRemove">The delegate or list of delegates to invoke when an item is removed from the dictionary. Cannot be null.</param>
    /// <param name="onClear">The delegate or list of delegates to invoke when the dictionary is cleared. Cannot be null.</param>
    /// <param name="kv">The underlying dictionary that stores the key-value pairs. Cannot be null.</param>
    public ClosedManagedDictionary(OnAddList<TKey, TValue> onAdd, OnRemoveList<TKey, TValue> onRemove, OnClearList onClear, Dictionary<TKey, TValue> kv) : base(onAdd, onRemove, onClear, kv)
    {
    }
}

/// <summary>
/// Provides an abstract base class for a dictionary that supports managed add, remove, and clear operations with
/// customizable event handling.
/// </summary>
/// <remarks>ManagedDictionary enables derived classes to attach custom logic to add, remove, and clear operations
/// by registering event handlers. This allows for scenarios such as auditing, validation, or asynchronous processing
/// when dictionary contents change. The class implements IDictionary<TKey, TValue> and is not read-only. Thread safety
/// is not guaranteed; external synchronization may be required for concurrent access.</remarks>
/// <typeparam name="TKey">The type of keys in the dictionary. Must be non-nullable.</typeparam>
/// <typeparam name="TValue">The type of values in the dictionary.</typeparam>
public abstract class ManagedDictionary<TKey, TValue> : IDictionary<TKey, TValue> where TKey : notnull
{
    private readonly Dictionary<TKey, TValue> _initial = new();
    private readonly OnAddList<TKey, TValue> _onAdd;
    private readonly OnRemoveList<TKey, TValue> _onRemove;
    private readonly OnClearList _onClear;

    /// <summary>
    /// Gets a collection containing the keys in the dictionary.
    /// </summary>
    public ICollection<TKey> Keys => _initial.Keys;

    /// <summary>
    /// Gets a collection containing the values in the dictionary.
    /// </summary>
    public ICollection<TValue> Values => _initial.Values;

    /// <summary>
    /// Gets the number of elements contained in the collection.
    /// </summary>
    public int Count => _initial.Count;

    /// <summary>
    /// Gets a value indicating whether the dictionary is read-only. This implementation always returns false,
    /// </summary>
    public bool IsReadOnly => false;

    /// <summary>
    /// Gets or sets the value associated with the specified key.
    /// </summary>
    /// <param name="key">The key whose value to get or set.</param>
    /// <returns></returns>
    public TValue this[TKey key] { get => _initial[key]; set => _initial[key] = value; }

    /// <summary>
    /// Initializes a new instance of the ManagedDictionary class with specified callbacks for add, remove, and clear
    /// operations, and an optional initial dictionary state.
    /// </summary>
    /// <remarks>The provided delegates enable external code to respond to changes in the dictionary's
    /// contents. This can be useful for event-driven scenarios or for maintaining synchronization with other data
    /// structures.</remarks>
    /// <param name="onAdd">A delegate or list of delegates invoked when an item is added to the dictionary. Can be used to perform custom
    /// actions upon addition.</param>
    /// <param name="onRemove">A delegate or list of delegates invoked when an item is removed from the dictionary. Allows custom logic to be
    /// executed on removal.</param>
    /// <param name="onClear">A delegate or list of delegates invoked when the dictionary is cleared. Enables custom handling of the clear
    /// operation.</param>
    /// <param name="initial">The initial dictionary state to populate the ManagedDictionary. If null, the dictionary starts empty.</param>
    protected ManagedDictionary(OnAddList<TKey, TValue> onAdd, OnRemoveList<TKey, TValue> onRemove, OnClearList onClear, Dictionary<TKey, TValue> initial)
    {
        _onAdd = onAdd;
        _onRemove = onRemove;
        _onClear = onClear;
    }

    /// <summary>
    /// Initializes a new instance of the ManagedDictionary class with custom handlers for add, remove, and clear
    /// operations.
    /// </summary>
    /// <remarks>Use this constructor to inject custom behavior for add, remove, and clear actions. All
    /// delegates must be non-null to ensure correct operation.</remarks>
    /// <param name="onAdd">The delegate invoked when an item is added to the dictionary. Allows custom logic to be executed during add
    /// operations.</param>
    /// <param name="onRemove">The delegate invoked when an item is removed from the dictionary. Enables custom behavior during remove
    /// operations.</param>
    /// <param name="onClear">The delegate invoked when the dictionary is cleared. Provides a mechanism for custom logic during clear
    /// operations.</param>
    protected ManagedDictionary(OnAddList<TKey, TValue> onAdd, OnRemoveList<TKey, TValue> onRemove, OnClearList onClear) : base()
    {
        _onAdd = onAdd;
        _onRemove = onRemove;
        _onClear = onClear;
    }

    /// <summary>
    /// Registers a callback to be invoked when an item is added to the collection.
    /// </summary>
    /// <param name="func">The action to execute when an item is added. The callback receives the key and value of the added item as
    /// parameters. Cannot be null.</param>
    protected void InternalOnAdd(Action<TKey, TValue> func)
    {
        _onAdd.Add(func);
    }

    /// <summary>
    /// Registers an action to be invoked when the clear operation is performed.
    /// </summary>
    /// <param name="func">The action to execute when the clear event occurs. Cannot be null.</param>
    protected void InternalOnClear(Action func)
    {
        _onClear.Add(func);
    }

    /// <summary>
    /// Registers a callback to be invoked when an item is removed from the collection.
    /// </summary>
    /// <remarks>Multiple callbacks can be registered; all will be invoked when an item is removed. Callbacks
    /// are invoked in the order they are added.</remarks>
    /// <param name="func">The action to execute when an item is removed. The callback receives the key and value of the removed item as
    /// parameters.</param>
    protected void InternalOnRemove(Action<TKey, TValue> func)
    {
        _onRemove.Add(func);
    }

    /// <summary>
    /// Adds a key/value pair to the collection and notifies registered handlers of the addition.
    /// </summary>
    /// <remarks>If a handler is registered, it will be invoked for each addition. Adding a duplicate key may
    /// result in an exception, depending on the underlying collection's behavior.</remarks>
    /// <param name="key">The key associated with the value to add. Cannot be null.</param>
    /// <param name="value">The value to associate with the specified key.</param>
    public void Add(TKey key, TValue value)
    {
        _initial.Add(key, value);
        foreach (var handler in _onAdd)
        {
            handler(key, value);
        }
    }

    /// <summary>
    /// Determines whether the dictionary contains an element with the specified key.
    /// </summary>
    /// <param name="key">The key to locate in the dictionary. Cannot be null.</param>
    /// <returns>true if the dictionary contains an element with the specified key; otherwise, false.</returns>
    public bool ContainsKey(TKey key)
    {
        return _initial.ContainsKey(key);
    }

    /// <summary>
    /// Removes the element with the specified key from the collection.
    /// </summary>
    /// <remarks>Any registered removal handlers are invoked before the element is removed. If the key does
    /// not exist, no handlers are called and the method returns false.</remarks>
    /// <param name="key">The key of the element to remove.</param>
    /// <returns>true if the element is successfully removed; otherwise, false. This method also returns false if the key was not
    /// found in the collection.</returns>
    public bool Remove(TKey key)
    {
        if (!_initial.ContainsKey(key))
        {
            return false;
        }

        var item = _initial[key];

        foreach (var handler in _onRemove)
        {
            handler(key, item);
        }

        return _initial.Remove(key);
    }

    /// <summary>
    /// Attempts to retrieve the value associated with the specified key.
    /// </summary>
    /// <param name="key">The key whose value to retrieve.</param>
    /// <param name="value">When this method returns, contains the value associated with the specified key, if the key is found; otherwise,
    /// the default value for the type of the value parameter. This parameter is passed uninitialized.</param>
    /// <returns>true if the key was found; otherwise, false.</returns>
    public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
    {
        return _initial.TryGetValue(key, out value);
    }

    /// <summary>
    /// Adds the specified key/value pair to the collection.
    /// </summary>
    /// <remarks>After the item is added, any registered add handlers are invoked with the key and value. If
    /// the key already exists, an exception may be thrown depending on the underlying collection's behavior.</remarks>
    /// <param name="item">The key/value pair to add to the collection. The key must not already exist in the collection.</param>
    public void Add(KeyValuePair<TKey, TValue> item)
    {
        _initial.Add(item.Key, item.Value);
        foreach (var handler in _onAdd)
        {
            handler(item.Key, item.Value);
        }
    }

    /// <summary>
    /// Clears all items from the collection and invokes any registered clear handlers.
    /// </summary>
    public void Clear()
    {
        foreach (var handler in _onClear)
        {
            handler();
        }
        _initial.Clear();
    }

    /// <summary>
    /// Determines whether the collection contains a specific key and value pair.
    /// </summary>
    /// <param name="item">The key and value pair to locate in the collection. Both the key and value are compared using the collection's
    /// equality comparer.</param>
    /// <returns>true if the specified key and value pair is found in the collection; otherwise, false.</returns>
    public bool Contains(KeyValuePair<TKey, TValue> item)
    {
        return _initial.Contains(item);
    }

    /// <summary>
    /// Copies the elements of the collection to the specified array, starting at the given array index.
    /// </summary>
    /// <param name="array">The one-dimensional array of <see cref="KeyValuePair{TKey, TValue}"/> elements that is the destination of the
    /// elements copied from the collection. The array must have zero-based indexing.</param>
    /// <param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.</param>
    /// <exception cref="NotImplementedException">Thrown in all cases, as this method is not implemented.</exception>
    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Removes the specified key and value pair from the collection, if it exists.
    /// </summary>
    /// <remarks>If the specified key exists, any registered removal handlers are invoked before the item is
    /// removed. Removal is based on the key; the value is not validated before removal.</remarks>
    /// <param name="item">The key and value pair to remove from the collection. The key is used to identify the entry to remove.</param>
    /// <returns>true if the key and value pair was found and removed; otherwise, false.</returns>
    public bool Remove(KeyValuePair<TKey, TValue> item)
    {
        if (!_initial.ContainsKey(item.Key))
        {
            return false;
        }

        foreach (var handler in _onRemove)
        {
            handler(item.Key, item.Value);
        }

        return _initial.Remove(item.Key);
    }

    /// <summary>
    /// Returns an enumerator that iterates through the collection of key/value pairs.
    /// </summary>
    /// <returns>An enumerator for the collection of <see cref="KeyValuePair{TKey, TValue}"/> elements.</returns>
    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        return _initial.GetEnumerator();
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
