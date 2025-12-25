using Shouldly;
namespace FrenchExDev.Net.CSharp.Object.Builder2.Tests;

public class BuilderListExtensionsTests
{
    [Fact]
    public void CreateBuilderList_ShouldCreateListWithFactory()
    {
        var list = BuilderListExtensions.CreateBuilderList<SimpleObject, SimpleObjectBuilder>(
            () => new SimpleObjectBuilder());

        list.ShouldNotBeNull();
        list.ShouldBeOfType<BuilderListWithFactory<SimpleObject, SimpleObjectBuilder>>();
    }

    [Fact]
    public void CreateBuilderList_ShouldUseProvidedFactory()
    {
        var callCount = 0;
        var list = BuilderListExtensions.CreateBuilderList<SimpleObject, SimpleObjectBuilder>(() =>
        {
            callCount++;
            return new SimpleObjectBuilder();
        });

        list.New(b => { });
        list.New(b => { });

        callCount.ShouldBe(2);
    }
}

