#region Licensing

// Copyright Stéphane Erard
// For licensing, please contact stephane.erard@gmail

#endregion

#region Usings

using FrenchExDev.Net.CSharp.Object.Builder;
using FrenchExDev.Net.CSharp.Object.Builder.Abstractions;

#endregion

namespace FrenchExDev.Net.Packer.Bundle;

/// <summary>
/// Provides a builder for constructing instances of <see cref="HttpDirectory"/> with a customizable set of files for
/// HTTP-based directory scenarios.
/// </summary>
/// <remarks>Use <see cref="AddFile"/> and <see cref="RemoveFile"/> to manage the files included in the directory
/// before building the final <see cref="HttpDirectory"/> instance. This builder supports fluent chaining of
/// configuration methods. The resulting <see cref="HttpDirectory"/> will contain all files added via this builder at
/// the time of construction.</remarks>
public class HttpDirectoryBuilder : DeconstructedAbstractObjectBuilder<HttpDirectory, HttpDirectoryBuilder>
{
    /// <summary>
    /// Stores the files to be included in the <see cref="HttpDirectory"/> being built.
    /// </summary>
    private readonly Dictionary<string, IFile> _files = new();

    /// <summary>
    /// Adds a file to the directory with the specified name.
    /// </summary>
    /// <param name="name">The name to associate with the file in the directory. Cannot be null or empty.</param>
    /// <param name="file">The file to add to the directory. Cannot be null.</param>
    /// <returns>The current <see cref="HttpDirectoryBuilder"/> instance to allow method chaining.</returns>
    public HttpDirectoryBuilder AddFile(string name, IFile file)
    {
        _files.Add(name, file);
        return this;
    }

    /// <summary>
    /// Removes the file with the specified name from the directory builder.
    /// </summary>
    /// <param name="name">The name of the file to remove. Cannot be null or empty.</param>
    /// <returns>The current <see cref="HttpDirectoryBuilder"/> instance to allow method chaining.</returns>
    public HttpDirectoryBuilder RemoveFile(string name)
    {
        _files.Remove(name);
        return this;
    }

    /// <summary>
    /// Creates a new instance of the <see cref="HttpDirectory"/> class initialized with the current set of files.
    /// </summary>
    /// <returns>A <see cref="HttpDirectory"/> object containing the files managed by this instance.</returns>
    protected override HttpDirectory Instantiate()
    {
        return new HttpDirectory()
        {
            Files = _files
        };
    }

    /// <summary>
    /// Validates the collection of files and adds any detected validation errors to the specified exception dictionary.
    /// </summary>
    /// <param name="exceptions">A dictionary used to collect validation exceptions for files that fail validation. Must not be null.</param>
    /// <param name="visited">A list of objects that have already been visited during validation to prevent redundant checks. Must not be
    /// null.</param>
    protected override void Validate(ExceptionBuildDictionary exceptions, VisitedObjectsList visited)
    {
        foreach (var file in _files)
            if (file.Value.Extension is "")
                exceptions.Add(file.Key, new InvalidDataException(nameof(file.Key)));
    }
}