using FrenchExDev.Net.Vagrant.Commands;
using FrenchExDev.Net.Vagrant.Commands.Builders;
using FrenchExDev.Net.Vagrant.Testing;
using Shouldly;

namespace FrenchExDev.Net.Vagrant.Tests.Builders;

public class PluginUninstallCommandBuilderTests : CommandBuilderTester<PluginUninstallCommandBuilder, PluginUninstallCommand>
{
    [Fact]
    public void Build_MissingName_FailsValidation()
    {
        Invalid(
            builder => builder.WorkingDirectory("foo"),
            failures =>
            {
                failures.Keys.ShouldContain(nameof(PluginUninstallCommand.Name));
                var f = failures[nameof(PluginUninstallCommand.Name)].First();
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
}
