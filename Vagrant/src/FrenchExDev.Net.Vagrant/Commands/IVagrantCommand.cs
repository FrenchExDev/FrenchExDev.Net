using System.Diagnostics;

namespace FrenchExDev.Net.Vagrant.Commands;

public interface IVagrantCommand
{
    string Executable => "vagrant";
    IReadOnlyList<string> ToArguments();
    string? WorkingDirectory { get; }
    IReadOnlyDictionary<string, string> EnvironmentVariables { get; }

    ProcessStartInfo ToProcessStartInfo()
    {
        var psi = new ProcessStartInfo(Executable)
        {
            WorkingDirectory = WorkingDirectory ?? Environment.CurrentDirectory,
            RedirectStandardError = true,
            RedirectStandardOutput = true,
            RedirectStandardInput = true,
            UseShellExecute = false
        };
        foreach (var arg in ToArguments()) psi.ArgumentList.Add(arg);
        foreach (var kv in EnvironmentVariables)
        {
            psi.Environment[kv.Key] = kv.Value;
        }
        return psi;
    }
}
