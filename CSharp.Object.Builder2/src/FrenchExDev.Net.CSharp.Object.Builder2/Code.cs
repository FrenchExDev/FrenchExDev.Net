namespace FrenchExDev.Net.CSharp.Object.Builder2;

/// <summary>
/// Represents the result of a build operation, providing information about the outcome and any associated data.
/// </summary>
/// <remarks>Implementations of this interface typically expose properties or methods to access build status,
/// errors, and output artifacts. The specific details available depend on the concrete implementation.</remarks>
public interface IBuildResult
{

}

/// <summary>
/// Represents a successful result of a build operation, containing the constructed object instance.
/// </summary>
/// <typeparam name="TClass">The type of the object produced by the build operation. Must be a reference type.</typeparam>
/// <param name="instance">The object instance that was successfully built. Cannot be null.</param>
public class SuccessBuildResult<TClass>(TClass instance) : IBuildResult where TClass : class
{
    public TClass Object { get; } = instance;
}

/// <summary>
/// Represents a collection that maps member names to lists of failure objects, allowing multiple validation failures to
/// be associated with each member.
/// </summary>
/// <remarks>Use this class to accumulate and organize validation failures for individual members, such as
/// properties or fields, during data validation processes. Each member name can be associated with one or more failure
/// objects, enabling detailed tracking of validation issues. This class extends <see cref="Dictionary{TKey, TValue}"/>
/// and provides additional functionality for managing validation failures.</remarks>
public class FailuresDictionary : Dictionary<string, List<object>>
{
    /// <summary>
    /// Adds an entry to the dictionary that associates the specified member name with the given exception to be thrown
    /// for invalid data.
    /// </summary>
    /// <param name="memberName">The name of the member to associate with the exception. Cannot be null.</param>
    /// <param name="exception">The exception to be thrown when the specified member contains invalid data. Cannot be null.</param>
    /// <returns>The current <see cref="ExceptionBuildDictionary"/> instance with the new association added.</returns>
    public FailuresDictionary Failure<T>(string memberName, T failure) where T : class
    {
        var list = this.GetValueOrDefault(memberName);
        if (list is null)
        {
            list = [];
            this[memberName] = list;
        }
        list.Add(failure);
        return this;
    }
}

/// <summary>
/// Represents the result of a build operation that has failed, including details about the failures encountered.
/// </summary>
/// <param name="failures">A dictionary containing information about each failure that occurred during the build process. Cannot be null.</param>
public class FailureBuildResult(FailuresDictionary failures) : IBuildResult
{
    public FailuresDictionary Failures { get; } = failures;
}

/// <summary>
/// Represents a dictionary that maps unique object identifiers to their corresponding object instances. Intended for
/// tracking visited objects during operations such as serialization or object graph traversal.
/// </summary>
/// <remarks>This class is typically used to prevent processing the same object multiple times, such as when
/// handling cyclic references or ensuring objects are only visited once in recursive algorithms. It inherits all
/// functionality from the generic Dictionary type.</remarks>
public class VisitedObjectDictionary : Dictionary<Guid, object>
{

}

/// <summary>
/// Defines a contract for building objects and managing their references within a construction workflow.
/// </summary>
/// <remarks>Implementations of this interface facilitate object construction, reference management, and
/// post-build actions. The interface supports tracking build results and provides hooks for custom logic after an
/// object is built.</remarks>
/// <typeparam name="TClass">The type of object being constructed. Must be a reference type.</typeparam>
/// <typeparam name="TReference">The type representing a reference to the constructed object. Must implement <see cref="IReference{TClass}"/>.</typeparam>
public interface IBuilder<TClass, TReference> where TClass : class where TReference : class, IReference<TClass>
{
    TReference Reference();
    Guid Id { get; }
    IBuildResult? Result { get; }
    IBuildResult? Build(VisitedObjectDictionary? visitedCollector = null);
    void OnBuilt(Action<TClass> hook);
}

/// <summary>
/// Represents a reference to an instance of a specified class type, providing mechanisms to resolve and access the
/// referenced object.
/// </summary>
/// <remarks>This interface allows deferred resolution of an object instance, enabling scenarios where the
/// referenced object may not be available at construction time. The reference can be resolved later using the Resolve
/// method, after which the Instance property provides access to the resolved object. The IsResolved property indicates
/// whether the reference has been successfully resolved.</remarks>
/// <typeparam name="TClass">The type of the class instance referenced by this interface. Must be a reference type.</typeparam>
public interface IReference<TClass> : IBuildResult where TClass : class
{
    TClass? Instance { get; }
    bool IsResolved => Instance is not null;
    void Resolve(TClass instance);
}

/// <summary>
/// Represents an abstract reference to an instance of a specified class type, providing mechanisms to resolve and
/// access the referenced object.
/// </summary>
/// <remarks>This class serves as a base for reference wrappers that may be resolved at runtime. It provides
/// methods to set and retrieve the referenced instance, and indicates whether the reference has been resolved. Derived
/// types can extend this functionality to implement custom resolution strategies.</remarks>
/// <typeparam name="TClass">The type of the class instance referenced by this object. Must be a reference type.</typeparam>
public class Reference<TClass> : IReference<TClass> where TClass : class
{
    public TClass? Instance { get; protected set; }
    public bool IsResolved => Instance is not null;
    public void Resolve(TClass instance)
    {
        Instance = instance;
    }
    public TClass Resolved() => Instance ?? throw new InvalidOperationException("Reference is not resolved yet.");
}

/// <summary>
/// Provides a base class for building objects of type <typeparamref name="TClass"/> using a reference of type
/// <typeparamref name="TReference"/>. Supports validation, build hooks, and result tracking for the build process.
/// </summary>
/// <remarks>This abstract class defines the core workflow for object construction, including validation and
/// result management. Derived classes should implement the abstract methods to provide specific build and validation
/// logic. The builder tracks build state and supports hooks that execute after an object is built. Thread safety is not
/// guaranteed; concurrent usage may require external synchronization.</remarks>
/// <typeparam name="TClass">The type of object to be constructed by the builder. Must be a reference type.</typeparam>
/// <typeparam name="TReference">The reference type used to track and resolve the built object. Must implement <see cref="IReference{TClass}"/> and
/// have a parameterless constructor.</typeparam>
public abstract class AbstractBuilder<TClass, TReference> : IBuilder<TClass, TReference> where TClass : class where TReference : class, IReference<TClass>, new()
{
    /// <summary>
    /// Holds actions to be executed after the object of type <typeparamref name="TClass"/> is successfully built.
    /// </summary>
    private readonly List<Action<TClass>> _hooks = [];

    /// <summary>
    /// Holds an instance of the reference type used by the containing class.
    /// </summary>
    protected TReference _reference = new();

    /// <summary>
    /// Gets the unique identifier for this instance.
    /// </summary>
    public Guid Id { get; } = Guid.NewGuid();

    /// <summary>
    /// Returns the current reference of type <typeparamref name="TReference"/> associated with this instance.
    /// </summary>
    /// <returns>The reference object of type <typeparamref name="TReference"/>. The returned value may be <see langword="null"/>
    /// if no reference has been set.</returns>
    public TReference Reference() => _reference;

    /// <summary>
    /// Gets the result of the build operation, if available.
    /// </summary>
    /// <remarks>The value is <c>null</c> if the build has not completed or if no result is available. Access
    /// to this property may be restricted depending on the context in which it is used.</remarks>
    public IBuildResult? Result { get; protected set; }

    /// <summary>
    /// Retrieves the result of the build operation.
    /// </summary>
    /// <returns>An object that represents the result of the build. The result is available only after a successful call to
    /// Build().</returns>
    /// <exception cref="InvalidOperationException">Thrown if Build() has not been called prior to accessing the result.</exception>
    public IBuildResult GetResult() => Result ?? throw new InvalidOperationException("You must call Build() before accessing the Result.");

    /// <summary>
    /// Builds the object and returns the result, performing validation and instantiation as needed.
    /// </summary>
    /// <remarks>If the object has already been visited, the method returns a reference result to avoid
    /// processing it again. Validation is performed before instantiation, and any failures are collected and returned
    /// in the result. This method supports scenarios with complex object graphs and circular references.</remarks>
    /// <param name="visitedCollector">An optional dictionary used to track visited objects during the build process. If provided, it helps prevent
    /// circular references by recording objects that have already been processed.</param>
    /// <returns>An object that represents the result of the build operation. The result indicates success or failure and may
    /// contain validation errors or the instantiated object.</returns>
    public IBuildResult Build(VisitedObjectDictionary? visitedCollector = null)
    {
        if (visitedCollector is not null && visitedCollector.ContainsKey(Id))
        {
            return Reference();
        }

        visitedCollector ??= [];

        visitedCollector.Add(Id, this);

        var failuresCollector = new FailuresDictionary();

        Validate(visitedCollector, failuresCollector);

        BuildInternal(visitedCollector);

        Result = failuresCollector.Count > 0 ? Failure(failuresCollector) : Success(Instantiate());

        if (Result is SuccessBuildResult<TClass> success)
            _reference.Resolve(success.Object);

        return Result;
    }

    /// <summary>
    /// Performs the core build operation
    /// </summary>
    /// <param name="visitedCollector">A dictionary that tracks objects already visited during the build process. Cannot be null.</param>
    protected abstract void BuildInternal(VisitedObjectDictionary visitedCollector);

    /// <summary>
    /// Validates the current object, collecting any failures into the provided dictionary. Uses the visitedCollector to avoid redundant checks.
    /// </summary>
    /// <param name="visitedCollector"></param>
    /// <param name="failures"></param>
    protected void Validate(VisitedObjectDictionary visitedCollector, FailuresDictionary failures)
    {
        if (visitedCollector.ContainsKey(Id))
        {
            return;
        }

        visitedCollector[this.Id] = this;

        ValidateInternal(visitedCollector, failures);
    }

    /// <summary>
    /// Performs validation logic for the current object and records any validation failures encountered.
    /// </summary>
    /// <param name="visitedCollector">A dictionary used to track objects that have already been visited during validation to prevent redundant checks
    /// and handle circular references.</param>
    /// <param name="failures">A dictionary for collecting validation failures, where each entry represents a specific validation error found
    /// during the process.</param>
    protected abstract void ValidateInternal(VisitedObjectDictionary visitedCollector, FailuresDictionary failures);

    /// <summary>
    /// Creates a new instance of the type specified by the generic parameter.
    /// </summary>
    /// <returns>A new instance of <typeparamref name="TClass"/>.</returns>
    protected abstract TClass Instantiate();

    /// <summary>
    /// Creates a new <see cref="SuccessBuildResult{TClass}"/> representing a successful build operation for the
    /// specified instance.
    /// </summary>
    /// <param name="instance">The instance of <typeparamref name="TClass"/> that was successfully built. Cannot be null.</param>
    /// <returns>A <see cref="SuccessBuildResult{TClass}"/> containing the provided instance.</returns>
    protected static SuccessBuildResult<TClass> Success(TClass instance)
    {
        return new SuccessBuildResult<TClass>(instance);
    }

    /// <summary>
    /// Creates a new <see cref="FailureBuildResult"/> instance representing the specified failures.
    /// </summary>
    /// <param name="failures">A dictionary containing failure details to be included in the result. Cannot be null.</param>
    /// <returns>A <see cref="FailureBuildResult"/> that encapsulates the provided failures.</returns>
    protected static FailureBuildResult Failure(FailuresDictionary failures)
    {
        return new FailureBuildResult(failures);
    }

    /// <summary>
    /// Registers a callback to be invoked when the object has completed its build process.
    /// </summary>
    /// <remarks>Use this method to attach custom logic that should run after the build is finalized. Multiple
    /// hooks may be registered; they will be invoked in the order added.</remarks>
    /// <param name="hook">An action to execute with the built instance. Cannot be null.</param>
    public void OnBuilt(Action<TClass> hook)
    {
        _hooks.Add(hook);
    }
}

/// <summary>
/// Provides extension methods for extracting success objects and failure details from build result instances.
/// </summary>
/// <remarks>These methods simplify access to the underlying result data when working with types implementing <see
/// cref="IBuildResult"/>. They throw exceptions if the result does not match the expected success or failure state, so
/// callers should ensure the result type before invoking these methods.</remarks>
public static class Extensions
{
    /// <summary>
    /// Retrieves the successful result object from the specified build result.
    /// </summary>
    /// <remarks>Use this method to extract the result object when the build operation has completed
    /// successfully. If the build result is not successful, an exception is thrown.</remarks>
    /// <typeparam name="TClass">The type of the object contained in the successful build result.</typeparam>
    /// <param name="result">The build result instance from which to retrieve the successful object.</param>
    /// <returns>The object of type <typeparamref name="TClass"/> contained in the successful build result.</returns>
    /// <exception cref="InvalidOperationException">Thrown if <paramref name="result"/> does not represent a successful build result.</exception>
    public static TClass Success<TClass>(this IBuildResult result) where TClass : class
    {
        return result switch
        {
            SuccessBuildResult<TClass> success => success.Object,
            _ => throw new InvalidOperationException("Result is not a success"),
        };
    }

    /// <summary>
    /// Retrieves the collection of failures associated with the specified build result for the given class type.
    /// </summary>
    /// <remarks>Use this method to access detailed failure information when a build operation has failed.
    /// This method should only be called on build results that represent failures; otherwise, an exception is
    /// thrown.</remarks>
    /// <typeparam name="TClass">The class type for which failures are retrieved. Must be a reference type.</typeparam>
    /// <param name="result">The build result to inspect for failures. Must represent a failed build.</param>
    /// <returns>A dictionary containing failure information for the specified class type. The dictionary is empty if no failures
    /// are present.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the specified build result does not represent a failure.</exception>
    public static FailuresDictionary Failures<TClass>(this IBuildResult result) where TClass : class
    {
        return result switch
        {
            FailureBuildResult failure => failure.Failures,
            _ => throw new InvalidOperationException("Result is not a failure"),
        };
    }
}