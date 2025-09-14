#region Licensing

// Copyright Stéphane Erard
// For licensing, please contact stephane.erard@gmail

#endregion

namespace FrenchExDev.Net.Ssh.Config;

/// <summary>
/// Provides functionality to convert string representations of yes/no/ask values to the corresponding <see
/// cref="YesNoAskEnum"/> enumeration values.
/// </summary>
public class YesNoAskEnumReader
{
    /// <summary>
    /// Converts the specified string representation to its corresponding YesNoAskEnum value.
    /// </summary>
    /// <remarks>The conversion is case-sensitive. Only the exact strings "yes", "no", and "ask" are
    /// supported.</remarks>
    /// <param name="value">The string value to convert. Must be "yes", "no", or "ask" (case-sensitive).</param>
    /// <returns>A YesNoAskEnum value that corresponds to the specified string.</returns>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="value"/> does not match "yes", "no", or "ask".</exception>
    public YesNoAskEnum Read(string value)
    {
        return value switch
        {
            "ask" => YesNoAskEnum.Ask,
            "no" => YesNoAskEnum.No,
            "yes" => YesNoAskEnum.Yes,
            _ => throw new NotImplementedException()
        };
    }
}