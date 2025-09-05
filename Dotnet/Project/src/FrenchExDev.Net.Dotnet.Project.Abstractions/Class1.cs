namespace FrenchExDev.Net.Dotnet.Project.Abstractions;


public interface IProject
{

}

public abstract class AbstractProject<T> : IProject where T : IProject
{
    public string Name { get; set; }
    public string Directory { get; set; }
    public string Sdk { get; set; }
    public string TargetFramework { get; set; }
    public string OutputType { get; set; }
    public string LangVersion { get; set; }
    public bool Nullable { get; set; }
    public bool ImplicitUsings { get; set; }
    public List<string> ProjectReferences { get; set; } = new List<string>();
    public List<string> PackageReferences { get; set; } = new List<string>();
    public List<string> Analyzers { get; set; } = new List<string>();
    public List<string> AdditionalProperties { get; set; } = new List<string>();
}

public class ClassProject : AbstractProject<ClassProject>
{

}

public class LibraryProject : AbstractProject<LibraryProject>
{

}

public class DesktopProject : AbstractProject<DesktopProject>
{

}

public class CliProject : AbstractProject<CliProject>
{

}

public class WebApiProject : AbstractProject<WebApiProject>
{

}

public class WebWorkerProject : AbstractProject<WebWorkerProject>
{

}