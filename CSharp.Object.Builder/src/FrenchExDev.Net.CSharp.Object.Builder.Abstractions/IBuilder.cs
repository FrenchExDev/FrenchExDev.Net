namespace FrenchExDev.Net.CSharp.Object.Builder.Abstractions;

public interface IBuilder<TClass>
{
    IBuildResult<TClass> Build();
}
