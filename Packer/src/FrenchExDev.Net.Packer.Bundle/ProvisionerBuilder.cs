#region Licensing

// Copyright Stéphane Erard
// For licensing, please contact stephane.erard@gmail

#endregion

#region Usings

using FrenchExDev.Net.CSharp.Object.Builder;
using FrenchExDev.Net.CSharp.Object.Builder.Abstractions;

#endregion

namespace FrenchExDev.Net.Packer.Bundle;

public class ProvisionerBuilder : AbstractObjectBuilder<Provisioner, ProvisionerBuilder>
{
    private ProvisionerOverride? _override;
    private string? _pauseBefore;
    private List<string>? _scripts;
    private string? _type;

    public ProvisionerBuilder PauseBefore(string? value)
    {
        _pauseBefore = value;
        return this;
    }

    public ProvisionerBuilder Type(string? type)
    {
        _type = type;
        return this;
    }

    public ProvisionerBuilder AddScript(string name)
    {
        _scripts ??= new List<string>();
        _scripts.Add(name);
        return this;
    }

    public ProvisionerBuilder BeforeScript(string before, string name)
    {
        _scripts ??= new List<string>();
        var index = _scripts.IndexOf(before);
        if (index == -1) return this;

        _scripts.Insert(index, name);
        return this;
    }

    public ProvisionerBuilder Override(ProvisionerOverride? value)
    {
        _override = value;
        return this;
    }

    protected override IObjectBuildResult<Provisioner> BuildInternal(ExceptionBuildDictionary exceptions, VisitedObjectsList visited)
    {
        if (string.IsNullOrEmpty(_type))
            exceptions.Add(nameof(_type), new MissingFieldException(nameof(_type)));

        if (exceptions.Count > 0)
            return Failure(exceptions, visited);

        return Success(new Provisioner
        {
            Type = _type,
            Scripts = _scripts,
            Override = _override,
            PauseBefore = _pauseBefore
        });
    }
}