using FrenchExDev.Net.CSharp.Object.Builder2;

namespace FrenchExDev.Net.Vagrant.Commands.Builders;

public class SnapshotSaveCommandBuilder : VagrantCommandBuilder<SnapshotSaveCommandBuilder, SnapshotSaveCommand>
{
    private string? _name;

    public SnapshotSaveCommandBuilder Name(string name)
    {
        _name = name;
        return this;
    }

    protected override void ValidateInternal(VisitedObjectDictionary visitedCollector, FailuresDictionary failures)
    {
        base.ValidateInternal(visitedCollector, failures);

        if (_name is null || string.IsNullOrWhiteSpace(_name))
            failures.Failure(nameof(SnapshotSaveCommand.Name), new InvalidDataException("Missing required parameter 'name'"));
    }

    protected override SnapshotSaveCommand Instantiate()
    {
        return new SnapshotSaveCommand
        {
            Name = _name,
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
