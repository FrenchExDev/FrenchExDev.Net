using System.Collections;
using System.Diagnostics;

namespace FrenchExDev.Net.CSharp.Object.Builder2;

/// <summary>
/// Represents the result of a build operation, providing information about the outcome and any associated data.
/// </summary>
/// <remarks>Implementations of this interface typically expose properties or methods to access build status,
/// errors, and output artifacts. The specific details available depend on the concrete implementation.</remarks>
public interface IResult
{

}

/// <summary>
/// Represents a successful result of a build operation, containing the constructed object instance.
/// </summary>
/// <typeparam name="TClass">The type of the object produced by the build operation. Must be a reference type.</typeparam>
/// <param name="instance">The object instance that was successfully built. Cannot be null.</param>
[DebuggerDisplay("Object = {Object}")]
public class SuccessResult<TClass>(TClass instance) : IResult where TClass : class
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
public class FailureResult(FailuresDictionary failures) : IResult
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
/// Defines a builder interface for constructing instances of a specified reference type.
/// </summary>
/// <typeparam name="TClass">The type of class to be constructed by the builder. Must be a reference type.</typeparam>
public interface IBuilder<TClass> : IBuilder<TClass, Reference<TClass>> where TClass : class
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
    /// <summary>
    /// Validates the current object and records any validation failures encountered.
    /// </summary>
    /// <param name="visitedCollector">A dictionary used to track objects that have already been visited during validation to prevent redundant checks
    /// and circular references.</param>
    /// <param name="failures">A dictionary that collects validation failure information for objects that do not meet validation criteria.</param>
    void Validate(VisitedObjectDictionary visitedCollector, FailuresDictionary failures);

    /// <summary>
    /// Returns a reference to the underlying value of type <typeparamref name="TReference"/>.
    /// </summary>
    /// <returns>A reference to the value of type <typeparamref name="TReference"/>.</returns>
    TReference Reference();

    /// <summary>
    /// Performs an operation on the specified instance of <typeparamref name="TClass"/>.
    /// </summary>
    /// <param name="instance">The instance of <typeparamref name="TClass"/> to operate on. Cannot be null.</param>
    IBuilder<TClass, TReference> Existing(TClass instance);

    /// <summary>
    /// Gets the unique identifier for the instance.
    /// </summary>
    Guid Id { get; }
    /// <summary>
    /// Gets the result of the build operation, if available.
    /// </summary>
    /// <remarks>If the build has not completed or failed to produce a result, this property returns <see
    /// langword="null"/>. The returned <see cref="IResult"/> provides details about the outcome of the build
    /// process.</remarks>
    IResult? Result { get; }

    /// <summary>
    /// Builds the result object by traversing the current structure, optionally tracking visited objects to prevent
    /// redundant processing or circular references.
    /// </summary>
    /// <remarks>If the structure being built contains circular references, providing a <paramref
    /// name="visitedCollector"/> is recommended to ensure correct traversal and avoid stack overflow exceptions. The
    /// returned result reflects the state after traversing all reachable objects.</remarks>
    /// <param name="visitedCollector">An optional dictionary used to record objects that have already been visited during the build process. If
    /// provided, it helps avoid processing the same object multiple times and can prevent infinite loops in structures
    /// with cycles. If null, no tracking is performed.</param>
    /// <returns>An object implementing <see cref="IResult"/> that represents the outcome of the build operation.</returns>
    IResult Build(VisitedObjectDictionary? visitedCollector = null);

    /// <summary>
    /// Registers a callback to be invoked when the object has been fully constructed.
    /// </summary>
    /// <param name="hook">The action to execute after the object is built. The callback receives the constructed instance as its
    /// parameter.</param>
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
public interface IReference<TClass> : IResult where TClass : class
{
    /// <summary>
    /// Gets the current instance of type <typeparamref name="TClass"/> if available.
    /// </summary>
    TClass? Instance { get; }

    /// <summary>
    /// Gets a value indicating whether the instance has been successfully resolved.
    /// </summary>
    bool IsResolved => Instance is not null;

    /// <summary>
    /// Resolves a reference to the specified instance of type TClass.
    /// </summary>
    /// <param name="instance">The object instance for which to resolve a reference. Cannot be null.</param>
    /// <returns>An IReference<TClass> representing a reference to the specified instance.</returns>
    IReference<TClass> Resolve(TClass instance);

    /// <summary>
    /// Returns the resolved instance of type <typeparamref name="TClass"/> if the reference has been resolved.
    /// </summary>
    /// <returns>The resolved instance of type <typeparamref name="TClass"/>. Throws a <see cref="NotResolvedException"/> if the
    /// reference is not resolved.</returns>
    /// <exception cref="NotResolvedException">Thrown if the reference has not been resolved.</exception>
    TClass Resolved();
}

/// <summary>
/// Represents an exception that is thrown when a member cannot be resolved from a specified source.
/// </summary>
/// <remarks>Use this exception to indicate that a requested member was not found or could not be resolved during
/// an operation, such as dependency resolution or mapping. The <see cref="MemberOwner"/> and <see
/// cref="MemberSource"/> properties provide details about the unresolved member and its source, which can assist in
/// diagnosing the cause of the failure.</remarks>
public class NotResolvedException : Exception
{
    /// <summary>
    /// Gets the name of the owner associated with the member.
    /// </summary>
    public string MemberOwner { get; init; }

    /// <summary>
    /// Gets the source code or textual representation associated with the member.
    /// </summary>
    public string MemberSource { get; init; }

    /// <summary>
    /// Initializes a new instance of the NotResolvedException class with the specified target and source member names.
    /// </summary>
    /// <param name="target">The name of the member that could not be resolved.</param>
    /// <param name="source">The name of the source from which the member resolution was attempted.</param>
    public NotResolvedException(string target, string source)
    {
        MemberOwner = target;
        MemberSource = source;
    }

    /// <summary>
    /// Initializes a new instance of the NotResolvedException class for the specified target member.
    /// </summary>
    /// <param name="target">The name of the member that could not be resolved. Cannot be null.</param>
    public NotResolvedException(string target)
    {
        MemberOwner = target;
        MemberSource = target;
    }
}

/// <summary>
/// Represents an abstract reference to an instance of a specified class type, providing mechanisms to resolve and
/// access the referenced object.
/// </summary>
/// <remarks>This class serves as a base for reference wrappers that may be resolved at runtime. It provides
/// methods to set and retrieve the referenced instance, and indicates whether the reference has been resolved. Derived
/// types can extend this functionality to implement custom resolution strategies.</remarks>
/// <typeparam name="TClass">The type of the class instance referenced by this object. Must be a reference type.</typeparam>
[Serializable]
public class Reference<TClass> : IReference<TClass> where TClass : class
{
    /// <summary>
    /// Stores the instance of type <typeparamref name="TClass"/> that this reference points to.
    /// </summary>
    private TClass? _instance;

    /// <summary>
    /// Gets the current instance of type <typeparamref name="TClass"/> associated with this object.
    /// </summary>
    public TClass? Instance => _instance;

    /// <summary>
    /// Gets a value indicating whether the instance has been resolved and is available for use.
    /// </summary>
    public bool IsResolved => _instance is not null;

    /// <summary>
    /// Associates the specified instance with this reference and returns the updated reference.
    /// </summary>
    /// <param name="instance">The instance of type TClass to associate with this reference. Cannot be null.</param>
    /// <returns>The current reference object with its instance set to the specified value.</returns>
    public IReference<TClass> Resolve(TClass instance)
    {
        _instance = instance;
        return this;
    }

    /// <summary>
    /// Returns the resolved instance of type <typeparamref name="TClass"/>. Throws an exception if the reference has
    /// not been resolved.
    /// </summary>
    /// <returns>The resolved instance of type <typeparamref name="TClass"/>.</returns>
    /// <exception cref="NotResolvedException">Thrown if the reference has not been resolved.</exception>
    public TClass Resolved() => _instance ?? throw new NotResolvedException("Reference is not resolved yet.");

    /// <summary>
    /// Returns the resolved instance of type <typeparamref name="TClass"/> if available; otherwise, returns <see
    /// langword="null"/>.
    /// </summary>
    /// <returns>The resolved <typeparamref name="TClass"/> instance, or <see langword="null"/> if no instance is available.</returns>
    public TClass? ResolvedOrNull() => Instance ?? null;

    /// <summary>
    /// Initializes a new instance of the Reference class.
    /// </summary>
    public Reference() { }

    /// <summary>
    /// Initializes a new instance of the Reference class that wraps the specified object.
    /// </summary>
    /// <param name="existing">The object to be referenced. Can be null to indicate that no object is currently referenced.</param>
    public Reference(TClass? existing)
    {
        _instance = existing;
    }
}

/// <summary>
/// Provides a base class for building objects of type <typeparamref name="TClass"/> using a reference-based builder
/// pattern.
/// </summary>
/// <typeparam name="TClass">The type of object to be constructed by the builder. Must be a reference type.</typeparam>
public abstract class AbstractBuilder<TClass> : AbstractBuilder<TClass, Reference<TClass>>, IBuilder<TClass> where TClass : class
{

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
    /// Holds an existing instance of the type being built, if one has been provided.
    /// </summary>
    protected TClass? _existing;

    /// <summary>
    /// Sets the current instance of type TClass to be used by the class.
    /// </summary>
    /// <param name="instance">The instance of type TClass to assign. Cannot be null.</param>
    public IBuilder<TClass, TReference> Existing(TClass instance)
    {
        _existing = instance;
        return this;
    }

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
    public IResult? Result { get; protected set; }

    /// <summary>
    /// Retrieves the result of the build operation.
    /// </summary>
    /// <returns>An object that represents the result of the build. The result is available only after a successful call to
    /// Build().</returns>
    /// <exception cref="InvalidOperationException">Thrown if Build() has not been called prior to accessing the result.</exception>
    public IResult GetResult() => Result ?? throw new InvalidOperationException("You must call Build() before accessing the Result.");

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
    public IResult Build(VisitedObjectDictionary? visitedCollector = null)
    {
        if (visitedCollector is not null && visitedCollector.ContainsKey(Id))
        {
            return Reference();
        }

        if (_existing is not null)
        {
            _reference.Resolve(_existing);
            Result = Success(_existing);
            ExecuteHooks(_existing);
            return Result;
        }

        visitedCollector ??= [];

        var failuresCollector = new FailuresDictionary();

        Validate(visitedCollector, failuresCollector);

        if (failuresCollector.Count > 0)
        {
            Result = Failure(failuresCollector);
            return Result;
        }

        BuildInternal(visitedCollector);

        Result = Success(Instantiate());

        if (Result is SuccessResult<TClass> success)
        {
            _reference.Resolve(success.Object);
            ExecuteHooks(success.Object);
        }

        return Result;
    }

    /// <summary>
    /// Invokes all registered hook delegates for the specified instance of <typeparamref name="TClass"/>.
    /// </summary>
    /// <remarks>Hooks are executed in the order in which they were registered. This method does not handle
    /// exceptions thrown by individual hooks; callers should ensure that hooks are robust or handle exceptions as
    /// needed.</remarks>
    /// <param name="instance">The instance of <typeparamref name="TClass"/> to pass to each hook delegate.</param>
    private void ExecuteHooks(TClass instance)
    {
        foreach (var hook in _hooks)
        {
            hook(instance);
        }
    }

    /// <summary>
    /// Builds and returns an instance of type <typeparamref name="TClass"/> if the build process completes
    /// successfully.
    /// </summary>
    /// <param name="visitedCollector">An optional dictionary used to track visited objects during the build process. If provided, it helps prevent
    /// cycles or redundant processing.</param>
    /// <returns>An instance of <typeparamref name="TClass"/> representing the successfully built object.</returns>
    /// <exception cref="AggregateException">Thrown if the build process fails. The exception contains all errors encountered during the build.</exception>
    /// <exception cref="InvalidOperationException">Thrown if the build process results in an unknown or unsupported state.</exception>
    public TClass BuildSuccess(VisitedObjectDictionary? visitedCollector = null)
    {
        var result = Build(visitedCollector);
        return result switch
        {
            SuccessResult<TClass> success => success.Object,
            FailureResult failure => throw new AggregateException("Build failed with the following errors:", failure.Failures.SelectMany(f => f.Value).OfType<Exception>()),
            _ => throw new InvalidOperationException("Build resulted in an unknown state."),
        };
    }

    /// <summary>
    /// Performs the core build operation
    /// </summary>
    /// <param name="visitedCollector">A dictionary that tracks objects already visited during the build process. Cannot be null.</param>
    protected virtual void BuildInternal(VisitedObjectDictionary visitedCollector)
    {
        // nothing to do
    }

    /// <summary>
    /// Validates the current object, collecting any failures into the provided dictionary. Uses the visitedCollector to avoid redundant checks.
    /// </summary>
    /// <param name="visitedCollector"></param>
    /// <param name="failures"></param>
    public void Validate(VisitedObjectDictionary visitedCollector, FailuresDictionary failures)
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
    protected void ValidateInternal(VisitedObjectDictionary visitedCollector, FailuresDictionary failures)
    {

    }

    /// <summary>
    /// Creates a new instance of the type specified by the generic parameter.
    /// </summary>
    /// <returns>A new instance of <typeparamref name="TClass"/>.</returns>
    protected abstract TClass Instantiate();

    /// <summary>
    /// Creates a new <see cref="SuccessResult{TClass}"/> representing a successful build operation for the
    /// specified instance.
    /// </summary>
    /// <param name="instance">The instance of <typeparamref name="TClass"/> that was successfully built. Cannot be null.</param>
    /// <returns>A <see cref="SuccessResult{TClass}"/> containing the provided instance.</returns>
    protected static SuccessResult<TClass> Success(TClass instance)
    {
        return new SuccessResult<TClass>(instance);
    }

    /// <summary>
    /// Creates a new <see cref="FailureResult"/> instance representing the specified failures.
    /// </summary>
    /// <param name="failures">A dictionary containing failure details to be included in the result. Cannot be null.</param>
    /// <returns>A <see cref="FailureResult"/> that encapsulates the provided failures.</returns>
    protected static FailureResult Failure(FailuresDictionary failures)
    {
        return new FailureResult(failures);
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

    /// <summary>
    /// Invokes the build process for each builder in the specified collection, using the provided visited object
    /// dictionary to track processed objects.
    /// </summary>
    /// <typeparam name="TBuilder">The type of builder used to construct model instances. Must inherit from AbstractBuilder<TModel>.</typeparam>
    /// <typeparam name="TModel">The type of model being constructed. Must be a reference type.</typeparam>
    /// <param name="builders">A collection of builders that will be invoked to construct model instances.</param>
    /// <param name="visitedCollector">A dictionary used to track objects that have already been processed during the build operation.</param>
    protected void BuildList<TBuilder, TModel>(BuilderList<TModel, TBuilder> builders, VisitedObjectDictionary visitedCollector)
        where TBuilder : IBuilder<TModel>, new()
        where TModel : class
    {
        foreach (var builder in builders)
        {
            builder.Build(visitedCollector);
        }
    }

    /// <summary>
    /// Validates each item in the specified builder list and records any validation failures under the given property
    /// name.
    /// </summary>
    /// <remarks>This method is intended for internal use during complex object graph validation. Validation
    /// failures for individual items are grouped under the specified property name in the failures
    /// dictionary.</remarks>
    /// <typeparam name="TOtherClass">The type of object being built and validated. Must be a reference type.</typeparam>
    /// <typeparam name="TOtherBuilder">The builder type for <typeparamref name="TOtherClass"/>. Must implement <see cref="IBuilder{TClass}"/> and have a
    /// parameterless constructor.</typeparam>
    /// <param name="list">The list of builder objects to validate. Cannot be null.</param>
    /// <param name="propertyName">The name of the property under which any validation failures will be recorded. Cannot be null or empty.</param>
    /// <param name="visitedCollector">A dictionary used to track objects that have already been visited during validation to prevent redundant checks.
    /// Cannot be null.</param>
    /// <param name="failures">A dictionary that collects validation failures found during the process. Cannot be null.</param>
    protected void ValidateListInternal<TOtherClass, TOtherBuilder>(BuilderList<TOtherClass, TOtherBuilder> list, string propertyName, VisitedObjectDictionary visitedCollector, FailuresDictionary failures)
        where TOtherClass : class
        where TOtherBuilder : class, IBuilder<TOtherClass>, new()
    {
        foreach (var item in list)
        {
            var itemFailures = new FailuresDictionary();
            item.Validate(visitedCollector, itemFailures);
            if (itemFailures.Count > 0)
            {
                failures.Failure(propertyName, itemFailures);
            }
        }
    }

    public class StringIsEmptyOrWhitespaceException(string member) : Exception($"The member {member} is empty or contains only whitespaces") { }

    /// <summary>
    /// Asserts that the specified string value is not empty, or consists only of white-space characters. If the
    /// assertion fails, records a failure using the provided failures dictionary and exception builder.
    /// </summary>
    /// <remarks>This method does not throw an exception directly; instead, it records the failure in the
    /// provided failures dictionary. The exception builder allows customization of the exception type and message
    /// associated with the failure.</remarks>
    /// <param name="value">The string value to validate. Must not be empty, or contain only white-space characters.</param>
    /// <param name="name">The name of the parameter being validated. Used for reporting failures.</param>
    /// <param name="failures">The dictionary used to record validation failures.</param>
    /// <param name="builder">A function that creates an exception for the specified parameter name when the assertion fails.</param>
    protected void AssertNotEmptyOrWhitespace(string? value, string name, FailuresDictionary failures, Func<string, Exception> builder)
    {
        if (value is null) return;

        if (string.IsNullOrWhiteSpace(value))
        {
            failures.Failure(name, builder(name));
        }
    }

    /// <summary>
    /// Validates that each string in the specified list is not null, empty, or consists only of white-space characters.
    /// Records a failure for each invalid entry using the provided failures dictionary and exception builder.
    /// </summary>
    /// <param name="value">The list of strings to validate. If null, no validation is performed.</param>
    /// <param name="name">The name of the parameter being validated. Used in failure reporting.</param>
    /// <param name="failures">A dictionary used to record validation failures for the parameter.</param>
    /// <param name="builder">A function that creates an exception for a given parameter name when a validation failure occurs.</param>
    protected void AssertNotEmptyOrWhitespace(List<string>? value, string name, FailuresDictionary failures, Func<string, Exception> builder)
    {
        if (value is null) return;

        foreach (var item in value)
        {
            AssertNotEmptyOrWhitespace(value, name, failures, builder);
        }
    }

    /// <summary>
    /// Asserts that the specified string value is not null, empty, or consists only of white-space characters. If the
    /// assertion fails, records a failure for the given parameter name using the provided exception builder.
    /// </summary>
    /// <param name="value">The string value to validate. The assertion fails if this value is null, empty, or contains only white-space
    /// characters.</param>
    /// <param name="name">The name of the parameter being validated. Used to identify the failure in the failures dictionary.</param>
    /// <param name="failures">A dictionary used to record validation failures. The failure is added if the value does not meet the required
    /// criteria.</param>
    /// <param name="builder">A function that creates an exception for the specified parameter name. Invoked when the assertion fails.</param>
    protected void AssertNotNullOrEmptyOrWhitespace(string? value, string name, FailuresDictionary failures, Func<string, Exception> builder)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            failures.Failure(name, builder(name));
        }
    }

    /// <summary>
    /// Checks whether the specified value is not null and records a failure using the provided dictionary and exception
    /// builder if it is null.
    /// </summary>
    /// <param name="value">The object to validate for non-nullity.</param>
    /// <param name="name">The name associated with the value, used for reporting failures.</param>
    /// <param name="failures">The dictionary used to record validation failures.</param>
    /// <param name="builder">A function that creates an exception for the specified name when a failure is detected.</param>
    protected void AssertNotNull(object? value, string name, FailuresDictionary failures, Func<string, Exception> builder)
    {
        if (value is null)
        {
            failures.Failure(name, builder(name));
        }
    }
    /// <summary>
    /// Validates that the specified collection is not null and does not contain any string elements that are null,
    /// empty, or consist only of white-space characters. If an invalid string is found, records a failure using the
    /// provided failures dictionary and exception builder.
    /// </summary>
    /// <remarks>Only string elements within the collection are checked for null, empty, or white-space
    /// values. Non-string elements are ignored during validation.</remarks>
    /// <typeparam name="TOtherClass">The type of elements contained in the collection to validate.</typeparam>
    /// <param name="list">The collection of elements to validate. If any element is a string, it must not be null, empty, or white-space
    /// only.</param>
    /// <param name="name">The name associated with the collection, used for failure reporting.</param>
    /// <param name="failures">The dictionary used to record validation failures.</param>
    /// <param name="builder">A function that creates an exception for a given name when a validation failure occurs.</param>
    protected void AssertNotNullNotEmptyCollection<TOtherClass>(List<TOtherClass>? list, string name, FailuresDictionary failures, Func<string, Exception> builder)
    {
        if (list is null)
        {
            failures.Failure(name, builder(name));
            return;
        }

        foreach (var item in list)
        {
            if (item is string str && string.IsNullOrWhiteSpace(str))
            {
                failures.Failure(name, builder(name));
            }
        }
    }

    /// <summary>
    /// Evaluates the specified predicate and records a failure using the provided failures dictionary and exception
    /// builder if the predicate returns true.
    /// </summary>
    /// <remarks>This method allows custom validation logic to be executed and failures to be tracked in a
    /// centralized manner. The exception generated by the builder is not thrown, but is instead recorded in the
    /// failures dictionary for later processing.</remarks>
    /// <param name="predicate">A function that returns a Boolean value indicating whether the failure condition is met.</param>
    /// <param name="name">The name or key associated with the failure to record.</param>
    /// <param name="failures">The dictionary used to record failure information.</param>
    /// <param name="builder">A function that creates an exception instance based on the failure name.</param>
    protected void Assert(Func<bool> predicate, string name, FailuresDictionary failures, Func<string, Exception> builder)
    {
        if (predicate())
        {
            failures.Failure(name, builder(name));
        }
    }
}

/// <summary>
/// Represents errors that occur during the build process.
/// </summary>
/// <param name="message">The error message that describes the reason for the build failure.</param>
public class BuildException(string message) : Exception(message)
{
}

/// <summary>
/// Represents an exception that is thrown when a build process fails due to one or more errors.
/// </summary>
/// <param name="failures">A dictionary containing details about each failure that caused the build to fail. Cannot be null.</param>
public class BuildFailedException(FailuresDictionary failures) : BuildException("Build failed")
{
    /// <summary>
    /// Gets a collection containing details of all failures encountered during the operation.
    /// </summary>
    /// <remarks>Use this property to inspect which items failed and the associated error information. The
    /// returned dictionary is read-only and reflects the failures as of the last operation.</remarks>
    public FailuresDictionary Failures { get; } = failures;
}

/// <summary>
/// Represents an exception that is thrown to indicate that a build operation has completed successfully.
/// </summary>
/// <param name="instance">The object instance associated with the successful build operation. This value is accessible through the <see
/// cref="Instance"/> property.</param>
public class BuildSucceededException(object instance) : BuildException("Build succeeded")
{
    /// <summary>
    /// Gets the underlying instance associated with this object.
    /// </summary>
    public object Instance { get; } = instance;
}

/// <summary>
/// Provides extension methods for extracting success objects and failure details from build result instances.
/// </summary>
/// <remarks>These methods simplify access to the underlying result data when working with types implementing <see
/// cref="IResult"/>. They throw exceptions if the result does not match the expected success or failure state, so
/// callers should ensure the result type before invoking these methods.</remarks>
public static class Extensions
{
    /// <summary>
    /// Returns the successfully built object of type <typeparamref name="TClass"/> from the specified build result.
    /// </summary>
    /// <remarks>Use this method to extract the built object from a successful build result. If the build
    /// result represents a failure or an unsupported type, an exception is thrown.</remarks>
    /// <typeparam name="TClass">The type of the object to retrieve from the build result. Must be a reference type.</typeparam>
    /// <param name="result">The build result from which to retrieve the successfully built object.</param>
    /// <returns>The object of type <typeparamref name="TClass"/> if the build result indicates success.</returns>
    /// <exception cref="BuildFailedException">Thrown if the build result indicates failure.</exception>
    /// <exception cref="NotSupportedException">Thrown if the build result type is not supported by this method.</exception>
    public static TClass Success<TClass>(this IResult result) where TClass : class
    {
        return result switch
        {
            SuccessResult<TClass> success => success.Object,
            FailureResult failures => throw new BuildFailedException(failures.Failures),
            _ => throw new NotSupportedException(result.GetType().FullName),
        };
    }

    /// <summary>
    /// Retrieves the collection of build failures from the specified build result.
    /// </summary>
    /// <remarks>Use this method to access failure details when a build does not succeed. If the build result
    /// indicates success, an exception is thrown to signal that no failures are present.</remarks>
    /// <typeparam name="TClass">The type of the object produced by a successful build result.</typeparam>
    /// <param name="result">The build result from which to obtain failure information. Must be an instance of either FailureBuildResult or
    /// SuccessBuildResult<TClass>.</param>
    /// <returns>A FailuresDictionary containing details about the failures associated with the build result.</returns>
    /// <exception cref="BuildSucceededException">Thrown if the build result represents a successful build. The exception contains the successfully built object.</exception>
    /// <exception cref="NotSupportedException">Thrown if the build result is of an unsupported type.</exception>
    public static FailuresDictionary Failures<TClass>(this IResult result) where TClass : class
    {
        return result switch
        {
            FailureResult failure => failure.Failures,
            SuccessResult<TClass> success => throw new BuildSucceededException(success.Object),
            _ => throw new NotSupportedException(result.GetType().FullName),
        };
    }

    /// <summary>
    /// Determines whether the specified build result represents a successful build operation.
    /// </summary>
    /// <typeparam name="T">The type of the value produced by a successful build result.</typeparam>
    /// <param name="result">The build result to evaluate for success.</param>
    /// <returns>true if the build result is a SuccessBuildResult<T>; otherwise, false.</returns>
    public static bool IsSuccess<T>(this IResult result) where T : class => result is SuccessResult<T>;

    /// <summary>
    /// Determines whether the specified build result represents a failure.
    /// </summary>
    /// <param name="result">The build result to evaluate for failure status. Cannot be null.</param>
    /// <returns>true if the build result is a failure; otherwise, false.</returns>
    public static bool IsFailure(this IResult result) => result is FailureResult;
}

/// <summary>
/// Represents a collection that manages references to objects of type TClass, providing methods to add, query, and resolve referenced instances.
/// </summary>
/// <typeparam name="TClass"></typeparam>
public interface IReferenceList<TClass> : IList<TClass> where TClass : class
{
    IEnumerable<TClass> AsEnumerable();
    IQueryable<TClass> Queryable { get; }
    TClass ElementAt(int index);
    bool Any(Func<TClass, bool> value);
    bool Any();
    void Add(IReference<TClass> reference);
    bool Contains(IReference<TClass> reference);
}

/// <summary>
/// Represents a collection that manages references to objects of type TClass, providing methods to add, query, and
/// resolve referenced instances.
/// </summary>
/// <remarks>ReferenceList enables management of references to objects, supporting both direct reference addition
/// and resolution of referenced instances. The collection provides LINQ-compatible querying of resolved objects and
/// allows indexed access with automatic resolution. All references must implement IReference<TClass>. Thread safety is
/// not guaranteed; external synchronization is required for concurrent access.</remarks>
/// <typeparam name="TClass">The type of object referenced and managed by the collection. Must be a reference type.</typeparam>
public class ReferenceList<TClass> : IReferenceList<TClass> where TClass : class
{
    /// <summary>
    /// Holds the list of references to objects of type <typeparamref name="TClass"/>.
    /// </summary>
    private readonly List<IReference<TClass>> _references;

    /// <summary>
    /// Initializes a new instance of the ReferenceList class with the specified collection of references.
    /// </summary>
    /// <param name="references">The list of references to be managed by the ReferenceList. Cannot be null.</param>
    /// <exception cref="ArgumentNullException">Thrown if the references parameter is null.</exception>
    public ReferenceList(IEnumerable<IReference<TClass>> references)
    {
        _references = references.ToList() ?? throw new ArgumentNullException(nameof(references));
    }

    /// <summary>
    /// Returns an enumerable collection of resolved instances of type <typeparamref name="TClass"/>.
    /// </summary>
    /// <remarks>Only references that are marked as resolved are included in the returned collection. The
    /// enumeration reflects the current state of the underlying references at the time of the call.</remarks>
    /// <returns>An <see cref="IEnumerable{TClass}"/> containing all resolved instances. The collection will be empty if no
    /// references are resolved.</returns>
    public IEnumerable<TClass> AsEnumerable() => _references.Where(x => x.IsResolved && x.Instance is not null).Select(x => x.Instance!);

    /// <summary>
    /// Initializes a new instance of the ReferenceList class with an empty collection of references.
    /// </summary>
    public ReferenceList()
    {
        _references = [];
    }

    /// <summary>
    /// Adds the specified reference to the collection.
    /// </summary>
    /// <param name="reference">The reference to add. Cannot be null.</param>
    public void Add(IReference<TClass> reference) => _references.Add(reference);

    /// <summary>
    /// Adds a reference to the specified instance of type TClass.
    /// </summary>
    /// <param name="instance">The instance of type TClass to be referenced. Cannot be null.</param>
    public void Add(TClass instance) => _references.Add(new Reference<TClass>().Resolve(instance));

    /// <summary>
    /// Determines whether the collection contains the specified reference.
    /// </summary>
    /// <param name="reference">The reference to locate in the collection. Cannot be null.</param>
    /// <returns>true if the specified reference is found in the collection; otherwise, false.</returns>
    public bool Contains(IReference<TClass> reference) => _references.Contains(reference);

    /// <summary>
    /// Determines whether the specified instance is present among the resolved references.
    /// </summary>
    /// <param name="instance">The object to locate within the collection of resolved references. Cannot be null.</param>
    /// <returns>true if the specified instance is found among the resolved references; otherwise, false.</returns>
    public bool Contains(TClass instance) => _references.Any(x => x.IsResolved && x.Instance == instance);

    /// <summary>
    /// Gets a queryable collection of resolved instances of type <typeparamref name="TClass"/>.
    /// </summary>
    /// <remarks>The returned collection includes only instances that have been resolved. The collection
    /// supports LINQ queries for filtering, projection, and other operations.</remarks>
    public IQueryable<TClass> Queryable => AsEnumerable().AsQueryable();

    /// <summary>
    /// Returns the resolved element at the specified index in the collection.
    /// </summary>
    /// <param name="index">The zero-based index of the element to retrieve. Must be within the bounds of the collection.</param>
    /// <returns>The resolved element of type TClass at the specified index.</returns>
    /// <exception cref="NotResolvedException">Thrown if the element at the specified index cannot be resolved.</exception>
    public TClass ElementAt(int index) => _references[index].Resolved() ?? throw new NotResolvedException(nameof(_references));

    /// <summary>
    /// Determines whether any resolved references satisfy the specified predicate.
    /// </summary>
    /// <param name="value">A function that defines the condition to test for each resolved instance. The function receives an instance of
    /// <typeparamref name="TClass"/> and returns <see langword="true"/> to indicate a match.</param>
    /// <returns>true if at least one resolved reference matches the predicate; otherwise, false.</returns>
    public bool Any(Func<TClass, bool> value)
    {
        return _references.Any(x => x.IsResolved && x.Instance is not null && value(x.Instance));
    }

    /// <summary>
    /// Determines whether any referenced item is resolved and has a non-null instance.
    /// </summary>
    /// <returns>true if at least one referenced item is resolved and its instance is not null; otherwise, false.</returns>
    public bool Any() => _references.Any(x => x.IsResolved && x.Instance is not null);

    /// <summary>
    /// Determines whether all resolved references satisfy the specified condition.
    /// </summary>
    /// <remarks>Only references that are resolved and have a non-null instance are evaluated by the
    /// predicate. Unresolved or null instances are excluded from the check.</remarks>
    /// <param name="value">A predicate function that defines the condition to test for each resolved instance. The function receives an
    /// instance of <typeparamref name="TClass"/> and should return <see langword="true"/> if the condition is met;
    /// otherwise, <see langword="false"/>.</param>
    /// <returns>true if every resolved reference's instance satisfies the condition specified by <paramref name="value"/>;
    /// otherwise, false.</returns>
    public bool All(Func<TClass, bool> value)
    {
        return _references.All(x => x.IsResolved && x.Instance is not null && value(x.Instance));
    }

    /// <summary>
    /// Returns a filtered collection of resolved instances matching the specified predicate.
    /// </summary>
    /// <param name="predicate">A function to test each resolved instance for a condition.</param>
    /// <returns>An IEnumerable of resolved instances that satisfy the predicate.</returns>
    public IEnumerable<TClass> Where(Func<TClass, bool> predicate)
    {
        return _references.Where(x => x.IsResolved && x.Instance is not null && predicate(x.Instance)).Select(x => x.Instance!);
    }

    /// <summary>
    /// Projects each resolved reference to a new form by applying the specified mapping function.
    /// </summary>
    /// <remarks>Only references that are resolved and have a non-null instance are included in the result.
    /// The mapping function is applied to each such instance in sequence.</remarks>
    /// <typeparam name="TOtherClass">The type of the value returned by the mapping function.</typeparam>
    /// <param name="mapper">A function that transforms a resolved instance of <typeparamref name="TClass"/> to a value of type <typeparamref
    /// name="TOtherClass"/>. Cannot be null.</param>
    /// <returns>An enumerable collection of values produced by applying the mapping function to each resolved reference.</returns>
    public IEnumerable<TOtherClass> Select<TOtherClass>(Func<TClass, TOtherClass> mapper) =>
        AsEnumerable().Select(x => mapper(x));

    /// <summary>
    /// Returns the zero-based index of the first resolved reference whose instance matches the specified item.
    /// </summary>
    /// <remarks>This method only considers references that are marked as resolved. If no resolved reference
    /// matches the specified item, an exception is thrown.</remarks>
    /// <param name="item">The object to locate in the collection. The method searches for a resolved reference whose instance is equal to
    /// this item.</param>
    /// <returns>The zero-based index of the first occurrence of a resolved reference whose instance matches <paramref
    /// name="item"/>; otherwise, throws an exception if no such reference is found.</returns>
    public int IndexOf(TClass item)
    {
        return _references.IndexOf(_references.First(x => x.IsResolved && x.Instance == item));
    }

    /// <summary>
    /// Inserts the specified item into the collection at the given index.
    /// </summary>
    /// <param name="index">The zero-based index at which the item should be inserted. Must be greater than or equal to 0 and less than or
    /// equal to the number of items in the collection.</param>
    /// <param name="item">The item to insert into the collection.</param>
    public void Insert(int index, TClass item)
    {
        _references.Insert(index, new Reference<TClass>().Resolve(item));
    }

    /// <summary>
    /// Removes the reference at the specified index from the collection.
    /// </summary>
    /// <param name="index">The zero-based index of the reference to remove. Must be greater than or equal to 0 and less than the total
    /// number of references.</param>
    public void RemoveAt(int index)
    {
        _references.RemoveAt(index);
    }

    /// <summary>
    /// Removes all references from the collection.
    /// </summary>
    /// <remarks>After calling this method, the collection will be empty. This operation does not affect the
    /// state of the referenced objects themselves.</remarks>
    public void Clear()
    {
        _references.Clear();
    }

    /// <summary>
    /// Copies the elements of the collection to the specified array, starting at the given array index.
    /// </summary>
    /// <remarks>The destination array must be large enough to contain all elements from the collection
    /// starting at the specified index. If the array is not large enough, an exception will be thrown. The method
    /// copies elements in the same order as they are returned by the collection's enumerator.</remarks>
    /// <param name="array">The one-dimensional array that is the destination of the elements copied from the collection. The array must
    /// have sufficient space to accommodate the copied elements.</param>
    /// <param name="arrayIndex">The zero-based index in the destination array at which copying begins. Must be greater than or equal to zero.</param>
    public void CopyTo(TClass[] array, int arrayIndex)
    {
        AsEnumerable().ToList().CopyTo(array, arrayIndex);
    }

    /// <summary>
    /// Removes the specified resolved instance from the collection.
    /// </summary>
    /// <remarks>Only instances that are resolved and present in the collection will be removed. If the
    /// specified instance is not found or is not resolved, the method returns false.</remarks>
    /// <param name="item">The resolved instance to remove from the collection. Must not be null.</param>
    /// <returns>true if the instance was successfully removed; otherwise, false.</returns>
    public bool Remove(TClass item)
    {
        return _references.Remove(_references.First(x => x.IsResolved && x.Instance == item));
    }

    /// <summary>
    /// Returns an enumerator that iterates through the collection of resolved instances.
    /// </summary>
    /// <returns></returns>
    public IEnumerator<TClass> GetEnumerator()
    {
        return AsEnumerable().GetEnumerator();
    }

    /// <summary>
    /// Returns an enumerator that iterates through the collection.
    /// </summary>
    /// <returns>An <see cref="IEnumerator"/> object that can be used to iterate through the collection.</returns>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    /// <summary>
    /// Gets the number of references contained in the collection.
    /// </summary>
    public int Count => _references.Count;

    /// <summary>
    /// Gets a value indicating whether the collection is read-only.
    /// </summary>
    public bool IsReadOnly => false;

    /// <summary>
    /// Gets or sets the element at the specified index in the collection, resolving references as needed.
    /// </summary>
    /// <remarks>Setting a value at the specified index updates the underlying reference to point to the
    /// provided object. Getting a value automatically resolves the reference before returning the object. The index
    /// must be within the bounds of the collection.</remarks>
    /// <param name="index">The zero-based index of the element to get or set.</param>
    /// <returns>The resolved element of type TClass at the specified index.</returns>
    public TClass this[int index]
    {
        get => _references[index].Resolved();
        set => _references[index] = new Reference<TClass>().Resolve(value);
    }
}

/// <summary>
/// Provides a generic collection for managing builder instances and their associated objects, enabling fluent
/// construction and manipulation of a list of items using builder patterns.
/// </summary>
/// <typeparam name="TClass">The type of object to be constructed and managed by the builders. Must be a reference type.</typeparam>
/// <typeparam name="TBuilder">The type of builder used to construct instances of <typeparamref name="TClass"/>. Must implement <see
/// cref="IBuilder{TClass, Reference{TClass}}"/> and have a parameterless constructor.</typeparam>
public class BuilderList<TClass, TBuilder> : BuilderList<TClass, TBuilder, Reference<TClass>>
    where TClass : class
    where TBuilder : IBuilder<TClass, Reference<TClass>>, new()
{

}

/// <summary>
/// Represents a collection of builder objects that can construct and resolve references to instances of a specified
/// class type.
/// </summary>
/// <remarks>This class extends <see cref="List{TBuilder}"/> to provide additional functionality for managing and
/// resolving builder objects. Use <see cref="AsReferenceList"/> to obtain a list of resolved references from the
/// contained builders.</remarks>
/// <typeparam name="TClass">The type of object to be constructed by the builders.</typeparam>
/// <typeparam name="TBuilder">The type of builder that creates instances of <typeparamref name="TClass"/> and provides references.</typeparam>
/// <typeparam name="TReference">The type of reference associated with <typeparamref name="TClass"/>.</typeparam>
public class BuilderList<TClass, TBuilder, TReference> : List<TBuilder>
where TClass : class
where TBuilder : IBuilder<TClass, TReference>, new()
where TReference : class, IReference<TClass>, new()
{
    /// <summary>
    /// Creates a new reference list containing references to each item in the current collection.
    /// </summary>
    /// <returns>A <see cref="ReferenceList{TClass}"/> containing references to the items in the collection.</returns>
    public ReferenceList<TClass> AsReferenceList()
    {
        var references = this.Select(x => x.Reference());
#pragma warning disable IDE0306 // Simplifier l'initialisation des collections
        return new(references);
#pragma warning restore IDE0306 // Simplifier l'initialisation des collections
    }

    /// <summary>
    /// Adds a new builder to the list and applies the specified action to configure it.
    /// </summary>
    /// <remarks>The builder is created, configured using the provided action, and then added to the list.
    /// This method enables fluent configuration of multiple builders in sequence.</remarks>
    /// <param name="body">An action that receives the new builder instance to configure its properties before it is added to the list.</param>
    /// <returns>The current instance of <see cref="BuilderList{TClass, TBuilder}"/> to allow for method chaining.</returns>
    public BuilderList<TClass, TBuilder, TReference> New(Action<TBuilder> body)
    {
        var builder = new TBuilder();
        body(builder);
        Add(builder);
        return this;
    }

    /// <summary>
    /// Builds each item in the collection and returns a list of successful results of type TClass.
    /// </summary>
    /// <returns>A list of TClass instances representing the successful results from building each item. The list will be empty
    /// if no items succeed.</returns>
    public List<TClass> BuildSuccess() => [.. this.Select(x => x.Build().Success<TClass>())];

    /// <summary>
    /// Builds and returns a list of failure dictionaries for each item in the collection.
    /// </summary>
    /// <returns>A list of <see cref="FailuresDictionary"/> instances representing the failures associated with each built item.
    /// The list will be empty if there are no items or no failures.</returns>
    public List<FailuresDictionary> BuildFailures() => [.. this.Select(x => x.Build().Failures<TClass>())];
}