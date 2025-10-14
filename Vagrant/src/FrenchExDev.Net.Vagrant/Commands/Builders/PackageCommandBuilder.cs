using FrenchExDev.Net.CSharp.Object.Builder2;

namespace FrenchExDev.Net.Vagrant.Commands.Builders;

public class PackageCommandBuilder : VagrantCommandBuilder<PackageCommandBuilder, PackageCommand>
{
    private string? _nameOrId;
    private string? _output;
    private string? _base;
    private List<string> _include = new();
    private string? _vagrantfile;

    public PackageCommandBuilder NameOrId(string nameOrId)
    {
        _nameOrId = nameOrId;
        return this;
    }

    public PackageCommandBuilder Output(string file)
    {
        _output = file;
        return this;
    }

    public PackageCommandBuilder Base(string name)
    {
        _base = name;
        return this;
    }

    public PackageCommandBuilder Include(string file)
    {
        _include.Add(file);
        return this;
    }

    public PackageCommandBuilder Vagrantfile(string file)
    {
        _vagrantfile = file;
        return this;
    }

    protected override void ValidateInternal(VisitedObjectDictionary visitedCollector, FailuresDictionary failures)
    {
        base.ValidateInternal(visitedCollector, failures);
    }

    protected override PackageCommand Instantiate() => new PackageCommand
    {
        Output = _output,
        Base = _base,
        Include = _include.Any() ? _include : null,
        Vagrantfile = _vagrantfile,
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
        OnStdOut = _onStdOut,
        OnStdErr = _onStdErr
    };
}
