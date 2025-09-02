using FrenchExDev.Net.CSharp.Object.Builder.Abstractions;
using FrenchExDev.Net.CSharp.Object.Model.Abstractions;
using FrenchExDev.Net.CSharp.Object.Model.Infrastructure;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Shouldly;

namespace FrenchExDev.Net.CSharp.Object.Model.Testing;

public static class ModelSyntaxCodeTester
{
    public static void Valid<TModel, TBuilder>(
         Action<TBuilder> body,
         Action<TModel> assertBuiltModel,
         Action<string> assertGeneratedCode
     )
         where TModel : class
         where TBuilder : IObjectBuilder<TModel>, new()
    {
        var builder = new TBuilder();
        body(builder);
        var buildResult = builder.Build();
        buildResult.ShouldBeAssignableTo<SuccessObjectBuildResult<TModel>>();
        var builtModel = buildResult.Success<TModel>();
        assertBuiltModel(builtModel);

        var builtModelSyntax = builtModel switch
        {
            NamespaceDeclarationModel namespaceDeclarationModel => namespaceDeclarationModel.ToSyntax() as MemberDeclarationSyntax,
            InterfaceDeclarationModel interfaceModel => interfaceModel.ToSyntax() as MemberDeclarationSyntax,
            ClassDeclarationModel classModel => classModel.ToSyntax() as MemberDeclarationSyntax,
            EnumDeclarationModel enumModel => enumModel.ToSyntax() as MemberDeclarationSyntax,
            StructDeclarationModel structModel => structModel.ToSyntax() as MemberDeclarationSyntax,
            ConstructorDeclarationModel constructorModel => constructorModel.ToSyntax() as MemberDeclarationSyntax,
            PropertyDeclarationModel propertyModel => propertyModel.ToSyntax() as MemberDeclarationSyntax,
            MethodDeclarationModel methodModel => methodModel.ToSyntax() as MemberDeclarationSyntax,
            FieldDeclarationModel fieldModel => fieldModel.ToSyntax() as MemberDeclarationSyntax,
            EventModel eventModel => eventModel.ToSyntax() as MemberDeclarationSyntax,
            _ => throw new NotSupportedException($"Unsupported model type: {typeof(TModel).FullName}")
        };

        var builtModelSyntaxGeneratedCode = SyntaxFactory.CompilationUnit()
            .AddMembers(builtModelSyntax)
            .NormalizeWhitespace();

        assertGeneratedCode(builtModelSyntaxGeneratedCode.ToFullString());
    }

    public static void NamespaceInvalid(
        Action<NamespaceDeclarationModelBuilder> configure,
        Action<IObjectBuildResult<NamespaceDeclarationModel>> assertResult
    )
    {
        var builder = new NamespaceDeclarationModelBuilder();
        configure(builder);
        var buildResult = builder.Build();
        assertResult(buildResult);
    }
}
