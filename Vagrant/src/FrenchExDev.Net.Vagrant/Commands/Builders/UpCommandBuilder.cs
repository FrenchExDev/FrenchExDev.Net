using FrenchExDev.Net.CSharp.Object.Builder2;

namespace FrenchExDev.Net.Vagrant.Commands.Builders;

public class UpCommandBuilder : VagrantCommandBuilder<UpCommandBuilder, UpCommand>
{
    private bool? _provision;
    private string? _provider;
    private bool? _parallel;
    private bool? _destroyOnError;

    public UpCommandBuilder Provision(bool? provision = true)
    {
        _provision = provision;
        return this;
    }

    public UpCommandBuilder Provider(string provider)
    {
        _provider = provider;
        return this;
    }

    public UpCommandBuilder Parallel(bool? parallel = true)
    {
        _parallel = parallel;
        return this;
    }

    public UpCommandBuilder DestroyOnError(bool? destroy = true)
    {
        _destroyOnError = destroy;
        return this;
    }

    protected override void ValidateInternal(VisitedObjectDictionary visitedCollector, FailuresDictionary failures)
    {
        base.ValidateInternal(visitedCollector, failures);

        if (_provider is not null && string.IsNullOrWhiteSpace(_provider))
            failures.Failure(nameof(UpCommand.Provider), new InvalidDataException("--provider cannot be empty"));
    }

    protected override UpCommand Instantiate() => new UpCommand
    {
        Provision = _provision,
        Provider = _provider,
        NoColor = _noColor,
        Parallel = _parallel,
        DestroyOnError = _destroyOnError,
        WorkingDirectory = _workingDirectory,
        Debug = _debug,
        DebugTimestamp = _debugTimestamp,
        EnvironmentVariables = _environmentVariables,
        Help = _help,
        MachineReadable = _machineReadable,
        NoTty = _noTty,
        Timestamp = _timestamp,
        Version = _version,
        OnStdOut = _onStdOut,
        OnStdErr = _onStdErr
    };
}
