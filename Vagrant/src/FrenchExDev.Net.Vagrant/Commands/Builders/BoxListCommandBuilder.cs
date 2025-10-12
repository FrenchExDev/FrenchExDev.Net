namespace FrenchExDev.Net.Vagrant.Commands.Builders;

public class BoxListCommandBuilder : BoxCommandBuilder<BoxListCommandBuilder, BoxListCommand>
{
    private bool? _local;

    public BoxListCommandBuilder Local(bool? local = true)
    {
        _local = local;
        return this;
    }

    protected override BoxListCommand Instantiate()
    {
        return new BoxListCommand
        {
            Local = _local,
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
}
