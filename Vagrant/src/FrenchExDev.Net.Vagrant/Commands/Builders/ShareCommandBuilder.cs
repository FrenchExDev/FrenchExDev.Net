using FrenchExDev.Net.CSharp.Object.Builder2;

namespace FrenchExDev.Net.Vagrant.Commands.Builders;

public class ShareCommandBuilder : VagrantCommandBuilder<ShareCommandBuilder, ShareCommand>
{
    private string? _service;

    public ShareCommandBuilder Service(string s)
    {
        _service = s;
        return this;
    }

    protected override void ValidateInternal(VisitedObjectDictionary visitedCollector, FailuresDictionary failures)
    {
        base.ValidateInternal(visitedCollector, failures);

        if (_service is not null && string.IsNullOrWhiteSpace(_service))
            failures.Failure(nameof(ShareCommand.Service), new InvalidDataException("--service cannot be empty"));
    }

    protected override ShareCommand Instantiate() => new ShareCommand
    {
        Service = _service,
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
