using FrenchExDev.Net.CSharp.Object.Builder.Abstractions;

namespace FrenchExDev.Net.CSharp.Object.Builder;

/// <summary>
/// Provides a builder that constructs instances of <typeparamref name="TClass"/> using a specified lambda function.
/// </summary>
/// <typeparam name="TClass"></typeparam>
public class LambdaObjectBuilder<TClass, TBuilder> : AbstractObjectBuilder<TClass, LambdaObjectBuilder<TClass, TBuilder>> where TBuilder : IObjectBuilder<TClass>
{
    /// <summary>
    /// Holds the lambda function used to build the object.
    /// </summary>
    private readonly Func<TBuilder, ExceptionBuildList, VisitedObjectsList, IObjectBuildResult<TClass>> _buildFunc;

    /// <summary>
    /// Constructor for creating a new instance of <see cref="LambdaObjectBuilder{TClass}"/> with the specified build
    /// </summary>
    /// <param name="buildFunc"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public LambdaObjectBuilder(Func<TBuilder, ExceptionBuildList, VisitedObjectsList, IObjectBuildResult<TClass>> buildFunc)
    {
        _buildFunc = buildFunc ?? throw new ArgumentNullException(nameof(buildFunc));
    }

    /// <summary>
    /// Builds an object of type <typeparamref name="TClass"/> using the provided dictionary to track visited objects.
    /// </summary>
    /// <param name="visited">A dictionary used to track objects that have already been processed during the build operation.  Keys represent
    /// the objects being visited, and values represent their corresponding processed states.</param>
    /// <returns>An object that implements <see cref="IObjectBuildResult{TClass}"/>, representing the result of the build
    /// operation.</returns>
    protected override IObjectBuildResult<TClass> BuildInternal(ExceptionBuildList exceptions, VisitedObjectsList visited)
    {
        return _buildFunc((TBuilder)(this as IObjectBuilder<TClass>), exceptions, visited);
    }
}
