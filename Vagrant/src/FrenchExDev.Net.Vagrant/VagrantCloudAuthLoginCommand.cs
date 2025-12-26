namespace FrenchExDev.Net.Vagrant;

/// <summary>
/// Represents the vagrant cloud auth login command.
/// </summary>
public sealed class VagrantCloudAuthLoginCommand : VagrantCloudCommand
{
    /// <summary>Check if currently logged in.</summary>
    public bool Check { get; init; }

    /// <summary>Description of the token.</summary>
    public string? Description { get; init; }

    /// <summary>Token to use for authentication.</summary>
    public string? Token { get; init; }

    /// <inheritdoc/>
    public override IReadOnlyList<string> ToArguments()
    {
        var args = new List<string> { "cloud", "auth", "login" };
        if (Check)
            args.Add("--check");
        if (!string.IsNullOrWhiteSpace(Description))
        {
            args.Add("--description");
            args.Add(Description);
        }
        if (!string.IsNullOrWhiteSpace(Token))
        {
            args.Add("--token");
            args.Add(Token);
        }
        return args;
    }
}
