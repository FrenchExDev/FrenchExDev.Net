using FrenchExDev.Net.Packer.Bundle;

namespace FrenchExDev.Net.Packer.Abstractions;


/// <summary>
/// Represents a collection of boot command strings used to configure or initialize a system during startup.
/// </summary>
/// <remarks>This class inherits from <see cref="List{string}"/>, providing all standard list operations for
/// managing boot commands. Each string in the list typically corresponds to a command or instruction executed during
/// the boot process.</remarks>
public class PackerBuilderBootCommandList : List<string> { }

/// <summary>
/// Provides default configuration settings for VirtualBox virtual machines, including memory, CPU, chipset, and
/// virtualization options.
/// </summary>
/// <remarks>This class supplies a predefined set of VirtualBox command-line options commonly used to initialize
/// or modify virtual machine parameters. The settings are intended to optimize compatibility and performance for
/// typical Linux x64 guest environments. The values can be used as a baseline and may be customized as needed for
/// specific use cases.</remarks>
public static class PackerVirtualBoxSettings
{
    public static class ModifyVmList
    {
        public enum Profiles
        {
            Default,
            Performance
        }

        /// <summary>
        /// Applies the specified configuration profile to the target virtual machine settings.
        /// </summary>
        /// <remarks>This method updates the settings in <paramref name="target"/> according to the
        /// selected profile. Existing values in <paramref name="target"/> may be overwritten by the profile's
        /// settings.</remarks>
        /// <param name="target">The target collection of virtual machine settings to update with the selected profile values.</param>
        /// <param name="profile">The configuration profile to apply. Must be a defined value of the <see cref="Profiles"/> enumeration.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="profile"/> is not a valid value of the <see cref="Profiles"/> enumeration.</exception>
        public static void ApplyProfile(PackerVirtualBoxModifyVmList target, Profiles profile)
        {
            void Iterate(PackerVirtualBoxModifyVmList source)
            {
                foreach (var item in source)
                {
                    target[item.Key] = item.Value;
                }
            }

            switch (profile)
            {
                case Profiles.Default:
                    Iterate(Defaults);
                    break;
                case Profiles.Performance:
                    Iterate(Performance);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(profile), profile, null);
            }
        }

        /// <summary>
        /// Gets the default set of VirtualBox VM modification options used for configuring virtual machines in automated
        /// build scenarios.
        /// </summary>
        /// <remarks>The returned list includes commonly used VirtualBox command-line options and their default
        /// values, suitable for typical Linux x64 guest configurations. This list is intended for use with Packer or
        /// similar automation tools to standardize VM setup. To customize VM behavior, modify the returned list before
        /// applying it to a VM.</remarks>
        public static PackerVirtualBoxModifyVmList Defaults { get; } = new()
        {
            { "--memory", "{{user `memory`}}" } ,
            { "--cpus", "{{user `cpus`}}" },
            { "--nat-localhostreachable1", "on" },
            { "--vram", "{{user `vmemory`}}" },
            { "--natdnshostresolver1", "on" },
        };

        /// <summary>
        /// Gets a predefined set of VirtualBox VM modification options optimized for performance.
        /// </summary>
        /// <remarks>The options included in this set enable hardware virtualization features, advanced
        /// chipset settings, and other configurations that may improve VM performance. These settings are suitable for
        /// Linux x64 guests and may not be compatible with all host systems or guest operating systems. Review the
        /// options before applying them to ensure compatibility with your environment.</remarks>
        public static PackerVirtualBoxModifyVmList Performance { get; } = new(Defaults)
        {
            { "--ioapic", "on" },
            { "--hwvirtex", "on"} ,
            { "--hpet", "on" },
            { "--largepages", "on" },
            { "--vtxvpid", "on" },
            { "--vtxux", "on" },
            { "--pae", "on" },
            { "--acpi", "on"} ,
            { "--pagefusion", "on" },
            { "--chipset", "ich9" },
            { "--nested-hw-virt", "on" },
            { "--nestedpaging", "on"},
            { "--ostype", "Linux_x64" },
            { "--graphicscontroller", "vmsvga" }
        };

    }

    /// <summary>
    /// Provides predefined property lists and profile management for configuring VirtualBox settings in Packer builds.
    /// </summary>
    /// <remarks>This static class exposes common property sets for VirtualBox configuration, such as default
    /// and performance profiles, and offers methods to apply these profiles to a target property list. Use the provided
    /// profiles to standardize or optimize VirtualBox build settings across different environments.</remarks>
    public static class ModifyPropertyList
    {
        /// <summary>
        /// Provides a default set of VirtualBox property modifications for use with the Packer VirtualBox builder.
        /// </summary>
        /// <remarks>This list includes recommended default properties, such as enabling exclusive
        /// hardware virtualization. It can be used as a starting point and modified as needed for custom build
        /// scenarios.</remarks>
        public static PackerVirtualBoxModifyPropertyList Defaults = new()
        {
        };

        /// <summary>
        /// Provides a predefined property list that configures VirtualBox to optimize performance by enabling exclusive
        /// hardware virtualization.
        /// </summary>
        /// <remarks>This property list sets the 'hwvirtexclusive' option to 'on', which may improve
        /// virtual machine performance by allowing exclusive access to hardware virtualization features. Use this
        /// property list when creating or modifying VirtualBox VMs where maximum performance is desired.</remarks>
        public static PackerVirtualBoxModifyPropertyList Performance = new(Defaults)
        {
            { "hwvirtexclusive", "on" }
        };

        /// <summary>
        /// Specifies the available configuration profiles for the application.
        /// </summary>
        /// <remarks>Use this enumeration to select between different sets of application settings, such
        /// as default or performance-optimized configurations. The selected profile may affect resource usage,
        /// performance, or other operational characteristics depending on the application's implementation.</remarks>
        public enum Profiles
        {
            Default,
            Performance
        }

        /// <summary>
        /// Applies the specified profile settings to the target property list by updating its values according to the
        /// selected profile.
        /// </summary>
        /// <param name="target">The property list to which the profile settings will be applied. Existing values may be overwritten.</param>
        /// <param name="profile">The profile whose settings should be applied to the target property list. Must be a defined value of the
        /// Profiles enumeration.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the specified profile is not a valid value of the Profiles enumeration.</exception>
        public static void ApplyProfile(PackerVirtualBoxModifyPropertyList target, Profiles profile)
        {
            void Iterate(PackerVirtualBoxModifyPropertyList source)
            {
                foreach (var item in source)
                {
                    target[item.Key] = item.Value;
                }
            }
            switch (profile)
            {
                case Profiles.Default:
                    Iterate(Defaults);
                    break;
                case Profiles.Performance:
                    Iterate(Performance);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(profile), profile, null);
            }
        }
    }

    /// <summary>
    /// Provides predefined profiles and helper methods for configuring VirtualBox VM modification options using
    /// conditional rules.
    /// </summary>
    /// <remarks>This class supplies a default set of modification rules for VirtualBox VMs, tailored to
    /// common operating system scenarios. It also enables applying these profiles to a target list of modification
    /// rules. All members are static and intended for use in scenarios where VM configuration needs to be adjusted
    /// based on OS-specific requirements.</remarks>
    public static class ModifyVmIfList
    {
        /// <summary>
        /// Specifies the available profile options for configuration or selection within the application.
        /// </summary>
        public enum Profiles
        {
            Default,
        }

        /// <summary>
        /// Gets the default list of VirtualBox VM modification options based on the current operating system.
        /// </summary>
        /// <remarks>The returned list contains recommended configuration options for VirtualBox VMs,
        /// tailored to common operating systems. These defaults are intended to provide optimal compatibility and
        /// performance for typical use cases. The list is read-only and reflects the settings that are most commonly
        /// required when modifying VMs with VirtualBox.</remarks>
        public static PackerVirtualBoxModifyVmIfList Defaults { get; } = new()
        {
            new PackerVirtualBoxModifyVmIf(() => !OperatingSystem.IsWindows(), "--biosapic", "x2apic"),
            new PackerVirtualBoxModifyVmIf(OperatingSystem.IsLinux, "--biosapic", "x2apic"),
            new PackerVirtualBoxModifyVmIf(OperatingSystem.IsLinux, "--paravirtprovider", "kvm"),
            new PackerVirtualBoxModifyVmIf(OperatingSystem.IsWindows, "--paravirtprovider", "kvm"),
            new PackerVirtualBoxModifyVmIf(OperatingSystem.IsMacOS, "--paravirtprovider", "minimum"),
        };

        /// <summary>
        /// Applies the specified profile to the target interface list by adding the corresponding configuration items.
        /// </summary>
        /// <param name="target">The interface list to which the profile's configuration items will be added.</param>
        /// <param name="profile">The profile to apply. Must be a valid value of the <see cref="Profiles"/> enumeration.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="profile"/> is not a valid value of the <see cref="Profiles"/> enumeration.</exception>
        public static void ApplyProfile(PackerVirtualBoxModifyVmIfList target, Profiles profile)
        {
            void Iterate(PackerVirtualBoxModifyVmIfList source)
            {
                foreach (var item in source)
                {
                    target.Add(item);
                }
            }
            switch (profile)
            {
                case Profiles.Default:
                    Iterate(Defaults);
                    break;
                default: throw new ArgumentOutOfRangeException(nameof(profile), profile, null);
            }
        }
    }
}

/// <summary>
/// Represents a collection of key-value pairs used to specify VirtualBox VM modification parameters.
/// </summary>
/// <remarks>Each entry in the collection maps a VirtualBox VM property name to its corresponding value. This
/// class is typically used to construct command-line arguments or configuration settings when modifying VirtualBox
/// virtual machines.</remarks>
public class PackerVirtualBoxModifyVmList : Dictionary<string, string>
{
    public PackerVirtualBoxModifyVmList()
    {
    }

    public PackerVirtualBoxModifyVmList(IDictionary<string, string> dictionary) : base(dictionary)
    {
    }
}

/// <summary>
/// Represents a list of ISO-compliant URLs used for referencing resources in a standardized format.
/// </summary>
/// <remarks>This class inherits from <see cref="List{string}"/>, providing all standard list operations for
/// managing collections of URLs. It is intended for scenarios where a set of URLs must conform to ISO standards, such
/// as in data exchange or interoperability contexts.</remarks>
public class PackerVirtualBoxIsoUrlList : List<string> { }

/// <summary>
/// Represents a collection of key/value pairs for storing additional data, where each key is a string and each value is
/// an object.
/// </summary>
/// <remarks>This class extends <see cref="Dictionary{String, Object}"/> to provide a flexible structure for
/// associating arbitrary extra data with an entity or operation. Keys are case-sensitive. Values can be of any type,
/// but callers are responsible for casting objects to the expected type when retrieving data.</remarks>
public class PackerVirtualBoxExtraDataDictionary : Dictionary<string, object> { }

/// <summary>
/// Represents a collection of virtual machine interface modification requests.
/// </summary>
/// <remarks>Use this class to group multiple <see cref="PackerVirtualBoxModifyVmIf"/> instances when performing batch operations
/// on virtual machine interfaces. Inherits all standard collection functionality from <see cref="List{T}"/>.</remarks>
public class PackerVirtualBoxModifyVmIfList : List<PackerVirtualBoxModifyVmIf> { }

/// <summary>
/// Represents a conditional modification to a virtual machine, specifying the condition, the name of the property to
/// modify, and the new value to apply.
/// </summary>
/// <param name="IfBody">A delegate that evaluates the condition under which the modification should be applied. The modification is
/// performed only if this function returns <see langword="true"/>.</param>
/// <param name="Name">The name of the virtual machine property to be modified. Cannot be null or empty.</param>
/// <param name="Value">The value to assign to the specified property if the condition is met.</param>
public record PackerVirtualBoxModifyVmIf(Func<bool> IfBody, string Name, string Value);

/// <summary>
/// Represents a collection of key-value pairs for modifying properties, where each key is a string and each value is
/// </summary>
public class PackerVirtualBoxModifyPropertyList : Dictionary<string, object>
{
    public PackerVirtualBoxModifyPropertyList()
    {
    }

    public PackerVirtualBoxModifyPropertyList(IDictionary<string, object> dictionary) : base(dictionary)
    {
    }
}

/// <summary>
/// Represents a collection of storage controller modification operations for use with VirtualBox configuration changes.
/// </summary>
/// <remarks>Use this class to group multiple <see cref="PackerVirtualBoxModifyStorageController"/> instances when
/// performing batch modifications to VirtualBox storage controllers. Inherits all standard collection functionality
/// from <see cref="List{T}"/>.</remarks>
public class PackerVirtualBoxModifyStorageControllerList : List<PackerVirtualBoxModifyStorageController> { }

/// <summary>
/// Represents a modification to a VirtualBox storage controller, specifying the controller name, the option to modify,
/// and the new value to apply.
/// </summary>
/// <param name="Name">The name of the VirtualBox storage controller to be modified. This value cannot be null or empty.</param>
/// <param name="Option">The option or property of the storage controller to modify. Common options include controller type, port count, or
/// other controller-specific settings. This value cannot be null or empty.</param>
/// <param name="Value">The new value to assign to the specified option. The format and valid values depend on the option being modified.</param>
public record PackerVirtualBoxModifyStorageController(string Name, string Option, string Value);

/// <summary>
/// Represents a collection of storage attachment modifications to be applied to a VirtualBox virtual machine.
/// </summary>
/// <remarks>Use this class to group multiple <see cref="PackerVirtualBoxModifyStorageAttach"/> operations for batch
/// processing. The order of items in the list determines the sequence in which modifications are applied.</remarks>
public class PackerVirtualBoxModifyStorageAttachList : List<PackerVirtualBoxModifyStorageAttach> { }

/// <summary>
/// Represents the parameters required to attach or modify a storage device in a VirtualBox virtual machine
/// configuration.
/// </summary>
/// <param name="ControllerName">The name of the storage controller to which the device is attached. This value must match an existing controller in
/// the target virtual machine.</param>
/// <param name="Port">The port number on the specified controller where the device will be attached. Must be a non-negative integer within
/// the valid range for the controller.</param>
/// <param name="Option">The option specifying the type of attachment or modification to perform, such as the device type or attachment mode.</param>
/// <param name="Value">The value associated with the specified option, such as the path to a disk image or the identifier of the device.</param>
public record PackerVirtualBoxModifyStorageAttach(string ControllerName, int Port, string Option, string Value);

/// <summary>
/// Provides a builder for configuring VirtualBox ISO-based Packer builds with customizable settings and options.
/// </summary>
public class PackerVirtualBoxIsoBuilder
{
    /// <summary>
    /// Provides the default builder function for configuring a PackerVirtualBoxIsoBuilder with performance-oriented
    /// settings and storage controller options.
    /// </summary>
    /// <remarks>This function applies a standard set of modifications to a PackerVirtualBoxIsoBuilder
    /// instance, including performance settings and SATA controller configuration. The value of the isSataSsd parameter
    /// determines whether certain storage options, such as host I/O cache and non-rotational flags, are enabled. This
    /// builder is intended for use as a baseline configuration when creating VirtualBox VMs with Packer.</remarks>
    private static Func<bool, PackerVirtualBoxIsoBuilder, PackerVirtualBoxIsoBuilder> _DefaultBuilder = (isSataSsd, body) => body
        .WithModifyVm(PackerVirtualBoxSettings.ModifyVmList.Performance)
        .WithModifyProperty(PackerVirtualBoxSettings.ModifyPropertyList.Performance)
        .WithModifyVmIf(PackerVirtualBoxSettings.ModifyVmIfList.Defaults)
        .WithModifyStorageController("SATA Controller", "--hostiocache", isSataSsd ? "on" : "off")
        .WithModifyStorageAttach("SATA Controller", 0, "--nonrotational", isSataSsd ? "on" : "off")
        .WithModifyStorageAttach("SATA Controller", 0, "--discard", isSataSsd ? "on" : "off")
        .WithExtraData("VBoxInternal/Devices/ahci/0/Config/Port0/NonRotational", "1")
        .WithVmName("{{user `vm_name`}}")
        .WithHardDriveDiscard()
        .WithHardDriveInterface("sata");

    /// <summary>
    /// Provides a default factory delegate for creating a preconfigured instance of the PackerVirtualBoxIsoBuilder with
    /// recommended performance and storage settings for VirtualBox ISO builds.
    /// </summary>
    /// <remarks>The returned builder instance is initialized with settings optimized for performance and
    /// compatibility with Alpine Linux and VirtualBox. This includes default boot commands, VM modifications, storage
    /// controller configuration, and extra data for non-rotational storage. The VM name is set using the user variable
    /// 'vm_name'. Callers can further customize the builder after obtaining it from this delegate.</remarks>
    private static Func<bool, PackerVirtualBoxIsoBuilder> _Default = (bool isSataSsd) => _DefaultBuilder(isSataSsd, new PackerVirtualBoxIsoBuilder());


    /// <summary>
    /// Represents the builder type identifier for VirtualBox ISO images.
    /// </summary>
    public static readonly string VirtualBoxIso = "virtualbox-iso";

    /// <summary>
    /// Provides a delegate that returns a default instance of the PackerVirtualBoxIsoBuilder configured for either SSD
    /// or non-SSD environments.
    /// </summary>
    /// <remarks>Use this delegate to obtain a preconfigured PackerVirtualBoxIsoBuilder without manually
    /// specifying all settings. The configuration applied depends on the value of the input parameter, allowing for
    /// optimized defaults based on storage type.</remarks>
    public static Func<bool, PackerVirtualBoxIsoBuilder> Defaults = (bool isSsd) => _Default(isSsd);

    /// <summary>
    /// Provides a builder function that configures a PackerVirtualBoxIsoBuilder for optimal performance settings.
    /// </summary>
    /// <remarks>Use this delegate to apply recommended performance-related options when constructing a
    /// PackerVirtualBoxIsoBuilder instance. This can be useful for scenarios where build speed and resource efficiency
    /// are prioritized over other configuration aspects.</remarks>
    public static Func<PackerVirtualBoxIsoBuilder, PackerVirtualBoxIsoBuilder> Performance = (body) => _DefaultBuilder(true, body);

    #region Builder data holding fields

    private readonly PackerBuilderBootCommandList _bootCommandList = new();
    private readonly PackerVirtualBoxModifyVmIfList _modifyVmIfList = new();
    private readonly PackerVirtualBoxModifyVmList _modifyVmList = new();
    private readonly PackerVirtualBoxModifyStorageAttachList _modifyStorageAttachList = new();
    private readonly PackerVirtualBoxModifyStorageControllerList _modifyStorageControllerList = new();
    private readonly PackerVirtualBoxIsoUrlList _isoUrlList = new();
    private readonly PackerVirtualBoxExtraDataDictionary _extraDataDictionary = new();
    private readonly PackerVirtualBoxModifyPropertyList _modifyPropertyList = new();
    private bool? _hardDriveDiscard;
    private string? _hardDriveInterface;
    private bool? _keepRegistered;
    private string? _vmName;
    private string? _sshUsername;
    private string? _sshTimeout;
    private string? _sshPassword;
    private string? _shutdownCommand;
    private string? _virtualBoxVersionFile;
    private string? _guestAdditionMode;
    private string? _guestAdditionPath;
    private string? _guestAdditionSha256;
    private string? _guestAdditionUrl;
    private string? _isoChecksum;
    private string? _httpDirectory;
    private bool? _headless;
    private string? _guestOsType;
    private string? _format;
    private string? _diskSize;
    private string? _communicator;
    private string? _bootWait;

    #endregion

    /// <summary>
    /// Sets the name of the virtual machine to be used by the builder.
    /// </summary>
    /// <remarks>This method enables fluent configuration by returning the builder instance. Subsequent calls
    /// will overwrite any previously set virtual machine name.</remarks>
    /// <param name="vm">The name to assign to the virtual machine. Cannot be null or empty.</param>
    /// <returns>The current <see cref="PackerVirtualBoxIsoBuilder"/> instance with the updated virtual machine name.</returns>
    public PackerVirtualBoxIsoBuilder WithVmName(string vm)
    {
        _vmName = vm;
        return this;
    }

    /// <summary>
    /// Configures whether the hard drive discard feature is enabled for the VirtualBox ISO build process.
    /// </summary>
    /// <param name="discard">Specifies whether to enable hard drive discard. If <see langword="true"/>, discard is enabled; if <see
    /// langword="false"/>, discard is disabled. If <c>null</c>, the default behavior is used.</param>
    /// <returns>The current <see cref="PackerVirtualBoxIsoBuilder"/> instance with the updated hard drive discard setting.</returns>
    public PackerVirtualBoxIsoBuilder WithHardDriveDiscard(bool? discard = true)
    {
        _hardDriveDiscard = discard;
        return this;
    }

    /// <summary>
    /// Sets the hard drive interface type to use for the virtual machine configuration.
    /// </summary>
    /// <param name="inter">The name of the hard drive interface to assign, such as "IDE", "SATA", or "SCSI". Cannot be null.</param>
    /// <returns>The current instance of <see cref="PackerVirtualBoxIsoBuilder"/> with the updated hard drive interface setting.</returns>
    public PackerVirtualBoxIsoBuilder WithHardDriveInterface(string inter)
    {
        _hardDriveInterface = inter;
        return this;
    }

    /// <summary>
    /// Adds or updates a property in the builder's property list using the specified key and value.
    /// </summary>
    /// <remarks>This method enables fluent configuration by allowing multiple property assignments in a
    /// single statement. If a property with the same key already exists, its value will be overwritten.</remarks>
    /// <param name="key">The name of the property to add or update. Cannot be null.</param>
    /// <param name="value">The value to associate with the specified property key. Can be any object.</param>
    /// <returns>The current <see cref="PackerVirtualBoxIsoBuilder"/> instance with the updated property list.</returns>
    public PackerVirtualBoxIsoBuilder WithModifyProperty(string key, object value)
    {
        _modifyPropertyList[key] = value;
        return this;
    }

    /// <summary>
    /// Adds or updates multiple VirtualBox properties to be modified during the build process.
    /// </summary>
    /// <remarks>If a property name in <paramref name="dico"/> already exists, its value will be overwritten.
    /// This method enables fluent configuration by returning the builder instance.</remarks>
    /// <param name="dico">A dictionary containing property names and their corresponding values to be set. Each key represents a
    /// VirtualBox property name, and each value specifies the property value to apply.</param>
    /// <returns>The current <see cref="PackerVirtualBoxIsoBuilder"/> instance with the specified properties added or updated.</returns>
    public PackerVirtualBoxIsoBuilder WithModifyProperty(Dictionary<string, object> dico)
    {
        foreach (var kv in dico)
            _modifyPropertyList[kv.Key] = kv.Value;

        return this;
    }

    /// <summary>
    /// Add extra data.
    /// </summary>
    /// <param name="key">key.</param>
    /// <param name="value">value.</param>
    /// <returns></returns>
    public PackerVirtualBoxIsoBuilder WithExtraData(string key, string value)
    {
        _extraDataDictionary.Add(key, value);
        return this;
    }

    /// <summary>
    /// Adds an iso url.
    /// </summary>
    /// <param name="v"></param>
    /// <returns></returns>
    public PackerVirtualBoxIsoBuilder WithIsoUrl(string v)
    {
        _isoUrlList.Add(v);
        return this;
    }

    /// <summary>
    /// Adds a storage attachment modification to the builder configuration for the specified controller, port, and
    /// option.
    /// </summary>
    /// <remarks>Use this method to configure additional storage attachment options for the VirtualBox ISO
    /// builder. Multiple calls will add multiple modifications, which are applied in the order they are
    /// specified.</remarks>
    /// <param name="controllerName">The name of the storage controller to which the modification will be applied.</param>
    /// <param name="port">The port number on the controller where the storage device is attached.</param>
    /// <param name="option">The option to set for the storage attachment, such as the device type or attachment mode.</param>
    /// <param name="value">The value to assign to the specified option for the storage attachment.</param>
    /// <returns>The current <see cref="PackerVirtualBoxIsoBuilder"/> instance with the storage attachment modification applied.</returns>
    public PackerVirtualBoxIsoBuilder WithModifyStorageAttach(string controllerName, int port, string option, string value)
    {
        _modifyStorageAttachList.Add(new PackerVirtualBoxModifyStorageAttach(controllerName, port, option, value));
        return this;
    }

    /// <summary>
    /// Adds a storage controller modification to the builder configuration.
    /// </summary>
    /// <remarks>Use this method to specify custom options for a VirtualBox storage controller when building
    /// an ISO image. Multiple modifications can be added by calling this method repeatedly.</remarks>
    /// <param name="name">The name of the storage controller to modify. Cannot be null or empty.</param>
    /// <param name="option">The option to set for the specified storage controller. Cannot be null or empty.</param>
    /// <param name="value">The value to assign to the specified option. Cannot be null.</param>
    /// <returns>The current <see cref="PackerVirtualBoxIsoBuilder"/> instance with the modification applied.</returns>
    public PackerVirtualBoxIsoBuilder WithModifyStorageController(string name, string option, string value)
    {
        _modifyStorageControllerList.Add(new PackerVirtualBoxModifyStorageController(name, option, value));
        return this;
    }

    /// <summary>
    /// Adds or updates a VirtualBox VM modification setting to be applied during the build process.
    /// </summary>
    /// <remarks>This method enables fluent configuration by allowing multiple VM modifications to be chained
    /// together. If a setting with the same name already exists, its value will be overwritten.</remarks>
    /// <param name="name">The name of the VirtualBox VM setting to modify. Cannot be null or empty.</param>
    /// <param name="value">The value to assign to the specified VM setting. Cannot be null.</param>
    /// <returns>The current <see cref="PackerVirtualBoxIsoBuilder"/> instance with the modification applied.</returns>
    public PackerVirtualBoxIsoBuilder WithModifyVm(string name, string value)
    {
        _modifyVmList[name] = value;
        return this;
    }

    /// <summary>
    /// Adds or updates virtual machine configuration settings to be applied during the build process.
    /// </summary>
    /// <remarks>If a setting key already exists, its value will be updated to the new value provided. This
    /// method supports fluent configuration by returning the builder instance.</remarks>
    /// <param name="values">A dictionary containing key-value pairs that specify the virtual machine settings to modify. Each key represents
    /// a setting name, and its corresponding value specifies the desired configuration.</param>
    /// <returns>The current instance of <see cref="PackerVirtualBoxIsoBuilder"/> with the specified modifications applied,
    /// enabling method chaining.</returns>
    public PackerVirtualBoxIsoBuilder WithModifyVm(Dictionary<string, string> values)
    {
        foreach (var kv in values)
            _modifyVmList[kv.Key] = kv.Value;

        return this;
    }


    /// <summary>
    /// Adds a boot command to the builder configuration and returns the current instance for method chaining.
    /// </summary>
    /// <remarks>This method enables fluent configuration by allowing multiple boot commands to be added in a
    /// chained manner. If <paramref name="b"/> is null, an exception may be thrown.</remarks>
    /// <param name="b">The boot command string to add to the configuration. Cannot be null.</param>
    /// <returns>The current <see cref="PackerVirtualBoxIsoBuilder"/> instance with the specified boot command added.</returns>
    public PackerVirtualBoxIsoBuilder WithBootCommand(string b)
    {
        _bootCommandList.Add(b);
        return this;
    }

    /// <summary>
    /// Adds the specified boot command sequence to the builder configuration.
    /// </summary>
    /// <remarks>This method enables fluent configuration by allowing multiple calls to append additional boot
    /// commands. The order of commands in the list is preserved when sent to the virtual machine.</remarks>
    /// <param name="b">A list of strings representing the boot commands to be sent to the virtual machine during startup. Cannot be
    /// null.</param>
    /// <returns>The current <see cref="PackerVirtualBoxIsoBuilder"/> instance with the updated boot command sequence.</returns>
    public PackerVirtualBoxIsoBuilder WithBootCommand(List<string> b)
    {
        _bootCommandList.AddRange(b);
        return this;
    }

    /// <summary>
    /// Adds a conditional VirtualBox VM modification to the builder, which is applied only if the specified condition
    /// is met. 
    /// </summary>
    /// <remarks>Use this method to specify VM modifications that should only be applied when certain runtime
    /// conditions are satisfied. Multiple conditional modifications can be added by calling this method
    /// repeatedly.</remarks>
    /// <param name="ifBody">A delegate that returns <see langword="true"/> if the modification should be applied; otherwise, <see
    /// langword="false"/>.</param>
    /// <param name="name">The name of the VirtualBox VM property to modify. Cannot be null or empty.</param>
    /// <param name="value">The value to assign to the specified VM property. Cannot be null.</param>
    /// <returns>The current <see cref="PackerVirtualBoxIsoBuilder"/> instance with the conditional modification added.</returns>
    public PackerVirtualBoxIsoBuilder WithModifyVmIf(Func<bool> ifBody, string name, string value)
    {
        _modifyVmIfList.Add(new PackerVirtualBoxModifyVmIf(ifBody, name, value));
        return this;
    }

    /// <summary>
    /// Adds the specified conditional VM modification instructions to the builder configuration.
    /// </summary>
    /// <remarks>Use this method to specify VM modifications that should be applied only if certain conditions
    /// are met during the build process. This enables flexible configuration of the virtual machine based on runtime
    /// criteria.</remarks>
    /// <param name="list">A collection of conditional VM modification instructions to apply. Cannot be null.</param>
    /// <returns>The current <see cref="PackerVirtualBoxIsoBuilder"/> instance with the modifications applied.</returns>
    public PackerVirtualBoxIsoBuilder WithModifyVmIf(PackerVirtualBoxModifyVmIfList list)
    {
        _modifyVmIfList.AddRange(list);
        return this;
    }

    /// <summary>
    /// Applies all configured settings to a <see cref="PackerBuilderBuilder"/> (fluent chaining preserved).
    /// </summary>
    /// <param name="target">Target builder to receive settings.</param>
    public void Apply(PackerBuilderBuilder target)
    {
        target
            .Type(VirtualBoxIso)
            .BootWait(_bootWait)
            .Communicator(_communicator)
            .DiskSize(_diskSize)
            .Format(_format)
            .GuestOsType(_guestOsType)
            .Headless(_headless ?? false)
            .HttpDirectory(_httpDirectory)
            .IsoChecksum(_isoChecksum)
            .GuestAdditionUrl(_guestAdditionUrl)
            .GuestAdditionSha256(_guestAdditionSha256)
            .GuestAdditionPath(_guestAdditionPath)
            .GuestAdditionMode(_guestAdditionMode)
            .VirtualBoxVersionFile(_virtualBoxVersionFile)
            .KeepRegistered(_keepRegistered)
            .ShutdownCommand(_shutdownCommand ?? throw new MissingMemberException(nameof(_shutdownCommand)))
            .SshPassword(_sshPassword)
            .SshTimeout(_sshTimeout)
            .SshUsername(_sshUsername)
            .KeepRegistered(_keepRegistered)
            .VmName(_vmName)
            .HardDriveDiscard(_hardDriveDiscard)
            .HardDriveInterface(_hardDriveInterface ?? throw new MissingMemberException(nameof(_hardDriveInterface)))
            ;

        foreach (var bootCommand in _bootCommandList)
        {
            target.AddBootCommand(bootCommand);
        }

        foreach (var modifyVm in _modifyVmList)
        {
            target.ModifyVm(modifyVm.Key, modifyVm.Value);
        }

        foreach (var isoUrl in _isoUrlList)
        {
            target.AddIsoUrl(isoUrl);
        }

        foreach (var modifyProperty in _modifyPropertyList)
        {
            target.ModifyProperty(modifyProperty.Key, modifyProperty.Value.ToString() ?? string.Empty);
        }

        foreach (var modifyVmIf in _modifyVmIfList)
        {
            target.ModifyVmIf(modifyVmIf.IfBody, modifyVmIf.Name, modifyVmIf.Value);
        }

        foreach (var modifyStorageController in _modifyStorageControllerList)
        {
            target.ModifyStorageController(modifyStorageController.Name, modifyStorageController.Option, modifyStorageController.Value);
        }

        foreach (var modifyStorageAttach in _modifyStorageAttachList)
        {
            target.ModifyStorageAttach(modifyStorageAttach.ControllerName, modifyStorageAttach.Port, modifyStorageAttach.Option, modifyStorageAttach.Value);
        }

        foreach (var extraData in _extraDataDictionary)
        {
            target.SetExtraData(extraData.Key, extraData.Value.ToString() ?? string.Empty);
        }
    }
}

/// <summary>
/// Provides access to the default insecure public SSH key used by Vagrant for provisioning virtual machines with
/// Packer.
/// </summary>
/// <remarks>This class exposes the standard Vagrant public key, which is commonly used for automated VM setup and
/// testing scenarios. The key is intended for use in environments where security is not a concern, such as local
/// development or disposable test machines. For production environments, it is recommended to use a secure,
/// user-generated SSH key instead.</remarks>
public static class PackerVagrantPublicKey
{
    /// <summary>
    /// Provides a builder for the default insecure SSH public key used by Vagrant environments.
    /// </summary>
    /// <remarks>This property returns a delegate that creates a new instance of a file builder containing the
    /// standard Vagrant insecure SSH public key. Use this key only for development or testing purposes, as it is
    /// publicly known and not secure for production environments.</remarks>
    public static Func<FileBuilder> SshKey => () => new FileBuilder()
                            .AddLine("ssh-rsa AAAAB3NzaC1yc2EAAAABIwAAAQEA6NF8iallvQVp22WDkTkyrtvp9eWW6A8YVr+kz4TjGYe7gHzIw+niNltGEFHzD8+v1I2YJ6oXevct1YeS0o9HZyN1Q9qgCgzUFtdOKLv6IedplqoPkcmF0aYet2PkEDo3MlTBckFXPITAMzF8dJSIFo9D8HfdOV0IAdx4O7PtixWKn5y2hMNG0zQPyUecp4pzC6kivAIhyfHilFR61RGL+GPXQ2MWZWFYbAGjyiYJnAmCP3NOTd0jMZEnDkbUvxhMmBYSdETk1rRgm+R4LOzFUGaHqHDLKLX+FIPKcF96hrucXzcWyLbIbEgE98OHlnVYCzRdK8jlqm8tehUc9c9WhQ== vagrant insecure public key");
}