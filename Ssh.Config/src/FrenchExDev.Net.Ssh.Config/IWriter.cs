#region Licensing

// Copyright Stéphane Erard
// For licensing, please contact stephane.erard@gmail

#endregion

namespace FrenchExDev.Net.Ssh.Config;

/// <summary>
/// Defines a mechanism for writing a subject of type <typeparamref name="TSubject"/> and returning the resulting output
/// as a list of strings.
/// </summary>
/// <remarks>Implementations of this interface can be used to serialize, format, or otherwise process subjects
/// into string representations. The output list may contain multiple lines or segments, depending on the writer's
/// implementation.</remarks>
/// <typeparam name="TSubject">The type of the subject to be written. This type is provided to the writer for processing.</typeparam>
public interface IWriter<in TSubject>
{
    /// <summary>
    /// Generates a list of string representations for the specified subject.
    /// </summary>
    /// <param name="subject">The subject to be converted into one or more string representations. Cannot be null.</param>
    /// <returns>A list of strings representing the subject. The list will be empty if no representations are generated.</returns>
    List<string> Write(TSubject subject);
}