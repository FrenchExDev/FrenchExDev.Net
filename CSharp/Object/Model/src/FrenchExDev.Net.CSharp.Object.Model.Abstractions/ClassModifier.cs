namespace FrenchExDev.Net.CSharp.Object.Model.Abstractions;

/// <summary>
/// Specifies the possible modifiers for a C# class declaration.
/// These modifiers control the accessibility, inheritance, and behavior of the class.
/// </summary>
public enum ClassModifier
{
    /// <summary>
    /// The class is accessible from any other code.
    /// </summary>
    Public,

    /// <summary>
    /// The class is accessible only within its containing type.
    /// </summary>
    Private,

    /// <summary>
    /// The class is accessible only within its containing type or derived types.
    /// </summary>
    Protected,

    /// <summary>
    /// The class is accessible only within its containing assembly.
    /// </summary>
    Internal,

    /// <summary>
    /// The class is accessible within its containing assembly or derived types.
    /// </summary>
    ProtectedInternal,

    /// <summary>
    /// The class is accessible only within its containing type or derived types in the same assembly.
    /// </summary>
    PrivateProtected,

    /// <summary>
    /// The class is static and cannot be instantiated or inherited.
    /// </summary>
    Static,

    /// <summary>
    /// The class is sealed and cannot be inherited.
    /// </summary>
    Sealed,

    /// <summary>
    /// The class is abstract and cannot be instantiated directly.
    /// </summary>
    Abstract,

    /// <summary>
    /// The class is partial and its definition may be split across multiple files.
    /// </summary>
    Partial,

    /// <summary>
    /// The class is unsafe and may contain unsafe code blocks.
    /// </summary>
    Unsafe,

    /// <summary>
    /// The class hides an inherited member with the same name.
    /// </summary>
    New,
}
