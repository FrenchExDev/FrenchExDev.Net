using FrenchExDev.Net.CSharp.Object.Builder2;

namespace FrenchExDev.Net.Vagrant.Commands.Builders;

public class BoxPruneCommandBuilder : BoxCommandBuilder<BoxPruneCommandBuilder, BoxPruneCommand>
{
    private bool? _dryRun;
    private bool? _keepActiveProvider;
    private bool? _force;

    public BoxPruneCommandBuilder DryRun(bool? dry = true)
    {
        _dryRun = dry;
        return this;
    }

    public BoxPruneCommandBuilder KeepActiveProvider(bool? keep = true)
    {
        _keepActiveProvider = keep;
        return this;
    }

    public BoxPruneCommandBuilder Force(bool? force = true)
    {
        _force = force;
        return this;
    }

    protected override void ValidateInternal(VisitedObjectDictionary visitedCollector, FailuresDictionary failures)
    {
        base.ValidateInternal(visitedCollector, failures);
        if (failures.Count > 0) return;
        // no specific validations
    }

    protected override BoxPruneCommand Instantiate() => new BoxPruneCommand
    {
        DryRun = _dryRun,
        KeepActiveProvider = _keepActiveProvider,
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
        EnvironmentVariables = _environmentVariables
    };
}
