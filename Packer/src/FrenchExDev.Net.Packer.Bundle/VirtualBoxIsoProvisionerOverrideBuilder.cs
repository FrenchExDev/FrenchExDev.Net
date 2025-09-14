#region Licensing

// Copyright Stéphane Erard
// For licensing, please contact stephane.erard@gmail

#endregion

#region Usings

using FrenchExDev.Net.CSharp.Object.Builder;
using FrenchExDev.Net.CSharp.Object.Builder.Abstractions;

#endregion


namespace FrenchExDev.Net.Packer.Bundle;

/// <summary>
/// Provides a fluent builder for configuring and constructing instances of <see cref="VirtualBoxIsoProvisionerOverride"/>.
/// </summary>
/// <remarks>
/// Use this builder to specify the execute command for a VirtualBox ISO provisioner override in a Packer build.
/// Each configuration method returns the builder instance, allowing for method chaining. The builder validates that all mandatory properties are set before constructing the final <see cref="VirtualBoxIsoProvisionerOverride"/> object.
/// This class is not thread-safe.
/// <example>
/// <code>
/// var builder = new VirtualBoxIsoProvisionerOverrideBuilder()
///     .ExecuteCommand("echo Hello World");
/// var result = builder.Build();
/// </code>
/// </example>
/// </remarks>
public class VirtualBoxIsoProvisionerOverrideBuilder : AbstractObjectBuilder<VirtualBoxIsoProvisionerOverride, VirtualBoxIsoProvisionerOverrideBuilder>
{
    /// <summary>
    /// The command to execute during the provisioner override step.
    /// </summary>
    /// <remarks>Example: "echo Hello World".</remarks>
    private string? _executeCommand;

    /// <summary>
    /// Sets the command to execute during the provisioner override step.
    /// </summary>
    /// <param name="value">Command string to execute (e.g., "echo Hello World").</param>
    /// <returns>The builder instance.</returns>
    /// <remarks>Example: ExecuteCommand("echo Hello World")</remarks>
    public VirtualBoxIsoProvisionerOverrideBuilder ExecuteCommand(string? value)
    {
        _executeCommand = value;
        return this;
    }

    /// <summary>
    /// Builds the <see cref="VirtualBoxIsoProvisionerOverride"/> instance using the configured properties.
    /// </summary>
    /// <param name="exceptions">Dictionary to collect build exceptions.</param>
    /// <param name="visited">List of visited objects for cyclic dependency detection.</param>
    /// <returns>The build result containing the constructed <see cref="VirtualBoxIsoProvisionerOverride"/> or failure details.</returns>
    /// <remarks>
    /// All required properties are validated before building. If any required property is missing or invalid, the build will fail and exceptions will be collected.
    /// </remarks>
    protected override IObjectBuildResult<VirtualBoxIsoProvisionerOverride> BuildInternal(ExceptionBuildDictionary exceptions, VisitedObjectsList visited)
    {
        AssertNotEmptyOrWhitespace(_executeCommand, nameof(VirtualBoxIsoProvisionerOverride.ExecuteCommand), exceptions);

        if (exceptions.Count > 0)
            return Failure(exceptions, visited);

        return Success(new VirtualBoxIsoProvisionerOverride
        {
            ExecuteCommand = _executeCommand
        });
    }
}