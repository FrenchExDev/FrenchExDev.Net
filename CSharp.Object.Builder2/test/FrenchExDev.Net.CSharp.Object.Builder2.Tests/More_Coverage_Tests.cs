using System.Reflection;
using FrenchExDev.Net.CSharp.Object.Builder2;
using FrenchExDev.Net.CSharp.Object.Builder2.Testing;
using Shouldly;

namespace FrenchExDev.Net.CSharp.Object.Builder2.Tests;

public class More_Coverage_Tests
{
    [Fact]
    public void AddressBuilder_Instantiate_Throws_Specific_Exceptions()
    {
        var ab = new AddressBuilder();
        var mi = typeof(AddressBuilder).GetMethod("Instantiate", BindingFlags.Instance | BindingFlags.NonPublic);
        mi.ShouldNotBeNull();

        var streetType = typeof(AddressBuilder).GetNestedType("StreetCannotBeNullOrEmptyException", BindingFlags.NonPublic | BindingFlags.Public);
        var cityType = typeof(AddressBuilder).GetNestedType("CityCannotBeNullOrEmptyException", BindingFlags.NonPublic | BindingFlags.Public);
        streetType.ShouldNotBeNull(); cityType.ShouldNotBeNull();

        // no street => StreetCannotBeNullOrEmptyException
        var ex1 = Should.Throw<Exception>(() => {
            try { mi!.Invoke(ab, null); }
            catch (TargetInvocationException tie) { throw tie.InnerException!; }
        });
        ex1.GetType().ShouldBe(streetType);

        // street set, no city => CityCannotBeNullOrEmptyException
        var ab2 = new AddressBuilder().Street("S");
        var ex2 = Should.Throw<Exception>(() => {
            try { mi!.Invoke(ab2, null); }
            catch (TargetInvocationException tie) { throw tie.InnerException!; }
        });
        ex2.GetType().ShouldBe(cityType);
    }

    [Fact]
    public void Extensions_Success_And_Failures_Behavior()
    {
        IResult success = new SuccessResult<string>("ok");
        success.IsSuccess<string>().ShouldBeTrue();
        success.Success<string>().ShouldBe("ok");
        var ex = Should.Throw<Exception>(() => success.Failures());
        (ex is BuildSucceededException || ex is NotSupportedException).ShouldBeTrue();

        var failuresDict = new FailuresDictionary();
        failuresDict.Failure("x", new System.Exception("e"));
        IResult failure = new FailureResult(failuresDict);
        failure.IsFailure().ShouldBeTrue();
        var f = failure.Failures();
        f.ShouldBeSameAs(failuresDict);
        Should.Throw<BuildFailedException>(() => failure.Success<string>());
    }

    private class TestBuilder : AbstractBuilder<string>
    {
        private string? _v;
        public TestBuilder Value(string v) { _v = v; return this; }
        protected override string Instantiate() => _v ?? string.Empty;
    }

    [Fact]
    public void BuilderList_New_And_BuildSuccess()
    {
        var list = new BuilderList<string, TestBuilder>();
        list.New(b => b.Value("a"));
        list.New(b => b.Value("b"));

        var built = list.BuildSuccess();
        built.Count.ShouldBe(2);
        built[0].ShouldBe("a");
        built[1].ShouldBe("b");

        var valFail = list.ValidateFailures();
        valFail.Count.ShouldBe(0);
    }
}
