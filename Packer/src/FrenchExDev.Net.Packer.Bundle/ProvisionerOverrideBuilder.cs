#region Licensing

// Copyright Stéphane Erard
// For licensing, please contact stephane.erard@gmail

#endregion

#region Usings

using FrenchExDev.Net.CSharp.Object.Builder2;

#endregion

namespace FrenchExDev.Net.Packer.Bundle;

/// <summary>
/// Provides a builder for configuring and creating instances of the ProvisionerOverride class with custom provisioner
/// settings.
/// </summary>
/// <remarks>Use this builder to specify overrides for provisioner configurations, such as VirtualBox ISO
/// settings, before constructing a ProvisionerOverride instance. This class is typically used in scenarios where
/// provisioner behavior needs to be customized programmatically.</remarks>
public class ProvisionerOverrideBuilder : AbstractBuilder<ProvisionerOverride>
{
    /// <summary>
    /// Represents the override configuration for VirtualBox ISO provisioning, if specified.
    /// </summary>
    private VirtualBoxIsoProvisionerOverride? _virtualBoxIso;

    /// <summary>
    /// Sets the override configuration for the VirtualBox ISO provisioner.
    /// </summary>
    /// <param name="value">The override settings to apply to the VirtualBox ISO provisioner. Specify <see langword="null"/> to remove any
    /// existing override.</param>
    /// <returns>The current <see cref="ProvisionerOverrideBuilder"/> instance to allow method chaining.</returns>
    public ProvisionerOverrideBuilder VirtualBoxIso(VirtualBoxIsoProvisionerOverride? value)
    {
        _virtualBoxIso = value;
        return this;
    }

    /// <summary>
    /// Instantiate a new <see cref="ProvisionerOverride"/> using the configured VirtualBox ISO override settings.
    /// </summary>
    /// <returns></returns>
    protected override ProvisionerOverride Instantiate()
    {
        return new ProvisionerOverride() { VirtualBoxIso = _virtualBoxIso };
    }

    /// <summary>
    /// Performs validation logic for the current object, recording visited objects and any validation failures
    /// encountered.
    /// </summary>
    /// <param name="visitedCollector">A dictionary used to track objects that have already been visited during validation to prevent redundant checks
    /// and circular references.</param>
    /// <param name="failures">A dictionary for collecting validation failures, mapping objects to their corresponding error details.</param>
    protected override void ValidateInternal(VisitedObjectDictionary visitedCollector, FailuresDictionary failures)
    {

    }
}