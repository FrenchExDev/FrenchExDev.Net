namespace FrenchExDev.Net.Vagrant.Commands;

/// <summary>
/// A command node that can contain sub-commands (like `vagrant box`).
/// </summary>
public interface ICommandGroupNode : ICommandNode
{
    IReadOnlyDictionary<string, ICommandNode> Children { get; }
}
