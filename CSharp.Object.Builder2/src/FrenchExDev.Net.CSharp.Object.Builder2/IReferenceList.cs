namespace FrenchExDev.Net.CSharp.Object.Builder2;

/// <summary>
/// Mutable interface for a list of object references, extending <see cref="IList{T}"/> functionality.
/// </summary>
/// <typeparam name="TClass">The type of objects being referenced.</typeparam>
/// <remarks>
/// <para>
/// <see cref="IReferenceList{TClass}"/> combines <see cref="IList{T}"/> with <see cref="IReadOnlyReferenceList{TClass}"/>
/// to provide full mutable list functionality while adding reference-specific operations.
/// </para>
/// </remarks>
/// <seealso cref="IReadOnlyReferenceList{TClass}"/>
/// <seealso cref="ReferenceList{TClass}"/>
public interface IReferenceList<TClass> : IList<TClass>, IReadOnlyReferenceList<TClass> where TClass : class
{
    /// <summary>
    /// Gets an <see cref="IQueryable{T}"/> interface for LINQ query provider operations.
    /// </summary>
    IQueryable<TClass> Queryable { get; }

    /// <summary>
    /// Returns the resolved instance at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index of the element to retrieve.</param>
    /// <returns>The resolved <typeparamref name="TClass"/> instance at the specified index.</returns>
    TClass ElementAt(int index);

    /// <summary>
    /// Adds a reference to the list.
    /// </summary>
    /// <param name="reference">The reference to add.</param>
    void Add(Reference<TClass> reference);

    /// <summary>
    /// Determines whether the list contains a specific reference.
    /// </summary>
    /// <param name="reference">The reference to locate.</param>
    /// <returns><see langword="true"/> if the reference is found; otherwise, <see langword="false"/>.</returns>
    new bool Contains(Reference<TClass> reference);
}
