using Shouldly;
using FrenchExDev.Net.CSharp.Object.Result2;

namespace FrenchExDev.Net.CSharp.Object.Builder2.Tests;

public class AbstractBuilderBranchCoverageTests
{
    [Fact]
    public void Build_WithVisitedDictionary_PassingDifferentId_ShouldNotTriggerCircularRefPath()
    {
        var builder = new PersonBuilder().WithName("Test").WithAge(25);
        var visited = new VisitedObjectDictionary();
        var differentId = Guid.NewGuid();
        visited.MarkVisited(differentId, builder);

        var result = builder.Build(visited);

        result.IsSuccess.ShouldBeTrue();
    }

    [Fact]
    public void Build_WithVisitedDictionary_PassingSameIdButNotIBuilder_ShouldNotTriggerCircularRefPath()
    {
        var builder = new PersonBuilder().WithName("Test").WithAge(25);
        var visited = new VisitedObjectDictionary();
        visited.MarkVisited(builder.Id, "not a builder");

        var result = builder.Build(visited);

        result.IsSuccess.ShouldBeTrue();
    }

    [Fact]
    public void Build_WhenAlreadyBuilt_ShouldReturnCachedResult()
    {
        var builder = new PersonBuilder().WithName("Test").WithAge(25);
        
        var result1 = builder.Build();
        var result2 = builder.Build();

        result1.Value.ShouldBeSameAs(result2.Value);
        builder.BuildStatus.ShouldBe(BuildStatus.Built);
    }

    [Fact]
    public void Validate_WhenAlreadyValidated_ShouldSkip()
    {
        var builder = new PersonBuilder().WithName("Test").WithAge(25);
        var visited = new VisitedObjectDictionary();
        var failures1 = new FailuresDictionary();
        var failures2 = new FailuresDictionary();

        builder.Validate(visited, failures1);
        builder.ValidationStatus.ShouldBe(ValidationStatus.Validated);
        
        builder.Validate(visited, failures2);
        builder.ValidationStatus.ShouldBe(ValidationStatus.Validated);
    }

    [Fact]
    public void Result_WhenNotBuilt_ShouldThrow()
    {
        var builder = new PersonBuilder().WithName("Test").WithAge(25);

        Should.Throw<InvalidOperationException>(() => _ = builder.Result);
    }

    [Fact]
    public void Result_WhenBuilt_ShouldReturnInstance()
    {
        var builder = new PersonBuilder().WithName("Test").WithAge(25);
        builder.Build();

        var result = builder.Result;

        result.ShouldNotBeNull();
        result!.Name.ShouldBe("Test");
    }

    [Fact]
    public void Build_WithExisting_InsideBuildCore_ShouldUseExisting()
    {
        var existing = new Person { Name = "Existing", Age = 40 };
        var builder = new PersonBuilder().Existing(existing);
        var visited = new VisitedObjectDictionary();

        var result = builder.Build(visited);

        result.IsSuccess.ShouldBeTrue();
        result.Value.Resolved().ShouldBeSameAs(existing);
    }

    [Fact]
    public void BuildStatus_Initially_ShouldBeNotBuilding()
    {
        var builder = new PersonBuilder();

        builder.BuildStatus.ShouldBe(BuildStatus.NotBuilding);
    }

    [Fact]
    public void ValidationStatus_Initially_ShouldBeNotValidated()
    {
        var builder = new PersonBuilder();

        builder.ValidationStatus.ShouldBe(ValidationStatus.NotValidated);
    }

    [Fact]
    public void Build_WithCircularReference_ValidationNotValidated_ShouldReturnUnresolvedReference()
    {
        var builder = new PersonBuilder().WithName("Test").WithAge(25);
        var visited = new VisitedObjectDictionary();
        
        // Mark as visited but with NotValidated status (simulating circular reference during validation)
        visited.MarkVisited(builder.Id, builder);
        // ValidationStatus is NotValidated by default

        var result = builder.Build(visited);

        result.IsSuccess.ShouldBeTrue();
    }

    [Fact]
    public void Build_WithCircularReference_BuildBuilding_ShouldReturnReference()
    {
        var builder = new PersonBuilder().WithName("Test").WithAge(25);
        var visited = new VisitedObjectDictionary();
        
        // First build to set status to Built
        builder.Build();
        
        // Mark as visited
        visited.MarkVisited(builder.Id, builder);

        // Build again with visited - should hit the BuildStatus.Built path
        var result = builder.Build(visited);

        result.IsSuccess.ShouldBeTrue();
        result.Value.IsResolved.ShouldBeTrue();
    }

    [Fact]
    public void Build_WithCircularReference_SelfReference_ShouldHandleGracefully()
    {
        // Create a builder that references itself during build
        var builder = new CircularRefTestBuilder().WithName("Test");
        
        var result = builder.Build();
        
        result.IsSuccess.ShouldBeTrue();
    }
}

