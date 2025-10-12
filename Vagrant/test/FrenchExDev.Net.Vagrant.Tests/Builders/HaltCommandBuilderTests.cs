using FrenchExDev.Net.Vagrant.Commands;
using FrenchExDev.Net.Vagrant.Commands.Builders;
using FrenchExDev.Net.Vagrant.Testing;
using Shouldly;

namespace FrenchExDev.Net.Vagrant.Tests.Builders;

public class HaltCommandBuilderTests : CommandBuilderTester<HaltCommandBuilder, HaltCommand>
{
    [Fact]
    public void Build_Default_NoNameOrForce()
    {
        Valid(
            builder => builder.WorkingDirectory("foo"),
            cmd => { cmd.Name.ShouldBeNull(); cmd.Force.ShouldBeNull(); },
            args => { args.ShouldBe(new[] { "halt" }.ToList()); }
        );
    }

    [Fact]
    public void Build_WithName_IncludesName()
    {
        Valid(
            builder => builder.Name("vm1").WorkingDirectory("foo"),
            cmd => { cmd.Name.ShouldBe("vm1"); },
            args => { args.ShouldBe(new[] { "halt" }.ToList()); }
        );
    }

    [Fact]
    public void Build_WithForceTrue_IncludesForceFlag()
    {
        Valid(
            builder => builder.Force().WorkingDirectory("foo"),
            cmd => { cmd.Force.ShouldBe(true); },
            args => { args.ShouldContain("-f"); }
        );
    }

    [Fact]
    public void Build_WithForceFalse_SetsFalse()
    {
        Valid(
            builder => builder.Force(false).WorkingDirectory("foo"),
            cmd => { cmd.Force.ShouldBe(false); },
            args => { args.ShouldBe(new[] { "halt" }.ToList()); }
        );
    }
}
