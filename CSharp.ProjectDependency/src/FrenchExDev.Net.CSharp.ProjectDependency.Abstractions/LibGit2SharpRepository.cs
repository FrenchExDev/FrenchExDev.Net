using System;
using System.IO;
using System.Linq;
using LibGit2Sharp;
using FrenchExDev.Net.CSharp.Object.Result;

namespace FrenchExDev.Net.CSharp.ProjectDependency.Abstractions;

/// <summary>
/// LibGit2Sharp-based git repository access.
/// </summary>
public class LibGit2SharpRepository : IGitRepository
{
    public string? GetRepositoryRoot(string anyPathInsideRepo)
    {
        if (string.IsNullOrWhiteSpace(anyPathInsideRepo)) return null;
        var dir = new DirectoryInfo(anyPathInsideRepo);
        while (dir != null)
        {
            if (Directory.Exists(Path.Combine(dir.FullName, ".git"))) return dir.FullName;
            dir = dir.Parent;
        }
        return null;
    }

    public Result<int> GetCommitCount(string repoRoot, string pathRelativeToRepoRoot)
    {
        if (string.IsNullOrWhiteSpace(repoRoot) || string.IsNullOrWhiteSpace(pathRelativeToRepoRoot)) return Result<int>.Success(0);

        try
        {
            using var repo = new Repository(repoRoot);

            // Ensure path uses forward slashes as in git trees
            var normalized = pathRelativeToRepoRoot.Replace('\\', '/').TrimStart('/');

            int count = 0;

            var filter = new CommitFilter { IncludeReachableFrom = repo.Head };
            foreach (var commit in repo.Commits.QueryBy(filter))
            {
                var c = commit; // Commit

                // If commit has parents, compare to each parent; otherwise compare to empty tree
                if (c.Parents.Any())
                {
                    foreach (var parent in c.Parents)
                    {
                        var changes = repo.Diff.Compare<TreeChanges>(parent.Tree, c.Tree);
                        foreach (var change in changes)
                        {
                            if (!string.IsNullOrEmpty(change.Path) && change.Path.StartsWith(normalized, StringComparison.OrdinalIgnoreCase))
                            {
                                count++;
                                goto NextCommit;
                            }
                        }
                    }
                }
                else
                {
                    // root commit: check if the tree contains the path
                    var entry = c.Tree[normalized];
                    if (entry != null)
                    {
                        count++;
                    }
                }

            NextCommit: ;
            }

            return Result<int>.Success(count);
        }
        catch (Exception ex)
        {
            return Result<int>.Failure(ex);
        }
    }

    public Result<DateTimeOffset> GetLastCommitDate(string repoRoot, string pathRelativeToRepoRoot)
    {
        if (string.IsNullOrWhiteSpace(repoRoot) || string.IsNullOrWhiteSpace(pathRelativeToRepoRoot)) return Result<DateTimeOffset>.Failure((d)=>d.Add("error","invalid args"));

        try
        {
            using var repo = new Repository(repoRoot);
            var normalized = pathRelativeToRepoRoot.Replace('\\', '/').TrimStart('/');

            var filter = new CommitFilter { IncludeReachableFrom = repo.Head };
            foreach (var commit in repo.Commits.QueryBy(filter))
            {
                var c = commit; // Commit

                var entry = c.Tree[normalized];
                if (entry != null)
                {
                    return Result<DateTimeOffset>.Success(c.Author.When);
                }

                // otherwise check diffs against parents
                foreach (var parent in c.Parents)
                {
                    var changes = repo.Diff.Compare<TreeChanges>(parent.Tree, c.Tree);
                    foreach (var change in changes)
                    {
                        if (!string.IsNullOrEmpty(change.Path) && change.Path.StartsWith(normalized, StringComparison.OrdinalIgnoreCase))
                        {
                            return Result<DateTimeOffset>.Success(c.Author.When);
                        }
                    }
                }
            }

            return Result<DateTimeOffset>.Failure((d)=>d.Add("NoCommit","no commit found"));
        }
        catch (Exception ex)
        {
            return Result<DateTimeOffset>.Failure(ex);
        }
    }
}
