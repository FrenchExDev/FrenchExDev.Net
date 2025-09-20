#region Licensing

// Copyright Stéphane Erard
// For licensing please contact <stephane.erard@gmail.com>

#endregion

namespace FrenchExDev.Net.Alpine.Version;

/// <summary>
/// Represents a record containing a version string for an Alpine Linux distribution.
/// </summary>
/// <param name="Version">The version identifier of the Alpine Linux distribution. This value should follow the standard Alpine versioning
/// format, such as "3.18.2".</param>
public record AlpineVersionRecord(string Version);