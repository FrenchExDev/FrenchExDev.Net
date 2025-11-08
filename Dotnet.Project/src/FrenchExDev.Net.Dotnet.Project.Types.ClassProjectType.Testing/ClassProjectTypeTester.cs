using FrenchExDev.Net.CSharp.Object.Builder2;
using FrenchExDev.Net.Dotnet.Project.Types.ClassProjectType.Abstractions;

namespace FrenchExDev.Net.Dotnet.Project.Types.ClassProjectType.Testing;

/// <summary>
/// Provides utility methods for testing <see cref="ClassProjectModelBuilder"/> and validating the resulting <see cref="ClassProjectModel"/>.
/// </summary>
/// <remarks>
/// Use this class to assert that your builder produces valid or invalid project models in unit tests.
/// Example usage:
/// <code>
/// ClassProjectTypeTester.Valid(
///     builder => builder.Name("MyClassProject").Directory("src/MyClassProject"),
///     model => Assert.Equal("MyClassProject", model.Name)
/// );
/// ClassProjectTypeTester.Invalid(
///     builder => builder.Name(null),
///     result => Assert.False(result.IsSuccess)
/// );
/// </code>
/// </remarks>
public class ClassProjectTypeTester
{
    /// <summary>
    /// Asserts that the builder produces a valid <see cref="ClassProjectModel"/> and allows custom assertions on the built model.
    /// </summary>
    /// <param name="body">An action to configure the builder.</param>
    /// <param name="assertBuiltModel">An action to assert properties of the built model.</param>
    /// <remarks>
    /// Use this method in unit tests to verify that a builder configuration results in a valid project model.
    /// </remarks>
    public static void Valid(
        Action<ClassProjectModelBuilder> body,
        Action<ClassProjectModel> assertBuiltModel
        )
    {
        var builder = new ClassProjectModelBuilder();
        body(builder);
        var result = builder.Build().Success<ClassProjectModel>();
        assertBuiltModel(result);
    }

    /// <summary>
    /// Asserts that the builder produces an invalid <see cref="ClassProjectModel"/> and allows custom assertions on the build result.
    /// </summary>
    /// <param name="body">An action to configure the builder.</param>
    /// <param name="assertBuildResult">An action to assert the build result (expected to be invalid).</param>
    /// <remarks>
    /// Use this method in unit tests to verify that a builder configuration fails validation as expected.
    /// </remarks>
    public static void Invalid(
        Action<ClassProjectModelBuilder> body,
        Action<FailuresDictionary> assertBuildResult
        )
    {
        var builder = new ClassProjectModelBuilder();
        body(builder);
        var result = builder.Build().Failures();
        assertBuildResult(result);
    }
}
