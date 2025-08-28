namespace FrenchExDev.Net.CSharp.Object.Builder.Abstractions;

public class BasicBuildResult<TClass> : IBuildResult<TClass>
{
    public bool IsBuilt { get; init; }
    public TClass? Result { get; init; }
    public Exception? Exception { get; init; }

    public BasicBuildResult(bool isBuilt, TClass? result = default, Exception? exception = default)
    {
        IsBuilt = isBuilt;
        Result = result;
        Exception = exception;
    }
}