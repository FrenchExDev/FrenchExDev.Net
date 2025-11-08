using Shouldly;
using FrenchExDev.Net.CSharp.Object.Builder2;

namespace FrenchExDev.Net.CSharp.Object.Builder2.Tests.Builder;

public class BuilderList_Tests
{
    [Fact]
    public void Given_BuilderList_When_AsReferenceListAndBuildSuccess_Then_ReferencesResolve()
    {
        var l = new BuilderList<string, SimpleBuilder>();
        l.New(b => b.Value("a"));
        l.New(b => b.Value("b"));
        var refs = l.AsReferenceList();
        refs.Any().ShouldBeFalse(); // not built yet

        var built = l.BuildSuccess();
        built.Count.ShouldBe(2);
        built[0].ShouldBe("a");
        built[1].ShouldBe("b");
    }
}
