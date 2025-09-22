using FrenchExDev.Net.CSharp.Object.Builder2;

namespace FrenchExDev.Net.Dotnet.Project.Abstractions;

/// <summary>
/// Provides a builder for creating instances of the PackageName class with a specified name.
/// </summary>
/// <remarks>Use PackageNameBuilder to configure and construct PackageName objects in a fluent manner. The builder
/// enforces that a valid name is set before instantiation. This class is not thread-safe; concurrent access should be
/// synchronized if used across multiple threads.</remarks>
public class PackageNameBuilder : AbstractBuilder<PackageName>
{
    /// <summary>
    /// Stores the name of the package to be created.
    /// </summary>
    private string? _name;

    /// <summary>
    /// Sets the package name to the specified value and returns the current builder instance for method chaining.
    /// </summary>
    /// <remarks>This method enables fluent configuration by returning the same builder instance. If <paramref
    /// name="name"/> is null, subsequent operations may fail.</remarks>
    /// <param name="name">The package name to assign. Cannot be null.</param>
    /// <returns>The current <see cref="PackageNameBuilder"/> instance with the updated package name.</returns>
    public PackageNameBuilder Name(string name)
    {
        _name = name;
        return this;
    }

    /// <summary>
    /// Creates a new instance of the <see cref="PackageName"/> class using the configured name.
    /// </summary>
    /// <returns>A <see cref="PackageName"/> initialized with the specified name.</returns>
    /// <exception cref="ArgumentException">Thrown if the name is not set or is null.</exception>
    protected override PackageName Instantiate()
    {
        if (_name is null) throw new ArgumentException("Name is required");
        return new PackageName(_name);
    }

    /// <summary>
    /// Performs validation logic for the current object and records any validation failures encountered.
    /// </summary>
    /// <param name="visitedCollector">A dictionary used to track objects that have already been visited during validation to prevent redundant checks
    /// or circular references.</param>
    /// <param name="failures">A dictionary for collecting validation failures, where each failure is recorded with its associated property
    /// name and exception details.</param>
    protected new void ValidateInternal(VisitedObjectDictionary visitedCollector, FailuresDictionary failures)
    {
        if (string.IsNullOrWhiteSpace(_name)) failures.Failure(nameof(_name), new ArgumentException("Name cannot be empty"));
    }
}
