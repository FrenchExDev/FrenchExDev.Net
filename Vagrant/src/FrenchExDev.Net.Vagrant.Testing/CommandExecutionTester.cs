using FrenchExDev.Net.Vagrant.Commands;
using FrenchExDev.Net.Vagrant.Commands.Builders;

namespace FrenchExDev.Net.Vagrant.Testing;

/// <summary>
/// Provides functionality to execute a vagrant command using a builder and verify its output through assertions.
/// </summary>
/// <remarks>
/// This class is typically used in testing scenarios to construct commands, execute them asynchronously, and
/// validate their standard output and error streams. It accepts a builder configuration action and an assertion
/// action to examine the produced outputs.
/// </remarks>
/// <typeparam name="TBuilder">The builder type used to configure the command. Must inherit from <see cref="VagrantCommandBuilder{TBuilder, TCommand}"/>.</typeparam>
/// <typeparam name="TCommand">The command type produced by the builder. Must implement <see cref="IVagrantCommand"/>.</typeparam>
/// <example>
/// <code>
/// var tester = new CommandExecutionTester&lt;UpCommandBuilder, UpCommand&gt;();
/// await tester.ExecuteAsync(b => b.Provider("virtualbox"), (stdout, stderr) => {
///     // assert on stdout / stderr
/// });
/// </code>
/// </example>
public class CommandExecutionTester<TBuilder, TCommand>
    where TCommand : class, IVagrantCommand
    where TBuilder : VagrantCommandBuilder<TBuilder, TCommand>, new()
{
    /// <summary>
    /// Executes a build operation using the specified builder action, runs the resulting process asynchronously, and
    /// applies the provided assertion to its standard output and error streams.
    /// </summary>
    /// <param name="body">Action that configures the builder.</param>
    /// <param name="assert">Assertion action receiving standard output and error wrappers.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>A task that completes after execution and assertion.</returns>
    public async Task ExecuteAsync(Action<TBuilder> body, Action<StdOut, StdErr> assert, CancellationToken cancellationToken = default)
    {
        var builder = new TBuilder();
        body(builder);
        var built = builder.BuildSuccess();
        var executionResult = await built.ToExecutionAsync(cancellationToken);
        assert(executionResult.StdOut, executionResult.StdErr);
    }
}

/// <summary>
/// Tester for executing the <c>vagrant up</c> command using <see cref="UpCommandBuilder"/>.
/// </summary>
/// <remarks>
/// Inherits common behavior from <see cref="CommandExecutionTester{TBuilder,TCommand}"/> and specializes it for
/// the <c>up</c> command.
/// </remarks>
/// <example>
/// <code>
/// var tester = new UpCommandExecutionTester();
/// await tester.ExecuteAsync(b => b.Provider("virtualbox"), (stdout, stderr) => { /* asserts */ });
/// </code>
/// </example>
public class UpCommandExecutionTester : CommandExecutionTester<UpCommandBuilder, UpCommand>
{
}

/// <summary>
/// Tester for executing the <c>vagrant halt</c> command.
/// </summary>
/// <remarks>Use to validate shutdown behavior and output when running <c>vagrant halt</c>.</remarks>
/// <example>
/// <code>
/// var tester = new HaltCommandExecutionTester();
/// await tester.ExecuteAsync(b => b.Force(true), (stdout, stderr) => { /* asserts */ });
/// </code>
/// </example>
public class HaltCommandExecutionTester : CommandExecutionTester<HaltCommandBuilder, HaltCommand>
{
}

/// <summary>
/// Tester for executing the <c>vagrant suspend</c> command.
/// </summary>
/// <remarks>Used to verify suspend/resume lifecycle behavior in tests.</remarks>
/// <example>
/// <code>
/// var tester = new SuspendCommandExecutionTester();
/// await tester.ExecuteAsync(b => { /* configure */ }, (stdout, stderr) => { /* asserts */ });
/// </code>
/// </example>
public class SuspendCommandExecutionTester : CommandExecutionTester<SuspendCommandBuilder, SuspendCommand>
{
}

/// <summary>
/// Tester for executing the <c>vagrant resume</c> command.
/// </summary>
/// <example>
/// <code>
/// var tester = new ResumeCommandExecutionTester();
/// await tester.ExecuteAsync(b => { /* configure */ }, (stdout, stderr) => { /* asserts */ });
/// </code>
/// </example>
public class ResumeCommandExecutionTester : CommandExecutionTester<ResumeCommandBuilder, ResumeCommand>
{
}

/// <summary>
/// Tester for executing the <c>vagrant reload</c> command.
/// </summary>
/// <example>
/// <code>
/// var tester = new ReloadCommandExecutionTester();
/// await tester.ExecuteAsync(b => b.Provider("virtualbox"), (stdout, stderr) => { /* asserts */ });
/// </code>
/// </example>
public class ReloadCommandExecutionTester : CommandExecutionTester<ReloadCommandBuilder, ReloadCommand>
{
}

/// <summary>
/// Tester for executing the <c>vagrant destroy</c> command.
/// </summary>
/// <example>
/// <code>
/// var tester = new DestroyCommandExecutionTester();
/// await tester.ExecuteAsync(b => b.Force(true), (stdout, stderr) => { /* asserts */ });
/// </code>
/// </example>
public class DestroyCommandExecutionTester : CommandExecutionTester<DestroyCommandBuilder, DestroyCommand>
{
}

/// <summary>
/// Tester for executing the <c>vagrant status</c> command.
/// </summary>
/// <example>
/// <code>
/// var tester = new StatusCommandExecutionTester();
/// await tester.ExecuteAsync(b => b.MachineReadable(true), (stdout, stderr) => { /* parse output */ });
/// </code>
/// </example>
public class StatusCommandExecutionTester : CommandExecutionTester<StatusCommandBuilder, StatusCommand>
{
}

/// <summary>
/// Tester for executing the <c>vagrant provision</c> command.
/// </summary>
/// <remarks>Accepts configuration for provisioners via <see cref="ProvisionCommandBuilder"/>.</remarks>
/// <example>
/// <code>
/// var tester = new ProvisionCommandExecutionTester();
/// await tester.ExecuteAsync(b => b.ProvisionWith("shell"), (stdout, stderr) => { /* asserts */ });
/// </code>
/// </example>
public class ProvisionCommandExecutionTester : CommandExecutionTester<ProvisionCommandBuilder, ProvisionCommand>
{
}

/// <summary>
/// Tester for executing the <c>vagrant global-status</c> command.
/// </summary>
/// <example>
/// <code>
/// var tester = new GlobalStatusCommandExecutionTester();
/// await tester.ExecuteAsync(b => b.Prune(true), (stdout, stderr) => { /* asserts */ });
/// </code>
/// </example>
public class GlobalStatusCommandExecutionTester : CommandExecutionTester<GlobalStatusCommandBuilder, GlobalStatusCommand>
{
}

/// <summary>
/// Tester for executing <c>vagrant box add</c> commands.
/// </summary>
/// <example>
/// <code>
/// var tester = new BoxAddCommandExecutionTester();
/// await tester.ExecuteAsync(b => b.UrlOrPath("hashicorp/ubuntu"), (stdout, stderr) => { /* asserts */ });
/// </code>
/// </example>
public class BoxAddCommandExecutionTester : CommandExecutionTester<BoxAddCommandBuilder, BoxAddCommand>
{
}

/// <summary>
/// Tester for executing <c>vagrant box list</c>.
/// </summary>
public class BoxListCommandExecutionTester : CommandExecutionTester<BoxListCommandBuilder, BoxListCommand>
{
}

/// <summary>
/// Tester for executing <c>vagrant box remove</c>.
/// </summary>
public class BoxRemoveCommandExecutionTester : CommandExecutionTester<BoxRemoveCommandBuilder, BoxRemoveCommand>
{
}

/// <summary>
/// Tester for executing <c>vagrant box update</c>.
/// </summary>
public class BoxUpdateCommandExecutionTester : CommandExecutionTester<BoxUpdateCommandBuilder, BoxUpdateCommand>
{
}

/// <summary>
/// Tester for executing <c>vagrant box outdated</c>.
/// </summary>
public class BoxOutdatedCommandExecutionTester : CommandExecutionTester<BoxOutdatedCommandBuilder, BoxOutdatedCommand>
{
}

/// <summary>
/// Tester for executing <c>vagrant box repackage</c>.
/// </summary>
public class BoxRepackageCommandExecutionTester : CommandExecutionTester<BoxRepackageCommandBuilder, BoxRepackageCommand>
{
}

/// <summary>
/// Tester for executing <c>vagrant box prune</c>.
/// </summary>
public class BoxPruneCommandExecutionTester : CommandExecutionTester<BoxPruneCommandBuilder, BoxPruneCommand>
{
}

/// <summary>
/// Tester for executing <c>vagrant plugin install</c>.
/// </summary>
public class PluginInstallCommandExecutionTester : CommandExecutionTester<PluginInstallCommandBuilder, PluginInstallCommand>
{
}

/// <summary>
/// Tester for executing <c>vagrant plugin list</c>.
/// </summary>
public class PluginListCommandExecutionTester : CommandExecutionTester<PluginListCommandBuilder, PluginListCommand>
{
}

/// <summary>
/// Tester for executing <c>vagrant plugin uninstall</c>.
/// </summary>
public class PluginUninstallCommandExecutionTester : CommandExecutionTester<PluginUninstallCommandBuilder, PluginUninstallCommand>
{
}

/// <summary>
/// Tester for executing <c>vagrant version</c>.
/// </summary>
public class VersionCommandExecutionTester : CommandExecutionTester<VersionCommandBuilder, VersionCommand>
{
}

/// <summary>
/// Tester for executing <c>vagrant init</c>.
/// </summary>
public class InitCommandExecutionTester : CommandExecutionTester<InitCommandBuilder, InitCommand>
{
}

/// <summary>
/// Tester for executing <c>vagrant ssh</c>.
/// </summary>
public class SshCommandExecutionTester : CommandExecutionTester<SshCommandBuilder, SshCommand>
{
}

/// <summary>
/// Tester for executing <c>vagrant ssh-config</c>.
/// </summary>
public class SshConfigCommandExecutionTester : CommandExecutionTester<SshConfigCommandBuilder, SshConfigCommand>
{
}

/// <summary>
/// Tester for executing <c>vagrant snapshot list</c>.
/// </summary>
public class SnapshotListCommandExecutionTester : CommandExecutionTester<SnapshotListCommandBuilder, SnapshotListCommand>
{
}

/// <summary>
/// Tester for executing <c>vagrant snapshot save</c>.
/// </summary>
public class SnapshotSaveCommandExecutionTester : CommandExecutionTester<SnapshotSaveCommandBuilder, SnapshotSaveCommand>
{
}

/// <summary>
/// Tester for executing <c>vagrant snapshot restore</c>.
/// </summary>
public class SnapshotRestoreCommandExecutionTester : CommandExecutionTester<SnapshotRestoreCommandBuilder, SnapshotRestoreCommand>
{
}

/// <summary>
/// Tester for executing <c>vagrant snapshot delete</c>.
/// </summary>
public class SnapshotDeleteCommandExecutionTester : CommandExecutionTester<SnapshotDeleteCommandBuilder, SnapshotDeleteCommand>
{
}

/// <summary>
/// Tester for executing <c>vagrant snapshot push</c>.
/// </summary>
public class SnapshotPushCommandExecutionTester : CommandExecutionTester<SnapshotPushCommandBuilder, SnapshotPushCommand>
{
}

/// <summary>
/// Tester for executing <c>vagrant snapshot pop</c>.
/// </summary>
public class SnapshotPopCommandExecutionTester : CommandExecutionTester<SnapshotPopCommandBuilder, SnapshotPopCommand>
{
}
