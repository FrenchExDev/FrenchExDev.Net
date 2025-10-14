using System.Diagnostics;

namespace FrenchExDev.Net.Vagrant.Commands;

/// <summary>
/// Defines the contract for a Vagrant command, including methods and properties for configuring arguments, environment
/// variables, working directory, and output handlers, as well as for creating and executing the command process.
/// </summary>
/// <remarks>Implementations of this interface allow customization of Vagrant command execution, including setting
/// command-line arguments, environment variables, and working directory. Output and error streams can be handled
/// asynchronously via registered handlers. The interface provides methods to construct a configured process and to
/// execute the command, capturing its output and exit code. Thread safety is not guaranteed; if used concurrently,
/// external synchronization may be required.</remarks>
public interface IVagrantCommand
{
    string Executable => "vagrant";
    IReadOnlyList<string> ToArguments();
    string? WorkingDirectory { get; }
    IReadOnlyDictionary<string, string> EnvironmentVariables { get; }
    void AddOnStdOut(Func<string, Task> func);
    void AddOnStdErr(Func<string, Task> func);
    List<Func<string, Task>> GetOnStdOut();
    List<Func<string, Task>> GetOnStdErr();

    /// <summary>
    /// Creates a configured <see cref="ProcessStartInfo"/> instance for launching the specified executable with the
    /// current arguments and environment variables.
    /// </summary>
    /// <remarks>The returned <see cref="ProcessStartInfo"/> is set to create no window and to redirect all
    /// standard streams. The working directory defaults to the current directory if not explicitly specified.</remarks>
    /// <returns>A <see cref="ProcessStartInfo"/> object initialized with the executable path, arguments, working directory, and
    /// environment variables. Standard input, output, and error streams are redirected; shell execution is disabled.</returns>
    ProcessStartInfo ToProcessStartInfo()
    {
        var psi = new ProcessStartInfo(Executable)
        {
            WorkingDirectory = WorkingDirectory ?? Environment.CurrentDirectory,
            RedirectStandardError = true,
            RedirectStandardOutput = true,
            RedirectStandardInput = true,
            UseShellExecute = false,
            CreateNoWindow = true,
            FileName = Executable
        };

        foreach (var arg in ToArguments())
            psi.ArgumentList.Add(arg);

        foreach (var kv in EnvironmentVariables)
            psi.Environment[kv.Key] = kv.Value;

        return psi;
    }

    /// <summary>
    /// Creates and configures a new <see cref="System.Diagnostics.Process"/> instance with custom handlers for standard
    /// output and error data events.
    /// </summary>
    /// <remarks>The returned process uses the start information provided by <c>ToProcessStartInfo()</c> and
    /// attaches asynchronous handlers to the <c>OutputDataReceived</c> and <c>ErrorDataReceived</c> events. Each
    /// handler registered via <c>GetOnStdOut()</c> or <c>GetOnStdErr()</c> will be invoked for each line of output or
    /// error data received. Ensure that handlers are thread-safe if concurrent processing is expected.</remarks>
    /// <returns>A <see cref="System.Diagnostics.Process"/> object that is set up to invoke registered handlers when output or
    /// error data is received.</returns>
    Process ToProcess()
    {
        var process = new Process()
        {
            StartInfo = ToProcessStartInfo()
        };

        process.OutputDataReceived += async (s, e) =>
        {
            if (e.Data is not null)
            {
                foreach (var handler in GetOnStdOut())
                {
                    await handler(e.Data);
                }
            }
        };

        process.ErrorDataReceived += async (s, e) =>
        {
            if (e.Data is not null)
            {
                foreach (var handler in GetOnStdErr())
                {
                    await handler(e.Data);
                }
            }
        };

        return process;
    }

    /// <summary>
    /// Runs the configured process asynchronously and captures its standard output, standard error, and exit code.
    /// </summary>
    /// <remarks>The process output and error streams are collected until the process exits. If the
    /// cancellation token is triggered before the process completes, the operation is canceled and the process may be
    /// terminated.</remarks>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the asynchronous operation. The default value is <see
    /// cref="CancellationToken.None"/>.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains an <see cref="Execution"/> object
    /// with the process exit code, standard output, and standard error.</returns>
    async Task<Execution> ToExecutionAsync(CancellationToken cancellationToken = default)
    {
        var stdout = new StdOut();
        var stderr = new StdErr();

        AddOnStdOut(line =>
        {
            stdout.Add(line);
            return Task.CompletedTask;
        });
        AddOnStdErr(line =>
        {
            stderr.Add(line);
            return Task.CompletedTask;
        });

        using var process = ToProcess();

        process.Start();

        process.BeginOutputReadLine();
        process.BeginErrorReadLine();

        await process.WaitForExitAsync(cancellationToken);

        return new Execution(process.ExitCode, stdout, stderr);
    }
}

/// <summary>
/// Record of the result of executing a command, including the exit code, standard output, and standard error.
/// </summary>
/// <param name="ExitCode"></param>
/// <param name="StdOut"></param>
/// <param name="StdErr"></param>
public record Execution(int ExitCode, StdOut StdOut, StdErr StdErr);

/// <summary>
/// Represents a collection of standard output lines as a list of strings.
/// </summary>
/// <remarks>This class inherits from <see cref="List{string}"/>, providing all standard list operations for
/// managing lines of output. It can be used to capture or manipulate output data in scenarios where standard output is
/// represented as a sequence of strings.</remarks>
public class StdOut : List<string> { }

/// <summary>
/// Represents a collection of standard error output lines.
/// </summary>
/// <remarks>This class inherits from <see cref="List{string}"/>, providing all standard list operations for
/// managing lines of error output. It is typically used to capture and process error messages generated by external
/// processes or command-line tools.</remarks>
public class StdErr : List<string> { }
