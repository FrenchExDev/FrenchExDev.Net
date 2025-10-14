using FrenchExDev.Net.CSharp.Object.Builder2;

namespace FrenchExDev.Net.Vagrant.Commands.Builders;

public class SuspendCommandBuilder : VagrantCommandBuilder<SuspendCommandBuilder, SuspendCommand>
{
    private string? _name;

    public SuspendCommandBuilder Name(string name)
    {
        _name = name;
        return this;
    }

    protected override void ValidateInternal(VisitedObjectDictionary visitedCollector, FailuresDictionary failures)
    {
        base.ValidateInternal(visitedCollector, failures);

        if (_name is not null && string.IsNullOrWhiteSpace(_name))
            failures.Failure(nameof(SuspendCommand.Name), new InvalidDataException("If specified, 'name' cannot be empty or whitespace"));
    }

    protected override SuspendCommand Instantiate()
    {
        return new SuspendCommand
        {
            Name = _name,
            MachineReadable = _machineReadable,
            WorkingDirectory = _workingDirectory,
            NoColor = _noColor,
            Debug = _debug,
            DebugTimestamp = _debugTimestamp,
            EnvironmentVariables = _environmentVariables,
            Help = _help,
            NoTty = _noTty,
            Timestamp = _timestamp,
            Version = _version,
            OnStdOut = _onStdOut,
            OnStdErr = _onStdErr
        };
    }
}