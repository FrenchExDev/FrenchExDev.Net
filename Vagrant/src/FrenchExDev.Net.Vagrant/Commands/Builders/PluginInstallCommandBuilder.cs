using FrenchExDev.Net.CSharp.Object.Builder2;

namespace FrenchExDev.Net.Vagrant.Commands.Builders;

public class PluginInstallCommandBuilder : VagrantCommandBuilder<PluginInstallCommandBuilder, PluginInstallCommand>
{
    private string? _name;
    private string? _pluginVersion;
    private bool? _local;

    public PluginInstallCommandBuilder Name(string name)
    {
        _name = name;
        return this;
    }

    public PluginInstallCommandBuilder Version(string version)
    {
        _pluginVersion = version;
        return this;
    }

    public PluginInstallCommandBuilder Local(bool? local = true)
    {
        _local = local;
        return this;
    }

    protected override void ValidateInternal(VisitedObjectDictionary visitedCollector, FailuresDictionary failures)
    {
        base.ValidateInternal(visitedCollector, failures);

        if (_name is null || string.IsNullOrWhiteSpace(_name))
            failures.Failure("Parameters", new InvalidDataException("Missing required parameter 'name'"));
        if (_pluginVersion is not null && string.IsNullOrWhiteSpace(_pluginVersion))
            failures.Failure("Options", new InvalidDataException("--plugin-version cannot be empty"));
    }

    protected override PluginInstallCommand Instantiate() => new PluginInstallCommand
    {
        Name = _name,
        PluginVersion = _pluginVersion,
        Local = _local,
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
