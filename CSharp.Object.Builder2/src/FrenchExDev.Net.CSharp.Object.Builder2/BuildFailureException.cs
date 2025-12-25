namespace FrenchExDev.Net.CSharp.Object.Builder2;

/// <summary>
/// Represents an exception that is thrown when a build process fails due to validation errors.
/// </summary>
public class BuildFailureException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BuildFailureException"/> class with the specified failures.
    /// </summary>
    /// <param name="failures">The collector containing all validation failures that caused the build to fail.</param>
    public BuildFailureException(FailuresDictionary failures) 
    { 
        Failures = failures; 
    }

    /// <summary>
    /// Gets the collection of failures that caused the build to fail.
    /// </summary>
    /// <value>An <see cref="FailuresDictionary"/> containing all recorded validation failures.</value>
    public FailuresDictionary Failures { get; }
}
