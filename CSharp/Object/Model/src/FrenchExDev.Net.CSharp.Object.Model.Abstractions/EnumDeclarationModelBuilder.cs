using FrenchExDev.Net.CSharp.Object.Builder;
using FrenchExDev.Net.CSharp.Object.Builder.Abstractions;

namespace FrenchExDev.Net.CSharp.Object.Model.Abstractions;

public class EnumDeclarationModelBuilder : AbstractObjectBuilder<EnumDeclarationModel, EnumDeclarationModelBuilder>
{
    private string? _name;
    public List<EnumModifier> Modifiers { get; } = new();
    private readonly List<AttributeDeclarationModel> _attributes = new();
    private string? _underlyingType;
    private readonly List<EnumMemberDeclarationModel> _members = new();

    public EnumDeclarationModelBuilder Name(string name)
    {
        _name = name;
        return this;
    }

    public EnumDeclarationModelBuilder Modifier(EnumModifier modifier)
    {
        Modifiers.Add(modifier);
        return this;
    }

    public EnumDeclarationModelBuilder Attribute(AttributeDeclarationModel attribute)
    {
        _attributes.Add(attribute);
        return this;
    }

    public EnumDeclarationModelBuilder UnderlyingType(string underlyingType)
    {
        _underlyingType = underlyingType;
        return this;
    }

    public EnumDeclarationModelBuilder Member(EnumMemberDeclarationModel member)
    {
        _members.Add(member);
        return this;
    }

    protected override IObjectBuildResult<EnumDeclarationModel> BuildInternal(ExceptionBuildList exceptions, VisitedObjectsList visited)
    {
        if (string.IsNullOrEmpty(_name))
        {
            exceptions.Add(new InvalidOperationException("Enum name must be provided."));
        }

        if (exceptions.Any())
        {
            return new FailureObjectBuildResult<EnumDeclarationModel, EnumDeclarationModelBuilder>(this, exceptions, visited);
        }

        ArgumentNullException.ThrowIfNull(_name);

        return new SuccessObjectBuildResult<EnumDeclarationModel>(new EnumDeclarationModel
        {
            Name = _name,
            Modifiers = Modifiers,
            Attributes = _attributes,
            UnderlyingType = _underlyingType,
            Members = _members
        });
    }
}
