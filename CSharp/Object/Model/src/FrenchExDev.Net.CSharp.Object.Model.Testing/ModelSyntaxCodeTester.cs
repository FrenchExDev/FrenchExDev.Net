using FrenchExDev.Net.CSharp.Object.Builder.Abstractions;
using FrenchExDev.Net.CSharp.Object.Model.Abstractions;
using FrenchExDev.Net.CSharp.Object.Model.Infrastructure;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Shouldly;

namespace FrenchExDev.Net.CSharp.Object.Model.Testing;

/// <summary>
/// Provides utility methods for testing C# object model builders and their generated syntax/code.
/// </summary>      
public static class ModelSyntaxCodeTester
{
    /// <summary>
    /// Validates that a builder produces a valid model and generates the expected C# code.
    /// </summary>
    /// <typeparam name="TModel">The type of the model to build (e.g., class, interface, enum).</typeparam>
    /// <typeparam name="TBuilder">The type of the builder for the model.</typeparam>
    /// <param name="body">An action to configure the builder.</param>
    /// <param name="assertBuiltModel">An action to assert properties of the built model.</param>
    /// <param name="assertGeneratedCode">An action to assert the generated C# code as a string.</param>
    public static void Valid<TModel, TBuilder>(
         Action<TBuilder> body,
         Action<TModel> assertBuiltModel,
         Action<string> assertGeneratedCode
     )
         where TModel : class
         where TBuilder : IObjectBuilder<TModel>, new()
    {
        // Instantiate the builder and configure it using the provided action
        var builder = new TBuilder();
        body(builder);

        // Build the model and ensure the result is successful
        var buildResult = builder.Build();
        buildResult.ShouldBeAssignableTo<SuccessObjectBuildResult<TModel>>();
        var builtModel = buildResult.Success<TModel>();

        // Assert the built model using the provided assertion
        assertBuiltModel(builtModel);

        // Convert the built model to a Roslyn syntax node for code generation
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

        // Generate the C# code from the syntax node and normalize whitespace
        var builtModelSyntaxGeneratedCode = SyntaxFactory.CompilationUnit()
            .AddMembers(builtModelSyntax)
            .NormalizeWhitespace();

        // Assert the generated code using the provided assertion
        assertGeneratedCode(builtModelSyntaxGeneratedCode.ToFullString());
    }

    /// <summary>
    /// Tests that a namespace builder produces an invalid result and allows custom assertions on the result.
    /// </summary>
    /// <param name="configure">An action to configure the namespace builder.</param>
    /// <param name="assertResult">An action to assert the build result (expected to be invalid).</param>
    public static void NamespaceInvalid(
        Action<NamespaceDeclarationModelBuilder> configure, 
        Action<IObjectBuildResult<NamespaceDeclarationModel>> assertResult
    )
    {
        // Instantiate the namespace builder and configure it
        var builder = new NamespaceDeclarationModelBuilder();
        configure(builder);

        // Build the namespace model and assert the result
        var buildResult = builder.Build();
        assertResult(buildResult);
    }
}
