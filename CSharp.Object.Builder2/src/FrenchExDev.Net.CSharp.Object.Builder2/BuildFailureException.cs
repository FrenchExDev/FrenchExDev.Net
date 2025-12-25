namespace FrenchExDev.Net.CSharp.Object.Builder2;

/// <summary>
/// Exception thrown when a build process fails.
/// </summary>
public class BuildFailureException : Exception
{
    public BuildFailureException(IFailureCollector failures) { Failures = failures; }
    public IFailureCollector Failures { get; }
}
