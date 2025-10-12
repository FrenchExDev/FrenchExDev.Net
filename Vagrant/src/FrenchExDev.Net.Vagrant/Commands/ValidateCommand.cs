namespace FrenchExDev.Net.Vagrant.Commands;

public sealed record ValidateCommand : VagrantCommandBase
{
    public override IReadOnlyList<string> ToArguments() => new List<string> { "validate" };
}
