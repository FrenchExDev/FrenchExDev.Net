namespace FrenchExDev.Net.CSharp.Object.Builder2;

/// <summary>
/// Defines a contract for objects that can provide a reference to themselves.
/// </summary>
public interface IReferenceable<TClass> where TClass : class
{
    /// <summary>
    /// Creates a reference to the current instance of type <typeparamref name="TClass"/>.
    /// </summary>
    /// <returns>A <see cref="Reference{TClass}"/> representing a reference to the current instance.</returns>
    Reference<TClass> Reference();
}
