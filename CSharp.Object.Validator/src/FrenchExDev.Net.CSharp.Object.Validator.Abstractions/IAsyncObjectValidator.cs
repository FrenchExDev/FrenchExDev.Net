namespace FrenchExDev.Net.CSharp.Object.Validator.Abstractions;

/// <summary>
/// Defines a contract for asynchronously validating an instance of a specified type.
/// </summary>
/// <typeparam name="TClass">The type of the object to validate.</typeparam>
public interface IAsyncObjectValidator<TClass> : IAbstractObjectValidator
{
    /// <summary>
    /// Asynchronously validates the specified instance and returns a collection of validation results.
    /// </summary>
    /// <param name="instance">The instance of type <typeparamref name="TClass"/> to validate. Cannot be <see langword="null"/>.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None"/>.</param>
    /// <returns>A task that represents the asynchronous validation operation. The task result contains a collection of  <see
    /// cref="IAbstractObjectValidator"/> objects representing the validation results. The collection will be empty if no validation
    /// errors are found.</returns>
    Task<IEnumerable<IObjectValidation>> ValidateAsync(TClass instance, CancellationToken cancellationToken = default);
}
