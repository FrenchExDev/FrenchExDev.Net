namespace FrenchExDev.Net.CSharp.Object.Builder2;

/// <summary>
/// Specifies the current status of a validation operation.
/// </summary>
/// <remarks>
/// <para>
/// The validation status follows a linear progression:
/// <c>NotValidated</c> → <c>Validating</c> → <c>Validated</c>
/// </para>
/// <para>
/// Once validated, a builder will not be validated again during the same build operation,
/// even if validation is explicitly requested. This prevents infinite loops in circular references.
/// </para>
/// </remarks>
/// <seealso cref="IValidatable"/>
/// <seealso cref="BuildStatus"/>
public enum ValidationStatus 
{ 
    /// <summary>
    /// Validation has not started. This is the initial state of a builder.
    /// </summary>
    NotValidated, 

    /// <summary>
    /// Validation is currently in progress. This state is used to detect circular validation.
    /// </summary>
    Validating, 

    /// <summary>
    /// Validation has completed. The builder has been validated (regardless of whether failures were found).
    /// </summary>
    Validated 
}
