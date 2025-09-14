#region Licensing

// Copyright Stéphane Erard
// For licensing, please contact stephane.erard@gmail

#endregion

namespace FrenchExDev.Net.Ssh.Config;

/// <summary>
/// Provides functionality to convert values of the TunnelEnum enumeration to their corresponding string
/// representations.
/// </summary>
/// <remarks>This class is typically used to serialize TunnelEnum values for configuration or network
/// communication purposes. The string representations produced by this builder match the expected format for tunnel
/// type identifiers in related systems.</remarks>
public class TunnelEnumStringBuilder : IEnumStringBuilder<TunnelEnum>
{
    /// <summary>
    /// Converts the specified tunnel type enumeration value to its corresponding string representation.
    /// </summary>
    /// <param name="enum">The tunnel type to convert to a string value.</param>
    /// <returns>A string that represents the specified tunnel type. Possible values include "ethernet", "no", "yes", and
    /// "point-to-point".</returns>
    /// <exception cref="NotImplementedException">Thrown if the specified tunnel type is not supported.</exception>
    public string Build(TunnelEnum @enum)
    {
        return @enum switch
        {
            TunnelEnum.Ethernet => "ethernet",
            TunnelEnum.No => "no",
            TunnelEnum.Yes => "yes",
            TunnelEnum.PointToPoint => "point-to-point",
            _ => throw new NotImplementedException()
        };
    }
}