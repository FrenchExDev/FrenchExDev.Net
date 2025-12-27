#region Licensing

// Copyright Stéphane Erard
// For licensing, please contact stephane.erard@gmail

#endregion

namespace FrenchExDev.Net.Ssh.Config;

/// <summary>
/// Provides functionality to serialize an <see cref="SshConfigFile"/> into its textual SSH configuration file
/// representation.
/// </summary>
/// <remarks>Use <see cref="SshConfigFileWriter"/> to generate the lines of an SSH config file from an <see
/// cref="SshConfigFile"/> instance. This class is typically used when saving or exporting SSH configuration data to
/// disk or for display. The output format follows standard SSH config file conventions.</remarks>
public class SshConfigFileWriter : IWriter<SshConfigFile>
{
    /// <summary>
    /// Generates a list of SSH configuration file lines representing all hosts defined in the specified configuration.
    /// </summary>
    /// <param name="subject">The SSH configuration file containing the collection of hosts to be written. Cannot be null.</param>
    /// <returns>A list of strings, each representing a line in the SSH configuration file for the provided hosts. The list will
    /// be empty if no hosts are defined.</returns>
    public List<string> Write(SshConfigFile subject)
    {
        var lines = new List<string>();

        foreach (var host in subject.Hosts) lines.AddRange(new SshConfigHostWriter().Write(host));

        return lines;
    }
}