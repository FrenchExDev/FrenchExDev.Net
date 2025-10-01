namespace FrenchExDev.Net.Vagrant.Commands;

/// <summary>
/// Basic abstraction for anything that can appear in the vagrant command tree (a group or a leaf command).
/// </summary>
public interface ICommandNode
{
    /// <summary>Name shown in CLI path (e.g. "box", "add").</summary>
    string Name { get; }

    /// <summary>Short description.</summary>
    string? Description { get; }

    /// <summary>Options directly belonging to this node (not including inherited parent options).</summary>
    IReadOnlyCollection<CommandOption> OwnOptions { get; }

    /// <summary>All options including inherited from parents.</summary>
    IReadOnlyCollection<CommandOption> GetEffectiveOptions();

    /// <summary>Parent, null for root.</summary>
    ICommandGroupNode? Parent { get; }

    /// <summary>Enumerates the path from root to this node.</summary>
    IEnumerable<ICommandNode> Path();

    /// <summary>Positional parameters this node defines (not including parent composition; parents usually don't define positionals).</summary>
    IReadOnlyList<CommandParameter> Parameters { get; }
}
