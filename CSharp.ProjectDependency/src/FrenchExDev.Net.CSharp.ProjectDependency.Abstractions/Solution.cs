using FrenchExDev.Net.CSharp.ManagedDictionary;
using FrenchExDev.Net.CSharp.Object.Result;
using Microsoft.Build.Evaluation;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Collections.Concurrent;

namespace FrenchExDev.Net.CSharp.ProjectDependency.Abstractions;

public class Solution
{
    private readonly Microsoft.CodeAnalysis.Solution _code;
    private readonly OpenManagedDictionary<string, Project> _projects = new OpenManagedDictionary<string, Project>();
    private readonly object _projectsLock = new();

    public Microsoft.CodeAnalysis.Solution Code => _code;

    public IReadOnlyDictionary<string, Project> Projects => _projects.ToReadOnly();
    public Solution(Microsoft.CodeAnalysis.Solution solution)
    {
        _code = solution;
    }

    public void LoadProjects(IProjectCollection pc)
    {
        // Use a concurrent visited set so multiple threads can walk different roots in parallel
        var visited = new ConcurrentDictionary<string, byte>(StringComparer.OrdinalIgnoreCase);

        // collect root project file paths
        var roots = _code.Projects
            .Where(p => !string.IsNullOrWhiteSpace(p.FilePath))
            .Select(p => Path.GetFullPath(p.FilePath!))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        // process each root in parallel
        Parallel.ForEach(roots, rootPath =>
        {
            // local DFS stack for this worker
            var stack = new Stack<string>();
            stack.Push(rootPath);

            while (stack.Count > 0)
            {
                var current = stack.Pop();

                // ensure single visit across all threads
                if (!visited.TryAdd(current, 0))
                    continue;

                // already added to projects? skip loading
                lock (_projectsLock)
                {
                    if (_projects.ContainsKey(current))
                        continue;
                }

                // try to load the MSBuild project; on failure continue
                Microsoft.Build.Evaluation.Project? msproj = null;
                try
                {
                    msproj = pc.LoadProject(current);
                }
                catch (Exception)
                {
                    // unable to load project file, skip recursion from this node
                    continue;
                }

                // try to find corresponding Roslyn project in the solution by file path
                var roslynForCurrent = _code.Projects.FirstOrDefault(p => !string.IsNullOrWhiteSpace(p.FilePath)
                    && Path.GetFullPath(p.FilePath!).Equals(current, StringComparison.OrdinalIgnoreCase));

                Microsoft.CodeAnalysis.Project codeProject;
                if (roslynForCurrent != null)
                {
                    codeProject = roslynForCurrent;
                }
                else
                {
                    // fallback: create a minimal project in adhoc workspace
                    var adhoc = new AdhocWorkspace();
                    var name = Path.GetFileNameWithoutExtension(current) ?? "Unknown";
                    var pid = ProjectId.CreateNewId();
                    var info = ProjectInfo.Create(pid, VersionStamp.Create(), name, name, LanguageNames.CSharp, filePath: current);
                    codeProject = adhoc.AddProject(info);
                }

                var project = new Abstractions.Project(codeProject, current, msproj);

                // add to shared dictionary under lock
                lock (_projectsLock)
                {
                    if (!_projects.ContainsKey(current))
                        _projects.Add(current, project);
                }

                // enqueue referenced project files for further processing
                foreach (var item in msproj.GetItems("ProjectReference"))
                {
                    var include = item.EvaluatedInclude ?? string.Empty;
                    if (string.IsNullOrWhiteSpace(include))
                        continue;

                    // resolve relative path from the current project's directory
                    var resolved = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(current) ?? string.Empty, include));

                    // schedule for visit if not already visited or loaded
                    if (!visited.ContainsKey(resolved))
                    {
                        // small race is fine; TryAdd will protect from duplicates when popped
                        stack.Push(resolved);
                    }
                }
            }
        });
    }

    public async IAsyncEnumerable<ProjectAnalysis> ScanProjectsAsync(IProjectCollection projectCollection)
    {
        foreach (var project in _projects)
        {
            yield return (await ScanProjectAsync(project.Value)).ObjectOrThrow();
        }
    }

    private async Task<Result<ProjectAnalysis>> ScanProjectAsync(Project project)
    {
        var filePath = Path.GetFullPath(project.FilePath) ?? string.Empty;
        var current = _projects[filePath];
        var packageRefs = new List<PackageReference>();
        var projectRefs = new List<ProjectReference>();

        if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
        {
            return Result<ProjectAnalysis>.Failure(d => d.Add("File", "does not exist"));
        }

        foreach (var item in project.Msproj.GetItems("PackageReference"))
        {
            var id = item.EvaluatedInclude ?? string.Empty;
            var version = item.GetMetadataValue("Version");
            if (string.IsNullOrWhiteSpace(version))
                version = item.GetMetadataValue("Version");
            packageRefs.Add(new PackageReference(id, version));
        }

        foreach (ProjectItem? item in project.Msproj.GetItems("ProjectReference"))
        {
            var include = item.EvaluatedInclude ?? string.Empty;
            var resolved = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(filePath) ?? string.Empty, include));

            if (_projects.ContainsKey(resolved))
            {
                projectRefs.Add(new ProjectReference(current, _projects[resolved]));
            }
        }

        // analyze constructs exported by this project
        ProjectConstructMetrics? constructs = null;
        List<DeclarationDescriptor>? declarations = null;
        try
        {
            var compilation = project.Code.GetCompilationAsync().GetAwaiter().GetResult();
            int records = 0, enums = 0, classes = 0, interfaces = 0, structs = 0, extensionMethods = 0, publicMembers = 0;
            var decls = new List<DeclarationDescriptor>();

            foreach (var tree in compilation.SyntaxTrees)
            {
                var model = compilation.GetSemanticModel(tree);
                var root = tree.GetRoot();

                // count type declarations
                var typeDecls = root.DescendantNodes().OfType<BaseTypeDeclarationSyntax>();
                foreach (var decl in typeDecls)
                {
                    var sym = model.GetDeclaredSymbol(decl) as INamedTypeSymbol;
                    if (sym == null) continue;
                    if (sym.DeclaredAccessibility != Accessibility.Public) continue;

                    string kind;
                    if (sym.TypeKind == TypeKind.Interface) { interfaces++; kind = "interface"; }
                    else if (sym.TypeKind == TypeKind.Enum) { enums++; kind = "enum"; }
                    else if (sym.TypeKind == TypeKind.Struct) { structs++; kind = "struct"; }
                    else if (sym.IsRecord) { records++; kind = "record"; }
                    else if (sym.TypeKind == TypeKind.Class) { classes++; kind = "class"; }
                    else { kind = "type"; }

                    // count public members
                    foreach (var m in sym.GetMembers())
                    {
                        if (m.DeclaredAccessibility == Accessibility.Public)
                            publicMembers++;
                    }

                    var isAbstract = sym.IsAbstract;
                    var name = sym.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
                    decls.Add(new DeclarationDescriptor(name, kind, isAbstract));
                }

                // extension methods
                var methodDecls = root.DescendantNodes().OfType<MethodDeclarationSyntax>();
                foreach (var md in methodDecls)
                {
                    var msym = model.GetDeclaredSymbol(md) as IMethodSymbol;
                    if (msym != null && msym.IsExtensionMethod && msym.DeclaredAccessibility == Accessibility.Public)
                        extensionMethods++;
                }
            }

            constructs = new ProjectConstructMetrics(records, enums, classes, interfaces, structs, extensionMethods, publicMembers);
            declarations = decls;
        }
        catch
        {
            // ignore analysis failures; leave constructs null
        }

        // analyze reference coupling consumption from this project to referenced projects
        var referenceCouplings = new List<ReferenceCoupling>();
        try
        {
            var compilation = project.Code.GetCompilationAsync().GetAwaiter().GetResult();

            foreach (var pref in projectRefs)
            {
                var referenced = pref.Project;
                if (referenced == null) continue;

                int totalUses = 0, ifaceUses = 0, classUses = 0;
                var targetAssemblyName = referenced.Code.AssemblyName ?? Path.GetFileNameWithoutExtension(referenced.FilePath);

                foreach (var tree in compilation.SyntaxTrees)
                {
                    var model = compilation.GetSemanticModel(tree);
                    var ids = tree.GetRoot().DescendantNodes().OfType<IdentifierNameSyntax>();
                    foreach (var id in ids)
                    {
                        var s = model.GetSymbolInfo(id).Symbol;
                        if (s is INamedTypeSymbol nts)
                        {
                            var asmName = nts.ContainingAssembly?.Name;
                            if (string.Equals(asmName, targetAssemblyName, StringComparison.OrdinalIgnoreCase))
                            {
                                totalUses++;
                                if (nts.TypeKind == TypeKind.Interface || nts.TypeKind == TypeKind.Enum || nts.IsValueType || nts.IsRecord) ifaceUses++;
                                else if (nts.TypeKind == TypeKind.Class) classUses++;
                            }
                        }
                        else if (s is IMethodSymbol msym)
                        {
                            var asmName = msym.ContainingAssembly?.Name;
                            if (string.Equals(asmName, targetAssemblyName, StringComparison.OrdinalIgnoreCase))
                            {
                                totalUses++;
                                // if method's receiver type is class or interface
                                var recv = msym.ReceiverType;
                                if (recv != null)
                                {
                                    if (recv.TypeKind == TypeKind.Class) classUses++;
                                    else ifaceUses++;
                                }
                            }
                        }
                    }
                }

                var level = CouplingLevel.Low;
                if (totalUses > 0)
                {
                    if ((double)classUses / Math.Max(1, totalUses) > 0.5) level = CouplingLevel.High;
                    else level = CouplingLevel.Low;
                }

                referenceCouplings.Add(new ReferenceCoupling(referenced.FilePath, level, totalUses, ifaceUses, classUses));
            }
        }
        catch
        {
            // ignore
        }

        return Result<ProjectAnalysis>.Success(new ProjectAnalysis(project.Name, filePath, packageRefs, projectRefs, referenceCouplings, constructs, null, null, null, null, null, declarations));
    }
}
