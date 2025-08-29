using FrenchExDev.Net.CSharp.Object.Builder.Abstractions;
using Shouldly;

namespace FrenchExDev.Net.CSharp.Object.Builder.Testing;

/// <summary>
/// Provides utility methods for testing builder implementations, including validation of valid and invalid build
/// scenarios.
/// </summary>
/// <remarks>This class contains methods to test both synchronous and asynchronous builders. It ensures that
/// builders conform to expected behaviors for valid and invalid build scenarios. The methods support custom assertions
/// and allow for cancellation in asynchronous operations.</remarks>
public static class BuilderTester
{
    /// <summary>
    /// Test a builder for a valid build scenario.
    /// </summary>
    /// <typeparam name="TBuilder">The type of the builder.</typeparam>
    /// <typeparam name="TClass">The type of the class being built.</typeparam>
    /// <param name="builderFactory">A factory method to create the builder.</param>
    /// <param name="body">A test action to perform on the builder.</param>
    public static void TestValid<TBuilder, TClass>
        (
            Func<TBuilder> builderFactory,
            Action<TBuilder> body
        ) where TBuilder : IBuilder<TClass>
    {
        var builder = builderFactory();
        body(builder);
        TestValidInternal<TBuilder, TClass>(builder);
    }

    /// <summary>
    /// Executes an asynchronous operation using a builder and validates the result.
    /// </summary>
    /// <remarks>This method creates a builder using the specified <paramref name="builderFactory"/>, executes
    /// the provided <paramref name="body"/> operation asynchronously, and then validates the builder's state. The
    /// validation is performed using an internal mechanism.</remarks>
    /// <typeparam name="TBuilder">The type of the builder used to construct the object.</typeparam>
    /// <typeparam name="TClass">The type of the object being built and validated.</typeparam>
    /// <param name="builderFactory">A function that creates an instance of <typeparamref name="TBuilder"/>.</param>
    /// <param name="body">An asynchronous operation that performs actions on the builder.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public static async Task TestValidAsync<TBuilder, TClass>
        (
            Func<TBuilder> builderFactory,
            Func<TBuilder, CancellationToken, Task> body,
            CancellationToken cancellationToken = default
        ) where TBuilder : IBuilder<TClass>
    {
        var builder = builderFactory();
        await body(builder, cancellationToken);
        TestValidInternal<TBuilder, TClass>(builder);
    }

    /// <summary>
    /// Tests the behavior of a builder implementation when constructing an invalid object.
    /// </summary>
    /// <remarks>This method is intended to test scenarios where the builder produces an invalid object.  The
    /// <paramref name="body"/> parameter should configure the builder to create an invalid state,  and the optional
    /// <paramref name="assert"/> parameter can be used to verify the expected behavior  or state of the build
    /// result.</remarks>
    /// <typeparam name="TBuilder">The type of the builder used to construct the object. Must implement <see cref="IBuilder{TClass}"/>.</typeparam>
    /// <typeparam name="TClass">The type of the object being constructed by the builder.</typeparam>
    /// <param name="builderFactory">A function that creates an instance of the builder.</param>
    /// <param name="body">An action that configures the builder to produce an invalid object.</param>
    /// <param name="assert">An optional action to validate the result of the build operation. If provided, it is invoked with the build
    /// result.</param>
    public static void TestInvalid<TBuilder, TClass>
        (
            Func<TBuilder> builderFactory,
            Action<TBuilder> body,
            Action<IBuildResult<TClass>>? assert = null
        ) where TBuilder : IBuilder<TClass>
    {
        var builder = builderFactory();
        body(builder);
        var built = builder.Build();
        InternalTestInvalid(assert, built);
    }

    /// <summary>
    /// Tests the behavior of an asynchronous builder by executing a specified body function and optionally asserting
    /// the result.
    /// </summary>
    /// <remarks>This method is designed to facilitate testing of asynchronous builders by allowing the caller
    /// to define a custom body function and optional assertions on the builder's result. The builder is created using
    /// the provided factory function, and the body function is executed before the builder's result is built and
    /// validated.</remarks>
    /// <typeparam name="TBuilder">The type of the builder, which must implement <see cref="IAsyncBuilder{TClass}"/>.</typeparam>
    /// <typeparam name="TClass">The type of the object being built by the builder.</typeparam>
    /// <param name="builderFactory">A factory function that creates an instance of <typeparamref name="TBuilder"/>.</param>
    /// <param name="body">An asynchronous function that operates on the builder. This function is executed before the builder's result is
    /// validated.</param>
    /// <param name="assert">An optional action to assert the result of the builder. If provided, it is invoked with the result of the build
    /// operation.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests. The token is passed to the body function and the build operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public static async Task TestInvalidAsync<TBuilder, TClass>
        (
            Func<TBuilder> builderFactory,
            Func<TBuilder, CancellationToken, Task> body,
            Action<IBuildResult<TClass>>? assert = null,
            CancellationToken cancellationToken = default
        ) where TBuilder : IAsyncBuilder<TClass>
    {
        var builder = builderFactory();
        await body(builder, cancellationToken);
        var built = await builder.BuildAsync(cancellationToken: cancellationToken);
        InternalTestInvalid(assert, built);
    }

    /// <summary>
    /// Tests the validity of a builder by verifying that the built object meets expected conditions.
    /// </summary>
    /// <remarks>This method ensures that the builder successfully creates an object of the expected type, 
    /// that the object is marked as built, and that no exceptions are present in the builder's state.</remarks>
    /// <typeparam name="TBuilder">The type of the builder, which must implement <see cref="IBuilder{TClass}"/>.</typeparam>
    /// <typeparam name="TClass">The type of the object being built by the builder.</typeparam>
    /// <param name="builder">The builder instance to validate. Must not be <c>null</c>.</param>
    public static void TestValidInternal<TBuilder, TClass>(TBuilder builder) where TBuilder : IBuilder<TClass>
    {
        var built = builder.Build();
        built.ShouldBeAssignableTo<SuccessResult<TClass>>();
    }

    /// <summary>
    /// Verifies that the specified build result is invalid and optionally applies an assertion.
    /// </summary>
    /// <remarks>This method ensures that the build result is in an invalid state by checking that the object
    /// was not built, the result is <see langword="null"/>, and the exceptions collection is not empty. If an assertion
    /// is provided, it is invoked with the build result.</remarks>
    /// <typeparam name="TClass">The type of the object being built.</typeparam>
    /// <param name="assert">An optional assertion to apply to the build result. Can be <see langword="null"/>.</param>
    /// <param name="built">The build result to validate. Must not be <see langword="null"/>.</param>
    private static void InternalTestInvalid<TClass>(Action<IBuildResult<TClass>>? assert, IBuildResult<TClass> built)
    {
        built.ShouldBeAssignableTo<FailureResult<TClass, IBuilder<TClass>>>();
        var failureResult = (FailureResult<TClass, IBuilder<TClass>>)built;
        failureResult.Exceptions.ShouldNotBeEmpty();
        failureResult.Exceptions.Count().ShouldBeGreaterThan(0);

        if (assert is not null)
        {
            assert(built);
        }
    }
}


