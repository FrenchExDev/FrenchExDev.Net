namespace FrenchExDev.Net.CSharp.Object.Builder.Abstractions;

/// <summary>
/// Represents the result of a failed attempt to build an object, including the builder used, the exceptions
/// encountered, and the set of visited objects.
/// </summary>
/// <remarks>This class provides detailed information about build failures, including all exceptions that occurred
/// and the objects that were visited during the build process. It is typically used for diagnostics or error reporting
/// in object construction workflows.</remarks>
/// <typeparam name="TClass">The type of object that was being built.</typeparam>
/// <typeparam name="TBuilder">The type of builder used to construct the object. Must implement <see cref="IObjectBuilder{TClass}"/>.</typeparam>
public class FailureObjectBuildResult<TClass, TBuilder> : AbstractFailureObjectBuildResult<TBuilder>, IObjectBuildResult<TClass> where TBuilder : IObjectBuilder<TClass>
{
    /// <summary>
    /// Initializes a new instance of the FailureObjectBuildResult class with the specified builder, exception list, and
    /// visited objects.
    /// </summary>
    /// <param name="builder">The builder instance used to construct the object. Cannot be null.</param>
    /// <param name="exceptions">The collection of exceptions encountered during the build process. Cannot be null.</param>
    /// <param name="visited">The list of objects that were visited during the build operation. Cannot be null.</param>
    public FailureObjectBuildResult(TBuilder builder, ExceptionBuildList exceptions, VisitedObjectsList visited) : base(builder, exceptions, visited)
    {
    }
}
