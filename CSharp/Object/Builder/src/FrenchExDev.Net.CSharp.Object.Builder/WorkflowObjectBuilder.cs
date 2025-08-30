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
public class WorkflowObjectBuilder<TClass> : IAsyncObjectBuilder<TClass>
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
    public WorkflowObjectBuilder<TClass> Step(IAbstractStep<TClass> step)
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
        var intermediates = new IntermediateObjectsList();
        visited ??= new VisitedObjectsList();

        foreach (var step in _steps)
        {
            switch (step)
            {
                case IStepObjectBuilder<TClass> syncStep:
                    syncStep.Build(intermediates, visited);
                    break;
                case IAsyncStepObjectBuilder<TClass> asyncStep:
                    await asyncStep.BuildAsync(intermediates, visited, cancellationToken);
                    break;
                default: throw new InvalidOperationException($"Unsupported step type: {step.GetType().FullName}");
            }

            if (step.IsFinalStep)
            {
                _instance = await step.Result;
                break;
            }
        }

        ArgumentNullException.ThrowIfNull(_instance);

        return new SuccessObjectBuildResult<TClass>(_instance);
    }
}
