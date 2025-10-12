using FrenchExDev.Net.Vagrant.Commands;
using FrenchExDev.Net.Vagrant.Commands.Builders;
using FrenchExDev.Net.Vagrant.Testing;
using Shouldly;

namespace FrenchExDev.Net.Vagrant.Tests.Builders;

public class PortCommandBuilderTests : CommandBuilderTester<PortCommandBuilder, PortCommand>
{
    [Fact]
    public void Build_Default_NoFlagsOrMachine()
    {
        Valid(
            builder => builder.WorkingDirectory("foo"),
            cmd => { cmd.Private.ShouldBeNull(); cmd.Public.ShouldBeNull(); cmd.Machine.ShouldBeNull(); },
            args => { args.ShouldBe(new[] { "port" }.ToList()); }
        );
    }

    [Fact]
    public void Build_WithPrivateTrue_IncludesPrivateFlag()
    {
        Valid(
            builder => builder.Private().WorkingDirectory("foo"),
            cmd => { cmd.Private.ShouldBe(true); },
            args => { args.ShouldContain("--private"); }
        );
    }

    [Fact]
    public void Build_WithPrivateFalse_SetsFalse()
    {
        Valid(
            builder => builder.Private(false).WorkingDirectory("foo"),
            cmd => { cmd.Private.ShouldBe(false); },
            args => { args.ShouldBe(new[] { "port" }.ToList()); }
        );
    }

    [Fact]
    public void Build_WithPublicTrue_IncludesPublicFlag()
    {
        Valid(
            builder => builder.Public().WorkingDirectory("foo"),
            cmd => { cmd.Public.ShouldBe(true); },
            args => { args.ShouldContain("--public"); }
        );
    }

    [Fact]
    public void Build_WithMachine_IncludesMachineOption()
    {
        Valid(
            builder => builder.Machine("vm1").WorkingDirectory("foo"),
            cmd => { cmd.Machine.ShouldBe("vm1"); },
            args => { args.ShouldContain("--machine"); args.ShouldContain("vm1"); }
        );
    }

    [Fact]
    public void Build_WithBothFlagsAndMachine_IncludesAll()
    {
        Valid(
            builder => builder.Private().Public().Machine("vm1").WorkingDirectory("foo"),
            cmd => { cmd.Private.ShouldBe(true); cmd.Public.ShouldBe(true); cmd.Machine.ShouldBe("vm1"); },
            args => { args.ShouldContain("--private"); args.ShouldContain("--public"); args.ShouldContain("vm1"); }
        );
    }
}
