using FrenchExDev.Net.CSharp.Object.Model.Abstractions;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace FrenchExDev.Net.CSharp.Object.Model.Infrastructure;

/// <summary>
/// Provides methods to convert various model representations of C# code elements  (e.g., classes, interfaces, enums,
/// structs, methods, properties) into their corresponding  Roslyn syntax tree representations.
/// </summary>
/// <remarks>This utility class is designed to facilitate the generation of C# syntax trees using the Roslyn API 
/// by converting domain-specific models (e.g., <c>ClassDeclarationModel</c>, <c>PropertyDeclarationModel</c>)  into
/// Roslyn syntax nodes (e.g., <c>ClassDeclarationSyntax</c>, <c>PropertyDeclarationSyntax</c>).  The methods in this
/// class handle the creation of syntax nodes for various C# constructs, including  namespaces, classes, interfaces,
/// enums, structs, methods, properties, fields, events, and constructors.  Each method takes a specific model type as
/// input and returns the corresponding Roslyn syntax node.</remarks>
public class ModelRoslynConverter : IModelRoselynConverter
{
    /// <summary>
    /// Converts the specified <see cref="IDeclarationModel"/> into its corresponding <see
    /// cref="MemberDeclarationSyntax"/> representation.
    /// </summary>
    /// <param name="model">The declaration model to convert. This parameter must be an instance of a supported <see
    /// cref="IDeclarationModel"/> subtype, such as <see cref="NamespaceDeclarationModel"/>, <see
    /// cref="ClassDeclarationModel"/>, or other supported types.</param>
    /// <returns>A <see cref="MemberDeclarationSyntax"/> representing the syntax tree node for the provided declaration model.</returns>
    /// <exception cref="NotSupportedException">Thrown if the <paramref name="model"/> is not of a supported type.</exception>
    public MemberDeclarationSyntax ToSyntax(IDeclarationModel model)
    {
        return model switch
        {
            NamespaceDeclarationModel ns => ToSyntax(ns),
            InterfaceDeclarationModel iface => ToSyntax(iface),
            ClassDeclarationModel cls => ToSyntax(cls),
            PropertyDeclarationModel prop => ToSyntax(prop),
            EnumDeclarationModel enm => ToSyntax(enm),
            ConstructorDeclarationModel ctor => ToSyntax(ctor),
            EventModel evt => ToSyntax(evt),
            StructDeclarationModel strct => ToSyntax(strct),
            FieldDeclarationModel field => ToSyntax(field),
            MethodDeclarationModel method => ToSyntax(method),
            _ => throw new NotSupportedException($"Model type {model.GetType().Name} not supported.")
        };
    }

    /// <summary>
    /// Converts a <see cref="NamespaceDeclarationModel"/> to a Roslyn <see cref="NamespaceDeclarationSyntax"/>.
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    public NamespaceDeclarationSyntax ToSyntax(NamespaceDeclarationModel model)
    {
        var members = new List<MemberDeclarationSyntax>();
        members.AddRange(model.Classes.Select(ToSyntax));
        members.AddRange(model.Interfaces.Select(ToSyntax));
        members.AddRange(model.Enums.Select(ToSyntax));
        members.AddRange(model.Structs.Select(ToSyntax));
        members.AddRange(model.NestedNamespaces.Select(ToSyntax));

        var ns = SyntaxFactory
            .NamespaceDeclaration(SyntaxFactory.ParseName(model.Name))
            .AddMembers(members.ToArray());

        return ns;
    }

    /// <summary>
    /// Converts a <see cref="ClassDeclarationModel"/> to a Roslyn <see cref="ClassDeclarationSyntax"/>.
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    public ClassDeclarationSyntax ToSyntax(ClassDeclarationModel model)
    {
        var modifiers = model.Modifiers.Select(m => SyntaxFactory.ParseToken(ToModifierString(m))).ToArray();

        var attributes = SyntaxFactory.List(model.Attributes.Select(attr =>
            SyntaxFactory.AttributeList(
                SyntaxFactory.SingletonSeparatedList(
                    SyntaxFactory.Attribute(SyntaxFactory.IdentifierName(attr.Name))
                        .WithArgumentList(
                            attr.Arguments.Count > 0
                                ? SyntaxFactory.AttributeArgumentList(
                                    SyntaxFactory.SeparatedList(
                                        attr.Arguments.Select(a => SyntaxFactory.AttributeArgument(SyntaxFactory.ParseExpression(a)))
                                    ))
                                : null
                        )
                )
            )
        ));

        var baseType = !string.IsNullOrEmpty(model.BaseType) ? SyntaxFactory.SimpleBaseType(SyntaxFactory.ParseTypeName(model.BaseType)) : null;
        var baseTypes = new List<BaseTypeSyntax>();
        if (!string.IsNullOrWhiteSpace(model.BaseType))
            baseTypes.Add(SyntaxFactory.SimpleBaseType(SyntaxFactory.ParseTypeName(model.BaseType)));
        baseTypes.AddRange(model.ImplementedInterfaces.Select(i => SyntaxFactory.SimpleBaseType(SyntaxFactory.ParseTypeName(i))));

        var members = new List<MemberDeclarationSyntax>();
        members.AddRange(model.Fields.Select((x) => ToSyntax(x)));
        members.AddRange(model.Properties.Select((x) => ToSyntax(x)));
        members.AddRange(model.Methods.Select((x) => ToSyntax(x)));
        members.AddRange(model.Constructors.Select((x) => ToSyntax(x)));
        members.AddRange(model.NestedClasses.Select((x) => ToSyntax(x)));

        var classDeclaration = SyntaxFactory.ClassDeclaration(model.Name)
            .AddModifiers(modifiers)
            .AddAttributeLists(attributes.ToArray())
            .AddMembers(members.ToArray());

        if (!string.IsNullOrEmpty(model.BaseType) || model.ImplementedInterfaces.Count > 0)
            classDeclaration = classDeclaration.AddBaseListTypes(baseTypes.ToArray());

        return classDeclaration;
    }

    /// <summary>
    /// Converts a <see cref="PropertyDeclarationModel"/> to a Roslyn <see cref="PropertyDeclarationSyntax"/>.
    /// </summary>
    /// <param name="prop"></param>
    /// <returns></returns>
    public PropertyDeclarationSyntax ToSyntax(PropertyDeclarationModel prop)
    {
        var modifiers = prop.Modifiers.Select(SyntaxFactory.ParseToken).ToArray();
        var accessors = new List<AccessorDeclarationSyntax>();
        if (prop.HasGetter)
            accessors.Add(SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)));
        if (prop.HasSetter)
            accessors.Add(SyntaxFactory.AccessorDeclaration(SyntaxKind.SetAccessorDeclaration)
                .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)));

        var property = SyntaxFactory.PropertyDeclaration(
                SyntaxFactory.ParseTypeName(prop.Type),
                SyntaxFactory.Identifier(prop.Name))
            .AddModifiers(modifiers)
            .AddAccessorListAccessors(accessors.ToArray());

        if (prop.Initializer != null)
            property = property.WithInitializer(
                SyntaxFactory.EqualsValueClause(SyntaxFactory.ParseExpression(prop.Initializer)))
                .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));

        return property;
    }

    /// <summary>
    /// Converts a <see cref="InterfaceDeclarationModel"/> to a Roslyn <see cref="InterfaceDeclarationSyntax"/>.
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    public InterfaceDeclarationSyntax ToSyntax(InterfaceDeclarationModel model)
    {
        var modifiers = model.Modifiers.Select((x) => SyntaxFactory.ParseToken(x.ToString())).ToArray();
        var attributes = SyntaxFactory.List(model.Attributes.Select(attr =>
            SyntaxFactory.AttributeList(
                SyntaxFactory.SingletonSeparatedList(
                    SyntaxFactory.Attribute(SyntaxFactory.IdentifierName(attr.Name))
                )
            )
        ));

        var baseTypes = model.BaseInterfaces
            .Select(i => SyntaxFactory.SimpleBaseType(SyntaxFactory.ParseTypeName(i)))
            .ToArray();

        var members = new List<MemberDeclarationSyntax>();
        members.AddRange(model.Properties.Select(ToSyntax));
        members.AddRange(model.Methods.Select(ToSyntax));
        members.AddRange(model.Events.Select(ToSyntax));
        members.AddRange(model.NestedInterfaces.Select(ToSyntax));

        return SyntaxFactory.InterfaceDeclaration(model.Name)
            .AddModifiers(modifiers)
            .AddAttributeLists(attributes.ToArray())
            .AddBaseListTypes(baseTypes)
            .AddMembers(members.ToArray());
    }

    /// <summary>
    /// Converts an <see cref="EventModel"/> to a Roslyn <see cref="EventDeclarationSyntax"/>.
    /// </summary>
    /// <param name="evt"></param>
    /// <returns></returns>
    public EventDeclarationSyntax ToSyntax(EventModel evt)
    {
        var modifiers = evt.Modifiers.Select(SyntaxFactory.ParseToken).ToArray();

        var attributes = SyntaxFactory.List(evt.Attributes.Select(attr =>
            SyntaxFactory.AttributeList(
                SyntaxFactory.SingletonSeparatedList(
                    SyntaxFactory.Attribute(SyntaxFactory.IdentifierName(attr.Name))
                )
            )
        ));

        var eventDeclaration = SyntaxFactory.EventDeclaration(
                SyntaxFactory.ParseTypeName(evt.Type),
                SyntaxFactory.Identifier(evt.Name))
            .AddModifiers(modifiers)
            .AddAttributeLists(attributes.ToArray())
            .AddAccessorListAccessors(
                SyntaxFactory.AccessorDeclaration(SyntaxKind.AddAccessorDeclaration)
                    .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)),
                SyntaxFactory.AccessorDeclaration(SyntaxKind.RemoveAccessorDeclaration)
                    .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken))
            );

        return eventDeclaration;
    }

    /// <summary>
    /// Converts an <see cref="EnumDeclarationModel"/> to a Roslyn <see cref="EnumDeclarationSyntax"/>.
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    public EnumDeclarationSyntax ToSyntax(EnumDeclarationModel model)
    {
        var modifiers = model.Modifiers.Select((x) => SyntaxFactory.ParseToken(x.ToString())).ToArray();
        var attributes = SyntaxFactory.List(model.Attributes.Select(attr =>
            SyntaxFactory.AttributeList(
                SyntaxFactory.SingletonSeparatedList(
                    SyntaxFactory.Attribute(SyntaxFactory.IdentifierName(attr.Name))
                )
            )
        ));

        var members = model.Members.Select(m =>
            SyntaxFactory.EnumMemberDeclaration(m.Name)
                .WithEqualsValue(m.Value != null
                    ? SyntaxFactory.EqualsValueClause(SyntaxFactory.ParseExpression(m.Value))
                    : null)
        ).ToArray();

        var enumDecl = SyntaxFactory.EnumDeclaration(model.Name)
            .AddModifiers(modifiers)
            .AddAttributeLists(attributes.ToArray())
            .AddMembers(members);

        if (!string.IsNullOrWhiteSpace(model.UnderlyingType))
            enumDecl = enumDecl.WithBaseList(
                SyntaxFactory.BaseList(
                    SyntaxFactory.SingletonSeparatedList<BaseTypeSyntax>(
                        SyntaxFactory.SimpleBaseType(SyntaxFactory.ParseTypeName(model.UnderlyingType))
                    )
                )
            );

        return enumDecl;
    }

    /// <summary>
    /// Converts a <see cref="StructDeclarationModel"/> to a <see cref="StructDeclarationSyntax"/> representation.
    /// </summary>
    /// <remarks>This method generates a syntax node for a structure declaration using the information
    /// provided in the <paramref name="model"/>. The modifiers and attributes are parsed and applied to the resulting
    /// syntax node.</remarks>
    /// <param name="model">The model representing the structure declaration, including its name, modifiers, and attributes.</param>
    /// <returns>A <see cref="StructDeclarationSyntax"/> object that represents the structure declaration described by the
    /// provided <paramref name="model"/>.</returns>
    public StructDeclarationSyntax ToSyntax(StructDeclarationModel model)
    {
        var modifiers = model.Modifiers.Select((x) => SyntaxFactory.ParseToken(x.ToString())).ToArray();
        var attributes = SyntaxFactory.List(model.Attributes.Select(attr =>
            SyntaxFactory.AttributeList(
                SyntaxFactory.SingletonSeparatedList(
                    SyntaxFactory.Attribute(SyntaxFactory.IdentifierName(attr.Name))
                )
            )
        ));

        return SyntaxFactory.StructDeclaration(model.Name)
            .AddModifiers(modifiers)
            .AddAttributeLists(attributes.ToArray());
    }


    /// <summary>
    /// Converts a <see cref="ConstructorDeclarationModel"/> to a Roslyn <see cref="ConstructorDeclarationSyntax"/>.
    /// </summary>
    /// <param name="ctor"></param>
    /// <returns></returns>
    public ConstructorDeclarationSyntax ToSyntax(ConstructorDeclarationModel ctor)
    {
        var modifiers = ctor.Modifiers.Select(SyntaxFactory.ParseToken).ToArray();

        var parameters = SyntaxFactory.ParameterList(
            SyntaxFactory.SeparatedList(
                ctor.Parameters.Select(p =>
                    SyntaxFactory.Parameter(SyntaxFactory.Identifier(p.Name))
                        .WithType(SyntaxFactory.ParseTypeName(p.Type))
                        .WithDefault(
                            p.DefaultValue != null
                                ? SyntaxFactory.EqualsValueClause(SyntaxFactory.ParseExpression(p.DefaultValue))
                                : null
                        )
                )
            )
        );

        ConstructorDeclarationSyntax ctorSyntax = SyntaxFactory.ConstructorDeclaration(ctor.Name)
            .AddModifiers(modifiers)
            .WithParameterList(parameters);

        ctorSyntax = !string.IsNullOrWhiteSpace(ctor.Body)
            ? ctorSyntax.WithBody(
                SyntaxFactory.Block(
                    SyntaxFactory.ParseStatement(ctor.Body)
                )
            )
            : ctorSyntax.WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));

        return ctorSyntax;
    }

    /// <summary>
    /// Converts a <see cref="FieldDeclarationModel"/> to a Roslyn <see cref="FieldDeclarationSyntax"/> representation.
    /// </summary>
    /// <param name="field">The model representing the field declaration, including its name, type, and modifiers.</param>
    /// <returns>A <see cref="FieldDeclarationSyntax"/> object that represents the field declaration described by the
    /// provided <paramref name="field"/>.</returns>
    public FieldDeclarationSyntax ToSyntax(FieldDeclarationModel field)
    {
        var modifiers = field.Modifiers.Select(SyntaxFactory.ParseToken).ToArray();

        var variable = SyntaxFactory.VariableDeclarator(SyntaxFactory.Identifier(field.Name));
        if (!string.IsNullOrWhiteSpace(field.Initializer))
        {
            variable = variable.WithInitializer(
                SyntaxFactory.EqualsValueClause(SyntaxFactory.ParseExpression(field.Initializer))
            );
        }

        var declaration = SyntaxFactory.VariableDeclaration(SyntaxFactory.ParseTypeName(field.Type))
            .AddVariables(variable);

        var fieldDeclaration = SyntaxFactory.FieldDeclaration(declaration)
            .AddModifiers(modifiers);

        return fieldDeclaration;
    }

    /// <summary>
    /// Converts a <see cref="MethodDeclarationModel"/> into a Roslyn <see cref="MethodDeclarationSyntax"/>
    /// representation.
    /// </summary>
    /// <remarks>The returned syntax node includes the method's modifiers, attributes, parameters, and body
    /// (if provided). If the method body is not specified, the method will be generated as a semicolon-terminated
    /// declaration.</remarks>
    /// <param name="method">The model representing the method, including its name, return type, modifiers, attributes, parameters, and body.</param>
    /// <returns>A <see cref="MethodDeclarationSyntax"/> object representing the method declaration in Roslyn's syntax tree.</returns>
    public MethodDeclarationSyntax ToSyntax(MethodDeclarationModel method)
    {
        var modifiers = method.Modifiers.Select(SyntaxFactory.ParseToken).ToArray();

        var attributes = SyntaxFactory.List(method.Attributes.Select(attr =>
            SyntaxFactory.AttributeList(
                SyntaxFactory.SingletonSeparatedList(
                    SyntaxFactory.Attribute(SyntaxFactory.IdentifierName(attr.Name))
                )
            )
        ));

        var parameters = SyntaxFactory.ParameterList(
            SyntaxFactory.SeparatedList(
                method.Parameters.Select(p =>
                    SyntaxFactory.Parameter(SyntaxFactory.Identifier(p.Name))
                        .WithType(SyntaxFactory.ParseTypeName(p.Type))
                        .WithDefault(
                            p.DefaultValue != null
                                ? SyntaxFactory.EqualsValueClause(SyntaxFactory.ParseExpression(p.DefaultValue))
                                : null
                        )
                )
            )
        );

        MethodDeclarationSyntax methodSyntax = SyntaxFactory.MethodDeclaration(
                SyntaxFactory.ParseTypeName(method.ReturnType),
                SyntaxFactory.Identifier(method.Name))
            .AddModifiers(modifiers)
            .AddAttributeLists(attributes.ToArray())
            .WithParameterList(parameters);

        if (!string.IsNullOrWhiteSpace(method.Body))
        {
            methodSyntax = methodSyntax.WithBody(
                SyntaxFactory.Block(
                    SyntaxFactory.ParseStatement(method.Body)
                )
            );
        }
        else
        {
            methodSyntax = methodSyntax.WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));
        }

        return methodSyntax;
    }

    /// <summary>
    /// Converts a <see cref="ClassModifier"/> value to its corresponding string representation.
    /// </summary>
    /// <param name="modifier">The <see cref="ClassModifier"/> value to convert.</param>
    /// <returns>A string representing the specified <see cref="ClassModifier"/>. For example,  <see
    /// cref="ClassModifier.Public"/> is converted to "public".</returns>
    /// <exception cref="NotSupportedException">Thrown if the specified <paramref name="modifier"/> is not a recognized <see cref="ClassModifier"/> value.</exception>
    public string ToModifierString(ClassModifier modifier)
    {
        return modifier switch
        {
            ClassModifier.Public => "public",
            ClassModifier.Internal => "internal",
            ClassModifier.Abstract => "abstract",
            ClassModifier.Sealed => "sealed",
            ClassModifier.Static => "static",
            _ => throw new NotSupportedException($"Modifier not supported: {modifier}")
        };
    }
}