namespace FrenchExDev.Net.Vagrant.Commands.Builders;

public class SnapshotPushCommandBuilder : VagrantCommandBuilder<SnapshotPushCommandBuilder, SnapshotPushCommand>
{
    protected override SnapshotPushCommand Instantiate() => new SnapshotPushCommand
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
        Version = _version
    };
}
