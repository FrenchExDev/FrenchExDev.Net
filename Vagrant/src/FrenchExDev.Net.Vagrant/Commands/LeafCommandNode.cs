using System.Threading.Tasks;
using System.Linq;

namespace FrenchExDev.Net.Vagrant.Commands;

/// <summary>
/// A command that executes an action (leaf, like `vagrant init`).
/// </summary>
public sealed class LeafCommandNode : CommandNodeBase
{
    public LeafCommandNode(string name, string? description = null, ICommandGroupNode? parent = null)
        : base(name, description, parent)
    {
    }

    /// <summary>Optional handler invoked to execute this command abstraction (returns exit code).</summary>
    public Func<LeafCommandNode, Task<int>>? Handler { get; private set; }

    /// <summary>Optional builder that produces a command line template string (usage) for this node.</summary>
    public Func<LeafCommandNode, string>? CommandLineBuilder { get; private set; }

    public LeafCommandNode WithOption(CommandOption option)
    {
        AddOption(option);
        return this;
    }

    public LeafCommandNode WithParameter(CommandParameter parameter)
    {
        AddParameter(parameter);
        return this;
    }

    public LeafCommandNode WithHandler(Func<LeafCommandNode, Task<int>> handler)
    {
        Handler = handler;
        return this;
    }

    public LeafCommandNode ToCommandLine(Func<LeafCommandNode, string> builder)
    {
        CommandLineBuilder = builder;
        return this;
    }

    /// <summary>Invoke the handler if present else throw.</summary>
    public Task<int> InvokeAsync() => Handler is null
        ? Task.FromException<int>(new InvalidOperationException($"No handler assigned for command '{Name}'."))
        : Handler(this);

    /// <summary>Return command line template (usage) string.</summary>
    public string GetCommandLineTemplate()
    {
        if (CommandLineBuilder is not null) return CommandLineBuilder(this);
        return BuildCommandLine();
    }

    public string BuildCommandLine()
    {
        var path = string.Join(' ', Path().Skip(1).Select(n => n.Name)); // skip root
        var opts = GetEffectiveOptions()
            .OrderBy(o => o.Name, StringComparer.OrdinalIgnoreCase)
            .Select(o =>
            {
                var name = o.Name.Length == 1 ? $"-{o.Name}" : $"--{o.Name}";
                if (o.HasValue)
                {
                    var valueName = o.ValueName ?? "VALUE";
                    return $"[{name} <{valueName}>]";
                }
                return $"[{name}]";
            });
        var parms = Parameters.Select(p =>
        {
            var core = p.Name + (p.IsVariadic ? "..." : string.Empty);
            return p.Required ? core : $"[{core}]";
        });
        var parts = new[] { path }.Concat(opts).Concat(parms).Where(s => s.Length > 0);
        return string.Join(' ', parts);
    }
}
