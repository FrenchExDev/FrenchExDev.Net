#region Licensing

// Copyright Stéphane Erard
// For licensing, please contact stephane.erard@gmail

#endregion

namespace FrenchExDev.Net.Ssh.Config;

/// <summary>
/// Provides functionality to convert values of the ControlMasterEnum enumeration to their corresponding SSH
/// configuration string representations.
/// </summary>
/// <remarks>This class is typically used to generate valid string values for the 'ControlMaster' option in SSH
/// configuration files. It ensures that only supported enumeration values are converted; unsupported values will result
/// in an exception.</remarks>
public class ControlMasterEnumStringBuilder : IEnumStringBuilder<ControlMasterEnum>
{
    /// <summary>
    /// Converts a specified <see cref="ControlMasterEnum"/> value to its corresponding string representation for use in
    /// configuration settings.
    /// </summary>
    /// <remarks>This method is typically used to generate configuration values for systems that require
    /// string representations of control master settings. Only defined <see cref="ControlMasterEnum"/> values are
    /// supported; passing an undefined value will result in an exception.</remarks>
    /// <param name="enum">The <see cref="ControlMasterEnum"/> value to convert. Must be a defined enumeration value.</param>
    /// <returns>A string representing the specified <paramref name="enum"/> value, suitable for configuration usage.</returns>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="enum"/> is not a recognized <see cref="ControlMasterEnum"/> value.</exception>
    public string Build(ControlMasterEnum @enum)
    {
        return @enum switch
        {
            ControlMasterEnum.Yes => "yes",
            ControlMasterEnum.No => "no",
            ControlMasterEnum.Ask => "ask",
            ControlMasterEnum.Auto => "auto",
            ControlMasterEnum.AutoAsk => "autoask",
            _ => throw new NotImplementedException()
        };
    }
}