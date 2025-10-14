namespace FrenchExDev.Net.Vagrant.Commands.Builders;

public class DestroyCommandBuilder : VagrantCommandBuilder<DestroyCommandBuilder, DestroyCommand>
{
    private bool? _force;
    private bool? _graceful;

    public DestroyCommandBuilder Force(bool? force = true)
    {
        _force = force;
        return this;
    }

    public DestroyCommandBuilder Graceful(bool? graceful = true)
    {
        _graceful = graceful;
        return this;
    }

    protected override DestroyCommand Instantiate() => new()
    {
        Force = _force,
        Graceful = _graceful,
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
