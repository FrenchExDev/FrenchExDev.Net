using FrenchExDev.Net.CSharp.Object.Builder;
using FrenchExDev.Net.CSharp.Object.Builder.Abstractions;

namespace FrenchExDev.Net.CSharp.Object.Model.Abstractions;

public class StructDeclarationModelBuilder : AbstractObjectBuilder<StructDeclarationModel, StructDeclarationModelBuilder>
{
    private string? _name;
    public List<StructModifier> Modifiers { get; } = new();
    private readonly List<AttributeDeclarationModel> _attributes = new();
    public StructDeclarationModelBuilder Name(string name)
    {
        _name = name;
        return this;
    }
    public StructDeclarationModelBuilder Modifier(StructModifier modifier)
    {
        Modifiers.Add(modifier);
        return this;
    }
    public StructDeclarationModelBuilder Attribute(AttributeDeclarationModel attribute)
    {
        _attributes.Add(attribute);
        return this;
    }
    protected override IObjectBuildResult<StructDeclarationModel> BuildInternal(ExceptionBuildList exceptions, VisitedObjectsList visited)
    {
        if (string.IsNullOrEmpty(_name))
        {
            exceptions.Add(new InvalidOperationException("Struct name must be provided."));
        }
        if (exceptions.Any())
        {
            return new FailureObjectBuildResult<StructDeclarationModel, StructDeclarationModelBuilder>(this, exceptions, visited);
        }
        ArgumentNullException.ThrowIfNull(_name);
        return new SuccessObjectBuildResult<StructDeclarationModel>(new StructDeclarationModel
        {
            Name = _name,
            Modifiers = Modifiers,
            Attributes = _attributes
        });
    }
}
