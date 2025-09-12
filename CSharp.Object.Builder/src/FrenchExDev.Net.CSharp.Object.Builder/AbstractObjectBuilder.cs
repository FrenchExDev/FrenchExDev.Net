using FrenchExDev.Net.CSharp.Object.Builder.Abstractions;

namespace FrenchExDev.Net.CSharp.Object.Builder;

/// <summary>
/// Provides a base class for building objects of type specified by <typeparamref name="TClass"/> using a builder
/// pattern, supporting cyclic dependency detection and customizable build result handling.
/// </summary>
/// <remarks>This abstract class defines common functionality for object builders, including support for tracking
/// visited objects to prevent cycles, handling build failures, and aggregating exceptions. Derived classes should
/// implement the build logic by overriding the relevant methods. The class is intended for use in scenarios where
/// object graphs may contain cycles or shared references, and provides utility methods to facilitate robust object
/// construction workflows.</remarks>
/// <typeparam name="TClass">The type of object to be constructed by the builder.</typeparam>
/// <typeparam name="TBuilder">The type of builder implementing <see cref="IObjectBuilder{TClass}"/>. Must be compatible with <typeparamref
/// name="TClass"/>.</typeparam>
public abstract class AbstractObjectBuilder<TClass, TBuilder> : IObjectBuilder<TClass> where TBuilder : class, IObjectBuilder<TClass>
{
    /// <summary>
    /// Builds an instance of <typeparamref name="TClass"/> using the current builder configuration, optionally tracking
    /// visited objects to prevent cyclic dependencies.
    /// </summary>
    /// <remarks>This method is intended for use in scenarios where object graphs may contain cycles or shared
    /// references. By supplying a <paramref name="visited"/> list, repeated builds of the same object are avoided,
    /// ensuring correct handling of cyclic dependencies.</remarks>
    /// <param name="visited">An optional list of previously visited objects used to detect and prevent cycles during the build process. If
    /// not provided, a new list is created internally.</param>
    /// <returns>An <see cref="IObjectBuildResult{TClass}"/> representing the result of the build operation. If the object was
    /// already built and tracked in <paramref name="visited"/>, returns the existing instance; otherwise, returns the
    /// newly built instance.</returns>
    public IObjectBuildResult<TClass> Build(VisitedObjectsList? visited = null)
    {
        if (_reference is not null)
        {
            return new SuccessObjectBuildResult<TClass>(_reference);
        }

        if (_referencedBuilder is not null && visited is not null)
        {
            var visitedBuildRef = visited[_referencedBuilder];
            if (visitedBuildRef is BuildReference<TClass, TBuilder> castBuildRef)
            {
                return castBuildRef;
            }
            throw new InvalidOperationException("Referenced builder was not found in visited list.");
        }

        // Initialize exceptions build list to collect exceptions during the build process.
        var exceptions = new ExceptionBuildList();

        // Initialize visited objects list if not provided.
        visited ??= new VisitedObjectsList();

        // Check if the current builder instance has already been visited to prevent cyclic dependencies.
        if (visited.TryGetValue(this, out var existing))
        {
            switch (existing)
            {
                case BuildReference<TClass, TBuilder> existingBuildRef:
                    if (existingBuildRef.HasReference)
                    {
                        ArgumentNullException.ThrowIfNull(existingBuildRef.Reference);
                        return new SuccessObjectBuildResult<TClass>(existingBuildRef.Reference);
                    }

                    existingBuildRef.AddAction(result =>
                    {
                        ArgumentNullException.ThrowIfNull(result);
                        _reference = result;
                    });
                    break;
                case SuccessObjectBuildResult<TClass> existingSuccessObject:
                    return existingSuccessObject;
                default:
                    throw new NotSupportedException(existing.GetType().FullName);
            }
        }

        // Mark the current builder instance as visited with a temporary default value.
        var visitMe = new BuildReference<TClass, TBuilder>(default);

        // Add to the list of objects currently being built to detect self-references.
        visited[this] = visitMe;

        // Perform the actual build operation using the internal method.
        var built = BuildInternal(exceptions, visited);

        // Assign the built result to the visited objects list.
        switch (built)
        {
            case SuccessObjectBuildResult<TClass> success:
                visitMe.SetReference(success.Result);
                break;
            case FailureObjectBuildResult<TClass, TBuilder> failure:
                return Failure(exceptions, visited);
            default: throw new NotSupportedException(built.GetType().FullName);
        }

        return built;
    }

    /// <summary>
    /// Constructs the object of type <typeparamref name="TClass"/> using the specified context of visited objects.
    /// </summary>
    /// <remarks>This method is intended to be implemented by derived classes to define the specific logic for
    /// constructing  an object of type <typeparamref name="TClass"/>. The <paramref name="visited"/> dictionary can be
    /// used to  prevent cyclic dependencies or redundant processing during the build process.</remarks>
    /// <param name="visited">A dictionary used to track objects that have already been processed during the build operation.  This parameter
    /// may be <see langword="null"/> if no tracking is required.</param>
    /// <returns>An instance of <see cref="IObjectBuildResult{TClass}"/> representing the result of the build operation.</returns>
    protected abstract IObjectBuildResult<TClass> BuildInternal(ExceptionBuildList exceptions, VisitedObjectsList visited);


    /// <summary>
    /// Creates a failure result containing the specified error message and a record of visited objects.
    /// </summary>
    /// <remarks>This method is intended to be overridden in derived classes to customize the behavior of
    /// failure handling.</remarks>
    /// <param name="message">The error message describing the reason for the failure.</param>
    /// <param name="visited">A dictionary of objects that have been visited during the operation, used to track state and prevent cycles.</param>
    /// <returns>A <see cref="FailureObjectBuildResult{TClass, TBuilder}"/> representing the failure, including the error details and
    /// visited objects.</returns>
    protected virtual FailureObjectBuildResult<TClass, TBuilder> Failure(string message, VisitedObjectsList visited)
    {
        return new FailureObjectBuildResult<TClass, TBuilder>((TBuilder)(object)this, [
            new BasicObjectBuildException<TClass, TBuilder>(message, (TBuilder)(this as IObjectBuilder<TClass>), visited),
        ], visited);
    }

    /// <summary>
    /// Creates a failure result containing the specified exceptions and a record of visited objects.
    /// </summary>
    /// <param name="exceptions"></param>
    /// <param name="visited"></param>
    /// <returns></returns>
    protected virtual FailureObjectBuildResult<TClass, TBuilder> Failure(ExceptionBuildList exceptions, VisitedObjectsList visited)
    {
        return new FailureObjectBuildResult<TClass, TBuilder>((TBuilder)(object)this, exceptions, visited);
    }

    /// <summary>
    /// Creates a successful result containing the specified instance.
    /// </summary>
    /// <param name="instance">The instance to include in the successful result. Cannot be <see langword="null"/>.</param>
    /// <returns>A <see cref="SuccessObjectBuildResult{TClass}"/> representing a successful operation with the provided instance.</returns>
    protected virtual SuccessObjectBuildResult<TClass> Success(TClass instance)
    {
        return new SuccessObjectBuildResult<TClass>(instance);
    }

    /// <summary>
    /// Builds a list of object build results by invoking the builder on each provided instance.
    /// </summary>
    /// <typeparam name="TOtherClass">The type of object to be built by each builder.</typeparam>
    /// <typeparam name="TOtherBuilder">The type of builder used to construct objects of type <typeparamref name="TOtherClass"/>. Must implement <see
    /// cref="IObjectBuilder{TOtherClass}"/>.</typeparam>
    /// <param name="instances">A list of builder instances that will be used to construct objects.</param>
    /// <param name="visited">A list tracking objects that have already been built to prevent redundant construction or circular references.</param>
    /// <returns>A list of build results for each object constructed by the provided builders.</returns>
    protected List<IObjectBuildResult<TOtherClass>> BuildList<TOtherClass, TOtherBuilder>(List<TOtherBuilder> instances, VisitedObjectsList visited) where TOtherBuilder : IObjectBuilder<TOtherClass>
    {
        return instances.Select(x => x.Build(visited)).ToList();
    }

    /// <summary>
    /// Adds exceptions from failed object build results to the specified exception list.
    /// </summary>
    /// <remarks>Only exceptions from build results of type <see cref="FailureObjectBuildResult{TOtherClass,
    /// TOtherBuilder}"/> are added. If there are no failures, the exception list remains unchanged.</remarks>
    /// <typeparam name="TOtherClass">The type of object being built in the build results.</typeparam>
    /// <typeparam name="TOtherBuilder">The type of builder used to construct objects of type <typeparamref name="TOtherClass"/>.</typeparam>
    /// <param name="results">A list of object build results from which exceptions will be collected.</param>
    /// <param name="exceptions">The exception list to which exceptions from failed build results will be added.</param>
    protected void AddExceptions<TOtherClass, TOtherBuilder>(List<IObjectBuildResult<TOtherClass>> results, ExceptionBuildList exceptions) where TOtherBuilder : IObjectBuilder<TOtherClass>
    {
        if (!results.OfType<FailureObjectBuildResult<TOtherClass, TOtherBuilder>>().Any())
        {
            return;
        }

        exceptions.AddRange(results.OfType<FailureObjectBuildResult<TOtherClass, TOtherBuilder>>().SelectMany(x => x.Exceptions));
    }

    /// <summary>
    /// Holds a reference to an instance of <typeparamref name="TClass"/> or <see langword="null"/> if no reference is set.
    /// </summary>
    protected TClass? _reference;

    /// <summary>
    /// Holds a reference to instance of <typeparamref name="TBuilder"/> or <see langword="null"/> if no reference is set.
    /// </summary>
    private TBuilder? _referencedBuilder;

    /// <summary>
    /// Sets the reference to the specified person and returns the current builder instance.
    /// </summary>
    /// <param name="person">The person object to be used as the reference. Cannot be null.</param>
    /// <returns>The current builder instance, cast to the type specified by <typeparamref name="TBuilder"/>.</returns>
    /// <exception cref="InvalidCastException">Thrown if the current instance cannot be cast to <typeparamref name="TBuilder"/>.</exception>
    protected TBuilder Is(TClass person)
    {
        _reference = person;
        return this as TBuilder ?? throw new InvalidCastException();
    }

    /// <summary>
    /// Sets the reference to the specified person and returns the current builder instance.
    /// </summary>
    /// <param name="person">The person object to be used as the reference. Cannot be null.</param>
    /// <returns>The current builder instance, cast to the type specified by <typeparamref name="TBuilder"/>.</returns>
    /// <exception cref="InvalidCastException">Thrown if the current instance cannot be cast to <typeparamref name="TBuilder"/>.</exception>
    public TBuilder References(TBuilder person)
    {
        _referencedBuilder = person;
        return this as TBuilder ?? throw new InvalidCastException();
    }

    /// <summary>
    /// Sets the reference value using the specified delegate and returns the current builder instance.
    /// </summary>
    /// <param name="func">A delegate that provides the reference value to assign. Cannot be null.</param>
    /// <returns>The current builder instance, cast to the type parameter <typeparamref name="TBuilder"/>.</returns>
    /// <exception cref="InvalidCastException">Thrown if the current instance cannot be cast to <typeparamref name="TBuilder"/>.</exception>
    protected TBuilder References(Func<TClass> func)
    {
        _reference = func();
        return this as TBuilder ?? throw new InvalidCastException();
    }
}
