namespace FrenchExDev.Net.Vagrant.Commands;

public sealed record SshConfigCommand : VagrantCommandBase
{
    /// <summary>
    /// Optional machine name (generally the VM name defined in the Vagrantfile).
    /// When provided, the command becomes: `vagrant ssh-config {MachineName}`
    /// </summary>
    public string? MachineName { get; init; }

    /// <summary>
    /// Optional additional argument forwarded to the ssh config command (kept generic for future flags).
    /// Example: some implementations support --host or other flags; add here if needed.
    /// </summary>
    public string? ExtraArgument { get; init; }

    public string? Host { get; init; }

    public override IReadOnlyList<string> ToArguments()
    {
        var args = new List<string> { "ssh-config" };
        if (!string.IsNullOrWhiteSpace(MachineName)) args.Add(MachineName);
        if (!string.IsNullOrWhiteSpace(ExtraArgument)) args.Add(ExtraArgument);
        if (!string.IsNullOrWhiteSpace(Host)) { args.Add("--host"); args.Add(Host!); }
        return args;
    }
}
