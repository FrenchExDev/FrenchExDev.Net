#region Licensing

// Copyright Stéphane Erard
// For licensing please contact <stephane.erard@gmail.com>

#endregion

#region Usings


#endregion

namespace FrenchExDev.Net.Alpine.Version;


/// <summary>
/// Builder class for <see cref="AlpineVersionSearchingFilters"/>.
/// Allows fluent configuration of version search filters such as architectures, flavors, minimum/maximum/exact version, and release candidate flag.
/// </summary>
/// <remarks>
/// Use the provided methods to specify search criteria, then call <see cref="Build"/> to create an <see cref="AlpineVersionSearchingFilters"/> instance.
/// <para>Example usage:
/// <code>
/// var filters = new AlpineVersionSearchingFiltersBuilder()
///     .WithMinimumVersion("3.18")
///     .WithArch(AlpineArchitectures.x86_64)
///     .WithFlavor(AlpineFlavors.Standard)
///     .WithRc()
///     .Build();
/// </code>
/// </para>
/// </remarks>
public class AlpineVersionSearchingFiltersBuilder
{
    private readonly List<AlpineArchitectures> _archs = new();
    private readonly List<AlpineFlavors> _flavors = new();
    private string? _exactVersion;
    private string? _maximumVersion;
    private string? _minimumVersion;
    private bool _rc;

    /// <summary>
    /// Sets the minimum version string for the filter.
    /// </summary>
    /// <remarks>
    /// Only Alpine versions greater than or equal to this value will be included in results.
    /// <para>Example: <c>.WithMinimumVersion("3.18")</c></para>
    /// </remarks>
    /// <param name="minimumVersion">Minimum version string (e.g., "3.18").</param>
    /// <returns>The builder instance for chaining.</returns>
    public AlpineVersionSearchingFiltersBuilder WithMinimumVersion(string minimumVersion)
    {
        _minimumVersion = minimumVersion;
        return this;
    }

    /// <summary>
    /// Sets the maximum version string for the filter.
    /// </summary>
    /// <remarks>
    /// Only Alpine versions less than or equal to this value will be included in results.
    /// <para>Example: <c>.WithMaximumVersion("3.19")</c></para>
    /// </remarks>
    /// <param name="maximumVersion">Maximum version string (e.g., "3.19").</param>
    /// <returns>The builder instance for chaining.</returns>
    public AlpineVersionSearchingFiltersBuilder WithMaximumVersion(string maximumVersion)
    {
        _maximumVersion = maximumVersion;
        return this;
    }

    /// <summary>
    /// Sets the exact version string for the filter.
    /// </summary>
    /// <remarks>
    /// Only Alpine versions matching this exact value will be included in results. Overrides minimum and maximum filters.
    /// <para>Example: <c>.WithExactVersion("3.18.2_rc1")</c></para>
    /// </remarks>
    /// <param name="exactVersion">Exact version string (e.g., "3.18.2_rc1").</param>
    /// <returns>The builder instance for chaining.</returns>
    public AlpineVersionSearchingFiltersBuilder WithExactVersion(string exactVersion)
    {
        _exactVersion = exactVersion;
        return this;
    }

    /// <summary>
    /// Adds multiple flavors to the filter.
    /// </summary>
    /// <remarks>
    /// Only results matching these flavors will be included. Flavors correspond to different ISO types (e.g., Standard, Virt).
    /// <para>Example: <c>.WithFlavors(new List&lt;AlpineFlavors&gt; { AlpineFlavors.Standard, AlpineFlavors.Virt })</c></para>
    /// </remarks>
    /// <param name="flavors">List of flavors to include.</param>
    /// <returns>The builder instance for chaining.</returns>
    public AlpineVersionSearchingFiltersBuilder WithFlavors(List<AlpineFlavors> flavors)
    {
        _flavors.AddRange(flavors);
        return this;
    }

    /// <summary>
    /// Adds a single flavor to the filter.
    /// </summary>
    /// <remarks>
    /// Only results matching this flavor will be included.
    /// <para>Example: <c>.WithFlavor(AlpineFlavors.Standard)</c></para>
    /// </remarks>
    /// <param name="flavor">Flavor to include.</param>
    /// <returns>The builder instance for chaining.</returns>
    public AlpineVersionSearchingFiltersBuilder WithFlavor(AlpineFlavors flavor)
    {
        _flavors.Add(flavor);
        return this;
    }

    /// <summary>
    /// Adds multiple architectures to the filter.
    /// </summary>
    /// <remarks>
    /// Only results matching these architectures will be included (e.g., x86_64, aarch64).
    /// <para>Example: <c>.WithArchs(new List&lt;AlpineArchitectures&gt; { AlpineArchitectures.x86_64 })</c></para>
    /// </remarks>
    /// <param name="archs">List of architectures to include.</param>
    /// <returns>The builder instance for chaining.</returns>
    public AlpineVersionSearchingFiltersBuilder WithArchs(List<AlpineArchitectures> archs)
    {
        _archs.AddRange(archs);
        return this;
    }

    /// <summary>
    /// Adds a single architecture to the filter.
    /// </summary>
    /// <remarks>
    /// Only results matching this architecture will be included.
    /// <para>Example: <c>.WithArch(AlpineArchitectures.x86_64)</c></para>
    /// </remarks>
    /// <param name="arch">Architecture to include.</param>
    /// <returns>The builder instance for chaining.</returns>
    public AlpineVersionSearchingFiltersBuilder WithArch(AlpineArchitectures arch)
    {
        _archs.Add(arch);
        return this;
    }

    /// <summary>
    /// Sets the release candidate flag for the filter.
    /// </summary>
    /// <remarks>
    /// If set to <c>true</c>, release candidate (RC) versions will be included in results.
    /// <para>Example: <c>.WithRc()</c> or <c>.WithRc(true)</c></para>
    /// </remarks>
    /// <param name="rc">Whether to include RC versions (default: true).</param>
    /// <returns>The builder instance for chaining.</returns>
    public AlpineVersionSearchingFiltersBuilder WithRc(bool rc = true)
    {
        _rc = rc;
        return this;
    }

    /// <summary>
    /// Builds and returns an <see cref="AlpineVersionSearchingFilters"/> instance based on the configured criteria.
    /// </summary>
    /// <remarks>
    /// Call this method after configuring the builder to obtain a filter object for use in search operations.
    /// <para>Example:
    /// <code>
    /// var filters = new AlpineVersionSearchingFiltersBuilder()
    ///     .WithMinimumVersion("3.18")
    ///     .WithArch(AlpineArchitectures.x86_64)
    ///     .Build();
    /// </code>
    /// </para>
    /// </remarks>
    /// <returns>A configured <see cref="AlpineVersionSearchingFilters"/> instance.</returns>
    public AlpineVersionSearchingFilters Build()
    {
        return new AlpineVersionSearchingFilters
        {
            MinimumVersion = !string.IsNullOrEmpty(_minimumVersion) ? AlpineVersion.From(_minimumVersion) : null,
            MaximumVersion = !string.IsNullOrEmpty(_maximumVersion) ? AlpineVersion.From(_maximumVersion) : null,
            Flavors = _flavors,
            Architectures = _archs,
            Rc = _rc,
            ExactVersion = !string.IsNullOrEmpty(_exactVersion) ? AlpineVersion.From(_exactVersion) : null
        };
    }
}
