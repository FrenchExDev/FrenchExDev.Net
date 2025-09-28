using FrenchExDev.Net.CSharp.Object.Builder2;

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
public class NamespaceDeclarationModelBuilder : AbstractBuilder<NamespaceDeclarationModel>, IDeclarationModelBuilder
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
    private readonly BuilderList<InterfaceDeclarationModel, InterfaceDeclarationModelBuilder> _interfaces = [];

    /// <summary>
    /// Stores the list of class builders for the namespace.
    /// </summary>
    private readonly BuilderList<ClassDeclarationModel, ClassDeclarationModelBuilder> _classes = [];

    /// <summary>
    /// Stores the list of enum builders for the namespace.
    /// </summary>
    private readonly BuilderList<EnumDeclarationModel, EnumDeclarationModelBuilder> _enums = [];

    /// <summary>
    /// Stores the list of struct builders for the namespace.
    /// </summary>
    private readonly BuilderList<StructDeclarationModel, StructDeclarationModelBuilder> _structs = [];

    /// <summary>
    /// Stores the list of nested namespace builders for the namespace.
    /// </summary>
    private readonly BuilderList<NamespaceDeclarationModel, NamespaceDeclarationModelBuilder> _nestedNamespaces = [];

    /// <summary>
    /// Sets the name of the namespace.
    /// </summary>
    /// <param name="name">The namespace name (e.g., "System.Collections.Generic").</param>
    /// <returns>The current builder instance.</returns>
    /// <example>
    /// builder.Name("MyCompany.MyProduct.Core");
    /// </example>
    public NamespaceDeclarationModelBuilder WithName(string name)
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
    public NamespaceDeclarationModelBuilder WithClass(Action<ClassDeclarationModelBuilder> classBuilder)
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
    /// Builds the internal representation of the namespace by processing its contained interfaces, classes, enums,
    /// structs, and nested namespaces.
    /// </summary>
    /// <remarks>This method is typically called as part of a recursive traversal when constructing a model of
    /// the namespace and its members. The provided collector should be reused across recursive calls to ensure
    /// consistency.</remarks>
    /// <param name="visitedCollector">A dictionary used to track objects that have already been visited during the build process. This helps prevent
    /// redundant processing and circular references.</param>
    protected override void BuildInternal(VisitedObjectDictionary visitedCollector)
    {
        BuildList(_interfaces, visitedCollector);
        BuildList(_classes, visitedCollector);
        BuildList(_enums, visitedCollector);
        BuildList(_structs, visitedCollector);
        BuildList(_nestedNamespaces, visitedCollector);
    }

    /// <summary>
    /// Performs validation logic specific to the current object and records any validation failures encountered.
    /// </summary>
    /// <param name="visitedCollector">A dictionary used to track objects that have already been visited during validation to prevent redundant checks
    /// and circular references.</param>
    /// <param name="failures">A dictionary for collecting validation failures, where each failure is recorded with its associated property
    /// name and exception details.</param>
    protected override void ValidateInternal(VisitedObjectDictionary visitedCollector, FailuresDictionary failures)
    {
        if (string.IsNullOrEmpty(_name))
        {
            failures.Failure(nameof(_name), new InvalidOperationException("Namespace name must be provided."));
        }

        if (_scoping is null)
        {
            failures.Failure(nameof(_scoping), new InvalidOperationException("Namespace scoping must be specified (file-scoped or name-scoped)."));
        }
    }

    /// <summary>
    /// Creates a new instance of the namespace declaration model using the configured name, scoping, and member
    /// collections.
    /// </summary>
    /// <returns>A <see cref="NamespaceDeclarationModel"/> representing the namespace with its associated scoping, interfaces,
    /// classes, enums, structs, and nested namespaces.</returns>
    /// <exception cref="MissingMemberException">Thrown if the scoping information has not been set prior to instantiation.</exception>
    protected override NamespaceDeclarationModel Instantiate()
    {
        ArgumentNullException.ThrowIfNull(_name);

        return new(
            name: _name,
            scoping: _scoping ?? throw new MissingMemberException(),
            interfaces: _interfaces.AsReferenceList(),
            classes: _classes.AsReferenceList(),
            enums: _enums.AsReferenceList(),
            structs: _structs.AsReferenceList(),
            nestedNamespaces: _nestedNamespaces.AsReferenceList()
        );
    }
}
