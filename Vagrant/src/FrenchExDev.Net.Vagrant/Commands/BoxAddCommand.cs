namespace FrenchExDev.Net.Vagrant.Commands;

public sealed record BoxAddCommand : BoxCommand
{
    public string NameOrUrlOrPath { get; init; }

    public BoxAddCommand(string nameOrUrlOrPath)
    {
        NameOrUrlOrPath = nameOrUrlOrPath;
    }
    public string? Provider { get; init; }
    public bool? Force { get; init; }
    public string? Checksum { get; init; }
    public string? ChecksumType { get; init; }
    public string? BoxVersion { get; init; }
    public string? Architecture { get; init; }

    public bool? Insecure { get; init; }
    public string? CaCert { get; init; }
    public string? CaPath { get; init; }
    public string? Cert { get; init; }
    public bool? LocationTrusted { get; init; }

    public override IReadOnlyList<string> ToArguments()
    {
        var args = new List<string> { "box", "add" };
        // flags and options
        if (Force == true) args.Add("--force");
        if (Insecure == true) args.Add("--insecure");
        if (LocationTrusted == true) args.Add("--location-trusted");
        if (!string.IsNullOrWhiteSpace(CaCert)) { args.Add("--cacert"); args.Add(CaCert!); }
        if (!string.IsNullOrWhiteSpace(CaPath)) { args.Add("--capath"); args.Add(CaPath!); }
        if (!string.IsNullOrWhiteSpace(Cert)) { args.Add("--cert"); args.Add(Cert!); }
        if (!string.IsNullOrWhiteSpace(Architecture)) { args.Add("--architecture"); args.Add(Architecture!); }
        if (!string.IsNullOrWhiteSpace(Provider)) { args.Add("--provider"); args.Add(Provider!); }
        if (!string.IsNullOrWhiteSpace(BoxVersion)) { args.Add("--box-version"); args.Add(BoxVersion!); }
        if (!string.IsNullOrWhiteSpace(Checksum)) { args.Add("--checksum"); args.Add(Checksum!); }
        if (!string.IsNullOrWhiteSpace(ChecksumType)) { args.Add("--checksum-type"); args.Add(ChecksumType!); }
        // positional source
        if (!string.IsNullOrWhiteSpace(NameOrUrlOrPath)) args.Add(NameOrUrlOrPath!);
        return args;
    }
}
