using FrenchExDev.Net.CSharp.Object.Builder;
using FrenchExDev.Net.CSharp.Object.Builder.Abstractions;

namespace FrenchExDev.Net.CSharp.Object.Model.Abstractions;

public class EnumMemberDeclarationModelBuilder : AbstractObjectBuilder<EnumMemberDeclarationModel, EnumMemberDeclarationModelBuilder>
{
    private string? _name;
    private string? _value;
    private readonly List<AttributeDeclarationModel> _attributes = new();
    public EnumMemberDeclarationModelBuilder Name(string name)
    {
        _name = name;
        return this;
    }
    public EnumMemberDeclarationModelBuilder Value(string value)
    {
        _value = value;
        return this;
    }
    public EnumMemberDeclarationModelBuilder Attribute(AttributeDeclarationModel attribute)
    {
        _attributes.Add(attribute);
        return this;
    }
    protected override IObjectBuildResult<EnumMemberDeclarationModel> BuildInternal(ExceptionBuildList exceptions, VisitedObjectsList visited)
    {
        if (string.IsNullOrEmpty(_name))
        {
            exceptions.Add(new InvalidOperationException("Enum member name must be provided."));
        }
        if (exceptions.Any())
        {
            return new FailureObjectBuildResult<EnumMemberDeclarationModel, EnumMemberDeclarationModelBuilder>(this, exceptions, visited);
        }
        ArgumentNullException.ThrowIfNull(_name);
        return new SuccessObjectBuildResult<EnumMemberDeclarationModel>(new EnumMemberDeclarationModel
        {
            Name = _name,
            Value = _value,
            Attributes = _attributes
        });
    }
}
