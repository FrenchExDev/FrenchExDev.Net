namespace FrenchExDev.Net.CSharp.Object.Builder2;

/// <summary>
/// Exception thrown when accessing an unresolved reference.
/// </summary>
public class ReferenceNotResolvedException : Exception
{
    public ReferenceNotResolvedException() { }
    public ReferenceNotResolvedException(string? message) : base(message) { }
    public ReferenceNotResolvedException(string? message, Exception? innerException) : base(message, innerException) { }
}
