#region Licensing

// Copyright Stéphane Erard
// For licensing please contact <stephane.erard@gmail.com>

#endregion

namespace FrenchExDev.Net.Alpine.Version;

/// <summary>
/// Specifies the available distribution flavors of Alpine Linux supported by the application.
/// </summary>
/// <remarks>Each flavor represents a different variant of Alpine Linux, tailored for specific use cases such as
/// minimal installations, virtualization, or network boot scenarios. Use this enumeration to select the desired Alpine
/// Linux flavor when configuring or deploying Alpine-based environments.</remarks>
public enum AlpineFlavors
{
    Extended,
    MiniRootFs,
    NetBoot,
    Standard,
    Virt,
    Xen
}