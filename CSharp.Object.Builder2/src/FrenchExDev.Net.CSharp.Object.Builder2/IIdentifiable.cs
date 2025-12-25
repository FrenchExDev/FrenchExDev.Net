namespace FrenchExDev.Net.CSharp.Object.Builder2;

/// <summary>
/// Defines a contract for objects that have a unique identifier.
/// </summary>
/// <remarks>
/// <para>
/// <see cref="IIdentifiable"/> is implemented by builders to provide unique identification
/// during build and validation operations. The identifier is used for:
/// </para>
/// <list type="bullet">
///   <item><description>Tracking visited objects to detect cycles</description></item>
///   <item><description>Caching build results</description></item>
///   <item><description>Correlating builders across operations</description></item>
/// </list>
/// </remarks>
/// <seealso cref="IBuilder{TClass}"/>
/// <seealso cref="VisitedObjectDictionary"/>
public interface IIdentifiable
{
    /// <summary>
    /// Gets the unique identifier for this object.
    /// </summary>
    /// <value>A <see cref="Guid"/> that uniquely identifies this instance.</value>
    /// <remarks>
    /// The identifier should remain constant throughout the lifetime of the object
    /// and should be unique within the scope of a build operation.
    /// </remarks>
    Guid Id { get; }
}
