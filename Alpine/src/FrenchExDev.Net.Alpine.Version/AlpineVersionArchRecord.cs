#region Licensing

// Copyright Stéphane Erard
// For licensing please contact <stephane.erard@gmail.com>

#endregion

namespace FrenchExDev.Net.Alpine.Version;

/// <summary>
/// Represents a record containing an Alpine Linux version and its associated architecture.
/// </summary>
/// <param name="Version">The version string of Alpine Linux. This typically follows the format 'major.minor' (for example, '3.18'). Cannot be
/// null.</param>
/// <param name="Architecture">The architecture identifier for the Alpine Linux version (for example, 'x86_64', 'arm64'). Cannot be null.</param>
public record AlpineVersionArchRecord(string Version, string Architecture);