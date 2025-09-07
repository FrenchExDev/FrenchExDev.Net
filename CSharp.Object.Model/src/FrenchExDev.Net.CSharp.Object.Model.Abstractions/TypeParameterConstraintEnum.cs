namespace FrenchExDev.Net.CSharp.Object.Model.Abstractions;

/// <summary>
/// Specifies the possible constraints for a generic type parameter in C#.
/// Used to describe type parameter restrictions in code generation and analysis scenarios.
/// </summary>
/// <remarks>
/// Example usage:
/// <code>
/// var constraint = TypeParameterConstraintEnum.Class;
/// </code>
/// </remarks>
public enum TypeParameterConstraintEnum
{
    /// <summary>
    /// The type parameter must be a reference type (class constraint).
    /// </summary>
    Class,
    /// <summary>
    /// The type parameter must be a value type (struct constraint).
    /// </summary>
    Struct,
    /// <summary>
    /// The type parameter must have a public parameterless constructor (new() constraint).
    /// </summary>
    New,
    /// <summary>
    /// The type parameter must be non-nullable (notnull constraint).
    /// </summary>
    NotNull
}
