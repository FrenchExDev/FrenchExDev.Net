using FrenchExDev.Net.CSharp.Object.Model.Abstractions;
using FrenchExDev.Net.CSharp.Object.Model.Testing;
using Shouldly;

namespace FrenchExDev.Net.CSharp.Object.Model.Tests;

/// <summary>
/// Contains unit tests for validating the behavior of NamespaceDeclarationModel building and code generation.
/// </summary>
public class NamespaceDeclarationModelSyntaxCodeTests
{
    /// <summary>
    /// Tests that building a namespace without specifying a name results in a failure with the correct exception message.
    /// </summary>
    [Fact]
    public void Cannot_Build_Namespace_Without_Name()
    {
        // Attempt to build a namespace without a name and assert that the result is a failure with the expected exception.
        ModelSyntaxCodeTester.Invalid<NamespaceDeclarationModel, NamespaceDeclarationModelBuilder>(
            body: (builder) =>
            {
                // No name configured
            },
            assertResult: (result) =>
            {
                result.Count().ShouldBe(1);
            });
    }

    /// <summary>
    /// Tests that a simple namespace with a class and a field can be modeled, built, and code-generated correctly.
    /// </summary>
    [Fact]
    public void Can_Model_And_Build_And_Generate_Simple_Namespace_With_Class_With_Field()
    {
        // Build a namespace with a public class containing a private int field with an initializer, then assert the model and generated code.
        ModelSyntaxCodeTester.Valid<NamespaceDeclarationModel, NamespaceDeclarationModelBuilder>(
            body: (builder) =>
            {
                builder
                .WithName("FrenchExDev.MyDev.Lib")
                .FileScoped()
                .WithClass((b) =>
                {
                    b.WithName("MyFoo")
                    .WithModifier(ClassModifier.Public)
                    .WithField(f => f
                        .WithModifier("private")
                        .WithType("int")
                        .WithName("_myField")
                        .Initializer("0"));
                });
            }, assertBuiltModel: (@namespace) =>
            {
                // Assert the namespace and class structure
                @namespace.Name.ShouldBe("FrenchExDev.MyDev.Lib");
                @namespace.Classes.Count().ShouldBe(1);
                @namespace.Classes.ElementAt(0).Name.ShouldBe("MyFoo");
                @namespace.Classes.ElementAt(0).Modifiers.ShouldContain(ClassModifier.Public);
                @namespace.Classes.ElementAt(0).Fields.Count().ShouldBe(1);
                @namespace.Classes.ElementAt(0).Fields.ElementAt(0).Name.ShouldBe("_myField");
                @namespace.Classes.ElementAt(0).Fields.ElementAt(0).Type.ShouldBe("int");
                @namespace.Classes.ElementAt(0).Fields.ElementAt(0).Modifiers.ShouldContain("private");

            }, assertGeneratedCode: (namespaceGeneratedCode) =>
            {
                // Assert the generated C# code matches the expected output
                namespaceGeneratedCode.ShouldBeEquivalentTo(@"namespace FrenchExDev.MyDev.Lib
{
    public class MyFoo
    {
        private int _myField = 0;
    }
}");
            });
    }
}
