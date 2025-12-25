using FrenchExDev.Net.CSharp.Object.Result2;

namespace FrenchExDev.Net.CSharp.Object.Builder2;

/// <summary>
/// Provides an abstract base class for building and validating instances of a specified reference type, supporting
/// custom build and validation logic, instance reuse, and synchronization strategies.
/// </summary>
/// <remarks>This class implements common builder patterns, including support for existing instances, build status
/// tracking, validation, and thread-safe operations via a synchronization strategy. Derived classes should implement
/// the abstract methods to define specific instantiation and validation logic. The builder can be used to construct new
/// instances or reuse existing ones, and provides mechanisms to track build and validation progress. Thread safety is
/// ensured for build and validation operations. For advanced scenarios, custom reference factories and synchronization
/// strategies can be supplied via the constructor.</remarks>
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
    private readonly IBuildOrchestrator? _orchestrator;

    protected AbstractBuilder() : this(DefaultReferenceFactory.Instance, LockSynchronizationStrategy.Instance, null) { }

    protected AbstractBuilder(IReferenceFactory referenceFactory) 
        : this(referenceFactory, LockSynchronizationStrategy.Instance, null) { }

    protected AbstractBuilder(IReferenceFactory referenceFactory, ISynchronizationStrategy syncStrategy)
        : this(referenceFactory, syncStrategy, null) { }

    protected AbstractBuilder(IBuildOrchestrator orchestrator)
        : this(DefaultReferenceFactory.Instance, LockSynchronizationStrategy.Instance, orchestrator) { }

    protected AbstractBuilder(IReferenceFactory referenceFactory, ISynchronizationStrategy syncStrategy, IBuildOrchestrator? orchestrator)
    {
        _referenceFactory = referenceFactory ?? throw new ArgumentNullException(nameof(referenceFactory));
        _syncStrategy = syncStrategy ?? throw new ArgumentNullException(nameof(syncStrategy));
        _orchestrator = orchestrator;
        _reference = _referenceFactory.Create<TClass>();
    }

    /// <summary>
    /// Gets the unique identifier for this instance.
    /// </summary>
    public Guid Id => _guid;

    /// <summary>
    /// Gets the build orchestrator used by this builder, if any.
    /// </summary>
    protected IBuildOrchestrator? Orchestrator => _orchestrator;

    /// <summary>
    /// Gets the result of the build operation if it has completed successfully.
    /// </summary>
    /// <remarks>Accessing this property before the build operation has completed will throw an exception.
    /// This property is thread-safe and may block if the result is not yet available.</remarks>
    public TClass? Result
    {
        get
        {
            return _syncStrategy.Execute(_buildLock, () =>
                _buildResult is not null ? _buildResult.Value.Value : throw new InvalidOperationException());
        }
    }

    /// <summary>
    /// Gets the current status of the build process.
    /// </summary>
    /// <remarks>The returned value reflects the most recent build status in a thread-safe manner. This
    /// property can be accessed from multiple threads without additional synchronization.</remarks>
    public BuildStatus BuildStatus => (BuildStatus)Volatile.Read(ref _buildStatus);

    /// <summary>
    /// Gets the current validation status of the object.
    /// </summary>
    public ValidationStatus ValidationStatus => (ValidationStatus)Volatile.Read(ref _validationStatus);

    /// <summary>
    /// Returns a reference wrapper for the current instance of type <typeparamref name="TClass"/>.
    /// </summary>
    /// <returns>A <see cref="Reference{TClass}"/> representing a reference to the current object.</returns>
    public Reference<TClass> Reference() => _reference;
    
    /// <summary>
    /// Returns whether an existing instance has been provided to the builder.
    /// </summary>
    public bool HasExisting => _existing is not null;
    
    /// <summary>
    /// Gets the existing instance of type <typeparamref name="TClass"/>, if one is available.
    /// </summary>
    /// <remarks>Returns the previously created instance if it exists; otherwise, returns <c>null</c>. This
    /// property does not create a new instance.</remarks>
    public TClass? ExistingInstance => _existing;

    public AbstractBuilder<TClass> Existing(TClass instance)
    {
        _existing = instance;
        return this;
    }

    /// <summary>
    /// Builds the reference to the target object, resolving dependencies and ensuring the object is constructed if
    /// necessary.
    /// </summary>
    /// <remarks>If the object has already been built or resolved, this method returns the existing reference
    /// without performing additional work. The method is thread-safe and supports incremental or cyclic builds by using
    /// the provided visited dictionary.</remarks>
    /// <param name="visited">An optional dictionary used to track objects that have already been visited during the build process. This helps
    /// prevent redundant construction and supports handling of cyclic references.</param>
    /// <returns>A result containing the reference to the built object. If the object has already been built or resolved, the
    /// result contains the existing reference.</returns>
    public Result<Reference<TClass>> Build(VisitedObjectDictionary? visited = null)
    {
        if (_existing is not null)
            return Result<Reference<TClass>>.Success(_reference.Resolve(_existing));

        if (BuildStatus == BuildStatus.Built)
            return Result<Reference<TClass>>.Success(_reference);

        if (visited is not null && visited.TryGet(Id, out var v) && v is IBuilder<TClass> builder)
        {
            if (builder.ValidationStatus is ValidationStatus.NotValidated or ValidationStatus.Validating)
                return Result<Reference<TClass>>.Success(_reference);
            if (builder.BuildStatus is BuildStatus.Built or BuildStatus.Building)
                return Result<Reference<TClass>>.Success(_reference);
        }

        return _syncStrategy.Execute(_buildLock, () => BuildCore(visited));
    }

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
            return Result<Reference<TClass>>.Success(_reference);
        }

        BuildInternal(visited);
        var instance = Instantiate();
        _buildResult = Result<TClass>.Success(instance);
        Interlocked.Exchange(ref _buildStatus, (int)BuildStatus.Built);
        _reference.Resolve(instance);
        return Result<Reference<TClass>>.Success(_reference);
    }

    protected virtual FailuresDictionary CreateFailureCollector() => new();

    public TClass BuildOrThrow(VisitedObjectDictionary? visitedCollector = null)
    {
        Build(visitedCollector);

        return _syncStrategy.Execute(_buildLock, () =>
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
        });
    }

    private static List<Exception> ExtractExceptionsFromFailures(IFailureCollector failures)
    {
        var result = new List<Exception>();
        if (failures is FailuresDictionary dict)
        {
            foreach (var kvp in dict)
            {
                foreach (var failure in kvp.Value)
                {
                    failure.Match(
                        onException: ex => result.Add(ex),
                        onMessage: msg => result.Add(new InvalidOperationException(msg)),
                        onNested: nested => result.AddRange(ExtractExceptionsFromFailures(nested))
                    );
                }
            }
        }
        return result;
    }

    protected virtual void BuildInternal(VisitedObjectDictionary visitedCollector) { }

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

    protected virtual void ValidateInternal(VisitedObjectDictionary visitedCollector, IFailureCollector failures) { }
    protected abstract TClass Instantiate();

    /// <summary>
    /// Builds a list of builders using the orchestrator if available, otherwise builds directly.
    /// </summary>
    protected void BuildList<TBuilder, TModel>(BuilderList<TModel, TBuilder> builders, VisitedObjectDictionary visitedCollector)
        where TBuilder : IBuilder<TModel>, new() where TModel : class
    {
        foreach (var builder in builders)
        {
            if (_orchestrator is not null)
                _orchestrator.Build(builder, visitedCollector);
            else
                builder.Build(visitedCollector);
        }
    }

    /// <summary>
    /// Builds a single child builder using the orchestrator if available, otherwise builds directly.
    /// </summary>
    protected Result<Reference<TChild>> BuildChild<TChild>(IBuilder<TChild> childBuilder, VisitedObjectDictionary visitedCollector)
        where TChild : class
    {
        if (_orchestrator is not null)
            return _orchestrator.Build(childBuilder, visitedCollector);
        return childBuilder.Build(visitedCollector);
    }

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

    protected void AssertNotEmptyOrWhitespace(string? value, string name, IFailureCollector failures, Func<string, Exception> builder)
        => ValidationAssertions.AssertNotEmptyOrWhitespace(value, name, failures, builder);
    protected void AssertNotEmptyOrWhitespace(List<string>? value, string name, IFailureCollector failures, Func<string, Exception> builder)
        => ValidationAssertions.AssertNotEmptyOrWhitespace(value, name, failures, builder);
    protected void AssertNotNullOrEmptyOrWhitespace(string? value, string name, IFailureCollector failures, Func<string, Exception> builder)
        => ValidationAssertions.AssertNotNullOrEmptyOrWhitespace(value, name, failures, builder);
    protected void AssertNotNull(object? value, string name, IFailureCollector failures, Func<string, Exception> builder)
        => ValidationAssertions.AssertNotNull(value, name, failures, builder);
    protected void AssertNotNullNotEmptyCollection<TOtherClass>(List<TOtherClass>? list, string name, IFailureCollector failures, Func<string, Exception> builder)
        => ValidationAssertions.AssertNotNullNotEmptyCollection(list, name, failures, builder);
    protected void Assert(Func<bool> predicate, string name, IFailureCollector failures, Func<string, Exception> builder)
        => ValidationAssertions.Assert(predicate, name, failures, builder);
}
