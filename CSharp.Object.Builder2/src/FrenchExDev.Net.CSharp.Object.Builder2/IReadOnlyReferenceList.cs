namespace FrenchExDev.Net.CSharp.Object.Builder2;

/// <summary>
/// Read-only interface for a list of object references.
/// </summary>
/// <typeparam name="TClass">The type of objects being referenced.</typeparam>
/// <remarks>
/// <para>
/// <see cref="IReadOnlyReferenceList{TClass}"/> extends <see cref="IReadOnlyList{T}"/> to provide
/// read-only access to a collection of referenced objects, along with methods for checking
/// whether specific instances or references are contained in the list.
/// </para>
/// </remarks>
/// <seealso cref="IReferenceList{TClass}"/>
/// <seealso cref="ReferenceList{TClass}"/>
public interface IReadOnlyReferenceList<TClass> : IReadOnlyList<TClass> where TClass : class
{
    /// <summary>
    /// Returns an enumerable that yields only the resolved instances from this list.
    /// </summary>
    /// <returns>An enumerable of resolved <typeparamref name="TClass"/> instances.</returns>
    IEnumerable<TClass> AsEnumerable();

    /// <summary>
    /// Determines whether the list contains a specific instance.
    /// </summary>
    /// <param name="instance">The instance to locate.</param>
    /// <returns><see langword="true"/> if the instance is found; otherwise, <see langword="false"/>.</returns>
    bool Contains(TClass instance);

    /// <summary>
    /// Determines whether the list contains a specific reference.
    /// </summary>
    /// <param name="reference">The reference to locate.</param>
    /// <returns><see langword="true"/> if the reference is found; otherwise, <see langword="false"/>.</returns>
    bool Contains(Reference<TClass> reference);
}
