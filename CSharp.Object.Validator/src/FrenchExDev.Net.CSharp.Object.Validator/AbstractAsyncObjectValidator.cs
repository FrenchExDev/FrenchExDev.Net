using FrenchExDev.Net.CSharp.Object.Validator.Abstractions;

namespace FrenchExDev.Net.CSharp.Object.Validator;

/// <summary>
/// Provides an abstract base class for asynchronously validating objects of type <typeparamref name="TClass"/>.
/// </summary>
/// <remarks>Implementations of this class must define the <see cref="ValidateAsync"/> method to perform
/// validation logic for instances of <typeparamref name="TClass"/>. This class is intended to be used as a base for
/// creating custom asynchronous validators.</remarks>
/// <typeparam name="TClass">The type of object to validate.</typeparam>
public abstract class AbstractAsyncObjectValidator<TClass> : IAsyncValidator<TClass>
{
    /// <summary>
    /// Asynchronously validates the specified instance and returns a collection of validation results.
    /// </summary>
    /// <param name="instance">The instance of type <typeparamref name="TClass"/> to validate. Cannot be <see langword="null"/>.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None"/>.</param>
    /// <returns>A task that represents the asynchronous validation operation. The task result contains a collection of  <see
    /// cref="IValidation"/> objects representing the validation results. The collection will be empty if no validation
    /// errors are found.</returns>
    public async Task<IEnumerable<IValidation>> ValidateAsync(TClass instance, CancellationToken cancellationToken = default)
    {
        var list = new List<IValidation>();
        await ValidateInternalAsync(instance, list, cancellationToken);
        return list;
    }
    
    /// <summary>
    /// Validates the specified instance and adds any validation results to the provided collection.
    /// </summary>
    /// <remarks>This method is intended to be implemented by derived classes to define custom validation
    /// logic.</remarks>
    /// <param name="instance">The instance of type <typeparamref name="TClass"/> to validate.</param>
    /// <param name="validations">A collection to which validation results will be added. Cannot be null.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None"/>.</param>
    /// <returns>A task that represents the asynchronous validation operation.</returns>
    protected abstract Task ValidateInternalAsync(TClass instance, List<IValidation> validations, CancellationToken cancellationToken = default);
}