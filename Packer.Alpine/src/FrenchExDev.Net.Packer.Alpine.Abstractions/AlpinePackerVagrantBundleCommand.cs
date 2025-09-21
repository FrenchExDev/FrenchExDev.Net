#region Licensing

// Copyright Stéphane Erard
// For licensing, please contact stephane.erard@gmail

#endregion

#region Usings

using FrenchExDev.Net.Alpine.Version;

#endregion

namespace FrenchExDev.Net.Packer.Alpine.Abstractions;

/// <summary>
/// Represents the configuration options required to generate an Alpine Linux Vagrant box using Packer and VirtualBox.
/// </summary>
/// <remarks>This class encapsulates all parameters needed for the Packer build process, including VM hardware
/// specifications, ISO sources and checksums, repository URLs, and versioning information. All properties are required
/// and must be set before use. Typically, an instance of this class is used to supply input values to a build
/// automation workflow that provisions Alpine Linux virtual machines for Vagrant environments.</remarks>
public class AlpinePackerVagrantBundleCommand
{
    /// <summary>
    /// Directory where .box files will be output by Packer's Vagrant post-processor.
    /// </summary>
    public required string OutputVagrant { get; init; } = "output-vagrant";

    /// <summary>
    /// Size of the disk to be used for the VM (e.g., "20GiB").
    /// </summary>
    public required string DiskSize { get; init; } = "20GiB";

    /// <summary>
    /// Amount of memory allocated to the VM (in MB).
    /// </summary>
    public required string Memory { get; init; } = "256";

    /// <summary>
    /// Number of CPUs allocated to the VM.
    /// </summary>
    public required string Cpus { get; init; } = "1";

    /// <summary>
    /// Version of the box to be created.
    /// </summary>
    public required string BoxVersion { get; init; } = "1.0.0";

    /// <summary>
    /// Version of VirtualBox to be used.
    /// </summary>
    public required string VirtualBoxVersion { get; init; }

    /// <summary>
    /// Type of checksum used for the ISO file (e.g., "sha256").
    /// </summary>
    public required string IsoChecksumType { get; init; }

    /// <summary>
    /// Checksum value for the ISO file.
    /// </summary>
    public required string IsoChecksum { get; init; }

    /// <summary>
    /// URL to download the ISO file from the internet.
    /// </summary>
    public required string IsoDownloadUrl { get; init; }

    /// <summary>
    /// Local path to the ISO file.
    /// </summary>
    public required string IsoLocalUrl { get; init; }

    /// <summary>
    /// SHA256 checksum for the VirtualBox Guest Additions ISO file.
    /// </summary>
    public required string VirtualBoxGuestAdditionsIsoSha256 { get; init; }

    /// <summary>
    /// Name of the virtual machine.
    /// </summary>
    public required string VmName { get; init; }

    /// <summary>
    /// Amount of video memory allocated to the VM.
    /// </summary>
    public required string VideoMemory { get; init; }

    /// <summary>
    /// URL of the Alpine community repository.
    /// </summary>
    public required string CommunityRepository { get; init; }

    /// <summary>
    /// Alpine Linux version to be used for the VM.
    /// </summary>
    public required AlpineVersion AlpineVersion { get; init; }
}