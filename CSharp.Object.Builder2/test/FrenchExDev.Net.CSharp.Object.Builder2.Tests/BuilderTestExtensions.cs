namespace FrenchExDev.Net.CSharp.Object.Builder2.Tests;

/// <summary>
/// Extension methods for backward compatibility in tests.
/// </summary>
public static class BuilderTestExtensions
{
    /// <summary>
    /// Builds and returns the resolved instance, throwing if validation fails.
    /// </summary>
    public static TClass BuildOrThrow<TClass>(this AbstractBuilder<TClass> builder) where TClass : class
    {
        var result = builder.Build();
        if (result.TryGetException<BuildFailureException>(out var ex))
        {
            var exceptions = new List<Exception>();
            if (ex!.Failures is FailuresDictionary dict)
            {
                ExtractExceptions(dict, exceptions);
            }
            throw new AggregateException(exceptions);
        }
        return result.Value.Resolved();
    }

    private static void ExtractExceptions(FailuresDictionary dict, List<Exception> exceptions)
    {
        foreach (var kvp in dict)
        {
            foreach (var failure in kvp.Value)
            {
                if (failure.TryGetException(out var e))
                {
                    exceptions.Add(e!);
                }
                else if (failure.TryGetMessage(out var m))
                {
                    exceptions.Add(new InvalidOperationException(m));
                }
                else if (failure.TryGetNested(out var nested) && nested is FailuresDictionary nestedDict)
                {
                    ExtractExceptions(nestedDict, exceptions);
                }
            }
        }
    }
}
