#region Licensing

// Copyright Stéphane Erard
// For licensing, please contact stephane.erard@gmail

#endregion

#region Usings

using FrenchExDev.Net.CSharp.Object.Builder2;

#endregion

namespace FrenchExDev.Net.Packer.Bundle;

/// <summary>
/// Provides a fluent builder for configuring and constructing instances of <see cref="ShellScript"/>.
/// </summary>
/// <remarks>
/// Use this builder to add lines, set shell options, and configure the name and newline character for a shell script.
/// Each configuration method returns the builder instance, allowing for method chaining. The builder validates that all mandatory properties are set before constructing the final <see cref="ShellScript"/> object.
/// This class is not thread-safe.
/// <example>
/// <code>
/// var builder = new ShellScriptBuilder()
///     .Name("setup.sh")
///     .Set("-e")
///     .AddLine("echo Hello World")
///     .SetNewLine("\n");
/// var script = builder.Build();
/// </code>
/// </example>
/// </remarks>
public class ShellScriptBuilder : AbstractBuilder<ShellScript>
{
    /// <summary>
    /// List of lines to include in the shell script.
    /// </summary>
    /// <remarks>Each string represents a line in the script. Example: "echo Hello World".</remarks>
    private readonly List<string> _lines = new();
    /// <summary>
    /// Newline character to use in the script (default: "\n").
    /// </summary>
    /// <remarks>Set to "\r\n" for Windows scripts.</remarks>
    private string _newLine = "\n";
    /// <summary>
    /// Shell options to set at the beginning of the script (e.g., "-e", "-x").
    /// </summary>
    /// <remarks>Example: Set("-e") will add "set -e" as the first line.</remarks>
    private string? _set;
    /// <summary>
    /// Name of the shell script file.
    /// </summary>
    /// <remarks>Example: "setup.sh".</remarks>
    private string? _name;

    /// <summary>
    /// Sets the name of the shell script file.
    /// </summary>
    /// <param name="name">Script file name (e.g., "setup.sh").</param>
    /// <returns>The builder instance.</returns>
    public ShellScriptBuilder Name(string name)
    {
        _name = name;
        return this;
    }

    /// <summary>
    /// Sets shell options to be added as the first line (e.g., "-e", "-x").
    /// </summary>
    /// <param name="value">Shell options string.</param>
    /// <returns>The builder instance.</returns>
    /// <remarks>Example: Set("-e") will add "set -e" as the first line.</remarks>
    public ShellScriptBuilder Set(string value)
    {
        _set = value;
        return this;
    }

    /// <summary>
    /// Adds multiple lines to the shell script from a list of strings.
    /// </summary>
    /// <param name="value">List of lines to add.</param>
    /// <returns>The builder instance.</returns>
    /// <remarks>Example: AddLines(new List&lt;string&gt; { "echo Hello", "exit 0" })</remarks>
    public ShellScriptBuilder AddLines(List<string> value)
    {
        _lines.AddRange(value);
        return this;
    }

    /// <summary>
    /// Adds multiple lines to the shell script by splitting a string using the specified end-of-line delimiter.
    /// </summary>
    /// <param name="lines">String containing multiple lines.</param>
    /// <param name="eol">End-of-line delimiter (e.g., "\n").</param>
    /// <returns>The builder instance.</returns>
    /// <remarks>Example: AddLines("echo Hello\necho World", "\n")</remarks>
    public ShellScriptBuilder AddLines(string lines, string eol)
    {
        _lines.AddRange(lines.Split(eol));
        return this;
    }

    /// <summary>
    /// Adds multiple lines to the shell script from an array of strings.
    /// </summary>
    /// <param name="value">Array of lines to add.</param>
    /// <returns>The builder instance.</returns>
    /// <remarks>Example: AddLines(new[] { "echo Hello", "exit 0" })</remarks>
    public ShellScriptBuilder AddLines(string[] value)
    {
        _lines.AddRange(value);
        return this;
    }

    /// <summary>
    /// Adds a single line to the shell script.
    /// </summary>
    /// <param name="value">Line to add.</param>
    /// <returns>The builder instance.</returns>
    /// <remarks>Example: AddLine("echo Hello World")</remarks>
    public ShellScriptBuilder AddLine(string value)
    {
        _lines.Add(value);
        return this;
    }

    /// <summary>
    /// Sets the newline character for the shell script.
    /// </summary>
    /// <param name="newLine">Newline character (e.g., "\n" or "\r\n").</param>
    /// <returns>The builder instance.</returns>
    /// <remarks>Set to "\r\n" for Windows scripts.</remarks>
    public ShellScriptBuilder SetNewLine(string newLine)
    {
        _newLine = newLine;
        return this;
    }

    /// <summary>
    /// Creates and initializes a new instance of the <see cref="ShellScript"/> class using the configured name, lines,
    /// and newline settings.
    /// </summary>
    /// <returns>A <see cref="ShellScript"/> object populated with the specified name, script lines, and newline character.</returns>
    /// <exception cref="InvalidDataException">Thrown if the configured name is null when instantiating the <see cref="ShellScript"/>.</exception>
    protected override ShellScript Instantiate()
    {
        var shellScript = new ShellScript() { Name = _name ?? throw new InvalidDataException(nameof(_name)) };

        if (!string.IsNullOrEmpty(_set)) shellScript.Lines.Add("set " + _set);

        shellScript.Lines.AddRange(_lines);
        shellScript.NewLine = _newLine;

        return shellScript;
    }
}