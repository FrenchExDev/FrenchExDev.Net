namespace FrenchExDev.Net.CSharp.Object.Builder2.Tests;

public class ExceptionFailureTestBuilder : AbstractBuilder<SimpleObject>
{
    protected override SimpleObject Instantiate() => new() { Value = "test" };

    protected override void ValidateInternal(VisitedObjectDictionary visitedCollector, IFailureCollector failures)
    {
        failures.AddFailure("test", Failure.FromException(new ArgumentException("Direct exception failure")));
    }
}
