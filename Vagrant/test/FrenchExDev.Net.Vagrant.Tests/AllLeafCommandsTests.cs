using FenchExDev.Net.Testing;
using FrenchExDev.Net.Vagrant.Commands.Invocation;
using FrenchExDev.Net.Vagrant.Testing;

namespace FrenchExDev.Net.Vagrant.Tests;

[Feature(feature: "vagrant", TestKind.Unit)]
public class AllLeafCommandsTests
{
    private static Invocation Build(string command, string paramSpec, string optionSpec)
        => CommandTestHelper.BuildInvocation(command, paramSpec, optionSpec);

    public static IEnumerable<object[]> SuccessCases()
    {
        // up (root)
        yield return [SuccessCase.Create(id: "up", command: "up", paramSpec: "", optionSpec: "", equalsExpectation: "up")];
        yield return [SuccessCase.Create(id: "up_provider", command: "up", paramSpec: "", optionSpec: "provider=virtualbox", containsCsv: "--provider virtualbox")];
        yield return [SuccessCase.Create(id: "up_full", command: "up", paramSpec: "machine=default", optionSpec: "provider=virtualbox;parallel;destroy-on-error;color=true", containsCsv: "--provider virtualbox;--parallel;--destroy-on-error;--color true;default")];
        yield return [SuccessCase.Create(id: "up_parallel_only", command: "up", paramSpec: "", optionSpec: "parallel", containsCsv: "--parallel")];
        yield return [SuccessCase.Create(id: "up_color_only", command: "up", paramSpec: "", optionSpec: "color=true", containsCsv: "--color true")];
        yield return [SuccessCase.Create(id: "up_color_flag_only", command: "up", paramSpec: "", optionSpec: "color", containsCsv: "--color")];
        yield return [SuccessCase.Create(id: "up_color_false", command: "up", paramSpec: "", optionSpec: "color=false", containsCsv: "--color false")];
        yield return [SuccessCase.Create(id: "up_color_auto", command: "up", paramSpec: "", optionSpec: "color=auto", containsCsv: "--color auto")];
        yield return [SuccessCase.Create(id: "up_destroy_on_error", command: "up", paramSpec: "", optionSpec: "destroy-on-error", containsCsv: "--destroy-on-error")];
        yield return [SuccessCase.Create(id: "up_with_provision", command: "up", paramSpec: "", optionSpec: "provision", containsCsv: "--provision")];
        yield return [SuccessCase.Create(id: "up_with_no_provision", command: "up", paramSpec: "", optionSpec: "no-provision", containsCsv: "--no-provision")];
        yield return [SuccessCase.Create(id: "up_multi_machines", command: "up", paramSpec: "machine=default,db", optionSpec: "parallel", containsCsv: "default;db")];
        // added combos
        yield return [SuccessCase.Create(id: "up_multi_machines_provider_color_auto", command: "up", paramSpec: "machine=default,db", optionSpec: "provider=virtualbox;color=auto", containsCsv: "--provider virtualbox;--color auto;default;db")];
        yield return [SuccessCase.Create(id: "up_multi_machines_destroy_on_error", command: "up", paramSpec: "machine=default,db", optionSpec: "destroy-on-error", containsCsv: "default;db;--destroy-on-error")];
        yield return [SuccessCase.Create(id: "up_provider_destroy_on_error", command: "up", paramSpec: "", optionSpec: "provider=virtualbox;destroy-on-error", containsCsv: "--provider virtualbox;--destroy-on-error")];

        // box group
        yield return [SuccessCase.Create(id: "box_add_min", command: "box add", paramSpec: "source=hashicorp/bionic64", optionSpec: "", containsCsv: "hashicorp/bionic64")];
        yield return [SuccessCase.Create(id: "box_add_named", command: "box add", paramSpec: "source=hashicorp/bionic64", optionSpec: "name=bionic-custom", containsCsv: "--name bionic-custom")];
        yield return [SuccessCase.Create(id: "box_add_checksum_only", command: "box add", paramSpec: "source=hashicorp/bionic64", optionSpec: "checksum=abc123", containsCsv: "--checksum abc123")];
        yield return [SuccessCase.Create(id: "box_add_checksum_sha1", command: "box add", paramSpec: "source=hashicorp/bionic64", optionSpec: "checksum=aa11;checksum-type=sha1", containsCsv: "--checksum aa11;--checksum-type sha1")];
        yield return [SuccessCase.Create(id: "box_add_checksum_md5", command: "box add", paramSpec: "source=hashicorp/bionic64", optionSpec: "checksum=bb22;checksum-type=md5", containsCsv: "--checksum bb22;--checksum-type md5")];
        yield return [SuccessCase.Create(id: "box_add_checksum_min", command: "box add", paramSpec: "source=hashicorp/bionic64", optionSpec: "checksum=xyz;checksum-type=sha256", containsCsv: "--checksum xyz;--checksum-type sha256")];
        yield return [SuccessCase.Create(id: "box_add_full", command: "box add", paramSpec: "source=hashicorp/bionic64", optionSpec: "checksum=123;checksum-type=sha256;box-version=1.0.0;name=bionic;clean;force", containsCsv: "--checksum 123;--checksum-type sha256;--box-version 1.0.0;--name bionic;--clean;--force;hashicorp/bionic64")];
        // added minimal variant combos
        yield return [SuccessCase.Create(id: "box_add_force_only", command: "box add", paramSpec: "source=hashicorp/bionic64", optionSpec: "force", containsCsv: "--force")];
        yield return [SuccessCase.Create(id: "box_add_clean_only", command: "box add", paramSpec: "source=hashicorp/bionic64", optionSpec: "clean", containsCsv: "--clean")];
        yield return [SuccessCase.Create(id: "box_add_checksum_sha512", command: "box add", paramSpec: "source=hashicorp/bionic64", optionSpec: "checksum=ffff;checksum-type=sha512", containsCsv: "--checksum ffff;--checksum-type sha512")];
        yield return [SuccessCase.Create(id: "box_remove_basic", command: "box remove", paramSpec: "name=mybox", optionSpec: "", containsCsv: "remove;mybox")];
        yield return [SuccessCase.Create(id: "box_remove_with_provider", command: "box remove", paramSpec: "name=mybox", optionSpec: "provider=virtualbox", containsCsv: "--provider virtualbox")];
        yield return [SuccessCase.Create(id: "box_remove_with_box_version", command: "box remove", paramSpec: "name=mybox", optionSpec: "box-version=2.0.0", containsCsv: "--box-version 2.0.0")];
        yield return [SuccessCase.Create(id: "box_remove_force_only", command: "box remove", paramSpec: "name=mybox", optionSpec: "force", containsCsv: "--force")];
        yield return [SuccessCase.Create(id: "box_remove_all", command: "box remove", paramSpec: "name=mybox", optionSpec: "all", containsCsv: "--all")];
        yield return [SuccessCase.Create(id: "box_remove_provider_all", command: "box remove", paramSpec: "name=mybox", optionSpec: "provider=virtualbox;all", containsCsv: "--provider virtualbox;--all")];
        yield return [SuccessCase.Create(id: "box_list", command: "box list", paramSpec: "", optionSpec: "", equalsExpectation: "box list")];
        yield return [SuccessCase.Create(id: "box_list_provider", command: "box list", paramSpec: "", optionSpec: "provider=virtualbox", containsCsv: "--provider virtualbox")];
        yield return [SuccessCase.Create(id: "box_list_force", command: "box list", paramSpec: "", optionSpec: "force", containsCsv: "--force")];
        yield return [SuccessCase.Create(id: "box_list_provider_force", command: "box list", paramSpec: "", optionSpec: "provider=virtualbox;force", containsCsv: "--provider virtualbox;--force")];
        yield return [SuccessCase.Create(id: "box_outdated", command: "box outdated", paramSpec: "", optionSpec: "", equalsExpectation: "box outdated")];
        yield return [SuccessCase.Create(id: "box_outdated_provider", command: "box outdated", paramSpec: "", optionSpec: "provider=virtualbox", containsCsv: "--provider virtualbox")];
        yield return [SuccessCase.Create(id: "box_outdated_global", command: "box outdated", paramSpec: "", optionSpec: "global", containsCsv: "--global")];
        yield return [SuccessCase.Create(id: "box_outdated_insecure", command: "box outdated", paramSpec: "", optionSpec: "insecure", containsCsv: "--insecure")];
        yield return [SuccessCase.Create(id: "box_outdated_both", command: "box outdated", paramSpec: "", optionSpec: "global;insecure", containsCsv: "--global;--insecure")];
        // added outdated combos
        yield return [SuccessCase.Create(id: "box_outdated_provider_global", command: "box outdated", paramSpec: "", optionSpec: "provider=virtualbox;global", containsCsv: "--provider virtualbox;--global")];
        yield return [SuccessCase.Create(id: "box_outdated_provider_insecure", command: "box outdated", paramSpec: "", optionSpec: "provider=virtualbox;insecure", containsCsv: "--provider virtualbox;--insecure")];
        yield return [SuccessCase.Create(id: "box_outdated_provider_global_insecure", command: "box outdated", paramSpec: "", optionSpec: "provider=virtualbox;global;insecure", containsCsv: "--provider virtualbox;--global;--insecure")];
        yield return [SuccessCase.Create(id: "box_prune", command: "box prune", paramSpec: "", optionSpec: "", equalsExpectation: "box prune")];
        yield return [SuccessCase.Create(id: "box_prune_force", command: "box prune", paramSpec: "", optionSpec: "force", containsCsv: "--force")];
        yield return [SuccessCase.Create(id: "box_prune_force_dry", command: "box prune", paramSpec: "", optionSpec: "dry-run;force", containsCsv: "--dry-run;--force")];
        yield return [SuccessCase.Create(id: "box_prune_keep_active_provider", command: "box prune", paramSpec: "", optionSpec: "keep-active-provider", containsCsv: "--keep-active-provider")];
        yield return [SuccessCase.Create(id: "box_prune_full", command: "box prune", paramSpec: "", optionSpec: "dry-run;keep-active-provider;force", containsCsv: "--dry-run;--keep-active-provider;--force")];
        // added prune combos
        yield return [SuccessCase.Create(id: "box_prune_dry_run_only", command: "box prune", paramSpec: "", optionSpec: "dry-run", containsCsv: "--dry-run")];
        yield return [SuccessCase.Create(id: "box_prune_dry_run_keep_active", command: "box prune", paramSpec: "", optionSpec: "dry-run;keep-active-provider", containsCsv: "--dry-run;--keep-active-provider")];
        yield return [SuccessCase.Create(id: "box_repackage", command: "box repackage", paramSpec: "name=mybox;provider=virtualbox;version=1.0.0", optionSpec: "", equalsExpectation: "box repackage mybox virtualbox 1.0.0")];
        yield return [SuccessCase.Create(id: "box_repackage_output", command: "box repackage", paramSpec: "name=mybox;provider=virtualbox;version=1.0.0", optionSpec: "output=out.box", containsCsv: "--output out.box")];
        yield return [SuccessCase.Create(id: "box_update", command: "box update", paramSpec: "", optionSpec: "box=alpine", containsCsv: "--box alpine")];
        yield return [SuccessCase.Create(id: "box_update_provider", command: "box update", paramSpec: "", optionSpec: "provider=virtualbox", containsCsv: "--provider virtualbox")];
        yield return [SuccessCase.Create(id: "box_update_box_provider", command: "box update", paramSpec: "", optionSpec: "box=alpine;provider=virtualbox", containsCsv: "--box alpine;--provider virtualbox")];

        // init (root)
        yield return [SuccessCase.Create(id: "init_basic", command: "init", paramSpec: "", optionSpec: "", equalsExpectation: "init")];
        yield return [SuccessCase.Create(id: "init_box_only", command: "init", paramSpec: "", optionSpec: "box=alpine", containsCsv: "--box alpine")];
        yield return [SuccessCase.Create(id: "init_minimal_only", command: "init", paramSpec: "", optionSpec: "minimal", containsCsv: "--minimal")];
        yield return [SuccessCase.Create(id: "init_full", command: "init", paramSpec: "", optionSpec: "box=alpine;output=Vagrantfile;minimal;force", containsCsv: "--box alpine;--output Vagrantfile;--minimal;--force")];
        yield return [SuccessCase.Create(id: "init_with_box_name_param", command: "init", paramSpec: "box-name=mybox", optionSpec: "box=alpine", containsCsv: "mybox")];
        yield return [SuccessCase.Create(id: "init_with_box_url_param", command: "init", paramSpec: "box-url=http://example", optionSpec: "", containsCsv: "http://example")];
        // added init combos
        yield return [SuccessCase.Create(id: "init_output_only", command: "init", paramSpec: "", optionSpec: "output=Vagrantfile", containsCsv: "--output Vagrantfile")];
        yield return [SuccessCase.Create(id: "init_force_only", command: "init", paramSpec: "", optionSpec: "force", containsCsv: "--force")];
        yield return [SuccessCase.Create(id: "init_minimal_force", command: "init", paramSpec: "", optionSpec: "minimal;force", containsCsv: "--minimal;--force")];
        yield return [SuccessCase.Create(id: "init_box_force", command: "init", paramSpec: "", optionSpec: "box=alpine;force", containsCsv: "--box alpine;--force")];

        // other root commands
        yield return [SuccessCase.Create(id: "halt", command: "halt", paramSpec: "", optionSpec: "", equalsExpectation: "halt")];
        yield return [SuccessCase.Create(id: "halt_force", command: "halt", paramSpec: "", optionSpec: "force", containsCsv: "--force")];
        yield return [SuccessCase.Create(id: "halt_multi_machines", command: "halt", paramSpec: "machine=default,db", optionSpec: "force", containsCsv: "default;db")];
        // added halt multi without force
        yield return [SuccessCase.Create(id: "halt_multi_machines_no_force", command: "halt", paramSpec: "machine=default,db", optionSpec: "", containsCsv: "default;db")];
        yield return [SuccessCase.Create(id: "destroy", command: "destroy", paramSpec: "", optionSpec: "", equalsExpectation: "destroy")];
        yield return [SuccessCase.Create(id: "destroy_force", command: "destroy", paramSpec: "", optionSpec: "force", containsCsv: "--force")];
        yield return [SuccessCase.Create(id: "destroy_graceful", command: "destroy", paramSpec: "", optionSpec: "graceful", containsCsv: "--graceful")];
        yield return [SuccessCase.Create(id: "destroy_multi_machines", command: "destroy", paramSpec: "machine=default,db", optionSpec: "force", containsCsv: "default;db")];
        // added destroy multi + graceful
        yield return [SuccessCase.Create(id: "destroy_multi_machines_graceful", command: "destroy", paramSpec: "machine=default,db", optionSpec: "graceful", containsCsv: "default;db;--graceful")];
        yield return [SuccessCase.Create(id: "status", command: "status", paramSpec: "", optionSpec: "", equalsExpectation: "status")];
        yield return [SuccessCase.Create(id: "status_machine_readable", command: "status", paramSpec: "", optionSpec: "machine-readable", containsCsv: "--machine-readable")];
        yield return [SuccessCase.Create(id: "status_machine_param", command: "status", paramSpec: "machine=default", optionSpec: "", containsCsv: "default")];
        yield return [SuccessCase.Create(id: "status_multi_machines", command: "status", paramSpec: "machine=default,db", optionSpec: "", containsCsv: "default;db")];
        yield return [SuccessCase.Create(id: "ssh_machine", command: "ssh", paramSpec: "machine=default", optionSpec: "", containsCsv: "ssh;default")];
        yield return [SuccessCase.Create(id: "ssh_command_param_form", command: "ssh", paramSpec: "machine=default;command=whoami", optionSpec: "", containsCsv: "whoami")];
        yield return [SuccessCase.Create(id: "ssh_command_option_form", command: "ssh", paramSpec: "machine=default", optionSpec: "command=whoami", containsCsv: "--command whoami")];
        yield return [SuccessCase.Create(id: "ssh_command_option_plain", command: "ssh", paramSpec: "machine=default", optionSpec: "command=whoami;plain", containsCsv: "--command whoami;--plain")];
        yield return [SuccessCase.Create(id: "ssh_plain", command: "ssh", paramSpec: "machine=default", optionSpec: "plain", containsCsv: "--plain")];
        // added ssh param + plain
        yield return [SuccessCase.Create(id: "ssh_command_param_plain", command: "ssh", paramSpec: "machine=default;command=whoami", optionSpec: "plain", containsCsv: "whoami;--plain")];
        yield return [SuccessCase.Create(id: "ssh_config_basic", command: "ssh-config", paramSpec: "", optionSpec: "", equalsExpectation: "ssh-config")];
        yield return [SuccessCase.Create(id: "ssh_config_machine", command: "ssh-config", paramSpec: "machine=default", optionSpec: "", containsCsv: "default")];
        yield return [SuccessCase.Create(id: "ssh_config_host", command: "ssh-config", paramSpec: "", optionSpec: "host=myhost", containsCsv: "--host myhost")];
        yield return [SuccessCase.Create(id: "ssh_config_machine_host", command: "ssh-config", paramSpec: "machine=default", optionSpec: "host=myhost", containsCsv: "default;--host myhost")];
        yield return [SuccessCase.Create(id: "provision", command: "provision", paramSpec: "", optionSpec: "", equalsExpectation: "provision")];
        yield return [SuccessCase.Create(id: "provision_with", command: "provision", paramSpec: "", optionSpec: "provision-with=shell", containsCsv: "--provision-with shell")];
        yield return [SuccessCase.Create(id: "provision_machine", command: "provision", paramSpec: "machine=default", optionSpec: "", containsCsv: "default")];
        yield return [SuccessCase.Create(id: "provision_machine_with", command: "provision", paramSpec: "machine=default", optionSpec: "provision-with=shell", containsCsv: "default;--provision-with shell")];
        // added multi provision-with csv combos
        yield return [SuccessCase.Create(id: "provision_with_multi", command: "provision", paramSpec: "", optionSpec: "provision-with=shell,ansible", containsCsv: "--provision-with shell,ansible")];
        yield return [SuccessCase.Create(id: "provision_machine_with_multi", command: "provision", paramSpec: "machine=default", optionSpec: "provision-with=shell,ansible", containsCsv: "default;--provision-with shell,ansible")];
        yield return [SuccessCase.Create(id: "reload", command: "reload", paramSpec: "", optionSpec: "", equalsExpectation: "reload")];
        yield return [SuccessCase.Create(id: "reload_provider", command: "reload", paramSpec: "", optionSpec: "provider=virtualbox", containsCsv: "--provider virtualbox")];
        yield return [SuccessCase.Create(id: "reload_provision", command: "reload", paramSpec: "", optionSpec: "provision", containsCsv: "--provision")];
        yield return [SuccessCase.Create(id: "reload_no_provision", command: "reload", paramSpec: "", optionSpec: "no-provision", containsCsv: "--no-provision")];
        yield return [SuccessCase.Create(id: "reload_provider_provision", command: "reload", paramSpec: "", optionSpec: "provider=virtualbox;provision", containsCsv: "--provider virtualbox;--provision")];
        yield return [SuccessCase.Create(id: "reload_multi_machines_provider", command: "reload", paramSpec: "machine=default,db", optionSpec: "provider=virtualbox", containsCsv: "default;db;--provider virtualbox")];
        // added reload combos
        yield return [SuccessCase.Create(id: "reload_provider_no_provision", command: "reload", paramSpec: "", optionSpec: "provider=virtualbox;no-provision", containsCsv: "--provider virtualbox;--no-provision")];
        yield return [SuccessCase.Create(id: "reload_multi_machines_provision", command: "reload", paramSpec: "machine=default,db", optionSpec: "provision", containsCsv: "default;db;--provision")];
        yield return [SuccessCase.Create(id: "reload_multi_machines_no_provision", command: "reload", paramSpec: "machine=default,db", optionSpec: "no-provision", containsCsv: "default;db;--no-provision")];
        yield return [SuccessCase.Create(id: "validate", command: "validate", paramSpec: "", optionSpec: "", equalsExpectation: "validate")];
        yield return [SuccessCase.Create(id: "global_status", command: "global-status", paramSpec: "", optionSpec: "", equalsExpectation: "global-status")];
        yield return [SuccessCase.Create(id: "global_status_prune", command: "global-status", paramSpec: "", optionSpec: "prune", containsCsv: "--prune")];
        yield return [SuccessCase.Create(id: "package_basic", command: "package", paramSpec: "", optionSpec: "output=package.box", containsCsv: "--output package.box")];
        yield return [SuccessCase.Create(id: "package_base", command: "package", paramSpec: "", optionSpec: "base=vm1", containsCsv: "--base vm1")];
        yield return [SuccessCase.Create(id: "package_include", command: "package", paramSpec: "", optionSpec: "include=file1.txt", containsCsv: "--include file1.txt")];
        yield return [SuccessCase.Create(id: "package_vagrantfile", command: "package", paramSpec: "", optionSpec: "vagrantfile=Vagrantfile.custom", containsCsv: "--vagrantfile Vagrantfile.custom")];
        yield return [SuccessCase.Create(id: "package_full_combo", command: "package", paramSpec: "", optionSpec: "output=package.box;base=vm1;include=file1.txt;vagrantfile=Vagrantfile.custom", containsCsv: "--output package.box;--base vm1;--include file1.txt;--vagrantfile Vagrantfile.custom")];
        // added bare package (no options)
        yield return [SuccessCase.Create(id: "package_bare", command: "package", paramSpec: "", optionSpec: "", equalsExpectation: "package")];
        yield return [SuccessCase.Create(id: "port_basic", command: "port", paramSpec: "machine=default", optionSpec: "", containsCsv: "port;default")];
        yield return [SuccessCase.Create(id: "port_no_machine", command: "port", paramSpec: "", optionSpec: "", equalsExpectation: "port")];
        yield return [SuccessCase.Create(id: "suspend_basic", command: "suspend", paramSpec: "machine=default", optionSpec: "", containsCsv: "suspend;default")];
        yield return [SuccessCase.Create(id: "resume_basic", command: "resume", paramSpec: "machine=default", optionSpec: "", containsCsv: "resume;default")];

        // snapshot group
        yield return [SuccessCase.Create(id: "snapshot_save", command: "snapshot save", paramSpec: "name=snap1", optionSpec: "", containsCsv: "save;snap1")];
        yield return [SuccessCase.Create(id: "snapshot_save_machine", command: "snapshot save", paramSpec: "machine=default;name=snap1", optionSpec: "", containsCsv: "default")];
        yield return [SuccessCase.Create(id: "snapshot_restore", command: "snapshot restore", paramSpec: "name=snap1", optionSpec: "", containsCsv: "restore;snap1")];
        yield return [SuccessCase.Create(id: "snapshot_restore_machine", command: "snapshot restore", paramSpec: "machine=default;name=snap1", optionSpec: "", containsCsv: "default")];
        yield return [SuccessCase.Create(id: "snapshot_list", command: "snapshot list", paramSpec: "", optionSpec: "", equalsExpectation: "snapshot list")];
        yield return [SuccessCase.Create(id: "snapshot_list_machine", command: "snapshot list", paramSpec: "machine=default", optionSpec: "", containsCsv: "default")];
        yield return [SuccessCase.Create(id: "snapshot_delete", command: "snapshot delete", paramSpec: "name=snap1", optionSpec: "", containsCsv: "delete;snap1")];
        yield return [SuccessCase.Create(id: "snapshot_delete_machine", command: "snapshot delete", paramSpec: "machine=default;name=snap1", optionSpec: "", containsCsv: "default")];
        yield return [SuccessCase.Create(id: "snapshot_push", command: "snapshot push", paramSpec: "", optionSpec: "", equalsExpectation: "snapshot push")];
        yield return [SuccessCase.Create(id: "snapshot_push_machine", command: "snapshot push", paramSpec: "machine=default", optionSpec: "", containsCsv: "default")];
        yield return [SuccessCase.Create(id: "snapshot_pop", command: "snapshot pop", paramSpec: "", optionSpec: "", equalsExpectation: "snapshot pop")];
        yield return [SuccessCase.Create(id: "snapshot_pop_machine", command: "snapshot pop", paramSpec: "machine=default", optionSpec: "", containsCsv: "default")];

        // plugin group
        yield return [SuccessCase.Create(id: "plugin_install", command: "plugin install", paramSpec: "name=myplugin", optionSpec: "", containsCsv: "install;myplugin")];
        yield return [SuccessCase.Create(id: "plugin_install_version", command: "plugin install", paramSpec: "name=myplugin", optionSpec: "plugin-version=1.2.3", containsCsv: "--plugin-version 1.2.3")];
        yield return [SuccessCase.Create(id: "plugin_install_local", command: "plugin install", paramSpec: "name=myplugin", optionSpec: "local", containsCsv: "--local")];
        yield return [SuccessCase.Create(id: "plugin_install_version_local", command: "plugin install", paramSpec: "name=myplugin", optionSpec: "plugin-version=1.2.3;local", containsCsv: "--plugin-version 1.2.3;--local")];
        yield return [SuccessCase.Create(id: "plugin_uninstall", command: "plugin uninstall", paramSpec: "name=myplugin", optionSpec: "", containsCsv: "uninstall;myplugin")];
        yield return [SuccessCase.Create(id: "plugin_update_all", command: "plugin update", paramSpec: "", optionSpec: "", equalsExpectation: "plugin update")];
        yield return [SuccessCase.Create(id: "plugin_update_specific", command: "plugin update", paramSpec: "name=myplugin", optionSpec: "", containsCsv: "myplugin")];
        yield return [SuccessCase.Create(id: "plugin_list", command: "plugin list", paramSpec: "", optionSpec: "", equalsExpectation: "plugin list")];
        yield return [SuccessCase.Create(id: "plugin_expunge_force", command: "plugin expunge", paramSpec: "", optionSpec: "force", containsCsv: "--force")];
        yield return [SuccessCase.Create(id: "plugin_expunge_reinstall", command: "plugin expunge", paramSpec: "", optionSpec: "reinstall", containsCsv: "--reinstall")];
        yield return [SuccessCase.Create(id: "plugin_expunge_full", command: "plugin expunge", paramSpec: "", optionSpec: "force;reinstall", containsCsv: "--force;--reinstall")];

        // auth & sync
        yield return [SuccessCase.Create(id: "login", command: "login", paramSpec: "", optionSpec: "", equalsExpectation: "login")];
        yield return [SuccessCase.Create(id: "logout", command: "logout", paramSpec: "", optionSpec: "", equalsExpectation: "logout")];
        yield return [SuccessCase.Create(id: "rsync_basic", command: "rsync", paramSpec: "machine=default", optionSpec: "", containsCsv: "rsync;default")];
        yield return [SuccessCase.Create(id: "rsync_no_machine", command: "rsync", paramSpec: "", optionSpec: "", equalsExpectation: "rsync")];
        yield return [SuccessCase.Create(id: "rsync_auto", command: "rsync-auto", paramSpec: "machine=default", optionSpec: "", containsCsv: "rsync-auto;default")];
        yield return [SuccessCase.Create(id: "rsync_auto_no_machine", command: "rsync-auto", paramSpec: "", optionSpec: "", equalsExpectation: "rsync-auto")];
    }

    public static IEnumerable<object[]> FailureCases()
    {
        // up failures
        yield return [FailureCase.Create(id: "up_provision_conflict", command: "up", paramSpec: "", optionSpec: "provision;no-provision")];
        yield return [FailureCase.Create(id: "up_provider_empty", command: "up", paramSpec: "", optionSpec: "provider=")];
        yield return [FailureCase.Create(id: "up_color_invalid", command: "up", paramSpec: "", optionSpec: "color=purple")];
        yield return [FailureCase.Create(id: "up_color_empty", command: "up", paramSpec: "", optionSpec: "color=")];
        yield return [FailureCase.Create(id: "up_machine_empty_value", command: "up", paramSpec: "machine=", optionSpec: "")];
        yield return [FailureCase.Create(id: "up_machine_placeholder", command: "up", paramSpec: "machine", optionSpec: "")];

        // box group failures
        yield return [FailureCase.Create(id: "box_add_missing_source", command: "box add", paramSpec: "", optionSpec: "")];
        yield return [FailureCase.Create(id: "box_add_missing_source_with_opts", command: "box add", paramSpec: "", optionSpec: "name=bionic")];
        yield return [FailureCase.Create(id: "box_add_missing_source_with_other_name", command: "box add", paramSpec: "", optionSpec: "name=bionic-custom")];
        yield return [FailureCase.Create(id: "box_add_empty_source_equals", command: "box add", paramSpec: "source=", optionSpec: "")];
        yield return [FailureCase.Create(id: "box_add_empty_source_placeholder", command: "box add", paramSpec: "source", optionSpec: "")];
        yield return [FailureCase.Create(id: "box_add_empty_source_with_flags", command: "box add", paramSpec: "source", optionSpec: "force;clean")];
        yield return [FailureCase.Create(id: "box_add_checksum_type_without_checksum", command: "box add", paramSpec: "source=hashicorp/bionic64", optionSpec: "checksum-type=sha256")];
        yield return [FailureCase.Create(id: "box_add_checksum_empty", command: "box add", paramSpec: "source=hashicorp/bionic64", optionSpec: "checksum=")];
        yield return [FailureCase.Create(id: "box_add_checksum_type_empty", command: "box add", paramSpec: "source=hashicorp/bionic64", optionSpec: "checksum-type=")];
        yield return [FailureCase.Create(id: "box_add_checksum_type_unsupported", command: "box add", paramSpec: "source=hashicorp/bionic64", optionSpec: "checksum=abc;checksum-type=crc32")];
        yield return [FailureCase.Create(id: "box_add_box_version_empty", command: "box add", paramSpec: "source=hashicorp/bionic64", optionSpec: "box-version=")];
        // new placeholder failure for checksum-type
        yield return [FailureCase.Create(id: "box_add_checksum_type_placeholder", command: "box add", paramSpec: "source=hashicorp/bionic64", optionSpec: "checksum-type")];
        yield return [FailureCase.Create(id: "box_remove_missing_name", command: "box remove", paramSpec: "", optionSpec: "")];
        yield return [FailureCase.Create(id: "box_remove_missing_name_with_provider", command: "box remove", paramSpec: "", optionSpec: "provider=virtualbox")];
        yield return [FailureCase.Create(id: "box_remove_only_force", command: "box remove", paramSpec: "", optionSpec: "force")];
        yield return [FailureCase.Create(id: "box_remove_name_placeholder_no_value", command: "box remove", paramSpec: "name", optionSpec: "")];
        yield return [FailureCase.Create(id: "box_remove_name_empty_value", command: "box remove", paramSpec: "name=", optionSpec: "")];
        yield return [FailureCase.Create(id: "box_remove_all_with_box_version", command: "box remove", paramSpec: "name=mybox", optionSpec: "all;box-version=1.0.0")];
        yield return [FailureCase.Create(id: "box_remove_provider_empty", command: "box remove", paramSpec: "name=mybox", optionSpec: "provider=")];
        yield return [FailureCase.Create(id: "box_remove_box_version_empty", command: "box remove", paramSpec: "name=mybox", optionSpec: "box-version=")];
        yield return [FailureCase.Create(id: "box_repackage_none", command: "box repackage", paramSpec: "", optionSpec: "")];
        yield return [FailureCase.Create(id: "box_repackage_missing_provider_version", command: "box repackage", paramSpec: "name=mybox", optionSpec: "")];
        yield return [FailureCase.Create(id: "box_repackage_missing_version", command: "box repackage", paramSpec: "name=mybox;provider=virtualbox", optionSpec: "")];
        yield return [FailureCase.Create(id: "box_repackage_missing_name", command: "box repackage", paramSpec: "provider=virtualbox;version=1.0.0", optionSpec: "")];
        yield return [FailureCase.Create(id: "box_repackage_only_provider", command: "box repackage", paramSpec: "provider=virtualbox", optionSpec: "")];
        yield return [FailureCase.Create(id: "box_repackage_only_version", command: "box repackage", paramSpec: "version=1.0.0", optionSpec: "")];
        yield return [FailureCase.Create(id: "box_repackage_name_empty_value", command: "box repackage", paramSpec: "name=;provider=virtualbox;version=1.0.0", optionSpec: "")];
        yield return [FailureCase.Create(id: "box_repackage_version_empty_value", command: "box repackage", paramSpec: "name=mybox;provider=virtualbox;version=", optionSpec: "")];
        yield return [FailureCase.Create(id: "box_repackage_provider_empty_value", command: "box repackage", paramSpec: "name=mybox;provider=;version=1.0.0", optionSpec: "")];
        yield return [FailureCase.Create(id: "box_repackage_provider_placeholder", command: "box repackage", paramSpec: "name=mybox;provider;version=1.0.0", optionSpec: "")];
        yield return [FailureCase.Create(id: "box_repackage_name_placeholder", command: "box repackage", paramSpec: "name;provider=virtualbox;version=1.0.0", optionSpec: "")];
        yield return [FailureCase.Create(id: "box_repackage_output_empty", command: "box repackage", paramSpec: "name=mybox;provider=virtualbox;version=1.0.0", optionSpec: "output=")];
        yield return [FailureCase.Create(id: "box_update_box_empty", command: "box update", paramSpec: "", optionSpec: "box=")];
        yield return [FailureCase.Create(id: "box_update_provider_empty", command: "box update", paramSpec: "", optionSpec: "provider=")];

        // init failures
        yield return [FailureCase.Create(id: "init_box_empty", command: "init", paramSpec: "", optionSpec: "box=")];
        yield return [FailureCase.Create(id: "init_output_empty", command: "init", paramSpec: "", optionSpec: "output=")];

        // ssh failures
        yield return [FailureCase.Create(id: "ssh_command_both_forms", command: "ssh", paramSpec: "machine=default;command=whoami", optionSpec: "command=whoami")];
        yield return [FailureCase.Create(id: "ssh_command_option_empty", command: "ssh", paramSpec: "machine=default", optionSpec: "command=")];
        yield return [FailureCase.Create(id: "ssh_command_param_empty", command: "ssh", paramSpec: "machine=default;command=", optionSpec: "")];
        yield return [FailureCase.Create(id: "ssh_config_host_empty", command: "ssh-config", paramSpec: "", optionSpec: "host=")];
        // new placeholder failures
        yield return [FailureCase.Create(id: "ssh_command_param_placeholder", command: "ssh", paramSpec: "machine=default;command", optionSpec: "")];
        yield return [FailureCase.Create(id: "ssh_config_host_placeholder", command: "ssh-config", paramSpec: "", optionSpec: "host")];

        // provision failures
        yield return [FailureCase.Create(id: "provision_with_empty", command: "provision", paramSpec: "", optionSpec: "provision-with=")];
        yield return [FailureCase.Create(id: "provision_with_placeholder", command: "provision", paramSpec: "", optionSpec: "provision-with")];

        // reload failures
        yield return [FailureCase.Create(id: "reload_provision_conflict", command: "reload", paramSpec: "", optionSpec: "provision;no-provision")];
        yield return [FailureCase.Create(id: "reload_provider_empty", command: "reload", paramSpec: "", optionSpec: "provider=")];

        // package failures
        yield return [FailureCase.Create(id: "package_output_empty", command: "package", paramSpec: "", optionSpec: "output=")];
        yield return [FailureCase.Create(id: "package_base_empty", command: "package", paramSpec: "", optionSpec: "base=")];
        yield return [FailureCase.Create(id: "package_include_empty", command: "package", paramSpec: "", optionSpec: "include=")];
        yield return [FailureCase.Create(id: "package_vagrantfile_empty", command: "package", paramSpec: "", optionSpec: "vagrantfile=")];

        // snapshot group failures
        yield return [FailureCase.Create(id: "snapshot_save_missing_name", command: "snapshot save", paramSpec: "", optionSpec: "", expected: "Missing required parameter")];
        yield return [FailureCase.Create(id: "snapshot_save_empty_name", command: "snapshot save", paramSpec: "name=", optionSpec: "", expected: "Missing required parameter")];
        yield return [FailureCase.Create(id: "snapshot_save_name_placeholder", command: "snapshot save", paramSpec: "name", optionSpec: "", expected: "Missing required parameter")];
        yield return [FailureCase.Create(id: "snapshot_restore_missing_name", command: "snapshot restore", paramSpec: "", optionSpec: "", expected: "Missing required parameter")];
        yield return [FailureCase.Create(id: "snapshot_restore_empty_name", command: "snapshot restore", paramSpec: "name=", optionSpec: "", expected: "Missing required parameter")];
        yield return [FailureCase.Create(id: "snapshot_restore_name_placeholder", command: "snapshot restore", paramSpec: "name", optionSpec: "", expected: "Missing required parameter")];
        yield return [FailureCase.Create(id: "snapshot_delete_missing_name", command: "snapshot delete", paramSpec: "", optionSpec: "", expected: "Missing required parameter")];
        yield return [FailureCase.Create(id: "snapshot_delete_empty_name", command: "snapshot delete", paramSpec: "name=", optionSpec: "", expected: "Missing required parameter")];
        yield return [FailureCase.Create(id: "snapshot_delete_name_placeholder", command: "snapshot delete", paramSpec: "name", optionSpec: "", expected: "Missing required parameter")];

        // plugin group failures
        yield return [FailureCase.Create(id: "plugin_install_missing_name", command: "plugin install", paramSpec: "", optionSpec: "", expected: "Missing required parameter")];
        yield return [FailureCase.Create(id: "plugin_install_empty_name", command: "plugin install", paramSpec: "name=", optionSpec: "", expected: "Missing required parameter")];
        yield return [FailureCase.Create(id: "plugin_install_name_placeholder", command: "plugin install", paramSpec: "name", optionSpec: "", expected: "Missing required parameter")];
        yield return [FailureCase.Create(id: "plugin_install_version_empty", command: "plugin install", paramSpec: "name=myplugin", optionSpec: "plugin-version=")];
        yield return [FailureCase.Create(id: "plugin_install_version_no_name", command: "plugin install", paramSpec: "", optionSpec: "plugin-version=1.2.3")];
        yield return [FailureCase.Create(id: "plugin_uninstall_missing_name", command: "plugin uninstall", paramSpec: "", optionSpec: "", expected: "Missing required parameter")];
        yield return [FailureCase.Create(id: "plugin_uninstall_empty_name", command: "plugin uninstall", paramSpec: "name=", optionSpec: "", expected: "Missing required parameter")];
        yield return [FailureCase.Create(id: "plugin_uninstall_name_placeholder", command: "plugin uninstall", paramSpec: "name", optionSpec: "", expected: "Missing required parameter")];
        yield return [FailureCase.Create(id: "box_remove_full_invalid_combo", command: "box remove", paramSpec: "name=mybox", optionSpec: "provider=virtualbox;box-version=1.2.3;force;all")];
    }

    [Theory]
    [MemberData(nameof(SuccessCases))]
    public void Command_Success_Cases(SuccessCase c) => c.AssertInvocation(Build);

    [Theory]
    [MemberData(nameof(FailureCases))]
    public void Command_Failure_Cases(FailureCase c) => c.AssertInvocation(Build);
}
