using FrenchExDev.Net.CSharp.Object.Builder2;

namespace FrenchExDev.Net.Vagrant.Commands.Builders;

public class SshCommandBuilder : VagrantCommandBuilder<SshCommandBuilder, SshCommand>
{
    private string? _command;

    public SshCommandBuilder Command(string cmd)
    {
        _command = cmd;
        return this;
    }

    protected override void ValidateInternal(VisitedObjectDictionary visitedCollector, FailuresDictionary failures)
    {
        base.ValidateInternal(visitedCollector, failures);

        if (string.IsNullOrWhiteSpace(_command))
            failures.Failure(nameof(_command), new InvalidDataException("--command cannot be empty"));
    }

    protected override SshCommand Instantiate()
    {
        return new SshCommand
        {
            Command = _command,
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
}
