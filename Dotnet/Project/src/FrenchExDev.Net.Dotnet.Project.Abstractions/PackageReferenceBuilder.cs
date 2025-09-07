using FrenchExDev.Net.CSharp.Object.Builder;
using FrenchExDev.Net.CSharp.Object.Builder.Abstractions;

namespace FrenchExDev.Net.Dotnet.Project.Abstractions;

/// <summary>
/// Builder class for constructing <see cref="IPackageReference"/> instances, supporting fluent configuration of name and version.
/// </summary>
/// <remarks>
/// Example usage:
/// <code>
/// var reference = new PackageReferenceBuilder()
///     .Name("Newtonsoft.Json")
///     .Version(new PackageVersion("13.0.1"))
///     .Build();
/// </code>
/// </remarks>
public class PackageReferenceBuilder : AbstractObjectBuilder<IPackageReference, PackageReferenceBuilder>
{
    /// <summary>
    /// Stores the package name to be used in the reference.
    /// </summary>
    /// <remarks>Set via <see cref="Name(string)"/> method.</remarks>
    private string? _name;

    /// <summary>
    /// Stores the package version to be used in the reference.
    /// </summary>
    /// <remarks>Set via <see cref="Version(IPackageVersion)"/> method.</remarks>
    private IPackageVersion? _version;

    /// <summary>
    /// Sets the name of the package for the reference.
    /// </summary>
    /// <param name="name">The name of the package (e.g., "Newtonsoft.Json").</param>
    /// <returns>The current <see cref="PackageReferenceBuilder"/> instance for fluent chaining.</returns>
    /// <example>
    /// builder.Name("Newtonsoft.Json");
    /// </example>
    public PackageReferenceBuilder Name(string name)
    {
        _name = name;
        return this;
    }

    /// <summary>
    /// Sets the version of the package for the reference.
    /// </summary>
    /// <param name="version">The version object representing the package version.</param>
    /// <returns>The current <see cref="PackageReferenceBuilder"/> instance for fluent chaining.</returns>
    /// <example>
    /// builder.Version(new PackageVersion("13.0.1"));
    /// </example>
    public PackageReferenceBuilder Version(IPackageVersion version)
    {
        _version = version;
        return this;
    }

    public PackageReferenceBuilder Version(Action<PackageVersionBuilder> body)
    {
        var builder = new PackageVersionBuilder();
        body(builder);
        var _version = builder.Build().Success();
        return this;
    }
    /// <summary>
    /// Implements the build logic for creating an <see cref="IPackageReference"/> instance.
    /// </summary>
    /// <param name="exceptions">A list to collect any build exceptions.</param>
    /// <param name="visited">A dictionary of visited objects for cycle detection.</param>
    /// <returns>A build result containing the constructed <see cref="IPackageReference"/> or failure details.</returns>
    /// <remarks>
    /// If the name is not set, an exception is added and the build fails. If the version is set, a <see cref="PackageVersionReference"/> is created; otherwise, a <see cref="PackageReference"/> is created.
    /// </remarks>
    /// <example>
    /// var result = builder.Name("Newtonsoft.Json").Build();
    /// </example>
    protected override IObjectBuildResult<IPackageReference> BuildInternal(ExceptionBuildList exceptions, VisitedObjectsList visited)
    {
        if (string.IsNullOrWhiteSpace(_name))
        {
            exceptions.Add(new ArgumentException("Name is required"));
        }

        if (exceptions.Count > 0)
        {
            return Failure(exceptions, visited);
        }

        ArgumentNullException.ThrowIfNull(_name);

        if (_version is not null)
        {
            return Success(new PackageVersionReference(new PackageName(_name), _version));
        }

        return Success(new PackageReference(new PackageName(_name)));
    }
}
