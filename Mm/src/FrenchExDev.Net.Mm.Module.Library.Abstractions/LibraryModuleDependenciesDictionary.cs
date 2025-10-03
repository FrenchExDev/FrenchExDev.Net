using FrenchExDev.Net.Mm.Abstractions;

namespace FrenchExDev.Net.Mm.Module.Library.Abstractions;

/// <summary>
/// Represents a collection of module dependencies, where each dependency is identified by a <see cref="ModuleId"/> and
/// associated with a factory function that creates an instance of <see cref="ILibraryModule"/>.
/// </summary>
/// <remarks>This dictionary is typically used to manage and resolve dependencies between modules in a library or
/// application. The factory functions allow for deferred creation of module instances, enabling lazy initialization or
/// dynamic resolution.</remarks>
public class LibraryModuleDependenciesDictionary : Dictionary<ModuleId, Func<ILibraryModule>>
{
    public LibraryModuleDependenciesDictionary() : base()
    {
    }
    public LibraryModuleDependenciesDictionary(IDictionary<ModuleId, Func<ILibraryModule>> dictionary) : base(dictionary)
    {
    }
}
