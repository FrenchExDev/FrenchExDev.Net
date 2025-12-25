namespace FrenchExDev.Net.CSharp.Object.Builder2;

/// <summary>
/// List of builder instances that can be configured, validated, and built.
/// </summary>
public class BuilderList<TClass, TBuilder> : List<TBuilder>
    where TClass : class
    where TBuilder : IBuilder<TClass>, new()
{
    public ReferenceList<TClass> AsReferenceList() => new(this.Select(x => x.Reference()));

    public BuilderList<TClass, TBuilder> New(Action<TBuilder> body)
    {
        var builder = new TBuilder(); body(builder); Add(builder); return this;
    }

    public List<TClass> BuildSuccess() => [.. this.Select(x => x.Build().Value.Resolved())];

    public List<FailuresDictionary> ValidateFailures()
    {
        var visited = new VisitedObjectDictionary();
        return [.. this.Select(x => {
            var failures = new FailuresDictionary();
            x.Validate(visited, failures);
            return failures;
        }).Where(f => f.HasFailures)];
    }
}
