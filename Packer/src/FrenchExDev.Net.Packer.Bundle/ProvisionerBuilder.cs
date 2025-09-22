#region Licensing

// Copyright Stéphane Erard
// For licensing, please contact stephane.erard@gmail

#endregion

#region Usings

using FrenchExDev.Net.CSharp.Object.Builder2;

#endregion

namespace FrenchExDev.Net.Packer.Bundle;

/// <summary>
/// Provides a builder for configuring and creating instances of the <see cref="Provisioner"/> class. Enables fluent
/// specification of provisioner type, scripts, override settings, and pause duration before execution.
/// </summary>
/// <remarks>Use <see cref="ProvisionerBuilder"/> to incrementally configure a provisioner by chaining method
/// calls. After setting the desired options, call the build method (inherited from <see
/// cref="AbstractBuilder{Provisioner}"/>) to create a configured <see cref="Provisioner"/> instance. This builder is
/// not thread-safe; concurrent modifications may result in undefined behavior.</remarks>
public class ProvisionerBuilder : AbstractBuilder<Provisioner>
{
    /// <summary>
    /// Represents optional override settings for the provisioner. If set, these settings will take precedence over
    /// </summary>
    private ProvisionerOverride? _override;

    private string? _pauseBefore;
    private List<string>? _scripts;
    private string? _type;

    /// <summary>
    /// Specifies a pause duration or condition to apply before provisioning begins.
    /// </summary>
    /// <param name="value">A string representing the pause duration or condition. The format and interpretation depend on the provisioning
    /// workflow requirements. Can be null to indicate no pause.</param>
    /// <returns>The current <see cref="ProvisionerBuilder"/> instance for method chaining.</returns>
    public ProvisionerBuilder PauseBefore(string? value)
    {
        _pauseBefore = value;
        return this;
    }

    /// <summary>
    /// Sets the provisioner type to use for subsequent configuration.
    /// </summary>
    /// <param name="type">The name of the provisioner type to set. Can be null to clear the current type.</param>
    /// <returns>The current <see cref="ProvisionerBuilder"/> instance to allow method chaining.</returns>
    public ProvisionerBuilder Type(string? type)
    {
        _type = type;
        return this;
    }

    /// <summary>
    /// Adds a script with the specified name to the provisioner configuration.
    /// </summary>
    /// <param name="name">The name of the script to add. Cannot be null or empty.</param>
    /// <returns>The current <see cref="ProvisionerBuilder"/> instance to allow method chaining.</returns>
    public ProvisionerBuilder AddScript(string name)
    {
        _scripts ??= new List<string>();
        _scripts.Add(name);
        return this;
    }

    /// <summary>
    /// Inserts a script name into the provisioner sequence immediately before the specified existing script.
    /// </summary>
    /// <remarks>If the specified <paramref name="before"/> script is not found in the sequence, no changes
    /// are made.</remarks>
    /// <param name="before">The name of the existing script before which the new script will be inserted. Must not be null.</param>
    /// <param name="name">The name of the script to insert into the sequence. Must not be null.</param>
    /// <returns>The current <see cref="ProvisionerBuilder"/> instance, allowing for method chaining.</returns>
    public ProvisionerBuilder BeforeScript(string before, string name)
    {
        _scripts ??= new List<string>();
        var index = _scripts.IndexOf(before);
        if (index == -1) return this;

        _scripts.Insert(index, name);
        return this;
    }

    /// <summary>
    /// Sets the override configuration for the provisioner.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public ProvisionerBuilder Override(ProvisionerOverride? value)
    {
        _override = value;
        return this;
    }

    /// <summary>
    /// Performs validation on the current object and records any validation failures encountered.
    /// </summary>
    /// <param name="visitedCollector">A dictionary used to track objects that have already been visited during validation to prevent redundant checks
    /// and circular references.</param>
    /// <param name="failures">A dictionary for collecting validation failures, where each failure is recorded with its associated field and
    /// exception.</param>
    protected new void ValidateInternal(VisitedObjectDictionary visitedCollector, FailuresDictionary failures)
    {
        if (string.IsNullOrEmpty(_type))
            failures.Failure(nameof(_type), new MissingFieldException(nameof(_type)));

    }

    /// <summary>
    /// Creates a new instance of the provisioner configured with the current settings.
    /// </summary>
    /// <returns>A <see cref="Provisioner"/> instance initialized with the specified type, scripts, override settings, and pause
    /// duration.</returns>
    protected override Provisioner Instantiate()
    {
        return new(_type, _scripts, _override, _pauseBefore);
    }
}