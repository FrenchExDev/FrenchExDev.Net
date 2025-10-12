using FrenchExDev.Net.Vagrant.Commands;
using FrenchExDev.Net.Vagrant.Commands.Builders;
using FrenchExDev.Net.Vagrant.Testing;
using Shouldly;

namespace FrenchExDev.Net.Vagrant.Tests.Builders;

public class PluginUpdateCommandBuilderTests : CommandBuilderTester<PluginUpdateCommandBuilder, PluginUpdateCommand>
{
    [Fact]
    public void Build_MissingName_FailsValidation()
    {
        Invalid(
            builder => builder.WorkingDirectory("foo"),
            failures =>
            {
                // builder does not set a name -> validation should report missing name under key "_name"
                failures.Keys.ShouldContain("_name");
                var f = failures["_name"].First();
                f.Value.ShouldBeOfType<InvalidDataException>();
                ((InvalidDataException)f.Value).Message.ShouldContain("Missing required parameter 'name'");
            }
        );
    }

    [Fact]
    public void Build_EmptyName_FailsValidation()
    {
        Invalid(
            builder => builder.Name("").WorkingDirectory("foo"),
            failures =>
            {
                failures.Keys.ShouldContain("_name");
                var f = failures["_name"].First();
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
            args =>
            {
                args.ShouldContain("plugin");
                args.ShouldContain("update");
                args.ShouldContain("myplugin");
            }
        );
    }
}
