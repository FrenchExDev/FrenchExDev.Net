#region Licensing

// Copyright Stéphane Erard
// For licensing, please contact stephane.erard@gmail

#endregion

#region Usings

using FenchExDev.Net.Testing;
using FrenchExDev.Net.Packer.Bundle.Testing;
using Shouldly;

#endregion

namespace FrenchExDev.Net.Packer.Bundle.Tests;

[Feature(nameof(PackerBundle), TestKind.Unit)]
[Trait(Internet.Test, Internet.Offline)]
[Trait(Kind.Test, Kind.Unit)]
public class PackerBundlerTests : PackerBundlerTests<PackerBundlerTester>
{
    [Fact]
    public void Can_Build_And_Serialize_Alpine_PackerFile_Successfully() =>
        Tester().Valid(builder =>
            {
                
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