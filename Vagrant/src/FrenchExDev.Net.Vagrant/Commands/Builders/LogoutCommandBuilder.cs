namespace FrenchExDev.Net.Vagrant.Commands.Builders;

public class LogoutCommandBuilder : VagrantCommandBuilder<LogoutCommandBuilder, LogoutCommand>
{
    protected override LogoutCommand Instantiate() => new LogoutCommand
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
        OnStdErr = _onStdErr,
        OnStdOut = _onStdOut
    };
}
