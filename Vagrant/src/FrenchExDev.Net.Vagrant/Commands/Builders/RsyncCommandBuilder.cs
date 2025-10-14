using FrenchExDev.Net.CSharp.Object.Builder2;

namespace FrenchExDev.Net.Vagrant.Commands.Builders;

public class RsyncCommandBuilder : VagrantCommandBuilder<RsyncCommandBuilder, RsyncCommand>
{
    protected override RsyncCommand Instantiate() => new RsyncCommand
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
