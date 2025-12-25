namespace FrenchExDev.Net.CSharp.Object.Builder2;

/// <summary>
/// Defines a contract for objects that have a unique identifier.
/// </summary>
public interface IIdentifiable
{
    Guid Id { get; }
}
