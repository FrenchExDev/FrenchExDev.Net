namespace FrenchExDev.Net.Vagrant.Commands.Builders;

public class GlobalStatusCommandBuilder : VagrantCommandBuilder<GlobalStatusCommandBuilder, GlobalStatusCommand>
{
    private bool? _prune;

    public GlobalStatusCommandBuilder Prune(bool? prune = true)
    {
        _prune = prune;
        return this;
    }

    protected override GlobalStatusCommand Instantiate()
    {
        return new GlobalStatusCommand
        {
            Prune = _prune,
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
}
