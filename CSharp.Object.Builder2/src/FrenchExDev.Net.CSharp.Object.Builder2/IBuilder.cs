namespace FrenchExDev.Net.CSharp.Object.Builder2;

/// <summary>
/// Represents a builder that can construct instances of type <typeparamref name="TClass"/>.
/// </summary>
/// <typeparam name="TClass"></typeparam>
public interface IBuilder<TClass> : IBuildable<TClass>, IValidatable, IReferenceable<TClass>, IIdentifiable
    where TClass : class
{
    /// <summary>
    /// Gets the result value produced by the operation, or <see langword="null"/> if no result is available.
    /// </summary>
    TClass? Result { get; }
}
