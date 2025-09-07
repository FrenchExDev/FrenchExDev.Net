using FrenchExDev.Net.CSharp.Object.Builder.Abstractions;
using FrenchExDev.Net.CSharp.Object.Model.Abstractions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
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
         where TModel : class, IDeclarationModel
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
        var builtModelSyntax = builtModel.ToSyntax();
        builtModelSyntax.ShouldNotBeNull();

        // Generate the C# code from the syntax node and normalize whitespace
        var builtModelSyntaxGeneratedCode = SyntaxFactory.CompilationUnit()
            .AddMembers(builtModelSyntax)
            .NormalizeWhitespace();

        // Assert the generated code using the provided assertion
        assertGeneratedCode(builtModelSyntaxGeneratedCode.ToFullString());
    }

    /// <summary>
    /// Validates that a builder produces an invalid model and allows assertions on the build result.
    /// </summary>
    /// <typeparam name="TModel">The type of the model to build (e.g., class, interface, enum).</typeparam>
    /// <typeparam name="TBuilder">The type of the builder for the model.</typeparam>
    /// <param name="body">An action to configure the builder.</param>
    /// <param name="assertResult">An action to assert the build result (expected to be invalid).</param>
    /// <remarks>
    /// Use this method to test scenarios where the builder should fail validation, such as missing required properties.
    /// Example:
    /// <code>
    /// ModelSyntaxCodeTester.Invalid<MyModel, MyModelBuilder>(
    ///     builder => builder.Name(null),
    ///     result => result.ShouldBeAssignableTo<FailureObjectBuildResult<MyModel, MyModelBuilder>>()
    /// );
    /// </code>
    /// </remarks>
    public static void Invalid<TModel, TBuilder>(
         Action<TBuilder> body,
         Action<IObjectBuildResult<TModel>> assertResult
     )
         where TModel : class, IDeclarationModel
         where TBuilder : IObjectBuilder<TModel>, new()
    {
        // Instantiate the builder and configure it using the provided action
        var builder = new TBuilder();
        body(builder);
        // Build the model
        var buildResult = builder.Build();

        // Asserts the type of buildResult as Failure object
        buildResult.ShouldBeAssignableTo<FailureObjectBuildResult<TModel, TBuilder>>();

        // Assert the build result using the provided assertion (expected to be invalid)
        assertResult(buildResult);
    }
}
