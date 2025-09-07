using FrenchExDev.Net.CSharp.Object.Model.Abstractions;
using FrenchExDev.Net.CSharp.Object.Model.Infrastructure;
using Microsoft.CodeAnalysis.CSharp.Syntax;

/// <summary>
/// Extensions for converting model objects to Roslyn syntax nodes.
/// </summary>
public static class ModelSyntaxExtensions
{
    /// <summary>
    /// Converts an <see cref="IDeclarationModel"/> to the appropriate Roslyn <see cref="MemberDeclarationSyntax"/>.
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    /// <exception cref="NotSupportedException"></exception>
    public static MemberDeclarationSyntax ToSyntax(this IDeclarationModel model)
    {
        return new ModelRoslynConverter().ToSyntax(model);
    }
}
