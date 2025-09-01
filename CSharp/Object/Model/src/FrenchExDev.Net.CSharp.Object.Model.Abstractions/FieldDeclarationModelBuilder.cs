using FrenchExDev.Net.CSharp.Object.Builder;
using FrenchExDev.Net.CSharp.Object.Builder.Abstractions;

namespace FrenchExDev.Net.CSharp.Object.Model.Abstractions;

/// <summary>
/// Provides a builder for constructing instances of <see cref="FieldDeclarationModel"/>.
/// </summary>
/// <remarks>This builder allows the configuration of field modifiers, type, name, and an optional initializer for
/// creating a <see cref="FieldDeclarationModel"/>. Use the provided fluent methods to configure the field declaration
/// before calling <see cref="AbstractObjectBuilder{TObject, TBuilder}.Build"/> to generate the final model.</remarks>
public class FieldDeclarationModelBuilder : AbstractObjectBuilder<FieldDeclarationModel, FieldDeclarationModelBuilder>
{
    private readonly List<string> _modifiers = new();
    private string _type = string.Empty;
    private string _name = string.Empty;
    private string? _initializer;

    public FieldDeclarationModelBuilder Modifier(string modifier)
    {
        _modifiers.Add(modifier);
        return this;
    }
    public FieldDeclarationModelBuilder Type(string type)
    {
        _type = type;
        return this;
    }
    public FieldDeclarationModelBuilder Name(string name)
    {
        _name = name;
        return this;
    }
    public FieldDeclarationModelBuilder Initializer(string? initializer)
    {
        _initializer = initializer;
        return this;
    }
    protected override IObjectBuildResult<FieldDeclarationModel> BuildInternal(ExceptionBuildList exceptions, VisitedObjectsList visited)
    {
        if (visited.TryGetValue(this, out var existing) && existing is FieldDeclarationModel existingModel)
        {
            return new SuccessObjectBuildResult<FieldDeclarationModel>(existingModel);
        }
        visited.Set(this, null!);
        if (string.IsNullOrEmpty(_name))
        {
            exceptions.Add(new InvalidOperationException("Field name must be provided."));
        }
        if (exceptions.Any())
        {
            return new FailureObjectBuildResult<FieldDeclarationModel, FieldDeclarationModelBuilder>(this, exceptions, visited);
        }
        ArgumentNullException.ThrowIfNull(_name);
        return new SuccessObjectBuildResult<FieldDeclarationModel>(new FieldDeclarationModel
        {
            Modifiers = _modifiers,
            Type = _type,
            Name = _name,
            Initializer = _initializer
        });
    }
}
