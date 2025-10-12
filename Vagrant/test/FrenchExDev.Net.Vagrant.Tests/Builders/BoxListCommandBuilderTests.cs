using FrenchExDev.Net.Vagrant.Commands;
using FrenchExDev.Net.Vagrant.Commands.Builders;
using FrenchExDev.Net.Vagrant.Testing;
using Shouldly;

namespace FrenchExDev.Net.Vagrant.Tests.Builders;

public class BoxListCommandBuilderTests : CommandBuilderTester<BoxListCommandBuilder, BoxListCommand>
{
    [Fact]
    public void Build_Default_DoesNotIncludeLocal()
    {
        Valid(
            builder => { builder.WorkingDirectory("foo"); },
            cmd => { cmd.Local.ShouldBe(null); },
            args => { args.ShouldBe(new[] { "box", "list" }.ToList()); }
        );
    }

    [Fact]
    public void Build_WithLocalTrue_IncludesLocalFlag()
    {
        Valid(
            builder => builder.Local().WorkingDirectory("foo"),
            cmd => { cmd.Local.ShouldBe(true); },
            args => { args.ShouldContain("--local"); }
        );
    }

    [Fact]
    public void Build_WithLocalFalse_DoesNotIncludeLocalFlag()
    {
        Valid(
            builder => builder.Local(false).WorkingDirectory("foo"),
            cmd => { cmd.Local.ShouldBe(false); },
            args => { args.ShouldBe(new[] { "box", "list" }.ToList()); }
        );
    }
}
