using System.Threading;
using System.Threading.Tasks;
using FrenchExDev.Net.Vagrant.Commands;
using FrenchExDev.Net.Vagrant.Commands.Invocation;
using Shouldly;
using Xunit.Abstractions;

namespace FrenchExDev.Net.Vagrant.Testing;

public sealed class SuccessCase : IXunitSerializable
{
    public string Id { get; private set; } = string.Empty;
    public string Command { get; private set; } = string.Empty;
    public string ParamSpec { get; private set; } = string.Empty;
    public string OptionSpec { get; private set; } = string.Empty;
    public string? EqualsExpectation { get; private set; }
    public string? ContainsCsv { get; private set; }

    public static SuccessCase Create(string id, string command, string paramSpec, string optionSpec, string? equalsExpectation = null, string? containsCsv = null)
        => new()
        {
            Id = id,
            Command = command,
            ParamSpec = paramSpec,
            OptionSpec = optionSpec,
            EqualsExpectation = equalsExpectation,
            ContainsCsv = containsCsv
        };
    public void Serialize(IXunitSerializationInfo info)
    {
        info.AddValue(nameof(Id), Id);
        info.AddValue(nameof(Command), Command);
        info.AddValue(nameof(ParamSpec), ParamSpec);
        info.AddValue(nameof(OptionSpec), OptionSpec);
        info.AddValue(nameof(EqualsExpectation), EqualsExpectation);
        info.AddValue(nameof(ContainsCsv), ContainsCsv);
    }
    public void Deserialize(IXunitSerializationInfo info)
    {
        Id = info.GetValue<string>(nameof(Id));
        Command = info.GetValue<string>(nameof(Command));
        ParamSpec = info.GetValue<string>(nameof(ParamSpec));
        OptionSpec = info.GetValue<string>(nameof(OptionSpec));
        EqualsExpectation = info.GetValue<string?>(nameof(EqualsExpectation));
        ContainsCsv = info.GetValue<string?>(nameof(ContainsCsv));
    }
    public override string ToString() => Id;
    public void AssertInvocation(Func<string, string, string, Invocation> builder)
    {
        var invocation = builder(Command, ParamSpec, OptionSpec);
        var args = VagrantInvocationBuilder.BuildArgs(invocation);
        args.ShouldNotBeNull();
        args.ShouldContain(Command);
        if (EqualsExpectation is not null) args.ShouldBe(EqualsExpectation);
        if (!string.IsNullOrWhiteSpace(ContainsCsv))
            foreach (var token in ContainsCsv.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
                args.ShouldContain(token);
    }
}

public sealed class FailureCase : IXunitSerializable
{
    public string Id { get; private set; } = string.Empty;
    public string Command { get; private set; } = string.Empty;
    public string ParamSpec { get; private set; } = string.Empty;
    public string OptionSpec { get; private set; } = string.Empty;
    public string ExpectedMessagePart { get; private set; } = "Missing required parameter";

    public static FailureCase Create(string id, string command, string paramSpec, string optionSpec, string? expected = null)
        => new()
        {
            Id = id,
            Command = command,
            ParamSpec = paramSpec,
            OptionSpec = optionSpec,
            ExpectedMessagePart = expected ?? "Missing required parameter"
        };
    public void Serialize(IXunitSerializationInfo info)
    {
        info.AddValue(nameof(Id), Id);
        info.AddValue(nameof(Command), Command);
        info.AddValue(nameof(ParamSpec), ParamSpec);
        info.AddValue(nameof(OptionSpec), OptionSpec);
        info.AddValue(nameof(ExpectedMessagePart), ExpectedMessagePart);
    }
    public void Deserialize(IXunitSerializationInfo info)
    {
        Id = info.GetValue<string>(nameof(Id));
        Command = info.GetValue<string>(nameof(Command));
        ParamSpec = info.GetValue<string>(nameof(ParamSpec));
        OptionSpec = info.GetValue<string>(nameof(OptionSpec));
        ExpectedMessagePart = info.GetValue<string>(nameof(ExpectedMessagePart));
    }
    public override string ToString() => Id;
    public void AssertInvocation(Func<string, string, string, Invocation> builder)
    {
        var invocation = builder(Command, ParamSpec, OptionSpec);
        var ex = Should.Throw<Exception>(() => VagrantInvocationBuilder.BuildArgs(invocation));
        ex.Message.ShouldContain(ExpectedMessagePart);
    }
}
