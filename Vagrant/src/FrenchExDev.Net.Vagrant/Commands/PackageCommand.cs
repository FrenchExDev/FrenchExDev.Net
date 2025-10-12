namespace FrenchExDev.Net.Vagrant.Commands;

public sealed record PackageCommand : VagrantCommandBase
{
    public string? Output { get; init; }
    public List<string>? Include { get; init; }
    public string? Base { get; init; }
    public string? Info { get; init; }
    public string? Vagrantfile { get; init; }

    public override IReadOnlyList<string> ToArguments()
    {
        var args = new List<string> { "package" };

        if (!string.IsNullOrWhiteSpace(Output)) { args.Add("--output"); args.Add(Output!); }

        if (Include?.Count > 0)
        {
            args.Add("--include");
            foreach (var file in Include)
            {
                args.Add(file);
            }
        }

        if (!string.IsNullOrWhiteSpace(Base)) { args.Add("--base"); args.Add(Base!); }

        BaseArguments(args);

        return args;
    }
}
