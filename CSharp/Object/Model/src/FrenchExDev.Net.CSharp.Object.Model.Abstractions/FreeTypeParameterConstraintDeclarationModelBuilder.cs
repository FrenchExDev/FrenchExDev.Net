using FrenchExDev.Net.CSharp.Object.Builder;
using FrenchExDev.Net.CSharp.Object.Builder.Abstractions;

namespace FrenchExDev.Net.CSharp.Object.Model.Abstractions;

public class FreeTypeParameterConstraintDeclarationModelBuilder : AbstractObjectBuilder<FreeTypeParameterConstraintDeclarationModel, FreeTypeParameterConstraintDeclarationModelBuilder>
{
    private TypeParameterConstraintEnum? _constraint;
    private readonly List<string> _constraints = new();
    public FreeTypeParameterConstraintDeclarationModelBuilder Constraint(TypeParameterConstraintEnum constraint)
    {
        _constraint = constraint;
        return this;
    }
    public FreeTypeParameterConstraintDeclarationModelBuilder Type(string constraint)
    {
        _constraints.Add(constraint);
        return this;
    }
    protected override IObjectBuildResult<FreeTypeParameterConstraintDeclarationModel> BuildInternal(ExceptionBuildList exceptions, VisitedObjectsList visited)
    {
        if (_constraint == null && !_constraints.Any())
        {
            exceptions.Add(new InvalidOperationException("At least one constraint must be provided."));
        }
        if (exceptions.Any())
        {
            return new FailureObjectBuildResult<FreeTypeParameterConstraintDeclarationModel, FreeTypeParameterConstraintDeclarationModelBuilder>(this, exceptions, visited);
        }
        return new SuccessObjectBuildResult<FreeTypeParameterConstraintDeclarationModel>(new FreeTypeParameterConstraintDeclarationModel
        {
            Constraint = _constraint,
            Constraints = _constraints
        });
    }
}
