using System.Diagnostics;

namespace FrenchExDev.Net.Vagrant.Commands.Invocation;

public static class VagrantExecutionExtensions
{
    /// <summary>
    /// Build a <see cref="ProcessStartInfo"/> for the given invocation.
    /// </summary>
    public static ProcessStartInfo ToProcessStartInfo(this Invocation invocation, string vagrantExecutable = "vagrant")
    {
        var args = VagrantInvocationBuilder.BuildArgs(invocation);
        var psi = new ProcessStartInfo
        {
            FileName = vagrantExecutable,
            Arguments = args,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false
        };
        return psi;
    }

    public static void PrepareListeners(this Process process, Func<string> stdOutListener, Func<string> stdErrListener)
    {
        if (stdOutListener is not null)
        {
            process.OutputDataReceived += (_, e) => { if (e.Data is not null) stdOutListener.Invoke(); };
        }
        if (stdErrListener is not null)
        {
            process.ErrorDataReceived += (_, e) => { if (e.Data is not null) stdErrListener.Invoke(); };
        }
    }

    /// <summary>
    /// Executes the invocation and waits for exit. Returns (exitCode, stdout, stderr).
    /// </summary>
    public static async Task<InvocationResult> ExecuteAsync(this Invocation invocation, string vagrantExecutable = "vagrant", CancellationToken cancellationToken = default)
    {
        var psi = invocation.ToProcessStartInfo(vagrantExecutable);
        using var process = new Process { StartInfo = psi, EnableRaisingEvents = true };
        var stdOut = new List<string>();
        var stdErr = new List<string>();
        process.OutputDataReceived += (_, e) => { if (e.Data is not null) stdOut.Add(e.Data); };
        process.ErrorDataReceived += (_, e) => { if (e.Data is not null) stdErr.Add(e.Data); };
        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();
        await process.WaitForExitAsync(cancellationToken).ConfigureAwait(false);
        return new InvocationResult(ExitCode: process.ExitCode, StdOut: string.Join('\n', stdOut), StdErr: string.Join('\n', stdErr));
    }
}

public record InvocationResult(int ExitCode, string StdOut, string StdErr);;