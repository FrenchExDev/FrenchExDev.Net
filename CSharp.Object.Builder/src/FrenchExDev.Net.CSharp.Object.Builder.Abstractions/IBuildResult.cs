namespace FrenchExDev.Net.CSharp.Object.Builder.Abstractions;

/// <summary>
/// Represents the result of a build operation for an object of type <typeparamref name="TClass"/>.
/// </summary>
/// <typeparam name="TClass"></typeparam>
public interface IBuildResult<TClass>
{

    /// <summary>
    /// Gets a value indicating whether the object has been successfully built.
    /// </summary>
    public bool IsBuilt { get; }

    /// <summary>
    /// Holds the built object if the build was successful; otherwise, it is null.
    /// </summary>
    TClass? Result { get; }

    /// <summary>
    /// Gets the exception that occurred during the operation, if any.
    /// </summary>
    Exception? Exception { get; }
}
