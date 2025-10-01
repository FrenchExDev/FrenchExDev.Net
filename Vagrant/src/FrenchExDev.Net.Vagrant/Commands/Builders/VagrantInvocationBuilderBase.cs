using FrenchExDev.Net.CSharp.Object.Builder2;
using FrenchExDev.Net.Vagrant.Commands.Invocation;
using InvocationModel = FrenchExDev.Net.Vagrant.Commands.Invocation.Invocation;

namespace FrenchExDev.Net.Vagrant.Commands.Builders;

/// <summary>
/// Base generic builder for a concrete vagrant <see cref="Invocation"/> using the AbstractBuilder pattern.
/// Provides fluent helpers for setting options, flags and parameters with validation at build time
/// against the underlying <see cref="LeafCommandNode"/> metadata.
/// </summary>
public abstract class VagrantInvocationBuilderBase<TBuilder> : AbstractBuilder<InvocationModel>
    where TBuilder : VagrantInvocationBuilderBase<TBuilder>
{
    protected LeafCommandNode? _command;
    protected readonly Dictionary<string,string?> _options = new(StringComparer.OrdinalIgnoreCase);
    protected readonly Dictionary<string, string[]> _parameters = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>Assign the command leaf.</summary>
    public TBuilder Command(LeafCommandNode leaf) { _command = leaf; return (TBuilder)this; }

    /// <summary>Adds/replace a flag option (no value).</summary>
    public TBuilder Flag(string name) { _options[name] = null; return (TBuilder)this; }

    /// <summary>Adds/replace an option with a value.</summary>
    public TBuilder Option(string name, string value) { _options[name] = value; return (TBuilder)this; }

    /// <summary>Sets a positional parameter values (replaces existing).</summary>
    public TBuilder Param(string name, params string[] values) { _parameters[name] = values; return (TBuilder)this; }

    protected override InvocationModel Instantiate()
    {
        if (_command is null) throw new InvalidDataException("Command not set");
        var inv = new InvocationModel { Command = _command };
        foreach (var kv in _options) inv.OptionValues[kv.Key] = kv.Value; 
        foreach (var kv in _parameters) inv.ParameterValues.Add((kv.Key, kv.Value));
        return inv;
    }

    protected new void ValidateInternal(VisitedObjectDictionary visited, FailuresDictionary failures)
    {
        if (_command is null) { failures.Failure("Command", new InvalidDataException("Command not specified")); return; }

        // Validate options exist
        var availableOpts = _command.GetEffectiveOptions().Select(o => o.Name).ToHashSet(StringComparer.OrdinalIgnoreCase);
        foreach (var opt in _options.Keys)
            if (!availableOpts.Contains(opt)) failures.Failure("Options", new InvalidDataException($"Unknown option '{opt}'"));

        // Validate required parameters
        foreach (var p in _command.Parameters)
        {
            if (!p.Required) continue;
            if (!_parameters.TryGetValue(p.Name, out var vals) || vals.Length == 0)
                failures.Failure("Parameters", new InvalidDataException($"Missing required parameter '{p.Name}'"));
        }

        // Validate no extra unexpected parameters
        var declaredParams = _command.Parameters.Select(p => p.Name).ToHashSet(StringComparer.OrdinalIgnoreCase);
        foreach (var pname in _parameters.Keys)
            if (!declaredParams.Contains(pname)) failures.Failure("Parameters", new InvalidDataException($"Unknown parameter '{pname}'"));
    }
}
