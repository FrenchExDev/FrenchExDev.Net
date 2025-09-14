#region Licensing

// Copyright Stéphane Erard
// For licensing, please contact stephane.erard@gmail

#endregion

namespace FrenchExDev.Net.Ssh.Config;

/// <summary>
/// Provides functionality to convert values of the YesNoAskEnum enumeration to their corresponding string
/// representations.
/// </summary>
/// <remarks>This class implements the IEnumStringBuilder<YesNoAskEnum> interface, allowing standardized
/// conversion of YesNoAskEnum values to strings for scenarios such as serialization, display, or configuration. The
/// string representations are "yes", "no", and "ask" for the respective enumeration values.</remarks>
public class YesNoAskEnumStringBuilder : IEnumStringBuilder<YesNoAskEnum>
{
    /// <summary>
    /// Converts a value of <see cref="YesNoAskEnum"/> to its corresponding string representation.
    /// </summary>
    /// <param name="enum">The <see cref="YesNoAskEnum"/> value to convert. Must be one of <see cref="YesNoAskEnum.Yes"/>, <see
    /// cref="YesNoAskEnum.No"/>, or <see cref="YesNoAskEnum.Ask"/>.</param>
    /// <returns>A string representing the specified <paramref name="enum"/> value: "yes" for <see cref="YesNoAskEnum.Yes"/>,
    /// "no" for <see cref="YesNoAskEnum.No"/>, or "ask" for <see cref="YesNoAskEnum.Ask"/>.</returns>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="enum"/> is not a defined value of <see cref="YesNoAskEnum"/>.</exception>
    public string Build(YesNoAskEnum @enum)
    {
        return @enum switch
        {
            YesNoAskEnum.Ask => "ask",
            YesNoAskEnum.No => "no",
            YesNoAskEnum.Yes => "yes",
            _ => throw new NotImplementedException()
        };
    }
}