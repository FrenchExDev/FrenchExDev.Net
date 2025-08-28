namespace FrenchExDev.Mm.Net.Module.Library;

/// <summary>
/// Represents basic information about a module.
/// </summary>
public interface IModuleInformation
{
    /// <summary>
    /// Gets the display name of the module.
    /// </summary>
    string DisplayName { get; }

    /// <summary>
    /// Gets the name of the module.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Gets the description of the module
    /// </summary>
    string Description { get; }
}
