using FrenchExDev.Net.CSharp.Object.Builder;
using FrenchExDev.Net.CSharp.Object.Result;

namespace FrenchExDev.Net.Vagrant;

#region Box Command Builders

/// <summary>
/// Builder for <see cref="VagrantBoxAddCommand"/>.
/// </summary>
public sealed class VagrantBoxAddCommandBuilder : AbstractBuilder<VagrantBoxAddCommand>
{
    private string? _box;
    private string? _boxVersion;
    private string? _caCert;
    private string? _caPath;
    private string? _cert;
    private string? _checksum;
    private string? _checksumType;
    private bool _clean;
    private bool _force;
    private bool _insecure;
    private bool _locationTrusted;
    private string? _name;
    private string? _provider;
    private string? _workingDirectory;

    public VagrantBoxAddCommandBuilder Box(string value) { _box = value; return this; }
    public VagrantBoxAddCommandBuilder BoxVersion(string value) { _boxVersion = value; return this; }
    public VagrantBoxAddCommandBuilder CaCert(string value) { _caCert = value; return this; }
    public VagrantBoxAddCommandBuilder CaPath(string value) { _caPath = value; return this; }
    public VagrantBoxAddCommandBuilder Cert(string value) { _cert = value; return this; }
    public VagrantBoxAddCommandBuilder Checksum(string value) { _checksum = value; return this; }
    public VagrantBoxAddCommandBuilder ChecksumType(string value) { _checksumType = value; return this; }
    public VagrantBoxAddCommandBuilder Clean(bool value = true) { _clean = value; return this; }
    public VagrantBoxAddCommandBuilder Force(bool value = true) { _force = value; return this; }
    public VagrantBoxAddCommandBuilder Insecure(bool value = true) { _insecure = value; return this; }
    public VagrantBoxAddCommandBuilder LocationTrusted(bool value = true) { _locationTrusted = value; return this; }
    public VagrantBoxAddCommandBuilder Name(string value) { _name = value; return this; }
    public VagrantBoxAddCommandBuilder Provider(string value) { _provider = value; return this; }
    public VagrantBoxAddCommandBuilder WorkingDirectory(string value) { _workingDirectory = value; return this; }

    protected override void ValidateInternal(VisitedObjectDictionary visitedCollector, FailuresDictionary failures)
    {
        AssertNotNullOrEmptyOrWhitespace(_box, nameof(VagrantBoxAddCommand.Box), failures, n => new ArgumentException("Box is required.", n));
    }

    protected override VagrantBoxAddCommand Instantiate() => new()
    {
        Box = _box!,
        BoxVersion = _boxVersion,
        CaCert = _caCert,
        CaPath = _caPath,
        Cert = _cert,
        Checksum = _checksum,
        ChecksumType = _checksumType,
        Clean = _clean,
        Force = _force,
        Insecure = _insecure,
        LocationTrusted = _locationTrusted,
        Name = _name,
        Provider = _provider,
        WorkingDirectory = _workingDirectory
    };
}

/// <summary>
/// Builder for <see cref="VagrantBoxListCommand"/>.
/// </summary>
public sealed class VagrantBoxListCommandBuilder : AbstractBuilder<VagrantBoxListCommand>
{
    private bool _boxInfo;
    private bool _machineReadable;
    private string? _workingDirectory;

    public VagrantBoxListCommandBuilder BoxInfo(bool value = true) { _boxInfo = value; return this; }
    public VagrantBoxListCommandBuilder MachineReadable(bool value = true) { _machineReadable = value; return this; }
    public VagrantBoxListCommandBuilder WorkingDirectory(string value) { _workingDirectory = value; return this; }

    protected override VagrantBoxListCommand Instantiate() => new()
    {
        BoxInfo = _boxInfo,
        MachineReadable = _machineReadable,
        WorkingDirectory = _workingDirectory
    };
}

/// <summary>
/// Builder for <see cref="VagrantBoxOutdatedCommand"/>.
/// </summary>
public sealed class VagrantBoxOutdatedCommandBuilder : AbstractBuilder<VagrantBoxOutdatedCommand>
{
    private bool _force;
    private bool _global;
    private bool _machineReadable;
    private string? _workingDirectory;

    public VagrantBoxOutdatedCommandBuilder Force(bool value = true) { _force = value; return this; }
    public VagrantBoxOutdatedCommandBuilder Global(bool value = true) { _global = value; return this; }
    public VagrantBoxOutdatedCommandBuilder MachineReadable(bool value = true) { _machineReadable = value; return this; }
    public VagrantBoxOutdatedCommandBuilder WorkingDirectory(string value) { _workingDirectory = value; return this; }

    protected override VagrantBoxOutdatedCommand Instantiate() => new()
    {
        Force = _force,
        Global = _global,
        MachineReadable = _machineReadable,
        WorkingDirectory = _workingDirectory
    };
}

/// <summary>
/// Builder for <see cref="VagrantBoxPruneCommand"/>.
/// </summary>
public sealed class VagrantBoxPruneCommandBuilder : AbstractBuilder<VagrantBoxPruneCommand>
{
    private bool _dryRun;
    private bool _force;
    private bool _keepActiveBoxes;
    private string? _name;
    private string? _provider;
    private string? _workingDirectory;

    public VagrantBoxPruneCommandBuilder DryRun(bool value = true) { _dryRun = value; return this; }
    public VagrantBoxPruneCommandBuilder Force(bool value = true) { _force = value; return this; }
    public VagrantBoxPruneCommandBuilder KeepActiveBoxes(bool value = true) { _keepActiveBoxes = value; return this; }
    public VagrantBoxPruneCommandBuilder Name(string value) { _name = value; return this; }
    public VagrantBoxPruneCommandBuilder Provider(string value) { _provider = value; return this; }
    public VagrantBoxPruneCommandBuilder WorkingDirectory(string value) { _workingDirectory = value; return this; }

    protected override VagrantBoxPruneCommand Instantiate() => new()
    {
        DryRun = _dryRun,
        Force = _force,
        KeepActiveBoxes = _keepActiveBoxes,
        Name = _name,
        Provider = _provider,
        WorkingDirectory = _workingDirectory
    };
}

/// <summary>
/// Builder for <see cref="VagrantBoxRemoveCommand"/>.
/// </summary>
public sealed class VagrantBoxRemoveCommandBuilder : AbstractBuilder<VagrantBoxRemoveCommand>
{
    private string? _name;
    private bool _all;
    private string? _boxVersion;
    private bool _force;
    private string? _provider;
    private string? _workingDirectory;

    public VagrantBoxRemoveCommandBuilder Name(string value) { _name = value; return this; }
    public VagrantBoxRemoveCommandBuilder All(bool value = true) { _all = value; return this; }
    public VagrantBoxRemoveCommandBuilder BoxVersion(string value) { _boxVersion = value; return this; }
    public VagrantBoxRemoveCommandBuilder Force(bool value = true) { _force = value; return this; }
    public VagrantBoxRemoveCommandBuilder Provider(string value) { _provider = value; return this; }
    public VagrantBoxRemoveCommandBuilder WorkingDirectory(string value) { _workingDirectory = value; return this; }

    protected override void ValidateInternal(VisitedObjectDictionary visitedCollector, FailuresDictionary failures)
    {
        AssertNotNullOrEmptyOrWhitespace(_name, nameof(VagrantBoxRemoveCommand.Name), failures, n => new ArgumentException("Name is required.", n));
    }

    protected override VagrantBoxRemoveCommand Instantiate() => new()
    {
        Name = _name!,
        All = _all,
        BoxVersion = _boxVersion,
        Force = _force,
        Provider = _provider,
        WorkingDirectory = _workingDirectory
    };
}

/// <summary>
/// Builder for <see cref="VagrantBoxRepackageCommand"/>.
/// </summary>
public sealed class VagrantBoxRepackageCommandBuilder : AbstractBuilder<VagrantBoxRepackageCommand>
{
    private string? _name;
    private string? _provider;
    private string? _cartouche;
    private string? _workingDirectory;

    public VagrantBoxRepackageCommandBuilder Name(string value) { _name = value; return this; }
    public VagrantBoxRepackageCommandBuilder Provider(string value) { _provider = value; return this; }
    public VagrantBoxRepackageCommandBuilder Cartouche(string value) { _cartouche = value; return this; }
    public VagrantBoxRepackageCommandBuilder WorkingDirectory(string value) { _workingDirectory = value; return this; }

    protected override void ValidateInternal(VisitedObjectDictionary visitedCollector, FailuresDictionary failures)
    {
        AssertNotNullOrEmptyOrWhitespace(_name, nameof(VagrantBoxRepackageCommand.Name), failures, n => new ArgumentException("Name is required.", n));
        AssertNotNullOrEmptyOrWhitespace(_provider, nameof(VagrantBoxRepackageCommand.Provider), failures, n => new ArgumentException("Provider is required.", n));
        AssertNotNullOrEmptyOrWhitespace(_cartouche, nameof(VagrantBoxRepackageCommand.Cartouche), failures, n => new ArgumentException("Cartouche is required.", n));
    }

    protected override VagrantBoxRepackageCommand Instantiate() => new()
    {
        Name = _name!,
        Provider = _provider!,
        Cartouche = _cartouche!,
        WorkingDirectory = _workingDirectory
    };
}

/// <summary>
/// Builder for <see cref="VagrantBoxUpdateCommand"/>.
/// </summary>
public sealed class VagrantBoxUpdateCommandBuilder : AbstractBuilder<VagrantBoxUpdateCommand>
{
    private string? _box;
    private string? _caCert;
    private string? _caPath;
    private string? _cert;
    private bool _force;
    private bool _insecure;
    private string? _provider;
    private string? _workingDirectory;

    public VagrantBoxUpdateCommandBuilder Box(string value) { _box = value; return this; }
    public VagrantBoxUpdateCommandBuilder CaCert(string value) { _caCert = value; return this; }
    public VagrantBoxUpdateCommandBuilder CaPath(string value) { _caPath = value; return this; }
    public VagrantBoxUpdateCommandBuilder Cert(string value) { _cert = value; return this; }
    public VagrantBoxUpdateCommandBuilder Force(bool value = true) { _force = value; return this; }
    public VagrantBoxUpdateCommandBuilder Insecure(bool value = true) { _insecure = value; return this; }
    public VagrantBoxUpdateCommandBuilder Provider(string value) { _provider = value; return this; }
    public VagrantBoxUpdateCommandBuilder WorkingDirectory(string value) { _workingDirectory = value; return this; }

    protected override VagrantBoxUpdateCommand Instantiate() => new()
    {
        Box = _box,
        CaCert = _caCert,
        CaPath = _caPath,
        Cert = _cert,
        Force = _force,
        Insecure = _insecure,
        Provider = _provider,
        WorkingDirectory = _workingDirectory
    };
}

#endregion

#region Cloud Command Builders

/// <summary>
/// Builder for <see cref="VagrantCloudAuthCommand"/>.
/// </summary>
public sealed class VagrantCloudAuthCommandBuilder : AbstractBuilder<VagrantCloudAuthCommand>
{
    private string? _subCommand;
    private string? _workingDirectory;

    public VagrantCloudAuthCommandBuilder SubCommand(string value) { _subCommand = value; return this; }
    public VagrantCloudAuthCommandBuilder WorkingDirectory(string value) { _workingDirectory = value; return this; }

    protected override void ValidateInternal(VisitedObjectDictionary visitedCollector, FailuresDictionary failures)
    {
        AssertNotNullOrEmptyOrWhitespace(_subCommand, nameof(VagrantCloudAuthCommand.SubCommand), failures, n => new ArgumentException("SubCommand is required.", n));
    }

    protected override VagrantCloudAuthCommand Instantiate() => new()
    {
        SubCommand = _subCommand!,
        WorkingDirectory = _workingDirectory
    };
}

/// <summary>
/// Builder for <see cref="VagrantCloudBoxCommand"/>.
/// </summary>
public sealed class VagrantCloudBoxCommandBuilder : AbstractBuilder<VagrantCloudBoxCommand>
{
    private string? _subCommand;
    private string? _workingDirectory;

    public VagrantCloudBoxCommandBuilder SubCommand(string value) { _subCommand = value; return this; }
    public VagrantCloudBoxCommandBuilder WorkingDirectory(string value) { _workingDirectory = value; return this; }

    protected override void ValidateInternal(VisitedObjectDictionary visitedCollector, FailuresDictionary failures)
    {
        AssertNotNullOrEmptyOrWhitespace(_subCommand, nameof(VagrantCloudBoxCommand.SubCommand), failures, n => new ArgumentException("SubCommand is required.", n));
    }

    protected override VagrantCloudBoxCommand Instantiate() => new()
    {
        SubCommand = _subCommand!,
        WorkingDirectory = _workingDirectory
    };
}

/// <summary>
/// Builder for <see cref="VagrantCloudProviderCommand"/>.
/// </summary>
public sealed class VagrantCloudProviderCommandBuilder : AbstractBuilder<VagrantCloudProviderCommand>
{
    private string? _subCommand;
    private string? _workingDirectory;

    public VagrantCloudProviderCommandBuilder SubCommand(string value) { _subCommand = value; return this; }
    public VagrantCloudProviderCommandBuilder WorkingDirectory(string value) { _workingDirectory = value; return this; }

    protected override void ValidateInternal(VisitedObjectDictionary visitedCollector, FailuresDictionary failures)
    {
        AssertNotNullOrEmptyOrWhitespace(_subCommand, nameof(VagrantCloudProviderCommand.SubCommand), failures, n => new ArgumentException("SubCommand is required.", n));
    }

    protected override VagrantCloudProviderCommand Instantiate() => new()
    {
        SubCommand = _subCommand!,
        WorkingDirectory = _workingDirectory
    };
}

/// <summary>
/// Builder for <see cref="VagrantCloudSearchCommand"/>.
/// </summary>
public sealed class VagrantCloudSearchCommandBuilder : AbstractBuilder<VagrantCloudSearchCommand>
{
    private string? _query;
    private bool _json;
    private int? _limit;
    private int? _page;
    private string? _provider;
    private bool _short;
    private string? _sortBy;
    private string? _order;
    private string? _workingDirectory;

    public VagrantCloudSearchCommandBuilder Query(string value) { _query = value; return this; }
    public VagrantCloudSearchCommandBuilder Json(bool value = true) { _json = value; return this; }
    public VagrantCloudSearchCommandBuilder Limit(int value) { _limit = value; return this; }
    public VagrantCloudSearchCommandBuilder Page(int value) { _page = value; return this; }
    public VagrantCloudSearchCommandBuilder Provider(string value) { _provider = value; return this; }
    public VagrantCloudSearchCommandBuilder Short(bool value = true) { _short = value; return this; }
    public VagrantCloudSearchCommandBuilder SortBy(string value) { _sortBy = value; return this; }
    public VagrantCloudSearchCommandBuilder Order(string value) { _order = value; return this; }
    public VagrantCloudSearchCommandBuilder WorkingDirectory(string value) { _workingDirectory = value; return this; }

    protected override VagrantCloudSearchCommand Instantiate() => new()
    {
        Query = _query,
        Json = _json,
        Limit = _limit,
        Page = _page,
        Provider = _provider,
        Short = _short,
        SortBy = _sortBy,
        Order = _order,
        WorkingDirectory = _workingDirectory
    };
}

/// <summary>
/// Builder for <see cref="VagrantCloudVersionCommand"/>.
/// </summary>
public sealed class VagrantCloudVersionCommandBuilder : AbstractBuilder<VagrantCloudVersionCommand>
{
    private string? _subCommand;
    private string? _workingDirectory;

    public VagrantCloudVersionCommandBuilder SubCommand(string value) { _subCommand = value; return this; }
    public VagrantCloudVersionCommandBuilder WorkingDirectory(string value) { _workingDirectory = value; return this; }

    protected override void ValidateInternal(VisitedObjectDictionary visitedCollector, FailuresDictionary failures)
    {
        AssertNotNullOrEmptyOrWhitespace(_subCommand, nameof(VagrantCloudVersionCommand.SubCommand), failures, n => new ArgumentException("SubCommand is required.", n));
    }

    protected override VagrantCloudVersionCommand Instantiate() => new()
    {
        SubCommand = _subCommand!,
        WorkingDirectory = _workingDirectory
    };
}

#endregion

#region Plugin Command Builders

/// <summary>
/// Builder for <see cref="VagrantPluginInstallCommand"/>.
/// </summary>
public sealed class VagrantPluginInstallCommandBuilder : AbstractBuilder<VagrantPluginInstallCommand>
{
    private string? _name;
    private string? _entryPoint;
    private bool _local;
    private bool _pluginCleanSources;
    private string? _pluginSource;
    private string? _pluginVersion;
    private bool _verbose;
    private string? _workingDirectory;

    public VagrantPluginInstallCommandBuilder Name(string value) { _name = value; return this; }
    public VagrantPluginInstallCommandBuilder EntryPoint(string value) { _entryPoint = value; return this; }
    public VagrantPluginInstallCommandBuilder Local(bool value = true) { _local = value; return this; }
    public VagrantPluginInstallCommandBuilder PluginCleanSources(bool value = true) { _pluginCleanSources = value; return this; }
    public VagrantPluginInstallCommandBuilder PluginSource(string value) { _pluginSource = value; return this; }
    public VagrantPluginInstallCommandBuilder PluginVersion(string value) { _pluginVersion = value; return this; }
    public VagrantPluginInstallCommandBuilder Verbose(bool value = true) { _verbose = value; return this; }
    public VagrantPluginInstallCommandBuilder WorkingDirectory(string value) { _workingDirectory = value; return this; }

    protected override void ValidateInternal(VisitedObjectDictionary visitedCollector, FailuresDictionary failures)
    {
        AssertNotNullOrEmptyOrWhitespace(_name, nameof(VagrantPluginInstallCommand.Name), failures, n => new ArgumentException("Name is required.", n));
    }

    protected override VagrantPluginInstallCommand Instantiate() => new()
    {
        Name = _name!,
        EntryPoint = _entryPoint,
        Local = _local,
        PluginCleanSources = _pluginCleanSources,
        PluginSource = _pluginSource,
        PluginVersion = _pluginVersion,
        Verbose = _verbose,
        WorkingDirectory = _workingDirectory
    };
}

/// <summary>
/// Builder for <see cref="VagrantPluginLicenseCommand"/>.
/// </summary>
public sealed class VagrantPluginLicenseCommandBuilder : AbstractBuilder<VagrantPluginLicenseCommand>
{
    private string? _name;
    private string? _licenseFile;
    private string? _workingDirectory;

    public VagrantPluginLicenseCommandBuilder Name(string value) { _name = value; return this; }
    public VagrantPluginLicenseCommandBuilder LicenseFile(string value) { _licenseFile = value; return this; }
    public VagrantPluginLicenseCommandBuilder WorkingDirectory(string value) { _workingDirectory = value; return this; }

    protected override void ValidateInternal(VisitedObjectDictionary visitedCollector, FailuresDictionary failures)
    {
        AssertNotNullOrEmptyOrWhitespace(_name, nameof(VagrantPluginLicenseCommand.Name), failures, n => new ArgumentException("Name is required.", n));
        AssertNotNullOrEmptyOrWhitespace(_licenseFile, nameof(VagrantPluginLicenseCommand.LicenseFile), failures, n => new ArgumentException("LicenseFile is required.", n));
    }

    protected override VagrantPluginLicenseCommand Instantiate() => new()
    {
        Name = _name!,
        LicenseFile = _licenseFile!,
        WorkingDirectory = _workingDirectory
    };
}

/// <summary>
/// Builder for <see cref="VagrantPluginListCommand"/>.
/// </summary>
public sealed class VagrantPluginListCommandBuilder : AbstractBuilder<VagrantPluginListCommand>
{
    private bool _local;
    private string? _workingDirectory;

    public VagrantPluginListCommandBuilder Local(bool value = true) { _local = value; return this; }
    public VagrantPluginListCommandBuilder WorkingDirectory(string value) { _workingDirectory = value; return this; }

    protected override VagrantPluginListCommand Instantiate() => new()
    {
        Local = _local,
        WorkingDirectory = _workingDirectory
    };
}

/// <summary>
/// Builder for <see cref="VagrantPluginRepairCommand"/>.
/// </summary>
public sealed class VagrantPluginRepairCommandBuilder : AbstractBuilder<VagrantPluginRepairCommand>
{
    private bool _local;
    private string? _workingDirectory;

    public VagrantPluginRepairCommandBuilder Local(bool value = true) { _local = value; return this; }
    public VagrantPluginRepairCommandBuilder WorkingDirectory(string value) { _workingDirectory = value; return this; }

    protected override VagrantPluginRepairCommand Instantiate() => new()
    {
        Local = _local,
        WorkingDirectory = _workingDirectory
    };
}

/// <summary>
/// Builder for <see cref="VagrantPluginUninstallCommand"/>.
/// </summary>
public sealed class VagrantPluginUninstallCommandBuilder : AbstractBuilder<VagrantPluginUninstallCommand>
{
    private string? _name;
    private bool _local;
    private string? _workingDirectory;

    public VagrantPluginUninstallCommandBuilder Name(string value) { _name = value; return this; }
    public VagrantPluginUninstallCommandBuilder Local(bool value = true) { _local = value; return this; }
    public VagrantPluginUninstallCommandBuilder WorkingDirectory(string value) { _workingDirectory = value; return this; }

    protected override void ValidateInternal(VisitedObjectDictionary visitedCollector, FailuresDictionary failures)
    {
        AssertNotNullOrEmptyOrWhitespace(_name, nameof(VagrantPluginUninstallCommand.Name), failures, n => new ArgumentException("Name is required.", n));
    }

    protected override VagrantPluginUninstallCommand Instantiate() => new()
    {
        Name = _name!,
        Local = _local,
        WorkingDirectory = _workingDirectory
    };
}

/// <summary>
/// Builder for <see cref="VagrantPluginUpdateCommand"/>.
/// </summary>
public sealed class VagrantPluginUpdateCommandBuilder : AbstractBuilder<VagrantPluginUpdateCommand>
{
    private string? _name;
    private bool _local;
    private string? _workingDirectory;

    public VagrantPluginUpdateCommandBuilder Name(string value) { _name = value; return this; }
    public VagrantPluginUpdateCommandBuilder Local(bool value = true) { _local = value; return this; }
    public VagrantPluginUpdateCommandBuilder WorkingDirectory(string value) { _workingDirectory = value; return this; }

    protected override VagrantPluginUpdateCommand Instantiate() => new()
    {
        Name = _name,
        Local = _local,
        WorkingDirectory = _workingDirectory
    };
}

#endregion

#region Snapshot Command Builders

/// <summary>
/// Builder for <see cref="VagrantSnapshotDeleteCommand"/>.
/// </summary>
public sealed class VagrantSnapshotDeleteCommandBuilder : AbstractBuilder<VagrantSnapshotDeleteCommand>
{
    private string? _name;
    private string? _vmName;
    private string? _workingDirectory;

    public VagrantSnapshotDeleteCommandBuilder Name(string value) { _name = value; return this; }
    public VagrantSnapshotDeleteCommandBuilder VmName(string value) { _vmName = value; return this; }
    public VagrantSnapshotDeleteCommandBuilder WorkingDirectory(string value) { _workingDirectory = value; return this; }

    protected override void ValidateInternal(VisitedObjectDictionary visitedCollector, FailuresDictionary failures)
    {
        AssertNotNullOrEmptyOrWhitespace(_name, nameof(VagrantSnapshotDeleteCommand.Name), failures, n => new ArgumentException("Name is required.", n));
    }

    protected override VagrantSnapshotDeleteCommand Instantiate() => new()
    {
        Name = _name!,
        VmName = _vmName,
        WorkingDirectory = _workingDirectory
    };
}

/// <summary>
/// Builder for <see cref="VagrantSnapshotListCommand"/>.
/// </summary>
public sealed class VagrantSnapshotListCommandBuilder : AbstractBuilder<VagrantSnapshotListCommand>
{
    private string? _vmName;
    private string? _workingDirectory;

    public VagrantSnapshotListCommandBuilder VmName(string value) { _vmName = value; return this; }
    public VagrantSnapshotListCommandBuilder WorkingDirectory(string value) { _workingDirectory = value; return this; }

    protected override VagrantSnapshotListCommand Instantiate() => new()
    {
        VmName = _vmName,
        WorkingDirectory = _workingDirectory
    };
}

/// <summary>
/// Builder for <see cref="VagrantSnapshotPopCommand"/>.
/// </summary>
public sealed class VagrantSnapshotPopCommandBuilder : AbstractBuilder<VagrantSnapshotPopCommand>
{
    private string? _vmName;
    private bool _noDelete;
    private bool _noProvision;
    private bool _noStart;
    private bool _provision;
    private List<string>? _provisionWith;
    private string? _workingDirectory;

    public VagrantSnapshotPopCommandBuilder VmName(string value) { _vmName = value; return this; }
    public VagrantSnapshotPopCommandBuilder NoDelete(bool value = true) { _noDelete = value; return this; }
    public VagrantSnapshotPopCommandBuilder NoProvision(bool value = true) { _noProvision = value; return this; }
    public VagrantSnapshotPopCommandBuilder NoStart(bool value = true) { _noStart = value; return this; }
    public VagrantSnapshotPopCommandBuilder Provision(bool value = true) { _provision = value; return this; }
    public VagrantSnapshotPopCommandBuilder ProvisionWith(params string[] values) { _provisionWith = [.. values]; return this; }
    public VagrantSnapshotPopCommandBuilder WorkingDirectory(string value) { _workingDirectory = value; return this; }

    protected override VagrantSnapshotPopCommand Instantiate() => new()
    {
        VmName = _vmName,
        NoDelete = _noDelete,
        NoProvision = _noProvision,
        NoStart = _noStart,
        Provision = _provision,
        ProvisionWith = _provisionWith,
        WorkingDirectory = _workingDirectory
    };
}

/// <summary>
/// Builder for <see cref="VagrantSnapshotPushCommand"/>.
/// </summary>
public sealed class VagrantSnapshotPushCommandBuilder : AbstractBuilder<VagrantSnapshotPushCommand>
{
    private string? _vmName;
    private string? _workingDirectory;

    public VagrantSnapshotPushCommandBuilder VmName(string value) { _vmName = value; return this; }
    public VagrantSnapshotPushCommandBuilder WorkingDirectory(string value) { _workingDirectory = value; return this; }

    protected override VagrantSnapshotPushCommand Instantiate() => new()
    {
        VmName = _vmName,
        WorkingDirectory = _workingDirectory
    };
}

/// <summary>
/// Builder for <see cref="VagrantSnapshotRestoreCommand"/>.
/// </summary>
public sealed class VagrantSnapshotRestoreCommandBuilder : AbstractBuilder<VagrantSnapshotRestoreCommand>
{
    private string? _name;
    private string? _vmName;
    private bool _noProvision;
    private bool _noStart;
    private bool _provision;
    private List<string>? _provisionWith;
    private string? _workingDirectory;

    public VagrantSnapshotRestoreCommandBuilder Name(string value) { _name = value; return this; }
    public VagrantSnapshotRestoreCommandBuilder VmName(string value) { _vmName = value; return this; }
    public VagrantSnapshotRestoreCommandBuilder NoProvision(bool value = true) { _noProvision = value; return this; }
    public VagrantSnapshotRestoreCommandBuilder NoStart(bool value = true) { _noStart = value; return this; }
    public VagrantSnapshotRestoreCommandBuilder Provision(bool value = true) { _provision = value; return this; }
    public VagrantSnapshotRestoreCommandBuilder ProvisionWith(params string[] values) { _provisionWith = [.. values]; return this; }
    public VagrantSnapshotRestoreCommandBuilder WorkingDirectory(string value) { _workingDirectory = value; return this; }

    protected override void ValidateInternal(VisitedObjectDictionary visitedCollector, FailuresDictionary failures)
    {
        AssertNotNullOrEmptyOrWhitespace(_name, nameof(VagrantSnapshotRestoreCommand.Name), failures, n => new ArgumentException("Name is required.", n));
    }

    protected override VagrantSnapshotRestoreCommand Instantiate() => new()
    {
        Name = _name!,
        VmName = _vmName,
        NoProvision = _noProvision,
        NoStart = _noStart,
        Provision = _provision,
        ProvisionWith = _provisionWith,
        WorkingDirectory = _workingDirectory
    };
}

/// <summary>
/// Builder for <see cref="VagrantSnapshotSaveCommand"/>.
/// </summary>
public sealed class VagrantSnapshotSaveCommandBuilder : AbstractBuilder<VagrantSnapshotSaveCommand>
{
    private string? _name;
    private string? _vmName;
    private bool _force;
    private string? _workingDirectory;

    public VagrantSnapshotSaveCommandBuilder Name(string value) { _name = value; return this; }
    public VagrantSnapshotSaveCommandBuilder VmName(string value) { _vmName = value; return this; }
    public VagrantSnapshotSaveCommandBuilder Force(bool value = true) { _force = value; return this; }
    public VagrantSnapshotSaveCommandBuilder WorkingDirectory(string value) { _workingDirectory = value; return this; }

    protected override void ValidateInternal(VisitedObjectDictionary visitedCollector, FailuresDictionary failures)
    {
        AssertNotNullOrEmptyOrWhitespace(_name, nameof(VagrantSnapshotSaveCommand.Name), failures, n => new ArgumentException("Name is required.", n));
    }

    protected override VagrantSnapshotSaveCommand Instantiate() => new()
    {
        Name = _name!,
        VmName = _vmName,
        Force = _force,
        WorkingDirectory = _workingDirectory
    };
}

#endregion

#region Top-Level Command Builders

/// <summary>
/// Builder for <see cref="VagrantDestroyCommand"/>.
/// </summary>
public sealed class VagrantDestroyCommandBuilder : AbstractBuilder<VagrantDestroyCommand>
{
    private string? _vmName;
    private bool _force;
    private bool _graceful;
    private bool _parallel;
    private string? _workingDirectory;

    public VagrantDestroyCommandBuilder VmName(string value) { _vmName = value; return this; }
    public VagrantDestroyCommandBuilder Force(bool value = true) { _force = value; return this; }
    public VagrantDestroyCommandBuilder Graceful(bool value = true) { _graceful = value; return this; }
    public VagrantDestroyCommandBuilder Parallel(bool value = true) { _parallel = value; return this; }
    public VagrantDestroyCommandBuilder WorkingDirectory(string value) { _workingDirectory = value; return this; }

    protected override VagrantDestroyCommand Instantiate() => new()
    {
        VmName = _vmName,
        Force = _force,
        Graceful = _graceful,
        Parallel = _parallel,
        WorkingDirectory = _workingDirectory
    };
}

/// <summary>
/// Builder for <see cref="VagrantGlobalStatusCommand"/>.
/// </summary>
public sealed class VagrantGlobalStatusCommandBuilder : AbstractBuilder<VagrantGlobalStatusCommand>
{
    private bool _prune;
    private string? _workingDirectory;

    public VagrantGlobalStatusCommandBuilder Prune(bool value = true) { _prune = value; return this; }
    public VagrantGlobalStatusCommandBuilder WorkingDirectory(string value) { _workingDirectory = value; return this; }

    protected override VagrantGlobalStatusCommand Instantiate() => new()
    {
        Prune = _prune,
        WorkingDirectory = _workingDirectory
    };
}

/// <summary>
/// Builder for <see cref="VagrantHaltCommand"/>.
/// </summary>
public sealed class VagrantHaltCommandBuilder : AbstractBuilder<VagrantHaltCommand>
{
    private string? _vmName;
    private bool _force;
    private string? _workingDirectory;

    public VagrantHaltCommandBuilder VmName(string value) { _vmName = value; return this; }
    public VagrantHaltCommandBuilder Force(bool value = true) { _force = value; return this; }
    public VagrantHaltCommandBuilder WorkingDirectory(string value) { _workingDirectory = value; return this; }

    protected override VagrantHaltCommand Instantiate() => new()
    {
        VmName = _vmName,
        Force = _force,
        WorkingDirectory = _workingDirectory
    };
}

/// <summary>
/// Builder for <see cref="VagrantInitCommand"/>.
/// </summary>
public sealed class VagrantInitCommandBuilder : AbstractBuilder<VagrantInitCommand>
{
    private string? _boxName;
    private string? _boxUrl;
    private string? _boxVersion;
    private bool _force;
    private bool _minimal;
    private string? _output;
    private string? _template;
    private string? _workingDirectory;

    public VagrantInitCommandBuilder BoxName(string value) { _boxName = value; return this; }
    public VagrantInitCommandBuilder BoxUrl(string value) { _boxUrl = value; return this; }
    public VagrantInitCommandBuilder BoxVersion(string value) { _boxVersion = value; return this; }
    public VagrantInitCommandBuilder Force(bool value = true) { _force = value; return this; }
    public VagrantInitCommandBuilder Minimal(bool value = true) { _minimal = value; return this; }
    public VagrantInitCommandBuilder Output(string value) { _output = value; return this; }
    public VagrantInitCommandBuilder Template(string value) { _template = value; return this; }
    public VagrantInitCommandBuilder WorkingDirectory(string value) { _workingDirectory = value; return this; }

    protected override VagrantInitCommand Instantiate() => new()
    {
        BoxName = _boxName,
        BoxUrl = _boxUrl,
        BoxVersion = _boxVersion,
        Force = _force,
        Minimal = _minimal,
        Output = _output,
        Template = _template,
        WorkingDirectory = _workingDirectory
    };
}

/// <summary>
/// Builder for <see cref="VagrantPackageCommand"/>.
/// </summary>
public sealed class VagrantPackageCommandBuilder : AbstractBuilder<VagrantPackageCommand>
{
    private string? _vmName;
    private string? _base;
    private List<string>? _include;
    private string? _output;
    private string? _vagrantfile;
    private string? _workingDirectory;

    public VagrantPackageCommandBuilder VmName(string value) { _vmName = value; return this; }
    public VagrantPackageCommandBuilder Base(string value) { _base = value; return this; }
    public VagrantPackageCommandBuilder Include(params string[] values) { _include = [.. values]; return this; }
    public VagrantPackageCommandBuilder Output(string value) { _output = value; return this; }
    public VagrantPackageCommandBuilder Vagrantfile(string value) { _vagrantfile = value; return this; }
    public VagrantPackageCommandBuilder WorkingDirectory(string value) { _workingDirectory = value; return this; }

    protected override VagrantPackageCommand Instantiate() => new()
    {
        VmName = _vmName,
        Base = _base,
        Include = _include,
        Output = _output,
        Vagrantfile = _vagrantfile,
        WorkingDirectory = _workingDirectory
    };
}

/// <summary>
/// Builder for <see cref="VagrantPortCommand"/>.
/// </summary>
public sealed class VagrantPortCommandBuilder : AbstractBuilder<VagrantPortCommand>
{
    private string? _vmName;
    private int? _guest;
    private bool _machineReadable;
    private string? _workingDirectory;

    public VagrantPortCommandBuilder VmName(string value) { _vmName = value; return this; }
    public VagrantPortCommandBuilder Guest(int value) { _guest = value; return this; }
    public VagrantPortCommandBuilder MachineReadable(bool value = true) { _machineReadable = value; return this; }
    public VagrantPortCommandBuilder WorkingDirectory(string value) { _workingDirectory = value; return this; }

    protected override VagrantPortCommand Instantiate() => new()
    {
        VmName = _vmName,
        Guest = _guest,
        MachineReadable = _machineReadable,
        WorkingDirectory = _workingDirectory
    };
}

/// <summary>
/// Builder for <see cref="VagrantPowerShellCommand"/>.
/// </summary>
public sealed class VagrantPowerShellCommandBuilder : AbstractBuilder<VagrantPowerShellCommand>
{
    private string? _vmName;
    private string? _command;
    private bool _elevated;
    private string? _workingDirectory;

    public VagrantPowerShellCommandBuilder VmName(string value) { _vmName = value; return this; }
    public VagrantPowerShellCommandBuilder Command(string value) { _command = value; return this; }
    public VagrantPowerShellCommandBuilder Elevated(bool value = true) { _elevated = value; return this; }
    public VagrantPowerShellCommandBuilder WorkingDirectory(string value) { _workingDirectory = value; return this; }

    protected override VagrantPowerShellCommand Instantiate() => new()
    {
        VmName = _vmName,
        Command = _command,
        Elevated = _elevated,
        WorkingDirectory = _workingDirectory
    };
}

/// <summary>
/// Builder for <see cref="VagrantProvisionCommand"/>.
/// </summary>
public sealed class VagrantProvisionCommandBuilder : AbstractBuilder<VagrantProvisionCommand>
{
    private string? _vmName;
    private List<string>? _provisionWith;
    private string? _workingDirectory;

    public VagrantProvisionCommandBuilder VmName(string value) { _vmName = value; return this; }
    public VagrantProvisionCommandBuilder ProvisionWith(params string[] values) { _provisionWith = [.. values]; return this; }
    public VagrantProvisionCommandBuilder WorkingDirectory(string value) { _workingDirectory = value; return this; }

    protected override VagrantProvisionCommand Instantiate() => new()
    {
        VmName = _vmName,
        ProvisionWith = _provisionWith,
        WorkingDirectory = _workingDirectory
    };
}

/// <summary>
/// Builder for <see cref="VagrantRdpCommand"/>.
/// </summary>
public sealed class VagrantRdpCommandBuilder : AbstractBuilder<VagrantRdpCommand>
{
    private string? _vmName;
    private string? _extraArgs;
    private string? _workingDirectory;

    public VagrantRdpCommandBuilder VmName(string value) { _vmName = value; return this; }
    public VagrantRdpCommandBuilder ExtraArgs(string value) { _extraArgs = value; return this; }
    public VagrantRdpCommandBuilder WorkingDirectory(string value) { _workingDirectory = value; return this; }

    protected override VagrantRdpCommand Instantiate() => new()
    {
        VmName = _vmName,
        ExtraArgs = _extraArgs,
        WorkingDirectory = _workingDirectory
    };
}

/// <summary>
/// Builder for <see cref="VagrantReloadCommand"/>.
/// </summary>
public sealed class VagrantReloadCommandBuilder : AbstractBuilder<VagrantReloadCommand>
{
    private string? _vmName;
    private bool _force;
    private bool _noProvision;
    private bool _provision;
    private List<string>? _provisionWith;
    private string? _workingDirectory;

    public VagrantReloadCommandBuilder VmName(string value) { _vmName = value; return this; }
    public VagrantReloadCommandBuilder Force(bool value = true) { _force = value; return this; }
    public VagrantReloadCommandBuilder NoProvision(bool value = true) { _noProvision = value; return this; }
    public VagrantReloadCommandBuilder Provision(bool value = true) { _provision = value; return this; }
    public VagrantReloadCommandBuilder ProvisionWith(params string[] values) { _provisionWith = [.. values]; return this; }
    public VagrantReloadCommandBuilder WorkingDirectory(string value) { _workingDirectory = value; return this; }

    protected override VagrantReloadCommand Instantiate() => new()
    {
        VmName = _vmName,
        Force = _force,
        NoProvision = _noProvision,
        Provision = _provision,
        ProvisionWith = _provisionWith,
        WorkingDirectory = _workingDirectory
    };
}

/// <summary>
/// Builder for <see cref="VagrantResumeCommand"/>.
/// </summary>
public sealed class VagrantResumeCommandBuilder : AbstractBuilder<VagrantResumeCommand>
{
    private string? _vmName;
    private bool _noProvision;
    private bool _provision;
    private List<string>? _provisionWith;
    private string? _workingDirectory;

    public VagrantResumeCommandBuilder VmName(string value) { _vmName = value; return this; }
    public VagrantResumeCommandBuilder NoProvision(bool value = true) { _noProvision = value; return this; }
    public VagrantResumeCommandBuilder Provision(bool value = true) { _provision = value; return this; }
    public VagrantResumeCommandBuilder ProvisionWith(params string[] values) { _provisionWith = [.. values]; return this; }
    public VagrantResumeCommandBuilder WorkingDirectory(string value) { _workingDirectory = value; return this; }

    protected override VagrantResumeCommand Instantiate() => new()
    {
        VmName = _vmName,
        NoProvision = _noProvision,
        Provision = _provision,
        ProvisionWith = _provisionWith,
        WorkingDirectory = _workingDirectory
    };
}

/// <summary>
/// Builder for <see cref="VagrantSshCommand"/>.
/// </summary>
public sealed class VagrantSshCommandBuilder : AbstractBuilder<VagrantSshCommand>
{
    private string? _vmName;
    private string? _command;
    private string? _extraArgs;
    private bool _plain;
    private bool _tty;
    private string? _workingDirectory;

    public VagrantSshCommandBuilder VmName(string value) { _vmName = value; return this; }
    public VagrantSshCommandBuilder Command(string value) { _command = value; return this; }
    public VagrantSshCommandBuilder ExtraArgs(string value) { _extraArgs = value; return this; }
    public VagrantSshCommandBuilder Plain(bool value = true) { _plain = value; return this; }
    public VagrantSshCommandBuilder Tty(bool value = true) { _tty = value; return this; }
    public VagrantSshCommandBuilder WorkingDirectory(string value) { _workingDirectory = value; return this; }

    protected override VagrantSshCommand Instantiate() => new()
    {
        VmName = _vmName,
        Command = _command,
        ExtraArgs = _extraArgs,
        Plain = _plain,
        Tty = _tty,
        WorkingDirectory = _workingDirectory
    };
}

/// <summary>
/// Builder for <see cref="VagrantSshConfigCommand"/>.
/// </summary>
public sealed class VagrantSshConfigCommandBuilder : AbstractBuilder<VagrantSshConfigCommand>
{
    private string? _vmName;
    private string? _host;
    private string? _workingDirectory;

    public VagrantSshConfigCommandBuilder VmName(string value) { _vmName = value; return this; }
    public VagrantSshConfigCommandBuilder Host(string value) { _host = value; return this; }
    public VagrantSshConfigCommandBuilder WorkingDirectory(string value) { _workingDirectory = value; return this; }

    protected override VagrantSshConfigCommand Instantiate() => new()
    {
        VmName = _vmName,
        Host = _host,
        WorkingDirectory = _workingDirectory
    };
}

/// <summary>
/// Builder for <see cref="VagrantStatusCommand"/>.
/// </summary>
public sealed class VagrantStatusCommandBuilder : AbstractBuilder<VagrantStatusCommand>
{
    private string? _vmName;
    private string? _workingDirectory;

    public VagrantStatusCommandBuilder VmName(string value) { _vmName = value; return this; }
    public VagrantStatusCommandBuilder WorkingDirectory(string value) { _workingDirectory = value; return this; }

    protected override VagrantStatusCommand Instantiate() => new()
    {
        VmName = _vmName,
        WorkingDirectory = _workingDirectory
    };
}

/// <summary>
/// Builder for <see cref="VagrantSuspendCommand"/>.
/// </summary>
public sealed class VagrantSuspendCommandBuilder : AbstractBuilder<VagrantSuspendCommand>
{
    private string? _vmName;
    private string? _workingDirectory;

    public VagrantSuspendCommandBuilder VmName(string value) { _vmName = value; return this; }
    public VagrantSuspendCommandBuilder WorkingDirectory(string value) { _workingDirectory = value; return this; }

    protected override VagrantSuspendCommand Instantiate() => new()
    {
        VmName = _vmName,
        WorkingDirectory = _workingDirectory
    };
}

/// <summary>
/// Builder for <see cref="VagrantUpCommand"/>.
/// </summary>
public sealed class VagrantUpCommandBuilder : AbstractBuilder<VagrantUpCommand>
{
    private string? _vmName;
    private bool _destroyOnError;
    private bool _installProvider;
    private bool _noDestroyOnError;
    private bool _noInstallProvider;
    private bool _noParallel;
    private bool _noProvision;
    private bool _parallel;
    private string? _provider;
    private bool _provision;
    private List<string>? _provisionWith;
    private string? _workingDirectory;

    public VagrantUpCommandBuilder VmName(string value) { _vmName = value; return this; }
    public VagrantUpCommandBuilder DestroyOnError(bool value = true) { _destroyOnError = value; return this; }
    public VagrantUpCommandBuilder InstallProvider(bool value = true) { _installProvider = value; return this; }
    public VagrantUpCommandBuilder NoDestroyOnError(bool value = true) { _noDestroyOnError = value; return this; }
    public VagrantUpCommandBuilder NoInstallProvider(bool value = true) { _noInstallProvider = value; return this; }
    public VagrantUpCommandBuilder NoParallel(bool value = true) { _noParallel = value; return this; }
    public VagrantUpCommandBuilder NoProvision(bool value = true) { _noProvision = value; return this; }
    public VagrantUpCommandBuilder Parallel(bool value = true) { _parallel = value; return this; }
    public VagrantUpCommandBuilder Provider(string value) { _provider = value; return this; }
    public VagrantUpCommandBuilder Provision(bool value = true) { _provision = value; return this; }
    public VagrantUpCommandBuilder ProvisionWith(params string[] values) { _provisionWith = [.. values]; return this; }
    public VagrantUpCommandBuilder WorkingDirectory(string value) { _workingDirectory = value; return this; }

    protected override VagrantUpCommand Instantiate() => new()
    {
        VmName = _vmName,
        DestroyOnError = _destroyOnError,
        InstallProvider = _installProvider,
        NoDestroyOnError = _noDestroyOnError,
        NoInstallProvider = _noInstallProvider,
        NoParallel = _noParallel,
        NoProvision = _noProvision,
        Parallel = _parallel,
        Provider = _provider,
        Provision = _provision,
        ProvisionWith = _provisionWith,
        WorkingDirectory = _workingDirectory
    };
}

/// <summary>
/// Builder for <see cref="VagrantValidateCommand"/>.
/// </summary>
public sealed class VagrantValidateCommandBuilder : AbstractBuilder<VagrantValidateCommand>
{
    private string? _workingDirectory;

    public VagrantValidateCommandBuilder WorkingDirectory(string value) { _workingDirectory = value; return this; }

    protected override VagrantValidateCommand Instantiate() => new()
    {
        WorkingDirectory = _workingDirectory
    };
}

/// <summary>
/// Builder for <see cref="VagrantVersionCommand"/>.
/// </summary>
public sealed class VagrantVersionCommandBuilder : AbstractBuilder<VagrantVersionCommand>
{
    private string? _workingDirectory;

    public VagrantVersionCommandBuilder WorkingDirectory(string value) { _workingDirectory = value; return this; }

    protected override VagrantVersionCommand Instantiate() => new()
    {
        WorkingDirectory = _workingDirectory
    };
}

/// <summary>
/// Builder for <see cref="VagrantWinrmCommand"/>.
/// </summary>
public sealed class VagrantWinrmCommandBuilder : AbstractBuilder<VagrantWinrmCommand>
{
    private string? _vmName;
    private string? _command;
    private bool _elevated;
    private string? _shell;
    private string? _workingDirectory;

    public VagrantWinrmCommandBuilder VmName(string value) { _vmName = value; return this; }
    public VagrantWinrmCommandBuilder Command(string value) { _command = value; return this; }
    public VagrantWinrmCommandBuilder Elevated(bool value = true) { _elevated = value; return this; }
    public VagrantWinrmCommandBuilder Shell(string value) { _shell = value; return this; }
    public VagrantWinrmCommandBuilder WorkingDirectory(string value) { _workingDirectory = value; return this; }

    protected override VagrantWinrmCommand Instantiate() => new()
    {
        VmName = _vmName,
        Command = _command,
        Elevated = _elevated,
        Shell = _shell,
        WorkingDirectory = _workingDirectory
    };
}

/// <summary>
/// Builder for <see cref="VagrantWinrmConfigCommand"/>.
/// </summary>
public sealed class VagrantWinrmConfigCommandBuilder : AbstractBuilder<VagrantWinrmConfigCommand>
{
    private string? _vmName;
    private string? _host;
    private string? _workingDirectory;

    public VagrantWinrmConfigCommandBuilder VmName(string value) { _vmName = value; return this; }
    public VagrantWinrmConfigCommandBuilder Host(string value) { _host = value; return this; }
    public VagrantWinrmConfigCommandBuilder WorkingDirectory(string value) { _workingDirectory = value; return this; }

    protected override VagrantWinrmConfigCommand Instantiate() => new()
    {
        VmName = _vmName,
        Host = _host,
        WorkingDirectory = _workingDirectory
    };
}

#endregion
