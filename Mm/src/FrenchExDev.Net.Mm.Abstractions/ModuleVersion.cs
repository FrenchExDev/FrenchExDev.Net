namespace FrenchExDev.Net.Mm.Abstractions;

/// <summary>
/// Represents a specific version of a module.
/// </summary>
/// <param name="Version">The version information of the module, represented as an <see cref="IModuleVersion"/>.</param>
public record ModuleVersion(IModuleVersion Version);
