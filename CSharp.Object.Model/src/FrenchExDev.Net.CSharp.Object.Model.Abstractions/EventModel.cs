namespace FrenchExDev.Net.CSharp.Object.Model.Abstractions;

/// <summary>
/// Represents an event declaration, including its modifiers, type, name, and attributes.
/// </summary>
public class EventModel : IDeclarationModel
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
    public List<AttributeDeclarationModel> Attributes { get; set; } = new();
}
