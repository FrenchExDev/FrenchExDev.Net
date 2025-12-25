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
    /// <remarks>Use the visited parameter to enable cycle detection when building complex object graphs. This
    /// is particularly important when objects may reference each other, as it prevents infinite recursion.</remarks>
    /// <param name="visited">An optional dictionary used to record objects that have already been visited during the build process. If null,
    /// a new dictionary is created internally.</param>
    /// <returns>A Result containing a Reference to the built object of type TClass. The Result indicates success or failure of
    /// the build operation.</returns>
    Result<Reference<TClass>> Build(VisitedObjectDictionary? visited = null);
}
