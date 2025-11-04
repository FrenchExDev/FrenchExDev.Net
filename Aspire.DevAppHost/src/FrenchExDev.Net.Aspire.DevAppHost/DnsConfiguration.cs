using FrenchExDev.Net.CSharp.Object.Builder2;
using FrenchExDev.Net.CSharp.Object.Result;
using System.Text.Json;

namespace FrenchExDev.Net.Aspire.DevAppHost;

/// <summary>
/// Provides a builder for configuring and creating instances of the DnsConfiguration class.
/// </summary>
/// <remarks>Use this builder to fluently specify DNS configuration settings, such as the domain, DNS records, and
/// certificates directory, before constructing a DnsConfiguration object. This class is not thread-safe. For each
/// configuration, use a separate instance of the builder.</remarks>
public class DnsConfiguration2Builder : AbstractBuilder<DnsConfiguration2>
{
    private string? _domain;
    private Dictionary<string, DnsRecord2>? _records;
    private string? _certsDir;

    public DnsConfiguration2Builder WithDomain(string domain)
    {
        _domain = domain;
        return this;
    }

    public DnsConfiguration2Builder WithRecords(Dictionary<string, DnsRecord2> records)
    {
        _records = records;
        return this;
    }

    public DnsConfiguration2Builder WithCertificatesDirectory(string certsDir)
    {
        _certsDir = certsDir;
        return this;
    }

    public DnsConfiguration2Builder WithRecord(string name, DnsRecord2 record)
    {
        _records ??= new Dictionary<string, DnsRecord2>();
        _records[name] = record;
        return this;
    }

    public DnsConfiguration2Builder WithRecord(string name, string domain, int port, string ipv4, string? template = "", int? portRangeStart = 0)
    {
        _records ??= new Dictionary<string, DnsRecord2>();
        _records[name] = new DnsRecord2
        {
            Domain = domain,
            Port = port,
            DomainTemplate = template,
            Ipv4 = ipv4,
            PortRangeStart = portRangeStart
        };
        return this;
    }

    protected override DnsConfiguration2 Instantiate()
    {
        return new DnsConfiguration2()
        {
            Domain = _domain ?? throw new InvalidDataException(nameof(_domain)),
            Subdomains = _records ?? throw new InvalidDataException(nameof(_records)),
            CertificatesDirectory = _certsDir ?? throw new InvalidDataException(nameof(_certsDir))
        };
    }
}

/// <summary>
/// Represents the configuration settings for a DNS domain, including domain name, DNS records, and certificate file
/// paths.
/// </summary>
/// <remarks>Use this record to manage DNS-related configuration for a domain, including the mapping of DNS
/// records and the locations of associated certificate files. This type is typically used in scenarios where DNS and
/// certificate management are required together, such as automated certificate provisioning or DNS-based validation
/// workflows.</remarks>
public record DnsConfiguration
{
    public required string Domain { get; init; }
    public required Dictionary<string, DnsRecord2> Subdomains { get; init; }
    public string? CertPath { get; set; }
    public string? KeyPath { get; set; }
    public required string CertificatesDirectory { get; init; }
    public string CertPathOrDie() => CertPath ?? throw new InvalidOperationException("Certificates not initialized. Call EnsureSetup first.");
    public string KeyPathOrDie() => KeyPath ?? throw new InvalidOperationException("Certificates not initialized. Call EnsureSetup first.");
    public static DnsConfiguration Default()
    {
        return new DnsConfiguration
        {
            Domain = string.Empty,
            Subdomains = new Dictionary<string, DnsRecord2>(),
            CertificatesDirectory = string.Empty
        };
    }

    public Result<string> GetDomain(string name)
    {
        var exist = Subdomains.TryGetValue(name, out var dns);
        if (!exist || dns is null) return Result<string>.Failure();
        return (dns.Domain + "." + Domain).ToSuccess();
    }

    /// <summary>
    /// Deserializes a JSON string into a new instance of the DnsConfiguration class.
    /// </summary>
    /// <param name="json">A JSON-formatted string that represents a DnsConfiguration object. Cannot be null.</param>
    /// <returns>A DnsConfiguration instance deserialized from the specified JSON string, or null if the JSON is null or invalid.</returns>
    public static Result<DnsConfiguration> FromJson(string json) => Result<DnsConfiguration>.TryCatch(() => JsonSerializer.Deserialize<DnsConfiguration>(json) ?? throw new InvalidOperationException());

    /// <summary>
    /// Converts the current object to its JSON string representation.
    /// </summary>
    /// <remarks>The resulting JSON string includes all public properties of the object. The output may vary
    /// depending on the object's structure and the default serialization options. If the object contains circular
    /// references or unsupported types, serialization may fail.</remarks>
    /// <returns>A JSON-formatted string that represents the current object.</returns>
    public Result<string> ToJson()
    {
        return Result<string>.TryCatch(() => JsonSerializer.Serialize(this));
    }

    /// <summary>
    /// Retrieves the port number associated with the specified DNS record name.
    /// </summary>
    /// <param name="name">The name of the DNS record for which to retrieve the port number. Cannot be null.</param>
    /// <returns>A <see cref="Result{T}"/> containing the port number if the DNS record exists; otherwise, a failure result.</returns>
    public Result<int> GetPortOfDns(string name)
    {
        var exist = Subdomains.TryGetValue(name, out var dns);

        if (!exist || dns is null || dns.Port is null) return Result<int>.Failure();

        return dns.Port.Value.ToSuccess();
    }
}
