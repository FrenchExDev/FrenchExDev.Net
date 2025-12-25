namespace FrenchExDev.Net.CSharp.Object.Builder2.Tests;

public class NestedFailureTestBuilder : AbstractBuilder<SimpleObject>
{
    protected override SimpleObject Instantiate() => new() { Value = "test" };

    protected override void ValidateInternal(VisitedObjectDictionary visitedCollector, IFailureCollector failures)
    {
        var nested = new FailuresDictionary();
        nested.AddFailure("inner", Failure.FromException(new ArgumentException("Nested error")));
        failures.AddFailure("outer", Failure.FromNested(nested));
    }
}
