#region Licensing

// Copyright Stéphane Erard
// For licensing, please contact stephane.erard@gmail

#endregion

#region Usings

#endregion

namespace FrenchExDev.Net.Packer.Bundle.Testing;

public class PackerBundlerTester
{
    public void TestValid(
        Action<PackerBundleBuilder> builderBody,
        Action<PackerBundle> assertBuiltBody,
        Action<string> assertSerializedBody
    )
    {
        var packerBundleBuilder = new PackerBundleBuilder();
        builderBody(packerBundleBuilder);

        var packerBundle = packerBundleBuilder.BuildSuccess();

        assertBuiltBody(packerBundle);

        var serializedPackerFile = packerBundle.PackerFile.Serialize();

        assertSerializedBody(serializedPackerFile);
    }
}