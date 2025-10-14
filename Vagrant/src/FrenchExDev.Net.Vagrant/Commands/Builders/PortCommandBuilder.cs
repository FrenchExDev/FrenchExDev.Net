namespace FrenchExDev.Net.Vagrant.Commands.Builders;

public class PortCommandBuilder : VagrantCommandBuilder<PortCommandBuilder, PortCommand>
{
    private bool? _private;
    private bool? _public;
    private string? _machine;

    public PortCommandBuilder Private(bool? p = true)
    {
        _private = p;
        return this;
    }

    public PortCommandBuilder Public(bool? p = true)
    {
        _public = p;
        return this;
    }

    public PortCommandBuilder Machine(string name)
    {
        _machine = name;
        return this;
    }

    protected override PortCommand Instantiate() => new PortCommand
    {
        Private = _private,
        Public = _public,
        Machine = _machine,
        WorkingDirectory = _workingDirectory,
        Debug = _debug,
        DebugTimestamp = _debugTimestamp,
        EnvironmentVariables = _environmentVariables,
        Help = _help,
        MachineReadable = _machineReadable,
        NoColor = _noColor,
        NoTty = _noTty,
        Timestamp = _timestamp,
        Version = _version,
        OnStdErr = _onStdErr,
        OnStdOut = _onStdOut
    };
}
