using FrenchExDev.Net.CSharp.Object.Builder;
using FrenchExDev.Net.CSharp.Object.Builder.Abstractions;

namespace FrenchExDev.Net.CSharp.Object.Model.Abstractions;

public class AttributeDeclarationModelBuilder : AbstractObjectBuilder<AttributeDeclarationModel, AttributeDeclarationModelBuilder>
{
    private string? _name;
    private readonly List<string> _arguments = new();
    public AttributeDeclarationModelBuilder Name(string name)
    {
        _name = name;
        return this;
    }
    public AttributeDeclarationModelBuilder Argument(string argument)
    {
        _arguments.Add(argument);
        return this;
    }
    protected override IObjectBuildResult<AttributeDeclarationModel> BuildInternal(ExceptionBuildList exceptions, VisitedObjectsList visited)
    {
        if (string.IsNullOrEmpty(_name))
        {
            exceptions.Add(new InvalidOperationException("Attribute name must be provided."));
        }
        if (exceptions.Any())
        {
            return new FailureObjectBuildResult<AttributeDeclarationModel, AttributeDeclarationModelBuilder>(this, exceptions, visited);
        }
        ArgumentNullException.ThrowIfNull(_name);
        return new SuccessObjectBuildResult<AttributeDeclarationModel>(new AttributeDeclarationModel
        {
            Name = _name,
            Arguments = _arguments
        });
    }
}
