using FrenchExDev.Net.CSharp.Object.Builder2;
using Shouldly;

namespace FrenchExDev.Net.CSharp.Object.Builder2.Tests;

public class BuilderList_Tests
{
    public class SimpleBuilder : AbstractBuilder<string>
    {
        private string? _value;
        public SimpleBuilder Value(string v) { _value = v; return this; }
        protected override string Instantiate() => _value ?? string.Empty;
    }

    [Fact]
    public void BuilderList_AsReferenceList_And_BuildSuccess()
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
