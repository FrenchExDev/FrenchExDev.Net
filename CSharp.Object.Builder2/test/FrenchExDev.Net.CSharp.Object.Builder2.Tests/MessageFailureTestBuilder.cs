namespace FrenchExDev.Net.CSharp.Object.Builder2.Tests;

public class MessageFailureTestBuilder : AbstractBuilder<SimpleObject>
{
    protected override SimpleObject Instantiate() => new() { Value = "test" };

    protected override void ValidateInternal(VisitedObjectDictionary visitedCollector, IFailureCollector failures)
    {
        failures.AddFailure("test", Failure.FromMessage("Test message failure"));
    }
}
