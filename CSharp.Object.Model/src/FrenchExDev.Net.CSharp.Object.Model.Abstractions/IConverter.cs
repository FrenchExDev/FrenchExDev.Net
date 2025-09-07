namespace FrenchExDev.Net.CSharp.Object.Model.Abstractions;

public interface IConverter<TFrom, TTo>
{
    TTo Convert(TFrom from);
}

