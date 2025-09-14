#region Licensing

// Copyright Stéphane Erard
// For licensing, please contact stephane.erard@gmail

#endregion

namespace FrenchExDev.Net.Ssh.Config;

/// <summary>
/// Provides functionality to read and parse SSH configuration files from a collection of text lines into an <see
/// cref="SshConfigFile"/> object.
/// </summary>
/// <remarks>This class is typically used to process the contents of an SSH config file, such as those found in
/// OpenSSH environments, by converting the file's lines into a structured representation. It supports reading multiple
/// host entries and their associated configuration options. Instances of this class are not thread-safe.</remarks>
public class SshConfigFileReader : IReader<SshConfigFile>
{
    /// <summary>
    /// Parses a collection of SSH configuration file lines and constructs an object model representing the
    /// configuration.
    /// </summary>
    /// <remarks>Each host entry in the SSH configuration is identified and parsed based on standard SSH
    /// config file conventions. Lines that do not match a host declaration are grouped with the preceding host. The
    /// method does not validate the semantic correctness of individual configuration directives.</remarks>
    /// <param name="lines">An enumerable collection of strings, each representing a line from an SSH configuration file. Lines should be
    /// provided in the order they appear in the file.</param>
    /// <returns>An <see cref="SshConfigFile"/> object containing the parsed hosts and their configuration from the provided
    /// lines.</returns>
    public SshConfigFile Read(IEnumerable<string> lines)
    {
        var sshConfigFile = new SshConfigFile()
        {
            Hosts = new List<SshConfigHost>()
        };

        var currentHostLines = new List<string>();

        foreach (var line in lines)
        {
            var trimmedLine = line.TrimStart();
            if (!SshConfigHostReader.Name.Match(trimmedLine).Success)
            {
                currentHostLines.Add(trimmedLine);
                continue;
            }

            // new host !
            // if any line in current host, read it and append to hosts array
            if (currentHostLines.Count > 0)
            {
                var currentHost = new SshConfigHostReader().Read(currentHostLines);
                sshConfigFile.Hosts.Add(currentHost);
                currentHostLines.Clear();
            }

            currentHostLines.Add(line);
        }

        if (currentHostLines.Count > 0)
        {
            var currentHost = new SshConfigHostReader().Read(currentHostLines);
            sshConfigFile.Hosts.Add(currentHost);
            currentHostLines.Clear();
        }

        return sshConfigFile;
    }
}