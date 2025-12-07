using FrenchExDev.Net.CSharp.Object.Builder.Testing;

namespace FrenchExDev.Net.Vagrant.Testing;

#region Box Command Test Helpers

/// <summary>
/// Test helper for <see cref="VagrantBoxAddCommandBuilder"/>.
/// </summary>
public static class VagrantBoxAddCommandTest
{
    /// <summary>Configures and builds a valid command, asserting success.</summary>
    public static VagrantBoxAddCommand Valid(Action<VagrantBoxAddCommandBuilder> configure)
    {
        var builder = new VagrantBoxAddCommandBuilder();
        configure(builder);
        return builder.ShouldBuildSuccessfully();
    }

    /// <summary>Configures and builds an invalid command, asserting failure.</summary>
    public static void Invalid(Action<VagrantBoxAddCommandBuilder> configure)
    {
        var builder = new VagrantBoxAddCommandBuilder();
        configure(builder);
        builder.ShouldBuildFailing();
    }
}

/// <summary>
/// Test helper for <see cref="VagrantBoxListCommandBuilder"/>.
/// </summary>
public static class VagrantBoxListCommandTest
{
    /// <summary>Configures and builds a valid command, asserting success.</summary>
    public static VagrantBoxListCommand Valid(Action<VagrantBoxListCommandBuilder> configure)
    {
        var builder = new VagrantBoxListCommandBuilder();
        configure(builder);
        return builder.ShouldBuildSuccessfully();
    }

    /// <summary>Configures and builds an invalid command, asserting failure.</summary>
    public static void Invalid(Action<VagrantBoxListCommandBuilder> configure)
    {
        var builder = new VagrantBoxListCommandBuilder();
        configure(builder);
        builder.ShouldBuildFailing();
    }
}

/// <summary>
/// Test helper for <see cref="VagrantBoxOutdatedCommandBuilder"/>.
/// </summary>
public static class VagrantBoxOutdatedCommandTest
{
    /// <summary>Configures and builds a valid command, asserting success.</summary>
    public static VagrantBoxOutdatedCommand Valid(Action<VagrantBoxOutdatedCommandBuilder> configure)
    {
        var builder = new VagrantBoxOutdatedCommandBuilder();
        configure(builder);
        return builder.ShouldBuildSuccessfully();
    }

    /// <summary>Configures and builds an invalid command, asserting failure.</summary>
    public static void Invalid(Action<VagrantBoxOutdatedCommandBuilder> configure)
    {
        var builder = new VagrantBoxOutdatedCommandBuilder();
        configure(builder);
        builder.ShouldBuildFailing();
    }
}

/// <summary>
/// Test helper for <see cref="VagrantBoxPruneCommandBuilder"/>.
/// </summary>
public static class VagrantBoxPruneCommandTest
{
    /// <summary>Configures and builds a valid command, asserting success.</summary>
    public static VagrantBoxPruneCommand Valid(Action<VagrantBoxPruneCommandBuilder> configure)
    {
        var builder = new VagrantBoxPruneCommandBuilder();
        configure(builder);
        return builder.ShouldBuildSuccessfully();
    }

    /// <summary>Configures and builds an invalid command, asserting failure.</summary>
    public static void Invalid(Action<VagrantBoxPruneCommandBuilder> configure)
    {
        var builder = new VagrantBoxPruneCommandBuilder();
        configure(builder);
        builder.ShouldBuildFailing();
    }
}

/// <summary>
/// Test helper for <see cref="VagrantBoxRemoveCommandBuilder"/>.
/// </summary>
public static class VagrantBoxRemoveCommandTest
{
    /// <summary>Configures and builds a valid command, asserting success.</summary>
    public static VagrantBoxRemoveCommand Valid(Action<VagrantBoxRemoveCommandBuilder> configure)
    {
        var builder = new VagrantBoxRemoveCommandBuilder();
        configure(builder);
        return builder.ShouldBuildSuccessfully();
    }

    /// <summary>Configures and builds an invalid command, asserting failure.</summary>
    public static void Invalid(Action<VagrantBoxRemoveCommandBuilder> configure)
    {
        var builder = new VagrantBoxRemoveCommandBuilder();
        configure(builder);
        builder.ShouldBuildFailing();
    }
}

/// <summary>
/// Test helper for <see cref="VagrantBoxRepackageCommandBuilder"/>.
/// </summary>
public static class VagrantBoxRepackageCommandTest
{
    /// <summary>Configures and builds a valid command, asserting success.</summary>
    public static VagrantBoxRepackageCommand Valid(Action<VagrantBoxRepackageCommandBuilder> configure)
    {
        var builder = new VagrantBoxRepackageCommandBuilder();
        configure(builder);
        return builder.ShouldBuildSuccessfully();
    }

    /// <summary>Configures and builds an invalid command, asserting failure.</summary>
    public static void Invalid(Action<VagrantBoxRepackageCommandBuilder> configure)
    {
        var builder = new VagrantBoxRepackageCommandBuilder();
        configure(builder);
        builder.ShouldBuildFailing();
    }
}

/// <summary>
/// Test helper for <see cref="VagrantBoxUpdateCommandBuilder"/>.
/// </summary>
public static class VagrantBoxUpdateCommandTest
{
    /// <summary>Configures and builds a valid command, asserting success.</summary>
    public static VagrantBoxUpdateCommand Valid(Action<VagrantBoxUpdateCommandBuilder> configure)
    {
        var builder = new VagrantBoxUpdateCommandBuilder();
        configure(builder);
        return builder.ShouldBuildSuccessfully();
    }

    /// <summary>Configures and builds an invalid command, asserting failure.</summary>
    public static void Invalid(Action<VagrantBoxUpdateCommandBuilder> configure)
    {
        var builder = new VagrantBoxUpdateCommandBuilder();
        configure(builder);
        builder.ShouldBuildFailing();
    }
}

#endregion

#region Cloud Command Test Helpers

/// <summary>
/// Test helper for <see cref="VagrantCloudAuthCommandBuilder"/>.
/// </summary>
public static class VagrantCloudAuthCommandTest
{
    /// <summary>Configures and builds a valid command, asserting success.</summary>
    public static VagrantCloudAuthCommand Valid(Action<VagrantCloudAuthCommandBuilder> configure)
    {
        var builder = new VagrantCloudAuthCommandBuilder();
        configure(builder);
        return builder.ShouldBuildSuccessfully();
    }

    /// <summary>Configures and builds an invalid command, asserting failure.</summary>
    public static void Invalid(Action<VagrantCloudAuthCommandBuilder> configure)
    {
        var builder = new VagrantCloudAuthCommandBuilder();
        configure(builder);
        builder.ShouldBuildFailing();
    }
}

/// <summary>
/// Test helper for <see cref="VagrantCloudBoxCommandBuilder"/>.
/// </summary>
public static class VagrantCloudBoxCommandTest
{
    /// <summary>Configures and builds a valid command, asserting success.</summary>
    public static VagrantCloudBoxCommand Valid(Action<VagrantCloudBoxCommandBuilder> configure)
    {
        var builder = new VagrantCloudBoxCommandBuilder();
        configure(builder);
        return builder.ShouldBuildSuccessfully();
    }

    /// <summary>Configures and builds an invalid command, asserting failure.</summary>
    public static void Invalid(Action<VagrantCloudBoxCommandBuilder> configure)
    {
        var builder = new VagrantCloudBoxCommandBuilder();
        configure(builder);
        builder.ShouldBuildFailing();
    }
}

/// <summary>
/// Test helper for <see cref="VagrantCloudProviderCommandBuilder"/>.
/// </summary>
public static class VagrantCloudProviderCommandTest
{
    /// <summary>Configures and builds a valid command, asserting success.</summary>
    public static VagrantCloudProviderCommand Valid(Action<VagrantCloudProviderCommandBuilder> configure)
    {
        var builder = new VagrantCloudProviderCommandBuilder();
        configure(builder);
        return builder.ShouldBuildSuccessfully();
    }

    /// <summary>Configures and builds an invalid command, asserting failure.</summary>
    public static void Invalid(Action<VagrantCloudProviderCommandBuilder> configure)
    {
        var builder = new VagrantCloudProviderCommandBuilder();
        configure(builder);
        builder.ShouldBuildFailing();
    }
}

/// <summary>
/// Test helper for <see cref="VagrantCloudSearchCommandBuilder"/>.
/// </summary>
public static class VagrantCloudSearchCommandTest
{
    /// <summary>Configures and builds a valid command, asserting success.</summary>
    public static VagrantCloudSearchCommand Valid(Action<VagrantCloudSearchCommandBuilder> configure)
    {
        var builder = new VagrantCloudSearchCommandBuilder();
        configure(builder);
        return builder.ShouldBuildSuccessfully();
    }

    /// <summary>Configures and builds an invalid command, asserting failure.</summary>
    public static void Invalid(Action<VagrantCloudSearchCommandBuilder> configure)
    {
        var builder = new VagrantCloudSearchCommandBuilder();
        configure(builder);
        builder.ShouldBuildFailing();
    }
}

/// <summary>
/// Test helper for <see cref="VagrantCloudVersionCommandBuilder"/>.
/// </summary>
public static class VagrantCloudVersionCommandTest
{
    /// <summary>Configures and builds a valid command, asserting success.</summary>
    public static VagrantCloudVersionCommand Valid(Action<VagrantCloudVersionCommandBuilder> configure)
    {
        var builder = new VagrantCloudVersionCommandBuilder();
        configure(builder);
        return builder.ShouldBuildSuccessfully();
    }

    /// <summary>Configures and builds an invalid command, asserting failure.</summary>
    public static void Invalid(Action<VagrantCloudVersionCommandBuilder> configure)
    {
        var builder = new VagrantCloudVersionCommandBuilder();
        configure(builder);
        builder.ShouldBuildFailing();
    }
}

#endregion

#region Plugin Command Test Helpers

/// <summary>
/// Test helper for <see cref="VagrantPluginInstallCommandBuilder"/>.
/// </summary>
public static class VagrantPluginInstallCommandTest
{
    /// <summary>Configures and builds a valid command, asserting success.</summary>
    public static VagrantPluginInstallCommand Valid(Action<VagrantPluginInstallCommandBuilder> configure)
    {
        var builder = new VagrantPluginInstallCommandBuilder();
        configure(builder);
        return builder.ShouldBuildSuccessfully();
    }

    /// <summary>Configures and builds an invalid command, asserting failure.</summary>
    public static void Invalid(Action<VagrantPluginInstallCommandBuilder> configure)
    {
        var builder = new VagrantPluginInstallCommandBuilder();
        configure(builder);
        builder.ShouldBuildFailing();
    }
}

/// <summary>
/// Test helper for <see cref="VagrantPluginLicenseCommandBuilder"/>.
/// </summary>
public static class VagrantPluginLicenseCommandTest
{
    /// <summary>Configures and builds a valid command, asserting success.</summary>
    public static VagrantPluginLicenseCommand Valid(Action<VagrantPluginLicenseCommandBuilder> configure)
    {
        var builder = new VagrantPluginLicenseCommandBuilder();
        configure(builder);
        return builder.ShouldBuildSuccessfully();
    }

    /// <summary>Configures and builds an invalid command, asserting failure.</summary>
    public static void Invalid(Action<VagrantPluginLicenseCommandBuilder> configure)
    {
        var builder = new VagrantPluginLicenseCommandBuilder();
        configure(builder);
        builder.ShouldBuildFailing();
    }
}

/// <summary>
/// Test helper for <see cref="VagrantPluginListCommandBuilder"/>.
/// </summary>
public static class VagrantPluginListCommandTest
{
    /// <summary>Configures and builds a valid command, asserting success.</summary>
    public static VagrantPluginListCommand Valid(Action<VagrantPluginListCommandBuilder> configure)
    {
        var builder = new VagrantPluginListCommandBuilder();
        configure(builder);
        return builder.ShouldBuildSuccessfully();
    }

    /// <summary>Configures and builds an invalid command, asserting failure.</summary>
    public static void Invalid(Action<VagrantPluginListCommandBuilder> configure)
    {
        var builder = new VagrantPluginListCommandBuilder();
        configure(builder);
        builder.ShouldBuildFailing();
    }
}

/// <summary>
/// Test helper for <see cref="VagrantPluginRepairCommandBuilder"/>.
/// </summary>
public static class VagrantPluginRepairCommandTest
{
    /// <summary>Configures and builds a valid command, asserting success.</summary>
    public static VagrantPluginRepairCommand Valid(Action<VagrantPluginRepairCommandBuilder> configure)
    {
        var builder = new VagrantPluginRepairCommandBuilder();
        configure(builder);
        return builder.ShouldBuildSuccessfully();
    }

    /// <summary>Configures and builds an invalid command, asserting failure.</summary>
    public static void Invalid(Action<VagrantPluginRepairCommandBuilder> configure)
    {
        var builder = new VagrantPluginRepairCommandBuilder();
        configure(builder);
        builder.ShouldBuildFailing();
    }
}

/// <summary>
/// Test helper for <see cref="VagrantPluginUninstallCommandBuilder"/>.
/// </summary>
public static class VagrantPluginUninstallCommandTest
{
    /// <summary>Configures and builds a valid command, asserting success.</summary>
    public static VagrantPluginUninstallCommand Valid(Action<VagrantPluginUninstallCommandBuilder> configure)
    {
        var builder = new VagrantPluginUninstallCommandBuilder();
        configure(builder);
        return builder.ShouldBuildSuccessfully();
    }

    /// <summary>Configures and builds an invalid command, asserting failure.</summary>
    public static void Invalid(Action<VagrantPluginUninstallCommandBuilder> configure)
    {
        var builder = new VagrantPluginUninstallCommandBuilder();
        configure(builder);
        builder.ShouldBuildFailing();
    }
}

/// <summary>
/// Test helper for <see cref="VagrantPluginUpdateCommandBuilder"/>.
/// </summary>
public static class VagrantPluginUpdateCommandTest
{
    /// <summary>Configures and builds a valid command, asserting success.</summary>
    public static VagrantPluginUpdateCommand Valid(Action<VagrantPluginUpdateCommandBuilder> configure)
    {
        var builder = new VagrantPluginUpdateCommandBuilder();
        configure(builder);
        return builder.ShouldBuildSuccessfully();
    }

    /// <summary>Configures and builds an invalid command, asserting failure.</summary>
    public static void Invalid(Action<VagrantPluginUpdateCommandBuilder> configure)
    {
        var builder = new VagrantPluginUpdateCommandBuilder();
        configure(builder);
        builder.ShouldBuildFailing();
    }
}

#endregion

#region Snapshot Command Test Helpers

/// <summary>
/// Test helper for <see cref="VagrantSnapshotDeleteCommandBuilder"/>.
/// </summary>
public static class VagrantSnapshotDeleteCommandTest
{
    /// <summary>Configures and builds a valid command, asserting success.</summary>
    public static VagrantSnapshotDeleteCommand Valid(Action<VagrantSnapshotDeleteCommandBuilder> configure)
    {
        var builder = new VagrantSnapshotDeleteCommandBuilder();
        configure(builder);
        return builder.ShouldBuildSuccessfully();
    }

    /// <summary>Configures and builds an invalid command, asserting failure.</summary>
    public static void Invalid(Action<VagrantSnapshotDeleteCommandBuilder> configure)
    {
        var builder = new VagrantSnapshotDeleteCommandBuilder();
        configure(builder);
        builder.ShouldBuildFailing();
    }
}

/// <summary>
/// Test helper for <see cref="VagrantSnapshotListCommandBuilder"/>.
/// </summary>
public static class VagrantSnapshotListCommandTest
{
    /// <summary>Configures and builds a valid command, asserting success.</summary>
    public static VagrantSnapshotListCommand Valid(Action<VagrantSnapshotListCommandBuilder> configure)
    {
        var builder = new VagrantSnapshotListCommandBuilder();
        configure(builder);
        return builder.ShouldBuildSuccessfully();
    }

    /// <summary>Configures and builds an invalid command, asserting failure.</summary>
    public static void Invalid(Action<VagrantSnapshotListCommandBuilder> configure)
    {
        var builder = new VagrantSnapshotListCommandBuilder();
        configure(builder);
        builder.ShouldBuildFailing();
    }
}

/// <summary>
/// Test helper for <see cref="VagrantSnapshotPopCommandBuilder"/>.
/// </summary>
public static class VagrantSnapshotPopCommandTest
{
    /// <summary>Configures and builds a valid command, asserting success.</summary>
    public static VagrantSnapshotPopCommand Valid(Action<VagrantSnapshotPopCommandBuilder> configure)
    {
        var builder = new VagrantSnapshotPopCommandBuilder();
        configure(builder);
        return builder.ShouldBuildSuccessfully();
    }

    /// <summary>Configures and builds an invalid command, asserting failure.</summary>
    public static void Invalid(Action<VagrantSnapshotPopCommandBuilder> configure)
    {
        var builder = new VagrantSnapshotPopCommandBuilder();
        configure(builder);
        builder.ShouldBuildFailing();
    }
}

/// <summary>
/// Test helper for <see cref="VagrantSnapshotPushCommandBuilder"/>.
/// </summary>
public static class VagrantSnapshotPushCommandTest
{
    /// <summary>Configures and builds a valid command, asserting success.</summary>
    public static VagrantSnapshotPushCommand Valid(Action<VagrantSnapshotPushCommandBuilder> configure)
    {
        var builder = new VagrantSnapshotPushCommandBuilder();
        configure(builder);
        return builder.ShouldBuildSuccessfully();
    }

    /// <summary>Configures and builds an invalid command, asserting failure.</summary>
    public static void Invalid(Action<VagrantSnapshotPushCommandBuilder> configure)
    {
        var builder = new VagrantSnapshotPushCommandBuilder();
        configure(builder);
        builder.ShouldBuildFailing();
    }
}

/// <summary>
/// Test helper for <see cref="VagrantSnapshotRestoreCommandBuilder"/>.
/// </summary>
public static class VagrantSnapshotRestoreCommandTest
{
    /// <summary>Configures and builds a valid command, asserting success.</summary>
    public static VagrantSnapshotRestoreCommand Valid(Action<VagrantSnapshotRestoreCommandBuilder> configure)
    {
        var builder = new VagrantSnapshotRestoreCommandBuilder();
        configure(builder);
        return builder.ShouldBuildSuccessfully();
    }

    /// <summary>Configures and builds an invalid command, asserting failure.</summary>
    public static void Invalid(Action<VagrantSnapshotRestoreCommandBuilder> configure)
    {
        var builder = new VagrantSnapshotRestoreCommandBuilder();
        configure(builder);
        builder.ShouldBuildFailing();
    }
}

/// <summary>
/// Test helper for <see cref="VagrantSnapshotSaveCommandBuilder"/>.
/// </summary>
public static class VagrantSnapshotSaveCommandTest
{
    /// <summary>Configures and builds a valid command, asserting success.</summary>
    public static VagrantSnapshotSaveCommand Valid(Action<VagrantSnapshotSaveCommandBuilder> configure)
    {
        var builder = new VagrantSnapshotSaveCommandBuilder();
        configure(builder);
        return builder.ShouldBuildSuccessfully();
    }

    /// <summary>Configures and builds an invalid command, asserting failure.</summary>
    public static void Invalid(Action<VagrantSnapshotSaveCommandBuilder> configure)
    {
        var builder = new VagrantSnapshotSaveCommandBuilder();
        configure(builder);
        builder.ShouldBuildFailing();
    }
}

#endregion

#region Top-Level Command Test Helpers

/// <summary>
/// Test helper for <see cref="VagrantDestroyCommandBuilder"/>.
/// </summary>
public static class VagrantDestroyCommandTest
{
    /// <summary>Configures and builds a valid command, asserting success.</summary>
    public static VagrantDestroyCommand Valid(Action<VagrantDestroyCommandBuilder> configure)
    {
        var builder = new VagrantDestroyCommandBuilder();
        configure(builder);
        return builder.ShouldBuildSuccessfully();
    }

    /// <summary>Configures and builds an invalid command, asserting failure.</summary>
    public static void Invalid(Action<VagrantDestroyCommandBuilder> configure)
    {
        var builder = new VagrantDestroyCommandBuilder();
        configure(builder);
        builder.ShouldBuildFailing();
    }
}

/// <summary>
/// Test helper for <see cref="VagrantGlobalStatusCommandBuilder"/>.
/// </summary>
public static class VagrantGlobalStatusCommandTest
{
    /// <summary>Configures and builds a valid command, asserting success.</summary>
    public static VagrantGlobalStatusCommand Valid(Action<VagrantGlobalStatusCommandBuilder> configure)
    {
        var builder = new VagrantGlobalStatusCommandBuilder();
        configure(builder);
        return builder.ShouldBuildSuccessfully();
    }

    /// <summary>Configures and builds an invalid command, asserting failure.</summary>
    public static void Invalid(Action<VagrantGlobalStatusCommandBuilder> configure)
    {
        var builder = new VagrantGlobalStatusCommandBuilder();
        configure(builder);
        builder.ShouldBuildFailing();
    }
}

/// <summary>
/// Test helper for <see cref="VagrantHaltCommandBuilder"/>.
/// </summary>
public static class VagrantHaltCommandTest
{
    /// <summary>Configures and builds a valid command, asserting success.</summary>
    public static VagrantHaltCommand Valid(Action<VagrantHaltCommandBuilder> configure)
    {
        var builder = new VagrantHaltCommandBuilder();
        configure(builder);
        return builder.ShouldBuildSuccessfully();
    }

    /// <summary>Configures and builds an invalid command, asserting failure.</summary>
    public static void Invalid(Action<VagrantHaltCommandBuilder> configure)
    {
        var builder = new VagrantHaltCommandBuilder();
        configure(builder);
        builder.ShouldBuildFailing();
    }
}

/// <summary>
/// Test helper for <see cref="VagrantInitCommandBuilder"/>.
/// </summary>
public static class VagrantInitCommandTest
{
    /// <summary>Configures and builds a valid command, asserting success.</summary>
    public static VagrantInitCommand Valid(Action<VagrantInitCommandBuilder> configure)
    {
        var builder = new VagrantInitCommandBuilder();
        configure(builder);
        return builder.ShouldBuildSuccessfully();
    }

    /// <summary>Configures and builds an invalid command, asserting failure.</summary>
    public static void Invalid(Action<VagrantInitCommandBuilder> configure)
    {
        var builder = new VagrantInitCommandBuilder();
        configure(builder);
        builder.ShouldBuildFailing();
    }
}

/// <summary>
/// Test helper for <see cref="VagrantPackageCommandBuilder"/>.
/// </summary>
public static class VagrantPackageCommandTest
{
    /// <summary>Configures and builds a valid command, asserting success.</summary>
    public static VagrantPackageCommand Valid(Action<VagrantPackageCommandBuilder> configure)
    {
        var builder = new VagrantPackageCommandBuilder();
        configure(builder);
        return builder.ShouldBuildSuccessfully();
    }

    /// <summary>Configures and builds an invalid command, asserting failure.</summary>
    public static void Invalid(Action<VagrantPackageCommandBuilder> configure)
    {
        var builder = new VagrantPackageCommandBuilder();
        configure(builder);
        builder.ShouldBuildFailing();
    }
}

/// <summary>
/// Test helper for <see cref="VagrantPortCommandBuilder"/>.
/// </summary>
public static class VagrantPortCommandTest
{
    /// <summary>Configures and builds a valid command, asserting success.</summary>
    public static VagrantPortCommand Valid(Action<VagrantPortCommandBuilder> configure)
    {
        var builder = new VagrantPortCommandBuilder();
        configure(builder);
        return builder.ShouldBuildSuccessfully();
    }

    /// <summary>Configures and builds an invalid command, asserting failure.</summary>
    public static void Invalid(Action<VagrantPortCommandBuilder> configure)
    {
        var builder = new VagrantPortCommandBuilder();
        configure(builder);
        builder.ShouldBuildFailing();
    }
}

/// <summary>
/// Test helper for <see cref="VagrantPowerShellCommandBuilder"/>.
/// </summary>
public static class VagrantPowerShellCommandTest
{
    /// <summary>Configures and builds a valid command, asserting success.</summary>
    public static VagrantPowerShellCommand Valid(Action<VagrantPowerShellCommandBuilder> configure)
    {
        var builder = new VagrantPowerShellCommandBuilder();
        configure(builder);
        return builder.ShouldBuildSuccessfully();
    }

    /// <summary>Configures and builds an invalid command, asserting failure.</summary>
    public static void Invalid(Action<VagrantPowerShellCommandBuilder> configure)
    {
        var builder = new VagrantPowerShellCommandBuilder();
        configure(builder);
        builder.ShouldBuildFailing();
    }
}

/// <summary>
/// Test helper for <see cref="VagrantProvisionCommandBuilder"/>.
/// </summary>
public static class VagrantProvisionCommandTest
{
    /// <summary>Configures and builds a valid command, asserting success.</summary>
    public static VagrantProvisionCommand Valid(Action<VagrantProvisionCommandBuilder> configure)
    {
        var builder = new VagrantProvisionCommandBuilder();
        configure(builder);
        return builder.ShouldBuildSuccessfully();
    }

    /// <summary>Configures and builds an invalid command, asserting failure.</summary>
    public static void Invalid(Action<VagrantProvisionCommandBuilder> configure)
    {
        var builder = new VagrantProvisionCommandBuilder();
        configure(builder);
        builder.ShouldBuildFailing();
    }
}

/// <summary>
/// Test helper for <see cref="VagrantRdpCommandBuilder"/>.
/// </summary>
public static class VagrantRdpCommandTest
{
    /// <summary>Configures and builds a valid command, asserting success.</summary>
    public static VagrantRdpCommand Valid(Action<VagrantRdpCommandBuilder> configure)
    {
        var builder = new VagrantRdpCommandBuilder();
        configure(builder);
        return builder.ShouldBuildSuccessfully();
    }

    /// <summary>Configures and builds an invalid command, asserting failure.</summary>
    public static void Invalid(Action<VagrantRdpCommandBuilder> configure)
    {
        var builder = new VagrantRdpCommandBuilder();
        configure(builder);
        builder.ShouldBuildFailing();
    }
}

/// <summary>
/// Test helper for <see cref="VagrantReloadCommandBuilder"/>.
/// </summary>
public static class VagrantReloadCommandTest
{
    /// <summary>Configures and builds a valid command, asserting success.</summary>
    public static VagrantReloadCommand Valid(Action<VagrantReloadCommandBuilder> configure)
    {
        var builder = new VagrantReloadCommandBuilder();
        configure(builder);
        return builder.ShouldBuildSuccessfully();
    }

    /// <summary>Configures and builds an invalid command, asserting failure.</summary>
    public static void Invalid(Action<VagrantReloadCommandBuilder> configure)
    {
        var builder = new VagrantReloadCommandBuilder();
        configure(builder);
        builder.ShouldBuildFailing();
    }
}

/// <summary>
/// Test helper for <see cref="VagrantResumeCommandBuilder"/>.
/// </summary>
public static class VagrantResumeCommandTest
{
    /// <summary>Configures and builds a valid command, asserting success.</summary>
    public static VagrantResumeCommand Valid(Action<VagrantResumeCommandBuilder> configure)
    {
        var builder = new VagrantResumeCommandBuilder();
        configure(builder);
        return builder.ShouldBuildSuccessfully();
    }

    /// <summary>Configures and builds an invalid command, asserting failure.</summary>
    public static void Invalid(Action<VagrantResumeCommandBuilder> configure)
    {
        var builder = new VagrantResumeCommandBuilder();
        configure(builder);
        builder.ShouldBuildFailing();
    }
}

/// <summary>
/// Test helper for <see cref="VagrantSshCommandBuilder"/>.
/// </summary>
public static class VagrantSshCommandTest
{
    /// <summary>Configures and builds a valid command, asserting success.</summary>
    public static VagrantSshCommand Valid(Action<VagrantSshCommandBuilder> configure)
    {
        var builder = new VagrantSshCommandBuilder();
        configure(builder);
        return builder.ShouldBuildSuccessfully();
    }

    /// <summary>Configures and builds an invalid command, asserting failure.</summary>
    public static void Invalid(Action<VagrantSshCommandBuilder> configure)
    {
        var builder = new VagrantSshCommandBuilder();
        configure(builder);
        builder.ShouldBuildFailing();
    }
}

/// <summary>
/// Test helper for <see cref="VagrantSshConfigCommandBuilder"/>.
/// </summary>
public static class VagrantSshConfigCommandTest
{
    /// <summary>Configures and builds a valid command, asserting success.</summary>
    public static VagrantSshConfigCommand Valid(Action<VagrantSshConfigCommandBuilder> configure)
    {
        var builder = new VagrantSshConfigCommandBuilder();
        configure(builder);
        return builder.ShouldBuildSuccessfully();
    }

    /// <summary>Configures and builds an invalid command, asserting failure.</summary>
    public static void Invalid(Action<VagrantSshConfigCommandBuilder> configure)
    {
        var builder = new VagrantSshConfigCommandBuilder();
        configure(builder);
        builder.ShouldBuildFailing();
    }
}

/// <summary>
/// Test helper for <see cref="VagrantStatusCommandBuilder"/>.
/// </summary>
public static class VagrantStatusCommandTest
{
    /// <summary>Configures and builds a valid command, asserting success.</summary>
    public static VagrantStatusCommand Valid(Action<VagrantStatusCommandBuilder> configure)
    {
        var builder = new VagrantStatusCommandBuilder();
        configure(builder);
        return builder.ShouldBuildSuccessfully();
    }

    /// <summary>Configures and builds an invalid command, asserting failure.</summary>
    public static void Invalid(Action<VagrantStatusCommandBuilder> configure)
    {
        var builder = new VagrantStatusCommandBuilder();
        configure(builder);
        builder.ShouldBuildFailing();
    }
}

/// <summary>
/// Test helper for <see cref="VagrantSuspendCommandBuilder"/>.
/// </summary>
public static class VagrantSuspendCommandTest
{
    /// <summary>Configures and builds a valid command, asserting success.</summary>
    public static VagrantSuspendCommand Valid(Action<VagrantSuspendCommandBuilder> configure)
    {
        var builder = new VagrantSuspendCommandBuilder();
        configure(builder);
        return builder.ShouldBuildSuccessfully();
    }

    /// <summary>Configures and builds an invalid command, asserting failure.</summary>
    public static void Invalid(Action<VagrantSuspendCommandBuilder> configure)
    {
        var builder = new VagrantSuspendCommandBuilder();
        configure(builder);
        builder.ShouldBuildFailing();
    }
}

/// <summary>
/// Test helper for <see cref="VagrantUpCommandBuilder"/>.
/// </summary>
public static class VagrantUpCommandTest
{
    /// <summary>Configures and builds a valid command, asserting success.</summary>
    public static VagrantUpCommand Valid(Action<VagrantUpCommandBuilder> configure)
    {
        var builder = new VagrantUpCommandBuilder();
        configure(builder);
        return builder.ShouldBuildSuccessfully();
    }

    /// <summary>Configures and builds an invalid command, asserting failure.</summary>
    public static void Invalid(Action<VagrantUpCommandBuilder> configure)
    {
        var builder = new VagrantUpCommandBuilder();
        configure(builder);
        builder.ShouldBuildFailing();
    }
}

/// <summary>
/// Test helper for <see cref="VagrantValidateCommandBuilder"/>.
/// </summary>
public static class VagrantValidateCommandTest
{
    /// <summary>Configures and builds a valid command, asserting success.</summary>
    public static VagrantValidateCommand Valid(Action<VagrantValidateCommandBuilder> configure)
    {
        var builder = new VagrantValidateCommandBuilder();
        configure(builder);
        return builder.ShouldBuildSuccessfully();
    }

    /// <summary>Configures and builds an invalid command, asserting failure.</summary>
    public static void Invalid(Action<VagrantValidateCommandBuilder> configure)
    {
        var builder = new VagrantValidateCommandBuilder();
        configure(builder);
        builder.ShouldBuildFailing();
    }
}

/// <summary>
/// Test helper for <see cref="VagrantVersionCommandBuilder"/>.
/// </summary>
public static class VagrantVersionCommandTest
{
    /// <summary>Configures and builds a valid command, asserting success.</summary>
    public static VagrantVersionCommand Valid(Action<VagrantVersionCommandBuilder> configure)
    {
        var builder = new VagrantVersionCommandBuilder();
        configure(builder);
        return builder.ShouldBuildSuccessfully();
    }

    /// <summary>Configures and builds an invalid command, asserting failure.</summary>
    public static void Invalid(Action<VagrantVersionCommandBuilder> configure)
    {
        var builder = new VagrantVersionCommandBuilder();
        configure(builder);
        builder.ShouldBuildFailing();
    }
}

/// <summary>
/// Test helper for <see cref="VagrantWinrmCommandBuilder"/>.
/// </summary>
public static class VagrantWinrmCommandTest
{
    /// <summary>Configures and builds a valid command, asserting success.</summary>
    public static VagrantWinrmCommand Valid(Action<VagrantWinrmCommandBuilder> configure)
    {
        var builder = new VagrantWinrmCommandBuilder();
        configure(builder);
        return builder.ShouldBuildSuccessfully();
    }

    /// <summary>Configures and builds an invalid command, asserting failure.</summary>
    public static void Invalid(Action<VagrantWinrmCommandBuilder> configure)
    {
        var builder = new VagrantWinrmCommandBuilder();
        configure(builder);
        builder.ShouldBuildFailing();
    }
}

/// <summary>
/// Test helper for <see cref="VagrantWinrmConfigCommandBuilder"/>.
/// </summary>
public static class VagrantWinrmConfigCommandTest
{
    /// <summary>Configures and builds a valid command, asserting success.</summary>
    public static VagrantWinrmConfigCommand Valid(Action<VagrantWinrmConfigCommandBuilder> configure)
    {
        var builder = new VagrantWinrmConfigCommandBuilder();
        configure(builder);
        return builder.ShouldBuildSuccessfully();
    }

    /// <summary>Configures and builds an invalid command, asserting failure.</summary>
    public static void Invalid(Action<VagrantWinrmConfigCommandBuilder> configure)
    {
        var builder = new VagrantWinrmConfigCommandBuilder();
        configure(builder);
        builder.ShouldBuildFailing();
    }
}

#endregion
