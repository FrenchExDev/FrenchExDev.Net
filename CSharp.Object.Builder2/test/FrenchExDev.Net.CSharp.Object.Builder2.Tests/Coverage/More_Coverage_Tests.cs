using System.Reflection;
using FrenchExDev.Net.CSharp.Object.Builder2;
using FrenchExDev.Net.CSharp.Object.Builder2.Testing;
using Shouldly;

namespace FrenchExDev.Net.CSharp.Object.Builder2.Tests.Coverage;

public class More_Coverage_Tests
{
    [Fact]
    public void Given_AddressBuilder_When_MissingFields_Then_InstantiateThrowsSpecificExceptions()
    {
        var ab = new AddressBuilder();
        var mi = typeof(AddressBuilder).GetMethod("Instantiate", BindingFlags.Instance | BindingFlags.NonPublic);
        mi.ShouldNotBeNull();

        var streetType = typeof(AddressBuilder).GetNestedType("StreetCannotBeNullOrEmptyException", BindingFlags.NonPublic | BindingFlags.Public);
        var cityType = typeof(AddressBuilder).GetNestedType("CityCannotBeNullOrEmptyException", BindingFlags.NonPublic | BindingFlags.Public);
        streetType.ShouldNotBeNull(); cityType.ShouldNotBeNull();

        var ex1 = Should.Throw<Exception>(() => { try { mi!.Invoke(ab, null); } catch (TargetInvocationException tie) { throw tie.InnerException!; } });
        ex1.GetType().ShouldBe(streetType);

        var ab2 = new AddressBuilder().Street("S");
        var ex2 = Should.Throw<Exception>(() => { try { mi!.Invoke(ab2, null); } catch (TargetInvocationException tie) { throw tie.InnerException!; } });
        ex2.GetType().ShouldBe(cityType);
    }

    [Fact]
    public void Given_ResultExtensions_When_SuccessOrFailure_Then_BehaviorMatches()
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
}
