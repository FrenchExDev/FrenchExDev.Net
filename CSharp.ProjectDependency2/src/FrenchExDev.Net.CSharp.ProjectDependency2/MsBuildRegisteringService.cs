using Microsoft.Build.Locator;

namespace FrenchExDev.Net.CSharp.ProjectDependency2;

/// <summary>
/// Provides functionality to register MSBuild assemblies for use within the current application domain.
/// </summary>
/// <remarks>This service ensures that MSBuild is registered only once per application domain. Registering MSBuild
/// enables APIs that depend on MSBuild assemblies to function correctly. Typically, registration should occur before
/// invoking any MSBuild-dependent operations.</remarks>
public class MsBuildRegisteringService : IMsBuildRegisteringService
{
    private bool _isRegistered = false;
    public void RegisterIfNeeded()
    {
        if (_isRegistered) return;

        if (!MSBuildLocator.IsRegistered)
        {
            MSBuildLocator.RegisterDefaults();
        }

        _isRegistered = true;
    }
}
