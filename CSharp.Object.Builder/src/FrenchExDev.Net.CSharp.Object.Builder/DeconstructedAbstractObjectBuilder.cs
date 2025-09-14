using FrenchExDev.Net.CSharp.Object.Builder.Abstractions;

namespace FrenchExDev.Net.CSharp.Object.Builder;

/// <summary>
/// Provides an abstract base for object builders that construct instances of a class by validating input and
/// instantiating the object in a deconstructed manner.
/// </summary>
/// <remarks>This class defines a template for building objects where validation and instantiation are separated
/// into distinct steps. Derived classes must implement the validation and instantiation logic by overriding the
/// corresponding abstract methods. The build process collects any validation exceptions and returns a failure result if
/// validation fails, or a success result if instantiation succeeds.</remarks>
/// <typeparam name="TClass">The type of object to be constructed by the builder.</typeparam>
/// <typeparam name="TBuilder">The type of the builder implementing the construction logic for the object.</typeparam>
public abstract class DeconstructedAbstractObjectBuilder<TClass, TBuilder> : AbstractObjectBuilder<TClass, TBuilder> where TBuilder : class, IObjectBuilder<TClass>
{
    /// <summary>
    /// Attempts to construct an instance of the target class, returning a result that indicates success or failure
    /// based on the provided exception and visitation state.
    /// </summary>
    /// <param name="exceptions">A dictionary containing any exceptions encountered during the build process. If this collection contains
    /// entries, the build will be considered unsuccessful.</param>
    /// <param name="visited">A list of objects that have already been visited during the build process. Used to prevent circular references
    /// and redundant processing.</param>
    /// <returns>An object representing the result of the build operation. If exceptions are present, the result indicates
    /// failure; otherwise, it contains the successfully constructed instance.</returns>
    protected override IObjectBuildResult<TClass> BuildInternal(ExceptionBuildDictionary exceptions, VisitedObjectsList visited)
    {
        Validate(exceptions, visited);

        if (exceptions.Count > 0)
        {
            return Failure(exceptions, visited);
        }

        return Success(Instantiate());
    }

    /// <summary>
    /// Creates a new instance of the type specified by the generic parameter.
    /// </summary>
    /// <remarks>Derived classes must implement this method to provide the logic for instantiating objects of
    /// type <typeparamref name="TClass"/>.</remarks>
    /// <returns>A new instance of <typeparamref name="TClass"/>.</returns>
    protected abstract TClass Instantiate();

    /// <summary>
    /// Performs validation on the current object and adds any validation errors to the specified exception dictionary.
    /// </summary>
    /// <param name="exceptions">A dictionary used to collect validation exceptions encountered during the validation process. Cannot be null.</param>
    /// <param name="visited">A list of objects that have already been visited during validation to prevent redundant checks or circular
    /// references. Cannot be null.</param>
    protected abstract void Validate(ExceptionBuildDictionary exceptions, VisitedObjectsList visited);
}
