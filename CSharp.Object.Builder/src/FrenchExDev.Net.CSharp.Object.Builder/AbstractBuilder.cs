using FrenchExDev.Net.CSharp.Object.Builder.Abstractions;

namespace FrenchExDev.Net.CSharp.Object.Builder;

/// <summary>
/// Abstract base class for building objects of type <typeparamref name="TClass"/>.
/// </summary>
/// <typeparam name="TClass"></typeparam>
public abstract class AbstractBuilder<TClass> : IBuilder<TClass>
{
    /// <summary>
    /// Builds an instance of <typeparamref name="TClass"/>.
    /// </summary>
    /// <returns></returns>
    public abstract IBuildResult<TClass> Build();
}
