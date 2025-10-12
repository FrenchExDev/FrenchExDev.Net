using FrenchExDev.Net.Vagrant.Commands;
using FrenchExDev.Net.Vagrant.Commands.Builders;
using FrenchExDev.Net.Vagrant.Testing;
using Shouldly;

namespace FrenchExDev.Net.Vagrant.Tests.Builders;

public class PackageCommandBuilderTests : CommandBuilderTester<PackageCommandBuilder, PackageCommand>
{
    [Fact]
    public void Build_Default_NoOptions()
    {
        Valid(
            builder => builder.WorkingDirectory("foo"),
            cmd =>
            {
                cmd.Output.ShouldBeNull();
                cmd.Include.ShouldBeNull();
                cmd.Base.ShouldBeNull();
            },
            args => { args.ShouldBe(new[] { "package" }.ToList()); }
        );
    }

    [Fact]
    public void Build_WithOutput_IncludesOutputOption()
    {
        Valid(
            builder => builder.Output("out.box").WorkingDirectory("foo"),
            cmd => { cmd.Output.ShouldBe("out.box"); },
            args =>
            {
                args.ShouldContain("--output");
                args.ShouldContain("out.box");
            }
        );
    }

    [Fact]
    public void Build_WithInclude_IncludesFiles()
    {
        Valid(
            builder => builder.Include("a.txt").Include("b.log").WorkingDirectory("foo"),
            cmd =>
            {
                cmd.Include.ShouldNotBeNull();
                cmd.Include.Count.ShouldBe(2);
            },
            args =>
            {
                args.ShouldContain("--include");
                args.ShouldContain("a.txt");
                args.ShouldContain("b.log");
            }
        );
    }

    [Fact]
    public void Build_WithBase_IncludesBaseOption()
    {
        Valid(
            builder => builder.Base("virtualmachine").WorkingDirectory("foo"),
            cmd => { cmd.Base.ShouldBe("virtualmachine"); },
            args =>
            {
                args.ShouldContain("--base");
                args.ShouldContain("virtualmachine");
            }
        );
    }

    [Fact]
    public void Build_WithAllOptions_IncludesAll()
    {
        Valid(
            builder => builder.Output("out.box").Include("a.txt").Base("vm").WorkingDirectory("foo"),
            cmd =>
            {
                cmd.Output.ShouldBe("out.box");
                cmd.Include.ShouldNotBeNull();
                cmd.Base.ShouldBe("vm");
            },
            args =>
            {
                args.ShouldContain("--output"); args.ShouldContain("out.box");
                args.ShouldContain("--include"); args.ShouldContain("a.txt");
                args.ShouldContain("--base"); args.ShouldContain("vm");
            }
        );
    }
}
