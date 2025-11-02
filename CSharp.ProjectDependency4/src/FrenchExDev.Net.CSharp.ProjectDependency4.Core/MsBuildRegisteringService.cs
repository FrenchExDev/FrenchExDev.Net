using Microsoft.Build.Locator;

namespace FrenchExDev.Net.CSharp.ProjectDependency4.Core;

/// <summary>
/// Provides functionality to register MSBuild assemblies for use within the current application domain.
/// </summary>
/// <remarks>This service ensures that MSBuild is registered only once per application domain. Registering MSBuild
/// enables APIs that depend on MSBuild assemblies to function correctly. Typically, registration should occur before
/// invoking any MSBuild-dependent operations.</remarks>
public class MsBuildRegisteringService : IMsBuildRegisteringService
{
    private bool _isRegistered = false;

    /// <summary>
    /// Ensures that the MSBuild assemblies are registered with the default settings if they have not already been
    /// registered.
    /// </summary>
    /// <remarks>This method is idempotent and will only perform registration once per instance, even if
    /// called multiple times. Registration is required before using MSBuild APIs that depend on the locator.</remarks>
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
