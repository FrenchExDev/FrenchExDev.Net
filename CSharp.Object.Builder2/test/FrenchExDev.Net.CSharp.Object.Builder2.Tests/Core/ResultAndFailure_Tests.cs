using FrenchExDev.Net.CSharp.Object.Builder2;
using Shouldly;

namespace FrenchExDev.Net.CSharp.Object.Builder2.Tests.Core;

public class ResultAndFailure_Tests
{
    [Fact]
    public void Given_SuccessResult_When_Accessed_Then_Returns_Object()
    {
        IResult success = new SuccessResult<string>("ok");
        success.IsSuccess<string>().ShouldBeTrue();
        success.Success<string>().ShouldBe("ok");
    }

    [Fact]
    public void Given_SuccessResult_When_AskedFor_Failures_Then_Throws()
    {
        IResult success = new SuccessResult<string>("ok");
        var ex = Should.Throw<Exception>(() => success.Failures());
        (ex is BuildSucceededException || ex is NotSupportedException).ShouldBeTrue();
    }

    [Fact]
    public void Given_FailureResult_When_AskedFor_Success_Then_Throws()
    {
        var failuresDict = new FailuresDictionary();
        failuresDict.Failure("x", new System.Exception("e"));
        IResult failure = new FailureResult(failuresDict);
        failure.IsFailure().ShouldBeTrue();
        var f = failure.Failures();
        f.ShouldBeSameAs(failuresDict);
        Should.Throw<BuildFailedException>(() => failure.Success<string>());
    }

    [Fact]
    public void Given_FailureConversions_When_Created_Then_Values_Are_Stored()
    {
        var ex = new System.Exception("e");
        Failure f1 = ex; f1.Value.ShouldBe(ex);
        Failure f2 = "msg"; f2.Value.ShouldBe("msg");
        var dict = new FailuresDictionary(); Failure f3 = dict; f3.Value.ShouldBe(dict);
    }

    [Fact]
    public void Given_NotResolvedException_When_Constructed_Then_StoresValues()
    {
        var e = new NotResolvedException("target","source");
        e.MemberOwner.ShouldBe("target");
        e.MemberSource.ShouldBe("source");

        var e2 = new NotResolvedException("only");
        e2.MemberOwner.ShouldBe("only");
        e2.MemberSource.ShouldBe("only");
    }

    [Fact]
    public void Given_ReferenceResolve_When_CalledTwice_Then_OriginalSticks()
    {
        var r = new Reference<string>();
        r.Resolve("first");
        r.Resolve("second");
        r.Resolved().ShouldBe("first");
    }

    [Fact]
    public void Given_ResultExtensions_When_SuccessOrFailure_Then_BehaviorMatches()
    {
        IResult success = new SuccessResult<string>("ok");
        var ex = Should.Throw<Exception>(() => success.Failures());
        (ex is BuildSucceededException || ex is NotSupportedException).ShouldBeTrue();

        var failuresDict = new FailuresDictionary();
        failuresDict.Failure("a", new Failure(new System.Exception("err")));
        IResult failure = new FailureResult(failuresDict);
        Should.Throw<BuildFailedException>(() => failure.Success<string>());
    }

    [Fact]
    public void Given_BuildExceptions_When_Constructed_Then_Messages_Are_Set()
    {
        var be = new BuildSucceededException("inst");
        be.Message.ShouldBe("Build succeeded");
        var fd = new FailuresDictionary();
        var bf = new BuildFailedException(fd);
        bf.Message.ShouldBe("Build failed");
        bf.Failures.ShouldBeSameAs(fd);
    }
}
