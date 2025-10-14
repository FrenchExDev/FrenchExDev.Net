using FrenchExDev.Net.CSharp.Object.Builder2;

namespace FrenchExDev.Net.Vagrant.Commands.Builders;

public class BoxUpdateCommandBuilder : BoxCommandBuilder<BoxUpdateCommandBuilder, BoxUpdateCommand>
{
    private string? _name;
    private string? _provider;

    public BoxUpdateCommandBuilder Name(string name)
    {
        _name = name;
        return this;
    }

    public BoxUpdateCommandBuilder Provider(string provider)
    {
        _provider = provider;
        return this;
    }

    protected override void ValidateInternal(VisitedObjectDictionary visitedCollector, FailuresDictionary failures)
    {
        base.ValidateInternal(visitedCollector, failures);

        if (_name is not null && string.IsNullOrWhiteSpace(_name))
            failures.Failure(nameof(BoxUpdateCommand.Name), new InvalidDataException("--box cannot be empty"));

        if (_provider is not null && string.IsNullOrWhiteSpace(_provider))
            failures.Failure(nameof(BoxUpdateCommand.Provider), new InvalidDataException("--provider cannot be empty"));
    }

    protected override BoxUpdateCommand Instantiate()
    {
        return new BoxUpdateCommand
        {
            Name = _name,
            Provider = _provider,
            WorkingDirectory = _workingDirectory,
            EnvironmentVariables = _environmentVariables,
            NoColor = _noColor == true ? false : null,
            Debug = _debug == true ? true : null,
            DebugTimestamp = _debugTimestamp == true ? true : null,
            MachineReadable = _machineReadable == true ? true : null,
            NoTty = _noTty == true ? true : null,
            Timestamp = _timestamp == true ? true : null,
            Version = _version == true ? true : null,
            Help = _help == true ? true : null,
            OnStdOut = _onStdOut,
            OnStdErr = _onStdErr
        };
    }
}
