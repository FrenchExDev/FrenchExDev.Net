using System.Formats.Asn1;

namespace FrenchExDev.Net.CSharp.Object.Builder.Abstractions;

public class FailureObjectBuildResultException<TClass, TBuilder> : Exception where TBuilder : IObjectBuilder<TClass>
{
    public FailureObjectBuildResultException(FailureObjectBuildResult<TClass, TBuilder> failure, string message) : base(message)
    {
    }
}