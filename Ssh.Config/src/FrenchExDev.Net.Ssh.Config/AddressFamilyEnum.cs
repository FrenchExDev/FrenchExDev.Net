#region Licensing

// Copyright Stéphane Erard
// For licensing, please contact stephane.erard@gmail

#endregion

namespace FrenchExDev.Net.Ssh.Config;

/// <summary>
/// Specifies the supported address families for network operations.
/// </summary>
/// <remarks>Use this enumeration to indicate whether an operation should target IPv4 addresses, IPv6 addresses,
/// or both. The values correspond to common address families used in networking APIs.</remarks>
public enum AddressFamilyEnum
{
    Inet,
    Inet6,
    All
}