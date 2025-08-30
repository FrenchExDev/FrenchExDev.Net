namespace FrenchExDev.Net.CSharp.Object.Builder;

public interface IAbstractStep<TClass>
{
    bool IsFinalStep { get; }
    Task<TClass> Result { get; }
}
