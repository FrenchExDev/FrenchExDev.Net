using FrenchExDev.Net.Vagrant.Commands;
using FrenchExDev.Net.Vagrant.Commands.Builders;
using FrenchExDev.Net.Vagrant.Testing;
using Shouldly;

namespace FrenchExDev.Net.Vagrant.Tests.Builders;

public class PluginExpungeCommandBuilderTests : CommandBuilderTester<PluginExpungeCommandBuilder, PluginExpungeCommand>
{
    [Fact]
    public void Build_Default_NoFlags()
    {
        Valid(
            builder => builder.WorkingDirectory("foo"),
            cmd => { cmd.Force.ShouldBeNull(); cmd.Reinstall.ShouldBeNull(); },
            args => { args.ShouldBe(new[] { "plugin", "expunge" }.ToList()); }
        );
    }

    [Fact]
    public void Build_WithForceTrue_IncludesForce()
    {
        Valid(
            builder => builder.Force().WorkingDirectory("foo"),
            cmd => { cmd.Force.ShouldBe(true); },
            args => { args.ShouldContain("--force"); }
        );
    }

    [Fact]
    public void Build_WithForceFalse_SetsFalse()
    {
        Valid(
            builder => builder.Force(false).WorkingDirectory("foo"),
            cmd => { cmd.Force.ShouldBe(false); },
            args => { args.ShouldBe(new[] { "plugin", "expunge" }.ToList()); }
        );
    }

    [Fact]
    public void Build_WithReinstallTrue_IncludesReinstall()
    {
        Valid(
            builder => builder.Reinstall().WorkingDirectory("foo"),
            cmd => { cmd.Reinstall.ShouldBe(true); },
            args => { args.ShouldContain("--reinstall"); }
        );
    }

    [Fact]
    public void Build_WithBothFlags_IncludesBoth()
    {
        Valid(
            builder => builder.Force().Reinstall().WorkingDirectory("foo"),
            cmd => { cmd.Force.ShouldBe(true); cmd.Reinstall.ShouldBe(true); },
            args => { args.ShouldContain("--force"); args.ShouldContain("--reinstall"); }
        );
    }
}
