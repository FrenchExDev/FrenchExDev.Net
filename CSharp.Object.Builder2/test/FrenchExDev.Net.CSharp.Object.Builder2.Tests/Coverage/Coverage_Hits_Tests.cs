using FrenchExDev.Net.CSharp.Object.Builder2;
using Shouldly;

namespace FrenchExDev.Net.CSharp.Object.Builder2.Tests.Coverage;

public class Coverage_Hits_Tests
{
    [Fact]
    public void Given_ReferenceList_When_ConstructWithEnumerable_Then_CountAndIndexerWork()
    {
        var r = new Reference<string>().Resolve("one");
        var refs = new[] { r };

        var list = new ReferenceList<string>(refs);

        list.Count.ShouldBe(1);
        list[0].ShouldBe("one");
        list.Contains(r).ShouldBeTrue();
    }

    [Fact]
    public void Given_StringIsEmptyOrWhitespaceException_When_Constructed_Then_MessageContainsMember()
    {
        var ex = new StringIsEmptyOrWhitespaceException("memberX");
        ex.Message.ShouldContain("memberX");
    }
}
