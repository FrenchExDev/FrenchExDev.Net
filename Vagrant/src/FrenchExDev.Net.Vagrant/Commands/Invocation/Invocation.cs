namespace FrenchExDev.Net.Vagrant.Commands.Invocation;

/// <summary>
/// Represents a concrete invocation of a CLI command (leaf) with arguments & options.
/// </summary>
public sealed class Invocation
{
    public required LeafCommandNode Command { get; init; }

    public Dictionary<string, string?> OptionValues { get; } = new(StringComparer.OrdinalIgnoreCase);
    public List<(string Name, string[] Values)> ParameterValues { get; } = new();

    /// <summary>Set an option flag (no value).</summary>
    public Invocation Flag(string optionName)
    {
        OptionValues[optionName] = null;
        return this;
    }

    /// <summary>Set an option with a single value.</summary>
    public Invocation Option(string optionName, string value)
    {
        OptionValues[optionName] = value;
        return this;
    }

    /// <summary>Add or replace a positional parameter.</summary>
    public Invocation Param(string name, params string[] values)
    {
        var idx = ParameterValues.FindIndex(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        if (idx >= 0) ParameterValues[idx] = (name, values);
        else ParameterValues.Add((name, values));
        return this;
    }
}
