using FrenchExDev.Net.CSharp.Object.Builder;
using FrenchExDev.Net.CSharp.Object.Builder.Abstractions;

namespace FrenchExDev.Net.Mm.Abstractions;

/// <summary>
/// Represents a builder for creating instances of <see cref="BasicModuleInformation"/>.
/// </summary>
public class BasicModuleInformationBuilder : AbstractObjectBuilder<BasicModuleInformation, BasicModuleInformationBuilder>
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

    protected override IObjectBuildResult<BasicModuleInformation> BuildInternal(ExceptionBuildDictionary exceptions, VisitedObjectsList visited)
    {
        if (string.IsNullOrEmpty(_name))
        {
            exceptions.Add(new InvalidDataException($"The {nameof(Name)} field is required."));
        }

        if (string.IsNullOrEmpty(_displayName))
        {
            exceptions.Add(new InvalidDataException($"The {nameof(DisplayName)} field is required."));
        }

        if (string.IsNullOrEmpty(_description))
        {
            exceptions.Add(new InvalidDataException($"The {nameof(Description)} field is required."));
        }

        if (exceptions.Count > 0)
        {
            return Failure(exceptions, visited);
        }

        return Success(new BasicModuleInformation(
            _displayName ?? throw new InvalidDataException(nameof(_displayName)),
            _name ?? throw new InvalidDataException(nameof(_name)),
            _description ?? throw new InvalidDataException(nameof(_description)),
            _website ?? "",
            _documentation ?? ""
        ));
    }
}