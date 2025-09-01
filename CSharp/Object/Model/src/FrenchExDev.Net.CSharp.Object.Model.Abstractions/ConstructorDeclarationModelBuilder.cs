using FrenchExDev.Net.CSharp.Object.Builder;
using FrenchExDev.Net.CSharp.Object.Builder.Abstractions;

namespace FrenchExDev.Net.CSharp.Object.Model.Abstractions;

public class ConstructorDeclarationModelBuilder : AbstractObjectBuilder<ConstructorDeclarationModel, ConstructorDeclarationModelBuilder>
{
    private string? _name;
    public List<string> Modifiers { get; } = new();
    private readonly List<ParameterDeclarationModel> _parameters = new();
    private string? _body;
    public ConstructorDeclarationModelBuilder Name(string name)
    {
        _name = name;
        return this;
    }
    public ConstructorDeclarationModelBuilder Modifier(string modifier)
    {
        Modifiers.Add(modifier);
        return this;
    }
    public ConstructorDeclarationModelBuilder Parameter(ParameterDeclarationModel parameter)
    {
        _parameters.Add(parameter);
        return this;
    }
    public ConstructorDeclarationModelBuilder Body(string body)
    {
        _body = body;
        return this;
    }
    protected override IObjectBuildResult<ConstructorDeclarationModel> BuildInternal(ExceptionBuildList exceptions, VisitedObjectsList visited)
    {
        if (string.IsNullOrEmpty(_name))
        {
            exceptions.Add(new InvalidOperationException("Constructor name must be provided."));
        }
        if (exceptions.Any())
        {
            return new FailureObjectBuildResult<ConstructorDeclarationModel, ConstructorDeclarationModelBuilder>(this, exceptions, visited);
        }
        ArgumentNullException.ThrowIfNull(_name);
        return new SuccessObjectBuildResult<ConstructorDeclarationModel>(new ConstructorDeclarationModel
        {
            Name = _name,
            Modifiers = Modifiers,
            Parameters = _parameters,
            Body = _body
        });
    }
}
