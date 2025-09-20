using FrenchExDev.Net.Dotnet.Project.Types.DesktopProjectType.Abstractions;

namespace FrenchExDev.Net.Dotnet.Project.Types.DesktopProjectType;

/// <summary>
/// Provides utility methods for creating and configuring default <see cref="DesktopProjectModel"/> instances and builders for desktop projects.
/// </summary>
/// <remarks>
/// Use this class to quickly obtain a default desktop project model or a pre-configured builder for further customization.
/// Example usage:
/// <code>
/// var defaultModel = DesktopProjectTypeBuilder.Defaults;
/// var builder = new DesktopProjectTypeBuilder().Default();
/// builder.Name("MyDesktopApp");
/// var result = builder.Build();
/// </code>
/// </remarks>
public class DesktopProjectTypeBuilder
{
    /// <summary>
    /// Gets a default <see cref="DesktopProjectModel"/> instance with recommended settings for a Windows desktop application.
    /// </summary>
    /// <remarks>
    /// The default configuration includes:
    /// - Nullable enabled
    /// - Implicit usings enabled
    /// - C# 13.0
    /// - Output type "WinExe"
    /// - SDK "Microsoft.NET.Sdk.WindowsDesktop"
    /// - Target framework "net9.0-windows"
    /// </remarks>
    public static DesktopProjectModel Defaults => new DesktopProjectModelBuilder()
        .Nullable(true)
        .ImplicitUsings(true)
        .LangVersion("13.0")
        .OutputType("WinExe")
        .Sdk("Microsoft.NET.Sdk.WindowsDesktop")
        .TargetFramework("net9.0-windows")
        .BuildSuccess();

    /// <summary>
    /// Creates a <see cref="DesktopProjectModelBuilder"/> pre-configured with the default settings from <see cref="Defaults"/>.
    /// </summary>
    /// <remarks>
    /// Use this method to start with a default builder and customize additional properties as needed.
    /// Example:
    /// <code>
    /// var builder = new DesktopProjectTypeBuilder().Default();
    /// builder.Name("MyDesktopApp");
    /// </code>
    /// </remarks>
    /// <returns>A <see cref="DesktopProjectModelBuilder"/> instance with default settings applied.</returns>
    public DesktopProjectModelBuilder Default() => new DesktopProjectModelBuilder()
        .Nullable(Defaults.Nullable)
        .ImplicitUsings(Defaults.Nullable)
        .LangVersion(Defaults.LangVersion)
        .OutputType(Defaults.OutputType)
        .Sdk(Defaults.Sdk)
        .TargetFramework(Defaults.TargetFramework)
        .PackageReferences(Defaults.PackageReferences)
        .ProjectReferences(Defaults.ProjectReferences)
        .AdditionalProperties(Defaults.AdditionalProperties)
        ;

}
