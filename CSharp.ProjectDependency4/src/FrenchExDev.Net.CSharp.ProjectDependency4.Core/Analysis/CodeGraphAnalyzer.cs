using System.Text.Json;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace FrenchExDev.Net.CSharp.ProjectDependency4.Core.Analysis;

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

        // Create project nodes (always internal)
        var projectNodes = new Dictionary<ProjectId, GraphNode>();
        foreach (var p in roslynSolution.Projects)
        {
            var node = new GraphNode { Id = p.Id.Id.ToString(), Name = p.Name, Kind = "Project" };
            model.Nodes.Add(node);
            projectNodes[p.Id] = node;
        }

        // First pass: gather all declared types across the whole solution (internal elements only)
        var declared = new Dictionary<string, (string Name, string Kind, string ProjectNodeId)>(StringComparer.Ordinal);
        foreach (var p in roslynSolution.Projects)
        {
            var comp = await p.GetCompilationAsync(cancellationToken).ConfigureAwait(false);
            if (comp is null) continue;
            var projectNodeId = p.Id.Id.ToString();
            foreach (var tree in comp.SyntaxTrees)
            {
                var semantic = comp.GetSemanticModel(tree);
                var root = await tree.GetRootAsync(cancellationToken).ConfigureAwait(false);
                foreach (var decl in root.DescendantNodes().OfType<BaseTypeDeclarationSyntax>())
                {
                    var symbol = semantic.GetDeclaredSymbol(decl, cancellationToken) as ITypeSymbol;
                    if (symbol is null) continue;
                    var id = symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
                    var kind = SymbolKindToNodeKind(symbol);
                    declared[id] = (symbol.Name, kind, projectNodeId);
                }
            }
        }

        // Add internal type nodes and containment links
        var nodesById = new Dictionary<string, GraphNode>(StringComparer.Ordinal);
        foreach (var (id, info) in declared)
        {
            var node = new GraphNode { Id = id, Name = info.Name, Kind = info.Kind, ParentId = info.ProjectNodeId };
            model.Nodes.Add(node);
            nodesById[id] = node;
            model.Links.Add(new GraphLink { SourceId = info.ProjectNodeId, TargetId = id, Kind = "Contains" });
        }

        // Dedup set for links
        var linkSet = new HashSet<(string s, string t, string k)>();

        // Second pass: relationships (only between internal types)
        foreach (var p in roslynSolution.Projects)
        {
            var comp = await p.GetCompilationAsync(cancellationToken).ConfigureAwait(false);
            if (comp is null) continue;
            var projectNodeId = p.Id.Id.ToString();

            foreach (var tree in comp.SyntaxTrees)
            {
                var semantic = comp.GetSemanticModel(tree);
                var root = await tree.GetRootAsync(cancellationToken).ConfigureAwait(false);

                // Type relationships
                foreach (var decl in root.DescendantNodes().OfType<BaseTypeDeclarationSyntax>())
                {
                    var symbol = semantic.GetDeclaredSymbol(decl, cancellationToken) as ITypeSymbol;
                    if (symbol is null) continue;
                    var id = symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

                    // Interfaces implemented (internal only)
                    foreach (var iface in symbol.Interfaces)
                    {
                        var bid = iface.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
                        if (!declared.ContainsKey(bid)) continue;
                        var tup = (id, bid, "Implements");
                        if (linkSet.Add(tup)) model.Links.Add(new GraphLink { SourceId = id, TargetId = bid, Kind = "Implements" });
                    }

                    // Base type (internal only and not special)
                    if (symbol.BaseType is { } bt && bt.SpecialType == SpecialType.None)
                    {
                        var bid = bt.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
                        if (declared.ContainsKey(bid))
                        {
                            var tup = (id, bid, "Inherits");
                            if (linkSet.Add(tup)) model.Links.Add(new GraphLink { SourceId = id, TargetId = bid, Kind = "Inherits" });
                        }
                    }
                }

                // Usages (project -> type) limited to internal targets only
                foreach (var node in root.DescendantNodes())
                {
                    if (cancellationToken.IsCancellationRequested) break;
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
                    if (!declared.ContainsKey(targetId)) continue; // skip external
                    var tup = (projectNodeId, targetId, "Usage");
                    if (linkSet.Add(tup)) model.Links.Add(new GraphLink { SourceId = projectNodeId, TargetId = targetId, Kind = "Usage" });
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
}
