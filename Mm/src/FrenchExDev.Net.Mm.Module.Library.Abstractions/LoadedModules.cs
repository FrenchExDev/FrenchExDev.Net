using FrenchExDev.Net.Mm.Abstractions;

namespace FrenchExDev.Net.Mm.Module.Library.Abstractions;

public class LoadedModules : Dictionary<ModuleId, ILibraryModule>
{
    public LoadedModules Add(Guid moduleGuid, ILibraryModule module)
    {
        this[new ModuleId(moduleGuid)] = module;
        return this;
    }
}
