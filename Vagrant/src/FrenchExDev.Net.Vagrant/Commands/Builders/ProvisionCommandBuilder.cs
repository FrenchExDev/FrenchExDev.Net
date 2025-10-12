using FrenchExDev.Net.CSharp.Object.Builder2;

namespace FrenchExDev.Net.Vagrant.Commands.Builders;

public class ProvisionCommandBuilder : VagrantCommandBuilder<ProvisionCommandBuilder, ProvisionCommand>
{
    private string? _name;
    private List<string> _provisionWith = new();

    public ProvisionCommandBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    public ProvisionCommandBuilder ProvisionWith(string name)
    {
        _provisionWith.Add(name);
        return this;
    }

    protected override void ValidateInternal(VisitedObjectDictionary visitedCollector, FailuresDictionary failures)
    {
        base.ValidateInternal(visitedCollector, failures);

        if (_provisionWith.Count > 0)
        {
            foreach (var provisionWith in _provisionWith)
            {
                if (string.IsNullOrWhiteSpace(provisionWith))
                    failures.Failure(nameof(_provisionWith), new InvalidDataException("--provision-with cannot be empty"));
            }
        }
    }

    protected override ProvisionCommand Instantiate()
    {
        return new ProvisionCommand
        {
            Name = _name,
            ProvisionWith = _provisionWith.Any() ? _provisionWith : null,
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
