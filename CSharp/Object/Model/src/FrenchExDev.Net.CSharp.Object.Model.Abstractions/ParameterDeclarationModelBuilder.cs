using FrenchExDev.Net.CSharp.Object.Builder;
using FrenchExDev.Net.CSharp.Object.Builder.Abstractions;

namespace FrenchExDev.Net.CSharp.Object.Model.Abstractions;

public class ParameterDeclarationModelBuilder : AbstractObjectBuilder<ParameterDeclarationModel, ParameterDeclarationModelBuilder>
{
    private string? _type;
    private string? _name;
    private string? _defaultValue;

    public ParameterDeclarationModelBuilder Type(string type)
    {
        _type = type;
        return this;
    }

    public ParameterDeclarationModelBuilder Name(string name)
    {
        _name = name;
        return this;
    }

    public ParameterDeclarationModelBuilder DefaultValue(string defaultValue)
    {
        _defaultValue = defaultValue;
        return this;
    }

    protected override IObjectBuildResult<ParameterDeclarationModel> BuildInternal(ExceptionBuildList exceptions, VisitedObjectsList visited)
    {
        if (string.IsNullOrEmpty(_type))
        {
            exceptions.Add(new InvalidOperationException("Parameter type must be provided."));
        }

        if (string.IsNullOrEmpty(_name))
        {
            exceptions.Add(new InvalidOperationException("Parameter name must be provided."));
        }

        if (exceptions.Any())
        {
            return new FailureObjectBuildResult<ParameterDeclarationModel, ParameterDeclarationModelBuilder>(this, exceptions, visited);
        }

        ArgumentNullException.ThrowIfNull(_type);
        ArgumentNullException.ThrowIfNull(_name);

        return new SuccessObjectBuildResult<ParameterDeclarationModel>(new ParameterDeclarationModel
        {
            Type = _type,
            Name = _name,
            DefaultValue = _defaultValue
        });
    }
}