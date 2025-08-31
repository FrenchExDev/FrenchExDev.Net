using FrenchExDev.Net.Mm.Abstractions;
using FrenchExDev.Net.Mm.Module.Library.Abstractions;
using FrenchExDev.Net.Mm.Module.Library.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FrenchExDev.Net.Mm.Module.Library.Tests;

public class LibraryModuleLoaderTests
{
    internal sealed class ModuleA() : LibraryModule(Dependencies)
    {
        public static readonly Guid Guid = Guid.Parse("6102d0af-cb8b-492a-b746-1e9deb31106e");

        public static readonly ModuleDependenciesDictionary Dependencies = new()
        {
            { new ModuleId(ModuleB.Guid), () => new ModuleB() }
        };

        protected override ModuleId GetModuleId()
        {
            return new ModuleId(Guid);
        }

        protected override IModuleInformation GetModuleInformation()
        {
            return new BasicModuleInformation("fooA", "barA");
        }

        protected override IModuleVersion GetModuleVersion()
        {
            return new MajorMinorPatchModuleVersion(1, 0, 0);
        }
    }

    internal sealed class ModuleB() : LibraryModule(Dependencies)
    {
        public static readonly Guid Guid = Guid.Parse("a45e2506-75ac-47d3-b51f-d149b85c938c");

        public static readonly ModuleDependenciesDictionary Dependencies = new()
        {
            { new ModuleId(ModuleA.Guid), () => new ModuleA() }
        };
        protected override ModuleId GetModuleId()
        {
            return new ModuleId(Guid);
        }
        protected override IModuleInformation GetModuleInformation()
        {
            return new BasicModuleInformation("fooB", "barB");
        }
        protected override IModuleVersion GetModuleVersion()
        {
            return new MajorMinorPatchModuleVersion(1, 0, 0);
        }
    }

    [Fact]
    public async Task Test_Can_Load_Nested_And_Cyclic_Dependencies()
    {
        var moduleLoader = new LibraryModuleLoader(
            new LibraryModuleConfigurator(),
            new LibraryModuleMediatorConfigurator(),
            new Microsoft.Extensions.Logging.Abstractions.NullLogger<LibraryModuleLoader>()
        );

        await moduleLoader.LoadAsync(new Dictionary<ModuleId, Func<ILibraryModule>>()
        {
            { new ModuleId(ModuleA.Guid), () => new ModuleA() },
            { new ModuleId(ModuleB.Guid), () => new ModuleB() }
        }, new ServiceCollection(), new ConfigurationManager(), new HostEnvironment(), CancellationToken.None);
    }
}
