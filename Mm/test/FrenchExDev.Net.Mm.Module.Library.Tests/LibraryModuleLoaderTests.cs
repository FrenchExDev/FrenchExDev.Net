using FrenchExDev.Net.Mm.Abstractions;
using FrenchExDev.Net.Mm.Module.Library.Abstractions;
using FrenchExDev.Net.Mm.Module.Library.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting.Internal;

namespace FrenchExDev.Net.Mm.Module.Library.Tests;

public class LibraryModuleLoaderTests
{
    internal sealed class ModuleA() : LibraryModule(Dependencies)
    {
        public static readonly ModuleDependenciesDictionary Dependencies = new()
        {
            { new ModuleId(Guid.Parse("b07SDfe0-4323-4fbe-81f0-260792ffe243")), () => new ModuleB()   }
        };

        protected override ModuleId GetModuleId()
        {
            return new ModuleId(Guid.Parse("a07SDfe0-4323-4fbe-81f0-260792ffe243"));
        }

        protected override IModuleInformation GetModuleInformation()
        {
            return new BasicModuleInformation("foo", "bar");
        }

        protected override IModuleVersion GetModuleVersion()
        {
            return new MajorMinorPatchModuleVersion(1, 0, 0);
        }
    }

    internal sealed class ModuleB() : LibraryModule(Dependencies)
    {
        public static readonly ModuleDependenciesDictionary Dependencies = new()
        {
            { new ModuleId(Guid.Parse("a07SDfe0-4323-4fbe-81f0-260792ffe243")), () => new ModuleA() }
        };
        protected override ModuleId GetModuleId()
        {
            return new ModuleId(Guid.Parse("b07SDfe0-4323-4fbe-81f0-260792ffe243"));
        }
        protected override IModuleInformation GetModuleInformation()
        {
            return new BasicModuleInformation("foo", "bar");
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
            { new ModuleId(Guid.Parse("a07SDfe0-4323-4fbe-81f0-260792ffe243")), () => new ModuleA() },
            { new ModuleId(Guid.Parse("b07SDfe0-4323-4fbe-81f0-260792ffe243")), () => new ModuleB() }
        }, new ServiceCollection(), new ConfigurationManager(), new HostingEnvironment(), CancellationToken.None);
    }
}
