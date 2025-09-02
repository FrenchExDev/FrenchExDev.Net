using FrenchExDev.Net.CSharp.Object.Builder.Abstractions;
using FrenchExDev.Net.CSharp.Object.Model.Abstractions;
using FrenchExDev.Net.CSharp.Object.Model.Infrastructure;
using FrenchExDev.Net.CSharp.Object.Model.Testing;
using Shouldly;

namespace FrenchExDev.Net.CSharp.Object.Model.Tests;

public class NamespaceDeclarationModelSyntaxCodeTests
{
    [Fact]
    public void Cannot_Build_Namespace_Without_Name()
    {
        ModelSyntaxCodeTester.NamespaceInvalid(
            configure: (builder) =>
            {
                // No name configured
            },
            assertResult: (result) =>
            {
                result.ShouldBeAssignableTo<FailureObjectBuildResult<NamespaceDeclarationModel, NamespaceDeclarationModelBuilder>>();
                var failedResult = result.Failure<NamespaceDeclarationModel, NamespaceDeclarationModelBuilder>();
                failedResult.Exceptions.Count().ShouldBe(1);
                failedResult.Exceptions.ElementAt(0).Message.ShouldBe("Namespace name is required.");
            });
    }

    [Fact]
    public void Can_Model_And_Build_And_Generate_Simple_Namespace_With_Class_With_Field()
    {
        ModelSyntaxCodeTester.Valid<NamespaceDeclarationModel, NamespaceDeclarationModelBuilder>(
            body: (builder) =>
            {
                builder
                .Name("FrenchExDev.MyDev.Lib")
                .Class((b) =>
                {
                    b.Name("MyFoo")
                    .Modifier(ClassModifier.Public)
                    .Field(f => f
                        .Modifier("private")
                        .Type("int")
                        .Name("_myField")
                        .Initializer("0"));
                });
            }, assertBuiltModel: (@namespace) =>
            {
                @namespace.Name.ShouldBe("FrenchExDev.MyDev.Lib");
                @namespace.Classes.Count.ShouldBe(1);
                @namespace.Classes[0].Name.ShouldBe("MyFoo");
                @namespace.Classes[0].Modifiers.ShouldContain(ClassModifier.Public);
                @namespace.Classes[0].Fields.Count.ShouldBe(1);
                @namespace.Classes[0].Fields[0].Name.ShouldBe("_myField");
                @namespace.Classes[0].Fields[0].Type.ShouldBe("int");
                @namespace.Classes[0].Fields[0].Modifiers.ShouldContain("private");

            }, assertGeneratedCode: (namespaceGeneratorCode) =>
            {
                namespaceGeneratorCode.ShouldBeEquivalentTo(@"namespace FrenchExDev.MyDev.Lib
{
    public class MyFoo
    {
        private int _myField = 0;
    }
}");
            });
    }
}
