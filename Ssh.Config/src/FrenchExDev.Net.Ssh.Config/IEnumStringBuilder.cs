#region Licensing

// Copyright Stéphane Erard
// For licensing, please contact stephane.erard@gmail

#endregion

namespace FrenchExDev.Net.Ssh.Config;

/// <summary>
/// Defines a mechanism for converting enumeration values of type TEnum to their string representations.
/// </summary>
/// <typeparam name="TEnum">The type of the enumeration value to be converted to a string.</typeparam>
public interface IEnumStringBuilder<in TEnum>
{
    /// <summary>
    /// Builds a string representation for the specified enumeration value.
    /// </summary>
    /// <param name="enum">The enumeration value to convert to its string representation.</param>
    /// <returns>A string that represents the specified enumeration value.</returns>
    string Build(TEnum @enum);
}