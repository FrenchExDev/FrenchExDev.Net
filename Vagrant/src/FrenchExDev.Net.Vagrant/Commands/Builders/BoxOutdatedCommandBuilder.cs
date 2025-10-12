using FrenchExDev.Net.CSharp.Object.Builder2;

namespace FrenchExDev.Net.Vagrant.Commands.Builders;

public class BoxOutdatedCommandBuilder : BoxCommandBuilder<BoxOutdatedCommandBuilder, BoxOutdatedCommand>
{
    private string? _provider;
    private bool? _global;
    private bool? _insecure;

    public BoxOutdatedCommandBuilder Provider(string provider)
    {
        _provider = provider;
        return this;
    }

    public BoxOutdatedCommandBuilder Global(bool? global = true)
    {
        _global = global;
        return this;
    }

    public BoxOutdatedCommandBuilder Insecure(bool? insecure = true)
    {
        _insecure = insecure;
        return this;
    }

    protected override void ValidateInternal(VisitedObjectDictionary visitedCollector, FailuresDictionary failures)
    {
        base.ValidateInternal(visitedCollector, failures);
        if (failures.Count > 0) return;
        if (_provider is not null && string.IsNullOrWhiteSpace(_provider))
            failures.Failure("Options", new InvalidDataException("--provider cannot be empty"));
    }

    protected override BoxOutdatedCommand Instantiate() => new BoxOutdatedCommand
    {
        Provider = _provider,
        Global = _global,
        Insecure = _insecure,
        WorkingDirectory = _workingDirectory,
        NoColor = _noColor == true ? false : null,
        Debug = _debug == true ? true : null,
        DebugTimestamp = _debugTimestamp == true ? true : null,
        MachineReadable = _machineReadable == true ? true : null,
        NoTty = _noTty == true ? true : null,
        Timestamp = _timestamp == true ? true : null,
        Version = _version == true ? true : null,
        EnvironmentVariables = _environmentVariables,
        Help = _help == true ? true : null
    };
}
