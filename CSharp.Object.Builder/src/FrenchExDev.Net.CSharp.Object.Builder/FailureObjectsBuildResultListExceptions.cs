namespace FrenchExDev.Net.CSharp.Object.Builder;

/// <summary>
/// Represents an exception that is thrown when one or more object builders fail, aggregating the underlying exceptions.
/// </summary>
/// <remarks>This exception provides access to all individual exceptions encountered during the build process
/// through the <see cref="Exceptions"/> property. Use this type to inspect and handle multiple failures that occur in
/// batch or composite build operations.</remarks>
public class FailureObjectsBuildResultListExceptions : Exception
{
    /// <summary>
    /// Holds the exceptions
    /// </summary>
    public IEnumerable<Exception> Exceptions { get; init; }

    /// <summary>
    /// Initializes a new instance of the FailureObjectsBuildResultListExceptions class with the specified collection of
    /// exceptions that occurred during object building.
    /// </summary>
    /// <remarks>Use the Exceptions property to access the details of each failure. This constructor is
    /// typically used when aggregating multiple build errors for reporting or diagnostic purposes.</remarks>
    /// <param name="exceptions">The collection of exceptions representing the failures encountered by one or more builders. Cannot be null.</param>
    public FailureObjectsBuildResultListExceptions(IEnumerable<Exception> exceptions)
        : base("One or more builders failed. See the 'Exceptions' property for details.")
    {
        Exceptions = exceptions;
    }
}
