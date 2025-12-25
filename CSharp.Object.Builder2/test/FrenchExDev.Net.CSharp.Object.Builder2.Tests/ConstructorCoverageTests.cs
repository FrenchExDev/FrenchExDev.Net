using Shouldly;
namespace FrenchExDev.Net.CSharp.Object.Builder2.Tests;

public class ConstructorCoverageTests
{
    [Fact]
    public void AbstractBuilder_WithNullReferenceFactory_ShouldThrow()
    {
        Should.Throw<ArgumentNullException>(() => 
            new TestBuilderWithCustomStrategy(null!, NoSynchronizationStrategy.Instance));
    }

    [Fact]
    public void AbstractBuilder_WithNullSyncStrategy_ShouldThrow()
    {
        Should.Throw<ArgumentNullException>(() => 
            new TestBuilderWithCustomStrategy(DefaultReferenceFactory.Instance, null!));
    }
}

