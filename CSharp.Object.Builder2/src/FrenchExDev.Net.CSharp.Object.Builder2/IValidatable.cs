namespace FrenchExDev.Net.CSharp.Object.Builder2;

/// <summary>
/// Defines a contract for objects that can be validated.
/// </summary>
public interface IValidatable
{
    /// <summary>
    /// Gets the current validation status of the object.
    /// </summary>
    ValidationStatus ValidationStatus { get; }

    /// <summary>
    /// Validates the current object and records any validation failures encountered.
    /// </summary>
    /// <param name="visitedCollector">A dictionary used to track objects that have already been visited during validation to prevent redundant checks
    /// and handle circular references.</param>
    /// <param name="failures">An object that collects validation failures found during the validation process. All detected issues are added
    /// to this collector.</param>
    void Validate(VisitedObjectDictionary visitedCollector, IFailureCollector failures);
}
