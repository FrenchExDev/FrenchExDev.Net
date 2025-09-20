using FrenchExDev.Net.CSharp.Object.Builder2;

namespace FrenchExDev.Net.CSharp.Object.Model.Abstractions;

/// <summary>
/// Represents an event declaration, including its modifiers, type, name, and attributes.
/// </summary>
public class EventDeclarationModel : IDeclarationModel
{
    /// <summary>
    /// Gets or sets the list of modifiers applied to the event (e.g., public, static).
    /// </summary>
    public List<string> Modifiers { get; set; } = new();

    /// <summary>
    /// Gets or sets the type of the event handler (e.g., EventHandler, Action).
    /// </summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the name of the event.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the list of attributes applied to the event.
    /// </summary>
    private readonly ReferenceList<AttributeDeclarationModel> _attributes = new();

    public EventDeclarationModel(List<string> modifiers, string type, string name, ReferenceList<AttributeDeclarationModel> attributes)
    {
        Modifiers = modifiers;
        Type = type;
        Name = name;
        _attributes = attributes;
    }

    public IEnumerable<AttributeDeclarationModel> Attributes => _attributes.AsEnumerable();
}
