namespace FrenchExDev.Net.Vagrant;

/// <summary>
/// Represents the vagrant cloud box show command.
/// </summary>
public sealed class VagrantCloudBoxShowCommand : VagrantCloudCommand
{
    /// <summary>The name of the box (username/boxname).</summary>
    public required string Name { get; init; }

    /// <summary>Display versions in JSON format.</summary>
    public bool Json { get; init; }

    /// <summary>Display only the box versions.</summary>
    public bool Versions { get; init; }

    /// <inheritdoc/>
    public override IReadOnlyList<string> ToArguments()
    {
        var args = new List<string> { "cloud", "box", "show" };
        if (Json)
            args.Add("--json");
        if (Versions)
            args.Add("--versions");
        args.Add(Name);
        return args;
    }
}
