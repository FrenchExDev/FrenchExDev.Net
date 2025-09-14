#region Licensing

// Copyright Stéphane Erard
// For licensing, please contact stephane.erard@gmail

#endregion

namespace FrenchExDev.Net.Ssh.Config;

/// <summary>
/// Defines a mechanism for reading a sequence of text lines and producing an object of type TBuild.
/// </summary>
/// <typeparam name="TBuild">The type of object that is constructed from the input lines.</typeparam>
public interface IReader<out TBuild>
{
    /// <summary>
    /// Parses the specified collection of text lines and constructs an instance of type TBuild.
    /// </summary>
    /// <param name="lines">A sequence of strings representing the lines to be parsed. Cannot be null.</param>
    /// <returns>An instance of type TBuild created from the provided lines.</returns>
    TBuild Read(IEnumerable<string> lines);
}