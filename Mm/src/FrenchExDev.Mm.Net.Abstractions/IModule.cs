namespace FrenchExDev.Mm.Net.Abstractions;

/// <summary>
/// Represents a module with a unique identifier and a name.
/// </summary>
/// <remarks>This interface defines the basic structure for a module, including its unique identifier and display
/// name. Implementations of this interface can provide additional functionality specific to the module's
/// purpose.</remarks>
public interface IModule
{
    /// <summary>
    /// Holds the unique identifier for the module.
    /// </summary>
    ModuleId Id { get; }

    /// <summary>
    /// Gets the version information for the module.
    /// </summary>
    IModuleVersion Version { get; }

    /// <summary>
    /// Gets the module information associated with this instance.
    /// </summary>
    IModuleInformation Information { get; }
}
