namespace FrenchExDev.Net.CSharp.Object.Builder.Abstractions;

/// <summary>
/// Provides a wrapper for an object of type <typeparamref name="TClass"/> that allows actions to be registered and executed when the reference is updated.
/// </summary>
/// <remarks>
/// Actions added via <see cref="AddAction"/> are executed immediately after the reference is set using <see cref="SetReference"/>.
/// This class can be used to coordinate initialization or post-processing steps that depend on the referenced object.
/// </remarks>
/// <typeparam name="TClass">The type of the object being referenced and managed.</typeparam>
/// <example>
/// var buildRef = new BuildReference<MyClass>(null);
/// buildRef.AddAction(obj => Console.WriteLine($"Reference set: {obj}"));
/// buildRef.SetReference(new MyClass());
/// </example>
public class BuildReference<TClass, TBuilder> : AbstractBuildReference<TClass>, IObjectBuildResult<TClass> where TBuilder : IObjectBuilder<TClass>
{
    /// <summary>
    /// Initializes a new instance of the BuildReference class using the specified reference object.
    /// </summary>
    /// <param name="reference">The reference object to associate with this BuildReference instance. Can be null to indicate no reference.</param>
    public BuildReference(TClass? reference) : base(reference)
    {
    }
}
