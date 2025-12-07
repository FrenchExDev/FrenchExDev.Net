using FrenchExDev.Net.CSharp.Object.Builder;
using System.Diagnostics;
using System.Text;

namespace FrenchExDev.Net.Packer;

/// <summary>
/// Common contract for a Packer CLI command (e.g. build, plugins install).
/// Implementations provide an executable name (packer) and ordered argument list.
/// </summary>
public interface IPackerCommand
{
    /// <summary>Returns the executable file name (defaults to "packer").</summary>
    string Executable => "packer";
    /// <summary>Returns the ordered list of command line arguments.</summary>
    IReadOnlyList<string> ToArguments();
    /// <summary>Creates a configured <see cref="ProcessStartInfo"/> ready to start.</summary>
    ProcessStartInfo ToProcessStartInfo(string? workingDirectory = null)
    {
        var psi = new ProcessStartInfo(Executable)
        {
            WorkingDirectory = workingDirectory ?? WorkingDirectory ?? Environment.CurrentDirectory,
            RedirectStandardError = true,
            RedirectStandardOutput = true,
            RedirectStandardInput = true,
            UseShellExecute = true,
            ArgumentList = { }
        };
        foreach (var arg in ToArguments()) psi.ArgumentList.Add(arg);
        foreach (var kv in EnvironmentVariables)
        {
            psi.Environment[kv.Key] = kv.Value;
        }
        return psi;
    }
    /// <summary>Optional working directory hint (used when building a process start info).</summary>
    string? WorkingDirectory { get; }
    /// <summary>Optional environment variables to inject when launching the CLI.</summary>
    IReadOnlyDictionary<string, string> EnvironmentVariables { get; }
}

/// <summary>
/// Provides a builder for configuring and creating instances of <see cref="PackerBuildCommand"/> using a fluent
/// interface.
/// </summary>
/// <remarks>Use this class to incrementally specify options for a Packer build command before constructing the
/// final <see cref="PackerBuildCommand"/> instance. Each configuration method returns the builder itself, allowing
/// method chaining. The builder enforces that all required options are set before building the command; attempting to
/// build without specifying a required option will result in an exception.</remarks>
public sealed class PackerBuildCommandBuilder : AbstractBuilder<PackerBuildCommand>
{
    private bool? _debug;
    private bool? _disableColor;
    private bool? _force;
    private bool? _machineReadable;
    private OnErrorStrategy? _onError;
    private int? _parallelBuilds;
    private string? _templatePath;
    private bool? _timestampUi;
    private string? _workingDirectory;

    /// <summary>
    /// Enables or disables debug mode for the build command.
    /// </summary>
    /// <param name="debug">A value indicating whether debug mode should be enabled. If <see langword="true"/>, debug mode is enabled; if
    /// <see langword="false"/>, debug mode is disabled. If <c>null</c>, the default behavior is used.</param>
    /// <returns>The current <see cref="PackerBuildCommandBuilder"/> instance with the debug mode setting applied.</returns>
    public PackerBuildCommandBuilder WithDebug(bool? debug = true)
    {
        _debug = debug;
        return this;
    }

    /// <summary>
    /// Configures the command to disable colored output in the Packer build process.
    /// </summary>
    /// <param name="disable">Specifies whether to disable colored output. If <see langword="true"/>, colored output is disabled; if <see
    /// langword="false"/>, colored output is enabled. If <c>null</c>, the default behavior is used.</param>
    /// <returns>The current <see cref="PackerBuildCommandBuilder"/> instance with the updated color output setting.</returns>
    public PackerBuildCommandBuilder WithDisableColor(bool? disable = true)
    {
        _disableColor = disable;
        return this;
    }

    /// <summary>
    /// Specifies whether the build command should forcibly overwrite existing output files.
    /// </summary>
    /// <param name="force">A value indicating whether to force overwriting of existing output files. If <see langword="true"/>, the build
    /// will proceed even if output files already exist; if <see langword="false"/>, the build will not overwrite
    /// existing files. If <see langword="null"/>, the default behavior is used.</param>
    /// <returns>The current <see cref="PackerBuildCommandBuilder"/> instance with the force option configured.</returns>
    public PackerBuildCommandBuilder WithForce(bool? force = true)
    {
        _force = force;
        return this;
    }

    /// <summary>
    /// Configures the builder to produce machine-readable output when executing the Packer build command.
    /// </summary>
    /// <param name="readable">Specifies whether the output should be formatted for machine parsing. If <see langword="true"/>, the output will
    /// be machine-readable; if <see langword="false"/>, standard human-readable output is used. If <see
    /// langword="null"/>, the default behavior is applied.</param>
    /// <returns>The current <see cref="PackerBuildCommandBuilder"/> instance with the updated machine-readable setting.</returns>
    public PackerBuildCommandBuilder WithMachineReadable(bool? readable = true)
    {
        _machineReadable = readable;
        return this;
    }

    /// <summary>
    /// Sets the error handling strategy to use during the build process.
    /// </summary>
    /// <param name="onError">The error handling strategy to apply if an error occurs during the build. Specifies how the build should respond
    /// to errors.</param>
    /// <returns>The current <see cref="PackerBuildCommandBuilder"/> instance with the updated error handling strategy.</returns>
    public PackerBuildCommandBuilder WithOnError(OnErrorStrategy onError)
    {
        _onError = onError;
        return this;
    }

    /// <summary>
    /// Configures the number of parallel build instances to use when executing the build command.
    /// </summary>
    /// <remarks>Setting a higher number of parallel builds may improve build throughput but can increase
    /// resource usage. The default value is determined by the underlying build system if not specified.</remarks>
    /// <param name="instances">The number of parallel build instances to run. Specify 0 or null to use the default behavior, or a positive
    /// integer to set an explicit limit.</param>
    /// <returns>The current <see cref="PackerBuildCommandBuilder"/> instance with the parallel build setting applied.</returns>
    public PackerBuildCommandBuilder WithParallelBuilds(int? instances = 0)
    {
        _parallelBuilds = instances;
        return this;
    }

    /// <summary>
    /// Sets the path to the Packer template file to be used for the build operation.
    /// </summary>
    /// <param name="templatePath">The file system path to the Packer template. This value should specify a valid template file required for the
    /// build process.</param>
    /// <returns>The current <see cref="PackerBuildCommandBuilder"/> instance with the updated template path, enabling method
    /// chaining.</returns>
    public PackerBuildCommandBuilder WithTemplatePath(string templatePath)
    {
        _templatePath = templatePath;
        return this;
    }

    /// <summary>
    /// Configures whether the build command should include a timestamp in the UI output.
    /// </summary>
    /// <param name="timestampUi">A value indicating whether to enable timestamp display in the UI output. If <see langword="true"/>, timestamps
    /// are included; if <see langword="false"/>, timestamps are omitted. If <see langword="null"/>, the default
    /// behavior is used.</param>
    /// <returns>The current <see cref="PackerBuildCommandBuilder"/> instance with the timestamp UI setting applied.</returns>
    public PackerBuildCommandBuilder WithTimestampUi(bool? timestampUi = true)
    {
        _timestampUi = timestampUi;
        return this;
    }

    /// <summary>
    /// Sets the working directory to be used for the build command.
    /// </summary>
    /// <remarks>This method enables fluent configuration of the build command by allowing chaining of
    /// multiple setup methods.</remarks>
    /// <param name="wd">The path to the working directory. Can be absolute or relative. If null or empty, the default working directory
    /// will be used.</param>
    /// <returns>The current instance of <see cref="PackerBuildCommandBuilder"/> with the updated working directory.</returns>
    public PackerBuildCommandBuilder WithWorkingDirectory(string wd)
    {
        _workingDirectory = wd;
        return this;
    }

    /// <summary>
    /// Creates a new instance of the <see cref="PackerBuildCommand"/> class using the configured builder properties.
    /// </summary>
    /// <remarks>All builder properties must be assigned before invoking this method. If any property is
    /// missing, a <see cref="MissingMemberException"/> is thrown to indicate incomplete configuration.</remarks>
    /// <returns>A <see cref="PackerBuildCommand"/> initialized with the current builder settings.</returns>
    /// <exception cref="MissingMemberException">Thrown if any required builder property has not been set prior to calling this method.</exception>
    protected override PackerBuildCommand Instantiate()
    {
        return new PackerBuildCommand()
        {
            Debug = _debug ?? throw new MissingMemberException(nameof(_debug)),
            DisableColor = _disableColor ?? throw new MissingMemberException(nameof(_disableColor)),
            Force = _force ?? throw new MissingMemberException(nameof(_force)),
            MachineReadable = _machineReadable ?? throw new MissingMemberException(nameof(_machineReadable)),
            OnError = _onError ?? throw new MissingMemberException(nameof(_onError)),
            ParallelBuilds = _parallelBuilds ?? throw new MissingMemberException(nameof(_parallelBuilds)),
            TemplatePath = _templatePath ?? throw new MissingMemberException(nameof(_templatePath)),
            TimestampUi = _timestampUi ?? throw new MissingMemberException(nameof(_timestampUi)),
            WorkingDirectory = _workingDirectory ?? throw new MissingMemberException(nameof(_workingDirectory))
        };
    }
}

/// <summary>
/// Packer build command high-level options; generates arguments for: packer build [options] template.pkr.hcl
/// Supports json/hcl templates (auto-detected by extension).
/// </summary>
public sealed class PackerBuildCommand : IPackerCommand
{
    private readonly List<string> _only = new();
    private readonly List<string> _except = new();
    private readonly Dictionary<string, string> _vars = new(StringComparer.OrdinalIgnoreCase);
    private readonly List<string> _varFiles = new();
    private readonly Dictionary<string, string> _env = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>Path to the template file (json / pkr.hcl).</summary>
    public required string TemplatePath { get; init; }
    /// <summary>Optional custom working directory (defaults to current directory).</summary>
    public string? WorkingDirectory { get; init; }
    /// <summary>Disable ANSI color output.</summary>
    public bool DisableColor { get; init; }
    /// <summary>Enable timestamp prefix on UI output.</summary>
    public bool TimestampUi { get; init; }
    /// <summary>Force overwrite of existing build artifacts.</summary>
    public bool Force { get; init; }
    /// <summary>Enable debug output (PACKER_LOG=1 equivalent).</summary>
    public bool Debug { get; init; }
    /// <summary>Optional machine readable output (packer flag --machine-readable).</summary>
    public bool MachineReadable { get; init; }
    /// <summary>Number of parallel builds (null means default concurrency).</summary>
    public int? ParallelBuilds { get; init; }
    /// <summary>On-error behavior: cleanup, abort, or ask.</summary>
    public OnErrorStrategy OnError { get; init; } = OnErrorStrategy.Cleanup;

    /// <summary>Adds a -var 'key=value' pair.</summary>
    public PackerBuildCommand Var(string key, string value) { _vars[key] = value; return this; }
    /// <summary>Adds a -var-file=path.pkrvars.hcl argument.</summary>
    public PackerBuildCommand VarFile(string path) { _varFiles.Add(path); return this; }
    /// <summary>Restrict build to specific builder names (repeatable).</summary>
    public PackerBuildCommand Only(string builder) { if (!_only.Contains(builder)) _only.Add(builder); return this; }
    /// <summary>Exclude specific builder names.</summary>
    public PackerBuildCommand Except(string builder) { if (!_except.Contains(builder)) _except.Add(builder); return this; }
    /// <summary>Sets / overrides an environment variable passed to the process.</summary>
    public PackerBuildCommand Env(string key, string value) { _env[key] = value; return this; }

    IReadOnlyDictionary<string, string> IPackerCommand.EnvironmentVariables => _env;

    /// <summary>Returns ordered CLI arguments for the build command.</summary>
    public IReadOnlyList<string> ToArguments()
    {
        var args = new List<string> { "build" };
        if (DisableColor) args.Add("-color=false");
        if (TimestampUi) args.Add("-timestamp-ui");
        if (Force) args.Add("-force");
        if (Debug) args.Add("-debug");
        if (MachineReadable) args.Add("-machine-readable");
        if (ParallelBuilds is > 0) { args.Add("-parallel-builds"); args.Add(ParallelBuilds.Value.ToString()); }
        if (_only.Count > 0) { args.Add("-only=" + string.Join(",", _only)); }
        if (_except.Count > 0) { args.Add("-except=" + string.Join(",", _except)); }
        switch (OnError)
        {
            case OnErrorStrategy.Cleanup: /* default */ break;
            case OnErrorStrategy.Abort: args.Add("-on-error=abort"); break;
            case OnErrorStrategy.Ask: args.Add("-on-error=ask"); break;
        }
        foreach (var kv in _vars)
        {
            args.Add("-var");
            args.Add($"{kv.Key}={kv.Value}");
        }
        foreach (var vf in _varFiles)
        {
            args.Add("-var-file=" + vf);
        }
        args.Add(TemplatePath);
        return args;
    }
}

/// <summary>
/// Provides a builder for configuring and creating instances of <see cref="PackerInstallPluginCommand"/> to install
/// Packer plugins with customizable options.
/// </summary>
/// <remarks>Use this builder to fluently specify installation parameters such as destination directory, plugin
/// identifier, version, source URI, working directory, and whether to force installation. All required properties must
/// be set before calling <see cref="Build"/>; otherwise, an exception will be thrown. This class is not
/// thread-safe.</remarks>
public sealed class PackerInstallPluginCommandBuilder : AbstractBuilder<PackerInstallPluginCommand>
{
    private string? _destinationDir;
    private string? _pluginIdentifier;
    private bool? _force;
    private string? _sourceUri;
    private string? _version;
    private string? _workingDirectory;

    /// <summary>
    /// Sets the destination directory for the plugin installation command.
    /// </summary>
    /// <param name="d">The path to the directory where the plugin will be installed. This value cannot be null or empty.</param>
    /// <returns>The current instance of <see cref="PackerInstallPluginCommandBuilder"/> with the updated destination directory.</returns>
    public PackerInstallPluginCommandBuilder WithDestinationDir(string d)
    {
        _destinationDir = d;
        return this;
    }

    /// <summary>
    /// Sets the plugin identifier to use for the installation command builder.
    /// </summary>
    /// <param name="n">The plugin identifier to associate with the command builder. Cannot be null.</param>
    /// <returns>The current instance of <see cref="PackerInstallPluginCommandBuilder"/> with the specified plugin identifier
    /// set.</returns>
    public PackerInstallPluginCommandBuilder WithPluginIdentifier(string n)
    {
        _pluginIdentifier = n;
        return this;
    }

    /// <summary>
    /// Specifies whether the installation should proceed with force, overriding any existing checks or prompts.
    /// </summary>
    /// <param name="force">A value indicating whether to force the installation. If <see langword="true"/>, the installation will proceed
    /// even if it would normally be blocked; if <see langword="false"/>, standard checks are enforced. If <see
    /// langword="null"/>, the default behavior is used.</param>
    /// <returns>The current <see cref="PackerInstallPluginCommandBuilder"/> instance with the force option configured.</returns>
    public PackerInstallPluginCommandBuilder WithForce(bool? force = true)
    {
        _force = force;
        return this;
    }

    /// <summary>
    /// Sets the source URI to be used for the plugin installation command.
    /// </summary>
    /// <param name="source">The URI of the plugin source. This value can be a local file path or a remote URL. Cannot be null.</param>
    /// <returns>The current <see cref="PackerInstallPluginCommandBuilder"/> instance with the updated source URI.</returns>
    public PackerInstallPluginCommandBuilder WithSourceUri(string source)
    {
        _sourceUri = source;
        return this;
    }

    /// <summary>
    /// Sets the version of the plugin to be installed and returns the current builder instance for method chaining.
    /// </summary>
    /// <param name="v">The version string to use for the plugin installation. Cannot be null.</param>
    /// <returns>The current <see cref="PackerInstallPluginCommandBuilder"/> instance with the specified version set.</returns>
    public PackerInstallPluginCommandBuilder WithVersion(string v)
    {
        _version = v;
        return this;
    }

    /// <summary>
    /// Sets the working directory to use when executing the plugin installation command.
    /// </summary>
    /// <param name="d">The path of the working directory to use. Can be absolute or relative. If null or empty, the default working
    /// directory will be used.</param>
    /// <returns>The current instance of <see cref="PackerInstallPluginCommandBuilder"/> with the updated working directory.</returns>
    public PackerInstallPluginCommandBuilder WithWorkingDirectory(string d)
    {
        _workingDirectory = d;
        return this;
    }

    /// <summary>
    /// Creates a new instance of the <see cref="PackerInstallPluginCommand"/> class using the configured parameters.
    /// </summary>
    /// <remarks>All required parameters must be configured before invoking this method. This method is
    /// typically used to construct a command object for installing a Packer plugin with the specified
    /// settings.</remarks>
    /// <returns>A <see cref="PackerInstallPluginCommand"/> initialized with the specified destination directory, plugin
    /// identifier, force flag, source URI, version, and working directory.</returns>
    /// <exception cref="MissingMemberException">Thrown if any required parameter has not been set prior to calling this method.</exception>
    protected override PackerInstallPluginCommand Instantiate()
    {
        return new PackerInstallPluginCommand()
        {
            DestinationDir = _destinationDir ?? throw new MissingMemberException(nameof(_destinationDir)),
            PluginIdentifier = _pluginIdentifier ?? throw new MissingMemberException(nameof(_pluginIdentifier)),
            Force = _force ?? throw new MissingMemberException(nameof(_force)),
            SourceUri = _sourceUri ?? throw new MissingMemberException(nameof(_sourceUri)),
            Version = _version ?? throw new MissingMemberException(nameof(_version)),
            WorkingDirectory = _workingDirectory ?? throw new MissingMemberException(nameof(_workingDirectory))
        };
    }
}

/// <summary>
/// Represents a packer plugin installation command (packer plugins install TYPE:NAME VERSION).
/// For registry sources, use the short form (e.g. hashicorp/amazon). For custom url / path sources specify SourceUri.
/// </summary>
public sealed class PackerInstallPluginCommand : IPackerCommand
{
    private readonly Dictionary<string, string> _env = new(StringComparer.OrdinalIgnoreCase);
    /// <summary>Fully qualified plugin target (e.g. github.com/hashicorp/amazon, hashicorp/amazon).</summary>
    public required string PluginIdentifier { get; init; }
    /// <summary>Specific version or version constraint (e.g. 1.0.0 or ~> 1.1).</summary>
    public required string Version { get; init; }
    /// <summary>Optional destination directory override.</summary>
    public string? DestinationDir { get; init; }
    /// <summary>
    /// Gets the URI of the source from which the content was obtained.
    /// </summary>
    public string? SourceUri { get; init; }
    /// <summary>
    /// Gets a value indicating whether the operation should be performed forcefully, overriding standard checks or
    /// constraints.
    /// </summary>
    public bool Force { get; init; }
    /// <summary>
    /// Gets the path of the working directory to use for the operation.
    /// </summary>
    /// <remarks>If not set, the default working directory will be used. The path should be a valid directory
    /// accessible by the process.</remarks>
    public string? WorkingDirectory { get; init; }

    /// <summary>
    /// Adds or updates an environment variable for the plugin installation process.
    /// </summary>
    /// <remarks>Environment variables set using this method will be available to the plugin during
    /// installation. If the specified key already exists, its value will be overwritten.</remarks>
    /// <param name="key">The name of the environment variable to set. Cannot be null or empty.</param>
    /// <param name="value">The value to assign to the environment variable. Cannot be null.</param>
    /// <returns>The current <see cref="PackerInstallPluginCommand"/> instance to allow method chaining.</returns>
    public PackerInstallPluginCommand Env(string key, string value) { _env[key] = value; return this; }

    /// <summary>
    /// Gets the collection of environment variables to be used by the packer command.
    /// </summary>
    /// <remarks>The returned dictionary contains key-value pairs representing environment variable names and
    /// their corresponding values. The collection is read-only and reflects the environment variables that will be set
    /// when executing the command.</remarks>
    IReadOnlyDictionary<string, string> IPackerCommand.EnvironmentVariables => _env;

    /// <summary>
    /// Builds a read-only list of command-line arguments for installing a plugin, based on the current property values.
    /// </summary>
    /// <remarks>The returned argument list reflects the values of properties such as Force, DestinationDir,
    /// SourceUri, PluginIdentifier, and Version. Optional arguments are included only if their corresponding properties
    /// are set. The order of arguments matches the expected format for the installation command.</remarks>
    /// <returns>A read-only list of strings containing the arguments to be passed to the plugin installation command. The list
    /// includes required and optional arguments depending on the configured properties.</returns>
    public IReadOnlyList<string> ToArguments()
    {
        var args = new List<string> { "plugins", "install" };
        if (Force) args.Add("-force");
        if (!string.IsNullOrWhiteSpace(DestinationDir)) { args.Add("-path"); args.Add(DestinationDir!); }
        if (!string.IsNullOrWhiteSpace(SourceUri)) { args.Add("-source"); args.Add(SourceUri!); }
        args.Add(PluginIdentifier);
        args.Add(Version);
        return args;
    }
}

/// <summary>On-error strategy for packer build.</summary>
public enum OnErrorStrategy
{
    /// <summary>Cleanup temporary artifacts (packer default).</summary>
    Cleanup,
    /// <summary>Abort immediately, leaving partial state.</summary>
    Abort,
    /// <summary>Prompt interactively (ask user); requires interactive terminal.</summary>
    Ask
}

/// <summary>
/// Utility helpers for <see cref="IPackerCommand"/>.
/// </summary>
public static class PackerCommandExtensions
{
    /// <summary>Renders a shell-safe single string (quoted where needed).</summary>
    public static string ToCommandLine(this IPackerCommand command)
    {
        var sb = new StringBuilder(command.Executable);
        foreach (var a in command.ToArguments())
        {
            sb.Append(' ');
            sb.Append(NeedsQuoting(a) ? Quote(a) : a);
        }
        return sb.ToString();
    }

    /// <summary>
    /// Determines whether the specified text requires quoting based on its content.
    /// </summary>
    /// <param name="text">The text to evaluate for quoting requirements. If the text is null, empty, contains whitespace, or includes a
    /// double quote character, quoting is considered necessary.</param>
    /// <returns>true if the text is null, empty, contains whitespace, or includes a double quote character; otherwise, false.</returns>
    private static bool NeedsQuoting(string text) => string.IsNullOrEmpty(text) || text.Any(char.IsWhiteSpace) || text.Contains('"');

    /// <summary>
    /// Encloses the specified string in double quotes and escapes any internal double quote characters.
    /// </summary>
    /// <remarks>This method produces output compatible with Windows and POSIX command-line parsing, where
    /// arguments containing spaces or special characters are typically quoted. The returned string can be safely used
    /// as a command-line argument.</remarks>
    /// <param name="value">The string to be quoted. If the string contains double quote characters, they will be escaped.</param>
    /// <returns>A string that is wrapped in double quotes, with internal double quotes escaped using a backslash.</returns>
    private static string Quote(string value)
    {
        // Basic Windows / POSIX compatible quoting (double quotes + escape internal quotes)
        return "\"" + value.Replace("\"", "\\\"") + "\"";
    }

    /// <summary>
    /// Starts a new process using the specified packer command and attaches optional handlers for standard output and
    /// error data.
    /// </summary>
    /// <remarks>The returned process is already started and begins reading output and error streams
    /// asynchronously. The caller is responsible for managing the process lifecycle, including waiting for completion
    /// and disposing of the process when finished.</remarks>
    /// <param name="packerCommand">The packer command to execute. Must not be null.</param>
    /// <param name="onStdOut">An optional callback invoked for each line of standard output received from the process. If null, standard
    /// output is not handled.</param>
    /// <param name="onStdErr">An optional callback invoked for each line of standard error received from the process. If null, standard error
    /// is not handled.</param>
    /// <returns>A <see cref="Process"/> instance representing the started process. The process will have asynchronous handlers
    /// for output and error streams if callbacks are provided.</returns>
    public static Process ToProcess(this IPackerCommand packerCommand, Action<string?>? onStdOut = null, Action<string?>? onStdErr = null)
    {
        var process = new Process();

        process.StartInfo = packerCommand.ToProcessStartInfo();

        if (onStdErr is not null)
        {
            process.ErrorDataReceived += (sender, e) =>
            {
                if (e.Data is not null)
                {
                    onStdErr(e.Data);
                }
            };
        }

        if (onStdOut is not null)
        {
            process.OutputDataReceived += (sender, e) =>
            {
                if (e.Data is not null)
                {
                    onStdOut(e.Data);
                }
            };
        }

        process.Start();
        process.BeginErrorReadLine();
        process.BeginOutputReadLine();

        return process;
    }

    /// <summary>
    /// Asynchronously waits for the associated process to exit.
    /// </summary>
    /// <param name="process">The process to wait for. Must not be null and must have been started.</param>
    /// <returns>A task that represents the asynchronous wait operation. The task result is <see langword="true"/> if the process
    /// has exited; otherwise, <see langword="false"/>.</returns>
    public static async Task<bool> WaitAsync(this Process process)
    {
        return await process.WaitAsync();
    }
}
