using FrenchExDev.Net.CSharp.Object.Builder;
using FrenchExDev.Net.CSharp.Object.Builder.Abstractions;

namespace FrenchExDev.Net.CSharp.Object.Model.Abstractions;

/// <summary>
/// Builder class for constructing <see cref="EventModel"/> instances.
/// Provides a fluent interface to set event modifiers, type, name, and attributes.
/// Ensures required properties are set and validates attribute builders.
/// </summary>
public class EventDeclarationModelBuilder : AbstractObjectBuilder<EventModel, EventDeclarationModelBuilder>
{
    /// <summary>
    /// Stores the type of the event handler (e.g., EventHandler, Action).
    /// </summary>
    private string? _type;
    /// <summary>
    /// Stores the name of the event.
    /// </summary>
    private string? _name;
    /// <summary>
    /// Stores the list of modifiers applied to the event (e.g., public, static).
    /// </summary>
    private readonly List<string> _modifiers = new();
    /// <summary>
    /// Stores the list of attribute builders for the event.
    /// </summary>
    private readonly List<AttributeDeclarationModelBuilder> _attributes = new();

    /// <summary>
    /// Adds a modifier to the event (e.g., public, static).
    /// </summary>
    /// <param name="modifier">The modifier to add.</param>
    /// <returns>The current builder instance.</returns>
    public EventDeclarationModelBuilder Modifier(string modifier)
    {
        _modifiers.Add(modifier);
        return this;
    }

    /// <summary>
    /// Sets the type of the event handler.
    /// </summary>
    /// <param name="type">The event handler type.</param>
    /// <returns>The current builder instance.</returns>
    public EventDeclarationModelBuilder Type(string type)
    {
        _type = type;
        return this;
    }

    /// <summary>
    /// Sets the name of the event.
    /// </summary>
    /// <param name="name">The event name.</param>
    /// <returns>The current builder instance.</returns>
    public EventDeclarationModelBuilder Name(string name)
    {
        _name = name;
        return this;
    }

    /// <summary>
    /// Adds an attribute to the event using a builder action.
    /// </summary>
    /// <param name="attribute">An action to configure the attribute builder.</param>
    /// <returns>The current builder instance.</returns>
    public EventDeclarationModelBuilder Attribute(Action<AttributeDeclarationModelBuilder> attribute)
    {
        var builder = new AttributeDeclarationModelBuilder();
        attribute(builder);
        _attributes.Add(builder);
        return this;
    }

    /// <summary>
    /// Builds the <see cref="EventModel"/> instance, validating required properties and attributes.
    /// </summary>
    /// <param name="exceptions">A list to collect build exceptions.</param>
    /// <param name="visited">A list of visited objects for cycle detection.</param>
    /// <returns>A build result containing either the constructed model or failure details.</returns>
    protected override IObjectBuildResult<EventModel> BuildInternal(ExceptionBuildDictionary exceptions, VisitedObjectsList visited)
    {
        // Build all attributes and collect their results
        var attributes = BuildBuildList<AttributeDeclarationModel, AttributeDeclarationModelBuilder>(_attributes, visited);
        AddExceptions<AttributeDeclarationModel, AttributeDeclarationModelBuilder>(nameof(_attributes), attributes, exceptions);

        // Validate that the event name is provided
        if (string.IsNullOrEmpty(_name))
        {
            exceptions.Add(nameof(_name), new InvalidOperationException("Event name must be provided."));
        }

        // Validate that the event type is provided
        if (string.IsNullOrEmpty(_type))
        {
            exceptions.Add(nameof(_type), new InvalidOperationException("Event type must be provided."));
        }

        // If there are any exceptions, return a failure result
        if (exceptions.Any())
        {
            return Failure(exceptions, visited);
        }

        // Ensure required fields are not null
        ArgumentNullException.ThrowIfNull(_name);
        ArgumentNullException.ThrowIfNull(_type);

        // Return a successful build result with the constructed EventModel
        return Success(new EventModel
        {
            Modifiers = _modifiers,
            Type = _type,
            Name = _name,
            Attributes = attributes.ToResultList()
        });
    }
}
