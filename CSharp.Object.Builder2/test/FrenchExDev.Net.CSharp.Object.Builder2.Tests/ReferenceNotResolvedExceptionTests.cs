using Shouldly;
namespace FrenchExDev.Net.CSharp.Object.Builder2.Tests;

public class ReferenceNotResolvedExceptionTests
{
    [Fact]
    public void DefaultConstructor_ShouldCreateException()
    {
        var ex = new ReferenceNotResolvedException();
        ex.ShouldNotBeNull();
    }

    [Fact]
    public void Constructor_WithMessage_ShouldSetMessage()
    {
        var ex = new ReferenceNotResolvedException("Test message");
        ex.Message.ShouldBe("Test message");
    }

    [Fact]
    public void Constructor_WithMessageAndInner_ShouldSetBoth()
    {
        var inner = new InvalidOperationException("Inner");
        var ex = new ReferenceNotResolvedException("Test message", inner);
        ex.Message.ShouldBe("Test message");
        ex.InnerException.ShouldBe(inner);
    }
}

