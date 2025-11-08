using FrenchExDev.Net.Dotnet.Project.Types.ClassProjectType.Testing;
using Shouldly;

namespace FrenchExDev.Net.Dotnet.Project.Types.ClassProjectType.Tests;

/// <summary>
/// Provides unit tests for validating the behavior of <see cref="ClassProjectModelBuilder"/> and <see cref="ClassProjectModel"/>.
/// </summary>
/// <remarks>
/// Use this class to ensure that your class project builder produces valid and invalid models as expected.
/// Example usage:
/// <code>
/// // Valid configuration test
/// ClassProjectTypeTester.Valid(
///     builder => builder.Name("MyClassProject").Directory("src/MyClassProject"),
///     model => Assert.Equal("MyClassProject", model.Name)
/// );
/// // Invalid configuration test
/// ClassProjectTypeTester.Invalid(
///     builder => builder.Name(null),
///     result => Assert.False(result.IsSuccess)
/// );
/// </code>
/// </remarks>
public class ClassProjectTypeTests
{
    /// <summary>
    /// Tests that building a project with an invalid configuration fails as expected.
    /// </summary>
    /// <remarks>
    /// This test verifies that the builder does not produce a valid model when required properties are missing or invalid.
    /// </remarks>
    [Fact]
    public void Cannot_Build_Invalid_Projec()
    {
        ClassProjectTypeTester.Invalid(body: (b) => { }, assertBuildResult: (r) => { });
    }

    /// <summary>
    /// Tests that building a project with a valid configuration succeeds and produces the expected model.
    /// </summary>
    /// <remarks>
    /// This test verifies that all required properties are set and the resulting model matches the expected configuration.
    /// </remarks>
    [Fact]
    public void Can_Build_Project_With_Valid_Configuration()
    {
        ClassProjectTypeTester.Valid(body: (b) => ClassProjectType.Default(b)
                .Name("MyClassProject")
                .Directory("src/MyClassProject")
                .LangVersion("13.0")
                .TargetFramework("net9.0")
                .Sdk("Microsoft.NET.Sdk")
                .OutputType("Library")
                .Nullable()
                .ImplicitUsings()
                .Version("1.0.0")
                .GeneratePackageOnBuild()
                .Authors("FrenchExDev Stéphane Erard")
                .PackageTags(["tag1", "tag2", "tag3"])
        ,
        assertBuiltModel: (classProjectModel) =>
        {
            classProjectModel.Name.ShouldBe("MyClassProject");
            classProjectModel.Directory.ShouldBe("src/MyClassProject");
            classProjectModel.LangVersion.ShouldBe("13.0");
            classProjectModel.TargetFramework.ShouldBe("net9.0");
            classProjectModel.Sdk.ShouldBe("Microsoft.NET.Sdk");
            classProjectModel.OutputType.ShouldBe("Library");

            (true == classProjectModel.Nullable).ShouldBeTrue();
            (true == classProjectModel.ImplicitUsings).ShouldBeTrue();

            classProjectModel.ProjectReferences.ShouldBeEmpty();
            classProjectModel.PackageReferences.ShouldBeEmpty();
            classProjectModel.Analyzers.ShouldBeEmpty();
            classProjectModel.DeclarationModels.ShouldBeEmpty();
            classProjectModel.AdditionalProperties.ShouldBeEmpty();
        });
    }
}
