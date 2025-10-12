namespace FrenchExDev.Net.Vagrant.Commands;

public abstract record VagrantCommandBase : IVagrantCommand
{
    public required bool? NoColor { get; init; }
    public required bool? MachineReadable { get; init; }
    public required bool? Version { get; init; }
    public required bool? Debug { get; init; }
    public required bool? Timestamp { get; init; }
    public required bool? DebugTimestamp { get; init; }
    public required bool? NoTty { get; init; }
    public required bool? Help { get; init; }
    public required string? WorkingDirectory { get; init; }
    public required IReadOnlyDictionary<string, string> EnvironmentVariables { get; init; } = new Dictionary<string, string>();
    public abstract IReadOnlyList<string> ToArguments();

    public void BaseArguments(List<string> arguments)
    {
        if (NoColor == true)
            arguments.Add("--no-color");

        if (MachineReadable == true)
            arguments.Add("--machine-readable");

        if (Version == true)
            arguments.Add("--version");

        if (Debug == true)
            arguments.Add("--debug");

        if (Timestamp == true)
            arguments.Add("--timestamp");

        if (DebugTimestamp == true)
            arguments.Add("--debug-timestamp");

        if (NoTty == true)
            arguments.Add("--no-tty");

        if (Help == true)
            arguments.Add("--help");
    }
}
