using FrenchExDev.Net.CSharp.Object.Result2;

namespace FrenchExDev.Net.CSharp.Object.Builder2;

/// <summary>
/// Defines a contract for objects that can be built.
/// </summary>
public interface IBuildable<TClass> where TClass : class
{
    /// <summary>
    /// Gets the current status of the build process.
    /// </summary>
    BuildStatus BuildStatus { get; }

    /// <summary>
    /// Builds a reference to the target object of type TClass, tracking visited objects to prevent cycles.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Use the visited parameter to enable cycle detection when building complex object graphs. This
    /// is particularly important when objects may reference each other, as it prevents infinite recursion.
    /// </para>
    /// <para>
    /// The returned <see cref="Reference{TClass}"/> may be resolved (if build succeeded) or unresolved
    /// (if a circular reference was detected and the build is still in progress). Use 
    /// <see cref="Reference{TClass}.IsResolved"/> to check, or <see cref="Reference{TClass}.ResolvedOrNull"/>
    /// for safe access.
    /// </para>
    /// </remarks>
    /// <param name="visited">An optional dictionary used to record objects that have already been visited during the build process. If null,
    /// a new dictionary is created internally.</param>
    /// <returns>
    /// A <see cref="Result{T}"/> containing a <see cref="Reference{TClass}"/> to the built object on success,
    /// or a failure with validation errors. The reference may be unresolved if circular reference handling is in progress.
    /// </returns>
    Result<Reference<TClass>> Build(VisitedObjectDictionary? visited = null);
}
