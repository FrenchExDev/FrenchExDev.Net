#region Licensing

// Copyright Stéphane Erard
// For licensing, please contact stephane.erard@gmail

#endregion

#region Usings

using System.Text.Json.Serialization;

#endregion

namespace FrenchExDev.Net.Packer.Bundle;

/// <summary>
/// Represents the serialized builder section of a Packer template (e.g. virtualbox-iso).
/// </summary>
/// <remarks>All properties map directly to Packer JSON/HCL fields. Set every required value before serialization.
/// Example usage: populate via a fluent builder then serialize to JSON.</remarks>
public class PackerBuilder
{
    /// <summary>Ordered boot keystrokes sent to the VM (e.g. automated installer answers).</summary>
    /// <example>"root&lt;enter&gt;", "ifconfig eth0 up&lt;enter&gt;"</example>
    [JsonPropertyName("boot_command")] public required List<string>? BootCommand { get; set; }

    /// <summary>Delay before first boot command (e.g. 10s, 1m).</summary>
    [JsonPropertyName("boot_wait")] public required string? BootWait { get; set; }

    /// <summary>Remote communicator type (ssh, winrm).</summary>
    [JsonPropertyName("communicator")] public required string? Communicator { get; set; }

    /// <summary>Primary disk size (builder specific units; often MB).</summary>
    [JsonPropertyName("disk_size")] public required string? DiskSize { get; set; }

    /// <summary>Artifact output format (e.g. ova, vdi).</summary>
    [JsonPropertyName("format")] public required string? Format { get; set; }

    /// <summary>Run the VM headless (true = no GUI).</summary>
    [JsonPropertyName("headless")] public required bool? Headless { get; set; }

    /// <summary>Directory served over HTTP to the guest (for kickstart/answers files).</summary>
    [JsonPropertyName("http_directory")] public required string? HttpDirectory { get; set; }

    /// <summary>ISO checksum with type prefix if required (e.g. sha256:xxxx).</summary>
    [JsonPropertyName("iso_checksum")] public required string? IsoChecksum { get; set; }

    /// <summary>Candidate ISO URLs (Packer tries in order).</summary>
    [JsonPropertyName("iso_urls")] public required List<string>? IsoUrls { get; set; }

    /// <summary>Keep the VM registered in the provider after build.</summary>
    [JsonPropertyName("keep_registered")] public required bool? KeepRegistered { get; set; }

    /// <summary>Guest shutdown command executed after provisioning.</summary>
    [JsonPropertyName("shutdown_command")] public required string? ShutdownCommand { get; set; }

    /// <summary>SSH password for communicator (if applicable).</summary>
    [JsonPropertyName("ssh_password")] public required string? SshPassword { get; set; }

    /// <summary>SSH connect timeout (e.g. 10m).</summary>
    [JsonPropertyName("ssh_timeout")] public required string? SshTimeout { get; set; }

    /// <summary>SSH username for communicator.</summary>
    [JsonPropertyName("ssh_username")] public required string? SshUsername { get; set; }

    /// <summary>Packer builder type (e.g. virtualbox-iso).</summary>
    [JsonPropertyName("type")] public required string? Type { get; set; }

    /// <summary>Guest OS type hint for provider (e.g. Linux_64).</summary>
    [JsonPropertyName("guest_os_type")] public required string? GuestOsType { get; set; }

    /// <summary>Guest Additions ISO download URL.</summary>
    [JsonPropertyName("guest_additions_url")] public required string? GuestAdditionUrl { get; set; }

    /// <summary>Guest Additions ISO sha256 checksum.</summary>
    [JsonPropertyName("guest_additions_sha256")] public required string? GuestAdditionSha256 { get; set; }

    /// <summary>Guest Additions ISO path inside build context/VM.</summary>
    [JsonPropertyName("guest_additions_path")] public required string? GuestAdditionPath { get; set; }

    /// <summary>Guest Additions mode (e.g. upload, attach).</summary>
    [JsonPropertyName("guest_additions_mode")] public required string? GuestAdditionMode { get; set; }

    /// <summary>File name to write VirtualBox version into (VBoxVersion.txt typical).</summary>
    [JsonPropertyName("virtualbox_version_file")] public required string? VirtualBoxVersionFile { get; set; }

    /// <summary>VBoxManage command matrix (each inner list is one command+args).</summary>
    /// <example>[ ["modifyvm","{{ .Name }}","--memory","1024" ] ]</example>
    [JsonPropertyName("vboxmanage")] public required List<List<string>>? VboxManage { get; set; }

    /// <summary>Hard drive controller interface (e.g. sata, ide).</summary>
    [JsonPropertyName("hard_drive_interface")] public required string? HardDriveInterface { get; set; }

    /// <summary>Enable discard/TRIM on primary disk.</summary>
    [JsonPropertyName("hard_drive_discard")] public required bool? HardDriveDiscard { get; set; }

    /// <summary>Resulting VM name / base name for artifact.</summary>
    [JsonPropertyName("vm_name")] public required string? VmName { get; set; }
}