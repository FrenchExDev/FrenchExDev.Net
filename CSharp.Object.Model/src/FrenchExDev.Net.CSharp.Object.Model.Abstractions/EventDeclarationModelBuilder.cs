using FrenchExDev.Net.CSharp.Object.Builder2;

namespace FrenchExDev.Net.CSharp.Object.Model.Abstractions;

/// <summary>
/// Builder class for constructing <see cref="EventDeclarationModel"/> instances.
/// Provides a fluent interface to set event modifiers, type, name, and attributes.
/// Ensures required properties are set and validates attribute builders.
/// </summary>
public class EventDeclarationModelBuilder : AbstractBuilder<EventDeclarationModel>, IDeclarationModelBuilder
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
    private readonly List<string> _modifiers = [];

    /// <summary>
    /// Stores the list of attribute builders for the event.
    /// </summary>
    private readonly BuilderList<AttributeDeclarationModel, AttributeDeclarationModelBuilder> _attributes = [];

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
    /// Builds the internal representation of the object's attributes using the specified visited object dictionary.
    /// </summary>
    /// <param name="visitedCollector">A dictionary used to track objects that have already been visited during the build process. This helps prevent
    /// processing the same object multiple times.</param>
    protected override void BuildInternal(VisitedObjectDictionary visitedCollector)
    {
        BuildList(_attributes, visitedCollector);
    }

    /// <summary>
    /// Performs validation on the current event object and records any validation failures encountered.
    /// </summary>
    /// <remarks>This method checks that the event name and event type are provided, and validates each
    /// attribute associated with the event. Any validation errors are recorded in the <paramref name="failures"/>
    /// dictionary. This method is intended to be called by the validation framework and is not typically invoked
    /// directly.</remarks>
    /// <param name="visitedCollector">A dictionary used to track objects that have already been visited during validation to prevent redundant checks
    /// and circular references.</param>
    /// <param name="failures">A dictionary for collecting validation failures found during the validation process. Failures are added to this
    /// dictionary if required event properties are missing or if attribute validation fails.</param>
    protected new void ValidateInternal(VisitedObjectDictionary visitedCollector, FailuresDictionary failures)
    {
        // Validate that the event name is provided
        if (string.IsNullOrEmpty(_name))
        {
            failures.Failure(nameof(_name), new InvalidOperationException("Event name must be provided."));
        }

        // Validate that the event type is provided
        if (string.IsNullOrEmpty(_type))
        {
            failures.Failure(nameof(_type), new InvalidOperationException("Event type must be provided."));
        }

        foreach (var attr in _attributes)
        {
            var attrFailures = new FailuresDictionary();
            attr.Validate(visitedCollector, attrFailures);
            if (attrFailures.Count > 0)
            {
                failures.Failure(nameof(_attributes), attrFailures);
            }
        }
    }

    /// <summary>
    /// Creates a new instance of the event declaration model using the current builder state.
    /// </summary>
    /// <remarks>Throws an exception if required fields are not set prior to instantiation. This method is
    /// typically called after all necessary properties have been configured.</remarks>
    /// <returns>An <see cref="EventDeclarationModel"/> representing the event declaration configured by the builder.</returns>
    protected override EventDeclarationModel Instantiate()
    {
        ArgumentNullException.ThrowIfNull(_name);
        ArgumentNullException.ThrowIfNull(_type);

        return new(_modifiers, _type, _name, _attributes.AsReferenceList());
    }
}
