using FrenchExDev.Net.CSharp.Object.Builder.Abstractions;

namespace FrenchExDev.Net.CSharp.Object.Builder;

/// <summary>
/// Provides a builder for constructing objects of type <typeparamref name="TClass"/> using a workflow of steps.
/// </summary>
/// <remarks>This builder allows the user to define a sequence of steps, each implementing <see
/// cref="IAbstractStep{TClass}"/>,  to construct an object of type <typeparamref name="TClass"/>. Steps can be
/// synchronous or asynchronous, and the  builder ensures that all steps are executed in the order they are
/// added.</remarks>
/// <typeparam name="TClass">The type of object to be built.</typeparam>
public class WorkflowObjectBuilder<TClass, TBuilder> : IAsyncObjectBuilder<TClass> where TBuilder : IAsyncObjectBuilder<TClass>
{
    /// <summary>
    /// List of steps to be executed in the workflow.
    /// </summary>
    private readonly List<IAbstractStep<TClass>> _steps = new();

    /// <summary>
    /// Holds the instance of the object being built.
    /// </summary>
    protected TClass? _instance { get; set; }

    /// <summary>
    /// Returns the built object of type <typeparamref name="TClass"/> if the build process has been completed
    /// </summary>
    public TClass Result => _instance ?? throw new InvalidOperationException("The object has not been built yet.");

    /// <summary>
    /// Adds a step to the workflow.
    /// </summary>
    /// <param name="step"></param>
    /// <returns></returns>
    public WorkflowObjectBuilder<TClass, TBuilder> Step(IAbstractStep<TClass> step)
    {
        _steps.Add(step);
        return this;
    }

    /// <summary>
    /// Execute steps in the workflow to build the object of type <typeparamref name="TClass"/>.
    /// </summary>
    /// <param name="visited"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task<IObjectBuildResult<TClass>> BuildAsync(VisitedObjectsList? visited = null, CancellationToken cancellationToken = default)
    {
        visited ??= new VisitedObjectsList();

        if (visited.TryGetValue(this, out var existing))
        {
            return new SuccessObjectBuildResult<TClass>((TClass)existing);
        }

        var intermediates = new IntermediateObjectDictionary();
        var exceptions = new ExceptionBuildDictionary();

        try
        {
            foreach (var step in _steps)
            {
                switch (step)
                {
                    case IStepObjectBuilder<TClass> syncStep:
                        syncStep.Build(exceptions, intermediates, visited);
                        break;
                    case IAsyncStepObjectBuilder<TClass> asyncStep:
                        await asyncStep.BuildAsync(exceptions, intermediates, visited, cancellationToken);
                        break;
                    default: throw new InvalidOperationException($"Unsupported step type: {step.GetType().FullName}");
                }

                if (exceptions.Count > 0)
                {
                    return new FailureAsyncObjectBuildResult<TClass, TBuilder>((TBuilder)(IAsyncObjectBuilder<TClass>)this, exceptions, visited);
                }

                if (step.HasResult())
                {
                    _instance = step.Result();
                    break;
                }
            }

            ArgumentNullException.ThrowIfNull(_instance);

            return new SuccessObjectBuildResult<TClass>(_instance);
        }
        catch (Exception ex)
        {
            exceptions.Add(nameof(_steps).ToMemberName(), new BasicAsyncObjectBuildException<TClass, IAsyncObjectBuilder<TClass>>(nameof(_steps).ToMemberName(), ex.Message, this, visited));
            return new FailureAsyncObjectBuildResult<TClass, TBuilder>((TBuilder)(IAsyncObjectBuilder<TClass>)this, exceptions, visited);
        }
    }
}
