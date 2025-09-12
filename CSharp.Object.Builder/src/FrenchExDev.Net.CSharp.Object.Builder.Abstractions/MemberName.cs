
namespace FrenchExDev.Net.CSharp.Object.Builder.Abstractions;

/// <summary>
/// Represents a member's name as an immutable value object.
/// </summary>
/// <param name="Name">The name of the member to be represented. Cannot be null.</param>
public record MemberName(string Name);

/// <summary>
/// Provides extension methods for converting strings to <see cref="MemberName"/> instances.
/// </summary>
public static class MemberNameExtensions
{
    /// <summary>
    /// Converts a string to a <see cref="MemberName"/> instance.
    /// </summary>
    /// <param name="name">The string to convert. Cannot be null.</param>
    /// <returns>A <see cref="MemberName"/> instance representing the provided string.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the provided string is null.</exception>
    public static MemberName ToMemberName(this string name)
    {
        if (name is null) throw new ArgumentNullException(nameof(name));
        return new MemberName(name);
    }
}