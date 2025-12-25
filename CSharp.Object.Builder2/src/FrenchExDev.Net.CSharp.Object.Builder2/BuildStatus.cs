namespace FrenchExDev.Net.CSharp.Object.Builder2;

/// <summary>
/// Specifies the current status of a build operation.
/// </summary>
/// <remarks>
/// <para>
/// The build status follows a linear progression:
/// <c>NotBuilding</c> → <c>Building</c> → <c>Built</c>
/// </para>
/// <para>
/// If validation fails during building, the status may remain in <c>Building</c> or revert to <c>NotBuilding</c>
/// depending on the implementation.
/// </para>
/// </remarks>
/// <seealso cref="IBuildable{TClass}"/>
/// <seealso cref="ValidationStatus"/>
public enum BuildStatus 
{ 
    /// <summary>
    /// The build has not started. This is the initial state of a builder.
    /// </summary>
    NotBuilding, 

    /// <summary>
    /// The build is currently in progress. Validation has passed and instantiation is occurring.
    /// </summary>
    Building, 

    /// <summary>
    /// The build has completed successfully. The result is available and the reference is resolved.
    /// </summary>
    Built 
}
