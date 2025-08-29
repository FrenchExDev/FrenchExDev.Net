using FrenchExDev.Net.CSharp.Object.Validator.Abstractions;

namespace FrenchExDev.Net.CSharp.Object.Validator;

/// <summary>
/// Provides a mechanism to validate objects of type <typeparamref name="TClass"/> using a user-defined validation
/// function.
/// </summary>
/// <remarks>This class allows the caller to supply a custom validation function via the <see
/// cref="ValidatorFunc"/> property. The validation function is invoked when the <see cref="Validate"/> method is
/// called, and it is expected to return a collection of validation results.</remarks>
/// <typeparam name="TClass">The type of object to validate.</typeparam>
public class LambdaObjectValidator<TClass> : IObjectValidator<TClass>
{
    /// <summary>
    /// Gets the function used to validate an instance of <typeparamref name="TClass"/>.
    /// </summary>
    public Func<TClass, IEnumerable<IValidation>> ValidatorFunc { get; init; }

    /// <summary>
    /// Initializes a new instance of the <see cref="LambdaObjectValidator{TClass}"/> class with the specified
    /// validation function.
    /// </summary>
    /// <remarks>The <paramref name="validatorFunc"/> parameter is used to define the validation logic for
    /// objects of type <typeparamref name="TClass"/>. The function should return an <see cref="IEnumerable{T}"/> of
    /// <see cref="IValidation"/> representing the validation results.</remarks>
    /// <param name="validatorFunc">A function that takes an instance of <typeparamref name="TClass"/> and returns a collection of validation
    /// results.</param>
    public LambdaObjectValidator(Func<TClass, IEnumerable<IValidation>> validatorFunc)
    {
        ValidatorFunc = validatorFunc;
    }

    /// <summary>
    /// Validates the specified instance and returns a collection of validation results.
    /// </summary>
    /// <param name="instance">The instance of type <typeparamref name="TClass"/> to validate.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="IValidation"/> objects representing the validation results. The
    /// collection will be empty if the instance passes all validations.</returns>
    public IEnumerable<IValidation> Validate(TClass instance)
    {
        return ValidatorFunc(instance);
    }
}
