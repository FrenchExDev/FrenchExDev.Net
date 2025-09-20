#region Licensing

// Copyright Stéphane Erard
// For licensing please contact <stephane.erard@gmail.com>

#endregion

namespace FrenchExDev.Net.Alpine.Version;

/// <summary>
/// Specifies the supported CPU architectures for Alpine Linux distributions.
/// </summary>
/// <remarks>Use this enumeration to identify or select the target architecture when working with Alpine Linux
/// images or packages. The values correspond to common architecture identifiers used in Alpine repositories and
/// tooling.</remarks>
public enum AlpineArchitectures
{
    aarch64,
    armhf,
    armv7,
    cloud,
    ppc64le,
    s390x,
    x86,
    x86_64
}