using FrenchExDev.Net.CSharp.Object.Result2;

namespace FrenchExDev.Net.CSharp.Object.Builder2;

/// <summary>
/// Default build orchestrator that handles the complete build lifecycle.
/// Follows Single Responsibility Principle - only handles orchestration.
/// </summary>
/// <typeparam name="TClass">The type of object being built.</typeparam>
public class DefaultBuildOrchestrator<TClass> : IBuildOrchestrator<TClass> where TClass : class
{
    private readonly ISynchronizationStrategy _syncStrategy;
    private readonly Func<IFailureCollector> _failureCollectorFactory;

    public DefaultBuildOrchestrator()
        : this(LockSynchronizationStrategy.Instance, () => new FailuresDictionary())
    {
    }

    public DefaultBuildOrchestrator(
        ISynchronizationStrategy syncStrategy,
        Func<IFailureCollector> failureCollectorFactory)
    {
        _syncStrategy = syncStrategy ?? throw new ArgumentNullException(nameof(syncStrategy));
        _failureCollectorFactory = failureCollectorFactory ?? throw new ArgumentNullException(nameof(failureCollectorFactory));
    }

    public Result<Reference<TClass>> Build(IBuilder<TClass> builder, IVisitedTracker? visited = null)
    {
        ArgumentNullException.ThrowIfNull(builder);

        // Check for existing instance first (fast path)
        if (builder is IExistingInstanceProvider<TClass> existingProvider && existingProvider.HasExisting)
        {
            return Result<Reference<TClass>>.Success(builder.Reference().Resolve(existingProvider.ExistingInstance!));
        }

        // Check if already built
        if (builder.BuildStatus == BuildStatus.Built)
        {
            return Result<Reference<TClass>>.Success(builder.Reference());
        }

        // Check visited for cycle detection
        if (visited is not null && visited.TryGet(builder.Id, out var v) && v is IBuilder<TClass> visitedBuilder)
        {
            if (visitedBuilder.ValidationStatus is ValidationStatus.NotValidated or ValidationStatus.Validating)
                return Result<Reference<TClass>>.Success(builder.Reference());
            if (visitedBuilder.BuildStatus is BuildStatus.Built or BuildStatus.Building)
                return Result<Reference<TClass>>.Success(builder.Reference());
        }

        // Delegate actual build to the builder's internal method
        // The builder handles its own synchronization
        return builder.Build(visited as VisitedObjectDictionary);
    }
}

/// <summary>
/// Marker interface for builders that can provide an existing instance.
/// </summary>
/// <typeparam name="TClass">The type of the existing instance.</typeparam>
public interface IExistingInstanceProvider<TClass> where TClass : class
{
    /// <summary>
    /// Gets whether an existing instance has been set.
    /// </summary>
    bool HasExisting { get; }

    /// <summary>
    /// Gets the existing instance, if set.
    /// </summary>
    TClass? ExistingInstance { get; }
}
