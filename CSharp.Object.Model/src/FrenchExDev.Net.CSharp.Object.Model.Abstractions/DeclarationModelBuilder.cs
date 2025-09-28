using FrenchExDev.Net.CSharp.Object.Builder2;

namespace FrenchExDev.Net.CSharp.Object.Model.Abstractions;

/// <summary>
/// Builder for constructing declaration models (namespace, class, struct, enum, interface, attribute).
/// Provides a fluent API to select and configure the type of declaration to build.
/// </summary>
public class DeclarationModelBuilder : IBuilder<IDeclarationModel>
{
    /// <summary>
    /// Holds the Id
    /// </summary>
    private Guid _id = Guid.NewGuid();

    /// <summary>
    /// Gets a new unique identifier for this builder instance.
    /// <remarks>Each access returns a new Guid; not stable for tracking.</remarks>
    /// </summary>
    public Guid Id => _id;

    /// <summary>
    /// Holds the result of the build operation, if any.
    /// <remarks>Null until build is performed.</remarks>
    /// </summary>
    private IResult? _result;

    /// <summary>
    /// The underlying builder for the specific declaration type being constructed.
    /// <remarks>Set when a specific declaration method is called (e.g., Class(), Enum()).</remarks>
    /// </summary>
    private IDeclarationModelBuilder? _builder;

    /// <summary>
    /// Gets the current declaration model builder used to configure the type definition.
    /// </summary>
    /// <remarks>Accessing this property before configuring a declaration type, such as by calling Class(),
    /// Enum(), or a similar method, will result in an exception. This property is typically used to further customize
    /// the declaration after its type has been set.</remarks>
    public IDeclarationModelBuilder Builder => _builder ?? throw new InvalidOperationException("No declaration type has been configured. Call a method like Class(), Enum(), etc. to set the builder.");

    /// <summary>
    /// Gets the result of the build operation, if available.
    /// </summary>
    public IResult? Result => _result;

    /// <summary>
    /// Gets the current status of the build process.
    /// </summary>
    public BuildStatus BuildStatus
    {
        get
        {
            return _builder switch
            {
                AttributeDeclarationModelBuilder n => n.BuildStatus,
                ClassDeclarationModelBuilder n => n.BuildStatus,
                ConstructorDeclarationModelBuilder n => n.BuildStatus,
                EnumDeclarationModelBuilder n => n.BuildStatus,
                EnumMemberDeclarationModelBuilder n => n.BuildStatus,
                EventDeclarationModelBuilder n => n.BuildStatus,
                FieldDeclarationModelBuilder n => n.BuildStatus,
                FreeTypeParameterConstraintDeclarationModelBuilder n => n.BuildStatus,
                InterfaceDeclarationModelBuilder n => n.BuildStatus,
                NamespaceDeclarationModelBuilder n => n.BuildStatus,
                StructDeclarationModelBuilder n => n.BuildStatus,
                _ => throw new NotSupportedException(_builder?.GetType().FullName)
            };
        }
    }

    /// <summary>
    /// Gets the current validation status for the associated object or operation.
    /// </summary>
    public ValidationStatus ValidationStatus
    {
        get
        {
            return _builder switch
            {
                AttributeDeclarationModelBuilder n => n.ValidationStatus,
                ClassDeclarationModelBuilder n => n.ValidationStatus,
                ConstructorDeclarationModelBuilder n => n.ValidationStatus,
                EnumDeclarationModelBuilder n => n.ValidationStatus,
                EnumMemberDeclarationModelBuilder n => n.ValidationStatus,
                EventDeclarationModelBuilder n => n.ValidationStatus,
                FieldDeclarationModelBuilder n => n.ValidationStatus,
                FreeTypeParameterConstraintDeclarationModelBuilder n => n.ValidationStatus,
                InterfaceDeclarationModelBuilder n => n.ValidationStatus,
                NamespaceDeclarationModelBuilder n => n.ValidationStatus,
                StructDeclarationModelBuilder n => n.ValidationStatus,
                _ => throw new NotSupportedException(_builder?.GetType().FullName)
            };
        }
    }

    /// <summary>
    /// List of hooks to execute after the declaration model is built.
    /// <remarks>Hooks allow custom post-build logic.</remarks>
    /// </summary>
    private readonly List<Action<IDeclarationModel>> _onBuiltHooks = [];

    /// <summary>
    /// Stores an existing declaration model instance, if provided.
    /// </summary>
    private IDeclarationModel? _existing;

    /// <summary>
    /// Returns a reference to the built declaration model.
    /// <remarks>Not implemented; should provide a reference wrapper for the built model.</remarks>
    /// </summary>
    public Reference<IDeclarationModel> Reference()
    {
        ArgumentNullException.ThrowIfNull(_builder, "No declaration type has been configured. Call a method like Class(), Enum(), etc. to set the builder.");

        return _builder switch
        {
            AttributeDeclarationModelBuilder n => (Reference<IDeclarationModel>)(object)n.Reference(),
            ClassDeclarationModelBuilder n => (Reference<IDeclarationModel>)(object)n.Reference(),
            ConstructorDeclarationModelBuilder n => (Reference<IDeclarationModel>)(object)n.Reference(),
            EnumDeclarationModelBuilder n => (Reference<IDeclarationModel>)(object)n.Reference(),
            EnumMemberDeclarationModelBuilder n => (Reference<IDeclarationModel>)(object)n.Reference(),
            EventDeclarationModelBuilder n => (Reference<IDeclarationModel>)(object)n.Reference(),
            FieldDeclarationModelBuilder n => (Reference<IDeclarationModel>)(object)n.Reference(),
            FreeTypeParameterConstraintDeclarationModelBuilder n => (Reference<IDeclarationModel>)(object)n.Reference(),
            InterfaceDeclarationModelBuilder n => (Reference<IDeclarationModel>)(object)n.Reference(),
            NamespaceDeclarationModelBuilder n => (Reference<IDeclarationModel>)(object)n.Reference(),
            StructDeclarationModelBuilder n => (Reference<IDeclarationModel>)(object)n.Reference(),
            _ => throw new NotSupportedException(_builder.GetType().FullName)
        };
    }

    /// <summary>
    /// Builds the result for the current declaration model, optionally tracking visited objects to prevent redundant
    /// processing.
    /// </summary>
    /// <param name="visitedCollector">An optional dictionary used to track objects that have already been visited during the build process. If
    /// provided, it helps avoid processing the same object multiple times.</param>
    /// <returns>An object implementing <see cref="IResult"/> that represents the outcome of the build operation. The result
    /// may indicate success or failure depending on the state of the declaration model.</returns>
    public IResult Build(VisitedObjectDictionary? visitedCollector = null)
    {
        if (_result is not null) return _result;
        _result = BuildInternal(visitedCollector);
        foreach (var hook in _onBuiltHooks)
        {
            if (_result is SuccessResult<IDeclarationModel> success)
            {
                hook(success.Object);
            }
        }
        return _result;
    }

    /// <summary>
    /// Builds the declaration model using the configured builder.
    /// <remarks>Not implemented; should invoke the build process and store the result.</remarks>
    /// </summary>
    /// <param name="visitedCollector">Optional dictionary for tracking visited objects to prevent cycles.</param>
    /// <returns>The build result, or null if not built.</returns>
    protected IResult BuildInternal(VisitedObjectDictionary? visitedCollector = null)
    {
        if (_existing is not null) return new SuccessResult<IDeclarationModel>(_existing);

        ArgumentNullException.ThrowIfNull(_builder, "No declaration type has been configured. Call a method like Class(), Enum(), etc. to set the builder.");

        return _builder switch
        {
            AttributeDeclarationModelBuilder n => (Reference<IDeclarationModel>)(object)n.Build(visitedCollector),
            ClassDeclarationModelBuilder n => (Reference<IDeclarationModel>)(object)n.Build(visitedCollector),
            ConstructorDeclarationModelBuilder n => (Reference<IDeclarationModel>)(object)n.Build(visitedCollector),
            EnumDeclarationModelBuilder n => (Reference<IDeclarationModel>)(object)n.Build(visitedCollector),
            EnumMemberDeclarationModelBuilder n => (Reference<IDeclarationModel>)(object)n.Build(visitedCollector),
            EventDeclarationModelBuilder n => (Reference<IDeclarationModel>)(object)n.Build(visitedCollector),
            FieldDeclarationModelBuilder n => (Reference<IDeclarationModel>)(object)n.Build(visitedCollector),
            FreeTypeParameterConstraintDeclarationModelBuilder n => (Reference<IDeclarationModel>)(object)n.Build(visitedCollector),
            InterfaceDeclarationModelBuilder n => (Reference<IDeclarationModel>)(object)n.Build(visitedCollector),
            NamespaceDeclarationModelBuilder n => (Reference<IDeclarationModel>)(object)n.Build(visitedCollector),
            StructDeclarationModelBuilder n => (Reference<IDeclarationModel>)(object)n.Build(visitedCollector),
            _ => throw new NotSupportedException(_builder.GetType().FullName)
        };
    }

    /// <summary>
    /// Registers a hook to be executed after the declaration model is built.
    /// <remarks>Useful for custom post-processing or validation.</remarks>
    /// </summary>
    /// <param name="hook">Action to execute with the built model.</param>
    public void OnBuilt(Action<IDeclarationModel> hook)
    {
        _onBuiltHooks.Add(hook);
    }

    /// <summary>
    /// Configures the builder to construct a namespace declaration.
    /// </summary>
    /// <returns>A builder for namespace declarations.</returns>
    public NamespaceDeclarationModelBuilder Namespace()
    {
        var n = new NamespaceDeclarationModelBuilder();
        _builder = n;
        return n;
    }

    /// <summary>
    /// Configures the builder to construct an interface declaration.
    /// </summary>
    /// <returns>A builder for interface declarations.</returns>
    public InterfaceDeclarationModelBuilder Interface()
    {
        var n = new InterfaceDeclarationModelBuilder();
        _builder = n;
        return n;
    }

    /// <summary>
    /// Configures the builder to construct a class declaration.
    /// </summary>
    /// <returns>A builder for class declarations.</returns>
    public ClassDeclarationModelBuilder Class()
    {
        var n = new ClassDeclarationModelBuilder();
        _builder = n;
        return n;
    }

    /// <summary>
    /// Configures the builder to construct a struct declaration.
    /// </summary>
    /// <returns>A builder for struct declarations.</returns>
    public StructDeclarationModelBuilder Struct()
    {
        var n = new StructDeclarationModelBuilder();
        _builder = n;
        return n;
    }

    /// <summary>
    /// Configures the builder to construct an enum declaration.
    /// </summary>
    /// <returns>A builder for enum declarations.</returns>
    public EnumDeclarationModelBuilder Enum()
    {
        var n = new EnumDeclarationModelBuilder();
        _builder = n;
        return n;

    }

    /// <summary>
    /// Configures the builder to construct an attribute declaration.
    /// </summary>
    /// <returns>A builder for attribute declarations.</returns>
    public AttributeDeclarationModelBuilder Attribute()
    {
        var n = new AttributeDeclarationModelBuilder();
        _builder = n;
        return n;
    }

    /// <summary>
    /// Validates the current declaration model using the configured builder and records any validation failures
    /// encountered.
    /// </summary>
    /// <remarks>This method delegates validation to the specific builder that was previously configured for
    /// the declaration type. Ensure that a builder has been set before calling this method; otherwise, an exception
    /// will be thrown.</remarks>
    /// <param name="visitedCollector">A dictionary used to track objects that have already been visited during validation to prevent redundant
    /// processing.</param>
    /// <param name="failures">A dictionary that collects validation failures detected during the validation process. Failures are added to
    /// this collection as they are found.</param>
    /// <exception cref="NotSupportedException">Thrown if the configured builder type is not supported for validation.</exception>
    public void Validate(VisitedObjectDictionary visitedCollector, FailuresDictionary failures)
    {
        ArgumentNullException.ThrowIfNull(_builder, "No declaration type has been configured. Call a method like Class(), Enum(), etc. to set the builder.");

        switch (_builder)
        {
            case AttributeDeclarationModelBuilder attributeDeclarationModelBuilder:
                attributeDeclarationModelBuilder.Validate(visitedCollector, failures);
                break;
            case ClassDeclarationModelBuilder classDeclarationModelBuilder:
                classDeclarationModelBuilder.Validate(visitedCollector, failures);
                break;
            case ConstructorDeclarationModelBuilder constructorDeclarationModelBuilder:
                constructorDeclarationModelBuilder.Validate(visitedCollector, failures);
                break;
            case EnumDeclarationModelBuilder enumDeclarationModelBuilder:
                enumDeclarationModelBuilder.Validate(visitedCollector, failures);
                break;
            case EnumMemberDeclarationModelBuilder enumMemberDeclarationModelBuilder:
                enumMemberDeclarationModelBuilder.Validate(visitedCollector, failures);
                break;
            case EventDeclarationModelBuilder eventDeclarationModelBuilder:
                eventDeclarationModelBuilder.Validate(visitedCollector, failures);
                break;
            case FieldDeclarationModelBuilder fieldDeclarationModelBuilder:
                fieldDeclarationModelBuilder.Validate(visitedCollector, failures);
                break;
            case FreeTypeParameterConstraintDeclarationModelBuilder freeTypeParameterConstraintDeclarationModelBuilder:
                freeTypeParameterConstraintDeclarationModelBuilder.Validate(visitedCollector, failures);
                break;
            case InterfaceDeclarationModelBuilder interfaceDeclarationModelBuilder:
                interfaceDeclarationModelBuilder.Validate(visitedCollector, failures);
                break;
            case NamespaceDeclarationModelBuilder namespaceDeclarationModelBuilder:
                namespaceDeclarationModelBuilder.Validate(visitedCollector, failures);
                break;
            case StructDeclarationModelBuilder structDeclarationModelBuilder:
                structDeclarationModelBuilder.Validate(visitedCollector, failures);
                break;
            default:
                throw new NotSupportedException(_builder.GetType().FullName);
        }
    }

    /// <summary>
    /// Configures the builder to use an existing declaration model instance.
    /// </summary>
    /// <param name="instance">The existing <see cref="IDeclarationModel"/> instance to be used by the builder. Cannot be null.</param>
    /// <returns>The current builder instance configured to use the specified declaration model.</returns>
    IBuilder<IDeclarationModel> IBuilder<IDeclarationModel>.Existing(IDeclarationModel instance)
    {
        _existing = instance;
        return this;
    }
}