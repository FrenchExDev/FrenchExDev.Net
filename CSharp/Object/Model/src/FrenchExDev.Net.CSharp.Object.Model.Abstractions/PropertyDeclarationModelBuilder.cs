using FrenchExDev.Net.CSharp.Object.Builder;
using FrenchExDev.Net.CSharp.Object.Builder.Abstractions;

namespace FrenchExDev.Net.CSharp.Object.Model.Abstractions;

public class PropertyDeclarationModelBuilder : AbstractObjectBuilder<PropertyDeclarationModel, PropertyDeclarationModelBuilder>
{
    private readonly List<string> _modifiers = new();
    private string _type = string.Empty;
    private string _name = string.Empty;
    private bool _hasGetter = true;
    private bool _hasSetter = true;
    private string? _initializer;

    public PropertyDeclarationModelBuilder Modifier(string modifier)
    {
        _modifiers.Add(modifier);
        return this;
    }

    public PropertyDeclarationModelBuilder Type(string type)
    {
        _type = type;
        return this;
    }

    public PropertyDeclarationModelBuilder Name(string name)
    {
        _name = name;
        return this;
    }

    public PropertyDeclarationModelBuilder HasGetter(bool hasGetter)
    {
        _hasGetter = hasGetter;
        return this;
    }

    public PropertyDeclarationModelBuilder HasSetter(bool hasSetter)
    {
        _hasSetter = hasSetter;
        return this;
    }

    public PropertyDeclarationModelBuilder Initializer(string? initializer)
    {
        _initializer = initializer;
        return this;
    }

    protected override IObjectBuildResult<PropertyDeclarationModel> BuildInternal(ExceptionBuildList exceptions, VisitedObjectsList visited)
    {
        if (visited.TryGetValue(this, out var existing) && existing is PropertyDeclarationModel existingModel)
        {
            return new SuccessObjectBuildResult<PropertyDeclarationModel>(existingModel);
        }

        visited.Set(this, null!);

        if (string.IsNullOrEmpty(_name))
        {
            exceptions.Add(new InvalidOperationException("Property name must be provided."));
        }
        if (exceptions.Any())
        {
            return new FailureObjectBuildResult<PropertyDeclarationModel, PropertyDeclarationModelBuilder>(this, exceptions, visited);
        }

        ArgumentNullException.ThrowIfNull(_name);

        return new SuccessObjectBuildResult<PropertyDeclarationModel>(new PropertyDeclarationModel
        {
            Modifiers = _modifiers,
            Type = _type,
            Name = _name,
            HasGetter = _hasGetter,
            HasSetter = _hasSetter,
            Initializer = _initializer
        });
    }
}
