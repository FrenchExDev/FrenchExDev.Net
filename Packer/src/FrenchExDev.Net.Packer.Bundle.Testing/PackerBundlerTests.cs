#region Licensing

// Copyright Stéphane Erard
// For licensing, please contact stephane.erard@gmail

#endregion

namespace FrenchExDev.Net.Packer.Bundle.Testing;

/// <summary>
/// Provides a test fixture for creating instances of a command tester type used in packer bundler tests.
/// </summary>
/// <typeparam name="TCommandTester">The type of the command tester to instantiate. Must be a reference type with a public parameterless constructor.</typeparam>
public class PackerBundlerTests<TCommandTester>
    where TCommandTester : class, new()
{
    public TCommandTester Tester()
    {
        return new TCommandTester();
    }
}