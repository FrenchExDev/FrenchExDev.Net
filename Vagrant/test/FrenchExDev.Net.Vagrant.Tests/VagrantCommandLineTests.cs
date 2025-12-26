using FrenchExDev.Net.Vagrant.Testing;

namespace FrenchExDev.Net.Vagrant.Tests;

public class VagrantCommandLineTests
{
    [Fact]
    public void ToCommandLine_GeneratesCorrectCommandLine()
    {
        var cmd = (new VagrantCommandTest<VagrantUpCommandBuilder, VagrantUpCommand>()).Valid(b => b.VmName("web").Provider("virtualbox").NoProvision());

        var commandLine = cmd.ToCommandLine();

        commandLine.ShouldStartWith("vagrant");
        commandLine.ShouldContain("up");
        commandLine.ShouldContain("--provider");
        commandLine.ShouldContain("virtualbox");
        commandLine.ShouldContain("--no-provision");
        commandLine.ShouldContain("web");
    }

    [Fact]
    public void ToCommandLine_QuotesArgumentsWithSpaces()
    {
        var cmd = (new VagrantCommandTest<VagrantSshCommandBuilder, VagrantSshCommand>()).Valid(b => b.Command("ls -la /home"));

        var commandLine = cmd.ToCommandLine();

        commandLine.ShouldContain("\"ls -la /home\"");
    }

    [Fact]
    public void ToCommandLine_QuotesEmptyString()
    {
        var cmd = new VagrantSshCommand { Command = "" };
        var commandLine = cmd.ToCommandLine();
        commandLine.ShouldContain("ssh");
    }

    [Fact]
    public void ToCommandLine_EscapesQuotesInArguments()
    {
        var cmd = new VagrantSshCommand { Command = "echo \"hello\"" };
        var commandLine = cmd.ToCommandLine();
        commandLine.ShouldContain("\\\"hello\\\"");
    }
}
