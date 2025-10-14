using FrenchExDev.Net.CSharp.Object.Builder2;

namespace FrenchExDev.Net.Vagrant.Commands.Builders;

public class ReloadCommandBuilder : VagrantCommandBuilder<ReloadCommandBuilder, ReloadCommand>
{
    private bool? _provision;
    private bool? _noProvision;
    private string? _provider;

    public ReloadCommandBuilder Provision(bool? provision = true)
    {
        _provision = provision;
        return this;
    }

    public ReloadCommandBuilder NoProvision(bool? noProvision = true)
    {
        _noProvision = noProvision;
        return this;
    }

    public ReloadCommandBuilder Provider(string provider)
    {
        _provider = provider;
        return this;
    }

    protected override void ValidateInternal(VisitedObjectDictionary visitedCollector, FailuresDictionary failures)
    {
        base.ValidateInternal(visitedCollector, failures);

        if (_provision == true && _noProvision == true)
            failures.Failure(nameof(_provision), new InvalidDataException("Options --provision and --no-provision are mutually exclusive."));
        if (_provider is not null && string.IsNullOrWhiteSpace(_provider))
            failures.Failure(nameof(_provider), new InvalidDataException("--provider cannot be empty"));
    }

    protected override ReloadCommand Instantiate() => new ReloadCommand
    {
        Provision = _provision,
        NoProvision = _noProvision,
        Provider = _provider,
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
