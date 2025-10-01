using FenchExDev.Net.Testing;
using FrenchExDev.Net.Vagrant.Commands;
using FrenchExDev.Net.Vagrant.Commands.Invocation;
using FrenchExDev.Net.Vagrant.Testing;

namespace FrenchExDev.Net.Vagrant.Tests;

/// <summary>
/// Provides unit tests for all leaf command nodes in the Vagrant command tree, verifying correct invocation
/// construction and expected behavior for both successful and failure scenarios.
/// </summary>
/// <remarks>This test class covers a comprehensive set of Vagrant CLI commands, ensuring that each leaf command
/// is properly parsed and invoked with various combinations of parameters and options. The tests validate both valid
/// and invalid input cases, helping to maintain the reliability of command parsing logic. These tests are intended to
/// be run as part of the automated test suite and do not require external dependencies.</remarks>
[Feature(feature: "vagrant", TestKind.Unit)]
public class AllLeafCommandsTests
{
    /// <summary>
    /// Retrieves the leaf command node associated with the specified command name.
    /// </summary>
    /// <param name="name">The name of the command to locate. Cannot be null or empty.</param>
    /// <returns>The leaf command node corresponding to the specified name.</returns>
    /// <exception cref="InvalidOperationException">Thrown if a leaf command node with the specified name cannot be found.</exception>
    private static LeafCommandNode GetLeaf(string name)
    {
        var tree = VagrantCommandTree.Build();
        if (tree.Children.TryGetValue(name, out var direct) && direct is LeafCommandNode lc1) return lc1;
        foreach (var child in tree.Children.Values)
            if (child is ICommandGroupNode grp && grp.Children.TryGetValue(name, out var found) && found is LeafCommandNode lc) return lc;
        throw new InvalidOperationException($"Command '{name}' not found.");
    }

    /// <summary>
    /// Constructs an Invocation object representing a command with specified parameters and options.
    /// </summary>
    /// <remarks>Parameter and option specifications are parsed from the provided strings. Parameters and
    /// options without values are treated as flags. Invalid or empty specifications are ignored.</remarks>
    /// <param name="command">The name of the command to invoke. Cannot be null or whitespace.</param>
    /// <param name="paramSpec">A semicolon-delimited string specifying parameters and their values. Each parameter is defined as
    /// 'name=value1,value2', or simply 'name' for parameters without values. Can be null or empty if no parameters are
    /// required.</param>
    /// <param name="optionSpec">A semicolon-delimited string specifying options and their values. Each option is defined as 'name=value' for
    /// options with values, or simply 'name' for flag options. Can be null or empty if no options are required.</param>
    /// <returns>An Invocation object initialized with the specified command, parameters, and options.</returns>
    private static Invocation BuildInvocation(string command, string paramSpec, string optionSpec)
    {
        var leaf = GetLeaf(command);
        var inv = new Invocation { Command = leaf };

        if (!string.IsNullOrWhiteSpace(paramSpec))
        {
            foreach (var part in paramSpec.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            {
                var kv = part.Split('=', 2);
                var pName = kv[0];
                var values = kv.Length > 1 && kv[1].Length > 0 ? kv[1].Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries) : Array.Empty<string>();
                if (values.Length == 0) inv.Param(pName); else inv.Param(pName, values);
            }
        }
        if (!string.IsNullOrWhiteSpace(optionSpec))
        {
            foreach (var part in optionSpec.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            {
                var kv = part.Split('=', 2);
                if (kv.Length == 1) inv.Flag(kv[0]); else inv.Option(kv[0], kv[1]);
            }
        }
        return inv;
    }

    /// <summary>
    /// Provides a collection of test cases representing valid command scenarios for box operations.
    /// </summary>
    /// <remarks>Each test case is constructed using the SuccessCase.Create method and covers a variety of box
    /// commands, including 'up', 'add', 'remove', 'list', 'prune', 'repackage', 'init', 'halt', 'destroy', 'status',
    /// 'ssh', 'provision', 'reload', and 'validate'. This method is typically used to supply data for parameterized
    /// unit tests that verify command parsing and formatting logic.</remarks>
    /// <returns>An enumerable sequence of object arrays, each containing a single test case that defines a valid command,
    /// parameters, options, and expected output for box-related operations.</returns>
    public static IEnumerable<object[]> SuccessCases()
    {
        yield return [SuccessCase.Create(id: "up", command: "up", paramSpec: "", optionSpec: "", equalsExpectation: "up")];
        yield return [SuccessCase.Create(id: "up_provider", command: "up", paramSpec: "", optionSpec: "provider=virtualbox", containsCsv: "--provider virtualbox")];
        yield return [SuccessCase.Create(id: "up_full", command: "up", paramSpec: "machine=default", optionSpec: "provider=virtualbox;parallel;destroy-on-error;color=true", containsCsv: "--provider virtualbox;--parallel;--destroy-on-error;--color true;default")];
        yield return [SuccessCase.Create(id: "up_parallel_only", command: "up", paramSpec: "", optionSpec: "parallel", containsCsv: "--parallel")];
        yield return [SuccessCase.Create(id: "up_color_only", command: "up", paramSpec: "", optionSpec: "color=true", containsCsv: "--color true")];
        yield return [SuccessCase.Create(id: "add_min", command: "add", paramSpec: "source=hashicorp/bionic64", optionSpec: "", containsCsv: "hashicorp/bionic64")];
        yield return [SuccessCase.Create(id: "add_named", command: "add", paramSpec: "source=hashicorp/bionic64", optionSpec: "name=bionic-custom", containsCsv: "--name bionic-custom")];
        yield return [SuccessCase.Create(id: "add_checksum_only", command: "add", paramSpec: "source=hashicorp/bionic64", optionSpec: "checksum=abc123", containsCsv: "--checksum abc123")];
        yield return [SuccessCase.Create(id: "add_full", command: "add", paramSpec: "source=hashicorp/bionic64", optionSpec: "checksum=123;checksum-type=sha256;box-version=1.0.0;name=bionic;clean;force", containsCsv: "--checksum 123;--checksum-type sha256;--box-version 1.0.0;--name bionic;--clean;--force;hashicorp/bionic64")];
        yield return [SuccessCase.Create(id: "remove_basic", command: "remove", paramSpec: "name=mybox", optionSpec: "", containsCsv: "remove;mybox")];
        yield return [SuccessCase.Create(id: "remove_with_provider", command: "remove", paramSpec: "name=mybox", optionSpec: "provider=virtualbox", containsCsv: "--provider virtualbox")];
        yield return [SuccessCase.Create(id: "remove_with_box_version", command: "remove", paramSpec: "name=mybox", optionSpec: "box-version=2.0.0", containsCsv: "--box-version 2.0.0")];
        yield return [SuccessCase.Create(id: "remove_force_only", command: "remove", paramSpec: "name=mybox", optionSpec: "force", containsCsv: "--force")];
        yield return [SuccessCase.Create(id: "remove_full", command: "remove", paramSpec: "name=mybox", optionSpec: "provider=virtualbox;box-version=1.2.3;force", containsCsv: "--provider virtualbox;--box-version 1.2.3;--force;mybox")];
        yield return [SuccessCase.Create(id: "list", command: "list", paramSpec: "", optionSpec: "", equalsExpectation: "box list")];
        yield return [SuccessCase.Create(id: "outdated", command: "outdated", paramSpec: "", optionSpec: "", equalsExpectation: "box outdated")];
        yield return [SuccessCase.Create(id: "prune", command: "prune", paramSpec: "", optionSpec: "", equalsExpectation: "box prune")];
        yield return [SuccessCase.Create(id: "prune_force", command: "prune", paramSpec: "", optionSpec: "force", containsCsv: "--force")];
        yield return [SuccessCase.Create(id: "prune_force_dry", command: "prune", paramSpec: "", optionSpec: "dry-run;force", containsCsv: "--dry-run;--force")];
        yield return [SuccessCase.Create(id: "repackage", command: "repackage", paramSpec: "name=mybox;provider=virtualbox;version=1.0.0", optionSpec: "", equalsExpectation: "box repackage mybox virtualbox 1.0.0")];
        yield return [SuccessCase.Create(id: "repackage_output", command: "repackage", paramSpec: "name=mybox;provider=virtualbox;version=1.0.0", optionSpec: "output=out.box", containsCsv: "--output out.box")];
        yield return [SuccessCase.Create(id: "init_basic", command: "init", paramSpec: "", optionSpec: "", equalsExpectation: "init")];
        yield return [SuccessCase.Create(id: "init_box_only", command: "init", paramSpec: "", optionSpec: "box=alpine", containsCsv: "--box alpine")];
        yield return [SuccessCase.Create(id: "init_minimal_only", command: "init", paramSpec: "", optionSpec: "minimal", containsCsv: "--minimal")];
        yield return [SuccessCase.Create(id: "init_full", command: "init", paramSpec: "", optionSpec: "box=alpine;output=Vagrantfile;minimal;force", containsCsv: "--box alpine;--output Vagrantfile;--minimal;--force")];
        yield return [SuccessCase.Create(id: "halt", command: "halt", paramSpec: "", optionSpec: "", equalsExpectation: "halt")];
        yield return [SuccessCase.Create(id: "halt_force", command: "halt", paramSpec: "", optionSpec: "force", containsCsv: "--force")];
        yield return [SuccessCase.Create(id: "destroy", command: "destroy", paramSpec: "", optionSpec: "", equalsExpectation: "destroy")];
        yield return [SuccessCase.Create(id: "destroy_force", command: "destroy", paramSpec: "", optionSpec: "force", containsCsv: "--force")];
        yield return [SuccessCase.Create(id: "status", command: "status", paramSpec: "", optionSpec: "", equalsExpectation: "status")];
        yield return [SuccessCase.Create(id: "ssh_machine", command: "ssh", paramSpec: "machine=default", optionSpec: "", containsCsv: "ssh;default")];
        yield return [SuccessCase.Create(id: "ssh_command", command: "ssh", paramSpec: "machine=default", optionSpec: "command=whoami", containsCsv: "--command whoami;default")];
        yield return [SuccessCase.Create(id: "provision", command: "provision", paramSpec: "", optionSpec: "", equalsExpectation: "provision")];
        yield return [SuccessCase.Create(id: "provision_with", command: "provision", paramSpec: "", optionSpec: "provision-with=shell", containsCsv: "--provision-with shell")];
        yield return [SuccessCase.Create(id: "reload", command: "reload", paramSpec: "", optionSpec: "", equalsExpectation: "reload")];
        yield return [SuccessCase.Create(id: "reload_provider", command: "reload", paramSpec: "", optionSpec: "provider=virtualbox", containsCsv: "--provider virtualbox")];
        yield return [SuccessCase.Create(id: "validate", command: "validate", paramSpec: "", optionSpec: "", equalsExpectation: "validate")];
        yield return [SuccessCase.Create(id: "global_status", command: "global-status", paramSpec: "", optionSpec: "", equalsExpectation: "global-status")];
        yield return [SuccessCase.Create(id: "global_status_prune", command: "global-status", paramSpec: "", optionSpec: "prune", containsCsv: "--prune")];
        yield return [SuccessCase.Create(id: "package_basic", command: "package", paramSpec: "", optionSpec: "output=package.box", containsCsv: "--output package.box")];
        yield return [SuccessCase.Create(id: "port_basic", command: "port", paramSpec: "machine=default", optionSpec: "", containsCsv: "port;default")];
        yield return [SuccessCase.Create(id: "suspend_basic", command: "suspend", paramSpec: "machine=default", optionSpec: "", containsCsv: "suspend;default")];
        yield return [SuccessCase.Create(id: "resume_basic", command: "resume", paramSpec: "machine=default", optionSpec: "", containsCsv: "resume;default")];
        yield return [SuccessCase.Create(id: "snapshot_save", command: "save", paramSpec: "name=snap1", optionSpec: "", containsCsv: "save;snap1")];
        yield return [SuccessCase.Create(id: "snapshot_restore", command: "restore", paramSpec: "name=snap1", optionSpec: "", containsCsv: "restore;snap1")];
        yield return [SuccessCase.Create(id: "snapshot_list", command: "snapshot list", paramSpec: "", optionSpec: "", equalsExpectation: "snapshot list")];
        yield return [SuccessCase.Create(id: "snapshot_delete", command: "delete", paramSpec: "name=snap1", optionSpec: "", containsCsv: "delete;snap1")];
        yield return [SuccessCase.Create(id: "snapshot_push", command: "push", paramSpec: "", optionSpec: "", equalsExpectation: "snapshot push")];
        yield return [SuccessCase.Create(id: "snapshot_pop", command: "pop", paramSpec: "", optionSpec: "", equalsExpectation: "snapshot pop")];
        yield return [SuccessCase.Create(id: "plugin_install", command: "install", paramSpec: "name=myplugin", optionSpec: "", containsCsv: "install;myplugin")];
        yield return [SuccessCase.Create(id: "plugin_uninstall", command: "uninstall", paramSpec: "name=myplugin", optionSpec: "", containsCsv: "uninstall;myplugin")];
        yield return [SuccessCase.Create(id: "plugin_update_all", command: "plugin update", paramSpec: "", optionSpec: "", equalsExpectation: "plugin update")];
        yield return [SuccessCase.Create(id: "plugin_list", command: "plugin list", paramSpec: "", optionSpec: "", equalsExpectation: "plugin list")];
        yield return [SuccessCase.Create(id: "plugin_expunge_force", command: "expunge", paramSpec: "", optionSpec: "force", containsCsv: "--force")];
        yield return [SuccessCase.Create(id: "login", command: "login", paramSpec: "", optionSpec: "", equalsExpectation: "login")];
        yield return [SuccessCase.Create(id: "logout", command: "logout", paramSpec: "", optionSpec: "", equalsExpectation: "logout")];
        yield return [SuccessCase.Create(id: "rsync_basic", command: "rsync", paramSpec: "machine=default", optionSpec: "", containsCsv: "rsync;default")];
        yield return [SuccessCase.Create(id: "rsync_auto", command: "rsync-auto", paramSpec: "machine=default", optionSpec: "", containsCsv: "rsync-auto;default")];
        yield return [SuccessCase.Create(id: "box_update", command: "update", paramSpec: "", optionSpec: "box=alpine", containsCsv: "--box alpine")];
    }

    public static IEnumerable<object[]> FailureCases()
    {
        yield return [FailureCase.Create(id: "add_missing_source", command: "add", paramSpec: "", optionSpec: "")];
        yield return [FailureCase.Create(id: "add_missing_source_with_opts", command: "add", paramSpec: "", optionSpec: "name=bionic")];
        yield return [FailureCase.Create(id: "add_missing_source_with_other_name", command: "add", paramSpec: "", optionSpec: "name=bionic-custom")];
        yield return [FailureCase.Create(id: "add_empty_source_equals", command: "add", paramSpec: "source=", optionSpec: "")];
        yield return [FailureCase.Create(id: "add_empty_source_placeholder", command: "add", paramSpec: "source", optionSpec: "")];
        yield return [FailureCase.Create(id: "add_empty_source_with_flags", command: "add", paramSpec: "source", optionSpec: "force;clean")];
        yield return [FailureCase.Create(id: "remove_missing_name", command: "remove", paramSpec: "", optionSpec: "")];
        yield return [FailureCase.Create(id: "remove_missing_name_with_provider", command: "remove", paramSpec: "", optionSpec: "provider=virtualbox")];
        yield return [FailureCase.Create(id: "remove_only_force", command: "remove", paramSpec: "", optionSpec: "force")];
        yield return [FailureCase.Create(id: "remove_name_placeholder_no_value", command: "remove", paramSpec: "name", optionSpec: "")];
        yield return [FailureCase.Create(id: "repackage_none", command: "repackage", paramSpec: "", optionSpec: "")];
        yield return [FailureCase.Create(id: "repackage_missing_provider_version", command: "repackage", paramSpec: "name=mybox", optionSpec: "")];
        yield return [FailureCase.Create(id: "repackage_missing_version", command: "repackage", paramSpec: "name=mybox;provider=virtualbox", optionSpec: "")];
        yield return [FailureCase.Create(id: "repackage_missing_name", command: "repackage", paramSpec: "provider=virtualbox;version=1.0.0", optionSpec: "")];
        yield return [FailureCase.Create(id: "repackage_only_provider", command: "repackage", paramSpec: "provider=virtualbox", optionSpec: "")];
        yield return [FailureCase.Create(id: "repackage_only_version", command: "repackage", paramSpec: "version=1.0.0", optionSpec: "")];
        yield return [FailureCase.Create(id: "repackage_name_empty_value", command: "repackage", paramSpec: "name=;provider=virtualbox;version=1.0.0", optionSpec: "")];
        yield return [FailureCase.Create(id: "repackage_version_empty_value", command: "repackage", paramSpec: "name=mybox;provider=virtualbox;version=", optionSpec: "")];
        yield return [FailureCase.Create(id: "snapshot_save_missing_name", command: "save", paramSpec: "", optionSpec: "", expected: "Missing required parameter")];
        yield return [FailureCase.Create(id: "snapshot_restore_missing_name", command: "restore", paramSpec: "", optionSpec: "", expected: "Missing required parameter")];
        yield return [FailureCase.Create(id: "snapshot_delete_missing_name", command: "delete", paramSpec: "", optionSpec: "", expected: "Missing required parameter")];
        yield return [FailureCase.Create(id: "plugin_install_missing_name", command: "install", paramSpec: "", optionSpec: "", expected: "Missing required parameter")];
        yield return [FailureCase.Create(id: "plugin_uninstall_missing_name", command: "uninstall", paramSpec: "", optionSpec: "", expected: "Missing required parameter")];
    }

    /// <summary>
    /// Executes a test case that verifies successful command invocation using the provided success scenario.
    /// </summary>
    /// <remarks>This method is used in parameterized unit tests to ensure that various command scenarios
    /// succeed as expected. Each test case is defined by the <paramref name="c"/> parameter and validated through its
    /// assertions.</remarks>
    /// <param name="c">The success case to execute, containing the expected inputs and assertions for a valid command invocation.</param>
    [Theory]
    [MemberData(nameof(SuccessCases))]
    public void Command_Success_Cases(SuccessCase c) => c.AssertInvocation(BuildInvocation);

    /// <summary>
    /// Executes a test case that verifies command invocation failure scenarios using the specified failure case.
    /// </summary>
    /// <remarks>This method is used in parameterized unit tests to ensure that command invocations fail as
    /// expected under various conditions. Each failure case provides its own assertion logic.</remarks>
    /// <param name="c">The failure case to test, which defines the expected command invocation and its failure conditions.</param>
    [Theory]
    [MemberData(nameof(FailureCases))]
    public void Command_Failure_Cases(FailureCase c) => c.AssertInvocation(BuildInvocation);
}
