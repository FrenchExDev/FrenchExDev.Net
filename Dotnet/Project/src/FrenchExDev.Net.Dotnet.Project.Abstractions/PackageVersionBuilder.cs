using FrenchExDev.Net.CSharp.Object.Builder2;

namespace FrenchExDev.Net.Dotnet.Project.Abstractions;

/// <summary>
/// Provides a builder for creating instances of <see cref="PackageVersion"/> with a specified version string.
/// </summary>
/// <remarks>Use <see cref="Version(string)"/> to set the required version before building. If the version is not
/// set, building the object will throw a <see cref="PackageVersionBuilder.MissingVersionException"/>. This class is
/// typically used to construct <see cref="PackageVersion"/> objects in a controlled manner, ensuring that all required
/// information is provided.</remarks>
public class PackageVersionBuilder : AbstractBuilder<IPackageVersion>
{
    /// <summary>
    /// Represents an exception that is thrown when a required version value is missing.
    /// </summary>
    /// <remarks>This exception is typically thrown to indicate that an operation or method requires a version
    /// to be specified, but none was provided. It can be used to signal configuration or API contract violations where
    /// versioning is mandatory.</remarks>
    public class MissingVersionException : Exception
    {
        public MissingVersionException() : base("Version is required")
        {
        }
    }

    /// <summary>
    /// Stores the version string for the package.
    /// </summary>
    private string? _version;

    /// <summary>
    /// Sets the package version to use for the builder.
    /// </summary>
    /// <param name="version">The version string to assign to the package. Must not be null or empty.</param>
    /// <returns>The current <see cref="PackageVersionBuilder"/> instance with the updated version.</returns>
    public PackageVersionBuilder Version(string version)
    {
        _version = version;
        return this;
    }

    /// <summary>
    /// Creates a new instance of the <see cref="PackageVersion"/> class using the current version information.
    /// </summary>
    /// <returns>A <see cref="PackageVersion"/> object initialized with the current version data.</returns>
    /// <exception cref="MissingVersionException">Thrown if the current version information is not available.</exception>
    protected override PackageVersion Instantiate()
    {
        if (_version is null) throw new MissingVersionException();
        return new PackageVersion(_version);
    }

    /// <summary>
    /// Validates that the required version information has been set, recording a failure if it is missing.
    /// </summary>
    /// <param name="visitedCollector"></param>
    /// <param name="failures"></param>
    protected override void ValidateInternal(VisitedObjectDictionary visitedCollector, FailuresDictionary failures)
    {
        if (_version is null) failures.Failure(nameof(_version), new MissingVersionException());
    }
}
