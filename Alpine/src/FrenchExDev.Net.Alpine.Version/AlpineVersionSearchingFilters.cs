#region Licensing

// Copyright Stéphane Erard
// For licensing please contact <stephane.erard@gmail.com>

#endregion

#region Usings

#endregion

namespace FrenchExDev.Net.Alpine.Version;

[Serializable]
public class AlpineVersionSearchingFilters
{
    /// <summary>
    /// Minimum Alpine version to include in search results.
    /// </summary>
    /// <remarks>
    /// If set, only versions greater than or equal to this value will be returned.
    /// <para>Example: <c>new AlpineVersion { Major = "3", Minor = "18" }</c></para>
    /// </remarks>
    public AlpineVersion? MinimumVersion { get; init; }

    /// <summary>
    /// Maximum Alpine version to include in search results.
    /// </summary>
    /// <remarks>
    /// If set, only versions less than or equal to this value will be returned.
    /// <para>Example: <c>new AlpineVersion { Major = "3", Minor = "19" }</c></para>
    /// </remarks>
    public AlpineVersion? MaximumVersion { get; init; }

    /// <summary>
    /// List of Alpine flavors to include in search results.
    /// </summary>
    /// <remarks>
    /// If set, only results matching these flavors will be returned. Flavors correspond to different ISO types (e.g., Standard, Virt).
    /// <para>Example: <c>new List<AlpineFlavors> { AlpineFlavors.Standard, AlpineFlavors.Virt }</c></para>
    /// </remarks>
    public List<AlpineFlavors>? Flavors { get; init; }

    /// <summary>
    /// List of Alpine architectures to include in search results.
    /// </summary>
    /// <remarks>
    /// If set, only results matching these architectures will be returned (e.g., x86_64, aarch64).
    /// <para>Example: <c>new List<AlpineArchitectures> { AlpineArchitectures.x86_64 }</c></para>
    /// </remarks>
    public List<AlpineArchitectures>? Architectures { get; init; }

    /// <summary>
    /// Whether to include release candidate (RC) versions in search results.
    /// </summary>
    /// <remarks>
    /// If <c>true</c>, RC versions (e.g., 3.18.0_rc1) will be included. If <c>false</c>, only stable versions are returned.
    /// <para>Example: <c>Rc = true</c> to include RCs.</para>
    /// </remarks>
    public bool Rc { get; init; }

    /// <summary>
    /// Exact Alpine version to match in search results.
    /// </summary>
    /// <remarks>
    /// If set, only results matching this exact version will be returned. Overrides minimum and maximum version filters.
    /// <para>Example: <c>new AlpineVersion { Major = "3", Minor = "18", Patch = "2" }</c></para>
    /// </remarks>
    public AlpineVersion? ExactVersion { get; set; }
}