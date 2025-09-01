using FrenchExDev.Net.CSharp.Object.Builder;
using FrenchExDev.Net.CSharp.Object.Builder.Abstractions;

namespace FrenchExDev.Net.CSharp.Object.Model.Abstractions;

public class EventModelBuilder : AbstractObjectBuilder<EventModel, EventModelBuilder>
{
    private string? _type;
    private string? _name;
    private readonly List<string> _modifiers = new();
    private readonly List<AttributeDeclarationModelBuilder> _attributes = new();

    public EventModelBuilder Modifier(string modifier)
    {
        _modifiers.Add(modifier);
        return this;
    }
    public EventModelBuilder Type(string type)
    {
        _type = type;
        return this;
    }
    public EventModelBuilder Name(string name)
    {
        _name = name;
        return this;
    }
    public EventModelBuilder Attribute(Action<AttributeDeclarationModelBuilder> attribute)
    {
        var builder = new AttributeDeclarationModelBuilder();
        attribute(builder);
        _attributes.Add(builder);
        return this;
    }

    protected override IObjectBuildResult<EventModel> BuildInternal(ExceptionBuildList exceptions, VisitedObjectsList visited)
    {
        var attributes = _attributes
            .Select(x => x.Build(visited))
            .ToList();

        if (attributes.OfType<FailureObjectBuildResult<AttributeDeclarationModel, AttributeDeclarationModelBuilder>>().Any())
        {
            exceptions.AddRange(attributes
                .OfType<FailureObjectBuildResult<AttributeDeclarationModel, AttributeDeclarationModelBuilder>>()
                .SelectMany(x => x.Exceptions));
        }

        if (string.IsNullOrEmpty(_name))
        {
            exceptions.Add(new InvalidOperationException("Event name must be provided."));
        }

        if (string.IsNullOrEmpty(_type))
        {
            exceptions.Add(new InvalidOperationException("Event type must be provided."));
        }

        if (exceptions.Any())
        {
            return new FailureObjectBuildResult<EventModel, EventModelBuilder>(this, exceptions, visited);
        }

        ArgumentNullException.ThrowIfNull(_name);
        ArgumentNullException.ThrowIfNull(_type);

        return new SuccessObjectBuildResult<EventModel>(new EventModel
        {
            Modifiers = _modifiers,
            Type = _type,
            Name = _name,
            Attributes = attributes.ToResultList()
        });
    }
}
