namespace FrenchExDev.Net.Vagrant.Commands.Builders;

public class SnapshotListCommandBuilder : VagrantCommandBuilder<SnapshotListCommandBuilder, SnapshotListCommand>
{
    protected override SnapshotListCommand Instantiate()
    {
        return new SnapshotListCommand
        {
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
            OnStdOut = _onStdOut,
            OnStdErr = _onStdErr
        };
    }
}