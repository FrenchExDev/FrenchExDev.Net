using FrenchExDev.Net.CSharp.Object.Builder2;

namespace FrenchExDev.Net.Vagrant.Commands.Builders;

public class BoxAddCommandBuilder : BoxCommandBuilder<BoxAddCommandBuilder, BoxAddCommand>
{
    private string? _nameUrlOrPath;
    private string? _provider;
    private bool? _force;
    private string? _checksum;
    private string? _checksumType;
    private string? _boxVersion;
    private string? _architecture;
    private bool? _insecure;
    private string? _cacert;
    private string? _capath;
    private string? _cert;
    private bool? _locationTrusted;

    /// <summary>
    /// Specifies the URL or file system path to use for the box add command.
    /// </summary>
    /// <param name="url">The URL or file system path that identifies the resource to be added. Cannot be null.</param>
    /// <returns>The current <see cref="BoxAddCommandBuilder"/> instance to allow method chaining.</returns>
    public BoxAddCommandBuilder UrlOrPath(string url)
    {
        _nameUrlOrPath = url;
        return this;
    }

    public BoxAddCommandBuilder Provider(string provider)
    {
        _provider = provider;
        return this;
    }

    public BoxAddCommandBuilder Force(bool? force = true)
    {
        _force = force;
        return this;
    }

    public BoxAddCommandBuilder Checksum(string checksum)
    {
        _checksum = checksum;
        return this;
    }

    public BoxAddCommandBuilder ChecksumType(string type)
    {
        _checksumType = type;
        return this;
    }

    public BoxAddCommandBuilder BoxVersion(string version)
    {
        _boxVersion = version;
        return this;
    }

    public BoxAddCommandBuilder Architecture(string arch)
    {
        _architecture = arch;
        return this;
    }

    public BoxAddCommandBuilder Insecure(bool? insecure = true)
    {
        _insecure = insecure;
        return this;
    }

    public BoxAddCommandBuilder CaCert(string file)
    {
        _cacert = file;
        return this;
    }

    public BoxAddCommandBuilder CaPath(string dir)
    {
        _capath = dir;
        return this;
    }

    public BoxAddCommandBuilder Cert(string file)
    {
        _cert = file;
        return this;
    }

    public BoxAddCommandBuilder LocationTrusted(bool? trusted = true)
    {
        _locationTrusted = trusted;
        return this;
    }

    public static readonly string[] ChecksumTypes = ["sha1", "sha256", "sha512", "md5"];

    protected override void ValidateInternal(VisitedObjectDictionary visitedCollector, FailuresDictionary failures)
    {
        base.ValidateInternal(visitedCollector, failures);

        // url/path (source) is required
        if (string.IsNullOrWhiteSpace(_nameUrlOrPath))
            failures.Failure(nameof(BoxAddCommand.NameOrUrlOrPath), new InvalidDataException("Missing required parameter 'source' (name, url or path)."));

        // provider option must not be empty when provided
        if (_provider is not null && string.IsNullOrWhiteSpace(_provider))
            failures.Failure(nameof(BoxAddCommand.Provider), new InvalidDataException("--provider cannot be empty"));

        // box-version if present must not be empty
        if (_boxVersion is not null && string.IsNullOrWhiteSpace(_boxVersion))
            failures.Failure(nameof(BoxAddCommand.BoxVersion), new InvalidDataException("--box-version cannot be empty"));

        // architecture if present must not be empty
        if (_architecture is not null && string.IsNullOrWhiteSpace(_architecture))
            failures.Failure(nameof(BoxAddCommand.Architecture), new InvalidDataException("--architecture cannot be empty"));

        // SSL options must not be empty when provided
        if (_cacert is not null && string.IsNullOrWhiteSpace(_cacert))
            failures.Failure(nameof(BoxAddCommand.CaCert), new InvalidDataException("--cacert cannot be empty"));
        if (_capath is not null && string.IsNullOrWhiteSpace(_capath))
            failures.Failure(nameof(BoxAddCommand.CaPath), new InvalidDataException("--capath cannot be empty"));
        if (_cert is not null && string.IsNullOrWhiteSpace(_cert))
            failures.Failure(nameof(BoxAddCommand.Cert), new InvalidDataException("--cert cannot be empty"));

        // checksum rules
        if (_checksum is not null && string.IsNullOrWhiteSpace(_checksum))
            failures.Failure(nameof(BoxAddCommand.Checksum), new InvalidDataException("--checksum cannot be empty"));

        if (_checksumType is not null)
        {
            if (string.IsNullOrWhiteSpace(_checksumType))
            {
                failures.Failure(nameof(BoxAddCommand.ChecksumType), new InvalidDataException("--checksum-type cannot be empty"));
            }
            else
            {
                if (!ChecksumTypes.Contains(_checksumType, StringComparer.OrdinalIgnoreCase))
                    failures.Failure(nameof(BoxAddCommand.ChecksumType), new InvalidDataException("Unsupported checksum type (allowed: sha1, sha256, sha512, md5)."));
                if (string.IsNullOrWhiteSpace(_checksum))
                    failures.Failure(nameof(BoxAddCommand.ChecksumType), new InvalidDataException("--checksum-type requires --checksum"));
            }
        }
    }

    protected override BoxAddCommand Instantiate()
    {
        return new BoxAddCommand(_nameUrlOrPath ?? throw new InvalidDataException(nameof(_nameUrlOrPath)))
        {
            Force = _force,
            Checksum = _checksum,
            ChecksumType = _checksumType,
            BoxVersion = _boxVersion,
            Architecture = _architecture,
            Insecure = _insecure,
            CaCert = _cacert,
            CaPath = _capath,
            Cert = _cert,
            LocationTrusted = _locationTrusted,
            WorkingDirectory = _workingDirectory,
            NoColor = _noColor == true ? false : null,
            MachineReadable = _machineReadable,
            Version = _version,
            Debug = _debug,
            Timestamp = _timestamp,
            DebugTimestamp = _debugTimestamp,
            NoTty = _noTty,
            Provider = _provider,
            Help = _help,
            EnvironmentVariables = _environmentVariables,
            OnStdOut = _onStdOut,
            OnStdErr = _onStdErr
        };
    }
}
