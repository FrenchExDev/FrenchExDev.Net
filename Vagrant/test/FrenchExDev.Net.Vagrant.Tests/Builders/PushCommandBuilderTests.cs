using System.Linq;
using FrenchExDev.Net.Vagrant.Commands;
using FrenchExDev.Net.Vagrant.Commands.Builders;
using FrenchExDev.Net.Vagrant.Testing;
using Shouldly;
using Xunit;

namespace FrenchExDev.Net.Vagrant.Tests.Builders;

public class PushCommandBuilderTests : CommandBuilderTester<PushCommandBuilder, PushCommand>
{
    [Fact]
    public void Build_MissingName_FailsValidation()
    {
        Invalid(
            builder => builder.WorkingDirectory("foo"),
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
            builder => builder.Name("target").WorkingDirectory("foo"),
            cmd => { cmd.Name.ShouldBe("target"); },
            args => { args.ShouldContain("target"); }
        );
    }
}
