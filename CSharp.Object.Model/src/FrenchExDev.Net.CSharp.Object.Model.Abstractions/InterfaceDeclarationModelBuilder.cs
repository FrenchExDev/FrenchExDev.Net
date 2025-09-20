using FrenchExDev.Net.CSharp.Object.Builder2;

namespace FrenchExDev.Net.CSharp.Object.Model.Abstractions;

/// <summary>
/// Builder for constructing <see cref="InterfaceDeclarationModel"/> instances.
/// Provides a fluent API to configure interface name, modifiers, attributes, base interfaces,
/// type parameters, constraints, properties, methods, events, and nested interfaces.
/// Validates required properties and produces a build result indicating success or failure.
/// </summary>
public class InterfaceDeclarationModelBuilder : AbstractBuilder<InterfaceDeclarationModel>, IDeclarationModelBuilder
{
    /// <summary>
    /// Stores the name of the interface to be built.
    /// <remarks>Should be set before building to ensure a valid model.</remarks>
    /// </summary>
    private string? _name;

    /// <summary>
    /// List of access modifiers applied to the interface (e.g., public, internal).
    /// <remarks>Defines the visibility and accessibility of the interface.</remarks>
    /// </summary>
    private readonly List<InterfaceModifier> _modifiers = [];

    /// <summary>
    /// List of attribute builders applied to the interface.
    /// <remarks>Use <see cref="Attribute"/> to add custom attributes.</remarks>
    /// </summary>
    private readonly BuilderList<AttributeDeclarationModel, AttributeDeclarationModelBuilder> _attributes = [];

    /// <summary>
    /// List of base interfaces that this interface inherits from.
    /// <remarks>Supports modeling multiple interface inheritance.</remarks>
    /// </summary>
    private readonly BuilderList<InterfaceDeclarationModel, InterfaceDeclarationModelBuilder> _baseInterfaces = [];

    /// <summary>
    /// List of generic type parameters declared by the interface.
    /// <remarks>Useful for generic interfaces.</remarks>
    /// </summary>
    private readonly BuilderList<TypeParameterDeclarationModel, TypeParameterDeclarationModelBuilder> _typeParameters = [];

    /// <summary>
    /// List of constraints applied to type parameters.
    /// <remarks>Restricts the types usable in generic parameters.</remarks>
    /// </summary>
    private readonly BuilderList<TypeParameterConstraintModel, TypeParameterConstraintModelBuilder> _typeParameterConstraints = [];

    /// <summary>
    /// List of property builders for properties declared in the interface.
    /// <remarks>Each property should be configured with its type, name, and accessors.</remarks>
    /// </summary>
    private readonly BuilderList<PropertyDeclarationModel, PropertyDeclarationModelBuilder> _properties = [];

    /// <summary>
    /// List of method builders for methods declared in the interface.
    /// <remarks>Methods should be configured with their name, return type, and parameters.</remarks>
    /// </summary>
    private readonly BuilderList<MethodDeclarationModel, MethodDeclarationModelBuilder> _methods = [];

    /// <summary>
    /// List of event builders for events declared in the interface.
    /// <remarks>Events should specify their type and name.</remarks>
    /// </summary>
    private readonly BuilderList<EventDeclarationModel, EventDeclarationModelBuilder> _events = [];

    /// <summary>
    /// List of nested interface builders for interfaces declared within this interface.
    /// <remarks>Supports complex interface hierarchies.</remarks>
    /// </summary>
    private readonly BuilderList<InterfaceDeclarationModel, InterfaceDeclarationModelBuilder> _nestedInterfaces = [];

    /// <summary>
    /// Sets the name of the interface.
    /// </summary>
    /// <param name="name">The interface name.</param>
    /// <returns>The current builder instance.</returns>
    public InterfaceDeclarationModelBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    /// <summary>
    /// Adds an access modifier to the interface (e.g., public, internal).
    /// </summary>
    /// <param name="modifier">The modifier to add.</param>
    /// <returns>The current builder instance.</returns>
    public InterfaceDeclarationModelBuilder WithModifier(InterfaceModifier modifier)
    {
        _modifiers.Add(modifier);
        return this;
    }

    /// <summary>
    /// Adds an attribute to the interface using a builder action.
    /// </summary>
    /// <param name="attribute">An action to configure the attribute builder.</param>
    /// <returns>The current builder instance.</returns>
    public InterfaceDeclarationModelBuilder Attribute(Action<AttributeDeclarationModelBuilder> attribute)
    {
        var builder = new AttributeDeclarationModelBuilder();
        attribute(builder);
        _attributes.Add(builder);
        return this;
    }

    /// <summary>
    /// Adds a base interface to the interface.
    /// </summary>
    /// <param name="baseInterface">The builder for the base interface.</param>
    /// <returns>The current builder instance.</returns>
    /// <remarks>Supports multiple inheritance for interfaces.</remarks>
    public InterfaceDeclarationModelBuilder BaseInterface(InterfaceDeclarationModelBuilder baseInterface)
    {
        _baseInterfaces.Add(baseInterface);
        return this;
    }

    /// <summary>
    /// Adds a generic type parameter to the interface using a builder action.
    /// </summary>
    /// <param name="typeParameter">An action to configure the type parameter builder.</param>
    /// <returns>The current builder instance.</returns>
    public InterfaceDeclarationModelBuilder TypeParameter(Action<TypeParameterDeclarationModelBuilder> typeParameter)
    {
        var builder = new TypeParameterDeclarationModelBuilder();
        typeParameter(builder);
        _typeParameters.Add(builder);
        return this;
    }

    /// <summary>
    /// Adds a type parameter constraint to the interface using a builder action.
    /// </summary>
    /// <param name="typeParameterConstraint">An action to configure the constraint builder.</param>
    /// <returns>The current builder instance.</returns>
    public InterfaceDeclarationModelBuilder TypeParameterConstraint(Action<TypeParameterConstraintModelBuilder> typeParameterConstraint)
    {
        var builder = new TypeParameterConstraintModelBuilder();
        typeParameterConstraint(builder);
        _typeParameterConstraints.Add(builder);
        return this;
    }

    /// <summary>
    /// Adds a property to the interface using a builder action.
    /// </summary>
    /// <param name="property">An action to configure the property builder.</param>
    /// <returns>The current builder instance.</returns>
    public InterfaceDeclarationModelBuilder Property(Action<PropertyDeclarationModelBuilder> property)
    {
        var builder = new PropertyDeclarationModelBuilder();
        property(builder);
        _properties.Add(builder);
        return this;
    }

    /// <summary>
    /// Adds a method to the interface using a builder action.
    /// </summary>
    /// <param name="method">An action to configure the method builder.</param>
    /// <returns>The current builder instance.</returns>
    public InterfaceDeclarationModelBuilder WithMethod(Action<MethodDeclarationModelBuilder> method)
    {
        var builder = new MethodDeclarationModelBuilder();
        method(builder);
        _methods.Add(builder);
        return this;
    }

    /// <summary>
    /// Adds an event to the interface using a builder action.
    /// </summary>
    /// <param name="eventModel">An action to configure the event builder.</param>
    /// <returns>The current builder instance.</returns>
    public InterfaceDeclarationModelBuilder Event(Action<EventDeclarationModelBuilder> eventModel)
    {
        var builder = new EventDeclarationModelBuilder();
        eventModel(builder);
        _events.Add(builder);
        return this;
    }

    /// <summary>
    /// Adds a nested interface to the interface using a builder action.
    /// </summary>
    /// <param name="nestedInterface">An action to configure the nested interface builder.</param>
    /// <returns>The current builder instance.</returns>
    /// <remarks>Supports complex interface hierarchies.</remarks>
    public InterfaceDeclarationModelBuilder NestedInterface(Action<InterfaceDeclarationModelBuilder> nestedInterface)
    {
        var builder = new InterfaceDeclarationModelBuilder();
        nestedInterface(builder);
        _nestedInterfaces.Add(builder);
        return this;
    }

    /// <summary>
    /// Performs the build operation for all child builders.
    /// <remarks>Called during the build process to ensure all nested builders are built.</remarks>
    /// </summary>
    /// <param name="visitedCollector">Dictionary to track visited objects and prevent cycles.</param>
    protected override void BuildInternal(VisitedObjectDictionary visitedCollector)
    {
        BuildList(_attributes, visitedCollector);
        BuildList(_baseInterfaces, visitedCollector);
        BuildList(_typeParameters, visitedCollector);
        BuildList(_typeParameterConstraints, visitedCollector);
        BuildList(_properties, visitedCollector);
        BuildList(_methods, visitedCollector);
        BuildList(_events, visitedCollector);
        BuildList(_nestedInterfaces, visitedCollector);
    }

    /// <summary>
    /// Validates all child builders and collects failures.
    /// <remarks>Ensures that all nested builders are valid before instantiation.</remarks>
    /// </summary>
    /// <param name="visitedCollector">Dictionary to track visited objects and prevent cycles.</param>
    /// <param name="failures">Dictionary to collect validation failures.</param>
    protected override void ValidateInternal(VisitedObjectDictionary visitedCollector, FailuresDictionary failures)
    {
        if (string.IsNullOrEmpty(_name))
        {
            failures.Failure(nameof(_name), new DeclarationHaveNoNameException());
        }

        ValidateListInternal(_attributes, nameof(_attributes), visitedCollector, failures);
        ValidateListInternal(_baseInterfaces, nameof(_baseInterfaces), visitedCollector, failures);
        ValidateListInternal(_typeParameters, nameof(_typeParameters), visitedCollector, failures);
        ValidateListInternal(_typeParameterConstraints, nameof(_typeParameterConstraints), visitedCollector, failures);
        ValidateListInternal(_properties, nameof(_properties), visitedCollector, failures);
        ValidateListInternal(_methods, nameof(_methods), visitedCollector, failures);
        ValidateListInternal(_events, nameof(_events), visitedCollector, failures);
        ValidateListInternal(_nestedInterfaces, nameof(_nestedInterfaces), visitedCollector, failures);
    }

    /// <summary>
    /// Instantiates a new <see cref="InterfaceDeclarationModel"/> using the current builder state.
    /// <remarks>Called after successful validation to produce the final model.</remarks>
    /// </summary>
    /// <returns>An <see cref="InterfaceDeclarationModel"/> initialized with the specified name, modifiers, attributes, base interfaces, type parameters, constraints, properties, methods, events, and nested interfaces.</returns>
    protected override InterfaceDeclarationModel Instantiate() => new(
            _name ?? throw new DeclarationHaveNoNameException(),
            _modifiers,
            _attributes.AsReferenceList(),
            _baseInterfaces.AsReferenceList(),
            _typeParameters.AsReferenceList(),
            _typeParameterConstraints.AsReferenceList(),
            _properties.AsReferenceList(),
            _methods.AsReferenceList(),
            _events.AsReferenceList(),
            _nestedInterfaces.AsReferenceList());
}
