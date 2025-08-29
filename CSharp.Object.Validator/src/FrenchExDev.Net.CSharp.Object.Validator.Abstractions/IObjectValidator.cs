namespace FrenchExDev.Net.CSharp.Object.Validator.Abstractions;

/// <summary>
/// Validator for objects of type <typeparamref name="TClass"/>
/// </summary>
/// <typeparam name="TClass"></typeparam>
public interface IObjectValidator<TClass>
{
    /// <summary>
    /// Validates the specified instance and returns a collection of validation results.
    /// </summary>
    /// <param name="instance">The instance of type <typeparamref name="TClass"/> to validate. Cannot be null.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="IValidation"/> objects representing the validation results. The
    /// collection will be empty if the instance is valid.</returns>
    IEnumerable<IValidation> Validate(TClass instance);
}
