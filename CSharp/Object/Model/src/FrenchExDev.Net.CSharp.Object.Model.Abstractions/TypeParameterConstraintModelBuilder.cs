using FrenchExDev.Net.CSharp.Object.Builder;
using FrenchExDev.Net.CSharp.Object.Builder.Abstractions;

namespace FrenchExDev.Net.CSharp.Object.Model.Abstractions;

public class TypeParameterConstraintModelBuilder : AbstractObjectBuilder<TypeParameterConstraintModel, TypeParameterConstraintModelBuilder>
{
    private string? _typeParameter;
    private readonly List<FreeTypeParameterConstraintDeclarationModel> _constraints = new();
    public TypeParameterConstraintModelBuilder TypeParameter(string typeParameter)
    {
        _typeParameter = typeParameter;
        return this;
    }
    public TypeParameterConstraintModelBuilder Constraint(FreeTypeParameterConstraintDeclarationModel constraint)
    {
        _constraints.Add(constraint);
        return this;
    }
    protected override IObjectBuildResult<TypeParameterConstraintModel> BuildInternal(ExceptionBuildList exceptions, VisitedObjectsList visited)
    {
        if (string.IsNullOrEmpty(_typeParameter))
        {
            exceptions.Add(new InvalidOperationException("Type parameter name must be provided."));
        }

        if (exceptions.Any())
        {
            return new FailureObjectBuildResult<TypeParameterConstraintModel, TypeParameterConstraintModelBuilder>(this, exceptions, visited);
        }

        ArgumentNullException.ThrowIfNull(_typeParameter);

        return new SuccessObjectBuildResult<TypeParameterConstraintModel>(new TypeParameterConstraintModel
        {
            TypeParameter = _typeParameter,
            Constraints = _constraints
        });
    }
}
