using System;
using System.IO;
using System.Diagnostics;
using FrenchExDev.Net.CSharp.Object.Result;

namespace FrenchExDev.Net.CSharp.ProjectDependency.Abstractions;

public class GitCliRepository : IGitRepository
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

    private static string RunGit(string repoRoot, string args)
    {
        var psi = new ProcessStartInfo("git", args)
        {
            WorkingDirectory = repoRoot,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        };
        using var p = Process.Start(psi);
        if (p == null) return string.Empty;
        var outStr = p.StandardOutput.ReadToEnd();
        p.WaitForExit(30000);
        return outStr.Trim();
    }

    public Result<int> GetCommitCount(string repoRoot, string pathRelativeToRepoRoot)
    {
        if (string.IsNullOrWhiteSpace(repoRoot) || string.IsNullOrWhiteSpace(pathRelativeToRepoRoot)) return Result<int>.Success(0);
        try
        {
            // counts commits that touch the path
            var args = $"rev-list --count HEAD -- \"{pathRelativeToRepoRoot}\"";
            var res = RunGit(repoRoot, args);
            if (int.TryParse(res, out var n)) return Result<int>.Success(n);
            return Result<int>.Success(0);
        }
        catch (Exception ex) { return Result<int>.Failure(ex); }
    }

    public Result<DateTimeOffset> GetLastCommitDate(string repoRoot, string pathRelativeToRepoRoot)
    {
        if (string.IsNullOrWhiteSpace(repoRoot) || string.IsNullOrWhiteSpace(pathRelativeToRepoRoot)) return Result<DateTimeOffset>.Failure((d)=>d.Add("error","invalid args"));
        try
        {
            var args = $"log -1 --format=%cI -- \"{pathRelativeToRepoRoot}\"";
            var res = RunGit(repoRoot, args);
            if (DateTimeOffset.TryParse(res, out var dt)) return Result<DateTimeOffset>.Success(dt);
            return Result<DateTimeOffset>.Failure((d)=>d.Add("NoCommit","no commit found"));
        }
        catch (Exception ex) { return Result<DateTimeOffset>.Failure(ex); }
    }
}
