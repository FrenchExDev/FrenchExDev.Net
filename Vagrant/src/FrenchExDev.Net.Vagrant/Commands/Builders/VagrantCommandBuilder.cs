using FrenchExDev.Net.CSharp.Object.Builder2;

namespace FrenchExDev.Net.Vagrant.Commands.Builders;

public abstract class VagrantCommandBuilder<TBuilder, TCommand> : AbstractBuilder<TCommand>
    where TCommand : VagrantCommandBase
    where TBuilder : VagrantCommandBuilder<TBuilder, TCommand>
{
    protected string? _workingDirectory;
    protected bool? _noColor;
    protected bool? _machineReadable;
    protected bool? _version;
    protected bool? _debug;
    protected bool? _timestamp;
    protected bool? _debugTimestamp;
    protected bool? _noTty;
    protected bool? _help;
    protected Dictionary<string, string> _environmentVariables = new();

    public TBuilder EnvironmentVariable(string name, string value)
    {
        _environmentVariables[name] = value;
        return this as TBuilder ?? throw new InvalidCastException(nameof(TBuilder));
    }

    public TBuilder Color(bool? with = true)
    {
        _noColor = with is false ? false : null;
        return this as TBuilder ?? throw new InvalidCastException(nameof(TBuilder));
    }

    public TBuilder NoColor(bool? noColor = true)
    {
        _noColor = noColor;
        return this as TBuilder ?? throw new InvalidCastException(nameof(TBuilder));
    }

    public TBuilder MachineReadable(bool? machineReadable = true)
    {
        _machineReadable = machineReadable;
        return this as TBuilder ?? throw new InvalidCastException(nameof(TBuilder));
    }

    public TBuilder Version(bool? version = true)
    {
        _version = version;
        return this as TBuilder ?? throw new InvalidCastException(nameof(TBuilder));
    }

    public TBuilder Debug(bool? debug = true)
    {
        _debug = debug;
        return this as TBuilder ?? throw new InvalidCastException(nameof(TBuilder));
    }

    public TBuilder Timestamp(bool? timestamp = true)
    {
        _timestamp = timestamp;
        return this as TBuilder ?? throw new InvalidCastException(nameof(TBuilder));
    }

    public TBuilder DebugTimestamp(bool? debugTimestamp = true)
    {
        _debugTimestamp = debugTimestamp;
        return this as TBuilder ?? throw new InvalidCastException(nameof(TBuilder));
    }

    public TBuilder NoTty(bool? noTty = true)
    {
        _noTty = noTty;
        return this as TBuilder ?? throw new InvalidCastException(nameof(TBuilder));
    }

    public TBuilder Help(bool? help = true)
    {
        _help = help;
        return this as TBuilder ?? throw new InvalidCastException(nameof(TBuilder));
    }

    public TBuilder WorkingDirectory(string wd)
    {
        _workingDirectory = wd;
        return this as TBuilder ?? throw new InvalidCastException(nameof(TBuilder));
    }

    protected override void ValidateInternal(VisitedObjectDictionary visitedCollector, FailuresDictionary failures)
    {
        if (string.IsNullOrWhiteSpace(_workingDirectory))
        {
            failures.Failure(nameof(WorkingDirectory), new InvalidDataException("Working directory must be set and not empty or whitespace."));
        }
    }
}
