namespace FrenchExDev.Net.Vagrant;

public sealed class VagrantBoxRepackageCommand : VagrantBoxCommand
{
    /// <summary>The name of the box to repackage.</summary>
    public required string Name { get; init; }

    /// <summary>The provider of the box.</summary>
    public required string Provider { get; init; }

    /// <summary>A cartouche is a carved tablet or drawing representing a scroll with twisted ends.</summary>
    /// <remarks>It is used in the context of packaging and distributing Vagrant boxes.</remarks>
    public required string Cartouche { get; init; }

    /// <inheritdoc/>
    public override IReadOnlyList<string> ToArguments()
    {
        var args = new List<string> { "box", "repackage", Name, Provider, Cartouche };
        return args;
    }
}
