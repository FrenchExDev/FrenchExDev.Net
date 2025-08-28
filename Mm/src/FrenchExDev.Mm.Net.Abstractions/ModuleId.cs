namespace FrenchExDev.Mm.Net.Abstractions;

/// <summary>
/// Represents a unique identifier for a module.
/// </summary>
/// <remarks>This type is a record that encapsulates a <see cref="Guid"/> value, ensuring a strongly-typed
/// identifier for modules. It is primarily used to uniquely identify modules in a system or application.</remarks>
/// <param name="Id"></param>
public record ModuleId(Guid Id);
