using FrenchExDev.Net.CSharp.Object.Builder2;

namespace FrenchExDev.Net.Vagrant.Commands.Builders;

public class InitCommandBuilder : VagrantCommandBuilder<InitCommandBuilder, InitCommand>
{
    private string? _nameOrUrl;
    private string? _output;
    private bool? _minimal;
    private bool? _force;
    private string? _boxVersion;
    private string? _template;

    public InitCommandBuilder Template(string template)
    {
        _template = template;
        return this;
    }

    public InitCommandBuilder BoxVersion(string boxVersion)
    {
        _boxVersion = boxVersion;
        return this;
    }

    public InitCommandBuilder NameOrUrl(string nameOrUrl)
    {
        _nameOrUrl = nameOrUrl;
        return this;
    }

    public InitCommandBuilder Output(string file)
    {
        _output = file;
        return this;
    }

    public InitCommandBuilder Minimal(bool? minimal = true)
    {
        _minimal = minimal;
        return this;
    }

    public InitCommandBuilder Force(bool? force = true)
    {
        _force = force;
        return this;
    }

    protected override void ValidateInternal(VisitedObjectDictionary visitedCollector, FailuresDictionary failures)
    {
        base.ValidateInternal(visitedCollector, failures);
        if (_nameOrUrl is not null && string.IsNullOrWhiteSpace(_nameOrUrl))
            failures.Failure(nameof(InitCommand.NameOrUrl), new InvalidDataException("--box cannot be empty"));
        if (_output is not null && string.IsNullOrWhiteSpace(_output))
            failures.Failure(nameof(InitCommand.Output), new InvalidDataException("--output cannot be empty"));
    }

    protected override InitCommand Instantiate()
    {
        return new InitCommand
        {
            NameOrUrl = _nameOrUrl,
            Output = _output,
            Minimal = _minimal,
            Force = _force,
            WorkingDirectory = _workingDirectory,
            NoColor = _noColor == true ? false : null,
            MachineReadable = _machineReadable,
            Version = _version,
            Debug = _debug,
            Timestamp = _timestamp,
            DebugTimestamp = _debugTimestamp,
            NoTty = _noTty,
            EnvironmentVariables = _environmentVariables,
            Help = _help,
            OnStdErr = _onStdErr,
            OnStdOut = _onStdOut
        };
    }
}
