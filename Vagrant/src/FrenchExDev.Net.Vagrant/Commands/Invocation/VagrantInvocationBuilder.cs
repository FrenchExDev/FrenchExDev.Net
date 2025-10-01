using System.Diagnostics;

namespace FrenchExDev.Net.Vagrant.Commands.Invocation;

public static class VagrantInvocationBuilder
{
    /// <summary>
    /// Builds the argument list string for a given invocation (excluding the executable name 'vagrant').
    /// Options are ordered: path, options (root->leaf inherent ordering), then positionals.
    /// </summary>
    public static string BuildArgs(Invocation invocation)
    {
        var cmd = invocation.Command;
        var segments = new List<string>();
        // command path (skip root 'vagrant')
        foreach (var node in cmd.Path().Skip(1)) // root->...->leaf ; skip root for args
        {
            segments.Add(Escape(node.Name));
        }

        // collect effective options in deterministic order
        var effective = cmd.GetEffectiveOptions().OrderBy(o => o.Name, StringComparer.OrdinalIgnoreCase);
        foreach (var opt in effective)
        {
            if (!invocation.OptionValues.TryGetValue(opt.Name, out var value)) continue; // not set
            var primary = opt.Name.Length == 1 ? $"-{opt.Name}" : $"--{opt.Name}";
            if (value is null)
            {
                segments.Add(primary);
            }
            else
            {
                if (opt.HasValue && opt.ValueOptional)
                {
                    if (value.Length == 0)
                    {
                        segments.Add(primary); // optional value omitted
                        continue;
                    }
                }
                segments.Add(primary);
                if (value.Length > 0)
                {
                    segments.Add(Escape(value));
                }
            }
        }

        // positional parameters in declared order
        foreach (var param in cmd.Parameters)
        {
            var found = invocation.ParameterValues.FirstOrDefault(p => p.Name.Equals(param.Name, StringComparison.OrdinalIgnoreCase));
            if (found.Name is null)
            {
                if (param.Required)
                    throw new InvalidOperationException($"Missing required parameter '{param.Name}'.");
                continue;
            }
            // Treat zero-length value list as missing for required params
            if (param.Required && (found.Values is null || found.Values.Length == 0))
                throw new InvalidOperationException($"Missing required parameter '{param.Name}'.");
            foreach (var v in found.Values)
            {
                segments.Add(Escape(v));
            }
        }

        return string.Join(' ', segments);
    }

    private static string Escape(string value)
        => value.Contains(' ') || value.Contains('"') || value.Contains('\'')
            ? '"' + value.Replace("\"", "\\\"") + '"'
            : value;
}
