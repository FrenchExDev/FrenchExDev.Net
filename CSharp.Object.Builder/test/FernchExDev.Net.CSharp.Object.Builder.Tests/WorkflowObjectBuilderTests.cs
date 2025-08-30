using FernchExDev.Net.CSharp.Object.Builder.Tests.Fixtures;
using FrenchExDev.Net.CSharp.Object.Builder;
using FrenchExDev.Net.CSharp.Object.Builder.Abstractions;
using Shouldly;

namespace FernchExDev.Net.CSharp.Object.Builder.Tests;

[Trait("unit", "virtual")]
public class WorkflowObjectBuilderTests
{
    internal sealed class PersonObjectBuilder : WorkflowObjectBuilder<Person>
    {
        public PersonObjectBuilder()
        {
            Step(new LambdaStepObjectBuilder<Person>(false, (step, intermediates, cancellationToken) =>
            {
                intermediates["Name"] = "John Doe";
                intermediates["Age"] = 30;
                intermediates["Addresses"] = new List<Address> { new Address("123 Main St", "12345") };
            }))
            .Step(new LambdaStepObjectBuilder<Person>(true, (step, intermediates, cancellationToken) =>
            {
                var person = new Person(
                    name: (string)intermediates["Name"],
                    age: (int)intermediates["Age"],
                    addresses: (IEnumerable<Address>)intermediates["Addresses"]
                );

                step.Set(person);
            }));
        }
    }

    [Fact]
    public async Task Can_Build_Complete_Person()
    {
        // Arrange
        var builder = new PersonObjectBuilder();

        // Act
        IObjectBuildResult<Person> result = await builder.BuildAsync();

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBeAssignableTo<SuccessObjectBuildResult<Person>>();

        var personResult = (SuccessObjectBuildResult<Person>)result;
        personResult.ShouldNotBeNull();

        var person = personResult.Result;

        person.ShouldNotBeNull();
        person.Name.ShouldBe("John Doe");
        person.Age.ShouldBe(30);
        person.Addresses.ShouldNotBeNull();
        person.Addresses.ShouldHaveSingleItem();
        person.Addresses.First().Street.ShouldBe("123 Main St");
        person.Addresses.First().ZipCode.ShouldBe("12345");
    }
}
