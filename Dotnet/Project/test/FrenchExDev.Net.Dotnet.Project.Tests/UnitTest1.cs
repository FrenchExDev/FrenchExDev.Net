using FrenchExDev.Net.CSharp.Object.Model.Abstractions;
using FrenchExDev.Net.CSharp.Object.Model.Infrastructure;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace FrenchExDev.Net.Dotnet.Project.Tests;

public class UnitTest1
{
    [Fact]
    public void Test1()
    {
        var compilationUnit = SyntaxFactory.CompilationUnit();

        NamespaceDeclarationModel myModel = new NamespaceDeclarationModelBuilder()
            .WithName("MyNamespace")
            .FileScoped()
            .Interface(b => b
                .WithName("IMyInterface")
                .WithModifier(InterfaceModifier.Public)
                .WithMethod(m => m
                    .WithName("MyMethod")
                    .WithReturnType("string")
                    .WithParameter((b) => b.Name("MyParameter").Type("string").Build())
                ))
                .BuildSuccess();

        var converter = new ModelRoslynConverter();

        var myModelSyntax = converter.ToSyntax(myModel);

        var code = SyntaxFactory.CompilationUnit()
            .AddMembers(myModelSyntax)
            .NormalizeWhitespace()
            .ToFullString();

    }
}
