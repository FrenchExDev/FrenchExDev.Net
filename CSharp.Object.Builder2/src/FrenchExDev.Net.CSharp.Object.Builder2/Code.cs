using System.Collections;
using System.Diagnostics;

namespace FrenchExDev.Net.CSharp.Object.Builder2;

/// <summary>
/// Lightweight wrapper for a validation/build failure payload (exception, nested dictionary, message, etc.).
/// </summary>
public readonly record struct Failure(object Value)
{
    public static implicit operator Failure(Exception ex) => new(ex);
    public static implicit operator Failure(string message) => new(message);
    public static implicit operator Failure(FailuresDictionary dict) => new(dict);
}

/// <summary>
/// Represents the result of a build operation, providing information about the outcome and any associated data.
/// </summary>
public interface IResult { }

/// <summary>
/// Represents a successful result of a build operation, containing the constructed object instance.
/// </summary>
[DebuggerDisplay("Object = {Object}")]
public class SuccessResult<TClass>(TClass instance) : IResult where TClass : class
{
    public TClass Object { get; } = instance;
}

/// <summary>
/// Dictionary mapping member names to collected failures (typed <see cref="Failure"/> instead of object).
/// </summary>
public class FailuresDictionary : Dictionary<string, List<Failure>>
{
    /// <summary>
    /// Adds a failure payload under a member name.
    /// </summary>
    public FailuresDictionary Failure(string memberName, Failure failure)
    {
        if (!TryGetValue(memberName, out var list))
        {
            list = [];
            this[memberName] = list;
        }
        list.Add(failure);
        return this;
    }
}

/// <summary>
/// Failed build result.
/// </summary>
public class FailureResult(FailuresDictionary failures) : IResult
{
    public FailuresDictionary Failures { get; } = failures;
}

/// <summary>
/// Dictionary to track visited objects during validation/build to prevent cycles and redundant work.
/// </summary>
public class VisitedObjectDictionary : Dictionary<Guid, object> { }

/// <summary>
/// Contract for a builder that constructs instances of type <typeparamref name="TClass"/>.
/// </summary>
/// <typeparam name="TClass"></typeparam>
public interface IBuilder<TClass> where TClass : class
{
    /// <summary>
    /// Gets the current status of the build process.
    /// </summary>
    BuildStatus BuildStatus { get; }

    /// <summary>
    /// Gets the current validation status of the object.
    /// </summary>
    ValidationStatus ValidationStatus { get; }

    /// <summary>
    /// Validates the current object and records any validation failures encountered.
    /// </summary>
    /// <remarks>This method performs a recursive validation, updating the provided dictionaries as it
    /// traverses object graphs. It is typically called as part of a larger validation workflow to ensure all related
    /// objects are checked and failures are aggregated.</remarks>
    /// <param name="visitedCollector">A dictionary used to track objects that have already been visited during validation to prevent redundant checks
    /// and circular references. Cannot be null.</param>
    /// <param name="failures">A dictionary in which validation failures are recorded. Each entry represents a specific validation error found
    /// during the process. Cannot be null.</param>
    void Validate(VisitedObjectDictionary visitedCollector, FailuresDictionary failures);

    /// <summary>
    /// Creates a reference to the current instance of type <typeparamref name="TClass"/>.
    /// </summary>
    /// <returns>A <see cref="Reference{TClass}"/> representing a reference to the current instance.</returns>
    Reference<TClass> Reference();

    /// <summary>
    /// Configures the builder to use an existing instance of the class as the basis for further modifications.
    /// </summary>
    /// <param name="instance">The instance of the class to be used for building. Cannot be null.</param>
    /// <returns>An <see cref="IBuilder{TClass}"/> that is initialized with the specified instance.</returns>
    IBuilder<TClass> Existing(TClass instance);

    /// <summary>
    /// Gets the unique identifier for the instance.
    /// </summary>
    Guid Id { get; }

    /// <summary>
    /// Gets the result of the operation, if available.
    /// </summary>
    IResult? Result { get; }

    /// <summary>
    /// Builds and returns the result of the current operation, optionally using a collector to track visited objects
    /// and prevent redundant processing.
    /// </summary>
    /// <param name="visitedCollector">An optional dictionary used to record objects that have already been visited during the build process. If
    /// provided, it helps avoid processing the same object multiple times; if null, no tracking is performed.</param>
    /// <returns>An object implementing <see cref="IResult"/> that represents the outcome of the build operation.</returns>
    IResult Build(VisitedObjectDictionary? visitedCollector = null);
}

/// <summary>
/// Represents an exception that is thrown when a member cannot be resolved from the specified source or owner.
/// </summary>
/// <remarks>Use this exception to indicate that a requested member could not be found or resolved in the given
/// context. The exception provides details about the member's owner and source to assist in diagnosing resolution
/// failures.</remarks>
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
/// Represents a reference to an instance of a specified class type, supporting deferred resolution and result status
/// tracking.
/// </summary>
/// <remarks>Use this type to encapsulate an object reference that may be resolved at a later time. The reference
/// can be checked for resolution status and safely accessed once resolved. This is useful in scenarios where an object
/// may not be immediately available, but consumers need a consistent way to interact with its eventual
/// presence.</remarks>
/// <typeparam name="TClass">The class type of the referenced instance. Must be a reference type.</typeparam>
public record Reference<TClass> : IResult where TClass : class
{
    /// <summary>
    /// Holds the actual instance of the referenced class, if resolved.
    /// </summary>
    private TClass? _instance;

    /// <summary>
    /// Gets the current instance of type <typeparamref name="TClass"/> managed by this object.
    /// </summary>
    public TClass? Instance => _instance;

    /// <summary>
    /// Gets a value indicating whether the current instance has been resolved and is available for use.
    /// </summary>
    public bool IsResolved => _instance is not null;

    /// <summary>
    /// Associates the specified instance with this reference if it has not already been resolved.
    /// </summary>
    /// <param name="instance">The instance of type TClass to associate with this reference. If the reference is already resolved, this
    /// parameter is ignored.</param>
    /// <returns>The current Reference<TClass> object, allowing for method chaining.</returns>
    public Reference<TClass> Resolve(TClass instance)
    {
        if (!IsResolved) _instance = instance;
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
    public TClass? ResolvedOrNull() => Instance;

    /// <summary>
    /// Constructs a new, empty reference that is not yet resolved.
    /// </summary>
    public Reference() { }

    /// <summary>
    /// Constructs a new reference initialized with an existing instance of type <typeparamref name="TClass"/>.
    /// </summary>
    /// <param name="existing"></param>
    public Reference(TClass? existing) { _instance = existing; }
}

/// <summary>
/// Specifies the status of a validation process.
/// </summary>
/// <remarks>Use this enumeration to represent the current state of validation operations, such as input
/// validation or data integrity checks. The possible values indicate whether validation has not started, is in
/// progress, or has completed.</remarks>
public enum ValidationStatus { NotValidated, Validating, Validated }

/// <summary>
/// Specifies the status of a build process.
/// </summary>
public enum BuildStatus { NotBuilding, Building, Built }

/// <summary>
/// Provides a base implementation for building instances of a specified class type using a customizable builder
/// pattern. Supports validation, tracking build status, and managing references to constructed objects.
/// </summary>
/// <remarks>This abstract class defines the core workflow for building and validating objects, including support
/// for tracking build progress and validation status. Derived classes should implement the object instantiation logic
/// by overriding the Instantiate method. The builder can be reused to build multiple instances, and supports handling
/// circular references via the VisitedObjectDictionary parameter in build and validation methods.</remarks>
/// <typeparam name="TClass">The type of class to be constructed by the builder. Must be a reference type.</typeparam>
public abstract class AbstractBuilder<TClass> : IBuilder<TClass> where TClass : class
{
    /// <summary>
    /// Represents a reference to an instance of type <typeparamref name="TClass"/> used internally by derived classes.
    /// </summary>
    protected Reference<TClass> _reference = new();

    /// <summary>
    /// Holds a reference to an existing instance of type <typeparamref name="TClass"/>, or <see langword="null"/> if no
    /// instance is assigned.
    /// </summary>
    protected TClass? _existing;

    /// <summary>
    /// Sets the builder to use an existing instance of the class for further configuration.
    /// </summary>
    /// <param name="instance">The existing instance of the class to be configured. Cannot be null.</param>
    /// <returns>The current builder instance configured to use the specified existing object.</returns>
    public IBuilder<TClass> Existing(TClass instance) { _existing = instance; return this; }

    /// <summary>
    /// Gets the unique identifier for this instance.
    /// </summary>
    public Guid Id { get; } = Guid.NewGuid();

    /// <summary>
    /// Returns a reference to the underlying object of type <typeparamref name="TClass"/>.
    /// </summary>
    /// <returns>A <see cref="Reference{TClass}"/> representing the referenced object.</returns>
    public Reference<TClass> Reference() => _reference;

    /// <summary>
    /// Gets the result of the operation, if available.
    /// </summary>
    /// <remarks>The value is <see langword="null"/> if the operation has not completed or did not produce a
    /// result. Access to this property may be restricted to derived classes.</remarks>
    public IResult? Result { get; protected set; }

    /// <summary>
    /// Gets the current validation status of the object.
    /// </summary>
    public ValidationStatus ValidationStatus { get; private set; }

    /// <summary>
    /// Gets the current status of the build process.
    /// </summary>
    public BuildStatus BuildStatus { get; private set; }

    /// <summary>
    /// Returns the result of the build operation. This method should be called only after a successful call to Build().
    /// </summary>
    /// <returns>An object that represents the result of the build operation.</returns>
    /// <exception cref="InvalidOperationException">Thrown if Build() has not been called prior to accessing the result.</exception>
    public IResult GetResult() => Result ?? throw new InvalidOperationException("You must call Build() before accessing the Result.");

    /// <summary>
    /// Builds the object represented by this builder, performing validation and returning the result of the build
    /// operation.
    /// </summary>
    /// <remarks>If the object has already been built or is currently being validated or built, the method
    /// returns a reference to the existing result. Validation is performed before building, and any validation failures
    /// are returned immediately. The method supports handling circular references by using the <paramref
    /// name="visited"/> dictionary.</remarks>
    /// <param name="visited">An optional dictionary used to track objects that have already been visited during the build process. This helps
    /// prevent redundant validation and building of objects with circular or shared references.</param>
    /// <returns>An <see cref="IResult"/> representing the outcome of the build operation. If validation fails, the result
    /// contains failure details; otherwise, it contains the successfully built object.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the build operation completes successfully but the result is not of the expected success type.</exception>
    public virtual IResult Build(VisitedObjectDictionary? visited = null)
    {
        if (visited is not null && visited.TryGetValue(Id, out var v) && v is IBuilder<TClass> builder)
        {
            if (builder.ValidationStatus is ValidationStatus.NotValidated or ValidationStatus.Validating) return _reference;
            if (builder.BuildStatus is BuildStatus.Built or BuildStatus.Building) return _reference;
        }

        BuildStatus = BuildStatus.Building;

        if (_existing is not null)
        {
            BuildStatus = BuildStatus.Built;
            _reference.Resolve(_existing);
            Result = Success(_existing);
            return Result;
        }

        visited ??= [];
        var failuresCollector = new FailuresDictionary();
        Validate(visited, failuresCollector);
        if (failuresCollector.Count > 0)
        {
            Result = Failure(failuresCollector);
            return Result;
        }

        BuildInternal(visited);
        Result = Success(Instantiate());
        BuildStatus = BuildStatus.Built;
        if (Result is not SuccessResult<TClass> success) throw new InvalidOperationException();
        _reference.Resolve(success.Object);
        return Result;
    }

    /// <summary>
    /// Builds the object and returns the result if the build operation is successful.
    /// </summary>
    /// <param name="visitedCollector">An optional dictionary used to track visited objects during the build process. If provided, it helps prevent
    /// cycles or redundant processing.</param>
    /// <returns>The successfully built object of type TClass.</returns>
    /// <exception cref="AggregateException">Thrown if the build operation fails. The exception contains all errors encountered during the build process.</exception>
    /// <exception cref="InvalidOperationException">Thrown if the build operation results in an unknown or unexpected state.</exception>
    public TClass BuildSuccess(VisitedObjectDictionary? visitedCollector = null)
    {
        var result = Build(visitedCollector);
        return result switch
        {
            SuccessResult<TClass> success => success.Object,
            FailureResult failure => throw new AggregateException("Build failed with the following errors:", failure.Failures.SelectMany(kvp => kvp.Value.Select(f => f.Value)).OfType<Exception>()),
            _ => throw new InvalidOperationException("Build resulted in an unknown state."),
        };
    }

    /// <summary>
    /// Performs custom build operations using the specified visited object dictionary. Intended to be overridden in
    /// derived classes to implement specialized build logic.
    /// </summary>
    /// <remarks>Override this method in a derived class to implement custom build behavior. The provided
    /// dictionary should be updated as objects are processed to ensure correct handling of object graphs.</remarks>
    /// <param name="visitedCollector">A dictionary that tracks objects already visited during the build process. Used to prevent redundant processing
    /// and handle object reference cycles.</param>
    protected virtual void BuildInternal(VisitedObjectDictionary visitedCollector) { }

    /// <summary>
    /// Performs validation on the current object and records any validation failures.
    /// </summary>
    /// <remarks>If the object has already been validated or is currently being validated, this method does
    /// not perform validation again. This ensures that each object is validated only once per validation
    /// cycle.</remarks>
    /// <param name="visitedCollector">A dictionary used to track objects that have already been visited during validation. This helps prevent
    /// redundant validation and circular references.</param>
    /// <param name="failures">A dictionary that collects validation failures encountered during the validation process. Each failure is
    /// recorded for later inspection.</param>
    public void Validate(VisitedObjectDictionary visitedCollector, FailuresDictionary failures)
    {
        switch (ValidationStatus)
        {
            case ValidationStatus.NotValidated:
                visitedCollector[Id] = this;
                ValidationStatus = ValidationStatus.Validating;
                ValidateInternal(visitedCollector, failures);
                ValidationStatus = ValidationStatus.Validated;
                break;
            case ValidationStatus.Validated:
            case ValidationStatus.Validating:
                return;
        }
    }

    /// <summary>
    /// Performs custom validation logic on the current object and records any validation failures encountered.
    /// </summary>
    /// <remarks>Override this method in a derived class to implement object-specific validation rules. This
    /// method is typically called as part of a recursive validation process and should add any detected failures to the
    /// provided dictionary.</remarks>
    /// <param name="visitedCollector">A dictionary used to track objects that have already been visited during validation to prevent redundant checks
    /// and handle circular references.</param>
    /// <param name="failures">A dictionary for collecting validation failures, where each entry represents a specific validation error found
    /// during the process.</param>
    protected virtual void ValidateInternal(VisitedObjectDictionary visitedCollector, FailuresDictionary failures) { }

    /// <summary>
    /// Creates a new instance of the type represented by <typeparamref name="TClass"/>.
    /// </summary>
    /// <returns>A new instance of <typeparamref name="TClass"/>.</returns>
    protected abstract TClass Instantiate();

    /// <summary>
    /// Creates a new successful result containing the specified instance.
    /// </summary>
    /// <param name="instance">The value to be wrapped in a successful result. Cannot be null.</param>
    /// <returns>A <see cref="SuccessResult{TClass}"/> that encapsulates the provided instance, indicating a successful
    /// operation.</returns>
    protected static SuccessResult<TClass> Success(TClass instance) => new(instance);

    /// <summary>
    /// Creates a new <see cref="FailureResult"/> instance representing one or more failures.
    /// </summary>
    /// <param name="failures">A dictionary containing details of the failures to be represented. Cannot be null.</param>
    /// <returns>A <see cref="FailureResult"/> that encapsulates the specified failures.</returns>
    protected static FailureResult Failure(FailuresDictionary failures) => new(failures);

    /// <summary>
    /// Invokes the build operation on each builder in the specified collection, passing the provided visited object
    /// dictionary to each builder.
    /// </summary>
    /// <typeparam name="TBuilder">The type of builder used to construct instances of <typeparamref name="TModel"/>. Must implement <see
    /// cref="IBuilder{TModel}"/> and have a parameterless constructor.</typeparam>
    /// <typeparam name="TModel">The type of model object to be built by the builders. Must be a reference type.</typeparam>
    /// <param name="builders">A collection of builders that will each perform a build operation for an instance of <typeparamref
    /// name="TModel"/>.</param>
    /// <param name="visitedCollector">A dictionary used to track objects that have already been visited during the build process. This helps prevent
    /// redundant processing or circular references.</param>
    protected void BuildList<TBuilder, TModel>(BuilderList<TModel, TBuilder> builders, VisitedObjectDictionary visitedCollector)
        where TBuilder : IBuilder<TModel>, new() where TModel : class
    {
        foreach (var builder in builders) builder.Build(visitedCollector);
    }

    /// <summary>
    /// Validates each item in the specified builder list and records any validation failures under the given property
    /// name.
    /// </summary>
    /// <remarks>This method iterates through each item in the list, validates it, and aggregates any failures
    /// under the specified property name. Items with no failures are not recorded.</remarks>
    /// <typeparam name="TOtherClass">The type of objects contained in the builder list to validate.</typeparam>
    /// <typeparam name="TOtherBuilder">The type of builder used to construct instances of <typeparamref name="TOtherClass"/>. Must implement <see
    /// cref="IBuilder{TOtherClass}"/> and have a parameterless constructor.</typeparam>
    /// <param name="list">The builder list containing items to validate. Cannot be null.</param>
    /// <param name="propertyName">The name of the property associated with the list. Used to categorize validation failures.</param>
    /// <param name="visitedCollector">A dictionary used to track objects that have already been visited during validation to prevent redundant checks.
    /// Cannot be null.</param>
    /// <param name="failures">A dictionary for collecting validation failures encountered during the process. Cannot be null.</param>
    protected void ValidateListInternal<TOtherClass, TOtherBuilder>(BuilderList<TOtherClass, TOtherBuilder> list, string propertyName, VisitedObjectDictionary visitedCollector, FailuresDictionary failures)
        where TOtherClass : class where TOtherBuilder : class, IBuilder<TOtherClass>, new()
    {
        foreach (var item in list)
        {
            var itemFailures = new FailuresDictionary();
            item.Validate(visitedCollector, itemFailures);
            if (itemFailures.Count > 0) failures.Failure(propertyName, new Failure(itemFailures));
        }
    }

    /// <summary>
    /// Checks whether the specified string value is not empty or consists solely of whitespace characters, and records
    /// a failure if the condition is not met.
    /// </summary>
    /// <param name="value">The string value to validate. If <paramref name="value"/> is null, no failure is recorded.</param>
    /// <param name="name">The name associated with the value being validated. Used to identify the failure.</param>
    /// <param name="failures">The dictionary in which validation failures are recorded.</param>
    /// <param name="builder">A function that creates an exception for the failure, given the parameter name.</param>
    protected void AssertNotEmptyOrWhitespace(string? value, string name, FailuresDictionary failures, Func<string, Exception> builder)
    {
        if (value is null) return; if (string.IsNullOrWhiteSpace(value)) failures.Failure(name, builder(name));
    }

    /// <summary>
    /// Validates that each string in the specified list is not null, empty, or consists only of white-space characters.
    /// If any invalid string is found, records a failure using the provided dictionary and exception builder.
    /// </summary>
    /// <param name="value">The list of strings to validate. If <paramref name="value"/> is null, no validation is performed.</param>
    /// <param name="name">The name of the parameter being validated. Used as a key in the failures dictionary.</param>
    /// <param name="failures">A dictionary for recording validation failures. An entry is added for each invalid string found.</param>
    /// <param name="builder">A function that creates an exception for a given parameter name when a validation failure occurs.</param>
    protected void AssertNotEmptyOrWhitespace(List<string>? value, string name, FailuresDictionary failures, Func<string, Exception> builder)
    {
        if (value is null) return; foreach (var item in value) AssertNotEmptyOrWhitespace(item, name, failures, builder);
    }

    /// <summary>
    /// Asserts that the specified string is not null, empty, or consists only of white-space characters. If the
    /// assertion fails, records the failure using the provided failures dictionary and exception builder.
    /// </summary>
    /// <param name="value">The string value to validate. The assertion fails if this value is null, empty, or contains only white-space
    /// characters.</param>
    /// <param name="name">The name of the parameter being validated. Used for reporting failures.</param>
    /// <param name="failures">A dictionary used to record validation failures. The failure is added if the assertion does not pass.</param>
    /// <param name="builder">A function that creates an exception for the specified parameter name. Invoked when the assertion fails.</param>
    protected void AssertNotNullOrEmptyOrWhitespace(string? value, string name, FailuresDictionary failures, Func<string, Exception> builder)
    {
        if (string.IsNullOrWhiteSpace(value)) failures.Failure(name, builder(name));
    }

    /// <summary>
    /// Checks that the specified value is not null and records a failure using the provided failure dictionary and
    /// exception builder if the value is null.
    /// </summary>
    /// <param name="value">The object to validate for non-nullity.</param>
    /// <param name="name">The name associated with the value, used for reporting failures.</param>
    /// <param name="failures">The dictionary that records validation failures.</param>
    /// <param name="builder">A function that creates an exception for the specified name when a failure is detected.</param>
    protected void AssertNotNull(object? value, string name, FailuresDictionary failures, Func<string, Exception> builder)
    {
        if (value is null) failures.Failure(name, builder(name));
    }

    /// <summary>
    /// Validates that the specified collection is not null and, if it contains string elements, that none are null,
    /// empty, or consist only of white-space characters. Records a failure using the provided failures dictionary and
    /// exception builder if the validation does not pass.
    /// </summary>
    /// <remarks>If the collection contains elements of type string, each element is checked to ensure it is
    /// not null, empty, or white-space only. If any validation fails, a failure is recorded using the provided failures
    /// dictionary and exception builder.</remarks>
    /// <typeparam name="TOtherClass">The type of elements contained in the collection to validate.</typeparam>
    /// <param name="list">The collection to validate for null and, if containing strings, for empty or white-space-only elements.</param>
    /// <param name="name">The name of the parameter being validated. Used for failure reporting.</param>
    /// <param name="failures">The dictionary used to record validation failures.</param>
    /// <param name="builder">A function that creates an exception for a given parameter name when a validation failure occurs.</param>
    protected void AssertNotNullNotEmptyCollection<TOtherClass>(List<TOtherClass>? list, string name, FailuresDictionary failures, Func<string, Exception> builder)
    {
        if (list is null) { failures.Failure(name, builder(name)); return; }
        foreach (var item in list)
            if (item is string str && string.IsNullOrWhiteSpace(str))
                failures.Failure(name, builder(name));
    }

    /// <summary>
    /// Evaluates the specified predicate and, if it returns true, records a failure for the given name using the
    /// provided exception builder.
    /// </summary>
    /// <remarks>This method is typically used to assert conditions and collect failures without immediately
    /// throwing exceptions. It allows deferred exception handling and aggregation of multiple failures.</remarks>
    /// <param name="predicate">A function that returns a boolean value indicating whether a failure should be recorded.</param>
    /// <param name="name">The name or key associated with the failure to be recorded.</param>
    /// <param name="failures">The dictionary that collects failure information and exceptions.</param>
    /// <param name="builder">A function that creates an exception instance based on the failure name.</param>
    protected void Assert(Func<bool> predicate, string name, FailuresDictionary failures, Func<string, Exception> builder)
    {
        if (predicate())
            failures.Failure(name, builder(name));
    }
}

/// <summary>
/// Base exception type for build-related errors.
/// </summary>
/// <param name="message"></param>
public class BuildException(string message) : Exception(message) { }

/// <summary>
/// String is null, empty or whitespace exception.
/// </summary>
/// <param name="member"></param>
public class StringIsEmptyOrWhitespaceException(string member) : BuildException($"The member {member} is empty or contains only whitespaces") { }

/// <summary>
/// Build failed exception.
/// </summary>
/// <param name="failures"></param>
public class BuildFailedException(FailuresDictionary failures) : BuildException("Build failed") { public FailuresDictionary Failures { get; } = failures; }

/// <summary>
/// Build succeeded exception.
/// </summary>
/// <param name="instance"></param>
public class BuildSucceededException(object instance) : BuildException("Build succeeded") { public object Instance { get; } = instance; }

/// <summary>
/// Provides extension methods for working with result objects, enabling convenient access to success and failure
/// information.
/// </summary>
/// <remarks>The methods in this class extend the <see cref="IResult"/> interface, allowing callers to retrieve
/// success objects, failure details, and to check the result state. These extensions simplify handling of result types
/// in scenarios where operations may succeed or fail, such as build or processing workflows.</remarks>
public static class Extensions
{
    /// <summary>
    /// Returns the result object of type <typeparamref name="TClass"/> if the operation was successful; otherwise,
    /// throws an exception indicating failure or unsupported result type.
    /// </summary>
    /// <remarks>Use this method to safely retrieve the object from a successful result. If the result is not
    /// successful or is of an unsupported type, an exception is thrown.</remarks>
    /// <typeparam name="TClass">The type of the object contained in a successful result. Must be a reference type.</typeparam>
    /// <param name="result">The result to extract the successful object from. Must implement <see cref="IResult"/>.</param>
    /// <returns>The object of type <typeparamref name="TClass"/> contained in the successful result.</returns>
    /// <exception cref="BuildFailedException">Thrown if <paramref name="result"/> represents a failure. The exception contains details about the failures.</exception>
    /// <exception cref="NotSupportedException">Thrown if <paramref name="result"/> is not a supported result type.</exception>
    public static TClass Success<TClass>(this IResult result) where TClass : class
        => result switch
        {
            SuccessResult<TClass> success => success.Object,
            FailureResult failures => throw new BuildFailedException(failures.Failures),
            _ => throw new NotSupportedException(result.GetType().FullName),
        };

    /// <summary>
    /// Retrieves the collection of failures associated with the specified result.
    /// </summary>
    /// <remarks>Use this method to access failure details when processing results. If the result indicates
    /// success, an exception is thrown to signal that no failures are present.</remarks>
    /// <typeparam name="TClass">The type of the object contained in a successful result.</typeparam>
    /// <param name="result">The result from which to obtain failure information. Must be an instance of either FailureResult or
    /// SuccessResult<TClass>.</param>
    /// <returns>A FailuresDictionary containing details about the failures if the result represents a failure.</returns>
    /// <exception cref="BuildSucceededException">Thrown if the result is a SuccessResult<TClass>, indicating that the build succeeded and no failures are
    /// available.</exception>
    /// <exception cref="NotSupportedException">Thrown if the result is not a recognized result type.</exception>
    public static FailuresDictionary Failures<TClass>(this IResult result) where TClass : class
        => result switch
        {
            FailureResult failure => failure.Failures,
            SuccessResult<TClass> success => throw new BuildSucceededException(success.Object),
            _ => throw new NotSupportedException(result.GetType().FullName),
        };

    /// <summary>
    /// Determines whether the specified result represents a successful operation containing a value of the specified
    /// type.
    /// </summary>
    /// <typeparam name="T">The type of the value expected in a successful result. Must be a reference type.</typeparam>
    /// <param name="result">The result to evaluate for success and type compatibility.</param>
    /// <returns>true if the result is a SuccessResult containing a value of type T; otherwise, false.</returns>
    public static bool IsSuccess<T>(this IResult result) where T : class => result is SuccessResult<T>;

    /// <summary>
    /// Determines whether the specified result represents a failure.
    /// </summary>
    /// <param name="result">The result to evaluate for failure status. Cannot be null.</param>
    /// <returns>true if the result is a failure; otherwise, false.</returns>
    public static bool IsFailure(this IResult result) => result is FailureResult;
}

/// <summary>
/// Represents a list of object references that supports enumeration, querying, and reference-based operations.
/// </summary>
/// <remarks>This interface extends <see cref="IList{TClass}"/> to provide additional methods for working with
/// references, including LINQ-style querying and reference containment checks. It is commonly used in scenarios where
/// objects are managed or tracked by reference rather than by value.</remarks>
/// <typeparam name="TClass">The type of class objects referenced by the list. Must be a reference type.</typeparam>
public interface IReferenceList<TClass> : IList<TClass> where TClass : class
{
    /// <summary>
    /// Returns an enumerable collection of resolved instances of type <typeparamref name="TClass"/> contained in the
    /// </summary>
    /// <returns></returns>
    IEnumerable<TClass> AsEnumerable();

    /// <summary>
    /// Gets the queryable collection of entities of type <typeparamref name="TClass"/> for LINQ operations.
    /// </summary>
    /// <remarks>Use this property to construct LINQ queries against the underlying data source. The returned
    /// <see cref="IQueryable{TClass}"/> supports deferred execution and can be used to filter, sort, and project
    /// entities before materializing results.</remarks>
    IQueryable<TClass> Queryable { get; }

    /// <summary>
    /// Returns the element at the specified zero-based index in the collection.
    /// </summary>
    /// <param name="index">The zero-based index of the element to retrieve. Must be greater than or equal to 0 and less than the total
    /// number of elements in the collection.</param>
    /// <returns>The element of type TClass at the specified index.</returns>
    TClass ElementAt(int index); bool Any(Func<TClass, bool> value); bool Any(); void Add(Reference<TClass> reference); bool Contains(Reference<TClass> reference);
}

/// <summary>
/// Implements a list of references to objects of type <typeparamref name="TClass"/>, providing methods for adding,
/// </summary>
/// <typeparam name="TClass"></typeparam>
public class ReferenceList<TClass> : IReferenceList<TClass> where TClass : class
{
    /// <summary>
    /// List of references to objects of type <typeparamref name="TClass"/>.
    /// </summary>
    private readonly List<Reference<TClass>> _references;

    /// <summary>
    /// Initializes a new instance of the ReferenceList class with the specified collection of references.
    /// </summary>
    /// <param name="references">An enumerable collection of Reference<TClass> objects to include in the list. Cannot be null.</param>
    /// <exception cref="ArgumentNullException">Thrown if references is null.</exception>
    public ReferenceList(IEnumerable<Reference<TClass>> references)
    {
        _references = references.ToList() ?? throw new ArgumentNullException(nameof(references));
    }

    /// <summary>
    /// Returns an enumerable collection of all resolved instances referenced by this object.
    /// </summary>
    /// <remarks>Only references that are both resolved and have a non-null instance are included in the
    /// returned collection. The enumeration reflects the current state of the references at the time of the
    /// call.</remarks>
    /// <returns>An <see cref="IEnumerable{TClass}"/> containing each resolved instance. The collection will be empty if no
    /// references are resolved.</returns>
    public IEnumerable<TClass> AsEnumerable()
    {
        foreach (var r in _references) if (r.IsResolved && r.Instance is not null) yield return r.Instance;
    }

    /// <summary>
    /// Initializes a new instance of the ReferenceList class.
    /// </summary>
    public ReferenceList() { _references = []; }

    /// <summary>
    /// Adds the specified reference to the collection.
    /// </summary>
    /// <param name="reference">The reference to add. Cannot be null.</param>
    public void Add(Reference<TClass> reference) => _references.Add(reference);

    /// <summary>
    /// Adds a reference to the specified instance of type TClass to the collection.
    /// </summary>
    /// <param name="instance">The instance of type TClass to be referenced and added. Cannot be null.</param>
    public void Add(TClass instance) => _references.Add(new Reference<TClass>().Resolve(instance));

    /// <summary>
    /// Determines whether the collection contains the specified reference.
    /// </summary>
    /// <param name="reference">The reference to locate in the collection. Cannot be null.</param>
    /// <returns>true if the specified reference is found in the collection; otherwise, false.</returns>
    public bool Contains(Reference<TClass> reference) => _references.Contains(reference);

    /// <summary>
    /// Determines whether the specified instance is present among the resolved references.
    /// </summary>
    /// <param name="instance">The object to locate within the collection of resolved references. Can be null if null references are supported.</param>
    /// <returns>true if the specified instance is found among the resolved references; otherwise, false.</returns>
    public bool Contains(TClass instance)
    {
        foreach (var r in _references) if (r.IsResolved && r.Instance == instance) return true; return false;
    }

    /// <summary>
    /// Gets an <see cref="IQueryable{TClass}"/> that enables LINQ queries over the collection of <typeparamref
    /// name="TClass"/> entities.
    /// </summary>
    /// <remarks>The returned <see cref="IQueryable{TClass}"/> supports deferred execution and can be used to
    /// compose LINQ queries. Changes made to the underlying collection after obtaining the queryable may not be
    /// reflected in previously constructed queries.</remarks>
    public IQueryable<TClass> Queryable => AsEnumerable().AsQueryable();

    /// <summary>
    /// Returns the resolved element at the specified index in the collection.
    /// </summary>
    /// <param name="index">The zero-based index of the element to retrieve. Must be within the bounds of the collection.</param>
    /// <returns>The resolved element of type TClass at the specified index.</returns>
    /// <exception cref="NotResolvedException">Thrown if the element at the specified index cannot be resolved.</exception>
    public TClass ElementAt(int index) => _references[index].Resolved() ?? throw new NotResolvedException(nameof(_references));

    /// <summary>
    /// Determines whether any resolved reference satisfies the specified predicate.
    /// </summary>
    /// <remarks>Only references that are resolved and have a non-null instance are considered. The predicate
    /// is not invoked for unresolved or null instances.</remarks>
    /// <param name="value">A function that defines the condition to test for each resolved instance. The function receives a non-null
    /// instance of type TClass and should return <see langword="true"/> to indicate a match.</param>
    /// <returns>true if at least one resolved reference matches the predicate; otherwise, false.</returns>
    public bool Any(Func<TClass, bool> value)
    {
        foreach (var r in _references) if (r.IsResolved && r.Instance is not null && value(r.Instance)) return true; return false;
    }

    /// <summary>
    /// Determines whether any referenced item is both resolved and has a non-null instance.
    /// </summary>
    /// <returns>true if at least one reference is resolved and its instance is not null; otherwise, false.</returns>
    public bool Any()
    {
        foreach (var r in _references) if (r.IsResolved && r.Instance is not null) return true; return false;
    }

    /// <summary>
    /// Determines whether all resolved references satisfy the specified predicate.
    /// </summary>
    /// <remarks>Only references that are resolved and have a non-null instance are evaluated. Unresolved
    /// references or references with null instances are considered as not satisfying the condition.</remarks>
    /// <param name="value">A function that defines the condition to test for each resolved instance. The function receives a resolved
    /// instance of type TClass and should return <see langword="true"/> if the condition is met; otherwise, <see
    /// langword="false"/>.</param>
    /// <returns>true if every resolved reference has a non-null instance and the predicate returns true for each instance;
    /// otherwise, false.</returns>
    public bool All(Func<TClass, bool> value)
    {
        foreach (var r in _references) if (!(r.IsResolved && r.Instance is not null && value(r.Instance))) return false; return true;
    }

    /// <summary>
    /// Returns an enumerable collection of resolved instances that satisfy the specified predicate.
    /// </summary>
    /// <remarks>Only instances that are both resolved and not <see langword="null"/> are considered. The
    /// returned sequence is evaluated lazily.</remarks>
    /// <param name="predicate">A function to test each resolved instance for a condition. The function should return <see langword="true"/> to
    /// include the instance in the result; otherwise, <see langword="false"/>.</param>
    /// <returns>An <see cref="IEnumerable{TClass}"/> containing the resolved instances for which <paramref name="predicate"/>
    /// returns <see langword="true"/>. The collection is empty if no instances match the condition.</returns>
    public IEnumerable<TClass> Where(Func<TClass, bool> predicate)
    {
        foreach (var r in _references) if (r.IsResolved && r.Instance is not null && predicate(r.Instance)) yield return r.Instance;
    }

    /// <summary>
    /// Projects each resolved reference in the collection into a new form by applying the specified mapping function.
    /// </summary>
    /// <remarks>Only references that are resolved and have a non-null instance are included in the result.
    /// The mapping function is applied to each such instance in the collection.</remarks>
    /// <typeparam name="TOtherClass">The type of the value returned by the mapping function.</typeparam>
    /// <param name="mapper">A function that transforms each resolved instance of type TClass into a value of type TOtherClass. Cannot be
    /// null.</param>
    /// <returns>An enumerable collection of mapped values of type TOtherClass, corresponding to each resolved reference.</returns>
    public IEnumerable<TOtherClass> Select<TOtherClass>(Func<TClass, TOtherClass> mapper)
    {
        foreach (var r in _references) if (r.IsResolved && r.Instance is not null) yield return mapper(r.Instance);
    }

    /// <summary>
    /// Returns the zero-based index of the first resolved reference whose instance matches the specified item.
    /// </summary>
    /// <param name="item">The object to locate in the collection. The search is performed among resolved references whose instance equals
    /// this item.</param>
    /// <returns>The zero-based index of the first occurrence of the specified item among resolved references.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the specified item is not found among resolved references.</exception>
    public int IndexOf(TClass item)
    {
        for (int i = 0; i < _references.Count; i++) { var r = _references[i]; if (r.IsResolved && r.Instance == item) return i; }
        throw new InvalidOperationException("Item not found");
    }

    /// <summary>
    /// Inserts the specified item into the collection at the given index.
    /// </summary>
    /// <param name="index">The zero-based index at which the item should be inserted. Must be greater than or equal to 0 and less than or
    /// equal to the number of items in the collection.</param>
    /// <param name="item">The item to insert into the collection.</param>
    public void Insert(int index, TClass item) => _references.Insert(index, new Reference<TClass>().Resolve(item));

    /// <summary>
    /// Removes the element at the specified index from the collection.
    /// </summary>
    /// <param name="index">The zero-based index of the element to remove. Must be greater than or equal to 0 and less than the number of
    /// elements in the collection.</param>
    public void RemoveAt(int index) => _references.RemoveAt(index);

    /// <summary>
    /// Removes all items from the collection.
    /// </summary>
    public void Clear() => _references.Clear();

    /// <summary>
    /// Copies the resolved instances contained in the collection to the specified array, starting at the specified
    /// array index.
    /// </summary>
    /// <remarks>Only instances that are resolved and not null are copied. The method does not resize the
    /// destination array or skip indices; ensure the array has enough capacity to hold all resolved instances starting
    /// at the specified index.</remarks>
    /// <param name="array">The destination array to which the resolved instances will be copied. Must not be null and must have sufficient
    /// space to accommodate the copied elements.</param>
    /// <param name="arrayIndex">The zero-based index in the destination array at which copying begins. Must be within the bounds of the array.</param>
    public void CopyTo(TClass[] array, int arrayIndex)
    {
        var i = arrayIndex; foreach (var r in _references) if (r.IsResolved && r.Instance is not null) array[i++] = r.Instance;
    }

    /// <summary>
    /// Removes the specified item from the collection if it is present and resolved.
    /// </summary>
    /// <param name="item">The item to remove from the collection. Only items that are currently resolved will be considered for removal.</param>
    /// <returns>true if the item was found and removed; otherwise, false.</returns>
    public bool Remove(TClass item)
    {
        for (int i = 0; i < _references.Count; i++) { var r = _references[i]; if (r.IsResolved && r.Instance == item) { _references.RemoveAt(i); return true; } }
        return false;
    }

    /// <summary>
    /// Returns an enumerator that iterates through the collection.
    /// </summary>
    /// <returns>An enumerator that can be used to iterate through the collection of <typeparamref name="TClass"/> objects.</returns>
    public IEnumerator<TClass> GetEnumerator() => AsEnumerable().GetEnumerator();

    /// <summary>
    /// Returns an enumerator that iterates through the collection.
    /// </summary>
    /// <returns></returns>
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>
    /// Gets the number of references contained in the collection.
    /// </summary>
    public int Count => _references.Count;

    /// <summary>
    /// Gets a value indicating whether the collection is read-only.
    /// </summary>
    public bool IsReadOnly => false;

    /// <summary>
    /// Gets or sets the resolved reference at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index of the reference to retrieve or assign. Must be within the bounds of the collection.</param>
    /// <returns>The resolved reference of type TClass at the specified index.</returns>
    public TClass this[int index]
    {
        get => _references[index].Resolved();
        set => _references[index] = new Reference<TClass>().Resolve(value);
    }
}

/// <summary>
/// Represents a collection of builder objects that can construct instances of a specified class type. Provides methods
/// to build, validate, and convert builders to reference lists.
/// </summary>
/// <remarks>Use this class to manage a set of builders for batch construction, validation, or reference
/// conversion of objects of type <typeparamref name="TClass"/>. The collection supports adding new builders, building
/// all objects, and retrieving validation or build failures for each builder.</remarks>
/// <typeparam name="TClass">The type of class instances to be constructed by the builders. Must be a reference type.</typeparam>
/// <typeparam name="TBuilder">The type of builder used to construct instances of <typeparamref name="TClass"/>. Must implement <see
/// cref="IBuilder{TClass}"/> and have a parameterless constructor.</typeparam>
public class BuilderList<TClass, TBuilder> : List<TBuilder>
where TClass : class
where TBuilder : IBuilder<TClass>, new()
{
    /// <summary>
    /// Creates a new reference list containing references to each item in the collection.
    /// </summary>
    /// <returns>A <see cref="ReferenceList{TClass}"/> containing references to the items in the current collection.</returns>
    public ReferenceList<TClass> AsReferenceList()
    {
        var references = this.Select(x => x.Reference());
        return new(references);
    }

    /// <summary>
    /// Creates a new builder instance, applies the specified configuration action, and adds the builder to the list.
    /// </summary>
    /// <param name="body">An action that configures the newly created builder instance before it is added to the list. Cannot be null.</param>
    /// <returns>The current list containing the newly added and configured builder instance.</returns>
    public BuilderList<TClass, TBuilder> New(Action<TBuilder> body)
    {
        var builder = new TBuilder(); body(builder); Add(builder); return this;
    }

    /// <summary>
    /// Builds and returns a list of successful results of type <typeparamref name="TClass"/> from the current
    /// collection.
    /// </summary>
    /// <returns>A list containing the successful results of type <typeparamref name="TClass"/>. The list will be empty if no
    /// successful results are found.</returns>
    public List<TClass> BuildSuccess() => [.. this.Select(x => x.Build().Success<TClass>())];

    /// <summary>
    /// Builds and returns a list of failure dictionaries for each item in the collection.
    /// </summary>
    /// <returns>A list of <see cref="FailuresDictionary"/> instances representing the failures associated with each built item.
    /// The list will be empty if there are no items or no failures.</returns>
    public List<FailuresDictionary> BuildFailures() => [.. this.Select(x => x.Build().Failures<TClass>())];

    /// <summary>
    /// Returns a list of failure dictionaries resulting from validating each builder in the collection.
    /// </summary>
    /// <returns></returns>
    public List<FailuresDictionary> ValidateFailures()
    {
        var visited = new VisitedObjectDictionary();

        return [.. this.Select(x => {
            var failures = new FailuresDictionary();
            x.Validate(visited, failures);
            return failures;
        }).Where(f => f.Count > 0)];
    }
}
