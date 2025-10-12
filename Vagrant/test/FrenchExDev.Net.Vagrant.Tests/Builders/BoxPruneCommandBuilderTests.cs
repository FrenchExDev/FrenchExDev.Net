using FrenchExDev.Net.Vagrant.Commands;
using FrenchExDev.Net.Vagrant.Commands.Builders;
using FrenchExDev.Net.Vagrant.Testing;
using Shouldly;

namespace FrenchExDev.Net.Vagrant.Tests.Builders;

public class BoxPruneCommandBuilderTests : CommandBuilderTester<BoxPruneCommandBuilder, BoxPruneCommand>
{
    [Fact]
    public void Build_Default_HasNoFlagsSet()
    {
        Valid(
            builder => { builder.WorkingDirectory("foo"); },
            cmd =>
            {
                cmd.DryRun.ShouldBeNull();
                cmd.KeepActiveProvider.ShouldBeNull();
                cmd.Force.ShouldBeNull();
            },
            args => { args.ShouldBe(new[] { "box", "prune" }.ToList()); }
        );
    }

    [Fact]
    public void Build_WithDryRunTrue_SetsDryRun()
    {
        Valid(
            builder => builder.DryRun().WorkingDirectory("foo"),
            cmd => { cmd.DryRun.ShouldBe(true); },
            args => { args.ShouldBe(new[] { "box", "prune" }.ToList()); }
        );
    }

    [Fact]
    public void Build_WithDryRunFalse_SetsDryRunFalse()
    {
        Valid(
            builder => builder.DryRun(false).WorkingDirectory("foo"),
            cmd => { cmd.DryRun.ShouldBe(false); },
            args => { args.ShouldBe(new[] { "box", "prune" }.ToList()); }
        );
    }

    [Fact]
    public void Build_WithKeepActiveProviderTrue_SetsFlag()
    {
        Valid(
            builder => builder.KeepActiveProvider().WorkingDirectory("foo"),
            cmd => { cmd.KeepActiveProvider.ShouldBe(true); },
            args => { args.ShouldBe(new[] { "box", "prune" }.ToList()); }
        );
    }

    [Fact]
    public void Build_WithForceTrue_SetsFlag()
    {
        Valid(
            builder => builder.Force().WorkingDirectory("foo"),
            cmd => { cmd.Force.ShouldBe(true); },
            args => { args.ShouldBe(new[] { "box", "prune" }.ToList()); }
        );
    }

    [Fact]
    public void Build_WithAllFlags_SetsAllProperties()
    {
        Valid(
            builder => builder.DryRun().KeepActiveProvider().Force().WorkingDirectory("foo"),
            cmd =>
            {
                cmd.DryRun.ShouldBe(true);
                cmd.KeepActiveProvider.ShouldBe(true);
                cmd.Force.ShouldBe(true);
            },
            args => { args.ShouldBe(new[] { "box", "prune" }.ToList()); }
        );
    }
}
