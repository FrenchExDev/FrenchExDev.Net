using FrenchExDev.Net.CSharp.Object.Result;
using FrenchExDev.Net.CSharp.ProjectDependency2.Analysis.Shared;
using Microsoft.CodeAnalysis;

namespace FrenchExDev.Net.CSharp.ProjectDependency2.Analysis.Api;

public class InterfaceDesignAnalyzer : IProjectAnalyzer
{
    private readonly int _maxMembers;

    public InterfaceDesignAnalyzer(int maxMembers = 20)
    {
        _maxMembers = maxMembers;
    }

    public Result<IProjectAnalysisResult> AnalyzeProject(Project project, Solution solution)
    {
        try
        {
            var code = project.Code;
            var items = new List<InterfaceDesignItem>();
            var compilation = code.GetCompilationAsync().GetAwaiter().GetResult();
            if (compilation == null) return Result<IProjectAnalysisResult>.Success(new InterfaceDesignReport(items));

            foreach (var tree in compilation.SyntaxTrees)
            {
                var model = compilation.GetSemanticModel(tree);
                var root = tree.GetRoot();
                var interfaces = root.DescendantNodes()
                    .Select(n => model.GetDeclaredSymbol(n))
                    .OfType<INamedTypeSymbol>()
                    .Where(s => s.TypeKind == TypeKind.Interface && s.DeclaredAccessibility == Accessibility.Public);

                foreach (var i in interfaces)
                {
                    int methods = i.GetMembers().OfType<IMethodSymbol>().Count(m => m.MethodKind == MethodKind.Ordinary);
                    int properties = i.GetMembers().OfType<IPropertySymbol>().Count();
                    int events = i.GetMembers().OfType<IEventSymbol>().Count();
                    int arity = i.Arity;
                    int depth = GetDepth(i);
                    bool startsWithI = i.Name.StartsWith("I");
                    bool tooLarge = (methods + properties + events) > _maxMembers;
                    bool onlyProps = methods == 0 && events == 0 && properties > 0;

                    items.Add(new InterfaceDesignItem(i.ToDisplayString(), methods, properties, events, arity, depth, startsWithI, tooLarge, onlyProps));
                }
            }

            return Result<IProjectAnalysisResult>.Success(new InterfaceDesignReport(items));
        }
        catch (Exception ex)
        {
            return Result<IProjectAnalysisResult>.Failure(ex);
        }
    }

    private static int GetDepth(INamedTypeSymbol iface)
    {
        int depth = 0;
        var cur = iface;
        while (cur.Interfaces.Length > 0)
        {
            depth++;
            cur = cur.Interfaces[0];
        }
        return depth;
    }
}
