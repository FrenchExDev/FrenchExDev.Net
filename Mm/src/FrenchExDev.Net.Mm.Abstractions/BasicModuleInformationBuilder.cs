using FrenchExDev.Net.CSharp.Object.Builder.Abstractions;

namespace FrenchExDev.Net.Mm.Abstractions;

/// <summary>
/// Represents a builder for creating instances of <see cref="BasicModuleInformation"/>.
/// </summary>
public class BasicModuleInformationBuilder : IBuilder<BasicModuleInformation>
{
    private string? _displayName;
    private string? _name;
    private string? _description;
    private string? _website;
    private string? _documentation;

    public BasicModuleInformationBuilder DisplayName(string displayName)
    {
        _displayName = displayName;
        return this;
    }

    public BasicModuleInformationBuilder Name(string name)
    {
        _name = name;
        return this;
    }

    public BasicModuleInformationBuilder Description(string description)
    {
        _description = description;
        return this;
    }

    public BasicModuleInformationBuilder Website(string website)
    {
        _website = website ?? "";
        return this;
    }

    public BasicModuleInformationBuilder Documentation(string documentation)
    {
        _documentation = documentation ?? "";
        return this;
    }

    public IBuildResult<BasicModuleInformation> Build()
    {
        return new BasicBuildResult<BasicModuleInformation>(true,
            new BasicModuleInformation(
                _displayName ?? throw new ArgumentNullException(nameof(_displayName)),
                _name ?? throw new ArgumentNullException(nameof(_name)),
                _description ?? throw new ArgumentNullException(nameof(_description)),
                _website ?? string.Empty,
                _documentation ?? string.Empty));
    }
}