#region Licensing

// Copyright Stéphane Erard
// All rights reserved

#endregion

namespace FrenchexDev.VirtualBox.Net;

/// <summary>
/// Represents VirtualBox version information, including the version string and the SHA-256 hash of the associated Guest
/// Additions ISO.
/// </summary>
/// <param name="Version">The VirtualBox version string, typically in the format 'major.minor.patch'.</param>
/// <param name="AdditionsIsoSha256">The SHA-256 hash of the VirtualBox Guest Additions ISO file corresponding to the specified version.</param>
public record VirtualBoxVersionInfos(string Version, string AdditionsIsoSha256);