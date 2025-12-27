#region Licensing

// Copyright Stéphane Erard
// For licensing, please contact stephane.erard@gmail

#endregion

namespace FrenchExDev.Net.Ssh.Config;

/// <summary>
/// Provides functionality to convert string representations of tunnel types to their corresponding <see
/// cref="TunnelEnum"/> values.
/// </summary>
/// <remarks>Use this class to parse tunnel type strings received from external sources into strongly typed <see
/// cref="TunnelEnum"/> values. The supported string values are "no", "ethernet", "point-to-point", and "yes". If an
/// unsupported value is provided, an exception is thrown.</remarks>
public class TunnelEnumReader
{
    /// <summary>
    /// Converts the specified string representation of a tunnel type to its corresponding <see cref="TunnelEnum"/>
    /// value.
    /// </summary>
    /// <param name="value">The string value representing a tunnel type. Valid values are "no", "ethernet", "point-to-point", and "yes". The
    /// comparison is case-sensitive.</param>
    /// <returns>The <see cref="TunnelEnum"/> value that corresponds to the specified string.</returns>
    /// <exception cref="NotImplementedException">Thrown when <paramref name="value"/> does not match any recognized tunnel type.</exception>
    public TunnelEnum Read(string value)
    {
        return value switch
        {
            "no" => TunnelEnum.No,
            "ethernet" => TunnelEnum.Ethernet,
            "point-to-point" => TunnelEnum.PointToPoint,
            "yes" => TunnelEnum.Yes,
            _ => throw new NotImplementedException()
        };
    }
}