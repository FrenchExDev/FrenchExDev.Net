namespace FrenchExDev.Net.CSharp.Object.Builder2;

/// <summary>
/// Exception thrown when attempting to access a <see cref="Reference{TClass}"/> that has not been resolved.
/// </summary>
/// <remarks>
/// <para>
/// This exception is thrown by <see cref="Reference{TClass}.Resolved()"/> when the reference
/// does not yet have an associated instance. This typically occurs when:
/// </para>
/// <list type="bullet">
///   <item><description>Accessing a reference before the builder has been built</description></item>
///   <item><description>The build failed and the reference was never resolved</description></item>
///   <item><description>Accessing a reference in a circular dependency before the parent is built</description></item>
/// </list>
/// <para>
/// To avoid this exception, check <see cref="Reference{TClass}.IsResolved"/> before accessing,
/// or use <see cref="Reference{TClass}.ResolvedOrNull()"/> for nullable access.
/// </para>
/// </remarks>
/// <example>
/// <code>
/// var reference = new Reference&lt;Person&gt;();
/// 
/// // Safe access patterns
/// if (reference.IsResolved)
/// {
///     var person = reference.Resolved();
/// }
/// 
/// var personOrNull = reference.ResolvedOrNull();
/// </code>
/// </example>
/// <seealso cref="Reference{TClass}"/>
public class ReferenceNotResolvedException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ReferenceNotResolvedException"/> class.
    /// </summary>
    public ReferenceNotResolvedException() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="ReferenceNotResolvedException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public ReferenceNotResolvedException(string? message) : base(message) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="ReferenceNotResolvedException"/> class with a specified error message
    /// and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public ReferenceNotResolvedException(string? message, Exception? innerException) : base(message, innerException) { }
}
