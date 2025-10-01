namespace FrenchExDev.Net.Vagrant.Commands;

/// <summary>
/// Represents a positional parameter (argument) for a command.
/// </summary>
/// <param name="Name">Parameter name placeholder (e.g. "source", "name").</param>
/// <param name="Description">Description for help output.</param>
/// <param name="Required">True if the parameter is mandatory.</param>
/// <param name="IsVariadic">True if it captures remaining arguments (e.g. -- extra args).</param>
public sealed record CommandParameter(
    string Name,
    string? Description = null,
    bool Required = true,
    bool IsVariadic = false)
{
    public override string ToString() => Required ? Name : $"[{Name}]";
}
