#region Licensing

// Copyright Stéphane Erard
// All rights reserved

#endregion

namespace FrenchExDev.Net.VirtualBox.Version;

/// <summary>
/// Defines a mechanism for asynchronously discovering the installed VirtualBox system version.
/// </summary>
public interface IVirtualBoxSystemVersionDiscoverer
{
    /// <summary>
    /// Asynchronously discovers the installed VirtualBox version on the local system.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the discovery operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a <see
    /// cref="VirtualBoxVersionRecord"/> describing the detected VirtualBox version, or <c>null</c> if VirtualBox is not
    /// installed.</returns>
    Task<VirtualBoxVersionRecord> DiscoverAsync(CancellationToken cancellationToken = default);
}