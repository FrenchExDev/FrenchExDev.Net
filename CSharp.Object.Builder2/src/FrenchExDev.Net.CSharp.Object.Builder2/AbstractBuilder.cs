using FrenchExDev.Net.CSharp.Object.Result2;
using System.Runtime.CompilerServices;

namespace FrenchExDev.Net.CSharp.Object.Builder2;

/// <summary>
/// Provides an abstract base class for building and validating instances of a specified reference type, supporting
/// custom build and validation logic, instance reuse, and synchronization strategies.
/// </summary>
/// <typeparam name="TClass">The type of object to be constructed by the builder. Must be a reference type.</typeparam>
public abstract class AbstractBuilder<TClass> : IBuilder<TClass>, IExistingInstanceProvider<TClass> where TClass : class
{
    private readonly Guid _guid = Guid.NewGuid();
    private Result<TClass>? _buildResult;
    private int _buildStatus = (int)BuildStatus.NotBuilding;
    private int _validationStatus = (int)ValidationStatus.NotValidated;
    private readonly Reference<TClass> _reference;
    private TClass? _existing;
    private readonly object _buildLock = new();
    private readonly object _validateLock = new();
    private readonly ISynchronizationStrategy _syncStrategy;
    private readonly IReferenceFactory _referenceFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="AbstractBuilder{TClass}"/> class
    /// using default reference factory and lock synchronization strategy.
    /// </summary>
    protected AbstractBuilder() : this(DefaultReferenceFactory.Instance, LockSynchronizationStrategy.Instance) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="AbstractBuilder{TClass}"/> class
    /// using the specified reference factory and default lock synchronization strategy.
    /// </summary>
    /// <param name="referenceFactory">The factory used to create references for built instances.</param>
    protected AbstractBuilder(IReferenceFactory referenceFactory)
        : this(referenceFactory, LockSynchronizationStrategy.Instance) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="AbstractBuilder{TClass}"/> class
    /// using the specified reference factory and synchronization strategy.
    /// </summary>
    /// <param name="referenceFactory">The factory used to create references for built instances.</param>
    /// <param name="syncStrategy">The synchronization strategy for thread-safe operations.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="referenceFactory"/> or <paramref name="syncStrategy"/> is <c>null</c>.
    /// </exception>
    protected AbstractBuilder(IReferenceFactory referenceFactory, ISynchronizationStrategy syncStrategy)
    {
        _referenceFactory = referenceFactory ?? throw new ArgumentNullException(nameof(referenceFactory));
        _syncStrategy = syncStrategy ?? throw new ArgumentNullException(nameof(syncStrategy));
        _reference = _referenceFactory.Create<TClass>();
    }

    /// <summary>
    /// Gets the unique identifier for this builder instance.
    /// </summary>
    public Guid Id => _guid;

    /// <summary>
    /// Gets the result of the build operation.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when the build has not been completed.</exception>
    public TClass? Result
    {
        get
        {
            return _syncStrategy.Execute(_buildLock, () =>
                _buildResult is not null ? _buildResult.Value.Value : throw new InvalidOperationException());
        }
    }

    /// <summary>
    /// Gets the current build status of this builder.
    /// </summary>
    public BuildStatus BuildStatus => (BuildStatus)Volatile.Read(ref _buildStatus);

    /// <summary>
    /// Gets the current validation status of this builder.
    /// </summary>
    public ValidationStatus ValidationStatus => (ValidationStatus)Volatile.Read(ref _validationStatus);

    /// <summary>
    /// Gets the reference to the instance that will be or has been built.
    /// </summary>
    /// <returns>A <see cref="Reference{TClass}"/> that resolves to the built instance.</returns>
    public Reference<TClass> Reference() => _reference;

    /// <summary>
    /// Gets a value indicating whether an existing instance has been provided to this builder.
    /// </summary>
    public bool HasExisting => _existing is not null;

    /// <summary>
    /// Gets the existing instance if one has been provided; otherwise, <c>null</c>.
    /// </summary>
    public TClass? ExistingInstance => _existing;

    /// <summary>
    /// Sets an existing instance to be used instead of building a new one.
    /// </summary>
    /// <param name="instance">The existing instance to use.</param>
    /// <returns>The current builder instance for method chaining.</returns>
    public AbstractBuilder<TClass> Existing(TClass instance)
    {
        _existing = instance;
        return this;
    }

    /// <summary>
    /// Builds the instance, performing validation and instantiation as needed.
    /// </summary>
    /// <param name="visited">
    /// An optional dictionary tracking visited builders to detect and handle circular references.
    /// </param>
    /// <returns>
    /// A <see cref="Result{T}"/> containing a <see cref="Reference{TClass}"/> to the built instance on success,
    /// or a failure with validation errors. The reference may be unresolved if circular reference handling is in progress.
    /// </returns>
    public Result<Reference<TClass>> Build(VisitedObjectDictionary? visited = null)
    {
        // fast-path when existing instance is provided
        if (_existing is not null)
        {
            _reference.Resolve(_existing);
            _buildResult = Result<TClass>.Success(_existing);
            Interlocked.Exchange(ref _buildStatus, (int)BuildStatus.Built);
            return Result<Reference<TClass>>.Success(_reference);
        }

        // fast-path when already built
        if (BuildStatus == BuildStatus.Built)
            return Result<Reference<TClass>>.Success(_reference);

        // detect circular references - return unresolved reference to allow deferred resolution
        if (visited is not null && visited.TryGet(Id, out var v) && v is IBuilder<TClass> builder)
        {
            if (builder.ValidationStatus is ValidationStatus.NotValidated or ValidationStatus.Validating)
                return Result<Reference<TClass>>.Success(_reference); // Return unresolved reference
            if (builder.BuildStatus is BuildStatus.Built or BuildStatus.Building)
                return Result<Reference<TClass>>.Success(_reference); // Return reference (may or may not be resolved)
        }

        return _syncStrategy.Execute(_buildLock, () => BuildCore(visited));
    }

    /// <summary>
    /// Core build logic executed within the synchronization context.
    /// </summary>
    /// <param name="visited">The dictionary tracking visited builders.</param>
    /// <returns>A result containing the reference to the built instance on success, or a failure with validation errors.</returns>
    private Result<Reference<TClass>> BuildCore(VisitedObjectDictionary? visited)
    {
        if (BuildStatus == BuildStatus.Built)
            return Result<Reference<TClass>>.Success(_reference);

        Interlocked.Exchange(ref _buildStatus, (int)BuildStatus.Building);

        if (_existing is not null)
        {
            Interlocked.Exchange(ref _buildStatus, (int)BuildStatus.Built);
            _reference.Resolve(_existing);
            _buildResult = Result<TClass>.Success(_existing);
            return Result<Reference<TClass>>.Success(_reference);
        }

        visited ??= [];
        var failuresCollector = CreateFailureCollector();
        Validate(visited, failuresCollector);

        if (failuresCollector.HasFailures)
        {
            _buildResult = Result<TClass>.Failure(new BuildFailureException(failuresCollector));
            return Result<Reference<TClass>>.Failure(new BuildFailureException(failuresCollector));
        }

        BuildInternal(visited);
        var instance = Instantiate();
        _buildResult = Result<TClass>.Success(instance);
        Interlocked.Exchange(ref _buildStatus, (int)BuildStatus.Built);
        _reference.Resolve(instance);
        return Result<Reference<TClass>>.Success(_reference);
    }

    /// <summary>
    /// Creates a new failure collector for gathering validation failures.
    /// </summary>
    /// <returns>A new <see cref="FailuresDictionary"/> instance.</returns>
    protected virtual FailuresDictionary CreateFailureCollector() => new();

    /// <summary>
    /// Called during the build process to perform additional build logic before instantiation.
    /// Override this method to build child objects or perform pre-instantiation setup.
    /// </summary>
    /// <param name="visitedCollector">The dictionary tracking visited builders.</param>
    protected virtual void BuildInternal(VisitedObjectDictionary visitedCollector) { }

    /// <summary>
    /// Validates the builder state, collecting any failures into the provided collector.
    /// </summary>
    /// <param name="visitedCollector">The dictionary tracking visited builders to prevent circular validation.</param>
    /// <param name="failures">The collector for accumulating validation failures.</param>
    public void Validate(VisitedObjectDictionary visitedCollector, IFailureCollector failures)
    {
        var currentStatus = (ValidationStatus)Volatile.Read(ref _validationStatus);
        if (currentStatus is ValidationStatus.Validated or ValidationStatus.Validating)
            return;

        _syncStrategy.Execute(_validateLock, () =>
        {
            currentStatus = (ValidationStatus)Volatile.Read(ref _validationStatus);
            if (currentStatus is ValidationStatus.Validated or ValidationStatus.Validating)
                return;

            visitedCollector.MarkVisited(Id, this);
            Interlocked.Exchange(ref _validationStatus, (int)ValidationStatus.Validating);

            try { ValidateInternal(visitedCollector, failures); }
            finally { Interlocked.Exchange(ref _validationStatus, (int)ValidationStatus.Validated); }
        });
    }

    /// <summary>
    /// Called during validation to perform custom validation logic.
    /// Override this method to add validation rules for the builder.
    /// </summary>
    /// <param name="visitedCollector">The dictionary tracking visited builders.</param>
    /// <param name="failures">The collector for accumulating validation failures.</param>
    protected virtual void ValidateInternal(VisitedObjectDictionary visitedCollector, IFailureCollector failures) { }

    /// <summary>
    /// Creates and returns a new instance of <typeparamref name="TClass"/>.
    /// </summary>
    /// <returns>A new instance of the target type.</returns>
    protected abstract TClass Instantiate();

    /// <summary>
    /// Builds all builders in the specified list.
    /// </summary>
    /// <typeparam name="TBuilder">The type of builder in the list.</typeparam>
    /// <typeparam name="TModel">The type of model being built.</typeparam>
    /// <param name="builders">The list of builders to build.</param>
    /// <param name="visitedCollector">The dictionary tracking visited builders.</param>
    protected void BuildList<TBuilder, TModel>(BuilderList<TModel, TBuilder> builders, VisitedObjectDictionary visitedCollector)
        where TBuilder : IBuilder<TModel>, new() where TModel : class
    {
        foreach (var builder in builders)
            builder.Build(visitedCollector);
    }

    /// <summary>
    /// Builds a child builder and returns its reference.
    /// </summary>
    /// <typeparam name="TChild">The type of the child object being built.</typeparam>
    /// <param name="childBuilder">The child builder to build.</param>
    /// <param name="visitedCollector">The dictionary tracking visited builders.</param>
    /// <returns>A result containing the reference to the built child instance.</returns>
    protected Result<Reference<TChild>> BuildChild<TChild>(IBuilder<TChild> childBuilder, VisitedObjectDictionary visitedCollector)
        where TChild : class
    {
        return childBuilder.Build(visitedCollector);
    }

    /// <summary>
    /// Validates all builders in a list and collects their failures.
    /// </summary>
    /// <typeparam name="TOtherClass">The type of model in the list.</typeparam>
    /// <typeparam name="TOtherBuilder">The type of builder in the list.</typeparam>
    /// <param name="list">The list of builders to validate.</param>
    /// <param name="propertyName">The property name associated with this list for failure reporting.</param>
    /// <param name="visitedCollector">The dictionary tracking visited builders.</param>
    /// <param name="failures">The collector for accumulating validation failures.</param>
    protected void ValidateListInternal<TOtherClass, TOtherBuilder>(BuilderList<TOtherClass, TOtherBuilder> list, string propertyName, VisitedObjectDictionary visitedCollector, IFailureCollector failures)
        where TOtherClass : class where TOtherBuilder : class, IBuilder<TOtherClass>, new()
    {
        foreach (var item in list)
        {
            var itemFailures = CreateFailureCollector();
            item.Validate(visitedCollector, itemFailures);
            if (itemFailures.HasFailures) failures.AddFailure(propertyName, Failure.FromNested(itemFailures));
        }
    }

    /// <summary>
    /// Asserts that a string value is not empty or whitespace (null is allowed).
    /// </summary>
    /// <param name="value">The value to validate.</param>
    /// <param name="name">The property name for failure reporting.</param>
    /// <param name="failures">The collector for accumulating validation failures.</param>
    /// <param name="builder">A factory function to create the exception for the failure.</param>
    protected void AssertNotEmptyOrWhitespace(string? value, string name, IFailureCollector failures, Func<string, Exception> builder)
        => ValidationAssertions.AssertNotEmptyOrWhitespace(value, name, failures, builder);

    /// <summary>
    /// Asserts that all strings in a list are not empty or whitespace (null list is allowed).
    /// </summary>
    /// <param name="value">The list of values to validate.</param>
    /// <param name="name">The property name for failure reporting.</param>
    /// <param name="failures">The collector for accumulating validation failures.</param>
    /// <param name="builder">A factory function to create the exception for the failure.</param>
    protected void AssertNotEmptyOrWhitespace(List<string>? value, string name, IFailureCollector failures, Func<string, Exception> builder)
        => ValidationAssertions.AssertNotEmptyOrWhitespace(value, name, failures, builder);

    /// <summary>
    /// Asserts that a string value is not null, empty, or whitespace.
    /// </summary>
    /// <param name="value">The value to validate.</param>
    /// <param name="name">The property name for failure reporting.</param>
    /// <param name="failures">The collector for accumulating validation failures.</param>
    /// <param name="builder">A factory function to create the exception for the failure.</param>
    protected void AssertNotNullOrEmptyOrWhitespace(string? value, string name, IFailureCollector failures, Func<string, Exception> builder)
        => ValidationAssertions.AssertNotNullOrEmptyOrWhitespace(value, name, failures, builder);

    /// <summary>
    /// Asserts that an object value is not null.
    /// </summary>
    /// <param name="value">The value to validate.</param>
    /// <param name="name">The property name for failure reporting.</param>
    /// <param name="failures">The collector for accumulating validation failures.</param>
    /// <param name="builder">A factory function to create the exception for the failure.</param>
    protected void AssertNotNull(object? value, string name, IFailureCollector failures, Func<string, Exception> builder)
        => ValidationAssertions.AssertNotNull(value, name, failures, builder);

    /// <summary>
    /// Asserts that a collection is not null and contains at least one element.
    /// </summary>
    /// <typeparam name="TOtherClass">The type of elements in the collection.</typeparam>
    /// <param name="list">The collection to validate.</param>
    /// <param name="name">The property name for failure reporting.</param>
    /// <param name="failures">The collector for accumulating validation failures.</param>
    /// <param name="builder">A factory function to create the exception for the failure.</param>
    protected void AssertNotNullNotEmptyCollection<TOtherClass>(List<TOtherClass>? list, string name, IFailureCollector failures, Func<string, Exception> builder)
        => ValidationAssertions.AssertNotNullNotEmptyCollection(list, name, failures, builder);

    /// <summary>
    /// Asserts that a predicate evaluates to true.
    /// </summary>
    /// <param name="predicate">The predicate to evaluate.</param>
    /// <param name="name">The property name for failure reporting.</param>
    /// <param name="failures">The collector for accumulating validation failures.</param>
    /// <param name="builder">A factory function to create the exception for the failure.</param>
    protected void Assert(Func<bool> predicate, string name, IFailureCollector failures, Func<string, Exception> builder)
        => ValidationAssertions.Assert(predicate, name, failures, builder);

    /// <summary>
    /// Ensures that a required reference type property has a value, throwing if it is null.
    /// Use this method in <see cref="Instantiate"/> to safely access required properties.
    /// </summary>
    /// <typeparam name="TValue">The type of the property value.</typeparam>
    /// <param name="value">The property value to check.</param>
    /// <param name="propertyName">
    /// The name of the property. Automatically captured from the argument expression.
    /// </param>
    /// <returns>The non-null value.</returns>
    /// <exception cref="RequiredPropertyNotSetException">
    /// Thrown when <paramref name="value"/> is <c>null</c>.
    /// </exception>
    /// <example>
    /// <code>
    /// protected override Person Instantiate() => new()
    /// {
    ///     Name = Require(Name),
    ///     Address = AddressBuilder?.Reference().ResolvedOrNull()
    /// };
    /// </code>
    /// </example>
    protected TValue Require<TValue>(
        TValue? value,
        [CallerArgumentExpression(nameof(value))] string? propertyName = null) where TValue : class
    {
        return value ?? throw new RequiredPropertyNotSetException(propertyName ?? "Unknown");
    }

    /// <summary>
    /// Ensures that a required nullable value type property has a value, throwing if it is null.
    /// Use this method in <see cref="Instantiate"/> to safely access required nullable value type properties.
    /// </summary>
    /// <typeparam name="TValue">The underlying value type of the property.</typeparam>
    /// <param name="value">The nullable property value to check.</param>
    /// <param name="propertyName">
    /// The name of the property. Automatically captured from the argument expression.
    /// </param>
    /// <returns>The non-null value.</returns>
    /// <exception cref="RequiredPropertyNotSetException">
    /// Thrown when <paramref name="value"/> is <c>null</c>.
    /// </exception>
    /// <example>
    /// <code>
    /// protected override Order Instantiate() => new()
    /// {
    ///     Quantity = Require(Quantity), // Quantity is int?
    ///     Price = Require(Price)        // Price is decimal?
    /// };
    /// </code>
    /// </example>
    protected TValue Require<TValue>(
        TValue? value,
        [CallerArgumentExpression(nameof(value))] string? propertyName = null) where TValue : struct
    {
        return value ?? throw new RequiredPropertyNotSetException(propertyName ?? "Unknown");
    }
}
