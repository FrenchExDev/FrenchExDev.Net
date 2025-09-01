using FrenchExDev.Net.CSharp.Object.Model.Abstractions;

namespace FrenchExDev.Net.CSharp.Object.Model.Tests;

public class UnitTest1
{
    [Fact]
    public void Test1()
    {
        var @namespace = new NamespaceDeclarationModelBuilder()
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
            })
            .Build();

    }
}
