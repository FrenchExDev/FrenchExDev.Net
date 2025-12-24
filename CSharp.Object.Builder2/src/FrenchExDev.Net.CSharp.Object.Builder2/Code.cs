using FrenchExDev.Net.CSharp.Object.Result2;

namespace FrenchExDev.Net.CSharp.Object.Builder2;

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
/// Lightweight wrapper for a validation/build failure payload (exception, nested dictionary, message, etc.).
/// </summary>
public readonly record struct Failure(object Value)
{
    public static implicit operator Failure(Exception ex) => new(ex);
    public static implicit operator Failure(string message) => new(message);
    public static implicit operator Failure(FailuresDictionary dict) => new(dict);
}

/// <summary>
/// Dictionary to track visited objects during validation/build to prevent cycles and redundant work.
/// </summary>
public class VisitedObjectDictionary : Dictionary<Guid, object> { }

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
/// Defines a contract for building objects of type TClass, providing access to build status, validation status, and the
/// result of the build operation.
/// </summary>
/// <typeparam name="TClass">The type of object to be constructed by the builder. Must be a reference type.</typeparam>
public interface IBuilder<TClass> where TClass : class
{
    /// <summary>
    /// Gets the unique identifier for the object.
    /// </summary>
    Guid Id { get; }

    /// <summary>
    /// Gets the result of the operation, if available.
    /// </summary>
    TClass? Result { get; }

    /// <summary>
    /// Creates a reference to the current entity of type <typeparamref name="TClass"/> for use in relationship
    /// navigation or updates.
    /// </summary>
    /// <remarks>Use the returned reference to navigate relationships or to perform operations that require an
    /// explicit entity reference, such as updating related entities. The reference does not load the entity data unless
    /// explicitly requested.</remarks>
    /// <returns>A <see cref="Reference{TClass}"/> representing a reference to the current entity instance.</returns>
    Reference<TClass> Reference();

    /// <summary>
    /// Gets the current status of the build process.
    /// </summary>
    BuildStatus BuildStatus { get; }

    /// <summary>
    /// Gets the current validation status of the object.
    /// </summary>
    ValidationStatus ValidationStatus { get; }

    /// <summary>
    /// Builds and returns the result object of type TClass, optionally tracking visited objects to prevent cycles.
    /// </summary>
    /// <remarks>Use the visited parameter to handle object graphs that may contain cycles or shared
    /// references. Passing the same dictionary across multiple build operations ensures correct handling of such
    /// scenarios.</remarks>
    /// <param name="visited">An optional dictionary used to track objects that have already been visited during the build process. If null, a
    /// new tracking dictionary may be created internally.</param>
    /// <returns>A Result<TClass> containing the constructed object and any associated build information.</returns>
    Result<Reference<TClass>> Build(VisitedObjectDictionary? visited = null);

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
}

/// <summary>
/// Represents an exception that is thrown when attempting to access a reference that has not been resolved.
/// </summary>
public class ReferenceNotResolvedException : Exception
{
    public ReferenceNotResolvedException()
    {
    }

    public ReferenceNotResolvedException(string? message) : base(message)
    {
    }

    public ReferenceNotResolvedException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}

/// <summary>
/// Represents a thread-safe reference to an instance of a specified class type, supporting deferred resolution.
/// </summary>
/// <typeparam name="TClass">The class type of the referenced instance. Must be a reference type.</typeparam>
public record Reference<TClass> where TClass : class
{
    private volatile TClass? _instance;

    public TClass? Instance => _instance;
    public bool IsResolved => _instance is not null;

    public Reference<TClass> Resolve(TClass instance)
    {
        Interlocked.CompareExchange(ref _instance, instance, null);
        return this;
    }

    public TClass Resolved() => _instance ?? throw new ReferenceNotResolvedException("Reference is not resolved yet.");
    public TClass? ResolvedOrNull() => _instance;

    public Reference() { }
    public Reference(TClass? existing) { _instance = existing; }
}

/// <summary>
/// Represents an exception that is thrown when a build process fails and provides details about the failures.
/// </summary>
public class BuildFailureException : Exception
{
    public BuildFailureException(FailuresDictionary failures) { Failures = failures; }
    public FailuresDictionary Failures { get; }
}

/// <summary>
/// Provides a thread-safe abstract base class for building and validating instances of a specified reference type.
/// </summary>
/// <typeparam name="TClass">The type of object to be constructed by the builder. Must be a reference type.</typeparam>
public abstract class AbstractBuilder<TClass> : IBuilder<TClass> where TClass : class
{
    private readonly Guid _guid = Guid.NewGuid();
    private Result<TClass>? _buildResult;
    private int _buildStatus = (int)BuildStatus.NotBuilding;
    private int _validationStatus = (int)ValidationStatus.NotValidated;
    private readonly Reference<TClass> _reference = new();
    private TClass? _existing;
    private readonly object _buildLock = new();
    private readonly object _validateLock = new();

    public Guid Id => _guid;

    public TClass? Result
    {
        get
        {
            lock (_buildLock)
            {
                return _buildResult is not null ? _buildResult.Value.Value : throw new InvalidOperationException();
            }
        }
    }

    public BuildStatus BuildStatus => (BuildStatus)Volatile.Read(ref _buildStatus);
    public ValidationStatus ValidationStatus => (ValidationStatus)Volatile.Read(ref _validationStatus);
    public Reference<TClass> Reference() => _reference;

    /// <summary>
    /// Configures the builder to use an existing instance of the class as the basis for further operations.
    /// When Build is called, this existing instance will be returned without creating a new one.
    /// </summary>
    /// <param name="instance">The existing instance to use. Cannot be null.</param>
    /// <returns>This builder instance for method chaining.</returns>
    public AbstractBuilder<TClass> Existing(TClass instance)
    {
        _existing = instance;
        return this;
    }

    public Result<Reference<TClass>> Build(VisitedObjectDictionary? visited = null)
    {
        // Fast path: if already built with existing instance
        if (_existing is not null && _existing is TClass)
        {
            return Result<Reference<TClass>>.Success(_reference.Resolve(_existing));
        }

        // Check if already built (thread-safe read)
        if (BuildStatus == BuildStatus.Built)
        {
            return Result<Reference<TClass>>.Success(_reference);
        }

        if (visited is not null && visited.TryGetValue(Id, out var v) && v is IBuilder<TClass> builder)
        {
            if (builder.ValidationStatus is ValidationStatus.NotValidated or ValidationStatus.Validating)
                return Result<Reference<TClass>>.Success(_reference);
            if (builder.BuildStatus is BuildStatus.Built or BuildStatus.Building)
                return Result<Reference<TClass>>.Success(_reference);
        }

        // Synchronize the build process
        lock (_buildLock)
        {
            // Double-check after acquiring lock
            if (BuildStatus == BuildStatus.Built)
            {
                return Result<Reference<TClass>>.Success(_reference);
            }

            // Set building status atomically
            Interlocked.Exchange(ref _buildStatus, (int)BuildStatus.Building);

            if (_existing is not null)
            {
                Interlocked.Exchange(ref _buildStatus, (int)BuildStatus.Built);
                _reference.Resolve(_existing);
                _buildResult = Result<TClass>.Success(_existing);
                return Result<Reference<TClass>>.Success(_reference);
            }

            visited ??= [];
            var failuresCollector = new FailuresDictionary();
            Validate(visited, failuresCollector);
            if (failuresCollector.Count > 0)
            {
                _buildResult = Result<TClass>.Failure(new BuildFailureException(failuresCollector));
                return Result<Reference<TClass>>.Success(_reference);
            }

            BuildInternal(visited);
            var instance = Instantiate();
            _buildResult = Result<TClass>.Success(instance);
            Interlocked.Exchange(ref _buildStatus, (int)BuildStatus.Built);
            _reference.Resolve(instance);
            return Result<Reference<TClass>>.Success(_reference);
        }
    }

    /// <summary>
    /// Builds the object and returns the result if the build operation is successful.
    /// </summary>
    /// <param name="visitedCollector">An optional dictionary used to track visited objects during the build process.</param>
    /// <returns>The successfully built object of type TClass.</returns>
    /// <exception cref="AggregateException">Thrown if the build operation fails with validation errors.</exception>
    /// <exception cref="InvalidOperationException">Thrown if the build operation results in an unknown state.</exception>
    public TClass BuildOrThrow(VisitedObjectDictionary? visitedCollector = null)
    {
        Build(visitedCollector);

        lock (_buildLock)
        {
            if (_buildResult is null)
                throw new InvalidOperationException("Build resulted in an unknown state.");

            if (_buildResult.Value.IsSuccess)
                return _buildResult.Value.Value;

            if (_buildResult.Value.TryGetException<BuildFailureException>(out var buildFailure))
            {
                var exceptions = ExtractExceptionsFromFailures(buildFailure!.Failures);
                throw new AggregateException("Build failed with the following errors:", exceptions);
            }

            throw new InvalidOperationException("Build resulted in an unknown state.");
        }
    }

    /// <summary>
    /// Recursively extracts all exceptions from a FailuresDictionary, including nested dictionaries.
    /// </summary>
    private static List<Exception> ExtractExceptionsFromFailures(FailuresDictionary failures)
    {
        var result = new List<Exception>();
        foreach (var kvp in failures)
        {
            foreach (var failure in kvp.Value)
            {
                if (failure.Value is Exception ex)
                {
                    result.Add(ex);
                }
                else if (failure.Value is FailuresDictionary nestedDict)
                {
                    result.AddRange(ExtractExceptionsFromFailures(nestedDict));
                }
            }
        }
        return result;
    }

    /// <summary>
    /// Performs custom build operations. Override in derived classes.
    /// </summary>
    protected virtual void BuildInternal(VisitedObjectDictionary visitedCollector) { }

    /// <summary>
    /// Performs validation on the current object and records any validation failures.
    /// </summary>
    public void Validate(VisitedObjectDictionary visitedCollector, FailuresDictionary failures)
    {
        var currentStatus = (ValidationStatus)Volatile.Read(ref _validationStatus);
        if (currentStatus is ValidationStatus.Validated or ValidationStatus.Validating)
            return;

        lock (_validateLock)
        {
            currentStatus = (ValidationStatus)Volatile.Read(ref _validationStatus);
            if (currentStatus is ValidationStatus.Validated or ValidationStatus.Validating)
                return;

            visitedCollector[Id] = this;
            Interlocked.Exchange(ref _validationStatus, (int)ValidationStatus.Validating);

            try
            {
                ValidateInternal(visitedCollector, failures);
            }
            finally
            {
                Interlocked.Exchange(ref _validationStatus, (int)ValidationStatus.Validated);
            }
        }
    }

    /// <summary>
    /// Performs custom validation logic. Override in derived classes.
    /// </summary>
    protected virtual void ValidateInternal(VisitedObjectDictionary visitedCollector, FailuresDictionary failures) { }

    /// <summary>
    /// Creates a new instance of the type. Must be implemented in derived classes.
    /// </summary>
    protected abstract TClass Instantiate();

    /// <summary>
    /// Invokes the build operation on each builder in the specified collection.
    /// </summary>
    protected void BuildList<TBuilder, TModel>(BuilderList<TModel, TBuilder> builders, VisitedObjectDictionary visitedCollector)
        where TBuilder : IBuilder<TModel>, new() where TModel : class
    {
        foreach (var builder in builders) builder.Build(visitedCollector);
    }

    /// <summary>
    /// Validates each item in the specified builder list.
    /// </summary>
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
        if (value is null) return;
        if (string.IsNullOrWhiteSpace(value)) failures.Failure(name, builder(name));
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
        if (value is null) return;
        foreach (var item in value) AssertNotEmptyOrWhitespace(item, name, failures, builder);
    }

    /// <summary>
    /// Asserts that the specified string is not null, empty, or consists only of white-space characters. If the
    /// assertion fails, records the failure using the provided failures dictionary and exception builder.
    /// </summary>
    /// <param name="value">The string value to validate. The assertion fails if this value is null, empty, or contains only white-space
    /// characters.</param>
    /// <param name="name">The name of the parameter being validated. Used for reporting failures.</param>
    /// <param name="failures">A dictionary used to record validation failures. The failure is added if the assertion does not pass.</param>
    /// <param name="builder">A function that creates an exception instance based on the failure name. Invoked when the assertion fails.</param>
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
    /// Validates that the specified collection is not null and not empty. If the collection contains string elements,
    /// validates that none are null, empty, or consist only of white-space characters.
    /// Records a failure using the provided failures dictionary and exception builder if the validation does not pass.
    /// </summary>
    /// <typeparam name="TOtherClass">The type of elements contained in the collection to validate.</typeparam>
    /// <param name="list">The collection to validate for null, empty, and if containing strings, for empty or white-space-only elements.</param>
    /// <param name="name">The name of the parameter being validated. Used for failure reporting.</param>
    /// <param name="failures">The dictionary used to record validation failures.</param>
    /// <param name="builder">A function that creates an exception for a given parameter name when a validation failure occurs.</param>
    protected void AssertNotNullNotEmptyCollection<TOtherClass>(List<TOtherClass>? list, string name, FailuresDictionary failures, Func<string, Exception> builder)
    {
        if (list is null) { failures.Failure(name, builder(name)); return; }
        if (list.Count == 0) { failures.Failure(name, builder(name)); return; }
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
        if (predicate()) failures.Failure(name, builder(name));
    }
}

/// <summary>
/// Represents a list of builder instances that can be configured, validated, and used to construct objects.
/// </summary>
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
    public List<TClass> BuildSuccess() => [.. this.Select(x => x.Build().Value.Resolved())];

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

/// <summary>
/// Represents a list of object references that supports enumeration, querying, and reference-based operations.
/// </summary>
public interface IReferenceList<TClass> : IList<TClass> where TClass : class
{
    IEnumerable<TClass> AsEnumerable();
    IQueryable<TClass> Queryable { get; }
    TClass ElementAt(int index);
    bool Any(Func<TClass, bool> value);
    bool Any();
    void Add(Reference<TClass> reference);
    bool Contains(Reference<TClass> reference);
}

/// <summary>
/// Implements a list of references to objects of type <typeparamref name="TClass"/>.
/// </summary>
public class ReferenceList<TClass> : IReferenceList<TClass> where TClass : class
{
    private readonly List<Reference<TClass>> _references;

    public ReferenceList(IEnumerable<Reference<TClass>> references)
    {
        _references = references.ToList() ?? throw new ArgumentNullException(nameof(references));
    }

    public ReferenceList() { _references = []; }

    public IEnumerable<TClass> AsEnumerable()
    {
        foreach (var r in _references) if (r.IsResolved && r.Instance is not null) yield return r.Instance;
    }

    public void Add(Reference<TClass> reference) => _references.Add(reference);
    public void Add(TClass instance) => _references.Add(new Reference<TClass>().Resolve(instance));
    public bool Contains(Reference<TClass> reference) => _references.Contains(reference);

    public bool Contains(TClass instance)
    {
        foreach (var r in _references) if (r.IsResolved && r.Instance == instance) return true;
        return false;
    }

    public IQueryable<TClass> Queryable => AsEnumerable().AsQueryable();
    public TClass ElementAt(int index) => _references[index].Resolved();

    public bool Any(Func<TClass, bool> value)
    {
        foreach (var r in _references) if (r.IsResolved && r.Instance is not null && value(r.Instance)) return true;
        return false;
    }

    public bool Any()
    {
        foreach (var r in _references) if (r.IsResolved && r.Instance is not null) return true;
        return false;
    }

    public bool All(Func<TClass, bool> value)
    {
        foreach (var r in _references) if (!(r.IsResolved && r.Instance is not null && value(r.Instance))) return false;
        return true;
    }

    public IEnumerable<TClass> Where(Func<TClass, bool> predicate)
    {
        foreach (var r in _references) if (r.IsResolved && r.Instance is not null && predicate(r.Instance)) yield return r.Instance;
    }

    public IEnumerable<TOtherClass> Select<TOtherClass>(Func<TClass, TOtherClass> mapper)
    {
        foreach (var r in _references) if (r.IsResolved && r.Instance is not null) yield return mapper(r.Instance);
    }

    public int IndexOf(TClass item)
    {
        for (int i = 0; i < _references.Count; i++) { var r = _references[i]; if (r.IsResolved && r.Instance == item) return i; }
        throw new InvalidOperationException("Item not found");
    }

    public void Insert(int index, TClass item) => _references.Insert(index, new Reference<TClass>().Resolve(item));
    public void RemoveAt(int index) => _references.RemoveAt(index);
    public void Clear() => _references.Clear();

    public void CopyTo(TClass[] array, int arrayIndex)
    {
        var i = arrayIndex;
        foreach (var r in _references) if (r.IsResolved && r.Instance is not null) array[i++] = r.Instance;
    }

    public bool Remove(TClass item)
    {
        for (int i = 0; i < _references.Count; i++)
        {
            var r = _references[i];
            if (r.IsResolved && r.Instance == item) { _references.RemoveAt(i); return true; }
        }
        return false;
    }

    public IEnumerator<TClass> GetEnumerator() => AsEnumerable().GetEnumerator();
    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();

    public int Count => _references.Count;
    public bool IsReadOnly => false;

    public TClass this[int index]
    {
        get => _references[index].Resolved();
        set => _references[index] = new Reference<TClass>().Resolve(value);
    }
}