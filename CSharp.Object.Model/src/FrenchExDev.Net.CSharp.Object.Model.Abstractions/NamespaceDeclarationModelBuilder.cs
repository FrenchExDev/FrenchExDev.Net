using FrenchExDev.Net.CSharp.Object.Builder;
using FrenchExDev.Net.CSharp.Object.Builder.Abstractions;

namespace FrenchExDev.Net.CSharp.Object.Model.Abstractions;

/// <summary>
/// Builder class for constructing <see cref="NamespaceDeclarationModel"/> instances.
/// Provides a fluent API to configure the namespace name, interfaces, classes, enums, structs, and nested namespaces for code generation scenarios.
/// Validates required properties and produces a build result indicating success or failure.
/// </summary>
/// <remarks>
/// Example usage:
/// <code>
/// var builder = new NamespaceDeclarationModelBuilder()
///     .Name("MyCompany.MyProduct.Core")
///     .Class(c => c.Name("MyClass"))
///     .Enum(e => e.Name("MyEnum"));
/// var result = builder.Build();
/// </code>
/// </remarks>
public class NamespaceDeclarationModelBuilder : AbstractObjectBuilder<NamespaceDeclarationModel, NamespaceDeclarationModelBuilder>
{
    /// <summary>
    /// Stores the name of the namespace to be built.
    /// </summary>
    private string? _name;

    /// <summary>
    /// Represents the scoping configuration for the namespace. 
    /// </summary>
    /// <remarks>This field determines the scope in which the namespace operates.  It may be null if no
    /// specific scoping is defined.</remarks>
    private NamespaceScoping? _scoping;

    /// <summary>
    /// Stores the list of interface builders for the namespace.
    /// </summary>
    private readonly List<InterfaceDeclarationModelBuilder> _interfaces = new();

    /// <summary>
    /// Stores the list of class builders for the namespace.
    /// </summary>
    private readonly List<ClassDeclarationModelBuilder> _classes = new();

    /// <summary>
    /// Stores the list of enum builders for the namespace.
    /// </summary>
    private readonly List<EnumDeclarationModelBuilder> _enums = new();

    /// <summary>
    /// Stores the list of struct builders for the namespace.
    /// </summary>
    private readonly List<StructDeclarationModelBuilder> _structs = new();

    /// <summary>
    /// Stores the list of nested namespace builders for the namespace.
    /// </summary>
    private readonly List<NamespaceDeclarationModelBuilder> _nestedNamespaces = new();

    /// <summary>
    /// Sets the name of the namespace.
    /// </summary>
    /// <param name="name">The namespace name (e.g., "System.Collections.Generic").</param>
    /// <returns>The current builder instance.</returns>
    /// <example>
    /// builder.Name("MyCompany.MyProduct.Core");
    /// </example>
    public NamespaceDeclarationModelBuilder Name(string name)
    {
        _name = name;
        return this;
    }

    /// <summary>
    /// Sets the namespace scoping to file-scoped.
    /// </summary>
    /// <returns>The current builder instance.</returns>
    public NamespaceDeclarationModelBuilder FileScoped()
    {
        _scoping = NamespaceScoping.File;
        return this;
    }

    /// <summary>
    /// Sets the namespace scoping to name-scoped (project/assembly level).
    /// </summary>
    /// <returns>The current builder instance.</returns>
    public NamespaceDeclarationModelBuilder NameScoped()
    {
        _scoping = NamespaceScoping.Name;
        return this;
    }

    /// <summary>
    /// Adds an interface to the namespace using a builder action.
    /// </summary>
    /// <param name="interfaceBuilder">An action to configure the interface builder.</param>
    /// <returns>The current builder instance.</returns>
    /// <example>
    /// builder.Interface(i => i.Name("IMyInterface"));
    /// </example>
    public NamespaceDeclarationModelBuilder Interface(Action<InterfaceDeclarationModelBuilder> interfaceBuilder)
    {
        var builder = new InterfaceDeclarationModelBuilder();
        interfaceBuilder(builder);
        _interfaces.Add(builder);
        return this;
    }

    /// <summary>
    /// Adds a class to the namespace using a builder action.
    /// </summary>
    /// <param name="classBuilder">An action to configure the class builder.</param>
    /// <returns>The current builder instance.</returns>
    /// <example>
    /// builder.Class(c => c.Name("MyClass"));
    /// </example>
    public NamespaceDeclarationModelBuilder Class(Action<ClassDeclarationModelBuilder> classBuilder)
    {
        var builder = new ClassDeclarationModelBuilder();
        classBuilder(builder);
        _classes.Add(builder);
        return this;
    }

    /// <summary>
    /// Adds an enum to the namespace using a builder action.
    /// </summary>
    /// <param name="enumBuilder">An action to configure the enum builder.</param>
    /// <returns>The current builder instance.</returns>
    /// <example>
    /// builder.Enum(e => e.Name("MyEnum"));
    /// </example>
    public NamespaceDeclarationModelBuilder Enum(Action<EnumDeclarationModelBuilder> enumBuilder)
    {
        var builder = new EnumDeclarationModelBuilder();
        enumBuilder(builder);
        _enums.Add(builder);
        return this;
    }

    /// <summary>
    /// Adds a struct to the namespace using a builder action.
    /// </summary>
    /// <param name="structBuilder">An action to configure the struct builder.</param>
    /// <returns>The current builder instance.</returns>
    /// <example>
    /// builder.Struct(s => s.Name("MyStruct"));
    /// </example>
    public NamespaceDeclarationModelBuilder Struct(Action<StructDeclarationModelBuilder> structBuilder)
    {
        var builder = new StructDeclarationModelBuilder();
        structBuilder(builder);
        _structs.Add(builder);
        return this;
    }

    /// <summary>
    /// Adds a nested namespace to the namespace using a builder action.
    /// </summary>
    /// <param name="namespaceBuilder">An action to configure the nested namespace builder.</param>
    /// <returns>The current builder instance.</returns>
    /// <example>
    /// builder.NestedNamespace(n => n.Name("MyCompany.MyProduct.Core.SubNamespace"));
    /// </example>
    public NamespaceDeclarationModelBuilder NestedNamespace(Action<NamespaceDeclarationModelBuilder> namespaceBuilder)
    {
        var builder = new NamespaceDeclarationModelBuilder();
        namespaceBuilder(builder);
        _nestedNamespaces.Add(builder);
        return this;
    }

    /// <summary>
    /// Builds the <see cref="NamespaceDeclarationModel"/> instance, validating required properties and collecting build errors from nested builders.
    /// </summary>
    /// <param name="exceptions">A list to collect build exceptions.</param>
    /// <param name="visited">A list of visited objects for cycle detection.</param>
    /// <returns>A build result containing either the constructed model or failure details.</returns>
    /// <remarks>
    /// This method builds all nested elements (interfaces, classes, enums, structs, and nested namespaces) and aggregates any exceptions.
    /// If the namespace name is not provided or any nested builder fails, a failure result is returned.
    /// </remarks>
    protected override IObjectBuildResult<NamespaceDeclarationModel> BuildInternal(ExceptionBuildList exceptions, VisitedObjectsList visited)
    {
        var interfaces = BuildList<InterfaceDeclarationModel, InterfaceDeclarationModelBuilder>(_interfaces, visited);
        var enums = BuildList<EnumDeclarationModel, EnumDeclarationModelBuilder>(_enums, visited);
        var classes = BuildList<ClassDeclarationModel, ClassDeclarationModelBuilder>(_classes, visited);
        if (string.IsNullOrEmpty(_name))
        {
            exceptions.Add(new InvalidOperationException("Namespace name must be provided."));
        }
        var structs = BuildList<StructDeclarationModel, StructDeclarationModelBuilder>(_structs, visited);
        var nestedNamespaces = BuildList<NamespaceDeclarationModel, NamespaceDeclarationModelBuilder>(_nestedNamespaces, visited);

        AddExceptions<NamespaceDeclarationModel, NamespaceDeclarationModelBuilder>(nestedNamespaces, exceptions);
        AddExceptions<StructDeclarationModel, StructDeclarationModelBuilder>(structs, exceptions);
        AddExceptions<EnumDeclarationModel, EnumDeclarationModelBuilder>(enums, exceptions);
        AddExceptions<ClassDeclarationModel, ClassDeclarationModelBuilder>(classes, exceptions);
        AddExceptions<InterfaceDeclarationModel, InterfaceDeclarationModelBuilder>(interfaces, exceptions);

        // Return failure if any exceptions were collected
        if (exceptions.Any())
        {
            return Failure(exceptions, visited);
        }

        // Ensure the namespace name is not null before proceeding
        ArgumentNullException.ThrowIfNull(_name);

        // Return a successful build result with the constructed NamespaceDeclarationModel
        return Success(new NamespaceDeclarationModel
        {
            Name = _name,
            Scoping = _scoping,
            Interfaces = interfaces.ToResultList(),
            Classes = classes.ToResultList(),
            Enums = enums.ToResultList(),
            Structs = structs.ToResultList(),
            NestedNamespaces = nestedNamespaces.ToResultList()
        });
    }
}
