using FrenchExDev.Net.CSharp.Object.Builder.Abstractions;

namespace FrenchExDev.Net.CSharp.Object.Builder;

public class LambdaStepObjectBuilder<TClass> : IStepObjectBuilder<TClass>
{
    private readonly Action<LambdaStepObjectBuilder<TClass>, IntermediateObjectsList, VisitedObjectsList> _buildAction;
    public LambdaStepObjectBuilder(bool isFinal, Action<LambdaStepObjectBuilder<TClass>, IntermediateObjectsList, VisitedObjectsList> buildAction)
    {
        IsFinalStep = isFinal;
        _buildAction = buildAction;
    }

    public bool IsFinalStep { get; set; }

    private TClass? _result;

    public void Set(TClass result)
    {
        _result = result;
    }

    public virtual Task<TClass> Result => _result is not null ? Task.FromResult(_result) : throw new InvalidOperationException("Result not available");

    public void Build(IntermediateObjectsList intermediate, VisitedObjectsList visited)
    {
        _buildAction(this, intermediate, visited);
    }
}
