using FrenchExDev.Net.Mm.Abstractions;
using FrenchExDev.Net.Mm.Module.Library.Abstractions;
using FrenchExDev.Net.Mm.Module.Library.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Shouldly;

namespace FrenchExDev.Net.Mm.Module.Library.Tests;

/// <summary>
/// Tests for the <see cref="LibraryModuleLoader"/> class, focusing on loading modules with indirect, nested, and cyclic dependencies.
/// </summary>
public class LibraryModuleLoaderTests
{
    #region Test classes with cyclic dependencies

    internal sealed class ModuleA() : LibraryModule(Dependencies)
    {
        public static readonly Guid Guid = Guid.Parse("6102d0af-cb8b-492a-b746-1e9deb31106e");

        public static readonly LoadableLibraryModules Dependencies = new LoadableLibraryModules()
            .Add(ModuleB.Guid, () => new ModuleB());

        public bool HasVisitedConfigureServicesAsync { get; private set; } = false;
        public bool HasVisitedConfigureConfigurationAsync { get; private set; } = false;
        public bool HasVisitedConfigureDependenciesAsync { get; private set; } = false;

        public override Task ConfigureConfigurationAsync(IConfigurationManager configurationManager, IHostEnvironment hostEnvironment, CancellationToken cancellationToken = default)
        {
            HasVisitedConfigureConfigurationAsync = true;
            return base.ConfigureConfigurationAsync(configurationManager, hostEnvironment, cancellationToken);
        }

        public override Task ConfigureServicesAsync(IServiceCollection serviceCollection, IConfigurationManager configurationManager, IHostEnvironment hostEnvironment, CancellationToken cancellationToken = default)
        {
            HasVisitedConfigureServicesAsync = true;
            return base.ConfigureServicesAsync(serviceCollection, configurationManager, hostEnvironment, cancellationToken);
        }

        public override Task ConfigureDependenciesAsync(ILibraryModuleLoader libraryModuleLoader, IServiceCollection serviceCollection, IConfigurationManager configurationManager, IHostEnvironment hostEnvironment, CancellationToken cancellationToken = default)
        {
            HasVisitedConfigureDependenciesAsync = true;
            return base.ConfigureDependenciesAsync(libraryModuleLoader, serviceCollection, configurationManager, hostEnvironment, cancellationToken);
        }

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

        public static readonly LoadableLibraryModules Dependencies = new LoadableLibraryModules()
            .Add(ModuleA.Guid, () => new ModuleA())
            .Add(ModuleC.Guid, () => new ModuleC())
            ;
        public bool HasVisitedConfigureServicesAsync { get; private set; } = false;
        public bool HasVisitedConfigureConfigurationAsync { get; private set; } = false;
        public bool HasVisitedConfigureDependenciesAsync { get; private set; } = false;
        public override Task ConfigureConfigurationAsync(IConfigurationManager configurationManager, IHostEnvironment hostEnvironment, CancellationToken cancellationToken = default)
        {
            HasVisitedConfigureConfigurationAsync = true;
            return base.ConfigureConfigurationAsync(configurationManager, hostEnvironment, cancellationToken);
        }

        public override Task ConfigureServicesAsync(IServiceCollection serviceCollection, IConfigurationManager configurationManager, IHostEnvironment hostEnvironment, CancellationToken cancellationToken = default)
        {
            HasVisitedConfigureServicesAsync = true;
            return base.ConfigureServicesAsync(serviceCollection, configurationManager, hostEnvironment, cancellationToken);
        }

        public override Task ConfigureDependenciesAsync(ILibraryModuleLoader libraryModuleLoader, IServiceCollection serviceCollection, IConfigurationManager configurationManager, IHostEnvironment hostEnvironment, CancellationToken cancellationToken = default)
        {
            HasVisitedConfigureDependenciesAsync = true;
            return base.ConfigureDependenciesAsync(libraryModuleLoader, serviceCollection, configurationManager, hostEnvironment, cancellationToken);
        }
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

    internal sealed class ModuleC() : LibraryModule(Dependencies)
    {
        public static readonly Guid Guid = Guid.Parse("89423378-8018-4570-a11d-d06d122b3305");

        public static readonly LoadableLibraryModules Dependencies = new LoadableLibraryModules()
            .Add(ModuleA.Guid, () => new ModuleA())
            .Add(ModuleB.Guid, () => new ModuleB());

        public bool HasVisitedConfigureServicesAsync { get; private set; } = false;
        public bool HasVisitedConfigureConfigurationAsync { get; private set; } = false;
        public bool HasVisitedConfigureDependenciesAsync { get; private set; } = false;
        public override Task ConfigureConfigurationAsync(IConfigurationManager configurationManager, IHostEnvironment hostEnvironment, CancellationToken cancellationToken = default)
        {
            HasVisitedConfigureConfigurationAsync = true;
            return base.ConfigureConfigurationAsync(configurationManager, hostEnvironment, cancellationToken);
        }

        public override Task ConfigureDependenciesAsync(ILibraryModuleLoader libraryModuleLoader, IServiceCollection serviceCollection, IConfigurationManager configurationManager, IHostEnvironment hostEnvironment, CancellationToken cancellationToken = default)
        {
            HasVisitedConfigureDependenciesAsync = true;
            return base.ConfigureDependenciesAsync(libraryModuleLoader, serviceCollection, configurationManager, hostEnvironment, cancellationToken);
        }

        public override Task ConfigureServicesAsync(IServiceCollection serviceCollection, IConfigurationManager configurationManager, IHostEnvironment hostEnvironment, CancellationToken cancellationToken = default)
        {
            HasVisitedConfigureServicesAsync = true;
            return base.ConfigureServicesAsync(serviceCollection, configurationManager, hostEnvironment, cancellationToken);
        }

        protected override ModuleId GetModuleId()
        {
            return new ModuleId(Guid);
        }
        protected override IModuleInformation GetModuleInformation()
        {
            return new BasicModuleInformation("fooC", "barC");
        }
        protected override IModuleVersion GetModuleVersion()
        {
            return new MajorMinorPatchModuleVersion(1, 0, 0);
        }
    }

    #endregion

    [Fact]
    public async Task Test_Can_Load_Indirect_And_Nested_And_Cyclic_Dependencies_Loading_A()
    {
        var libraryModuleLoader = new LibraryModuleLoader(
            new LibraryModuleConfigurator(),
            new LibraryModuleMediatorConfigurator(),
            new Microsoft.Extensions.Logging.Abstractions.NullLogger<LibraryModuleLoader>()
        );

        await libraryModuleLoader.LoadAsync(
            new LoadableLibraryModules()
            .Add(ModuleA.Guid, () => new ModuleA()),
            new ServiceCollection(),
            new ConfigurationManager(),
            new HostEnvironment(),
            CancellationToken.None);

        Asserts(libraryModuleLoader);
    }

    [Fact]
    public async Task Test_Can_Load_Indirect_And_Nested_And_Cyclic_Dependencies_Loading_B()
    {
        var libraryModuleLoader = new LibraryModuleLoader(
            new LibraryModuleConfigurator(),
            new LibraryModuleMediatorConfigurator(),
            new Microsoft.Extensions.Logging.Abstractions.NullLogger<LibraryModuleLoader>()
        );

        await libraryModuleLoader.LoadAsync(
            new LoadableLibraryModules()
            .Add(ModuleB.Guid, () => new ModuleB()),
            new ServiceCollection(),
            new ConfigurationManager(),
            new HostEnvironment(),
            CancellationToken.None);

        Asserts(libraryModuleLoader);
    }

    [Fact]
    public async Task Test_Can_Load_Indirect_And_Nested_And_Cyclic_Dependencies_Loading_C()
    {
        var libraryModuleLoader = new LibraryModuleLoader(
            new LibraryModuleConfigurator(),
            new LibraryModuleMediatorConfigurator(),
            new Microsoft.Extensions.Logging.Abstractions.NullLogger<LibraryModuleLoader>()
        );

        await libraryModuleLoader.LoadAsync(
            new LoadableLibraryModules()
            .Add(ModuleC.Guid, () => new ModuleC()),
            new ServiceCollection(),
            new ConfigurationManager(),
            new HostEnvironment(),
            CancellationToken.None);

        Asserts(libraryModuleLoader);
    }


    private static void Asserts(LibraryModuleLoader libraryModuleLoader)
    {
        libraryModuleLoader.LoadedModules.ContainsKey(new ModuleId(ModuleA.Guid)).ShouldBeTrue();
        libraryModuleLoader.LoadedModules.ContainsKey(new ModuleId(ModuleB.Guid)).ShouldBeTrue();
        libraryModuleLoader.LoadedModules.ContainsKey(new ModuleId(ModuleC.Guid)).ShouldBeTrue();

        var moduleA = (ModuleA)libraryModuleLoader.LoadedModules.Get(ModuleA.Guid);
        moduleA.HasVisitedConfigureConfigurationAsync.ShouldBeTrue();
        moduleA.HasVisitedConfigureServicesAsync.ShouldBeTrue();
        moduleA.HasVisitedConfigureDependenciesAsync.ShouldBeTrue();

        var moduleB = (ModuleB)libraryModuleLoader.LoadedModules.Get(ModuleB.Guid);
        moduleB.HasVisitedConfigureConfigurationAsync.ShouldBeTrue();
        moduleB.HasVisitedConfigureServicesAsync.ShouldBeTrue();
        moduleB.HasVisitedConfigureDependenciesAsync.ShouldBeTrue();

        var moduleC = (ModuleC)libraryModuleLoader.LoadedModules.Get(ModuleC.Guid);
        moduleC.HasVisitedConfigureConfigurationAsync.ShouldBeTrue();
        moduleC.HasVisitedConfigureServicesAsync.ShouldBeTrue();
        moduleC.HasVisitedConfigureDependenciesAsync.ShouldBeTrue();
    }
}
