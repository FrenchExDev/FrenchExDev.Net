using FrenchExDev.Net.CSharp.Object.Builder2;

namespace FrenchExDev.Net.Vagrant.Commands.Builders;

public class BoxRemoveCommandBuilder : BoxCommandBuilder<BoxRemoveCommandBuilder, BoxRemoveCommand>
{
    private string? _name;
    private string? _provider;
    private bool? _all;
    private bool? _force;

    public BoxRemoveCommandBuilder Name(string name)
    {
        _name = name;
        return this;
    }

    public BoxRemoveCommandBuilder Provider(string provider)
    {
        _provider = provider;
        return this;
    }

    public BoxRemoveCommandBuilder All(bool? all = true)
    {
        _all = all;
        return this;
    }

    public BoxRemoveCommandBuilder Force(bool? force = true)
    {
        _force = force;
        return this;
    }

    protected override void ValidateInternal(VisitedObjectDictionary visitedCollector, FailuresDictionary failures)
    {
        base.ValidateInternal(visitedCollector, failures);

        if (_provider is not null && string.IsNullOrWhiteSpace(_provider))
            failures.Failure(nameof(BoxRemoveCommand.Provider), new InvalidDataException("--provider cannot be empty"));
        if (_name is not null && string.IsNullOrWhiteSpace(_name))
            failures.Failure(nameof(BoxRemoveCommand.Name), new InvalidDataException("Name parameter cannot be empty"));
        if (_all == true && _name is not null)
            failures.Failure(nameof(BoxRemoveCommand.All), new InvalidDataException("--all cannot be combined with a name parameter"));
    }

    protected override BoxRemoveCommand Instantiate()
    {
        return new BoxRemoveCommand
        {
            Name = _name,
            Provider = _provider,
            All = _all,
            Force = _force,
            WorkingDirectory = _workingDirectory,
            NoColor = _noColor == true ? false : null,
            MachineReadable = _machineReadable,
            Version = _version,
            Debug = _debug,
            Timestamp = _timestamp,
            DebugTimestamp = _debugTimestamp,
            NoTty = _noTty,
            Help = _help,
            EnvironmentVariables = _environmentVariables,
            OnStdErr = _onStdErr,
            OnStdOut = _onStdOut
        };
    }
}
