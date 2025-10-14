using FrenchExDev.Net.CSharp.Object.Builder2;
using FrenchExDev.Net.Vagrant.Commands;
using FrenchExDev.Net.Vagrant.Commands.Builders;

namespace FrenchExDev.Net.Vagrant.Testing;

public abstract class CommandBuilderTester<TBuilder, TCommand>
    where TCommand : VagrantCommandBase
    where TBuilder : VagrantCommandBuilder<TBuilder, TCommand>, new()
{
    protected void Valid(Action<TBuilder> body, Action<TCommand> assertCommand, Action<List<string>> assertToString)
    {
        var builder = new TBuilder();
        body(builder);
        var built = builder.BuildSuccess();
        assertCommand(built);
        var args = built.ToArguments().ToList();
        assertToString(args);
    }

    protected void Invalid(Action<TBuilder> body, Action<FailuresDictionary> assertFailures)
    {
        var builder = new TBuilder();
        body(builder);
        var failures = builder.Build() as FailureResult;
        ArgumentNullException.ThrowIfNull(failures);
        assertFailures(failures.Failures);
    }
}
