using FrenchExDev.Net.Vagrant.Commands;
using FrenchExDev.Net.Vagrant.Commands.Builders;
using FrenchExDev.Net.Vagrant.Testing;
using Shouldly;

namespace FrenchExDev.Net.Vagrant.Tests.Builders;

public class PluginInstallCommandBuilderTests : CommandBuilderTester<PluginInstallCommandBuilder, PluginInstallCommand>
{
    [Fact]
    public void Build_MissingName_FailsValidation()
    {
        Invalid(
            builder => builder.WorkingDirectory("foo"),
            failures =>
            {
                failures.Keys.ShouldContain("Parameters");
                var f = failures["Parameters"].First();
                f.Value.ShouldBeOfType<InvalidDataException>();
                ((InvalidDataException)f.Value).Message.ShouldContain("Missing required parameter 'name'");
            }
        );
    }

    [Fact]
    public void Build_WithName_SucceedsAndIncludesName()
    {
        Valid(
            builder => builder.Name("myplugin").WorkingDirectory("foo"),
            cmd => { cmd.Name.ShouldBe("myplugin"); },
            args => { args.ShouldContain("myplugin"); }
        );
    }

    [Fact]
    public void PluginVersion_Empty_FailsValidation()
    {
        Invalid(
            builder => builder.Name("myplugin").Version("").WorkingDirectory("foo"),
            failures =>
            {
                failures.Keys.ShouldContain("Options");
                var f = failures["Options"].First();
                f.Value.ShouldBeOfType<InvalidDataException>();
                ((InvalidDataException)f.Value).Message.ShouldContain("--plugin-version cannot be empty");
            }
        );
    }

    [Fact]
    public void Build_WithPluginVersion_IncludesOption()
    {
        Valid(
            builder => builder.Name("myplugin").Version("1.2.3").WorkingDirectory("foo"),
            cmd => { cmd.PluginVersion.ShouldBe("1.2.3"); },
            args =>
            {
                args.ShouldContain("--plugin-version");
                args.ShouldContain("1.2.3");
            }
        );
    }

    [Fact]
    public void Local_True_SetsLocal()
    {
        Valid(
            builder => builder.Name("myplugin").Local().WorkingDirectory("foo"),
            cmd => { cmd.Local.ShouldBe(true); },
            args => { args.ShouldContain("myplugin"); }
        );
    }
}
