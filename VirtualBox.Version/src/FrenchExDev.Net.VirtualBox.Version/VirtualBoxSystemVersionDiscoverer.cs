#region Licensing

// Copyright Stéphane Erard
// All rights reserved

#endregion

using System.Diagnostics;

namespace FrenchExDev.Net.VirtualBox.Version;

/// <summary>
/// Provides functionality to discover the installed VirtualBox system version by invoking the VBoxManage command-line
/// tool.
/// </summary>
/// <remarks>This class uses the VBoxManage utility to retrieve version information from the local VirtualBox
/// installation. Ensure that VBoxManage is available in the system's PATH before using this class. Typically, this
/// class is used to obtain version details for compatibility checks or diagnostics.</remarks>
public class VirtualBoxSystemVersionDiscoverer : IVirtualBoxSystemVersionDiscoverer
{
    /// <summary>
    /// Retrieves the current version information of VirtualBox by executing the VBoxManage command asynchronously.
    /// </summary>
    /// <remarks>The method requires VBoxManage to be installed and accessible via the system PATH. The
    /// operation may take several seconds to complete, depending on system performance.</remarks>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A VirtualBoxVersionRecord containing the major, minor, patch, and build revision of the installed VirtualBox
    /// version.</returns>
    /// <exception cref="Exception">Thrown if the VBoxManage process cannot be started. This may indicate that VBoxManage is not available in the
    /// system PATH.</exception>
    public async Task<VirtualBoxVersionRecord> DiscoverAsync(CancellationToken cancellationToken = default)
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "VBoxManage",
                Arguments = "--version",
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                RedirectStandardInput = true,
                CreateNoWindow = true,
                UseShellExecute = false,
                WorkingDirectory = Environment.CurrentDirectory
            }
        };

        var started = process.Start();

        if (!started) throw new Exception("error starting VBoxManage. Check it is in PATH");

        await process.WaitForExitAsync(cancellationToken);

        var stdOut = await process.StandardOutput.ReadToEndAsync(cancellationToken);

        var split = stdOut.Split(".");

        var patch = split[2];
        var patchSplit = patch.Split("r");

        return new VirtualBoxVersionRecord(split[0], split[1], patchSplit[0], patchSplit[1].Replace("\r\n", ""));
    }
}