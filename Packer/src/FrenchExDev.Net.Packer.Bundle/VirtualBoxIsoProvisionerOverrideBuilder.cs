#region Licensing

// Copyright Stéphane Erard
// For licensing, please contact stephane.erard@gmail

#endregion

#region Usings

using FrenchExDev.Net.CSharp.Object.Builder2;

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
public class VirtualBoxIsoProvisionerOverrideBuilder : AbstractBuilder<VirtualBoxIsoProvisionerOverride>
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
    /// Creates a new instance of the derived VirtualBoxIsoProvisionerOverride class.
    /// </summary>
    /// <returns>A new instance of VirtualBoxIsoProvisionerOverride.</returns>
    /// <exception cref="NotImplementedException">Always thrown, as this method is not implemented.</exception>
    protected override VirtualBoxIsoProvisionerOverride Instantiate()
    {
        return new(_executeCommand);
    }

    protected override void ValidateInternal(VisitedObjectDictionary visitedCollector, FailuresDictionary failures)
    {
        AssertNotEmptyOrWhitespace(_executeCommand, nameof(VirtualBoxIsoProvisionerOverride.ExecuteCommand), failures, (s) => new StringIsEmptyOrWhitespaceException(s));
    }
}