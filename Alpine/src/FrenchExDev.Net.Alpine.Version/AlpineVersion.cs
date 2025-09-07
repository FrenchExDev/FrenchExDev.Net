#region Licensing

// Copyright Stéphane Erard
// For licensing please contact <stephane.erard@gmail.com>

#endregion

#region Usings

using System.Runtime.Serialization;

#endregion

namespace FrenchExDev.Net.Alpine.Version;

/// <summary>
/// Represents an Alpine Linux version with major, minor, and patch components, supporting edge and release candidate (rc) versions.
/// </summary>
/// <remarks>
/// Provides parsing, comparison, and string formatting for Alpine version strings. Handles numeric and special cases (e.g., "edge", "rc").
/// </remarks>
[Serializable]
public class AlpineVersion : IComparable<AlpineVersion>
{
    /// <summary>
    /// Major version component (string, may be "edge").
    /// </summary>
#pragma warning disable CS8618 // Un champ non-nullable doit contenir une valeur autre que Null lors de la fermeture du constructeur. Envisagez d’ajouter le modificateur « required » ou de déclarer le champ comme pouvant accepter la valeur Null.
    [DataMember] public string Major { get; set; }
#pragma warning restore CS8618 // Un champ non-nullable doit contenir une valeur autre que Null lors de la fermeture du constructeur. Envisagez d’ajouter le modificateur « required » ou de déclarer le champ comme pouvant accepter la valeur Null.
    /// <summary>
    /// Minor version component (string).
    /// </summary>
#pragma warning disable CS8618 // Un champ non-nullable doit contenir une valeur autre que Null lors de la fermeture du constructeur. Envisagez d’ajouter le modificateur « required » ou de déclarer le champ comme pouvant accepter la valeur Null.
    [DataMember] public string Minor { get; set; }
#pragma warning restore CS8618 // Un champ non-nullable doit contenir une valeur autre que Null lors de la fermeture du constructeur. Envisagez d’ajouter le modificateur « required » ou de déclarer le champ comme pouvant accepter la valeur Null.
    /// <summary>
    /// Patch version component (string, may include rc info).
    /// </summary>
#pragma warning disable CS8618 // Un champ non-nullable doit contenir une valeur autre que Null lors de la fermeture du constructeur. Envisagez d’ajouter le modificateur « required » ou de déclarer le champ comme pouvant accepter la valeur Null.
    [DataMember] public string Patch { get; set; }
#pragma warning restore CS8618 // Un champ non-nullable doit contenir une valeur autre que Null lors de la fermeture du constructeur. Envisagez d’ajouter le modificateur « required » ou de déclarer le champ comme pouvant accepter la valeur Null.

    /// <summary>
    /// Comparer for major, minor, and patch components.
    /// </summary>
    [IgnoreDataMember]
    public static IComparer<AlpineVersion> MajorMinorPatchComparer { get; } = new MajorMinorPatchRelationalComparer();

    /// <summary>
    /// True if the major version is "edge".
    /// </summary>
    [IgnoreDataMember] public bool IsEdge => Major == "edge";
    /// <summary>
    /// True if the major version is numeric.
    /// </summary>
    [IgnoreDataMember] public bool IsMajorNumber => int.TryParse(Major, out var value);
    /// <summary>
    /// Numeric value of major version, or 0 if not numeric.
    /// </summary>
    [IgnoreDataMember] public int MajorNumber => IsMajorNumber ? int.Parse(Major) : 0;
    /// <summary>
    /// True if the minor version is numeric.
    /// </summary>
    [IgnoreDataMember] public bool IsMinorNumber => int.TryParse(Minor, out var value);
    /// <summary>
    /// Numeric value of minor version, or 0 if not numeric.
    /// </summary>
    [IgnoreDataMember] public int MinorNumber => IsMinorNumber ? int.Parse(Minor) : 0;

    /// <summary>
    /// True if the patch version is numeric.
    /// </summary>
    [IgnoreDataMember]
    public bool IsPatchNumber => int.TryParse(Patch.Split("_").FirstOrDefault() ?? "0", out var value);
    /// <summary>
    /// Numeric value of patch version, or 0 if not numeric.
    /// </summary>
    [IgnoreDataMember] public int PatchNumber => IsPatchNumber ? int.Parse(Patch.Split("_").First()) : 0;
    /// <summary>
    /// Release candidate (rc) string if present in patch.
    /// </summary>
    [IgnoreDataMember] public string Rc => Patch.Split("_").Length > 1 ? Patch.Split("_")[1] : string.Empty;
    /// <summary>
    /// True if patch contains rc information.
    /// </summary>
    [IgnoreDataMember] public bool IsRcNumber => Patch.Split("_").Length > 1;
    /// <summary>
    /// Numeric value of rc, or 0 if not present.
    /// </summary>
    [IgnoreDataMember] public int RcNumber => IsRcNumber ? int.Parse(Patch.Split("_")[1].Replace("rc", "")) : 0;

    /// <summary>
    /// Compares this version to another using major, minor, and patch components.
    /// </summary>
    public int CompareTo(AlpineVersion? other)
    {
        return MajorMinorPatchComparer.Compare(this, other);
    }

    /// <summary>
    /// Returns the full version string (major.minor.patch).
    /// </summary>
    public override string ToString()
    {
        if (!string.IsNullOrEmpty(Patch))
            return $"{Major}.{Minor}.{Patch}";

        if (!string.IsNullOrEmpty(Minor))
            return $"{Major}.{Minor}";

        return $"{Major}";
    }

    /// <summary>
    /// Returns the major.minor version string, or "edge" if major is "edge".
    /// </summary>
    public string ToMajorMinor()
    {
        return Major == "edge" ? "edge" : $"{Major}.{Minor}";
    }

    /// <summary>
    /// Parses a version string into an <see cref="AlpineVersion"/> instance.
    /// </summary>
    /// <param name="versionString">Version string (e.g., "3.18.2_rc1").</param>
    /// <returns>Parsed <see cref="AlpineVersion"/>.</returns>
    public static AlpineVersion From(string versionString)
    {
        var split = versionString.Split(".");

        var major = split[0];
        var minor = split.Length > 1 ? split[1] : string.Empty;
        var patch = split.Length > 2 ? split[2] : string.Empty;

        return new AlpineVersion
        {
            Major = major,
            Minor = minor,
            Patch = patch
        };
    }

    /// <summary>
    /// Returns the major.minor version string formatted for URLs (e.g., "v3.18").
    /// </summary>
    public string ToMajorMinorUrl()
    {
        return Major == "edge" ? "edge" : "v" + ToMajorMinor();
    }
    /// <summary>
    /// Specifies the comparison operators that can be used in conditional expressions.
    /// </summary>
    /// <remarks>This enumeration defines a set of operators commonly used for comparing values.  It can be
    /// used in scenarios such as filtering, sorting, or evaluating conditions.</remarks>
    public enum Operator
    {
        Equal,
        NotEqual,
        GreaterThan,
        GreaterThanOrEqual,
        LessThan,
        LessThanOrEqual
    }

    /// <summary>
    /// Compares two <see cref="AlpineVersion"/> instances using the specified comparison operator.
    /// </summary>
    /// <param name="left">The first <see cref="AlpineVersion"/> instance to compare.</param>
    /// <param name="operator">The <see cref="Operator"/> that specifies the type of comparison to perform. Supported operators include <see
    /// cref="Operator.Equal"/>, <see cref="Operator.NotEqual"/>,  <see cref="Operator.GreaterThan"/>, <see
    /// cref="Operator.GreaterThanOrEqual"/>,  <see cref="Operator.LessThan"/>, and <see
    /// cref="Operator.LessThanOrEqual"/>.</param>
    /// <param name="right">The second <see cref="AlpineVersion"/> instance to compare.</param>
    /// <returns><see langword="true"/> if the comparison between <paramref name="left"/> and <paramref name="right"/>  evaluates
    /// to true based on the specified <paramref name="operator"/>; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="NotSupportedException">Thrown if the specified <paramref name="operator"/> is not a supported comparison operator.</exception>
    public static bool Compare(AlpineVersion left, Operator @operator, AlpineVersion right)
    {
        return (@operator) switch
        {
            Operator.Equal => AlpineVersion.MajorMinorPatchComparer.Compare(left, right) == 0,
            Operator.NotEqual => AlpineVersion.MajorMinorPatchComparer.Compare(left, right) != 0,
            Operator.GreaterThan => AlpineVersion.MajorMinorPatchComparer.Compare(left, right) > 0,
            Operator.GreaterThanOrEqual => AlpineVersion.MajorMinorPatchComparer.Compare(left, right) >= 0,
            Operator.LessThan => AlpineVersion.MajorMinorPatchComparer.Compare(left, right) < 0,
            Operator.LessThanOrEqual => AlpineVersion.MajorMinorPatchComparer.Compare(left, right) <= 0,
            _ => throw new NotSupportedException($"Unsupported operator: {@operator}")
        };
    }

    /// <summary>
    /// Internal comparer for AlpineVersion objects based on major, minor, patch, and rc components.
    /// </summary>
    public sealed class MajorMinorPatchRelationalComparer : IComparer<AlpineVersion>
    {
        
        /// <summary>
        /// Compares two <see cref="AlpineVersion"/> objects and determines their relative order.
        /// </summary>
        /// <remarks>The comparison considers the following factors in order of precedence: <list
        /// type="number"> <item><description>Whether the version is marked as "edge".</description></item>
        /// <item><description>Major version number, if both versions have one.</description></item>
        /// <item><description>Minor version number, if both versions have one and the major versions are
        /// equal.</description></item> <item><description>Patch version number, if both versions have one and the minor
        /// versions are equal.</description></item> <item><description>Release candidate (RC) number, if both versions
        /// have one.</description></item> </list> If either <paramref name="x"/> or <paramref name="y"/> is <see
        /// langword="null"/>, the non-<see langword="null"/> object is considered greater.</remarks>
        /// <param name="x">The first <see cref="AlpineVersion"/> to compare. Can be <see langword="null"/>.</param>
        /// <param name="y">The second <see cref="AlpineVersion"/> to compare. Can be <see langword="null"/>.</param>
        /// <returns>A signed integer that indicates the relative order of the two objects: <list type="bullet">
        /// <item><description>Returns <c>-1</c> if <paramref name="x"/> is less than <paramref
        /// name="y"/>.</description></item> <item><description>Returns <c>0</c> if <paramref name="x"/> is equal to
        /// <paramref name="y"/>.</description></item> <item><description>Returns <c>1</c> if <paramref name="x"/> is
        /// greater than <paramref name="y"/>.</description></item> </list></returns>
        public int Compare(AlpineVersion? x, AlpineVersion? y)
        {
            if (ReferenceEquals(x, y)) return 0;
            if (ReferenceEquals(null, y)) return 1;
            if (ReferenceEquals(null, x)) return -1;

            if (!x.IsEdge && y.IsEdge) return -1;
            if (x.IsEdge && y.IsEdge) return 0;
            if (x.IsEdge && !y.IsEdge) return 1;

            var xIsMajorNumber = x.IsMajorNumber;
            var xMajorNumber = x.MajorNumber;

            var yIsMajorNumber = y.IsMajorNumber;
            var yMajorNumber = y.MajorNumber;

            var majorComparison = xIsMajorNumber && yIsMajorNumber
                ? xMajorNumber == yMajorNumber ? 0 : xMajorNumber > yMajorNumber ? 1 : -1
                : 0;
            if (majorComparison != 0) return majorComparison;

            var xIsMinorNumber = x.IsMinorNumber;
            var xMinorNumber = x.MinorNumber;

            var yIsMinorNumber = y.IsMinorNumber;
            var yMinorNumber = y.MinorNumber;

            var minorComparison = xIsMinorNumber && yIsMinorNumber && xMajorNumber >= yMajorNumber
                ? xMinorNumber == yMinorNumber ? 0 : xMinorNumber > yMinorNumber ? 1 : -1
                : 0;
            if (minorComparison != 0) return minorComparison;

            var xIsPatchNumber = x.IsPatchNumber;
            var xPatchNumber = x.PatchNumber;
            var yIsPatchNumber = y.IsPatchNumber;
            var yPatchNumber = y.PatchNumber;

            var patchComparison =
                xIsPatchNumber && yIsPatchNumber && xIsMinorNumber && yIsMinorNumber && xMinorNumber == yMinorNumber
                    ? xPatchNumber == yPatchNumber
                        ? 0
                        : xIsPatchNumber && yIsPatchNumber && xIsMinorNumber && yIsMinorNumber &&
                          xMinorNumber == yMinorNumber && xPatchNumber > yPatchNumber
                            ? 1
                            : -1
                    : xIsPatchNumber && !yIsPatchNumber
                        ? -1
                        : !xIsPatchNumber && yIsPatchNumber
                            ? yPatchNumber == 0 ? 0
                            : yPatchNumber > 0 ? -1 : 0
                            : 0;
            if (patchComparison != 0) return patchComparison;

            var xIsRcNumber = x.IsRcNumber;
            var xRcNumber = x.RcNumber;
            var yIsRcNumber = y.IsRcNumber;
            var yRcNumber = y.RcNumber;

            var rcComparison = xIsRcNumber && yIsRcNumber
                ? xRcNumber == yRcNumber ? 0 : xRcNumber > yRcNumber ? 1 : xRcNumber < yRcNumber ? -1 : 0
                : xIsRcNumber && !yIsRcNumber
                    ? 1
                    : !xIsMajorNumber && yIsRcNumber
                        ? -1
                        : 0;

            if (rcComparison != 0) return rcComparison;

            return 0;
        }
    }
}
