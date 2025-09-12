namespace FrenchExDev.Net.CSharp.Object.Builder.Abstractions;

/// <summary>
/// Represents a reference to an asynchronously built object, providing access to the build result and related
/// operations.
/// </summary>
/// <remarks>This class is typically used in scenarios where object construction is performed asynchronously and a
/// reference to the build result is required. It combines build result access with asynchronous builder
/// integration.</remarks>
/// <typeparam name="TClass">The type of object being built and referenced.</typeparam>
/// <typeparam name="TBuilder">The type of asynchronous builder used to construct the object.</typeparam>
public class AsyncBuildReference<TClass, TBuilder> : AbstractBuildReference<TClass>, IObjectBuildResult<TClass> where TBuilder : IAsyncObjectBuilder<TClass>
{
    /// <summary>
    /// Initializes a new instance of the AsyncBuildReference class with the specified reference object.
    /// </summary>
    /// <param name="reference">The reference object to associate with this instance. Can be null if no reference is available.</param>
    public AsyncBuildReference(TClass? reference) : base(reference)
    {
    }
}
