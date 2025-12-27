#region Licensing

// Copyright Stéphane Erard
// For licensing, please contact stephane.erard@gmail

#endregion

namespace FrenchExDev.Net.Ssh.Config;

/// <summary>
/// Provides functionality to convert values of the AddressFamilyEnum enumeration to their corresponding string
/// representations.
/// </summary>
/// <remarks>This class is typically used to generate protocol-specific string values for address family settings.
/// It implements IEnumStringBuilder<AddressFamilyEnum> to support consistent string conversion for address family
/// enumerations.</remarks>
public class AddressFamilyStringBuilder : IEnumStringBuilder<AddressFamilyEnum>
{
    /// <summary>
    /// Converts the specified address family enumeration value to its corresponding string representation.
    /// </summary>
    /// <param name="enum">The address family value to convert. Must be a defined member of <see cref="AddressFamilyEnum"/>.</param>
    /// <returns>A string representing the specified address family. Returns "all" for <see cref="AddressFamilyEnum.All"/>,
    /// "inet" for <see cref="AddressFamilyEnum.Inet"/>, and "inet6" for <see cref="AddressFamilyEnum.Inet6"/>.</returns>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="enum"/> is not a supported value of <see cref="AddressFamilyEnum"/>.</exception>
    public string Build(AddressFamilyEnum @enum)
    {
        return @enum switch
        {
            AddressFamilyEnum.All => "all",
            AddressFamilyEnum.Inet => "inet",
            AddressFamilyEnum.Inet6 => "inet6",
            _ => throw new NotImplementedException()
        };
    }
}