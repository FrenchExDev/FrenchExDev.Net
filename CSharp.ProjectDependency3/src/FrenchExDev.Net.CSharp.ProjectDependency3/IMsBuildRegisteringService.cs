namespace FrenchExDev.Net.CSharp.ProjectDependency3;

/// <summary>
/// Provides a service for registering MSBuild components or functionality within the host environment.
/// </summary>
/// <remarks>Implementations of this interface should define the registration logic required to enable MSBuild
/// integration. This service is typically used to ensure that MSBuild-related features are available and properly
/// configured before use.</remarks>
public interface IMsBuildRegisteringService
{
    /// <summary>
    /// Ensures that the current instance is registered if it has not already been registered.
    /// </summary>
    void RegisterIfNeeded();
}
