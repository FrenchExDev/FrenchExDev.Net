using FrenchExDev.Net.CSharp.Object.Builder2;

namespace FrenchExDev.Net.Vagrant.Commands.Builders;

public class PushCommandBuilder : VagrantCommandBuilder<PushCommandBuilder, PushCommand>
{
    private string? _name;

    public PushCommandBuilder Name(string n)
    {
        _name = n;
        return this;
    }

    protected override void ValidateInternal(VisitedObjectDictionary visitedCollector, FailuresDictionary failures)
    {
        base.ValidateInternal(visitedCollector, failures);

        if (string.IsNullOrEmpty(_name))
        {
            failures.Failure(nameof(_name), new InvalidDataException("Missing required parameter 'name'"));
        }
    }

    protected override PushCommand Instantiate() => new PushCommand
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
        Version = _version,
        OnStdErr = _onStdErr,
        OnStdOut = _onStdOut
    };
}
