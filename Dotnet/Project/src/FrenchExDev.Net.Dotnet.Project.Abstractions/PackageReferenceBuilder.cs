using FrenchExDev.Net.CSharp.Object.Builder2;

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
public class PackageReferenceBuilder : AbstractBuilder<IPackageReference>
{
    /// <summary>
    /// Stores the package name to be used in the reference.
    /// </summary>
    /// <remarks>Set via <see cref="Name(string)"/> method.</remarks>
    private string? _name;

    /// <summary>
    /// Holds the package version builder for constructing the version reference.
    /// </summary>
    private PackageVersionBuilder? _version;

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
    /// Configures the package version using the specified builder action.
    /// </summary>
    /// <param name="body">An action that receives a <see cref="PackageVersionBuilder"/> to define the version details for the package.</param>
    /// <returns>The current <see cref="PackageReferenceBuilder"/> instance with the configured version.</returns>
    public PackageReferenceBuilder Version(Action<PackageVersionBuilder> body)
    {
        var builder = new PackageVersionBuilder();
        body(builder);
        _version = builder;
        return this;
    }

    /// <summary>
    /// Performs any additional build operations required for the object graph. In this implementation, no further
    /// actions are taken.
    /// </summary>
    /// <param name="visitedCollector">A dictionary used to track objects that have already been visited during the build process. This parameter is
    /// provided for consistency with the base implementation.</param>
    protected override void BuildInternal(VisitedObjectDictionary visitedCollector)
    {
        // nothing to do
    }

    /// <summary>
    /// Creates a package reference based on the current name and version settings.
    /// </summary>
    /// <returns>An <see cref="IPackageReference"/> instance representing the package. If a version is specified, returns a
    /// versioned package reference; otherwise, returns a non-versioned package reference.</returns>
    /// <exception cref="InvalidDataException">Thrown if the package name is null when a version is specified.</exception>
    protected override IPackageReference Instantiate()
    {
        if (_name is null) throw new InvalidDataException(nameof(_name));

        switch (_version)
        {
            case not null:
                return new PackageVersionReference(new PackageName(_name), _version.Reference());
            default:
                return new PackageReference(new PackageName(_name));
        }
    }

    /// <summary>
    /// Performs validation on the current object and records any validation failures encountered.
    /// </summary>
    /// <param name="visitedCollector">A dictionary used to track objects that have already been visited during validation to prevent redundant checks
    /// and handle circular references.</param>
    /// <param name="failures">A dictionary for collecting validation failures, where each failure is associated with a specific property or
    /// field.</param>
    protected new void ValidateInternal(VisitedObjectDictionary visitedCollector, FailuresDictionary failures)
    {
        if (string.IsNullOrEmpty(_name)) failures.Failure(nameof(_name), new ArgumentException("Name is required"));
    }
}
