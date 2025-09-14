#region Licensing

// Copyright Stéphane Erard
// For licensing, please contact stephane.erard@gmail

#endregion

#region Usings

#endregion

using FrenchExDev.Net.Alpine.Version;

namespace FrenchexDev.Packer.Net.PackerBundler.Tests;

public class AlpinePackerBundleCommand
{
    /// <summary>
    ///     Where .box files will be output by packer' vagrant post-processor
    /// </summary>
    public required string OutputVagrant { get; init; } = "output-vagrant";

    /// <summary>
    /// </summary>
    public required string DiskSize { get; init; } = "20GiB";

    /// <summary>
    /// </summary>
    public required string Memory { get; init; } = "256";

    /// <summary>
    /// </summary>
    public required string Cpus { get; init; } = "1";

    public required string BoxVersion { get; init; } = "1.0.0";
    public required string VirtualBoxVersion { get; init; }

    public required string IsoChecksumType { get; init; }
    public required string IsoChecksum { get; init; }
    public required string IsoDownloadUrl { get; init; }
    public required string IsoLocalUrl { get; init; }
    public required string VirtualBoxGuestAdditionsIsoSha256 { get; init; }
    public required string VmName { get; init; }
    public required string VideoMemory { get; init; }
    public required string CommunityRepository { get; init; }
    public required AlpineVersion AlpineVersion { get; init; }
}