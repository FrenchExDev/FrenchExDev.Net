namespace FrenchExDev.Net.Vagrant.Commands;

public sealed record ResumeCommand : VagrantCommandBase
{
    public override IReadOnlyList<string> ToArguments() => new List<string> { "resume" };
}
