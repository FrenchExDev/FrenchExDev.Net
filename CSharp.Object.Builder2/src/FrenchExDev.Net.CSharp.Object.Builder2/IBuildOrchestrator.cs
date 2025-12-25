using FrenchExDev.Net.CSharp.Object.Result2;

namespace FrenchExDev.Net.CSharp.Object.Builder2;

/// <summary>
/// Defines a contract for build orchestration.
/// Follows Single Responsibility Principle - separates orchestration from builder logic.
/// </summary>
public interface IBuildOrchestrator
{
    /// <summary>
    /// Orchestrates the build process for any builder type.
    /// </summary>
    /// <typeparam name="TClass">The type of object being built.</typeparam>
    /// <param name="builder">The builder to orchestrate.</param>
    /// <param name="visited">Optional tracker for visited objects to detect cycles.</param>
    /// <returns>A result containing a reference to the built object.</returns>
    Result<Reference<TClass>> Build<TClass>(IBuilder<TClass> builder, IVisitedTracker? visited = null) where TClass : class;
}
