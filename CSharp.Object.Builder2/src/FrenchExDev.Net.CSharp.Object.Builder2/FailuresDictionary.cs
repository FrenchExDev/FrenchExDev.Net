namespace FrenchExDev.Net.CSharp.Object.Builder2;

/// <summary>
/// Dictionary mapping member names to collected failures.
/// </summary>
public class FailuresDictionary : Dictionary<string, List<Failure>>, IFailureCollector
{
    public IFailureCollector AddFailure(string memberName, Failure failure)
    {
        if (!TryGetValue(memberName, out var list)) { list = []; this[memberName] = list; }
        list.Add(failure);
        return this;
    }

    public FailuresDictionary Failure(string memberName, Failure failure)
    {
        AddFailure(memberName, failure);
        return this;
    }

    public bool HasFailures => Count > 0;
    public int FailureCount => Count;
}
