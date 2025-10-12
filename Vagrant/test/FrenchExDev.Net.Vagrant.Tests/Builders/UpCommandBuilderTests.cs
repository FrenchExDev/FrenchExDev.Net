using FrenchExDev.Net.Vagrant.Commands;
using FrenchExDev.Net.Vagrant.Commands.Builders;
using FrenchExDev.Net.Vagrant.Testing;
using Shouldly;

namespace FrenchExDev.Net.Vagrant.Tests.Builders;

public class UpCommandBuilderTests : CommandBuilderTester<UpCommandBuilder, UpCommand>
{
    [Fact]
    public void Build_Default_NoFlags()
    {
        Valid(
            builder => builder.WorkingDirectory("foo"),
            cmd => { cmd.Provision.ShouldBeNull(); cmd.Provider.ShouldBeNull(); },
            args => { args.ShouldBe(new[] { "up" }.ToList()); }
        );
    }

    [Fact]
    public void Build_WithProvision_IncludesProvision()
    {
        Valid(
            builder => builder.Provision().WorkingDirectory("foo"),
            cmd => { cmd.Provision.ShouldBe(true); },
            args => { args.ShouldContain("--provision"); }
        );
    }

    [Fact]
    public void Build_WithNoProvision_IncludesNoProvision()
    {
        Valid(
            builder => builder.Provision(false).WorkingDirectory("foo"),
            cmd => { cmd.Provision.ShouldBe(false); },
            args => { args.ShouldContain("--no-provision"); }
        );
    }


    [Fact]
    public void Provider_Empty_FailsValidation()
    {
        Invalid(
            builder => builder.Provider("").WorkingDirectory("foo"),
            failures =>
            {
                failures.Keys.ShouldContain(nameof(UpCommand.Provider));
                var f = failures[nameof(UpCommand.Provider)].First();
                f.Value.ShouldBeOfType<InvalidDataException>();
                ((InvalidDataException)f.Value).Message.ShouldContain("--provider cannot be empty");
            }
        );
    }

    [Fact]
    public void Provider_Set_IncludesProviderArgument()
    {
        Valid(
            builder => builder.Provider("virtualbox").WorkingDirectory("foo"),
            cmd => { cmd.Provider.ShouldBe("virtualbox"); },
            args => { args.ShouldContain("--provider"); args.ShouldContain("virtualbox"); }
        );
    }

    [Fact]
    public void Parallel_And_DestroyOnError_Set_ReflectInCommand()
    {
        Valid(
            builder => builder.Parallel().DestroyOnError().WorkingDirectory("foo"),
            cmd => { cmd.Parallel.ShouldBe(true); cmd.DestroyOnError.ShouldBe(true); },
            args => { args.ShouldBe(new[] { "up" }.ToList()); }
        );
    }

    [Fact]
    public void Build_WithoutWorkingDirectory_FailsValidation()
    {
        Invalid(
            builder => { },
            failures =>
            {
                failures.Keys.ShouldContain(nameof(UpCommandBuilder.WorkingDirectory));
                var f = failures[nameof(UpCommandBuilder.WorkingDirectory)].First();
                f.Value.ShouldBeOfType<InvalidDataException>();
                ((InvalidDataException)f.Value).Message.ShouldContain("Working directory must be set");
            }
        );
    }
}
