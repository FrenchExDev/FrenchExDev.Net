using FrenchExDev.Net.Vagrant.Commands;
using FrenchExDev.Net.Vagrant.Commands.Builders;
using FrenchExDev.Net.Vagrant.Testing;
using Shouldly;

namespace FrenchExDev.Net.Vagrant.Tests.Builders;

public class ShareCommandBuilderTests : CommandBuilderTester<ShareCommandBuilder, ShareCommand>
{
    [Fact]
    public void Build_Default_NoService()
    {
        Valid(
            builder => builder.WorkingDirectory("foo"),
            cmd => { cmd.Service.ShouldBeNull(); },
            args => { args.ShouldBe(new[] { "share" }.ToList()); }
        );
    }

    [Fact]
    public void Build_WithService_IncludesService()
    {
        Valid(
            builder => builder.Service("myservice").WorkingDirectory("foo"),
            cmd => { cmd.Service.ShouldBe("myservice"); },
            args => { args.ShouldContain("myservice"); }
        );
    }

    [Fact]
    public void Service_Empty_FailsValidation()
    {
        Invalid(
            builder => builder.Service("").WorkingDirectory("foo"),
            failures =>
            {
                failures.Keys.ShouldContain(nameof(ShareCommand.Service));
                var f = failures[nameof(ShareCommand.Service)].First();
                f.Value.ShouldBeOfType<InvalidDataException>();
                ((InvalidDataException)f.Value).Message.ShouldContain("--service cannot be empty");
            }
        );
    }
}
