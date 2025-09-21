using FrenchExDev.Net.Alpine.Version;
using FrenchExDev.Net.CSharp.Object.Builder2;

namespace FrenchExDev.Net.Packer.Alpine.Abstractions;

/// <summary>
/// Provides a builder for configuring and creating an AlpinePackerVagrantBundleCommand, which defines the parameters
/// required to generate an Alpine Linux Vagrant box using Packer and VirtualBox.
/// </summary>
/// <remarks>Use this builder to specify all necessary options for the Vagrant box, such as disk size, memory, CPU
/// count, box version, ISO details, and more. All required parameters must be set before building the command; missing
/// or invalid values will result in validation errors. This class is not thread-safe and should not be shared between
/// threads.</remarks>
public class AlpinePackerVagrantBundleCommandBuilder : AbstractBuilder<AlpinePackerVagrantBundleCommand>
{
    // Directory where .box files will be output by Packer's Vagrant post-processor
    private string? _outputVagrant;

    // Size of the disk to be used for the VM (e.g., "20GiB")
    private string? _diskSize;

    // Amount of memory allocated to the VM (in MB)
    private string? _memory;

    // Number of CPUs allocated to the VM
    private string? _cpus;

    // Version of the box to be created
    private string? _boxVersion;

    // Version of VirtualBox to be used
    private string? _virtualBoxVersion;

    // Type of checksum used for the ISO file (e.g., "sha256")
    private string? _isoChecksumType;

    // Checksum value for the ISO file
    private string? _isoChecksum;

    // URL to download the ISO file from the internet
    private string? _isoDownloadUrl;

    // Local path to the ISO file
    private string? _isoLocalUrl;

    // SHA256 checksum for the VirtualBox Guest Additions ISO file
    private string? _virtualBoxGuestAdditionsIsoSha256;

    // Name of the virtual machine
    private string? _vmName;

    // Amount of video memory allocated to the VM
    private string? _videoMemory;

    // URL of the Alpine community repository
    private string? _communityRepository;

    // Alpine Linux version to be used for the VM
    private AlpineVersion? _alpineVersion;

    /// <summary>
    /// Sets the Alpine Linux version to use for the Vagrant bundle command.
    /// </summary>
    /// <param name="version">The Alpine Linux version to be applied to the bundle command. Cannot be null.</param>
    /// <returns>The current instance of <see cref="AlpinePackerVagrantBundleCommandBuilder"/> with the specified version set.</returns>
    public AlpinePackerVagrantBundleCommandBuilder Version(AlpineVersion version)
    {
        _alpineVersion = version;
        return this;
    }

    /// <summary>
    /// Sets the Alpine Linux version to use for the Vagrant bundle command.
    /// </summary>
    /// <param name="version">The version string representing the desired Alpine Linux release. Must not be null or empty.</param>
    /// <returns>The current instance of <see cref="AlpinePackerVagrantBundleCommandBuilder"/> with the specified version
    /// applied.</returns>
    public AlpinePackerVagrantBundleCommandBuilder Version(string version)
    {
        _alpineVersion = AlpineVersion.From(version);
        return this;
    }

    /// <summary>
    /// Specifies the output path for the generated Vagrant box file.
    /// </summary>
    /// <param name="value">The file path where the Vagrant box will be written. Cannot be null or empty.</param>
    /// <returns>The current instance of <see cref="AlpinePackerVagrantBundleCommandBuilder"/> to allow method chaining.</returns>
    public AlpinePackerVagrantBundleCommandBuilder OutputVagrant(string value)
    {
        _outputVagrant = value;
        return this;
    }

    /// <summary>
    /// Sets the URL of the community package repository to be used for the Alpine Packer Vagrant bundle.
    /// </summary>
    /// <param name="value">The URL of the community repository. This value determines where community packages will be sourced during
    /// bundle creation.</param>
    /// <returns>The current instance of <see cref="AlpinePackerVagrantBundleCommandBuilder"/> to allow method chaining.</returns>
    public AlpinePackerVagrantBundleCommandBuilder CommunityRepository(string value)
    {
        _communityRepository = value;
        return this;
    }

    /// <summary>
    /// Sets the disk size for the Vagrant box image configuration.
    /// </summary>
    /// <remarks>This method enables fluent configuration of the Vagrant box image by allowing multiple
    /// settings to be specified in a single statement. Ensure that the disk size value is valid for the target provider
    /// to avoid provisioning errors.</remarks>
    /// <param name="value">The disk size value to assign, typically specified in megabytes or gigabytes (e.g., "10240MB" or "10GB"). The
    /// format and units must be compatible with the underlying Vagrant provider requirements.</param>
    /// <returns>The current instance of <see cref="AlpinePackerVagrantBundleCommandBuilder"/> to allow method chaining.</returns>
    public AlpinePackerVagrantBundleCommandBuilder DiskSize(string value)
    {
        _diskSize = value;
        return this;
    }

    /// <summary>
    /// Sets the amount of memory to allocate for the virtual machine configuration.
    /// </summary>
    /// <param name="value">The memory size to assign, specified as a string. The format should match the requirements of the underlying
    /// virtualization provider (for example, "1024" for megabytes).</param>
    /// <returns>The current instance of <see cref="AlpinePackerVagrantBundleCommandBuilder"/> to allow method chaining.</returns>
    public AlpinePackerVagrantBundleCommandBuilder Memory(string value)
    {
        _memory = value;
        return this;
    }

    /// <summary>
    /// Sets the number of CPUs to allocate for the Vagrant virtual machine configuration.
    /// </summary>
    /// <param name="value">The number of CPUs to assign, specified as a string. This value should represent a positive integer.</param>
    /// <returns>The current instance of <see cref="AlpinePackerVagrantBundleCommandBuilder"/> to allow method chaining.</returns>
    public AlpinePackerVagrantBundleCommandBuilder Cpus(string value)
    {
        _cpus = value;
        return this;
    }

    /// <summary>
    /// Sets the version of the Vagrant box to be used in the bundle command.
    /// </summary>
    /// <param name="value">The version string to assign to the Vagrant box. Cannot be null or empty.</param>
    /// <returns>The current instance of <see cref="AlpinePackerVagrantBundleCommandBuilder"/> to allow method chaining.</returns>
    public AlpinePackerVagrantBundleCommandBuilder BoxVersion(string value)
    {
        _boxVersion = value;
        return this;
    }

    /// <summary>
    /// Sets the required VirtualBox version for the Vagrant bundle configuration.
    /// </summary>
    /// <param name="value">The version string specifying the VirtualBox version to use. This value should match the format expected by
    /// Vagrant and VirtualBox (for example, "6.1.36").</param>
    /// <returns>The current instance of <see cref="AlpinePackerVagrantBundleCommandBuilder"/> to allow method chaining.</returns>
    public AlpinePackerVagrantBundleCommandBuilder VirtualBoxVersion(string value)
    {
        _virtualBoxVersion = value;
        return this;
    }

    /// <summary>
    /// Sets the ISO checksum type to use for validating the ISO image in the Vagrant bundle command.
    /// </summary>
    /// <remarks>Specify the checksum type that matches the format of the ISO checksum value provided. This
    /// method enables fluent configuration of the bundle command.</remarks>
    /// <param name="value">The checksum algorithm type to apply, such as "md5", "sha1", or "sha256". Cannot be null or empty.</param>
    /// <returns>The current instance of <see cref="AlpinePackerVagrantBundleCommandBuilder"/> to allow method chaining.</returns>
    public AlpinePackerVagrantBundleCommandBuilder IsoChecksumType(string value)
    {
        _isoChecksumType = value;
        return this;
    }

    /// <summary>
    /// Sets the ISO image checksum to be used for validating the downloaded ISO file.
    /// </summary>
    /// <param name="value">The checksum value for the ISO image. This should match the expected hash of the ISO file to ensure its
    /// integrity. Cannot be null.</param>
    /// <returns>The current instance of <see cref="AlpinePackerVagrantBundleCommandBuilder"/> to allow method chaining.</returns>
    public AlpinePackerVagrantBundleCommandBuilder IsoChecksum(string value)
    {
        _isoChecksum = value;
        return this;
    }

    /// <summary>
    /// Sets the URL from which the Alpine ISO image will be downloaded for the Vagrant bundle build process.
    /// </summary>
    /// <param name="value">The URL to use for downloading the Alpine ISO image. Must be a valid, accessible HTTP or HTTPS URL.</param>
    /// <returns>The current instance of <see cref="AlpinePackerVagrantBundleCommandBuilder"/> to allow method chaining.</returns>
    public AlpinePackerVagrantBundleCommandBuilder IsoDownloadUrl(string value)
    {
        _isoDownloadUrl = value;
        return this;
    }

    /// <summary>
    /// Sets the local URL of the ISO image to be used for the Vagrant bundle command.
    /// </summary>
    /// <param name="value">The local file system path or URL of the ISO image. Cannot be null.</param>
    /// <returns>The current instance of <see cref="AlpinePackerVagrantBundleCommandBuilder"/> to allow method chaining.</returns>
    public AlpinePackerVagrantBundleCommandBuilder IsoLocalUrl(string value)
    {
        _isoLocalUrl = value;
        return this;
    }

    /// <summary>
    /// Sets the SHA-256 checksum for the VirtualBox Guest Additions ISO file to be used in the bundle configuration.
    /// </summary>
    /// <param name="value">The SHA-256 checksum string that verifies the integrity of the VirtualBox Guest Additions ISO file. This value
    /// should be a valid SHA-256 hash.</param>
    /// <returns>The current instance of <see cref="AlpinePackerVagrantBundleCommandBuilder"/> to allow method chaining.</returns>
    public AlpinePackerVagrantBundleCommandBuilder VirtualBoxGuestAdditionsIsoSha256(string value)
    {
        _virtualBoxGuestAdditionsIsoSha256 = value;
        return this;
    }

    /// <summary>
    /// Sets the name of the virtual machine to be used in the Vagrant bundle configuration.
    /// </summary>
    /// <param name="value">The name to assign to the virtual machine. Cannot be null or empty.</param>
    /// <returns>The current instance of <see cref="AlpinePackerVagrantBundleCommandBuilder"/> to allow method chaining.</returns>
    public AlpinePackerVagrantBundleCommandBuilder VmName(string value)
    {
        _vmName = value;
        return this;
    }

    /// <summary>
    /// Sets the amount of video memory to allocate for the virtual machine configuration.
    /// </summary>
    /// <param name="value">The video memory value to assign, typically specified in megabytes. Cannot be null or empty.</param>
    /// <returns>The current instance of <see cref="AlpinePackerVagrantBundleCommandBuilder"/> to allow method chaining.</returns>
    public AlpinePackerVagrantBundleCommandBuilder VideoMemory(string value)
    {
        _videoMemory = value;
        return this;
    }

    /// <summary>
    /// Performs validation of the object's configuration and records any validation failures.
    /// </summary>
    /// <param name="visitedCollector">A dictionary used to track objects that have already been visited during validation to prevent redundant checks.</param>
    /// <param name="failures">A dictionary for collecting validation failures encountered during the validation process. Entries are added for
    /// each failed validation check.</param>
    protected override void ValidateInternal(VisitedObjectDictionary visitedCollector, FailuresDictionary failures)
    {
        static Exception exceptionBuilder(string s) => new InvalidDataException(s);
        AssertNotEmptyOrWhitespace(_outputVagrant, nameof(_outputVagrant), failures, exceptionBuilder);
        AssertNotEmptyOrWhitespace(_diskSize, nameof(_diskSize), failures, exceptionBuilder);
        AssertNotEmptyOrWhitespace(_memory, nameof(_memory), failures, exceptionBuilder);
        AssertNotEmptyOrWhitespace(_cpus, nameof(_cpus), failures, exceptionBuilder);
        AssertNotEmptyOrWhitespace(_boxVersion, nameof(_boxVersion), failures, exceptionBuilder);
        AssertNotEmptyOrWhitespace(_virtualBoxVersion, nameof(_virtualBoxVersion), failures, exceptionBuilder);
        AssertNotEmptyOrWhitespace(_isoChecksumType, nameof(_isoChecksumType), failures, exceptionBuilder);
        AssertNotEmptyOrWhitespace(_isoChecksum, nameof(_isoChecksum), failures, exceptionBuilder);
        AssertNotEmptyOrWhitespace(_isoDownloadUrl, nameof(_isoDownloadUrl), failures, exceptionBuilder);
        AssertNotEmptyOrWhitespace(_isoLocalUrl, nameof(_isoLocalUrl), failures, exceptionBuilder);
        AssertNotEmptyOrWhitespace(_virtualBoxGuestAdditionsIsoSha256, nameof(_virtualBoxGuestAdditionsIsoSha256), failures, exceptionBuilder);
        AssertNotEmptyOrWhitespace(_vmName, nameof(_vmName), failures, exceptionBuilder);
        AssertNotEmptyOrWhitespace(_videoMemory, nameof(_videoMemory), failures, exceptionBuilder);
        AssertNotNull(_alpineVersion, nameof(_alpineVersion), failures, exceptionBuilder);
    }

    /// <summary>
    /// Generates a new instance of <see cref="AlpinePackerVagrantBundleCommand"/> using the configured properties.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="InvalidDataException"></exception>
    protected override AlpinePackerVagrantBundleCommand Instantiate() => new()
    {
        OutputVagrant = _outputVagrant ?? throw new InvalidDataException(nameof(_outputVagrant)),
        DiskSize = _diskSize ?? throw new InvalidDataException(nameof(_diskSize)),
        Memory = _memory ?? throw new InvalidDataException(nameof(_memory)),
        Cpus = _cpus ?? throw new InvalidDataException(nameof(_cpus)),
        BoxVersion = _boxVersion ?? throw new InvalidDataException(nameof(_boxVersion)),
        VirtualBoxVersion = _virtualBoxVersion ?? throw new InvalidDataException(nameof(_virtualBoxVersion)),
        IsoChecksumType = _isoChecksumType ?? throw new InvalidDataException(nameof(_isoChecksum)),
        IsoChecksum = _isoChecksum ?? throw new InvalidDataException(nameof(_isoChecksum)),
        IsoDownloadUrl = _isoDownloadUrl ?? throw new InvalidDataException(nameof(_isoDownloadUrl)),
        IsoLocalUrl = _isoLocalUrl ?? throw new InvalidDataException(nameof(_isoLocalUrl)),
        VirtualBoxGuestAdditionsIsoSha256 = _virtualBoxGuestAdditionsIsoSha256 ?? throw new InvalidDataException(nameof(_virtualBoxGuestAdditionsIsoSha256)),
        VmName = _vmName ?? throw new InvalidDataException(nameof(_vmName)),
        VideoMemory = _videoMemory ?? throw new InvalidDataException(nameof(_videoMemory)),
        CommunityRepository = _communityRepository ?? throw new InvalidDataException(nameof(_communityRepository)),
        AlpineVersion = _alpineVersion ?? throw new InvalidDataException(nameof(_alpineVersion)),
    };
}
