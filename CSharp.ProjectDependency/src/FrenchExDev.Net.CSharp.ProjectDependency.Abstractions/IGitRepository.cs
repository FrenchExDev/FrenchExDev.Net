using FrenchExDev.Net.CSharp.Object.Result;

namespace FrenchExDev.Net.CSharp.ProjectDependency.Abstractions;

public interface IGitRepository
{
    /// <summary>
    /// Given a file path inside a git working tree, returns the repository root path or null if none found.
    /// </summary>
    string? GetRepositoryRoot(string anyPathInsideRepo);

    /// <summary>
    /// Returns number of commits touching the specified path (relative or absolute) within the repository.
    /// </summary>
    Result<int> GetCommitCount(string repoRoot, string pathRelativeToRepoRoot);

    /// <summary>
    /// Returns the date of the last commit that touched the specified path, or null if none.
    /// </summary>
    Result<DateTimeOffset> GetLastCommitDate(string repoRoot, string pathRelativeToRepoRoot);
}
