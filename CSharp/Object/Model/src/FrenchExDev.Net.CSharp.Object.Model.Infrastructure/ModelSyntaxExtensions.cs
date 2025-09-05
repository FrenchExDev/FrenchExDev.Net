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
        return model switch
        {
            NamespaceDeclarationModel namespaceModel => RoslynModelConverter.ToSyntax(namespaceModel),
            ClassDeclarationModel classModel => RoslynModelConverter.ToSyntax(classModel),
            InterfaceDeclarationModel interfaceModel => RoslynModelConverter.ToSyntax(interfaceModel),
            EnumDeclarationModel enumModel => RoslynModelConverter.ToSyntax(enumModel),
            StructDeclarationModel structModel => RoslynModelConverter.ToSyntax(structModel),
            PropertyDeclarationModel propertyModel => RoslynModelConverter.ToSyntax(propertyModel),
            MethodDeclarationModel methodModel => RoslynModelConverter.ToSyntax(methodModel),
            FieldDeclarationModel fieldModel => RoslynModelConverter.ToSyntax(fieldModel),
            EventModel eventModel => RoslynModelConverter.ToSyntax(eventModel),
            ConstructorDeclarationModel constructorModel => RoslynModelConverter.ToSyntax(constructorModel),
            _ => throw new NotSupportedException($"Unsupported model type: {model.GetType().FullName}")
        };
    }
}
