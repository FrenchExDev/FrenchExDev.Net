using FrenchExDev.Net.CSharp.Object.Builder2;

namespace FrenchExDev.Net.Vagrant.Commands.Builders;

public class PluginUninstallCommandBuilder : VagrantCommandBuilder<PluginUninstallCommandBuilder, PluginUninstallCommand>
{
    private string? _name;

    public PluginUninstallCommandBuilder Name(string name)
    {
        _name = name;
        return this;
    }

    protected override void ValidateInternal(VisitedObjectDictionary visitedCollector, FailuresDictionary failures)
    {
        base.ValidateInternal(visitedCollector, failures);

        if (_name is null || string.IsNullOrWhiteSpace(_name))
            failures.Failure(nameof(PluginUninstallCommand.Name), new InvalidDataException("Missing required parameter 'name'"));
    }

    protected override PluginUninstallCommand Instantiate() => new PluginUninstallCommand
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
