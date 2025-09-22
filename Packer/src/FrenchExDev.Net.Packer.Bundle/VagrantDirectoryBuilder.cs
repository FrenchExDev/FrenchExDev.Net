#region Licensing

// Copyright Stéphane Erard
// For licensing, please contact stephane.erard@gmail

#endregion

#region Usings

using FrenchExDev.Net.CSharp.Object.Builder2;

#endregion

namespace FrenchExDev.Net.Packer.Bundle;

/// <summary>
/// Provides a fluent builder for configuring and constructing instances of <see cref="VagrantDirectory"/>.
/// </summary>
/// <remarks>
/// Use this builder to add or remove files in the Vagrant directory for a Packer bundle. Each configuration method returns the builder instance, allowing for method chaining.
/// The builder validates that all mandatory properties are set before constructing the final <see cref="VagrantDirectory"/> object.
/// This class is not thread-safe.
/// <example>
/// <code>
/// var builder = new VagrantDirectoryBuilder()
///     .AddFile("Vagrantfile", new File())
///     .RemoveFile("oldfile.txt");
/// var result = builder.Build();
/// </code>
/// </example>
/// </remarks>
public class VagrantDirectoryBuilder : AbstractBuilder<VagrantDirectory>
{
    /// <summary>
    /// Dictionary of files to include in the Vagrant directory, keyed by file name.
    /// </summary>
    private readonly Dictionary<string, IFile> _files = new();

    /// <summary>
    /// Adds a file to the Vagrant directory.
    /// </summary>
    /// <param name="name">File name (e.g., "Vagrantfile").</param>
    /// <param name="file">File instance implementing <see cref="IFile"/>.</param>
    /// <returns>The builder instance.</returns>
    /// <remarks>Example: AddFile("Vagrantfile", new File())</remarks>
    public VagrantDirectoryBuilder AddFile(string name, IFile file)
    {
        _files.Add(name, file);
        return this;
    }

    /// <summary>
    /// Removes a file from the Vagrant directory by name.
    /// </summary>
    /// <param name="name">File name to remove.</param>
    /// <returns>The builder instance.</returns>
    /// <remarks>Example: RemoveFile("oldfile.txt")</remarks>
    public VagrantDirectoryBuilder RemoveFile(string name)
    {
        _files.Remove(name);
        return this;
    }

    /// <summary>
    /// Instantiates a <see cref="VagrantDirectory"/> object using the configured files.
    /// </summary>
    /// <returns>The constructed <see cref="VagrantDirectory"/> instance.</returns>
    protected override VagrantDirectory Instantiate()
    {
        return new VagrantDirectory()
        {
            Files = _files
        };
    }

    /// <summary>
    /// Performs validation on the collection of files, recording any detected failures related to file properties.
    /// </summary>
    /// <remarks>Validation checks include ensuring that each file's extension, path, and new line properties
    /// are not empty. Any violations are reported through the failures dictionary.</remarks>
    /// <param name="visitedCollector">A dictionary used to track objects that have already been visited during validation to prevent redundant checks.</param>
    /// <param name="failures">A dictionary for collecting validation failures. Any issues found during validation are added to this
    /// collection.</param>
    protected new void ValidateInternal(VisitedObjectDictionary visitedCollector, FailuresDictionary failures)
    {
        foreach (var file in _files)
        {
            if (file.Value.Extension is "")
                failures.Failure(nameof(file.Value.Extension), new ArgumentException("File extension cannot be empty.", nameof(file.Value.Extension)));

            if (file.Value.Path is "")
                failures.Failure(nameof(file.Value.Path), new ArgumentException("File path cannot be empty.", nameof(file.Value.Path)));

            if (file.Value.NewLine is "")
                failures.Failure(nameof(file.Value.NewLine), new ArgumentException("File new line cannot be empty.", nameof(file.Value.NewLine)));
        }
    }
}