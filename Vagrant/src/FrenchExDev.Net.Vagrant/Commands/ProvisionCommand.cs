namespace FrenchExDev.Net.Vagrant.Commands;

public sealed record ProvisionCommand : VagrantCommandBase
{
    public string? Name { get; init; }
    public List<string>? ProvisionWith { get; init; }
    public override IReadOnlyList<string> ToArguments()
    {
        var args = new List<string> { "provision" };

        if (!string.IsNullOrWhiteSpace(Name)) args.Add(Name);

        if (ProvisionWith?.Count > 0)
        {
            args.Add("--provision-with");
            foreach (var name in ProvisionWith)
            {
                args.Add(name);
            }
        }

        return args;
    }
}
