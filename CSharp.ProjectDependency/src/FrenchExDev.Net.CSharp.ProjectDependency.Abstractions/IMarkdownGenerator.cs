namespace FrenchExDev.Net.CSharp.ProjectDependency.Abstractions;

public interface IMarkdownGenerator<T>
{
    string Generate(T item);
}
