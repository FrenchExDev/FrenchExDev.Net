using FrenchExDev.Net.CSharp.Object.Validator.Abstractions;

namespace FrenchExDev.Net.CSharp.Object.Validator;

/// <summary>
/// Abstract base class for creating object validators for type <typeparamref name="TClass"/>.
/// </summary>
/// <typeparam name="TClass"></typeparam>
public abstract class AbstractObjectValidator<TClass> : IObjectValidator<TClass>
{
    /// <summary>
    /// Validates the specified instance and returns a collection of validation results.
    /// </summary>
    /// <param name="instance"></param>
    /// <returns></returns>
    public IEnumerable<IValidation> Validate(TClass instance)
    {
        var result = new List<IValidation>();
        ValidateInternal(instance, result);
        return result;

    }

    /// <summary>
    /// Internal method to perform the actual validation logic.
    /// </summary>
    /// <param name="instance"></param>
    /// <param name="validations"></param>
    protected abstract void ValidateInternal(TClass instance, List<IValidation> validations);
}
