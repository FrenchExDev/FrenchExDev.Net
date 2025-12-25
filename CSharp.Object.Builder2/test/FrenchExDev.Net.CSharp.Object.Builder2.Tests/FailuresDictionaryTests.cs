using Shouldly;
namespace FrenchExDev.Net.CSharp.Object.Builder2.Tests;

public class FailuresDictionaryTests
{
    [Fact]
    public void AddFailure_ShouldAddToCorrectMember()
    {
        var dict = new FailuresDictionary();

        dict.AddFailure("field1", Failure.FromMessage("error1"));

        dict.HasFailures.ShouldBeTrue();
        dict.FailureCount.ShouldBe(1);
        dict["field1"].Count.ShouldBe(1);
    }

    [Fact]
    public void AddFailure_SameField_ShouldAppend()
    {
        var dict = new FailuresDictionary();

        dict.AddFailure("field", Failure.FromMessage("error1"));
        dict.AddFailure("field", Failure.FromMessage("error2"));

        dict["field"].Count.ShouldBe(2);
    }

    [Fact]
    public void Failure_ShouldReturnSelfForChaining()
    {
        var dict = new FailuresDictionary();

        var result = dict
            .Failure("f1", Failure.FromMessage("e1"))
            .Failure("f2", Failure.FromMessage("e2"));

        result.ShouldBeSameAs(dict);
        dict.FailureCount.ShouldBe(2);
    }

    [Fact]
    public void HasFailures_WhenEmpty_ShouldBeFalse()
    {
        var dict = new FailuresDictionary();

        dict.HasFailures.ShouldBeFalse();
    }
}

