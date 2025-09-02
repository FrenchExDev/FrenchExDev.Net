using FrenchExDev.Net.CSharp.Object.Model.Abstractions;
using FrenchExDev.Net.CSharp.Object.Model.Infrastructure;
using Microsoft.CodeAnalysis.CSharp.Syntax;

/// <summary>
/// Extensions for converting model objects to Roslyn syntax nodes.
/// </summary>
public static class ModelSyntaxExtensions
{
    /// <summary>
    /// Converts an <see cref="IDeclarationModel"/> to the appropriate Roslyn <see cref="MemberDeclarationSyntax"/>.
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    /// <exception cref="NotSupportedException"></exception>
    public static MemberDeclarationSyntax ToSyntax(this IDeclarationModel model)
    {
        return model switch
        {
            NamespaceDeclarationModel namespaceModel => namespaceModel.ToSyntax(),
            ClassDeclarationModel classModel => classModel.ToSyntax(),
            InterfaceDeclarationModel interfaceModel => interfaceModel.ToSyntax(),
            EnumDeclarationModel enumModel => enumModel.ToSyntax(),
            StructDeclarationModel structModel => structModel.ToSyntax(),
            PropertyDeclarationModel propertyModel => propertyModel.ToSyntax(),
            MethodDeclarationModel methodModel => methodModel.ToSyntax(),
            FieldDeclarationModel fieldModel => fieldModel.ToSyntax(),
            EventModel eventModel => eventModel.ToSyntax(),
            ConstructorDeclarationModel constructorModel => constructorModel.ToSyntax(),
            _ => throw new NotSupportedException($"Unsupported model type: {model.GetType().FullName}")
        };
    }

    /// <summary>
    /// Converts a <see cref="NamespaceDeclarationModel"/> to a Roslyn <see cref="NamespaceDeclarationSyntax"/>.
    /// Used to generate namespace syntax from the model object.
    /// </summary>
    /// <param name="namespace">The namespace model to convert.</param>
    /// <returns>A Roslyn syntax node representing the namespace.</returns>
    public static NamespaceDeclarationSyntax ToSyntax(this NamespaceDeclarationModel @namespace)
    {
        return RoslynModelConverter.ToSyntax(@namespace);
    }

    /// <summary>
    /// Converts a <see cref="ClassDeclarationModel"/> to a Roslyn <see cref="ClassDeclarationSyntax"/>.
    /// Used to generate class declaration syntax from the model object.
    /// </summary>
    /// <param name="model">The class model to convert.</param>
    /// <returns>A Roslyn syntax node representing the class.</returns>
    public static ClassDeclarationSyntax ToSyntax(this ClassDeclarationModel model)
    {
        return RoslynModelConverter.ToSyntax(model);
    }

    /// <summary>
    /// Converts an <see cref="InterfaceDeclarationModel"/> to a Roslyn <see cref="InterfaceDeclarationSyntax"/>.
    /// Used to generate interface declaration syntax from the model object.
    /// </summary>
    /// <param name="model">The interface model to convert.</param>
    /// <returns>A Roslyn syntax node representing the interface.</returns>
    public static InterfaceDeclarationSyntax ToSyntax(this InterfaceDeclarationModel model)
    {
        return RoslynModelConverter.ToSyntax(model);
    }

    /// <summary>
    /// Converts an <see cref="EnumDeclarationModel"/> to a Roslyn <see cref="EnumDeclarationSyntax"/>.
    /// Used to generate enum declaration syntax from the model object.
    /// </summary>
    /// <param name="model">The enum model to convert.</param>
    /// <returns>A Roslyn syntax node representing the enum.</returns>
    public static EnumDeclarationSyntax ToSyntax(this EnumDeclarationModel model)
    {
        return RoslynModelConverter.ToSyntax(model);
    }

    /// <summary>
    /// Converts a <see cref="StructDeclarationModel"/> to a Roslyn <see cref="StructDeclarationSyntax"/>.
    /// Used to generate struct declaration syntax from the model object.
    /// </summary>
    /// <param name="model">The struct model to convert.</param>
    /// <returns>A Roslyn syntax node representing the struct.</returns>
    public static StructDeclarationSyntax ToSyntax(this StructDeclarationModel model)
    {
        return RoslynModelConverter.ToSyntax(model);
    }

    /// <summary>
    /// Converts a <see cref="PropertyDeclarationModel"/> to a Roslyn <see cref="PropertyDeclarationSyntax"/>.
    /// Used to generate property declaration syntax from the model object.
    /// </summary>
    /// <param name="prop">The property model to convert.</param>
    /// <returns>A Roslyn syntax node representing the property.</returns>
    public static PropertyDeclarationSyntax ToSyntax(this PropertyDeclarationModel prop)
    {
        return RoslynModelConverter.ToSyntax(prop);
    }

    /// <summary>
    /// Converts a <see cref="MethodDeclarationModel"/> to a Roslyn <see cref="MethodDeclarationSyntax"/>.
    /// Used to generate method declaration syntax from the model object.
    /// </summary>
    /// <param name="method">The method model to convert.</param>
    /// <returns>A Roslyn syntax node representing the method.</returns>
    public static MethodDeclarationSyntax ToSyntax(this MethodDeclarationModel method)
    {
        return RoslynModelConverter.ToSyntax(method);
    }

    /// <summary>
    /// Converts a <see cref="FieldDeclarationModel"/> to a Roslyn <see cref="FieldDeclarationSyntax"/>.
    /// Used to generate field declaration syntax from the model object.
    /// </summary>
    /// <param name="field">The field model to convert.</param>
    /// <returns>A Roslyn syntax node representing the field.</returns>
    public static FieldDeclarationSyntax ToSyntax(this FieldDeclarationModel field)
    {
        return RoslynModelConverter.ToSyntax(field);
    }

    /// <summary>
    /// Converts an <see cref="EventModel"/> to a Roslyn <see cref="EventDeclarationSyntax"/>.
    /// Used to generate event declaration syntax from the model object.
    /// </summary>
    /// <param name="evt">The event model to convert.</param>
    /// <returns>A Roslyn syntax node representing the event.</returns>
    public static EventDeclarationSyntax ToSyntax(this EventModel evt)
    {
        return RoslynModelConverter.ToSyntax(evt);
    }

    /// <summary>
    /// Converts a <see cref="ConstructorDeclarationModel"/> to a Roslyn <see cref="ConstructorDeclarationSyntax"/>.
    /// Used to generate constructor declaration syntax from the model object.
    /// </summary>
    /// <param name="ctor">The constructor model to convert.</param>
    /// <returns>A Roslyn syntax node representing the constructor.</returns>
    public static ConstructorDeclarationSyntax ToSyntax(this ConstructorDeclarationModel ctor)
    {
        return RoslynModelConverter.ToSyntax(ctor);
    }
}
