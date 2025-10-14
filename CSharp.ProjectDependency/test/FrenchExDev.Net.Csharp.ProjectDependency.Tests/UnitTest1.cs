using FrenchExDev.Net.CSharp.ProjectDependency;
using FrenchExDev.Net.CSharp.ProjectDependency.Abstractions;
using Microsoft.CodeAnalysis;

namespace FrenchExDev.Net.Csharp.ProjectDependency.Tests;

public class UnitTest1
{
    [Fact]
    public async Task Test1()
    {
        // Try to find a solution file by walking up the directory tree from the current working directory.
        var rootSln = @"C:\code\FrenchExDev.Net\FrenchExDev.Net_i2\FrenchExDev.Net\FrenchExDev.Net.sln";

        var msBuildRegisteringService = new MsBuildRegisteringService();
        msBuildRegisteringService.Register();

        var msBuildWorkspace = new MsBuildWorkspace();
        msBuildWorkspace.Initialize();

        var solutionLoader = new SolutionLoader(msBuildRegisteringService, msBuildWorkspace);

        var solutionR = await solutionLoader.OpenSolutionAsync(rootSln);

        Assert.True(solutionR.IsSuccess);
    }

    List<Microsoft.CodeAnalysis.Project> TopologicallySortProjects(Microsoft.CodeAnalysis.Solution solution)
    {
        var projects = solution.Projects.ToList();
        var idToProject = projects.ToDictionary(p => p.Id);
        // adjacency: project -> list of projectIds it references
        var adj = projects.ToDictionary(
            p => p.Id,
            p => p.ProjectReferences.Select(r => r.ProjectId).ToList()
        );

        // compute in-degrees
        var inDegree = projects.ToDictionary(p => p.Id, p => 0);
        foreach (var kv in adj)
            foreach (var to in kv.Value)
                if (inDegree.ContainsKey(to))
                    inDegree[to]++;

        // nodes with zero in-degree
        var q = new Queue<ProjectId>(inDegree.Where(kv => kv.Value == 0).Select(kv => kv.Key));
        var sorted = new List<Microsoft.CodeAnalysis.Project>();

        while (q.Count > 0)
        {
            var id = q.Dequeue();
            sorted.Add(idToProject[id]);
            foreach (var nbr in adj[id])
            {
                if (!inDegree.ContainsKey(nbr)) continue;
                inDegree[nbr]--;
                if (inDegree[nbr] == 0) q.Enqueue(nbr);
            }
        }

        // if sorted doesn't contain all projects, there's a cycle
        if (sorted.Count != projects.Count)
            throw new InvalidOperationException("Cycle detected in project graph.");

        return sorted;
    }
}
