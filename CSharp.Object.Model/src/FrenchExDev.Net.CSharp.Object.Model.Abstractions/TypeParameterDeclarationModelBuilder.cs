using FrenchExDev.Net.CSharp.Object.Builder;
using FrenchExDev.Net.CSharp.Object.Builder.Abstractions;

namespace FrenchExDev.Net.CSharp.Object.Model.Abstractions;

public class TypeParameterDeclarationModelBuilder : AbstractObjectBuilder<TypeParameterDeclarationModel, TypeParameterDeclarationModelBuilder>
{
    private string? _name;

    public TypeParameterDeclarationModelBuilder Name(string name)
    {
        _name = name;
        return this;
    }

    protected override IObjectBuildResult<TypeParameterDeclarationModel> BuildInternal(ExceptionBuildDictionary exceptions, VisitedObjectsList visited)
    {
        if (string.IsNullOrEmpty(_name))
        {
            exceptions.Add(new InvalidOperationException("Type parameter name must be provided."));
        }

        if (exceptions.Any())
        {
            return Failure(exceptions, visited);
        }

        ArgumentNullException.ThrowIfNull(_name);

        return Success(new TypeParameterDeclarationModel
        {
            Name = _name
        });
    }
}
