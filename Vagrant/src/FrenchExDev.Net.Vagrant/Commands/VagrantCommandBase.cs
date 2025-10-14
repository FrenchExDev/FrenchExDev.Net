namespace FrenchExDev.Net.Vagrant.Commands;

/// <summary>
/// Provides a base abstraction for Vagrant command implementations, including common options, environment
/// configuration, and output handling delegates.
/// </summary>
/// <remarks>This type defines shared properties and behaviors for Vagrant commands, such as standard output and
/// error handlers, command-line options, and environment variables. Derived types should implement the argument
/// construction logic by overriding the ToArguments method. Instances of this type are intended to be used as immutable
/// command descriptors for executing Vagrant operations.</remarks>
public abstract record VagrantCommandBase : IVagrantCommand
{
    public required List<Func<string, Task>> OnStdOut { get; init; }
    public required List<Func<string, Task>> OnStdErr { get; init; }
    public List<Func<string, Task>> GetOnStdOut() => OnStdOut;
    public List<Func<string, Task>> GetOnStdErr() => OnStdErr;
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

    public void AddOnStdOut(Func<string, Task> func)
    {
        throw new NotImplementedException();
    }

    public void AddOnStdErr(Func<string, Task> func)
    {
        throw new NotImplementedException();
    }
}
