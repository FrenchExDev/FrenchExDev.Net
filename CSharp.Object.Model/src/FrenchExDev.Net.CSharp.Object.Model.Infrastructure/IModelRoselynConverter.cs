using FrenchExDev.Net.CSharp.Object.Model.Abstractions;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace FrenchExDev.Net.CSharp.Object.Model.Infrastructure;

/// <summary>
/// Defines a converter interface for transforming <see cref="IDeclarationModel"/> objects into Roslyn <see cref="MemberDeclarationSyntax"/> syntax nodes.
/// Used to bridge custom code model abstractions with Roslyn's syntax tree for code generation and analysis.
/// </summary>
/// <remarks>
/// Implement this interface to provide conversion logic from your domain model to Roslyn syntax nodes, enabling integration with Roslyn-based tooling and code generation workflows.
/// </remarks>
/// <example>
/// Example implementation:
/// <code>
/// public class MyModelRoselynConverter : IModelRoselynConverter
/// {
///     public MemberDeclarationSyntax Convert(IDeclarationModel model)
///     {
///         // Example: convert an InterfaceDeclarationModel to an InterfaceDeclarationSyntax
///         if (model is InterfaceDeclarationModel interfaceModel)
///         {
///             return SyntaxFactory.InterfaceDeclaration(interfaceModel.Name);
///         }
///         throw new NotSupportedException($"Model type {model.GetType().Name} not supported.");
///     }
/// }
/// </code>
/// </example>
public interface IModelRoselynConverter
{
    MemberDeclarationSyntax ToSyntax(IDeclarationModel model);
    NamespaceDeclarationSyntax ToSyntax(NamespaceDeclarationModel model);
    ClassDeclarationSyntax ToSyntax(ClassDeclarationModel model);
    PropertyDeclarationSyntax ToSyntax(PropertyDeclarationModel model);
    InterfaceDeclarationSyntax ToSyntax(InterfaceDeclarationModel model);
    EnumDeclarationSyntax ToSyntax(EnumDeclarationModel model);
    ConstructorDeclarationSyntax ToSyntax(ConstructorDeclarationModel model);
    EventDeclarationSyntax ToSyntax(EventModel evt);

}
