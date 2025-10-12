using FrenchExDev.Net.Vagrant.Commands;
using FrenchExDev.Net.Vagrant.Commands.Builders;
using FrenchExDev.Net.Vagrant.Testing;
using Shouldly;

namespace FrenchExDev.Net.Vagrant.Tests.Builders;

public class ProvisionCommandBuilderTests : CommandBuilderTester<ProvisionCommandBuilder, ProvisionCommand>
{
    [Fact]
    public void Build_Default_NoNameOrProvisionWith()
    {
        Valid(
            builder => builder.WorkingDirectory("foo"),
            cmd => { cmd.Name.ShouldBeNull(); cmd.ProvisionWith.ShouldBeNull(); },
            args => { args.ShouldBe(new[] { "provision" }.ToList()); }
        );
    }

    [Fact]
    public void Build_WithName_IncludesName()
    {
        Valid(
            builder => builder.WithName("default").WorkingDirectory("foo"),
            cmd => { cmd.Name.ShouldBe("default"); },
            args => { args.ShouldContain("default"); }
        );
    }

    [Fact]
    public void Build_WithProvisionWith_IncludesMultipleOptions()
    {
        Valid(
            builder => builder.ProvisionWith("ansible").ProvisionWith("shell").WorkingDirectory("foo"),
            cmd =>
            {
                cmd.ProvisionWith.ShouldNotBeNull();
                cmd.ProvisionWith.Count.ShouldBe(2);
            },
            args =>
            {
                args.ShouldContain("--provision-with");
                args.ShouldContain("ansible");
                args.ShouldContain("shell");
            }
        );
    }

    [Fact]
    public void ProvisionWith_Empty_FailsValidation()
    {
        Invalid(
            builder => builder.ProvisionWith("").WorkingDirectory("foo"),
            failures =>
            {
                failures.Keys.ShouldContain("_provisionWith");
                var f = failures["_provisionWith"].First();
                f.Value.ShouldBeOfType<InvalidDataException>();
                ((InvalidDataException)f.Value).Message.ShouldContain("--provision-with cannot be empty");
            }
        );
    }

    [Fact]
    public void Build_WithNameAndProvisionWith_IncludesAll()
    {
        Valid(
            builder => builder.WithName("default").ProvisionWith("ansible").WorkingDirectory("foo"),
            cmd =>
            {
                cmd.Name.ShouldBe("default");
                cmd.ProvisionWith.ShouldNotBeNull();
            },
            args =>
            {
                args.ShouldContain("default");
                args.ShouldContain("--provision-with");
                args.ShouldContain("ansible");
            }
        );
    }
}
