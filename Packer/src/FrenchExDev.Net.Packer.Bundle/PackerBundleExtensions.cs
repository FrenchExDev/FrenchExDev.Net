#region Licensing

// Copyright Stéphane Erard
// For licensing, please contact stephane.erard@gmail

#endregion

using FrenchExDev.Net.CSharp.Object.Builder.Abstractions;

namespace FrenchExDev.Net.Packer.Bundle;

public static class PackerBundleExtensions
{
    public static PackerBundle Provisioning(this PackerBundle packerBundle, Action<ProvisionerBuilder, Dictionary<string, IScript>> provision)
    {
        var pB = new ProvisionerBuilder();
        provision(pB, packerBundle.Scripts);

        packerBundle.PackerFile.Provisioners ??= new List<Provisioner>();
        var built = pB.Build();

        packerBundle.PackerFile.Provisioners.Add(built.Success());

        return packerBundle;
    }
}