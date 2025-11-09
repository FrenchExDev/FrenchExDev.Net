using Newtonsoft.Json.Linq;
using Formatting = Newtonsoft.Json.Formatting;

namespace FrenchExDev.Net.CSharp.Aspire.App;

/// <summary>
/// Provides helper methods for adding or updating environment variables in launchSettings.json profiles for .NET
/// projects.
/// </summary>
/// <remarks>This class offers static methods to simplify the management of environment variables within
/// launchSettings.json, including support for creating profiles and environment variable sections if they do not exist.
/// All methods are thread-safe with respect to their own execution, but concurrent access to the same
/// launchSettings.json file from multiple processes may result in race conditions. Use these methods to
/// programmatically configure environment variables for specific launch profiles, such as during build or deployment
/// automation.</remarks>
public static class LaunchSettingsHelper
{
    /// <summary>
    /// Adds or updates a single environment variable in the specified launchSettings.json profile.
    /// Creates the file/profile/environmentVariables object if necessary.
    /// </summary>
    public static void AddOrUpdateEnvironmentVariable(string launchSettingsPath, string profileName, string variableName, string variableValue)
    {
        AddOrUpdateEnvironmentVariables(launchSettingsPath, profileName, new Dictionary<string, string> { { variableName, variableValue } });
    }

    /// <summary>
    /// Adds or updates multiple environment variables in the specified launchSettings.json profile.
    /// </summary>
    public static void AddOrUpdateEnvironmentVariables(string launchSettingsPath, string profileName, IDictionary<string, string> variables)
    {
        JObject root;

        if (File.Exists(launchSettingsPath))
        {
            var text = File.ReadAllText(launchSettingsPath);
            root = string.IsNullOrWhiteSpace(text) ? new JObject() : JObject.Parse(text);
        }
        else
        {
            root = new JObject();
        }

        // Ensure 'profiles' object exists
        if (root["profiles"] == null || root["profiles"].Type != JTokenType.Object)
            root["profiles"] = new JObject();

        var profiles = (JObject)root["profiles"]!;

        // Ensure the profile object exists
        if (profiles[profileName] == null || profiles[profileName].Type != JTokenType.Object)
            profiles[profileName] = new JObject();

        var profile = (JObject)profiles[profileName]!;

        // Ensure environmentVariables object exists
        if (profile["environmentVariables"] == null || profile["environmentVariables"].Type != JTokenType.Object)
            profile["environmentVariables"] = new JObject();

        var env = (JObject)profile["environmentVariables"]!;

        foreach (var kvp in variables)
        {
            env[kvp.Key] = JToken.FromObject(kvp.Value);
        }

        // Persist back to file with indentation
        var serialized = root.ToString(Formatting.Indented);
        var directory = Path.GetDirectoryName(launchSettingsPath);
        if (!string.IsNullOrEmpty(directory)) Directory.CreateDirectory(directory);
        File.WriteAllText(launchSettingsPath, serialized);
    }

    /// <summary>
    /// Convenience: add/update environment variable for the profile named in DOTNET_LAUNCH_PROFILE env var.
    /// Falls back to provided defaultProfile if env var is not present.
    /// </summary>
    public static void AddOrUpdateEnvironmentVariableUsingEnvProfile(string launchSettingsPath, string variableName, string variableValue, string defaultProfile = "IIS Express")
    {
        var profile = Environment.GetEnvironmentVariable("DOTNET_LAUNCH_PROFILE");
        if (string.IsNullOrEmpty(profile)) profile = defaultProfile;
        AddOrUpdateEnvironmentVariable(launchSettingsPath, profile, variableName, variableValue);
    }
}
