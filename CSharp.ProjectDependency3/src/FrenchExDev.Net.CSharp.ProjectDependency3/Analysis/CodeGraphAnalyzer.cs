using System.Text.Json;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace FrenchExDev.Net.CSharp.ProjectDependency3.Analysis;

/// <summary>
/// Analyzes a project solution to generate a code graph representing type relationships, usages, and project structure.
/// </summary>
/// <remarks>The code graph produced by this analyzer includes nodes for projects and types, as well as links that
/// describe containment, inheritance, interface implementation, and usage relationships. This analyzer is useful for
/// visualizing and understanding the dependencies and structure within a codebase. Thread safety is not guaranteed;
/// concurrent calls should be externally synchronized if required.</remarks>
public class CodeGraphAnalyzer : IProjectAnalyzer
{
    public string Name => "CodeGraph";

    /// <summary>
    /// Analyzes the specified solution and produces a project-level code graph representing type relationships and
    /// symbol usages.
    /// </summary>
    /// <remarks>The analysis includes type containment, inheritance, interface implementation, and symbol
    /// usage relationships across all projects in the solution. The returned graph can be used for visualization or
    /// further inspection of code dependencies.</remarks>
    /// <param name="solution">The solution to analyze. Must contain valid project and code information.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the analysis operation.</param>
    /// <returns>An object containing the results of the project analysis, including a graph of projects, types, and their
    /// relationships.</returns>
    public async Task<IProjectAnalysisResult> AnalyzeAsync(Solution solution, CancellationToken cancellationToken = default)
    {
        var model = new GraphModel();
        var roslynSolution = solution.Code;
        var projectNodes = new Dictionary<ProjectId, GraphNode>();

        // Create project nodes
        foreach (var p in roslynSolution.Projects)
        {
            var node = new GraphNode { Id = p.Id.Id.ToString(), Name = p.Name, Kind = "Project" };
            model.Nodes.Add(node);
            projectNodes[p.Id] = node;
        }

        // Walk symbols per project
        foreach (var p in roslynSolution.Projects)
        {
            var comp = await p.GetCompilationAsync(cancellationToken).ConfigureAwait(false);
            if (comp is null) continue;
            var projectId = p.Id.Id.ToString();

            foreach (var tree in comp.SyntaxTrees)
            {
                var semantic = comp.GetSemanticModel(tree);
                var root = await tree.GetRootAsync(cancellationToken).ConfigureAwait(false);

                foreach (var decl in root.DescendantNodes().OfType<BaseTypeDeclarationSyntax>())
                {
                    var symbol = semantic.GetDeclaredSymbol(decl, cancellationToken);
                    if (symbol is null) continue;
                    var kind = SymbolKindToNodeKind(symbol);
                    var id = symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
                    var node = EnsureNode(model, id, symbol.Name, kind, projectId);
                    // containment link: project contains type
                    model.Links.Add(new GraphLink { SourceId = projectId, TargetId = id, Kind = "Contains" });

                    // Base types / interfaces
                    foreach (var baseType in symbol.Interfaces)
                    {
                        var bid = baseType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
                        EnsureNode(model, bid, baseType.Name, SymbolKindToNodeKind(baseType), projectId);
                        model.Links.Add(new GraphLink { SourceId = id, TargetId = bid, Kind = "Implements" });
                    }
                    if (symbol.BaseType is { } bt && bt.SpecialType == SpecialType.None)
                    {
                        var bid = bt.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
                        EnsureNode(model, bid, bt.Name, SymbolKindToNodeKind(bt), projectId);
                        model.Links.Add(new GraphLink { SourceId = id, TargetId = bid, Kind = "Inherits" });
                    }
                }

                // Usages: simple pass on IdentifierName/Invocation/ObjectCreation
                foreach (var node in root.DescendantNodes())
                {
                    ISymbol? sym = node switch
                    {
                        InvocationExpressionSyntax inv => semantic.GetSymbolInfo(inv.Expression, cancellationToken).Symbol,
                        ObjectCreationExpressionSyntax oc => semantic.GetSymbolInfo(oc.Type, cancellationToken).Symbol,
                        IdentifierNameSyntax idn => semantic.GetSymbolInfo(idn, cancellationToken).Symbol,
                        _ => null
                    };
                    if (sym is null) continue;
                    var owner = sym.ContainingType ?? sym;
                    var targetId = owner.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
                    var targetKind = SymbolKindToNodeKind(owner);
                    EnsureNode(model, targetId, owner.Name, targetKind, projectId);
                    // link from file/project scope to target; we use project as source for coarse granularity
                    model.Links.Add(new GraphLink { SourceId = projectId, TargetId = targetId, Kind = "Usage" });
                }
            }
        }

        return new CodeGraphResult(Name, model);
    }

    private static string SymbolKindToNodeKind(ISymbol symbol)
    {
        return symbol switch
        {
            ITypeSymbol ts when ts.TypeKind == TypeKind.Interface => "Interface",
            ITypeSymbol ts when ts.TypeKind == TypeKind.Enum => "Enum",
            ITypeSymbol ts when ts.TypeKind == TypeKind.Struct => "Struct",
            ITypeSymbol ts when ts.TypeKind == TypeKind.Class && ts.IsRecord => "Record",
            ITypeSymbol ts when ts.TypeKind == TypeKind.Class => "Class",
            INamespaceSymbol => "Namespace",
            _ => symbol.Kind.ToString()
        };
    }

    private static GraphNode EnsureNode(GraphModel model, string id, string name, string kind, string? parentId = null)
    {
        var existing = model.Nodes.FirstOrDefault(n => n.Id == id);
        if (existing is not null) return existing;
        var node = new GraphNode { Id = id, Name = name, Kind = kind, ParentId = parentId };
        model.Nodes.Add(node);
        return node;
    }
}
