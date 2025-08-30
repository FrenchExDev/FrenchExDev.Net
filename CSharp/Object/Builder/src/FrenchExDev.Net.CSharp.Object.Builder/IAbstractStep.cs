namespace FrenchExDev.Net.CSharp.Object.Builder;

public interface IAbstractStep<TClass>
{
    bool HasResult();
    TClass Result();
}
