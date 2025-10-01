namespace FrenchExDev.Net.Vagrant.Commands;

/// <summary>
/// Concrete group node.
/// </summary>
public class CommandGroupNode : CommandNodeBase, ICommandGroupNode
{
    private readonly Dictionary<string, ICommandNode> _children = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Optional delegate supplying default parameter values for this group (e.g. global/root parameters).
    /// Any values explicitly set on an invocation override these.
    /// Return dictionary keyed by parameter name to one or more values (comma separated values can be provided for variadic, or use spaces via multiple entries when building invocation).
    /// </summary>
    public Func<CommandGroupNode, IReadOnlyDictionary<string, string[]>>? ParameterValuesProvider { get; private set; }

    public CommandGroupNode(string name, string? description = null, ICommandGroupNode? parent = null)
        : base(name, description, parent)
    {
    }

    public IReadOnlyDictionary<string, ICommandNode> Children => _children;

    public CommandGroupNode WithOption(CommandOption option)
    {
        AddOption(option);
        return this;
    }

    public CommandGroupNode WithParameter(CommandParameter parameter)
    {
        AddParameter(parameter);
        return this;
    }

    /// <summary>Attach a lambda that supplies default parameter values for this group.</summary>
    public CommandGroupNode WithParameterDefaults(Func<CommandGroupNode, IReadOnlyDictionary<string, string[]>> provider)
    {
        ParameterValuesProvider = provider;
        return this;
    }

    public TChild AddChild<TChild>(TChild child) where TChild : ICommandNode
    {
        _children[child.Name] = child;
        return child;
    }
}
