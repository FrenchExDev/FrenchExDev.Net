#region Licensing

// Copyright Stéphane Erard
// For licensing, please contact stephane.erard@gmail

#endregion

namespace FrenchExDev.Net.Ssh.Config;

/// <summary>
/// Specifies the type of tunneling protocol used for a network connection.
/// </summary>
/// <remarks>Use this enumeration to indicate whether a connection utilizes tunneling, and if so, the specific
/// tunneling protocol. The values represent common tunneling types, such as point-to-point and Ethernet tunneling, as
/// well as the absence of tunneling.</remarks>
public enum TunnelEnum
{
    Yes,
    PointToPoint,
    Ethernet,
    No
}