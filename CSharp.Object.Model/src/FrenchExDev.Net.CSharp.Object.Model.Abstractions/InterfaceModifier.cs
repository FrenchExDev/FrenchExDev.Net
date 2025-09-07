namespace FrenchExDev.Net.CSharp.Object.Model.Abstractions;

/// <summary>
/// Specifies the possible access modifiers for an interface declaration.
/// </summary>
public enum InterfaceModifier
{
    /// <summary>
    /// The interface is public and accessible from any code.
    /// </summary>
    Public,
    /// <summary>
    /// The interface is internal and accessible only within its own assembly.
    /// </summary>
    Internal,
    /// <summary>
    /// The interface is protected internal and accessible within its own assembly or from derived types.
    /// </summary>
    ProtectedInternal,
    /// <summary>
    /// The interface is private protected and accessible only within its containing class or derived types in the same assembly.
    /// </summary>
    PrivateProtected,
}
