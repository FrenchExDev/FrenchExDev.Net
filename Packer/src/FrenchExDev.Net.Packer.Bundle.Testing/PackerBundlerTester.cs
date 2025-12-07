#region Licensing

// Copyright Stéphane Erard
// For licensing, please contact stephane.erard@gmail

#endregion

#region Usings

using FrenchExDev.Net.CSharp.Object.Builder;

#endregion

namespace FrenchExDev.Net.Packer.Bundle.Testing;

/// <summary>
/// Provides helper methods for testing the behavior of the PackerBundle and PackerBundleBuilder components.
/// </summary>
/// <remarks>Use this class to verify both valid and invalid scenarios when building and serializing packer
/// bundles. The methods allow you to supply custom actions for configuring the builder, asserting the built bundle, and
/// validating serialization or failure results. Intended for use in unit tests targeting PackerBundle-related
/// functionality.</remarks>
public class PackerBundlerTester
{
    /// <summary>
    /// Creates a new instance of the <see cref="PackerBundleBuilder"/> class for constructing packer bundles.
    /// </summary>
    /// <returns>A new <see cref="PackerBundleBuilder"/> instance that can be used to configure and build packer bundles.</returns>
    protected static PackerBundleBuilder Builder() => new();

    /// <summary>
    /// Executes a sequence of actions to build, validate, and serialize a PackerBundle using the provided delegates.
    /// </summary>
    /// <remarks>Use this method to define a complete test or validation workflow for a PackerBundle,
    /// including setup, post-build checks, and serialization checks. Each delegate is invoked in order, allowing for
    /// flexible customization of the build and validation process.</remarks>
    /// <param name="builderBody">An action that configures the <see cref="PackerBundleBuilder"/> before building the bundle. This delegate is
    /// invoked to set up the builder's state.</param>
    /// <param name="assertBuiltBody">An action that performs assertions or validations on the built <see cref="PackerBundle"/>. This delegate is
    /// called after the bundle is constructed.</param>
    /// <param name="assertSerializedBody">An action that performs assertions or validations on the serialized representation of the packer file. This
    /// delegate receives the serialized string after the bundle is built.</param>
    public void Valid(
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

    /// <summary>
    /// Executes the specified builder action to configure a PackerBundleBuilder, then invokes the provided assertion
    /// action on the resulting failures.
    /// </summary>
    /// <remarks>Use this method to set up a bundle and verify its failures in a single operation, typically
    /// for testing scenarios. Both actions must be provided; otherwise, an exception may occur.</remarks>
    /// <param name="builderBody">An action that receives a PackerBundleBuilder to configure the bundle before building. Cannot be null.</param>
    /// <param name="assertFailuresBody">An action that receives a FailuresDictionary containing the build failures to assert against. Cannot be null.</param>
    public void Invalid(
        Action<PackerBundleBuilder> builderBody,
        Action<FailuresDictionary> assertFailuresBody
    )
    {
        var packerBundleBuilder = Builder();
        builderBody(packerBundleBuilder);
        var failures = packerBundleBuilder.Build().Failures<PackerBundle>();
        assertFailuresBody(failures);
    }
}