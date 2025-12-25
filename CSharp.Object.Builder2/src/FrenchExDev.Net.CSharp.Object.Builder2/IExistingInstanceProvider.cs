namespace FrenchExDev.Net.CSharp.Object.Builder2;

/// <summary>
/// Marker interface for builders that can provide an existing instance.
/// </summary>
/// <typeparam name="TClass">The type of the existing instance.</typeparam>
public interface IExistingInstanceProvider<TClass> where TClass : class
{
    /// <summary>
    /// Gets whether an existing instance has been set.
    /// </summary>
    bool HasExisting { get; }

    /// <summary>
    /// Gets the existing instance, if set.
    /// </summary>
    TClass? ExistingInstance { get; }
}
