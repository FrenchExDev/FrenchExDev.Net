namespace FrenchExDev.Net.CSharp.Object.Builder2;

/// <summary>
/// Defines a contract for collecting validation/build failures.
/// </summary>
public interface IFailureCollector
{
    IFailureCollector AddFailure(string memberName, Failure failure);
    bool HasFailures { get; }
    int FailureCount { get; }
}
