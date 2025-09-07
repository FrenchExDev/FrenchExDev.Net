using FrenchExDev.Net.CSharp.Object.Builder.Abstractions;
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
            .Name("MyNamespace")
            .Interface(b => b
                .Name("IMyInterface")
                .Modifier(InterfaceModifier.Public)
                .Method(m => m
                    .Name("MyMethod")
                    .ReturnType("string")
                    .Parameter((b) => b.Name("MyParameter").Type("string").Build())
                ))
                .Build().Success();

        var converter = new ModelRoslynConverter();

        var myModelSyntax = converter.ToSyntax(myModel);

        var code = SyntaxFactory.CompilationUnit()
            .AddMembers(myModelSyntax)
            .NormalizeWhitespace()
            .ToFullString();

    }
}
