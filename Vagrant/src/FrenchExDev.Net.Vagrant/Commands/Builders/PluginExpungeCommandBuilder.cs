namespace FrenchExDev.Net.Vagrant.Commands.Builders;

public class PluginExpungeCommandBuilder : VagrantCommandBuilder<PluginExpungeCommandBuilder, PluginExpungeCommand>
{
    private bool? _force;
    private bool? _reinstall;

    public PluginExpungeCommandBuilder Force(bool? force = true)
    {
        _force = force;
        return this;
    }

    public PluginExpungeCommandBuilder Reinstall(bool? reinstall = true)
    {
        _reinstall = reinstall;
        return this;
    }

    protected override PluginExpungeCommand Instantiate() => new PluginExpungeCommand
    {
        Force = _force,
        Reinstall = _reinstall,
        WorkingDirectory = _workingDirectory,
        Debug = _debug,
        DebugTimestamp = _debugTimestamp,
        EnvironmentVariables = _environmentVariables,
        Help = _help,
        MachineReadable = _machineReadable,
        NoColor = _noColor,
        NoTty = _noTty,
        Timestamp = _timestamp,
        Version = _version
    };
}
