using FrenchExDev.Net.CSharp.Object.Result2;

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

    protected AbstractBuilder() : this(DefaultReferenceFactory.Instance, LockSynchronizationStrategy.Instance) { }

    protected AbstractBuilder(IReferenceFactory referenceFactory) 
        : this(referenceFactory, LockSynchronizationStrategy.Instance) { }

    protected AbstractBuilder(IReferenceFactory referenceFactory, ISynchronizationStrategy syncStrategy)
    {
        _referenceFactory = referenceFactory ?? throw new ArgumentNullException(nameof(referenceFactory));
        _syncStrategy = syncStrategy ?? throw new ArgumentNullException(nameof(syncStrategy));
        _reference = _referenceFactory.Create<TClass>();
    }

    public Guid Id => _guid;

    public TClass? Result
    {
        get
        {
            return _syncStrategy.Execute(_buildLock, () =>
                _buildResult is not null ? _buildResult.Value.Value : throw new InvalidOperationException());
        }
    }

    public BuildStatus BuildStatus => (BuildStatus)Volatile.Read(ref _buildStatus);
    public ValidationStatus ValidationStatus => (ValidationStatus)Volatile.Read(ref _validationStatus);
    public Reference<TClass> Reference() => _reference;
    public bool HasExisting => _existing is not null;
    public TClass? ExistingInstance => _existing;

    public AbstractBuilder<TClass> Existing(TClass instance)
    {
        _existing = instance;
        return this;
    }

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

    protected void BuildList<TBuilder, TModel>(BuilderList<TModel, TBuilder> builders, VisitedObjectDictionary visitedCollector)
        where TBuilder : IBuilder<TModel>, new() where TModel : class
    {
        foreach (var builder in builders)
            builder.Build(visitedCollector);
    }

    protected Result<Reference<TChild>> BuildChild<TChild>(IBuilder<TChild> childBuilder, VisitedObjectDictionary visitedCollector)
        where TChild : class
    {
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
