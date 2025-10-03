using FrenchExDev.Net.Mm.Abstractions;
using Shouldly;

namespace FrenchExDev.Net.Mm.Abstractions.Tests;

/// <summary>
/// Unit tests for MajorMinorPatchModuleVersion and SemanticModuleVersion classes.
/// </summary>
public class VersionTests
{
    /// <summary>
    /// Tests the construction and ToString method of MajorMinorPatchModuleVersion.
    /// </summary>
    [Fact]
    public void MajorMinorPatchModuleVersion_ToString_ReturnsExpectedFormat()
    {
        var version = new MajorMinorPatchModuleVersion(1, 2, 3);
        version.Major.ShouldBe(1);
        version.Minor.ShouldBe(2);
        version.Patch.ShouldBe(3);
        version.ToString().ShouldBe("1.2.3");
    }

    /// <summary>
    /// Tests the comparison of two MajorMinorPatchModuleVersion.
    /// </summary>
    [Fact]
    public void MajorMinorPatchModuleVersion_Compare_Equals()
    {
        var version1 = new MajorMinorPatchModuleVersion(1, 2, 3);
        var version2 = new MajorMinorPatchModuleVersion(1, 2, 3);
        version1.ShouldBeEquivalentTo(version2);
    }

    /// <summary>
    /// Tests the construction and ToString method of SemanticModuleVersion with only major, minor, and patch.
    /// </summary>
    [Fact]
    public void SemanticModuleVersion_ToString_Basic()
    {
        var version = new SemanticModuleVersion(1, 0, 0);
        version.Major.ShouldBe(1);
        version.Minor.ShouldBe(0);
        version.Patch.ShouldBe(0);
        version.PreRelease.ShouldBe("");
        version.BuildMetadata.ShouldBe("");
        version.ToString().ShouldBe("1.0.0");
    }

    /// <summary>
    /// Tests the comparison of two SemanticModuleVersion.
    /// </summary>
    [Fact]
    public void SemanticModuleVersion_Compare_Equals()
    {
        var version1 = new SemanticModuleVersion(1, 0, 0, "alpha", "build.42");
        var version2 = new SemanticModuleVersion(1, 0, 0, "alpha", "build.42");
        version1.ShouldBeEquivalentTo(version2);
    }

    /// <summary>
    /// Tests the ToString method of SemanticModuleVersion with pre-release and build metadata.
    /// </summary>
    [Fact]
    public void SemanticModuleVersion_ToString_WithPreReleaseAndBuildMetadata()
    {
        var version = new SemanticModuleVersion(2, 1, 5, "beta", "build.42");
        version.ToString().ShouldBe("2.1.5-beta+build.42");
    }

    /// <summary>
    /// Tests the ToString method of SemanticModuleVersion with only pre-release.
    /// </summary>
    [Fact]
    public void SemanticModuleVersion_ToString_WithPreReleaseOnly()
    {
        var version = new SemanticModuleVersion(3, 0, 0, "rc1");
        version.ToString().ShouldBe("3.0.0-rc1");
    }

    /// <summary>
    /// Tests the ToString method of SemanticModuleVersion with only build metadata.
    /// </summary>
    [Fact]
    public void SemanticModuleVersion_ToString_WithBuildMetadataOnly()
    {
        var version = new SemanticModuleVersion(4, 2, 1, "", "exp.sha.5114f85");
        version.ToString().ShouldBe("4.2.1+exp.sha.5114f85");
    }
}
