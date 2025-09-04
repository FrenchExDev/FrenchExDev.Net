namespace FrenchExDev.Net.CSharp.Object.Model.Abstractions;

/// <summary>
/// Specifies the possible modifiers for a struct declaration in C#.
/// Used to describe the accessibility, behavior, and characteristics of a struct in code generation and analysis scenarios.
/// </summary>
/// <remarks>
/// Example usage:
/// <code>
/// var modifiers = new List&lt;StructModifier&gt; { StructModifier.Public, StructModifier.ReadOnly };
/// </code>
/// </remarks>
public enum StructModifier
{
    /// <summary>
    /// The struct is public and accessible from any code.
    /// </summary>
    Public,
    /// <summary>
    /// The struct is private and accessible only within its containing type.
    /// </summary>
    Private,
    /// <summary>
    /// The struct is protected and accessible only within its containing type or derived types.
    /// </summary>
    Protected,
    /// <summary>
    /// The struct is internal and accessible only within its own assembly.
    /// </summary>
    Internal,
    /// <summary>
    /// The struct is protected internal and accessible within its own assembly or from derived types.
    /// </summary>
    ProtectedInternal,
    /// <summary>
    /// The struct is private protected and accessible only within its containing class or derived types in the same assembly.
    /// </summary>
    PrivateProtected,
    /// <summary>
    /// The struct is static and cannot be instantiated or inherited.
    /// </summary>
    Static,
    /// <summary>
    /// The struct is readonly and its fields cannot be modified after construction.
    /// </summary>
    ReadOnly,
    /// <summary>
    /// The struct is const and its value is constant at compile time (not valid for C# structs, but may be used for code modeling).
    /// </summary>
    Const,
    /// <summary>
    /// The struct is abstract and cannot be instantiated directly (not valid for C# structs, but may be used for code modeling).
    /// </summary>
    Abstract,
    /// <summary>
    /// The struct is sealed and cannot be inherited (not valid for C# structs, but may be used for code modeling).
    /// </summary>
    Sealed,
    /// <summary>
    /// The struct is virtual and can be overridden in a derived type (not valid for C# structs, but may be used for code modeling).
    /// </summary>
    Virtual,
    /// <summary>
    /// The struct overrides a base member (not valid for C# structs, but may be used for code modeling).
    /// </summary>
    Override,
    /// <summary>
    /// The struct is async and supports asynchronous operations (not valid for C# structs, but may be used for code modeling).
    /// </summary>
    Async,
    /// <summary>
    /// The struct is extern and implemented externally (not valid for C# structs, but may be used for code modeling).
    /// </summary>
    Extern,
    /// <summary>
    /// The struct is new and hides a member inherited from a base type.
    /// </summary>
    New,
}
