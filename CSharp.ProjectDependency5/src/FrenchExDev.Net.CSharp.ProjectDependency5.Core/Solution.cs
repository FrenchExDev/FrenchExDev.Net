using FrenchExDev.Net.CSharp.ManagedList;

namespace FrenchExDev.Net.CSharp.ProjectDependency5.Core;

public class Solution
{
    private readonly OpenManagedList<Project> _projects = new();
    public ICollection<Project> Projects => _projects;

    public Microsoft.CodeAnalysis.Solution Code { get; private set; }

    public Solution(Microsoft.CodeAnalysis.Solution code)
    {
        Code = code;
    }

    public void AddProject(Project project)
    {
        _projects.Add(project);
    }
}

