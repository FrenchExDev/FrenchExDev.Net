namespace FrenchExDev.Net.Vagrant.Commands.Builders;

public class StatusCommandBuilder : VagrantCommandBuilder<StatusCommandBuilder, StatusCommand>
{
    protected override StatusCommand Instantiate()
    {
        return new StatusCommand
        {
            MachineReadable = _machineReadable,
            WorkingDirectory = _workingDirectory,
            NoColor = _noColor,
            Debug = _debug,
            DebugTimestamp = _debugTimestamp,
            EnvironmentVariables = _environmentVariables,
            Help = _help,
            NoTty = _noTty,
            Timestamp = _timestamp,
            Version = _version
        };
    }
}
