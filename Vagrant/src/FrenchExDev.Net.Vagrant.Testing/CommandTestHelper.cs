using FrenchExDev.Net.Vagrant.Commands;
using FrenchExDev.Net.Vagrant.Commands.Invocation;

namespace FrenchExDev.Net.Vagrant.Testing;

public static class CommandTestHelper
{
    public static Invocation Build(string command, string paramSpec, string optionSpec)
       => CommandTestHelper.BuildInvocation(command, paramSpec, optionSpec);

    public static LeafCommandNode GetLeaf(string commandPath)
    {
        var tree = VagrantCommandTree.Build();
        var segments = commandPath.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (segments.Length == 0) throw new InvalidOperationException("Empty command path");
        ICommandNode current = tree;
        for (int i = 0; i < segments.Length; i++)
        {
            var seg = segments[i];
            if (current is ICommandGroupNode grp)
            {
                if (!grp.Children.TryGetValue(seg, out var next))
                {
                    if (segments.Length == 1)
                    {
                        foreach (var child in tree.Children.Values)
                        {
                            if (child is ICommandGroupNode g && g.Children.TryGetValue(seg, out var found) && found is LeafCommandNode lf1)
                                return lf1;
                        }
                    }
                    throw new InvalidOperationException($"Command segment '{seg}' not found in path '{commandPath}'.");
                }
                current = next;
            }
            else
            {
                throw new InvalidOperationException($"Unexpected non-group before end of path at segment '{seg}'.");
            }
        }
        return current as LeafCommandNode ?? throw new InvalidOperationException($"Path '{commandPath}' does not resolve to a leaf command.");
    }

    public static Invocation BuildInvocation(string command, string paramSpec, string optionSpec)
    {
        var leaf = GetLeaf(command);
        var inv = new Invocation { Command = leaf };
        if (!string.IsNullOrWhiteSpace(paramSpec))
        {
            foreach (var part in paramSpec.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            {
                var kv = part.Split('=', 2);
                var pName = kv[0];
                var values = kv.Length > 1 && kv[1].Length > 0 ? kv[1].Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries) : Array.Empty<string>();
                if (values.Length == 0) inv.Param(pName); else inv.Param(pName, values);
            }
        }
        if (!string.IsNullOrWhiteSpace(optionSpec))
        {
            foreach (var part in optionSpec.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            {
                var kv = part.Split('=', 2);
                if (kv.Length == 1) inv.Flag(kv[0]); else inv.Option(kv[0], kv[1]);
            }
        }
        return inv;
    }
}
