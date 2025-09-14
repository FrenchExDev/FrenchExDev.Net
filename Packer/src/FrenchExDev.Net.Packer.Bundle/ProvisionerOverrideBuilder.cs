#region Licensing

// Copyright Stéphane Erard
// For licensing, please contact stephane.erard@gmail

#endregion

#region Usings

using FrenchExDev.Net.CSharp.Object.Builder;
using FrenchExDev.Net.CSharp.Object.Builder.Abstractions;

#endregion

namespace FrenchExDev.Net.Packer.Bundle;

public class ProvisionerOverrideBuilder : AbstractObjectBuilder<ProvisionerOverride, ProvisionerOverrideBuilder>
{
    private VirtualBoxIsoProvisionerOverride? _virtualBoxIso;

    public ProvisionerOverrideBuilder VirtualBoxIso(VirtualBoxIsoProvisionerOverride? value)
    {
        _virtualBoxIso = value;
        return this;
    }

    protected override IObjectBuildResult<ProvisionerOverride> BuildInternal(ExceptionBuildDictionary exceptions, VisitedObjectsList visited)
    {
        return Success(new ProvisionerOverride() { VirtualBoxIso = _virtualBoxIso });
    }
}