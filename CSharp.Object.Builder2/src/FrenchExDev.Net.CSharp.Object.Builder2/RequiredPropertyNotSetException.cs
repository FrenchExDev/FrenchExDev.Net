namespace FrenchExDev.Net.CSharp.Object.Builder2;

/// <summary>
/// The exception that is thrown when a required property was not set before instantiation.
/// </summary>
/// <remarks>
/// This exception indicates that a builder property that is required for instantiation
/// was not configured before <see cref="AbstractBuilder{TClass}.Build"/> was called.
/// Use the <see cref="AbstractBuilder{TClass}.Require{TValue}(TValue?, string?)"/> helper
/// method in your <c>Instantiate()</c> implementation to throw this exception automatically.
/// </remarks>
public class RequiredPropertyNotSetException : InvalidOperationException
{
    /// <summary>
    /// Gets the name of the property that was not set.
    /// </summary>
    public string PropertyName { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="RequiredPropertyNotSetException"/> class
    /// with the specified property name.
    /// </summary>
    /// <param name="propertyName">The name of the property that was not set.</param>
    public RequiredPropertyNotSetException(string propertyName)
        : base($"Required property '{propertyName}' was not set before instantiation.")
    {
        PropertyName = propertyName;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RequiredPropertyNotSetException"/> class
    /// with the specified property name and inner exception.
    /// </summary>
    /// <param name="propertyName">The name of the property that was not set.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public RequiredPropertyNotSetException(string propertyName, Exception? innerException)
        : base($"Required property '{propertyName}' was not set before instantiation.", innerException)
    {
        PropertyName = propertyName;
    }
}
