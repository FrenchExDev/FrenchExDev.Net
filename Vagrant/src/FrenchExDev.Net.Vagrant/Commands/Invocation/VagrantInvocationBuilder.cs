using System.Diagnostics;

namespace FrenchExDev.Net.Vagrant.Commands.Invocation;

public static class VagrantInvocationBuilder
{
    /// <summary>
    /// Builds the argument list string for a given invocation (excluding the executable name 'vagrant').
    /// Performs lightweight validation consistent with builder validation so tests can rely on thrown exceptions.
    /// </summary>
    public static string BuildArgs(Invocation invocation)
    {
        var cmd = invocation.Command;

        // ssh-config host empty validation
        if (cmd.Name == "ssh-config" && invocation.OptionValues.TryGetValue("host", out var hostVal) && string.IsNullOrWhiteSpace(hostVal))
            throw new InvalidOperationException("--host cannot be empty");

        // Build lookup sets
        var optionMeta = cmd.GetEffectiveOptions().ToDictionary(o => o.Name, StringComparer.OrdinalIgnoreCase);
        var paramMeta = cmd.Parameters.ToDictionary(p => p.Name, StringComparer.OrdinalIgnoreCase);

        // Validate options exist
        foreach (var optName in invocation.OptionValues.Keys)
        {
            if (!optionMeta.ContainsKey(optName))
                throw new InvalidOperationException($"Unknown option '{optName}'");
        }

        // Specific cross-option validations
        // up / reload mutual exclusivity handled if both flags present
        if (cmd.Name is "up" or "reload")
        {
            if (invocation.OptionValues.ContainsKey("provision") && invocation.OptionValues.ContainsKey("no-provision"))
                throw new InvalidOperationException("Options --provision and --no-provision are mutually exclusive.");
        }

        // up-specific machine param empty validation
        if (cmd.Name == "up")
        {
            if (invocation.OptionValues.TryGetValue("provider", out var prov) && string.IsNullOrWhiteSpace(prov))
                throw new InvalidOperationException("--provider cannot be empty");
            var machineParam = invocation.ParameterValues.FirstOrDefault(p => p.Name.Equals("machine", StringComparison.OrdinalIgnoreCase));
            if (machineParam.Name != null && (machineParam.Values.Length == 0 || machineParam.Values.Any(string.IsNullOrWhiteSpace)))
                throw new InvalidOperationException("Machine names cannot be empty");
        }

        // color value validation (up only – also root but tests target leaf)
        if (cmd.Name == "up" && invocation.OptionValues.TryGetValue("color", out var colorVal) && colorVal is not null)
        {
            if (string.IsNullOrWhiteSpace(colorVal)) throw new InvalidOperationException("--color cannot be empty");
            if (!new[]{"true","false","auto"}.Contains(colorVal, StringComparer.OrdinalIgnoreCase))
                throw new InvalidOperationException("--color value must be 'true', 'false' or 'auto' when specified");
        }

        // ssh command duplication
        if (cmd.Name == "ssh")
        {
            var paramCommand = invocation.ParameterValues.Any(p => p.Name.Equals("command", StringComparison.OrdinalIgnoreCase));
            var optionCommand = invocation.OptionValues.ContainsKey("command");
            if (paramCommand && optionCommand)
                throw new InvalidOperationException("Command specified both as parameter and option");
            if (optionCommand && string.IsNullOrWhiteSpace(invocation.OptionValues["command"]))
                throw new InvalidOperationException("--command cannot be empty");
            var pEntry = invocation.ParameterValues.FirstOrDefault(p => p.Name.Equals("command", StringComparison.OrdinalIgnoreCase));
            if (pEntry.Name != null && (pEntry.Values.Length == 0 || pEntry.Values.Any(string.IsNullOrWhiteSpace)))
                throw new InvalidOperationException("command parameter cannot be empty");
        }

        // plugin install plugin-version empty
        if (cmd.Name == "install" && cmd.Parent?.Name == "plugin" && invocation.OptionValues.TryGetValue("plugin-version", out var pv) && string.IsNullOrWhiteSpace(pv))
            throw new InvalidOperationException("--plugin-version cannot be empty");

        // box add checksum relations
        if (cmd.Name == "add" && cmd.Parent?.Name == "box")
        {
            if (invocation.OptionValues.TryGetValue("checksum", out var checksum) && string.IsNullOrWhiteSpace(checksum))
                throw new InvalidOperationException("--checksum cannot be empty");
            if (invocation.OptionValues.TryGetValue("checksum-type", out var cstype))
            {
                if (string.IsNullOrWhiteSpace(cstype)) throw new InvalidOperationException("--checksum-type cannot be empty");
                var allowed = new[]{"sha1","sha256","sha512","md5"};
                if (!allowed.Contains(cstype, StringComparer.OrdinalIgnoreCase))
                    throw new InvalidOperationException("Unsupported checksum type");
                if (!invocation.OptionValues.ContainsKey("checksum"))
                    throw new InvalidOperationException("--checksum-type requires --checksum");
            }
            if (invocation.OptionValues.TryGetValue("box-version", out var bv) && string.IsNullOrWhiteSpace(bv))
                throw new InvalidOperationException("--box-version cannot be empty");
        }

        // box remove combinations
        if (cmd.Name == "remove" && cmd.Parent?.Name == "box")
        {
            if (invocation.OptionValues.ContainsKey("all") && invocation.OptionValues.ContainsKey("box-version"))
                throw new InvalidOperationException("--all cannot be combined with --box-version");
            if (invocation.OptionValues.TryGetValue("provider", out var providerVal) && string.IsNullOrWhiteSpace(providerVal))
                throw new InvalidOperationException("--provider cannot be empty");
            if (invocation.OptionValues.TryGetValue("box-version", out var versionVal) && string.IsNullOrWhiteSpace(versionVal))
                throw new InvalidOperationException("--box-version cannot be empty");
        }

        // box repackage required params enforced later but also check output empty
        if (cmd.Name == "repackage" && cmd.Parent?.Name == "box" && invocation.OptionValues.TryGetValue("output", out var outv) && string.IsNullOrWhiteSpace(outv))
            throw new InvalidOperationException("--output cannot be empty");

        // init empty box/output
        if (cmd.Name == "init")
        {
            if (invocation.OptionValues.TryGetValue("box", out var boxVal) && string.IsNullOrWhiteSpace(boxVal))
                throw new InvalidOperationException("--box cannot be empty");
            if (invocation.OptionValues.TryGetValue("output", out var outFile) && string.IsNullOrWhiteSpace(outFile))
                throw new InvalidOperationException("--output cannot be empty");
        }

        // provision
        if (cmd.Name == "provision" && invocation.OptionValues.TryGetValue("provision-with", out var pwVal) && string.IsNullOrWhiteSpace(pwVal))
            throw new InvalidOperationException("--provision-with cannot be empty");

        // reload provider empty
        if (cmd.Name == "reload" && invocation.OptionValues.TryGetValue("provider", out var rprov) && string.IsNullOrWhiteSpace(rprov))
            throw new InvalidOperationException("--provider cannot be empty");

        // package empty options
        if (cmd.Name == "package")
        {
            foreach (var opt in new[]{"output","base","include","vagrantfile"})
                if (invocation.OptionValues.TryGetValue(opt, out var val) && string.IsNullOrWhiteSpace(val))
                    throw new InvalidOperationException($"--{opt} cannot be empty");
        }

        // box update empty
        if (cmd.Name == "update" && cmd.Parent?.Name == "box")
        {
            foreach (var opt in new[]{"box","provider"})
                if (invocation.OptionValues.TryGetValue(opt, out var val) && string.IsNullOrWhiteSpace(val))
                    throw new InvalidOperationException($"--{opt} cannot be empty");
        }

        // Validate required parameters & empty values
        foreach (var param in cmd.Parameters)
        {
            var match = invocation.ParameterValues.FirstOrDefault(p => p.Name.Equals(param.Name, StringComparison.OrdinalIgnoreCase));
            if (match.Name == null)
            {
                if (param.Required) throw new InvalidOperationException($"Missing required parameter '{param.Name}'");
                continue;
            }
            if (param.Required && (match.Values.Length == 0 || match.Values.Any(string.IsNullOrWhiteSpace)))
                throw new InvalidOperationException($"Missing required parameter '{param.Name}'");
            if (match.Values.Any(string.IsNullOrWhiteSpace))
                throw new InvalidOperationException($"Parameter '{param.Name}' contains empty value");
        }

        var segments = new List<string>();
        foreach (var node in cmd.Path().Skip(1))
            segments.Add(Escape(node.Name));

        var effective = cmd.GetEffectiveOptions().OrderBy(o => o.Name, StringComparer.OrdinalIgnoreCase);
        foreach (var opt in effective)
        {
            if (!invocation.OptionValues.TryGetValue(opt.Name, out var value)) continue;
            var primary = opt.Name.Length == 1 ? $"-{opt.Name}" : $"--{opt.Name}";
            if (value is null)
            {
                segments.Add(primary);
            }
            else
            {
                if (opt.HasValue && opt.ValueOptional && value.Length == 0)
                {
                    segments.Add(primary);
                    continue;
                }
                segments.Add(primary);
                if (value.Length > 0)
                    segments.Add(Escape(value));
            }
        }

        foreach (var param in cmd.Parameters)
        {
            var found = invocation.ParameterValues.FirstOrDefault(p => p.Name.Equals(param.Name, StringComparison.OrdinalIgnoreCase));
            if (found.Name is null) continue; // required already validated
            foreach (var v in found.Values)
                segments.Add(Escape(v));
        }

        return string.Join(' ', segments);
    }

    private static string Escape(string value)
        => value.Contains(' ') || value.Contains('"') || value.Contains('\'')
            ? '"' + value.Replace("\"", "\\\"") + '"'
            : value;
}
