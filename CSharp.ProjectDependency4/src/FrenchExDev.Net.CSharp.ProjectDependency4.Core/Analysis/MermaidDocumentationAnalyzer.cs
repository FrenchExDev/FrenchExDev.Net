using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace FrenchExDev.Net.CSharp.ProjectDependency4.Core.Analysis;

/// <summary>
/// Analyzer that generates Mermaid markdown documentation for projects and their code elements.
/// </summary>
public class MermaidDocumentationAnalyzer : IProjectAnalyzer
{
    public string Name => "MermaidDocumentation";

    public async Task<IProjectAnalysisResult> AnalyzeAsync(Solution solution, CancellationToken cancellationToken = default)
    {
        var projectDocs = new List<ProjectDocumentation>();
        var solutionDir = System.IO.Path.GetDirectoryName(solution.Code.FilePath ?? string.Empty) ?? Environment.CurrentDirectory;

        foreach (var proj in solution.Projects)
        {
            if (cancellationToken.IsCancellationRequested)
                break;

            var doc = await AnalyzeProjectAsync(proj, solution, solutionDir, cancellationToken);
            projectDocs.Add(doc);
        }

        return new MermaidDocumentationResult(Name, projectDocs);
    }

    private async Task<ProjectDocumentation> AnalyzeProjectAsync(
        Project project,
        Solution solution,
        string solutionDir,
        CancellationToken cancellationToken)
    {
        var projectName = project.Name;
        var projectPath = project.FilePath;

        // Analyze project references (usings)
        var projectRefs = project.Msproj.GetItems("ProjectReference")
            .Select(i => MakeRelativeProjectPath(project.Msproj, i.EvaluatedInclude, solutionDir))
            .Where(p => p is not null)
            .Select(p => p!)
            .ToList();

        // Analyze which projects use this project (usages)
        var usedBy = solution.Projects
            .Where(p => p.Msproj.GetItems("ProjectReference")
            .Select(i => MakeRelativeProjectPath(p.Msproj, i.EvaluatedInclude, solutionDir))
            .Any(refPath => string.Equals(refPath, System.IO.Path.GetRelativePath(solutionDir, projectPath), StringComparison.OrdinalIgnoreCase)))
            .Select(p => p.Name)
            .ToList();

        // Analyze code elements
        var elements = new List<CodeElementDocumentation>();
        try
        {
            var compilation = await project.Code.GetCompilationAsync(cancellationToken);
            if (compilation != null)
            {
                foreach (var syntaxTree in compilation.SyntaxTrees)
                {
                    if (cancellationToken.IsCancellationRequested)
                        break;

                    var semanticModel = compilation.GetSemanticModel(syntaxTree);
                    var root = await syntaxTree.GetRootAsync(cancellationToken);

                    // Analyze type declarations
                    var typeDeclarations = root.DescendantNodes().OfType<BaseTypeDeclarationSyntax>();
                    foreach (var typeDecl in typeDeclarations)
                    {
                        var element = AnalyzeTypeDeclaration(typeDecl, semanticModel, compilation);
                        if (element != null)
                            elements.Add(element);
                    }
                }
            }
        }
        catch (Exception)
        {
            // Ignore compilation errors
        }

        return new ProjectDocumentation(projectName, projectPath, projectRefs, usedBy, elements);
    }

    private CodeElementDocumentation? AnalyzeTypeDeclaration(
            BaseTypeDeclarationSyntax typeDecl,
            SemanticModel semanticModel,
            Compilation compilation)
    {
        var symbol = semanticModel.GetDeclaredSymbol(typeDecl) as INamedTypeSymbol;
        if (symbol == null || symbol.DeclaredAccessibility != Accessibility.Public)
            return null;

        var elementName = symbol.Name;
        var elementKind = GetElementKind(symbol);
        var fullName = symbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);

        // Analyze usings (dependencies)
        var usings = new List<string>();
        var usages = new List<string>();

        // Get base types and implemented interfaces
        if (symbol.BaseType != null && symbol.BaseType.SpecialType != SpecialType.System_Object)
        {
            usings.Add(FormatTypeName(symbol.BaseType));
        }

        foreach (var iface in symbol.Interfaces)
        {
            usings.Add(FormatTypeName(iface));
        }

        // Analyze member types (fields, properties, method parameters and return types)
        foreach (var member in symbol.GetMembers())
        {
            if (member.DeclaredAccessibility != Accessibility.Public)
                continue;

            switch (member)
            {
                case IFieldSymbol field:
                    AddTypeReference(field.Type, usings);
                    break;
                case IPropertySymbol property:
                    AddTypeReference(property.Type, usings);
                    break;
                case IMethodSymbol method:
                    if (!method.MethodKind.ToString().Contains("Constructor"))
                    {
                        AddTypeReference(method.ReturnType, usings);
                        foreach (var param in method.Parameters)
                        {
                            AddTypeReference(param.Type, usings);
                        }
                    }
                    break;
            }
        }

        // Find usages of this type in the same compilation
        foreach (var syntaxTree in compilation.SyntaxTrees)
        {
            var root = syntaxTree.GetRoot();
            var model = compilation.GetSemanticModel(syntaxTree);

            var identifiers = root.DescendantNodes().OfType<IdentifierNameSyntax>();
            foreach (var identifier in identifiers)
            {
                var symbolInfo = model.GetSymbolInfo(identifier);
                var referencedSymbol = symbolInfo.Symbol;

                if (referencedSymbol != null)
                {
                    var containingType = referencedSymbol.ContainingType;
                    if (containingType != null && SymbolEqualityComparer.Default.Equals(containingType, symbol))
                    {
                        var usageType = FindContainingType(identifier);
                        if (usageType != null && !string.Equals(usageType, fullName, StringComparison.Ordinal))
                        {
                            usages.Add(usageType);
                        }
                    }
                }

                if (referencedSymbol is INamedTypeSymbol namedType && SymbolEqualityComparer.Default.Equals(namedType, symbol))
                {
                    var usageType = FindContainingType(identifier);
                    if (usageType != null && !string.Equals(usageType, fullName, StringComparison.Ordinal))
                    {
                        usages.Add(usageType);
                    }
                }
            }
        }

        return new CodeElementDocumentation(
            elementName,
 elementKind,
        fullName,
   usings.Distinct().ToList(),
    usages.Distinct().ToList()
        );
    }

    private static string GetElementKind(INamedTypeSymbol symbol)
    {
        if (symbol.TypeKind == TypeKind.Interface)
            return "interface";
        if (symbol.TypeKind == TypeKind.Enum)
            return "enum";
        if (symbol.TypeKind == TypeKind.Struct)
            return "struct";
        if (symbol.IsRecord)
            return "record";
        return "class";
    }

    private static string FormatTypeName(ITypeSymbol type)
    {
        var displayFormat = SymbolDisplayFormat.MinimallyQualifiedFormat;
        return type.ToDisplayString(displayFormat);
    }

    private static void AddTypeReference(ITypeSymbol type, List<string> usings)
    {
        if (type.SpecialType != SpecialType.None)
            return;

        if (type is INamedTypeSymbol namedType)
        {
            usings.Add(FormatTypeName(namedType));

            // Handle generic type arguments
            foreach (var typeArg in namedType.TypeArguments)
            {
                AddTypeReference(typeArg, usings);
            }
        }
        else if (type is IArrayTypeSymbol arrayType)
        {
            AddTypeReference(arrayType.ElementType, usings);
        }
    }

    private static string? FindContainingType(SyntaxNode node)
    {
        var typeDecl = node.Ancestors().OfType<BaseTypeDeclarationSyntax>().FirstOrDefault();
        if (typeDecl != null)
        {
            var name = typeDecl is TypeDeclarationSyntax tds ? tds.Identifier.Text :
           typeDecl is EnumDeclarationSyntax eds ? eds.Identifier.Text : null;

            // Include namespace if present
            var namespaceDecl = typeDecl.Ancestors().OfType<BaseNamespaceDeclarationSyntax>().FirstOrDefault();
            if (namespaceDecl != null && name != null)
            {
                return $"{namespaceDecl.Name}.{name}";
            }
            return name;
        }
        return null;
    }

    private static string? MakeRelativeProjectPath(Microsoft.Build.Evaluation.Project msproj, string include, string solutionDir)
    {
        try
        {
            var full = System.IO.Path.GetFullPath(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(msproj.FullPath)!, include));
            var rel = System.IO.Path.GetRelativePath(solutionDir, full);
            return rel;
        }
        catch
        {
            return null;
        }
    }
}

/// <summary>
/// Represents documentation for a project including its dependencies and code elements.
/// </summary>
public record ProjectDocumentation(
    string Name,
    string Path,
    IReadOnlyList<string> ProjectReferences,
    IReadOnlyList<string> UsedByProjects,
    IReadOnlyList<CodeElementDocumentation> Elements
);

/// <summary>
/// Represents documentation for a code element (class, interface, enum, struct, record).
/// </summary>
public record CodeElementDocumentation(
    string Name,
    string Kind,
    string FullName,
    IReadOnlyList<string> Usings,
    IReadOnlyList<string> Usages
);
