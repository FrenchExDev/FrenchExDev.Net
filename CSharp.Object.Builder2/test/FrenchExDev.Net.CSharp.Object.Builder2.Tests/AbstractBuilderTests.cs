using Shouldly;
using FrenchExDev.Net.CSharp.Object.Result2;

namespace FrenchExDev.Net.CSharp.Object.Builder2.Tests;

public class AbstractBuilderTests
{
    [Fact]
    public void Build_WithValidData_ShouldReturnSuccessResult()
    {
        var builder = new PersonBuilder().WithName("Alice").WithAge(30);

        var result = builder.Build();

        result.IsSuccess.ShouldBeTrue();
        result.Value.IsResolved.ShouldBeTrue();
        result.Value.Resolved().Name.ShouldBe("Alice");
        result.Value.Resolved().Age.ShouldBe(30);
    }

    [Fact]
    public void Build_WithInvalidData_ShouldReturnFailureResult()
    {
        var builder = new PersonBuilder().WithAge(25); // Missing Name

        var result = builder.Build();

        result.IsSuccess.ShouldBeFalse();
        result.TryGetException<BuildFailureException>(out var ex).ShouldBeTrue();
        ex!.Failures.HasFailures.ShouldBeTrue();
    }

    [Fact]
    public void Build_WithNegativeAge_ShouldReturnFailureResult()
    {
        var builder = new PersonBuilder().WithName("John").WithAge(-5);

        var result = builder.Build();

        result.IsSuccess.ShouldBeFalse();
    }

    [Fact]
    public void Build_ShouldSetBuildStatusToBuilt()
    {
        var builder = new PersonBuilder().WithName("Test").WithAge(25);

        builder.Build();

        builder.BuildStatus.ShouldBe(BuildStatus.Built);
    }

    [Fact]
    public void Build_CalledTwice_ShouldReturnSameInstance()
    {
        var builder = new PersonBuilder().WithName("Test").WithAge(25);

        var result1 = builder.Build();
        var result2 = builder.Build();

        result1.Value.Resolved().ShouldBeSameAs(result2.Value.Resolved());
    }

    [Fact]
    public void Build_WithNestedBuilder_ShouldBuildChildren()
    {
        var builder = new PersonBuilder()
            .WithName("Alice")
            .WithAge(30)
            .WithAddress(a => a.WithStreet("123 Main St").WithCity("Paris"));

        var result = builder.Build();

        result.IsSuccess.ShouldBeTrue();
        var person = result.Value.Resolved();
        person.Address.ShouldNotBeNull();
        person.Address!.Street.ShouldBe("123 Main St");
        person.Address.City.ShouldBe("Paris");
    }

    [Fact]
    public void Build_WithFriends_ShouldBuildAllFriends()
    {
        var builder = new PersonBuilder()
            .WithName("Alice")
            .WithAge(30)
            .WithFriend(f => f.WithName("Bob").WithAge(25))
            .WithFriend(f => f.WithName("Charlie").WithAge(28));

        var result = builder.Build();

        result.IsSuccess.ShouldBeTrue();
        var person = result.Value.Resolved();
        person.Friends.Count.ShouldBe(2);
        person.Friends[0].Name.ShouldBe("Bob");
        person.Friends[1].Name.ShouldBe("Charlie");
    }

    [Fact]
    public void Reference_BeforeBuild_ShouldBeUnresolved()
    {
        var builder = new PersonBuilder().WithName("Test").WithAge(25);

        var reference = builder.Reference();

        reference.IsResolved.ShouldBeFalse();
    }

    [Fact]
    public void Reference_AfterBuild_ShouldBeResolved()
    {
        var builder = new PersonBuilder().WithName("Test").WithAge(25);

        builder.Build();
        var reference = builder.Reference();

        reference.IsResolved.ShouldBeTrue();
    }

    [Fact]
    public void Existing_ShouldUseProvidedInstance()
    {
        var existing = new Person { Name = "Existing", Age = 40 };
        var builder = new PersonBuilder().Existing(existing);

        var result = builder.Build();

        result.IsSuccess.ShouldBeTrue();
        result.Value.Resolved().ShouldBeSameAs(existing);
    }

    [Fact]
    public void HasExisting_WhenExistingSet_ShouldReturnTrue()
    {
        var existing = new Person { Name = "Existing", Age = 40 };
        var builder = new PersonBuilder().Existing(existing);

        builder.HasExisting.ShouldBeTrue();
        builder.ExistingInstance.ShouldBeSameAs(existing);
    }

    [Fact]
    public void Id_ShouldBeUnique()
    {
        var builder1 = new PersonBuilder();
        var builder2 = new PersonBuilder();

        builder1.Id.ShouldNotBe(builder2.Id);
        builder1.Id.ShouldNotBe(Guid.Empty);
    }

    [Fact]
    public void Validate_ShouldSetValidationStatus()
    {
        var builder = new PersonBuilder().WithName("Test").WithAge(25);
        var visited = new VisitedObjectDictionary();
        var failures = new FailuresDictionary();

        builder.Validate(visited, failures);

        builder.ValidationStatus.ShouldBe(ValidationStatus.Validated);
    }

    [Fact]
    public void Build_WithInvalidNestedBuilder_ShouldReturnFailure()
    {
        var builder = new PersonBuilder()
            .WithName("Alice")
            .WithAge(30)
            .WithFriend(f => f.WithAge(25)); // Friend missing name

        var result = builder.Build();

        result.IsSuccess.ShouldBeFalse();
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

    [Fact]
    public async Task Build_ConcurrentBuilds_ShouldOnlyInstantiateOnce()
    {
        var counter = new ThreadSafeCounter();
        var builder = new SlowBuilder(counter).WithValue("test");

        var tasks = Enumerable.Range(0, 5).Select(_ => Task.Run(() => builder.Build())).ToArray();
        await Task.WhenAll(tasks);

        counter.InstantiateCount.ShouldBe(1);
    }

    [Fact]
    public async Task Validate_ConcurrentValidation_ShouldOnlyValidateOnce()
    {
        var validationCount = 0;
        var validatingBuilder = new CountingValidationBuilder(() => Interlocked.Increment(ref validationCount));

        var tasks = Enumerable.Range(0, 5).Select(_ => Task.Run(() =>
        {
            var visited = new VisitedObjectDictionary();
            var failures = new FailuresDictionary();
            validatingBuilder.Validate(visited, failures);
        })).ToArray();

        await Task.WhenAll(tasks);
        validationCount.ShouldBe(1);
    }

    [Fact]
    public void Build_WithExisting_ShouldSkipValidationAndInstantiation()
    {
        var existing = new Person { Name = "Existing", Age = 99 };
        var builder = new PersonBuilder()
            .WithName("Ignored")
            .WithAge(-1) // Would fail validation if not using existing
            .Existing(existing);

        var result = builder.Build();

        result.IsSuccess.ShouldBeTrue();
        result.Value.Resolved().ShouldBeSameAs(existing);
        result.Value.Resolved().Name.ShouldBe("Existing");
    }

    [Fact]
    public void CreateFailureCollector_ShouldReturnNewInstance()
    {
        var builder = new FailureCollectorTestBuilder();
        
        var collector1 = builder.TestCreateFailureCollector();
        var collector2 = builder.TestCreateFailureCollector();

        collector1.ShouldNotBeSameAs(collector2);
        collector1.ShouldBeOfType<FailuresDictionary>();
    }

    [Fact]
    public void Require_ReferenceType_WithNull_ShouldThrow()
    {
        var builder = new OrderBuilder();
        
        Should.Throw<RequiredPropertyNotSetException>(() => builder.Build());
    }

    [Fact]
    public void Require_ValueType_WithNull_ShouldThrow()
    {
        var builder = new OrderBuilder().WithCustomerId("C1");
        
        // Missing Quantity and Price (both nullable value types)
        Should.Throw<RequiredPropertyNotSetException>(() => builder.Build());
    }

    [Fact]
    public void Require_AllSet_ShouldSucceed()
    {
        var builder = new OrderBuilder()
            .WithCustomerId("C1")
            .WithQuantity(5)
            .WithPrice(99.99m);

        var result = builder.Build();

        result.IsSuccess.ShouldBeTrue();
        var order = result.Value.Resolved();
        order.CustomerId.ShouldBe("C1");
        order.Quantity.ShouldBe(5);
        order.Price.ShouldBe(99.99m);
    }
}

