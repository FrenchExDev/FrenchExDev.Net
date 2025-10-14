using FrenchExDev.Net.CSharp.ManagedList;

namespace FrenchExDev.Net.CSharp.SolutionEngineering;

public interface ISolution
{
    string Name { get; }

    SolutionParameterDictionary Parameters { get; }

    SolutionProjectList Projects { get; }
}

public class SolutionProjectList : OpenManagedList<SolutionProject>
{

}

public class Solution : ISolution
{
    public Solution(string name)
    {
        _name = name ?? throw new ArgumentNullException(nameof(name));
        _parameters = new SolutionParameterDictionary().ForSolution(this);
        _projects = new SolutionProjectList();
    }

    private string _name;
    public string Name => _name;

    private SolutionParameterDictionary _parameters;
    public SolutionParameterDictionary Parameters => _parameters;

    private SolutionProjectList _projects;
    public SolutionProjectList Projects => _projects;
}

public class SolutionParameterDictionary : Dictionary<string, object>
{
    public const string SolutionKey = "Solution";
    public SolutionParameterDictionary()
    {
    }

    public SolutionParameterDictionary(IDictionary<string, object> dictionary) : base(dictionary)
    {
    }

    public SolutionParameterDictionary ForSolution(ISolution solution)
    {
        this[SolutionKey] = solution;
        return this;
    }

    public ISolution Solution => (ISolution)this[SolutionKey];
}

public interface IProject
{
    ProgrammingLanguage ProgrammingLanguage { get; }
}

public class SolutionProject
{
    private ISolution _solution;
    private IProject _project;
    public ISolution Solution => _solution ?? throw new InvalidOperationException(nameof(_solution));
    public IProject Project => _project ?? throw new InvalidOperationException(nameof(_project));

    public ProjectParameterDictionary Parameters { get; }

    public SolutionProject(ISolution solution, IProject project)
    {
        _solution = solution ?? throw new ArgumentNullException(nameof(solution));
        _project = project ?? throw new ArgumentNullException(nameof(project));
        Parameters = new ProjectParameterDictionary().ForProject(project).ForSolution(solution);
    }
}

public class ProjectParameterDictionary : Dictionary<string, object>
{
    public ProjectParameterDictionary()
    {
    }

    public ProjectParameterDictionary(IDictionary<string, object> dictionary) : base(dictionary)
    {
    }

    public ProjectParameterDictionary ForProject(IProject project)
    {
        this["Project"] = project;
        return this;
    }

    public ProjectParameterDictionary ForSolution(ISolution solution)
    {
        this["Solution"] = solution;
        return this;
    }

    public ISolution Solution => (ISolution)this["Solution"];

    public IProject Project => (IProject)this["Project"];
}

public enum ProgrammingLanguages
{
    CSharp,
    TypeScript,
    Python
}

public enum ApplicationTargets
{
    Desktop,
    Embedded,
    Web,
    Service
}

public record ProgrammingLanguage(string Name, string Version)
{
    public static ProgrammingLanguage CPlusPlus(string version) => new("C++", version);
    public static ProgrammingLanguage CSharp(string version) => new("C#", version);
    public static ProgrammingLanguage TypeScript(string version) => new("TypeScript", version);
    public static ProgrammingLanguage Python(string version) => new("Python", version);
}

