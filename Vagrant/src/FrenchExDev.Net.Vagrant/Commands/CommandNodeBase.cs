namespace FrenchExDev.Net.Vagrant.Commands;

/// <summary>
/// Base implementation shared by group and leaf nodes.
/// Uses composition to inherit parent options at runtime rather than static inheritance.
/// </summary>
public abstract class CommandNodeBase : ICommandNode
{
    private readonly List<CommandOption> _ownOptions = [];
    private readonly List<CommandParameter> _parameters = [];

    protected CommandNodeBase(string name, string? description, ICommandGroupNode? parent)
    {
        Name = name;
        Description = description;
        Parent = parent;
    }

    public string Name { get; }
    public string? Description { get; }
    public ICommandGroupNode? Parent { get; }

    public IReadOnlyCollection<CommandOption> OwnOptions => _ownOptions;
    public IReadOnlyList<CommandParameter> Parameters => _parameters;

    protected void AddOption(CommandOption option) => _ownOptions.Add(option);
    protected void AddParameter(CommandParameter parameter) => _parameters.Add(parameter);

    public IReadOnlyCollection<CommandOption> GetEffectiveOptions()
    {
        // Compose options from root to leaf; later duplicates (by Name) override earlier.
        var dict = new Dictionary<string, CommandOption>(StringComparer.OrdinalIgnoreCase);
        foreach (var node in Path())
        {
            foreach (var opt in node.OwnOptions)
            {
                dict[opt.Name] = opt; // override
            }
        }
        return dict.Values.ToArray();
    }

    public IEnumerable<ICommandNode> Path()
    {
        var stack = new Stack<ICommandNode>();
        ICommandNode? current = this;
        while (current is not null)
        {
            stack.Push(current);
            current = current.Parent;
        }
        return stack; // root -> self order
    }
}
