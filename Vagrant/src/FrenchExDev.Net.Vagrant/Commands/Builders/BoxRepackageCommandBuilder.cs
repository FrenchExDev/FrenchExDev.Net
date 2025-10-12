using FrenchExDev.Net.CSharp.Object.Builder2;

namespace FrenchExDev.Net.Vagrant.Commands.Builders;

public class BoxRepackageCommandBuilder : BoxCommandBuilder<BoxRepackageCommandBuilder, BoxRepackageCommand>
{
    private string? _name;
    private string? _provider;
    private string? _boxVersion;
    private string? _output;

    public BoxRepackageCommandBuilder Name(string name)
    {
        _name = name;
        return this;
    }

    public BoxRepackageCommandBuilder Provider(string provider)
    {
        _provider = provider;
        return this;
    }

    public BoxRepackageCommandBuilder BoxVersion(string version)
    {
        _boxVersion = version;
        return this;
    }

    public BoxRepackageCommandBuilder Output(string output)
    {
        _output = output;
        return this;
    }

    protected override void ValidateInternal(VisitedObjectDictionary visitedCollector, FailuresDictionary failures)
    {
        base.ValidateInternal(visitedCollector, failures);

        if (_name is null || string.IsNullOrWhiteSpace(_name))
            failures.Failure(nameof(BoxRepackageCommand.Name), new InvalidDataException("Parameter 'name' is required and cannot be empty"));
        if (_provider is null || string.IsNullOrWhiteSpace(_provider))
            failures.Failure(nameof(BoxRepackageCommand.Provider), new InvalidDataException("Parameter 'provider' is required and cannot be empty"));
        if (_boxVersion is null || string.IsNullOrWhiteSpace(_boxVersion))
            failures.Failure(nameof(BoxRepackageCommand.Version), new InvalidDataException("Parameter 'version' is required and cannot be empty"));
        if (_output is not null && string.IsNullOrWhiteSpace(_output))
            failures.Failure(nameof(_output), new InvalidDataException("--output cannot be empty"));
    }

    protected override BoxRepackageCommand Instantiate() => new BoxRepackageCommand
    {
        Name = _name,
        Provider = _provider,
        BoxVersion = _boxVersion,
        WorkingDirectory = _workingDirectory,
        NoColor = _noColor == true ? false : null,
        MachineReadable = _machineReadable,
        Version = _version,
        Debug = _debug,
        Timestamp = _timestamp,
        DebugTimestamp = _debugTimestamp,
        NoTty = _noTty,
        Help = _help,
        EnvironmentVariables = _environmentVariables
    };
}
