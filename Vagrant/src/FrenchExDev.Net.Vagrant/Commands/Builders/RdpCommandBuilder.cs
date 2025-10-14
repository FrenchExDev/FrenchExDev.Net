namespace FrenchExDev.Net.Vagrant.Commands.Builders;

public class RdpCommandBuilder : VagrantCommandBuilder<RdpCommandBuilder, RdpCommand>
{
    protected override RdpCommand Instantiate() => new RdpCommand
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
