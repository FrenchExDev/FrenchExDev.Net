#region Licensing

// Copyright Stéphane Erard
// For licensing, please contact stephane.erard@gmail

#endregion

namespace FrenchExDev.Net.Ssh.Config;

/// <summary>
/// Specifies an option that can be set to Yes, No, or Ask to indicate a positive, negative, or user-prompted response.
/// </summary>
/// <remarks>Use this enumeration to represent settings or choices where a decision may be affirmative, negative,
/// or deferred to user input. The Ask value typically indicates that the application should prompt the user for a
/// decision at runtime.</remarks>
public enum YesNoAskEnum
{
    Yes,
    No,
    Ask
}