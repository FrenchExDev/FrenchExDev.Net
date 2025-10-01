#region Licensing

// Copyright Stéphane Erard
// For licensing, please contact stephane.erard@gmail

#endregion

#region Usings

using FrenchExDev.Net.CSharp.Object.Builder2;

#endregion

namespace FrenchExDev.Net.Packer.Bundle;

/// <summary>
/// Provides a fluent builder for configuring and constructing instances of <see cref="PackerBuilder"/> for VirtualBox-based Packer builds.
/// </summary>
/// <remarks>
/// Use this builder to specify required and optional settings for a Packer VirtualBox build, such as boot commands, disk size, ISO sources, guest additions, SSH credentials, and VirtualBox-specific options.
/// Each configuration method returns the builder instance, allowing for method chaining. The builder validates that all mandatory properties are set before constructing the final <see cref="PackerBuilder"/> object.
/// This class is not thread-safe.
/// <example>
/// <code>
/// var builder = new PackerBuilderBuilder()
///     .BootWait("10s")
///     .AddBootCommand("<wait>")
///     .DiskSize("20000")
///     .Format("ova")
///     .VmName("alpine-vm");
/// var result = builder.Build();
/// </code>
/// </example>
/// </remarks>
public class PackerBuilderBuilder : AbstractBuilder<PackerBuilder>
{
    /// <summary>
    /// List of boot commands to send to the VM during initial boot.
    /// </summary>
    /// <remarks>Each command is sent in sequence. Example: "<wait>".</remarks>
    private List<string>? _bootCommand;
    /// <summary>
    /// Time to wait before sending boot commands (e.g., "10s", "1m").
    /// </summary>
    private string? _bootWait;
    /// <summary>
    /// Communicator type (e.g., "ssh", "winrm").
    /// </summary>
    private string? _communicator;
    /// <summary>
    /// Disk size in MB (e.g., "20000").
    /// </summary>
    private string? _diskSize;
    /// <summary>
    /// Output format for the VM (e.g., "ova", "vdi").
    /// </summary>
    private string? _format;
    /// <summary>
    /// Guest additions installation mode (e.g., "upload", "iso").
    /// </summary>
    private string? _guestAdditionMode;
    /// <summary>
    /// Path to guest additions ISO or installer.
    /// </summary>
    private string? _guestAdditionPath;
    /// <summary>
    /// SHA256 checksum for guest additions ISO.
    /// </summary>
    private string? _guestAdditionSha256;
    /// <summary>
    /// URL to download guest additions ISO.
    /// </summary>
    private string? _guestAdditionUrl;
    /// <summary>
    /// Guest OS type (e.g., "Linux_64", "Windows10_64").
    /// </summary>
    private string? _guestOsType;
    /// <summary>
    /// Whether to discard hard drive changes after shutdown.
    /// </summary>
    private bool? _hardDriveDiscard;
    /// <summary>
    /// Hard drive interface type (e.g., "sata", "ide").
    /// </summary>
    private string? _hardDriveInterface;
    /// <summary>
    /// Whether to run the VM in headless mode (no GUI).
    /// </summary>
    private bool? _headless;
    /// <summary>
    /// Directory to serve files over HTTP during build.
    /// </summary>
    private string? _httpDirectory;
    /// <summary>
    /// ISO checksum for validating the downloaded ISO file.
    /// </summary>
    private string? _isoChecksum;
    /// <summary>
    /// List of ISO URLs to download the installation ISO from.
    /// </summary>
    private List<string>? _isoUrls;
    /// <summary>
    /// Whether to keep the VM registered after build.
    /// </summary>
    private bool? _keepRegistered;
    /// <summary>
    /// Command to shut down the VM after provisioning.
    /// </summary>
    private string? _shutdownCommand;
    /// <summary>
    /// SSH password for connecting to the VM.
    /// </summary>
    private string? _sshPassword;
    /// <summary>
    /// SSH connection timeout (e.g., "10m").
    /// </summary>
    private string? _sshTimeout;
    /// <summary>
    /// SSH username for connecting to the VM.
    /// </summary>
    private string? _sshUsername;
    /// <summary>
    /// Builder type (e.g., "virtualbox-iso").
    /// </summary>
    private string? _type;
    /// <summary>
    /// List of VBoxManage commands to run for VM customization.
    /// </summary>
    /// <remarks>Each inner list represents a command and its arguments.</remarks>
    private List<List<string>>? _vboxManage;
    /// <summary>
    /// Path to the VirtualBox version file.
    /// </summary>
    private string? _virtualBoxVersionFile;
    /// <summary>
    /// Name of the VM to create.
    /// </summary>
    private string? _vmName;

    /// <summary>
    /// Sets whether to discard hard drive changes after shutdown.
    /// </summary>
    /// <param name="discard">True to discard changes; false to keep them.</param>
    /// <returns>The builder instance.</returns>
    public PackerBuilderBuilder HardDriveDiscard(bool? discard = true)
    {
        _hardDriveDiscard = discard;
        return this;
    }

    /// <summary>
    /// Sets the hard drive interface type.
    /// </summary>
    /// <param name="iface">Interface type (e.g., "sata").</param>
    /// <returns>The builder instance.</returns>
    public PackerBuilderBuilder HardDriveInterface(string iface)
    {
        _hardDriveInterface = iface;
        return this;
    }

    /// <summary>
    /// Adds a boot command to the list of boot commands.
    /// </summary>
    /// <param name="value">Boot command string (e.g., "<wait>").</param>
    /// <returns>The builder instance.</returns>
    public PackerBuilderBuilder AddBootCommand(string value)
    {
        if (_bootCommand is null) _bootCommand = new List<string>();
        _bootCommand.Add(value);
        return this;
    }

    /// <summary>
    /// Sets the boot wait time before sending boot commands.
    /// </summary>
    /// <param name="value">Wait time (e.g., "10s").</param>
    /// <returns>The builder instance.</returns>
    public PackerBuilderBuilder BootWait(string? value)
    {
        _bootWait = value;
        return this;
    }

    /// <summary>
    /// Sets the communicator type for VM provisioning.
    /// </summary>
    /// <param name="value">Communicator type (e.g., "ssh").</param>
    /// <returns>The builder instance.</returns>
    public PackerBuilderBuilder Communicator(string? value)
    {
        _communicator = value;
        return this;
    }

    /// <summary>
    /// Sets the disk size for the VM.
    /// </summary>
    /// <param name="value">Disk size in MB (e.g., "20000").</param>
    /// <returns>The builder instance.</returns>
    public PackerBuilderBuilder DiskSize(string? value)
    {
        _diskSize = value;
        return this;
    }

    /// <summary>
    /// Sets the output format for the VM.
    /// </summary>
    /// <param name="value">Format (e.g., "ova").</param>
    /// <returns>The builder instance.</returns>
    public PackerBuilderBuilder Format(string? value)
    {
        _format = value;
        return this;
    }

    /// <summary>
    /// Sets the guest OS type for the VM.
    /// </summary>
    /// <param name="value">Guest OS type (e.g., "Linux_64").</param>
    /// <returns>The builder instance.</returns>
    public PackerBuilderBuilder GuestOsType(string? value)
    {
        _guestOsType = value;
        return this;
    }

    /// <summary>
    /// Sets whether to run the VM in headless mode.
    /// </summary>
    /// <param name="value">True for headless mode; false for GUI.</param>
    /// <returns>The builder instance.</returns>
    public PackerBuilderBuilder Headless(bool value)
    {
        _headless = value;
        return this;
    }

    /// <summary>
    /// Sets the HTTP directory to serve files during build.
    /// </summary>
    /// <param name="value">Directory path.</param>
    /// <returns>The builder instance.</returns>
    public PackerBuilderBuilder HttpDirectory(string? value)
    {
        _httpDirectory = value;
        return this;
    }

    /// <summary>
    /// Sets the ISO checksum for validating the installation ISO.
    /// </summary>
    /// <param name="value">Checksum string.</param>
    /// <returns>The builder instance.</returns>
    public PackerBuilderBuilder IsoChecksum(string? value)
    {
        _isoChecksum = value;
        return this;
    }

    /// <summary>
    /// Adds an ISO URL to the list of installation sources.
    /// </summary>
    /// <param name="value">ISO URL.</param>
    /// <returns>The builder instance.</returns>
    public PackerBuilderBuilder AddIsoUrl(string value)
    {
        if (_isoUrls is null) _isoUrls = new List<string>();
        _isoUrls.Add(value);
        return this;
    }

    /// <summary>
    /// Sets the URL for guest additions ISO.
    /// </summary>
    /// <param name="value">URL string.</param>
    /// <returns>The builder instance.</returns>
    public PackerBuilderBuilder GuestAdditionUrl(string? value)
    {
        _guestAdditionUrl = value;
        return this;
    }

    /// <summary>
    /// Sets the SHA256 checksum for guest additions ISO.
    /// </summary>
    /// <param name="value">Checksum string.</param>
    /// <returns>The builder instance.</returns>
    public PackerBuilderBuilder GuestAdditionSha256(string? value)
    {
        _guestAdditionSha256 = value;
        return this;
    }

    /// <summary>
    /// Sets the path for guest additions ISO or installer.
    /// </summary>
    /// <param name="value">Path string.</param>
    /// <returns>The builder instance.</returns>
    public PackerBuilderBuilder GuestAdditionPath(string? value)
    {
        _guestAdditionPath = value;
        return this;
    }

    /// <summary>
    /// Sets the guest additions installation mode.
    /// </summary>
    /// <param name="value">Mode string (e.g., "upload").</param>
    /// <returns>The builder instance.</returns>
    public PackerBuilderBuilder GuestAdditionMode(string? value)
    {
        _guestAdditionMode = value;
        return this;
    }

    /// <summary>
    /// Sets the path to the VirtualBox version file.
    /// </summary>
    /// <param name="value">File path.</param>
    /// <returns>The builder instance.</returns>
    public PackerBuilderBuilder VirtualBoxVersionFile(string? value)
    {
        _virtualBoxVersionFile = value;
        return this;
    }

    /// <summary>
    /// Sets whether to keep the VM registered after build.
    /// </summary>
    /// <param name="value">"true" or "false" as string.</param>
    /// <returns>The builder instance.</returns>
    public PackerBuilderBuilder KeepRegistered(bool? value)
    {
        _keepRegistered = value;
        return this;
    }

    /// <summary>
    /// Sets the shutdown command for the VM.
    /// </summary>
    /// <param name="value">Command string.</param>
    /// <returns>The builder instance.</returns>
    public PackerBuilderBuilder ShutdownCommand(string value)
    {
        _shutdownCommand = value;
        return this;
    }

    /// <summary>
    /// Sets the SSH password for VM access.
    /// </summary>
    /// <param name="value">Password string.</param>
    /// <returns>The builder instance.</returns>
    public PackerBuilderBuilder SshPassword(string? value)
    {
        _sshPassword = value;
        return this;
    }

    /// <summary>
    /// Sets the SSH connection timeout.
    /// </summary>
    /// <param name="value">Timeout string (e.g., "10m").</param>
    /// <returns>The builder instance.</returns>
    public PackerBuilderBuilder SshTimeout(string? value)
    {
        _sshTimeout = value;
        return this;
    }

    /// <summary>
    /// Sets the SSH username for VM access.
    /// </summary>
    /// <param name="value">Username string.</param>
    /// <returns>The builder instance.</returns>
    public PackerBuilderBuilder SshUsername(string? value)
    {
        _sshUsername = value;
        return this;
    }

    /// <summary>
    /// Sets the builder type (e.g., "virtualbox-iso").
    /// </summary>
    /// <param name="value">Type string.</param>
    /// <returns>The builder instance.</returns>
    public PackerBuilderBuilder Type(string? value)
    {
        _type = value;
        return this;
    }

    /// <summary>
    /// Adds a VBoxManage command to modify a storage controller.
    /// </summary>
    /// <param name="controllerName">Name of the storage controller.</param>
    /// <param name="key">Key argument for VBoxManage.</param>
    /// <param name="value">Value argument for VBoxManage.</param>
    /// <returns>The builder instance.</returns>
    /// <remarks>Example: ModifyStorageController("SATA Controller", "--add", "sata")</remarks>
    public PackerBuilderBuilder ModifyStorageController(string controllerName, string key, string value)
    {
        _vboxManage ??= new List<List<string>>();
        _vboxManage.Add(new List<string>()
        {
            "storagectl", "{{ .Name }}", "--name=" + controllerName, key, value
        });
        return this;
    }

    /// <summary>
    /// Conditionally adds a VBoxManage command to modify a storage controller.
    /// </summary>
    /// <param name="ifBody">Condition to check.</param>
    /// <param name="storageController">Name of the storage controller.</param>
    /// <param name="key">Key argument.</param>
    /// <param name="value">Value argument.</param>
    /// <returns>The builder instance.</returns>
    public PackerBuilderBuilder ModifyStorageControllerIf(Func<bool> ifBody, string storageController, string key, string value)
    {
        if (ifBody()) ModifyStorageController(storageController, key, value);
        return this;
    }

    /// <summary>
    /// Adds a VBoxManage command to attach storage to a controller.
    /// </summary>
    /// <param name="storageController">Name of the storage controller.</param>
    /// <param name="port">Port number.</param>
    /// <param name="key">Key argument.</param>
    /// <param name="value">Value argument.</param>
    /// <returns>The builder instance.</returns>
    public PackerBuilderBuilder ModifyStorageAttach(string storageController, int port, string key, string value)
    {
        _vboxManage ??= new List<List<string>>();
        _vboxManage.Add(new List<string>()
        {
            "storageattach", "{{ .Name }}", "--storagectl=" + storageController, "--port=" + port, key, value
        });
        return this;
    }

    /// <summary>
    /// Conditionally adds a VBoxManage command to attach storage to a controller.
    /// </summary>
    /// <param name="ifBody">Condition to check.</param>
    /// <param name="storageController">Name of the storage controller.</param>
    /// <param name="port">Port number.</param>
    /// <param name="key">Key argument.</param>
    /// <param name="value">Value argument.</param>
    /// <returns>The builder instance.</returns>
    public PackerBuilderBuilder ModifyStorageAttachIf(Func<bool> ifBody, string storageController, int port, string key, string value)
    {
        if (ifBody()) ModifyStorageAttach(storageController, port, key, value);
        return this;
    }

    /// <summary>
    /// Conditionally adds a VBoxManage command of any kind.
    /// </summary>
    /// <param name="ifBody">Condition to check.</param>
    /// <param name="kind">VBoxManage command kind (e.g., "modifyvm").</param>
    /// <param name="name">VM name or controller name.</param>
    /// <param name="key">Key argument.</param>
    /// <param name="value">Value argument.</param>
    /// <returns>The builder instance.</returns>
    public PackerBuilderBuilder AddVBoxManageIf(Func<bool> ifBody, string kind, string name, string key, string value)
    {
        return ifBody() ? AddVBoxManage(kind, name, key, value) : this;
    }

    /// <summary>
    /// Adds a VBoxManage command of any kind.
    /// </summary>
    /// <param name="kind">VBoxManage command kind.</param>
    /// <param name="name">VM name or controller name.</param>
    /// <param name="key">Key argument.</param>
    /// <param name="value">Value argument.</param>
    /// <returns>The builder instance.</returns>
    public PackerBuilderBuilder AddVBoxManage(string kind, string name, string key, string value)
    {
        _vboxManage ??= new List<List<string>>();
        _vboxManage.Add(new List<string>
        {
            kind, name, key, value
        });
        return this;
    }

    /// <summary>
    /// Conditionally adds a VBoxManage command to modify VM properties.
    /// </summary>
    /// <param name="ifBody">Condition to check.</param>
    /// <param name="name">Property name.</param>
    /// <param name="value">Property value.</param>
    /// <returns>The builder instance.</returns>
    public PackerBuilderBuilder ModifyVmIf(Func<bool> ifBody, string name, string value)
    {
        return ifBody() ? AddVBoxManage("modifyvm", "{{ .Name }}", name, value) : this;
    }

    /// <summary>
    /// Adds a VBoxManage command to modify VM properties.
    /// </summary>
    /// <param name="name">Property name.</param>
    /// <param name="value">Property value.</param>
    /// <returns>The builder instance.</returns>
    public PackerBuilderBuilder ModifyVm(string name, string value)
    {
        return AddVBoxManage("modifyvm", "{{ .Name }}", name, value);
    }

    /// <summary>
    /// Adds a VBoxManage command to set a VM property.
    /// </summary>
    /// <param name="name">Property name.</param>
    /// <param name="value">Property value.</param>
    /// <returns>The builder instance.</returns>
    public PackerBuilderBuilder ModifyProperty(string name, string value)
    {
        _vboxManage ??= new List<List<string>>();
        _vboxManage.Add(new List<string>
        {
            "setproperty", name, value
        });
        return this;
    }

    /// <summary>
    /// Adds a VBoxManage command to set extra data for the VM.
    /// </summary>
    /// <param name="key">Extra data key.</param>
    /// <param name="value">Extra data value.</param>
    /// <returns>The builder instance.</returns>
    public PackerBuilderBuilder SetExtraData(string key, string value)
    {
        _vboxManage ??= new List<List<string>>();
        _vboxManage.Add(new List<string>
        {
            "setextradata", "{{ .Name }}", key, value
        });
        return this;
    }

    /// <summary>
    /// Sets the name of the VM to create.
    /// </summary>
    /// <param name="value">VM name.</param>
    /// <returns>The builder instance.</returns>
    public PackerBuilderBuilder VmName(string? value)
    {
        _vmName = value;
        return this;
    }

   /// <summary>
   /// Performs validation of the builder's configuration properties and collects any validation failures encountered.
   /// </summary>
   /// <param name="visitedCollector">A dictionary used to track objects that have already been visited during validation to prevent redundant checks.</param>
   /// <param name="failures">A dictionary that accumulates validation failures, mapping property names to the corresponding exception details.</param>
    protected new void ValidateInternal(VisitedObjectDictionary visitedCollector, FailuresDictionary failures)
    {
        AssertNotNullNotEmptyCollection(_bootCommand, nameof(PackerBuilder.BootCommand), failures, (name) => new StringIsEmptyOrWhitespaceException(name));
        AssertNotEmptyOrWhitespace(_bootWait, nameof(PackerBuilder.BootWait), failures, (name) => new StringIsEmptyOrWhitespaceException(name));
        AssertNotEmptyOrWhitespace(_communicator, nameof(PackerBuilder.Communicator), failures, (name) => new StringIsEmptyOrWhitespaceException(name));
        AssertNotEmptyOrWhitespace(_diskSize, nameof(PackerBuilder.DiskSize), failures, (name) => new StringIsEmptyOrWhitespaceException(name));
        AssertNotEmptyOrWhitespace(_format, nameof(PackerBuilder.Format), failures, (name) => new StringIsEmptyOrWhitespaceException(name));
        AssertNotEmptyOrWhitespace(_guestOsType, nameof(PackerBuilder.GuestOsType), failures, (name) => new StringIsEmptyOrWhitespaceException(name));
        AssertNotEmptyOrWhitespace(_httpDirectory, nameof(PackerBuilder.HttpDirectory), failures, (name) => new StringIsEmptyOrWhitespaceException(name));
        AssertNotEmptyOrWhitespace(_isoChecksum, nameof(PackerBuilder.IsoChecksum), failures, (name) => new StringIsEmptyOrWhitespaceException(name));
        AssertNotNullNotEmptyCollection(_isoUrls, nameof(PackerBuilder.IsoUrls), failures, (name) => new StringIsEmptyOrWhitespaceException(name));
        AssertNotNullNotEmptyCollection(_isoUrls, nameof(PackerBuilder.IsoUrls), failures, (name) => new StringIsEmptyOrWhitespaceException(name));
        AssertNotEmptyOrWhitespace(_guestAdditionUrl, nameof(PackerBuilder.GuestAdditionUrl), failures, (name) => new StringIsEmptyOrWhitespaceException(name));
        AssertNotEmptyOrWhitespace(_guestAdditionSha256, nameof(PackerBuilder.GuestAdditionSha256), failures, (name) => new StringIsEmptyOrWhitespaceException(name));
        AssertNotEmptyOrWhitespace(_guestAdditionPath, nameof(PackerBuilder.GuestAdditionPath), failures, (name) => new StringIsEmptyOrWhitespaceException(name));
        AssertNotEmptyOrWhitespace(_guestAdditionMode, nameof(PackerBuilder.GuestAdditionMode), failures, (name) => new StringIsEmptyOrWhitespaceException(name));
        AssertNotEmptyOrWhitespace(_virtualBoxVersionFile, nameof(PackerBuilder.VirtualBoxVersionFile), failures, (name) => new StringIsEmptyOrWhitespaceException(name));
        AssertNotEmptyOrWhitespace(_shutdownCommand, nameof(PackerBuilder.ShutdownCommand), failures, (name) => new StringIsEmptyOrWhitespaceException(name));
        AssertNotEmptyOrWhitespace(_sshPassword, nameof(PackerBuilder.SshPassword), failures, (name) => new StringIsEmptyOrWhitespaceException(name));
        AssertNotEmptyOrWhitespace(_sshTimeout, nameof(PackerBuilder.SshTimeout), failures, (name) => new StringIsEmptyOrWhitespaceException(name));
        AssertNotEmptyOrWhitespace(_sshUsername, nameof(PackerBuilder.SshUsername), failures, (name) => new StringIsEmptyOrWhitespaceException(name));
        AssertNotEmptyOrWhitespace(_type, nameof(PackerBuilder.Type), failures, (name) => new StringIsEmptyOrWhitespaceException(name));
        AssertNotNullNotEmptyCollection(_vboxManage, nameof(PackerBuilder.VboxManage), failures, (name) => new StringIsEmptyOrWhitespaceException(name));
        AssertNotEmptyOrWhitespace(_vmName, nameof(PackerBuilder.VmName), failures, (name) => new StringIsEmptyOrWhitespaceException(name));
        AssertNotEmptyOrWhitespace(_hardDriveInterface, nameof(PackerBuilder.HardDriveInterface), failures, (name) => new StringIsEmptyOrWhitespaceException(name));
    }

    /// <summary>
    /// Creates and initializes a new instance of the <see cref="PackerBuilder"/> class with the current configuration
    /// settings.
    /// </summary>
    /// <remarks>The returned <see cref="PackerBuilder"/> instance reflects the current values of all relevant
    /// configuration fields. Changes to the configuration after calling this method will not affect the returned
    /// builder.</remarks>
    /// <returns>A <see cref="PackerBuilder"/> object populated with the configured properties.</returns>
    protected override PackerBuilder Instantiate()
    {
        return new PackerBuilder
        {
            BootCommand = _bootCommand,
            BootWait = _bootWait,
            Communicator = _communicator,
            DiskSize = _diskSize,
            Format = _format,
            GuestOsType = _guestOsType,
            Headless = _headless,
            HttpDirectory = _httpDirectory,
            IsoChecksum = _isoChecksum,
            IsoUrls = _isoUrls,
            GuestAdditionUrl = _guestAdditionUrl,
            GuestAdditionSha256 = _guestAdditionSha256,
            GuestAdditionPath = _guestAdditionPath,
            GuestAdditionMode = _guestAdditionMode,
            VirtualBoxVersionFile = _virtualBoxVersionFile,
            KeepRegistered = _keepRegistered,
            ShutdownCommand = _shutdownCommand,
            SshPassword = _sshPassword,
            SshTimeout = _sshTimeout,
            SshUsername = _sshUsername,
            Type = _type,
            VboxManage = _vboxManage,
            VmName = _vmName,
            HardDriveDiscard = _hardDriveDiscard,
            HardDriveInterface = _hardDriveInterface
        };
    }
}