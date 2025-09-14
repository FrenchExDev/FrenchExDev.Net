#region Licensing

// Copyright Stéphane Erard
// For licensing, please contact stephane.erard@gmail

#endregion

namespace FrenchExDev.Net.Packer.Bundle.Testing;

public class PackerBundlerTests<TCommandTester>
    where TCommandTester : class, new()
{
    public TCommandTester NewTester()
    {
        return new TCommandTester();
    }
}