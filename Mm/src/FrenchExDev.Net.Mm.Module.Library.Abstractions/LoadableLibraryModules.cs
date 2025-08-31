using FrenchExDev.Net.Mm.Abstractions;

namespace FrenchExDev.Net.Mm.Module.Library.Abstractions;

/// <summary>
/// Represents a collection of library modules that can be loaded dynamically,  where each module is identified by a
/// unique <see cref="ModuleId"/> and associated with a factory method.
/// </summary>
/// <remarks>This class extends <see cref="Dictionary{TKey, TValue}"/> with <see cref="ModuleId"/> as the key  and
/// a factory method (<see cref="Func{ILibraryModule}"/>) as the value. It provides a fluent API  for adding modules to
/// the collection.</remarks>
public class LoadableLibraryModules : Dictionary<ModuleId, Func<ILibraryModule>>
{
    /// <summary>
    /// Adds a library module to the collection using the specified module identifier and factory method.
    /// </summary>
    /// <param name="moduleGuid">The unique identifier of the module to add.</param>
    /// <param name="moduleFactory">A factory method that creates an instance of the module. This function is invoked when the module is loaded.</param>
    /// <returns>The current <see cref="LoadableLibraryModules"/> instance, allowing for method chaining.</returns>
    public LoadableLibraryModules Add(Guid moduleGuid, Func<ILibraryModule> moduleFactory)
    {
        this[new ModuleId(moduleGuid)] = moduleFactory;
        return this;
    }

    /// <summary>
    /// Determines whether the collection contains an entry with the specified module identifier.
    /// </summary>
    /// <remarks>This method checks for the presence of a module in the collection using its GUID. The
    /// comparison is based on the  <see cref="ModuleId"/> constructed from the provided <paramref
    /// name="moduleGuid"/>.</remarks>
    /// <param name="moduleGuid">The globally unique identifier (GUID) of the module to locate in the collection.</param>
    /// <returns><see langword="true"/> if the collection contains an entry with the specified module identifier; otherwise, <see
    /// langword="false"/>.</returns>
    public bool ContainsKey(Guid moduleGuid)
    {
        return this.ContainsKey(new ModuleId(moduleGuid));
    }
}