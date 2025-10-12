namespace FrenchExDev.Net.Vagrant.Commands.Builders;

public class VersionCommandBuilder : VagrantCommandBuilder<VersionCommandBuilder, VersionCommand>
{
    protected override VersionCommand Instantiate()
    {
        return new VersionCommand
        {
            MachineReadable = _machineReadable,
            WorkingDirectory = _workingDirectory,
            Debug = _debug,
            DebugTimestamp = _debugTimestamp,
            EnvironmentVariables = _environmentVariables,
            Help = _help,
            NoTty = _noTty,
            NoColor = _noColor,
            Timestamp = _timestamp,
            Version = _version
        };
    }
}
