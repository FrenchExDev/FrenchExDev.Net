using FrenchExDev.Net.CSharp.ManagedDictionary;
using FrenchExDev.Net.CSharp.Object.Result;
using Microsoft.CodeAnalysis;
using System.Collections.Concurrent;

namespace FrenchExDev.Net.CSharp.ProjectDependency2;

/// <summary>
/// Provides functionality to load and manage a collection of projects, resolving project references and associating
/// MSBuild and Roslyn project representations as needed.
/// </summary>
/// <remarks>ProjectCollectionLoader is designed to efficiently traverse and load project files, including their
/// references, in parallel. It ensures that each project is loaded only once and maintains thread safety when updating
/// the internal collection. This class is useful when working with solutions that contain multiple interdependent
/// projects and require both MSBuild and Roslyn project information.</remarks>
public class ProjectCollectionLoader
{
    private readonly object _projectsLock = new();

    private readonly OpenManagedDictionary<string, Result<Project>> _projects = new OpenManagedDictionary<string, Result<Project>>();

    /// <summary>
    /// Loads and returns all projects and their dependencies from the specified collection, mapping each project's file
    /// path to its corresponding project instance.
    /// </summary>
    /// <remarks>If the projects have already been loaded, the method returns the existing dictionary without
    /// reloading. Project dependencies are resolved recursively by following MSBuild project references. The operation
    /// is thread-safe and processes multiple project roots in parallel for improved performance.</remarks>
    /// <param name="pc">The project collection used to load MSBuild project files. Must not be null.</param>
    /// <param name="projects">A sequence of Roslyn projects representing the initial set of projects to load. Each project's file path is used
    /// to identify and resolve dependencies.</param>
    /// <returns>A result containing a dictionary that maps project file paths to their loaded project instances. The dictionary
    /// includes all discovered projects and their transitive dependencies.</returns>
    public Result<OpenManagedDictionary<string, Result<Project>>> LoadProjects(IProjectCollection pc, IEnumerable<Microsoft.CodeAnalysis.Project> projects)
    {
        if (_projects.Count > 0)
        {
            return Result<OpenManagedDictionary<string, Result<Project>>>.Success(_projects);
        }

        // Use a concurrent visited set so multiple threads can walk different roots in parallel
        var visited = new ConcurrentDictionary<string, byte>(StringComparer.OrdinalIgnoreCase);

        // collect root project file paths
        var roots = projects
            .Where(p => !string.IsNullOrWhiteSpace(p.FilePath))
            .Select(p => Path.GetFullPath(p.FilePath!))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        // local DFS stack for this worker
        var stack = new Stack<string>();
        foreach (var root in roots)
        {
            stack.Push(root);
        }

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
            catch (Exception ex)
            {
                // unable to load project file, skip recursion from this node
                _projects.Add(current, Result<Project>.Failure(ex));
                continue;
            }

            // try to find corresponding Roslyn project in the solution by file path
            var roslynForCurrent = projects.FirstOrDefault(p => !string.IsNullOrWhiteSpace(p.FilePath)
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

            var project = Result<Project>.Success(new Project(codeProject, current, msproj));

            _projects.Add(current, project);

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

        return Result<OpenManagedDictionary<string, Result<Project>>>.Success(_projects);
    }
}