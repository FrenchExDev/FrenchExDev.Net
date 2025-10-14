using FenchExDev.Net.Testing;
using FrenchExDev.Net.Alpine.Version;
using FrenchExDev.Net.Packer.Alpine.Abstractions;
using FrenchExDev.Net.Packer.Bundle;
using FrenchExDev.Net.Packer.Bundle.Testing;
using Shouldly;

namespace FrenchExDev.Net.Packer.Alpine.Tests;

/// <summary>
/// Contains unit tests for the <see cref="PackerBundle"/> component, verifying its ability to build and serialize Alpine Packer
/// files and related artifacts.
/// </summary>
/// <remarks>These tests ensure that the PackerBundler produces valid configuration files, scripts, and
/// directories required for Alpine-based Vagrant box creation. The class is intended to validate integration between
/// builder commands, provisioners, post-processors, and file generation logic. All tests are designed to run offline
/// and do not require internet connectivity.</remarks>
[Feature(nameof(PackerBundle), TestKind.Unit)]
[Trait(Internet.Test, Internet.Offline)]
[Trait(Kind.Test, Kind.Unit)]
public class PackerBundlerTests : TestsUsing<PackerBundlerTester>
{
    [Fact]
    public void Can_Build_And_Serialize_Alpine_PackerFile_Successfully() =>
        NewTester().Valid(builder =>
        {
            var alpinePackerBundleCommand = new AlpinePackerVagrantBundleCommand
            {
                OutputVagrant = "output-vagrant",
                DiskSize = "20 GiB",
                Memory = "256 MiB",
                Cpus = "2",
                BoxVersion = "1.0.0",
                VirtualBoxVersion = "7.0.1",
                IsoChecksumType = "iso checksump type",
                IsoChecksum = "iso checksum",
                IsoDownloadUrl = "iso download url",
                IsoLocalUrl = "iso local url",
                VirtualBoxGuestAdditionsIsoSha256 = "vboxisoguestadditionsisochecksum",
                VmName = "vmname",
                VideoMemory = "64 MiB",
                CommunityRepository = "communityrepository",
                AlpineVersion = AlpineVersion.From("3.20")
            };

            var @override = new ProvisionerOverrideBuilder()
                .VirtualBoxIso(new VirtualBoxIsoProvisionerOverrideBuilder()
                    .ExecuteCommand("{{.Vars}} /bin/sh {{.Path}}")
                    .BuildSuccess())
                .BuildSuccess();

            new PackerAlpineVirtualBoxVagrantBundleBuilder().Command(alpinePackerBundleCommand).Compose(builder);
        },
        assertBuiltBody: bundle =>
        {
            bundle.ShouldNotBeNull();
            bundle.ShouldBeAssignableTo<PackerBundle>();
        }, serialized =>
        {
            serialized.ShouldNotBeEmpty();
        });
}