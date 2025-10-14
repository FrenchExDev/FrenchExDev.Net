using FrenchExDev.Net.CSharp.Object.Builder2;

namespace FrenchExDev.Net.Vagrant.Commands.Builders;

public class SshConfigCommandBuilder : VagrantCommandBuilder<SshConfigCommandBuilder, SshConfigCommand>
{
    private string? _machineName;
    private string? _extraArg;
    private string? _host;

    public SshConfigCommandBuilder Machine(string machine)
    {
        _machineName = machine;
        return this;
    }

    public SshConfigCommandBuilder ExtraArg(string arg)
    {
        _extraArg = arg;
        return this;
    }

    public SshConfigCommandBuilder Host(string host)
    {
        _host = host;
        return this;
    }

    protected override void ValidateInternal(VisitedObjectDictionary visitedCollector, FailuresDictionary failures)
    {
        base.ValidateInternal(visitedCollector, failures);

        if (_machineName is not null && string.IsNullOrWhiteSpace(_machineName))
            failures.Failure(nameof(SshConfigCommand.MachineName), new InvalidDataException("Machine name cannot be empty"));

        if (_host is not null && string.IsNullOrWhiteSpace(_host))
            failures.Failure(nameof(SshConfigCommand.Host), new InvalidDataException("--host cannot be set and empty"));
    }

    protected override SshConfigCommand Instantiate()
    {
        return new SshConfigCommand
        {
            MachineName = _machineName,
            ExtraArgument = _extraArg,
            Host = _host,
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
