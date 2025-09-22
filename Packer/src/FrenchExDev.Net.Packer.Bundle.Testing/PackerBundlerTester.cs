#region Licensing

// Copyright Stéphane Erard
// For licensing, please contact stephane.erard@gmail

#endregion

#region Usings

#endregion

using FenchExDev.Net.Testing;

namespace FrenchExDev.Net.Packer.Bundle.Testing;

[Feature(nameof(PackerBundle), TestKind.Unit)]
[Feature(nameof(PackerBundleBuilder), TestKind.Unit)]
public class PackerBundlerTester
{
    protected static PackerBundleBuilder Builder() => new();

    public void TestValid(
        Action<PackerBundleBuilder> builderBody,
        Action<PackerBundle> assertBuiltBody,
        Action<string> assertSerializedBody
    )
    {
        var packerBundleBuilder = Builder();

        builderBody(packerBundleBuilder);

        var packerBundle = packerBundleBuilder.BuildSuccess();

        assertBuiltBody(packerBundle);

        var serializedPackerFile = packerBundle.PackerFile.Serialize();

        assertSerializedBody(serializedPackerFile);
    }
}