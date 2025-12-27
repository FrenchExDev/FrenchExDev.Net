#region Licensing

// Copyright Stéphane Erard
// For licensing, please contact stephane.erard@gmail

#endregion

namespace FrenchExDev.Net.Ssh.Config;

/// <summary>
/// Provides functionality to parse string values into corresponding <see cref="ControlMasterEnum"/> enumeration values.
/// </summary>
/// <remarks>Use this class to convert configuration string representations of ControlMaster options to their
/// strongly-typed enum equivalents. The parsing is case-insensitive and supports the values "yes", "ask", "auto", and
/// "auto-ask". An exception is thrown for unsupported values.</remarks>
public class ControlMasterEnumReader
{
    /// <summary>
    /// Converts the specified string representation to its corresponding ControlMasterEnum value.
    /// </summary>
    /// <remarks>The comparison is case-insensitive. Only the values "yes", "ask", "auto", and "auto-ask" are
    /// recognized; all other values will result in an exception.</remarks>
    /// <param name="value">The string value to convert. Supported values are "yes", "ask", "auto", and "auto-ask" (case-insensitive).</param>
    /// <returns>The ControlMasterEnum value that corresponds to the specified string.</returns>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="value"/> does not match any supported option.</exception>
    public ControlMasterEnum Read(string value)
    {
        return value.ToLowerInvariant() switch
        {
            "yes" => ControlMasterEnum.Yes,
            "ask" => ControlMasterEnum.Ask,
            "auto" => ControlMasterEnum.Auto,
            "auto-ask" => ControlMasterEnum.AutoAsk,
            _ => throw new NotImplementedException()
        };
    }
}