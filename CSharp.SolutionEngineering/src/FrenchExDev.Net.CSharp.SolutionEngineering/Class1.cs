namespace FrenchExDev.Net.CSharp.SolutionEngineering;

public interface ISolution
{
    string Name { get; }

    SolutionParameterDictionary Parameters { get; }

    ICollection<SolutionProject> Projects { get; }

    ICollection<IFeature> Features { get; }
}

public class SolutionParameterDictionary : Dictionary<string, object>
{
    public SolutionParameterDictionary()
    {
    }

    public SolutionParameterDictionary(IDictionary<string, object> dictionary) : base(dictionary)
    {
    }

    public SolutionParameterDictionary ForSolution(ISolution solution)
    {
        this["Solution"] = solution;
        return this;
    }

    public ISolution Solution => (ISolution)this["Solution"];
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
    public static ProgrammingLanguage CSharp(string version) => new("C#", version);

    public static ProgrammingLanguage TypeScript(string version) => new("TypeScript", version);

    public static ProgrammingLanguage Python(string version) => new("Python", version);
}


public interface IFeature
{

}

public interface IFeatureToggle<TFeature, TToggler>
    where TFeature : IFeature
    where TToggler : notnull
{
    TFeature Feature { get; }
    TToggler Toggler { get; }

    bool Toggle();
}

public interface IFeaturePart<TFeature> where TFeature : IFeature
{
}

public interface IFeaturePartWithToggle<TFeature, TToggler> : IFeaturePart<TFeature>, IFeatureToggle<TFeature, TToggler>
    where TFeature : IFeature
    where TToggler : notnull
{
}


