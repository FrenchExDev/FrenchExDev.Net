namespace FrenchExDev.Net.Vagrant.Commands.Builders;

public class LoginCommandBuilder : VagrantCommandBuilder<LoginCommandBuilder, LoginCommand>
{
    protected override LoginCommand Instantiate() => new LoginCommand
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
