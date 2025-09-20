namespace FrenchExDev.Net.CSharp.Object.Model.Abstractions;

/// <summary>
/// Represents a type parameter declaration within a generic type or method definition.
/// </summary>
/// <remarks>Use this class to model and access metadata about type parameters, such as their names, when
/// analyzing or generating code that involves generics.</remarks>
public class TypeParameterDeclarationModel : IDeclarationModel
{
    /// <summary>
    /// Initializes a new instance of the TypeParameterDeclarationModel class with the specified type parameter name.
    /// </summary>
    /// <param name="name">The name of the type parameter to be represented. Cannot be null or empty.</param>
    public TypeParameterDeclarationModel(string name)
    {
        Name = name;
    }

    /// <summary>
    /// Gets or sets the name associated with the object.
    /// </summary>
    public string Name { get; set; } = string.Empty;
}
