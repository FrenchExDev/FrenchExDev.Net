namespace FrenchExDev.Net.Vagrant.Commands.Builders;

public abstract class BoxCommandBuilder<TBuilder, TCommand> : VagrantCommandBuilder<TBuilder, TCommand>
    where TBuilder : BoxCommandBuilder<TBuilder, TCommand>
    where TCommand : BoxCommand
{
}
