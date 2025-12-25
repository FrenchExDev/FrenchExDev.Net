namespace FrenchExDev.Net.CSharp.Object.Builder2;

/// <summary>
/// A collection of <see cref="Reference{TClass}"/> objects that provides list-like access to resolved instances.
/// </summary>
/// <typeparam name="TClass">The type of objects being referenced. Must be a reference type.</typeparam>
/// <remarks>
/// <para>
/// <see cref="ReferenceList{TClass}"/> wraps a list of <see cref="Reference{TClass}"/> objects and provides
/// convenient methods for working with both references and resolved instances.
/// </para>
/// <para>
/// The list implements <see cref="IList{T}"/> for <typeparamref name="TClass"/>, allowing it to be used
/// wherever a list of instances is expected. However, only resolved references are enumerated.
/// </para>
/// <para>
/// Use <see cref="AsEnumerable"/> with standard LINQ methods for querying resolved instances.
/// </para>
/// </remarks>
/// <example>
/// <code>
/// // Create a reference list from builders
/// var refList = builders.AsReferenceList();
/// 
/// // Add instances directly
/// refList.Add(new Person { Name = "Alice" });
/// 
/// // Add references
/// refList.Add(builder.Reference());
/// 
/// // Query resolved instances
/// var names = refList.Where(p => p.Age > 18).Select(p => p.Name);
/// </code>
/// </example>
/// <seealso cref="Reference{TClass}"/>
/// <seealso cref="BuilderList{TClass, TBuilder}"/>
/// <seealso cref="IReferenceList{TClass}"/>
public class ReferenceList<TClass> : IReferenceList<TClass> where TClass : class
{
    private readonly List<Reference<TClass>> _references;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReferenceList{TClass}"/> class from existing references.
    /// </summary>
    /// <param name="references">The collection of references to initialize the list with.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="references"/> is <see langword="null"/>.</exception>
    public ReferenceList(IEnumerable<Reference<TClass>> references) 
    { 
        _references = references.ToList() ?? throw new ArgumentNullException(nameof(references)); 
    }

    /// <summary>
    /// Initializes a new empty instance of the <see cref="ReferenceList{TClass}"/> class.
    /// </summary>
    public ReferenceList() { _references = []; }

    /// <summary>
    /// Returns an enumerable that yields only the resolved instances from this list.
    /// </summary>
    /// <returns>An enumerable containing only instances from resolved references.</returns>
    /// <remarks>
    /// Unresolved references are silently skipped. This is useful in scenarios where some
    /// references may not yet be resolved due to circular dependencies or deferred building.
    /// </remarks>
    /// <example>
    /// <code>
    /// // Only iterate over resolved instances
    /// foreach (var person in refList.AsEnumerable())
    /// {
    ///     Console.WriteLine(person.Name);
    /// }
    /// 
    /// // Use with LINQ
    /// var adults = refList.AsEnumerable().Where(p => p.Age >= 18).ToList();
    /// </code>
    /// </example>
    public IEnumerable<TClass> AsEnumerable()
    {
        foreach (var r in _references) 
            if (r.IsResolved && r.Instance is not null) 
                yield return r.Instance;
    }

    /// <summary>
    /// Adds a reference to the list.
    /// </summary>
    /// <param name="reference">The reference to add.</param>
    public void Add(Reference<TClass> reference) => _references.Add(reference);

    /// <summary>
    /// Adds an instance to the list by wrapping it in a resolved reference.
    /// </summary>
    /// <param name="instance">The instance to add.</param>
    public void Add(TClass instance) => _references.Add(new Reference<TClass>().Resolve(instance));

    /// <summary>
    /// Determines whether the list contains a specific reference.
    /// </summary>
    /// <param name="reference">The reference to locate.</param>
    /// <returns><see langword="true"/> if the reference is found; otherwise, <see langword="false"/>.</returns>
    public bool Contains(Reference<TClass> reference) => _references.Contains(reference);

    /// <summary>
    /// Determines whether the list contains a specific instance (by reference equality).
    /// </summary>
    /// <param name="instance">The instance to locate.</param>
    /// <returns><see langword="true"/> if a resolved reference to the instance is found; otherwise, <see langword="false"/>.</returns>
    public bool Contains(TClass instance)
    {
        foreach (var r in _references) 
            if (r.IsResolved && r.Instance == instance) 
                return true;
        return false;
    }

    /// <summary>
    /// Gets an <see cref="IQueryable{T}"/> interface for LINQ query provider operations.
    /// </summary>
    public IQueryable<TClass> Queryable => AsEnumerable().AsQueryable();

    /// <summary>
    /// Returns the resolved instance at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get.</param>
    /// <returns>The resolved instance at the specified index.</returns>
    /// <exception cref="ReferenceNotResolvedException">Thrown if the reference at the index is not resolved.</exception>
    public TClass ElementAt(int index) => _references[index].Resolved();

    /// <summary>
    /// Searches for the specified instance and returns the zero-based index of its first occurrence.
    /// </summary>
    /// <param name="item">The instance to locate.</param>
    /// <returns>The zero-based index of the first occurrence if found.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the instance is not found.</exception>
    public int IndexOf(TClass item)
    {
        for (int i = 0; i < _references.Count; i++) 
        { 
            var r = _references[i]; 
            if (r.IsResolved && r.Instance == item) 
                return i; 
        }
        throw new InvalidOperationException("Item not found");
    }

    /// <summary>
    /// Inserts an instance at the specified index by wrapping it in a resolved reference.
    /// </summary>
    /// <param name="index">The zero-based index at which the item should be inserted.</param>
    /// <param name="item">The instance to insert.</param>
    public void Insert(int index, TClass item) => _references.Insert(index, new Reference<TClass>().Resolve(item));

    /// <summary>
    /// Removes the reference at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index of the reference to remove.</param>
    public void RemoveAt(int index) => _references.RemoveAt(index);

    /// <summary>
    /// Removes all references from the list.
    /// </summary>
    public void Clear() => _references.Clear();

    /// <summary>
    /// Copies resolved instances to an array, starting at a particular array index.
    /// </summary>
    /// <param name="array">The destination array.</param>
    /// <param name="arrayIndex">The zero-based index in the array at which copying begins.</param>
    public void CopyTo(TClass[] array, int arrayIndex)
    {
        var i = arrayIndex;
        foreach (var r in _references) 
            if (r.IsResolved && r.Instance is not null) 
                array[i++] = r.Instance;
    }

    /// <summary>
    /// Removes the first occurrence of a specific instance from the list.
    /// </summary>
    /// <param name="item">The instance to remove.</param>
    /// <returns><see langword="true"/> if the item was successfully removed; otherwise, <see langword="false"/>.</returns>
    public bool Remove(TClass item)
    {
        for (int i = 0; i < _references.Count; i++)
        {
            var r = _references[i];
            if (r.IsResolved && r.Instance == item) 
            { 
                _references.RemoveAt(i); 
                return true; 
            }
        }
        return false;
    }

    /// <summary>
    /// Returns an enumerator that iterates through resolved instances.
    /// </summary>
    /// <returns>An enumerator for resolved instances.</returns>
    public IEnumerator<TClass> GetEnumerator() => AsEnumerable().GetEnumerator();

    /// <inheritdoc/>
    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>
    /// Gets the number of references in the list.
    /// </summary>
    /// <remarks>
    /// This returns the total count of references, including unresolved ones.
    /// To get the count of resolved instances, use <c>AsEnumerable().Count()</c>.
    /// </remarks>
    public int Count => _references.Count;

    /// <summary>
    /// Gets a value indicating whether the list is read-only. Always returns <see langword="false"/>.
    /// </summary>
    public bool IsReadOnly => false;

    /// <summary>
    /// Gets or sets the resolved instance at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get or set.</param>
    /// <returns>The resolved instance at the specified index.</returns>
    /// <exception cref="ReferenceNotResolvedException">Thrown when getting if the reference is not resolved.</exception>
    public TClass this[int index]
    {
        get => _references[index].Resolved();
        set => _references[index] = new Reference<TClass>().Resolve(value);
    }
}
