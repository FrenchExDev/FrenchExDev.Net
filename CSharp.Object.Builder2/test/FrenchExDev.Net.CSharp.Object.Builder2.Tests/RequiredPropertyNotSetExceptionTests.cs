using Shouldly;
namespace FrenchExDev.Net.CSharp.Object.Builder2.Tests;

public class RequiredPropertyNotSetExceptionTests
{
    [Fact]
    public void Constructor_ShouldSetPropertyName()
    {
        var ex = new RequiredPropertyNotSetException("TestProperty");
        ex.PropertyName.ShouldBe("TestProperty");
        ex.Message.ShouldContain("TestProperty");
    }

    [Fact]
    public void Constructor_WithInnerException_ShouldPreserveIt()
    {
        var inner = new InvalidOperationException("inner");
        var ex = new RequiredPropertyNotSetException("TestProperty", inner);
        ex.InnerException.ShouldBe(inner);
    }

    [Fact]
    public void ShouldBeInvalidOperationException()
    {
        var ex = new RequiredPropertyNotSetException("Test");
        ex.ShouldBeAssignableTo<InvalidOperationException>();
    }
}

