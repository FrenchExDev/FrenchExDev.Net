using FrenchExDev.Net.CSharp.Object.Validator.Abstractions;

namespace FrenchExDev.Net.CSharp.Object.Validator;

/// <summary>
/// Async version of <see cref="LambdaObjectValidator{TClass}"/>
/// </summary>
/// <typeparam name="TClass"></typeparam>
public class AsyncLambdaObjectValidator<TClass> : IAsyncValidator<TClass>
{
    /// <summary>
    /// Reference to the function that performs the validation
    /// </summary>
    public Func<TClass, CancellationToken, Task<IEnumerable<IValidation>>> ValidatorFunc { get; init; }

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncLambdaObjectValidator{TClass}"/> class with the specified
    /// asynchronous validation function.
    /// </summary>
    /// <remarks>The provided <paramref name="validatorFunc"/> is expected to handle the validation logic for
    /// objects of type <typeparamref name="TClass"/>. The returned collection of <see cref="IValidation"/> objects
    /// should represent the validation results, such as errors or warnings.</remarks>
    /// <param name="validatorFunc">A function that performs asynchronous validation on an object of type <typeparamref name="TClass"/>. The
    /// function takes an instance of <typeparamref name="TClass"/> and a <see cref="CancellationToken"/> as input, and
    /// returns a task that produces a collection of validation results.</param>
    public AsyncLambdaObjectValidator(Func<TClass, CancellationToken, Task<IEnumerable<IValidation>>> validatorFunc)
    {
        ArgumentNullException.ThrowIfNull(validatorFunc);
        ValidatorFunc = validatorFunc;
    }

    /// <summary>
    /// Asynchronously validates the specified instance and returns a collection of validation results.
    /// </summary>
    /// <param name="instance">The instance of type <typeparamref name="TClass"/> to validate. Cannot be <see langword="null"/>.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None"/>.</param>
    /// <returns>A task that represents the asynchronous validation operation. The task result contains an <see
    /// cref="IEnumerable{T}"/> of <see cref="IValidation"/> objects representing the validation results. The collection
    /// will be empty if no validation errors are found.</returns>
    public Task<IEnumerable<IValidation>> ValidateAsync(TClass instance, CancellationToken cancellationToken = default)
    {
        return ValidatorFunc(instance, cancellationToken);
    }
}
