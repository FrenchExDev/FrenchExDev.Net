using ByteSizeLib;
using Figgle;
using FrenchExDev.Net.CSharp.Object.Builder2;
using FrenchExDev.Net.Packer.Bundle;
using System.Globalization;

namespace FrenchExDev.Net.Packer.Alpine.Abstractions;

/// <summary>
/// Provides a builder for creating Packer bundles that generate Alpine Linux Vagrant boxes using VirtualBox. This class
/// enables configuration of build parameters and scripts for automated image provisioning.
/// </summary>
/// <remarks>Use this builder to customize the build process for Alpine-based Vagrant boxes, including
/// provisioning scripts, VM settings, and output options. The builder supports chaining configuration methods and
/// requires a valid command object to specify build details. Thread safety is not guaranteed; use separate instances
/// for concurrent builds.</remarks>
public class AlpinePackerVagrantBundleBuilder : AbstractBuilder<PackerBundle>
{
    /// <summary>
    /// Initializes a new instance of the AlpinePackerVagrantBundleBuilder class and performs required pre-build setup.
    /// </summary>
    /// <remarks>The constructor automatically invokes pre-build logic to ensure the builder is ready for use
    /// immediately after instantiation. This setup is performed only once per instance.</remarks>
    public AlpinePackerVagrantBundleBuilder()
    {
        PreBuild();
        _hasBeenPreBuilt = true;
    }

    /// <summary>
    /// Holds a flag indicating whether the pre-build step has been executed to set up necessary configurations.
    /// </summary>
    private readonly bool _hasBeenPreBuilt = false;

    /// <summary>
    /// Holds the internal Packer bundle builder instance used to construct the final Packer bundle.
    /// </summary>
    private readonly PackerBundleBuilder _builder = new();

    /// <summary>
    /// Holds the command object containing configuration parameters for the build process.
    /// </summary>
    private AlpinePackerVagrantBundleCommand? _command;

    /// <summary>
    /// Sets the command to be executed by the bundle builder.
    /// </summary>
    /// <remarks>This method enables fluent configuration of the bundle builder by allowing multiple setup
    /// calls in a single statement.</remarks>
    /// <param name="command">The command to configure for execution. Cannot be null.</param>
    /// <returns>The current instance of <see cref="AlpinePackerVagrantBundleBuilder"/> to allow method chaining.</returns>
    public AlpinePackerVagrantBundleBuilder Command(AlpinePackerVagrantBundleCommand command)
    {
        _command = command;
        return this;
    }

    /// <summary>
    /// Configures the builder with all necessary settings, scripts, and files required to prepare an Alpine Vagrant
    /// bundle prior to the build process.
    /// </summary>
    /// <remarks>This method sets up boot commands, provisioning scripts, variables, and supporting files for
    /// the Alpine Vagrant image. It must be called before initiating the build to ensure the resulting bundle is
    /// correctly configured. The method will throw an <see cref="ArgumentNullException"/> if the command configuration
    /// is not initialized.</remarks>
    /// <returns>The current instance of <see cref="AlpinePackerVagrantBundleBuilder"/> with all pre-build configuration applied.</returns>
    protected AlpinePackerVagrantBundleBuilder PreBuild()
    {
        ArgumentNullException.ThrowIfNull(_command);

        const string eol = "\r\n";

        var @override = new ProvisionerOverrideBuilder()
            .VirtualBoxIso(new VirtualBoxIsoProvisionerOverrideBuilder()
                .ExecuteCommand("{{.Vars}} /bin/sh {{.Path}}")
                .BuildSuccess())
            .BuildSuccess();

        _builder
            .PackerFile(packerFileBuilder =>
            {
                packerFileBuilder.Builder(builderBuilder =>
                {
                    builderBuilder
                            .AddBootCommand("root<enter><wait>")
                            .AddBootCommand("ifconfig eth0 up && udhcpc -i eth0<enter><wait5>")
                            .AddBootCommand(
                                "wget -O $PWD/answers http://{{ .HTTPIP }}:{{ .HTTPPort }}/answers<enter><wait>")
                            .AddBootCommand(
                                "export USEROPTS='-a -u -g audio,video,netdev {{user `ssh_username`}}'<enter>")
                            .AddBootCommand(
                                "export USERSSHKEY='http://{{ .HTTPIP }}:{{ .HTTPPort }}/ssh.keys'<enter>")
                            .AddBootCommand("setup-alpine -f $PWD/answers<enter><wait5>")
                            .AddBootCommand("{{user `root_password`}}<enter><wait>")
                            .AddBootCommand("{{user `root_password`}}<enter><wait10>")
                            .AddBootCommand("mount /dev/sda3 /mnt<enter>")
                            .AddBootCommand("echo 'PermitRootLogin yes' >> /mnt/etc/ssh/sshd_config<enter>")
                            .AddBootCommand("umount /mnt; reboot<enter>")
                            .BootWait("10s")
                            .Communicator("ssh")
                            .DiskSize("{{ user `disk_size` }}")
                            .Format("ova")
                            .GuestOsType("Linux_64")
                            .Headless(false)
                            .HttpDirectory("http")
                            .IsoChecksum("{{user `iso_checksum_type`}}:{{user `iso_checksum`}}")
                            .AddIsoUrl("{{user `iso_local_url`}}")
                            .AddIsoUrl("{{user `iso_download_url`}}")
                            .GuestAdditionUrl(
                                "https://download.virtualbox.org/virtualbox/{{user `vbox_version`}}/VBoxGuestAdditions_{{user `vbox_version`}}.iso")
                            .GuestAdditionSha256("{{user `vbox_guest_additions_iso_sha256`}}")
                            .GuestAdditionPath("VBoxGuestAdditions.iso")
                            .GuestAdditionMode("upload")
                            .VirtualBoxVersionFile("VBoxVersion.txt")
                            .KeepRegistered("false")
                            .ShutdownCommand("/sbin/poweroff")
                            .SshPassword("{{user `root_password` }}")
                            .SshTimeout("10m")
                            .SshUsername("root")
                            .Type("virtualbox-iso")
                            .KeepRegistered("true")
                            .ModifyVm("--memory", "{{user `memory`}}")
                            .ModifyVm("--cpus", "{{user `cpus`}}")
                            .ModifyVm("--nat-localhostreachable1", "on")
                            .ModifyVm("--vram", "{{user `vmemory`}}")
                            .ModifyVm("--natdnshostresolver1", "on")
                            .ModifyVm("--ioapic", "on")
                            .ModifyVm("--hwvirtex", "on")
                            .ModifyVm("--hpet", "off")
                            .ModifyVm("--largepages", "on")
                            .ModifyVm("--vtxvpid", "on")
                            .ModifyVm("--vtxux", "on")
                            .ModifyVm("--pae", "on")
                            .ModifyVm("--acpi", "on")
                            .ModifyVm("--pagefusion", "on")
                            .ModifyVm("--chipset", "ich9")
                            .ModifyVm("--vrde", "off")
                            .ModifyVm("--usb", "off")
                            .ModifyVm("--nested-hw-virt", "on")
                            .ModifyVm("--nestedpaging", "on")
                            .ModifyVm("--ostype", "Linux_x64")
                            .ModifyVm("--graphicscontroller", "vmsvga")
                            .ModifyProperty("hwvirtexclusive", "on")
                            .ModifyVmIf(() => !OperatingSystem.IsWindows(), "--biosapic", "x2apic")
                            .ModifyVmIf(OperatingSystem.IsLinux, "--biosapic", "x2apic")
                            .ModifyVmIf(OperatingSystem.IsLinux, "--paravirtprovider", "kvm")
                            .ModifyVmIf(OperatingSystem.IsWindows, "--paravirtprovider", "kvm")
                            .ModifyVmIf(OperatingSystem.IsMacOS, "--paravirtprovider", "minimum")
                            .ModifyStorageController("SATA Controller", "--hostiocache", "off")
                            .ModifyStorageAttach("SATA Controller", 0, "--nonrotational", "on")
                            .ModifyStorageAttach("SATA Controller", 0, "--discard", "on")
                            .SetExtraData("VBoxInternal/Devices/ahci/0/Config/Port0/NonRotational", "1")
                            .VmName("{{user `vm_name`}}")
                            .HardDriveDiscard()
                            .HardDriveInterface("sata");
                })
                    .Description("description")
                    .Provisioner(provisionerBuilder =>
                    {
                        provisionerBuilder
                            .Type("shell")
                            .AddScript("scripts/00base.sh")
                            .AddScript("scripts/01alpine.sh")
                            .AddScript("scripts/01networking.sh")
                            .AddScript("scripts/02sshd.sh")
                            .AddScript("scripts/03vagrant.sh")
                            .AddScript("scripts/04sudoers.sh")
                            .AddScript("scripts/05cron.sh")
                            .AddScript("scripts/99reboot.sh")
                            .Override(@override);
                    })
                    .Provisioner(provisionerBuilder =>
                    {
                        provisionerBuilder
                            .Type("shell")
                            .PauseBefore("20s")
                            .AddScript("scripts/99minimize.sh")
                            .AddScript("scripts/99disable-ssh-root.sh")
                            .Override(@override);
                    })
                    .PostProcessor(postProcessorBuilder =>
                    {
                        postProcessorBuilder
                            .Type("vagrant")
                            .CompressionLevel(9)
                            .KeepInputArtefact()
                            .Output("{{user `output_vagrant` }}/{{user `vm_name`}}{{user `flavor`}}.box")
                            .VagrantfileTemplate("./vagrant/Vagrantfile");
                    })
                    .Variable("output_vagrant", _command.OutputVagrant)
                    .Variable("box-version", _command.BoxVersion)
                    .Variable("vbox_version", _command.VirtualBoxVersion)
                    .Variable("vbox_guest_additions_iso_sha256",
                        _command.VirtualBoxGuestAdditionsIsoSha256)
                    .Variable("community_repo", _command.CommunityRepository)
                    .Variable("cpus", _command.Cpus)
                    .Variable("disk_size",
                        Math.Round(ByteSize.Parse(_command.DiskSize).MebiBytes,
                            MidpointRounding.ToPositiveInfinity).ToString(CultureInfo.InvariantCulture))
                    .Variable("iso_checksum", _command.IsoChecksum)
                    .Variable("iso_checksum_type", _command.IsoChecksumType)
                    .Variable("iso_download_url", _command.IsoDownloadUrl)
                    .Variable("iso_local_url", _command.IsoLocalUrl)
                    .Variable("memory",
                        Math.Round(ByteSize.Parse(_command.Memory).MebiBytes,
                            MidpointRounding.ToPositiveInfinity).ToString(CultureInfo.InvariantCulture))
                    .Variable("root_password", "vagrant")
                    .Variable("ssh_password", "vagrant")
                    .Variable("ssh_username", "vagrant")
                    .Variable("vmemory",
                        Math.Round(ByteSize.Parse(_command.VideoMemory).MebiBytes,
                            MidpointRounding.ToPositiveInfinity).ToString(CultureInfo.InvariantCulture))
                    .Variable("vm_name", _command.VmName)
                    ;
            })
            .HttpDirectory(httpDirectoryBuilder =>
            {
                httpDirectoryBuilder
                    .AddFile("answers", new FileBuilder()
                        .AddLine("""
                                         KEYMAPOPTS="us us"
                                         """)
                        .AddLine("HOSTNAMEOPTS=alpine")
                        .AddLine("DEVDOPTS=mdev")
                        .AddLine("INTERFACESOPTS=\"auto lo")
                        .AddLine("iface lo inet loopback")
                        .AddLine("auto eth0")
                        .AddLine("iface eth0 inet dhcp")
                        .AddLine("  hostname alpine")
                        .AddLine("  domain local")
                        .AddLine("\"")
                        .AddLine(@"TIMEZONEOPTS=""UTC""")
                        .AddLine("PROXYOPTS=none")
                        .AddLine(@"APKREPOSOPTS=""-1""")
                        .AddLine(@"SSHDOPTS=""-c openssh""")
                        .AddLine(@"NTPOPTS=""-c openntpd""")
                        .AddLine(@"DISKOPTS=""-m sys /dev/sda""")
                        .AddLine("LBUOPTS=none")
                        .AddLine("APKCACHEOPTS=none")
                        .AddLine("export ERASE_DISKS=/dev/sda")
                        .BuildSuccess())
                    .AddFile("ssh.keys", new FileBuilder()
                        .AddLine(
                            "ssh-rsa AAAAB3NzaC1yc2EAAAABIwAAAQEA6NF8iallvQVp22WDkTkyrtvp9eWW6A8YVr+kz4TjGYe7gHzIw+niNltGEFHzD8+v1I2YJ6oXevct1YeS0o9HZyN1Q9qgCgzUFtdOKLv6IedplqoPkcmF0aYet2PkEDo3MlTBckFXPITAMzF8dJSIFo9D8HfdOV0IAdx4O7PtixWKn5y2hMNG0zQPyUecp4pzC6kivAIhyfHilFR61RGL+GPXQ2MWZWFYbAGjyiYJnAmCP3NOTd0jMZEnDkbUvxhMmBYSdETk1rRgm+R4LOzFUGaHqHDLKLX+FIPKcF96hrucXzcWyLbIbEgE98OHlnVYCzRdK8jlqm8tehUc9c9WhQ== vagrant insecure public key").BuildSuccess())
                    ;
            })
            .VagrantDirectory(vagrantDirectoryBuilder =>
            {
                vagrantDirectoryBuilder
                    .AddFile("info.json", new FileBuilder().AddLines("""
                                                                      {
                                                                          "Author": "Stéphane Erard",
                                                                          "Website": "",
                                                                          "Artifacts": "",
                                                                          "Repository": "",
                                                                          "Description": "Alpine image for development"
                                                                      }
                                                                      """, eol).BuildSuccess())
                    .AddFile("Vagrantfile", new FileBuilder().AddLines("""
                                                                        Vagrant.configure('2') do |config|
                                                                          config.vm.provider 'virtualbox' do |v|
                                                                            v.cpus = 1
                                                                            v.memory = "128"
                                                                          end
                                                                        end
                                                                        """, eol).BuildSuccess());
            })
            .Directory("output-vagrant")
            .Script("scripts/00base.sh", scriptBuilder =>
            {
                scriptBuilder
                    .Name("00base")
                    .Set("-ux")
                    .AddLines("""
                                      echo "Environment is:-"
                                      env
                                      source /etc/os-release

                                      cat <<EOT> /etc/motd
                                      """, eol)
                    .AddLines(FiggleFonts.Ogre.Render("Alpine Linux").Split(eol))
                    .AddLines("""
                                      Alpine ${VERSION_ID}
                                      EOT

                                      exit 0
                                      """, eol);
            })
            .Script("scripts/01alpine.sh", (b) => b
                .Name("01alpine")
                .Set("-ux")
                .AddLines($"""
                                   echo "Setting up remote repositories..."
                                   cat >/etc/apk/repositories <<EOT
                                   http://nl.alpinelinux.org/alpine/{_command.AlpineVersion.ToMajorMinorUrl()}/main/
                                   http://nl.alpinelinux.org/alpine/{_command.AlpineVersion.ToMajorMinorUrl()}/community/
                                   EOT

                                   echo "Performing an update/upgrade"
                                   apk update
                                   apk add --upgrade apk-tools
                                   apk upgrade --available
                                   apk add bash bash-completion sudo virtualbox-guest-additions

                                   rc-update add virtualbox-guest-additions boot

                                   echo "Setting systctl kernel settings to relax security"
                                   cat >/etc/sysctl.d/00-alpine.conf <<EOT
                                   net.ipv4.ip_forward = 1
                                   net.ipv4.tcp_syncookies = 1
                                   net.ipv4.conf.default.rp_filter = 0
                                   net.ipv4.conf.all.rp_filter = 0
                                   net.ipv4.ping_group_range=0 2147483647
                                   kernel.panic = 120
                                   EOT
                                   exit 0
                                   """, eol))
            .Script("scripts/01networking.sh", (b) => b.Name("networking")
                .Set("-ux")
                .AddLine("exit 0"))
            .Script("scripts/02sshd.sh", (b) => b
                .Name("sshd")
                .Set("-eux")
                .AddLines("""
                                  echo "UseDNS no" >> /etc/ssh/sshd_config
                                  exit 0                              
                                  """, eol))
            .Script("scripts/03vagrant.sh", (b) => b
                .Name("vagrant")
                .Set("-eux")
                .AddLines("""
                                  date > /etc/vagrant_box_build_time
                                  echo "vagrant:vagrant" | chpasswd
                                  mkdir -pm 700 /home/vagrant/.ssh
                                  wget -O /home/vagrant/.ssh/authorized_keys https://raw.githubusercontent.com/mitchellh/vagrant/master/keys/vagrant.pub
                                  chown -R vagrant:vagrant /home/vagrant/.ssh
                                  chmod -R go-rwsx /home/vagrant/.ssh
                                  echo "Use the bash shell for vagrant and root"
                                  sed -e 's@/bin/ash@/bin/bash@' -i /etc/passwd
                                  exit 0
                                  """, eol))
            .Script("scripts/04sudoers.sh", (b) => b
                .Name("sudoers")
                .Set("-eux")
                .AddLines("""
                                  adduser vagrant wheel
                                  echo "Defaults exempt_group=wheel" > /etc/sudoers
                                  echo "%wheel ALL=NOPASSWD:ALL" >> /etc/sudoers
                                  exit 0
                                  """, eol))
            .Script("scripts/05cron.sh", (b) => b
                .Name("cron")
                .Set("-eux")
                .AddLines("""
                                  echo "Adding a more regular 1min cron category"
                                  echo "*       *       *       *       *       run-parts /etc/periodic/1min" >>/etc/crontabs/root
                                  mkdir -p /etc/periodic/1min
                                  exit 0
                                  """, eol))
            .Script("scripts/99reboot.sh", (b) => b
                .Name("reboot")
                .Set("-ux")
                .AddLines("""
                                   echo Rebooting
                                   reboot
                                   exit 0
                                  """, eol))
            .Script("scripts/99minimize.sh", (b) => b
                .Name("minimalize")
                .Set("-ux")
                .AddLines("""
                                  echo "Clean up things (tmp, logs, apk cache)"
                                  rm -rf /tmp/* /var/log/* /var/cache/apk/*

                                  echo Whiteout root
                                  count=$(df -kP / | tail -n1  | awk -F ' ' '{print $4}')
                                  count=$(($count-1))
                                  dd if=/dev/zero of=/whitespace bs=1M count=$count || echo "dd exit code $? is suppressed";
                                  rm /whitespace

                                  echo Whiteout /boot
                                  count=$(df -kP /boot | tail -n1 | awk -F ' ' '{print $4}')
                                  count=$(($count-1))
                                  dd if=/dev/zero of=/boot/whitespace bs=1M count=$count || echo "dd exit code $? is suppressed";
                                  rm /boot/whitespace

                                  set +e
                                  swapuuid="`/sbin/blkid -o value -l -s UUID -t TYPE=swap`";
                                  case "$?" in
                                      2|0) ;;
                                      *) exit 1 ;;
                                  esac
                                  set -e

                                  if [ "x${swapuuid}" != "x" ]; then
                                      # Whiteout the swap partition to reduce box size
                                      # Swap is disabled till reboot
                                      swappart="`readlink -f /dev/disk/by-uuid/$swapuuid`";
                                      /sbin/swapoff "$swappart" || true;
                                      dd if=/dev/zero of="$swappart" bs=1M || echo "dd exit code $? is suppressed";
                                      /sbin/mkswap -L "$swapuuid" "$swappart";
                                  fi

                                  sync;
                                  sync;
                                  sync;

                                  exit 0
                                  """, eol))
            .Script("scripts/99disable-ssh-root.sh", (b) => b
                .Name("disable-ssh-root")
                .Set("-ux")
                .AddLine(@"sed '/PermitRootLogin yes/d' -i /etc/ssh/sshd_config"));

        return this;
    }

    /// <summary>
    /// Mutates the internal Packer bundle builder using the specified mutation action.
    /// </summary>
    /// <param name="mutation"></param>
    /// <returns></returns>
    public AlpinePackerVagrantBundleBuilder Mutate(Action<PackerBundleBuilder> mutation)
    {
        mutation(_builder);
        return this;
    }

    /// <summary>
    /// Builds the result using the configured builder after a successful pre-build operation.
    /// </summary>
    /// <returns>An object that represents the result of the build process.</returns>
    /// <exception cref="InvalidOperationException">Thrown if PreBuild() has not been called prior to invoking this method.</exception>
    protected IBuildResult BuildInternal()
    {
        if (!_hasBeenPreBuilt)
        {
            throw new InvalidOperationException("You must call PreBuild() before Build().");
        }
        var result = _builder.Build();

        return result;
    }

    /// <summary>
    /// Creates and returns a new instance of a successful <see cref="PackerBundle"/>.
    /// </summary>
    /// <returns>A <see cref="PackerBundle"/> representing a successful build operation.</returns>
    protected override PackerBundle Instantiate()
    {
        return _builder.BuildSuccess();
    }

    /// <summary>
    /// Performs validation using the specified collection of visited objects and records any validation failures.
    /// </summary>
    /// <param name="visitedCollector">A dictionary that tracks objects already visited during validation to prevent redundant checks and handle
    /// circular references.</param>
    /// <param name="failures">A dictionary used to collect and record validation failures encountered during the validation process.</param>
    protected override void ValidateInternal(VisitedObjectDictionary visitedCollector, FailuresDictionary failures)
    {
        _builder.Validate(visitedCollector, failures);
    }
}