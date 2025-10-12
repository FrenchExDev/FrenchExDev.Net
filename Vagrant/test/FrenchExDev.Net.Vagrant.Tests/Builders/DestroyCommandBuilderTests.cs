using FrenchExDev.Net.Vagrant.Commands;
using FrenchExDev.Net.Vagrant.Commands.Builders;
using FrenchExDev.Net.Vagrant.Testing;
using Shouldly;

namespace FrenchExDev.Net.Vagrant.Tests.Builders;

public class DestroyCommandBuilderTests : CommandBuilderTester<DestroyCommandBuilder, DestroyCommand>
{
    [Fact]
    public void Build_Default_NoFlagsSet()
    {
        Valid(
            builder => builder.WorkingDirectory("foo"),
            cmd =>
            {
                cmd.Force.ShouldBeNull();
                cmd.Graceful.ShouldBeNull();
            },
            args => { args.ShouldBe(new[] { "destroy" }.ToList()); }
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
            args => { args.ShouldBe(new[] { "destroy" }.ToList()); }
        );
    }

    [Fact]
    public void Build_WithGracefulTrue_IncludesGracefulFlag()
    {
        Valid(
            builder => builder.Graceful().WorkingDirectory("foo"),
            cmd => { cmd.Graceful.ShouldBe(true); },
            args => { args.ShouldContain("--graceful"); }
        );
    }

    [Fact]
    public void Build_WithBothFlags_IncludesBoth()
    {
        Valid(
            builder => builder.Force().Graceful().WorkingDirectory("foo"),
            cmd =>
            {
                cmd.Force.ShouldBe(true);
                cmd.Graceful.ShouldBe(true);
            },
            args =>
            {
                args.ShouldContain("-f");
                args.ShouldContain("--graceful");
            }
        );
    }
}
