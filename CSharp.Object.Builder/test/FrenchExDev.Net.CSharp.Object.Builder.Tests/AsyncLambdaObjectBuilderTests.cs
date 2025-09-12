using FrenchExDev.Net.CSharp.Object.Builder.Abstractions;
using FrenchExDev.Net.CSharp.Object.Builder.Testing;
using FrenchExDev.Net.CSharp.Object.Builder.Tests.Fixtures;
using Shouldly;

namespace FrenchExDev.Net.CSharp.Object.Builder.Tests;

/// <summary>
/// Contains unit tests for verifying the behavior of asynchronous object builders, such as <see cref="PersonBuilder"/>
/// and <see cref="AddressBuilder"/>.
/// </summary>
/// <remarks>This class includes tests to ensure that asynchronous object builders correctly handle various
/// scenarios, such as invalid input, incomplete configurations, and expected failure cases. The tests validate that the
/// builders produce the appropriate results, including success and failure outcomes, and that they handle exceptions
/// and error messages as expected.</remarks>
public class AsyncLambdaObjectBuilderTests
{
    #region Builder test classes

    /// <summary>
    /// Provides a builder for creating instances of the <see cref="Person"/> class.
    /// </summary>
    /// <remarks>This class is used to construct <see cref="Person"/> objects asynchronously using a specified
    /// build function. It extends the <see cref="LambdaAsyncObjectBuilder{TObject, TBuilder}"/> base class, enabling
    /// customizable object creation logic with support for asynchronous operations.</remarks>
    internal class PersonBuilder : LambdaAsyncObjectBuilder<Person, PersonBuilder>
    {
        public const string ErrorInvalidAge = "Invalid age";

        public PersonBuilder(Func<PersonBuilder, ExceptionBuildDictionary, VisitedObjectsList, CancellationToken, Task<IObjectBuildResult<Person>>> buildFunc) : base(buildFunc)
        {
        }
    }

    /// <summary>
    /// Provides a builder for creating instances of the <see cref="Address"/> class.
    /// </summary>
    /// <remarks>This class is used to construct <see cref="Address"/> objects using a customizable build
    /// function.  It extends the <see cref="LambdaObjectBuilder{TObject, TBuilder}"/> class, allowing for flexible 
    /// object creation patterns.</remarks>
    internal class AddressBuilder : LambdaAsyncObjectBuilder<Address, AddressBuilder>
    {
        public AddressBuilder(Func<AddressBuilder, ExceptionBuildDictionary, VisitedObjectsList, CancellationToken, Task<IObjectBuildResult<Address>>> buildFunc) : base(buildFunc)
        {
        }
    }

    #endregion

    /// <summary>
    /// Tests that attempting to build a <see cref="Person"/> using an incomplete <see cref="PersonBuilder"/>  results
    /// in a failure with the appropriate error message.
    /// </summary>
    /// <remarks>This test verifies that the <see cref="PersonBuilder"/> correctly identifies invalid input 
    /// (e.g., missing or invalid age) and produces a <see cref="FailureObjectBuildResult{T, TBuilder}"/>  containing
    /// the expected exception and error details.</remarks>
    [Fact]
    public async Task Cannot_Build_Incomplete_Person_Async()
    {
        await BuilderTester.InvalidAsync<PersonBuilder, Person>(
            builderFactory: () => new PersonBuilder((builder, exceptions, visited, cancellationToken) =>
            {
                exceptions.Add("age", new Exception(PersonBuilder.ErrorInvalidAge));
                return Task.FromResult<IObjectBuildResult<Person>>(new FailureAsyncObjectBuildResult<Person, PersonBuilder>(builder, exceptions, visited));
            }),
            body: (builder, cancellationToken) =>
            {
                return Task.CompletedTask;
            },
            assert: (failure) =>
            {
                failure.ShouldNotBeNull();
                failure.Exceptions.Count().ShouldBeEquivalentTo(1);
                failure.Exceptions["age"].ElementAt(0).Message.ShouldBe(PersonBuilder.ErrorInvalidAge);
                failure.Builder.ShouldNotBeNull();
                failure.Builder.ShouldBeAssignableTo<PersonBuilder>();
            });
    }
}