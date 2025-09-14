#region Licensing

// Copyright Stéphane Erard
// For licensing, please contact stephane.erard@gmail

#endregion

#region Usings

using System.Text.Json.Serialization;

#endregion

namespace FrenchExDev.Net.Packer.Bundle;

public class PackerBuilder
{
    [JsonPropertyName("boot_command")] public required List<string>? BootCommand { get; set; }
    [JsonPropertyName("boot_wait")] public required string? BootWait { get; set; }
    [JsonPropertyName("communicator")] public required string? Communicator { get; set; }
    [JsonPropertyName("disk_size")] public required string? DiskSize { get; set; }
    [JsonPropertyName("format")] public required string? Format { get; set; }
    [JsonPropertyName("headless")] public required bool? Headless { get; set; }
    [JsonPropertyName("http_directory")] public required string? HttpDirectory { get; set; }
    [JsonPropertyName("iso_checksum")] public required string? IsoChecksum { get; set; }
    [JsonPropertyName("iso_urls")] public required List<string>? IsoUrls { get; set; }
    [JsonPropertyName("keep_registered")] public required string? KeepRegistered { get; set; }
    [JsonPropertyName("shutdown_command")] public required string? ShutdownCommand { get; set; }
    [JsonPropertyName("ssh_password")] public required string? SshPassword { get; set; }
    [JsonPropertyName("ssh_timeout")] public required string? SshTimeout { get; set; }
    [JsonPropertyName("ssh_username")] public required string? SshUsername { get; set; }
    [JsonPropertyName("type")] public required string? Type { get; set; }
    [JsonPropertyName("guest_os_type")] public required string? GuestOsType { get; set; }

    [JsonPropertyName("guest_additions_url")]
    public required string? GuestAdditionUrl { get; set; }

    [JsonPropertyName("guest_additions_sha256")]
    public required string? GuestAdditionSha256 { get; set; }

    [JsonPropertyName("guest_additions_path")]
    public required string? GuestAdditionPath { get; set; }

    [JsonPropertyName("guest_additions_mode")]
    public required string? GuestAdditionMode { get; set; }

    [JsonPropertyName("virtualbox_version_file")]
    public required string? VirtualBoxVersionFile { get; set; }

    [JsonPropertyName("vboxmanage")] public required List<List<string>>? VboxManage { get; set; }

    [JsonPropertyName("hard_drive_interface")]
    public required string? HardDriveInterface { get; set; }

    [JsonPropertyName("hard_drive_discard")]
    public required bool? HardDriveDiscard { get; set; }

    [JsonPropertyName("vm_name")] public required string? VmName { get; set; }
}