using Figgle;
using FrenchExDev.Net.Alpine.Version;
using FrenchExDev.Net.Packer.Abstractions;
using FrenchExDev.Net.Packer.Bundle;

namespace FrenchExDev.Net.Packer.Alpine.Abstractions;

/// <summary>
/// Provides default boot command sequences for Alpine Linux initialization and setup.
/// </summary>
/// <remarks>The default commands automate network configuration, user setup, SSH key retrieval, and system
/// initialization steps for Alpine Linux environments. These commands are intended for use in automated provisioning
/// scenarios and may include placeholders for runtime values such as IP addresses and user credentials.</remarks>
public static class AlpineSettings
{
    /// <summary>
    /// Gets the default list of boot commands used to initialize and configure an Alpine Linux system in automated
    /// build scenarios.
    /// </summary>
    /// <remarks>The commands in this list perform essential setup steps, including network configuration,
    /// downloading configuration files, setting user options, and preparing SSH access. The sequence is designed for
    /// use with automated provisioning tools and may assume specific environment variables and template placeholders
    /// are available.</remarks>
    public static PackerBuilderBootCommandList DefaultBootCommands { get; } = new()
    {
        "root<enter><wait>",
        "ifconfig eth0 up && udhcpc -i eth0<enter><wait5>",
        "wget -O $PWD/answers http://{{ .HTTPIP }}:{{ .HTTPPort }}/answers<enter><wait>",
        "export USEROPTS='-a -u -g audio,video,netdev {{user `ssh_username`}}'<enter>",
        "export USERSSHKEY='http://{{ .HTTPIP }}:{{ .HTTPPort }}/ssh.keys'<enter>",
        "setup-alpine -f $PWD/answers<enter><wait5>",
        "{{user `root_password`}}<enter><wait>",
        "{{user `root_password`}}<enter><wait10>",
        "mount /dev/sda3 /mnt<enter>",
        "echo 'PermitRootLogin yes' >> /mnt/etc/ssh/sshd_config<enter>",
        "umount /mnt; reboot<enter>"
    };

    /// <summary>
    /// Adds a predefined set of configuration lines to the specified answer file builder for Alpine Linux automated
    /// setup.
    /// </summary>
    /// <remarks>The added lines configure system options such as keyboard layout, hostname, network
    /// interfaces, timezone, SSH, NTP, disk setup, and other installation parameters required for unattended Alpine
    /// Linux installation.</remarks>
    /// <param name="fileBuilder">The file builder to which the configuration lines will be added. Must not be null.</param>
    /// <returns>The same <see cref="FileBuilder"/> instance with the Alpine Linux answer file lines appended.</returns>
    public static FileBuilder AddAnswerFile(FileBuilder fileBuilder) => fileBuilder.AddLine("""
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
                   .AddLine("export ERASE_DISKS=/dev/sda");

    /// <summary>
    /// Gets the default answer file builder used for configuring automated responses.
    /// </summary>
    public static Func<FileBuilder> AnswerFile { get; } = () => AddAnswerFile(new FileBuilder());

    private static readonly string eol = "\r\n";

    public static Func<AlpineVersion, PackerBundleBuilder, PackerBundleBuilder> DefaultBuilder => (alpineVersion, builder) => builder
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
                                   http://nl.alpinelinux.org/alpine/{alpineVersion.ToMajorMinorUrl()}/main/
                                   http://nl.alpinelinux.org/alpine/{alpineVersion.ToMajorMinorUrl()}/community/
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


    public static Func<AlpineVersion, PackerBundleBuilder> Default => (alpineVersion) => DefaultBuilder(alpineVersion, new PackerBundleBuilder());
}
