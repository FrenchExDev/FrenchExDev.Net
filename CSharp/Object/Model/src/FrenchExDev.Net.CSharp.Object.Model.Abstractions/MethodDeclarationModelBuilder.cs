using FrenchExDev.Net.CSharp.Object.Builder;
using FrenchExDev.Net.CSharp.Object.Builder.Abstractions;

namespace FrenchExDev.Net.CSharp.Object.Model.Abstractions;

public class MethodDeclarationModelBuilder : AbstractObjectBuilder<MethodDeclarationModel, MethodDeclarationModelBuilder>
{
    private string? _name;
    public List<string> Modifiers { get; } = new();
    private string _returnType = "void";
    private readonly List<ParameterDeclarationModelBuilder> _parameters = new();
    private string? _body;
    private readonly List<AttributeDeclarationModelBuilder> _attributes = new();
    public MethodDeclarationModelBuilder Name(string name)
    {
        _name = name;
        return this;
    }
    public MethodDeclarationModelBuilder Modifier(string modifier)
    {
        Modifiers.Add(modifier);
        return this;
    }
    public MethodDeclarationModelBuilder ReturnType(string returnType)
    {
        _returnType = returnType;
        return this;
    }

    public MethodDeclarationModelBuilder Parameter(Action<ParameterDeclarationModelBuilder> body)
    {
        var builder = new ParameterDeclarationModelBuilder();
        body(builder);
        _parameters.Add(builder);
        return this;
    }

    public MethodDeclarationModelBuilder Body(string body)
    {
        _body = body;
        return this;
    }

    public MethodDeclarationModelBuilder Attribute(Action<AttributeDeclarationModelBuilder> body)
    {
        var builder = new AttributeDeclarationModelBuilder();
        body(builder);
        _attributes.Add(builder);
        return this;
    }

    protected override IObjectBuildResult<MethodDeclarationModel> BuildInternal(ExceptionBuildList exceptions, VisitedObjectsList visited)
    {
        if (visited.TryGetValue(this, out var existing) && existing is MethodDeclarationModel existingModel)
        {
            return new SuccessObjectBuildResult<MethodDeclarationModel>(existingModel);
        }

        visited.Set(this, null!);

        if (string.IsNullOrEmpty(_name))
        {
            exceptions.Add(new InvalidOperationException("Method name must be provided."));
        }
        if (exceptions.Any())
        {
            return new FailureObjectBuildResult<MethodDeclarationModel, MethodDeclarationModelBuilder>(this, exceptions, visited);
        }

        ArgumentNullException.ThrowIfNull(_name);

        var buildParameters = _parameters
            .Select(x => x.Build(visited))
            .ToList();

        if (buildParameters.OfType<FailureObjectBuildResult<ParameterDeclarationModel, ParameterDeclarationModelBuilder>>().Any())
        {
            exceptions.AddRange(buildParameters
                .OfType<FailureObjectBuildResult<ParameterDeclarationModel, ParameterDeclarationModelBuilder>>()
                .SelectMany(x => x.Exceptions));
        }

        var attributes = _attributes
            .Select(x => x.Build(visited))
            .ToList();

        if (attributes.OfType<FailureObjectBuildResult<AttributeDeclarationModel, AttributeDeclarationModelBuilder>>().Any())
        {
            exceptions.AddRange(attributes
                .OfType<FailureObjectBuildResult<AttributeDeclarationModel, AttributeDeclarationModelBuilder>>()
                .SelectMany(x => x.Exceptions));
        }

        if (exceptions.Any())
        {
            return new FailureObjectBuildResult<MethodDeclarationModel, MethodDeclarationModelBuilder>(this, exceptions, visited);
        }

        return new SuccessObjectBuildResult<MethodDeclarationModel>(new MethodDeclarationModel
        {
            Body = _body,
            Name = _name,
            Modifiers = Modifiers,
            ReturnType = _returnType,
            Parameters = buildParameters.ToResultList<ParameterDeclarationModel>(),
            Attributes = attributes.ToResultList<AttributeDeclarationModel>()
        });
    }
}
