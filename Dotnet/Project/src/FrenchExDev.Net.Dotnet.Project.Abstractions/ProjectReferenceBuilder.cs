using FrenchExDev.Net.CSharp.Object.Builder2;

namespace FrenchExDev.Net.Dotnet.Project.Abstractions;

/// <summary>
/// Provides a builder for creating and configuring instances of <see cref="ProjectReference"/> that reference a
/// specific project within a solution.
/// </summary>
/// <remarks>Use <see cref="ProjectReferenceBuilder"/> to fluently specify the referenced project and construct a
/// <see cref="ProjectReference"/> object. This builder enforces that a referenced project must be set before building.
/// The class is not thread-safe; concurrent access should be synchronized externally if required.</remarks>
public class ProjectReferenceBuilder : AbstractBuilder<ProjectReference>
{
    /// <summary>
    /// Holds the reference to the project being referenced.
    /// </summary>
    private Reference<IProjectModel>? _referencedProject;

    /// <summary>
    /// Specifies the project to be referenced by this builder.
    /// </summary>
    /// <param name="referencedProject">The project reference to associate with this builder. Cannot be null.</param>
    /// <returns>The current <see cref="ProjectReferenceBuilder"/> instance to allow method chaining.</returns>
    public ProjectReferenceBuilder ReferencedProject(Reference<IProjectModel> referencedProject)
    {
        _referencedProject = referencedProject;
        return this;
    }

    /// <summary>
    /// Performs any additional build operations required for the object. This override does not perform any actions.
    /// </summary>
    /// <param name="visitedCollector">A dictionary used to track objects that have already been visited during the build process. This parameter is
    /// provided for consistency with the base implementation.</param>
    protected override void BuildInternal(VisitedObjectDictionary visitedCollector)
    {
        // nothing to do
    }

    /// <summary>
    /// Creates a new <see cref="ProjectReference"/> instance for the referenced project.
    /// </summary>
    /// <returns>A <see cref="ProjectReference"/> representing the resolved referenced project.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the referenced project cannot be resolved.</exception>
    protected override ProjectReference Instantiate()
    {
        return new ProjectReference(_referencedProject?.Resolved() ?? throw new InvalidOperationException());
    }

    /// <summary>
    /// Performs validation logic specific to the derived class, recording any validation failures encountered.
    /// </summary>
    /// <param name="visitedCollector">A dictionary used to track objects that have already been visited during validation to prevent redundant checks
    /// and circular references.</param>
    /// <param name="failures">A dictionary for collecting validation failures. Any issues detected during validation should be added to this
    /// collection.</param>
    protected override void ValidateInternal(VisitedObjectDictionary visitedCollector, FailuresDictionary failures)
    {
        if (_referencedProject is null)
        {
            failures.Failure(nameof(_referencedProject), new InvalidOperationException("ReferencedProject is required"));
        }
    }
}
