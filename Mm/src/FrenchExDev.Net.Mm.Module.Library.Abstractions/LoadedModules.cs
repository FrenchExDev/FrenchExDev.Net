using FrenchExDev.Net.Mm.Abstractions;

namespace FrenchExDev.Net.Mm.Module.Library.Abstractions;

/// <summary>
/// Represents a collection of loaded library modules, keyed by their unique identifiers.
/// </summary>
/// <remarks>This class extends <see cref="Dictionary{TKey, TValue}"/> to provide additional functionality for
/// managing and accessing library modules by their <see cref="ModuleId"/> or <see cref="Guid"/>.</remarks>
public class LoadedModules : Dictionary<ModuleId, ILibraryModule>
{
    /// <summary>
    /// Adds a library module to the collection, associating it with the specified module identifier.
    /// </summary>
    /// <param name="moduleGuid">The unique identifier of the module to add.</param>
    /// <param name="module">The library module to associate with the specified identifier.</param>
    /// <returns>The updated <see cref="LoadedModules"/> instance, allowing for method chaining.</returns>
    public LoadedModules Add(Guid moduleGuid, ILibraryModule module)
    {
        this[new ModuleId(moduleGuid)] = module;
        return this;
    }

    /// <summary>
    /// Adds the specified library module to the collection.
    /// </summary>
    /// <param name="module">The library module to add. The module's <see cref="ILibraryModule.ModuleId"/> is used as the key.</param>
    /// <returns>The updated <see cref="LoadedModules"/> instance, allowing for method chaining.</returns>
    public LoadedModules Add(ILibraryModule module)
    {
        this[module.ModuleId] = module;
        return this;
    }

    /// <summary>
    /// Determines whether the collection contains a module with the specified unique identifier.
    /// </summary>
    /// <param name="moduleGuid">The globally unique identifier (GUID) of the module to locate.</param>
    /// <returns><see langword="true"/> if the collection contains a module with the specified GUID; otherwise, <see
    /// langword="false"/>.</returns>
    public bool Contains(Guid moduleGuid) => ContainsKey(new ModuleId(moduleGuid));

    /// <summary>
    /// Retrieves the library module associated with the specified unique identifier.
    /// </summary>
    /// <param name="moduleGuid">The globally unique identifier (GUID) of the module to retrieve.</param>
    /// <returns>The <see cref="ILibraryModule"/> instance corresponding to the specified <paramref name="moduleGuid"/>.</returns>
    public ILibraryModule Get(Guid moduleGuid) => this[new ModuleId(moduleGuid)];

    /// <summary>
    /// Retrieves the library module associated with the specified module identifier.
    /// </summary>
    /// <param name="moduleId">The identifier of the module to retrieve.</param>
    /// <returns>The <see cref="ILibraryModule"/> associated with the specified <paramref name="moduleId"/>.</returns>
    public ILibraryModule Get(ModuleId moduleId) => this[moduleId];
}
