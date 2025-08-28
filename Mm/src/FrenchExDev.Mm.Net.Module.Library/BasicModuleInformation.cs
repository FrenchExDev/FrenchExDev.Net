namespace FrenchExDev.Mm.Net.Module.Library;

/// <summary>
/// Represents basic information about a module.
/// </summary>
/// <param name="name"></param>
/// <param name="displayName"></param>
/// <param name="description"></param>
public class BasicModuleInformation(string name, string displayName, string description) : IModuleInformation
{
    /// <summary>
    /// Read-only property for the display name of the module.
    /// </summary>
    public string DisplayName => displayName;

    /// <summary>
    /// Read-only property for the name of the module.
    /// </summary>
    public string Name => name;

    /// <summary>
    /// Read-only property for the description of the module.
    /// </summary>
    public string Description => description;
}