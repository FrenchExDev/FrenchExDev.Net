namespace FrenchExDev.Net.Vagrant.Commands;

/// <summary>
/// Represents a command option (flag or switch) accepted by a Vagrant command.
/// </summary>
/// <param name="Name">Canonical long name (without leading dashes), e.g. "machine-readable"</param>
/// <param name="Description">Human friendly description.</param>
/// <param name="HasValue">True if the option expects a value (e.g. --provider virtualbox).</param>
/// <param name="ValueOptional">True if the value is optional when <see cref="HasValue"/> is true.</param>
/// <param name="ValueName">Name used in help output for the value placeholder.</param>
/// <param name="Aliases">Other names (short or long) for the option, without leading dashes (e.g. "h" for -h).</param>
public sealed record CommandOption(
    string Name,
    string? Description = null,
    bool HasValue = false,
    bool ValueOptional = false,
    string? ValueName = null,
    IReadOnlyCollection<string>? Aliases = null)
{
    public IEnumerable<string> AllNames => Aliases is null ? [Name] : Aliases.Prepend(Name);
}
