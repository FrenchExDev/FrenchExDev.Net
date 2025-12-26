namespace FrenchExDev.Net.Vagrant.Tests;

public class VagrantCloudAuthCommandBuilderTests : VagrantCommandTest<VagrantCloudAuthCommandBuilder, VagrantCloudAuthCommand>
{
    [Fact]
    public void Valid_WithRequiredSubCommand_BuildsSuccessfully()
    {
        var cmd = Valid(b => b.SubCommand("login"));
        cmd.SubCommand.ShouldBe("login");
    }

    [Theory]
    [InlineData("login")]
    [InlineData("logout")]
    [InlineData("whoami")]
    public void Valid_WithDifferentSubCommands_BuildsSuccessfully(string subCommand)
    {
        var cmd = Valid(b => b.SubCommand(subCommand));
        cmd.SubCommand.ShouldBe(subCommand);
    }

    [Fact]
    public void Invalid_WithoutSubCommand_Fails()
    {
        Invalid(b => { });
    }

    [Fact]
    public void Invalid_WithEmptySubCommand_Fails()
    {
        Invalid(b => b.SubCommand(""));
    }

    [Fact]
    public void Invalid_WithWhitespaceSubCommand_Fails()
    {
        Invalid(b => b.SubCommand("   "));
    }

    [Fact]
    public void ToArguments_GeneratesCorrectArguments()
    {
        var cmd = Valid(b => b.SubCommand("login"));

        var args = cmd.ToArguments();
        args.ShouldContain("cloud");
        args.ShouldContain("auth");
        args.ShouldContain("login");
    }
}
