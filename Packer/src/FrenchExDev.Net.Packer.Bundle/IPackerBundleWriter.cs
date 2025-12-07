#region Licensing

// Copyright Stéphane Erard
// For licensing, please contact stephane.erard@gmail

#endregion

namespace FrenchExDev.Net.Packer.Bundle;

public interface IPackerBundleWriter
{
    Task WriteAsync(PackerBundle bundle, PackerBundleWritingContext context,
        CancellationToken cancellationToken = default);
}