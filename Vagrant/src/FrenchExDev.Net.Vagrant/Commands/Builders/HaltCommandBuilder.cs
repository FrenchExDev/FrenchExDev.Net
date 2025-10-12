namespace FrenchExDev.Net.Vagrant.Commands.Builders;

public class HaltCommandBuilder : VagrantCommandBuilder<HaltCommandBuilder, HaltCommand>
{
    private string? _name;
    private bool? _force;

    public HaltCommandBuilder Name(string? name)
    {
        _name = name;
        return this;
    }

    public HaltCommandBuilder Force(bool? force = true)
    {
        _force = force;
        return this;
    }

    protected override HaltCommand Instantiate()
    {
        return new HaltCommand
        {
            Name = _name,
            Force = _force,
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
}
