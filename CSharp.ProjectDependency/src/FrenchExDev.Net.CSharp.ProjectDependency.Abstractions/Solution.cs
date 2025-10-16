using FrenchExDev.Net.CSharp.ManagedDictionary;
using FrenchExDev.Net.CSharp.Object.Result;
using Microsoft.Build.Evaluation;
using Microsoft.CodeAnalysis;

namespace FrenchExDev.Net.CSharp.ProjectDependency.Abstractions;

public class Solution
{
    private readonly Microsoft.CodeAnalysis.Solution _code;
    private readonly OpenManagedDictionary<string, Project> _projects = new OpenManagedDictionary<string, Project>();

    public Microsoft.CodeAnalysis.Solution Code => _code;

    public IReadOnlyDictionary<string, Project> Projects => _projects.ToReadOnly();
    public Solution(Microsoft.CodeAnalysis.Solution solution)
    {
        _code = solution;
    }

    public void LoadProjects(IProjectCollection pc)
    {
        var visited = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (Microsoft.CodeAnalysis.Project roslynProj in _code.Projects)
        {
            if (string.IsNullOrWhiteSpace(roslynProj.FilePath))
                continue;

            var rootPath = Path.GetFullPath(roslynProj.FilePath);
            if (visited.Contains(rootPath))
                continue;

            // DFS stack to walk project references recursively
            var stack = new Stack<string>();
            stack.Push(rootPath);

            while (stack.Count > 0)
            {
                var current = stack.Pop();
                if (visited.Contains(current))
                    continue;

                visited.Add(current);

                if (_projects.ContainsKey(current)) continue;

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
                var roslynForCurrent = _code.Projects.First(p => !string.IsNullOrWhiteSpace(p.FilePath)
                    && Path.GetFullPath(p.FilePath).Equals(current, StringComparison.OrdinalIgnoreCase));

                var project = new Abstractions.Project(roslynForCurrent, current, msproj);
                _projects.Add(current, project);

                foreach (var item in msproj.GetItems("ProjectReference"))
                {
                    var include = item.EvaluatedInclude ?? string.Empty;
                    if (string.IsNullOrWhiteSpace(include))
                        continue;

                    // resolve relative path from the current project's directory
                    var resolved = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(current) ?? string.Empty, include));

                    // schedule for visit if not already visited or loaded
                    if (!visited.Contains(resolved) && !_projects.ContainsKey(resolved))
                        stack.Push(resolved);
                }
            }
        }
    }

    public IEnumerable<ProjectAnalysis> ScanProjects(IProjectCollection projectCollection)
    {
        foreach (var project in _projects)
        {
            yield return ScanProject(project.Value).ObjectOrThrow();
        }
    }

    private Result<ProjectAnalysis> ScanProject(Project project)
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

        return Result<ProjectAnalysis>.Success(new ProjectAnalysis(project.Name, filePath, packageRefs, projectRefs));
    }
}
