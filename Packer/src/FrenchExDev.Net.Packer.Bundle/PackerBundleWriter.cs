#region Licensing

// Copyright Stéphane Erard
// For licensing, please contact stephane.erard@gmail

#endregion

#region Usings

using System.Text;

#endregion

namespace FrenchExDev.Net.Packer.Bundle;

public class PackerBundleWriter : IPackerBundleWriter
{
    public async Task WriteAsync(
        PackerBundle bundle,
        PackerBundleWritingContext context,
        CancellationToken cancellationToken = default
    )
    {
        var json = bundle.PackerFile.Serialize();

        await System.IO.File.WriteAllTextAsync(Path.Join(context.Path, "alpine.json"), json.ReplaceLineEndings("\r"),
            Encoding.ASCII, cancellationToken);

        Directory.CreateDirectory(Path.Join(context.Path, "http"));

        async Task WriteFileAsync(string path, List<string> data, string newLine, CancellationToken cT = default)
        {
            var fileInfo = new FileInfo(path);
            fileInfo.Directory?.Create();
            await using TextWriter writer = new StreamWriter(path);
            writer.NewLine = newLine;
            await writer.WriteAsync(new StringBuilder(string.Join("\n", data.Select(x => x.Replace("\n", "")))), cT);
        }

        foreach (var file in bundle.HttpDirectory.Files)
        {
            if (file.Value is not File fileCast) continue;
            var filePath = Path.Join(context.Path, "http", file.Key);
            await WriteFileAsync(filePath, fileCast.Lines, fileCast.NewLine, cancellationToken);
        }

        foreach (var dir in bundle.Directories) Directory.CreateDirectory(Path.Join(context.Path, dir));

        Directory.CreateDirectory(Path.Join(context.Path, "vagrant"));

        foreach (var file in bundle.VagrantDirectory.Files)
        {
            if (file.Value is not File fileCast) continue;
            var filePath = Path.Join(context.Path, "vagrant", file.Key);
            await WriteFileAsync(filePath, fileCast.Lines, fileCast.NewLine, cancellationToken);
        }

        foreach (var file in bundle.Scripts)
        {
            if (file.Value is not ShellScript shellScript) continue;

            var path = new List<string> { context.Path };
            path.AddRange(file.Key.Split("/"));
            var fileName = path.Last();
            path.Remove(fileName);

            var dirInfo = new DirectoryInfo(string.Join("/", path));
            if (!dirInfo.Exists) dirInfo.Create();

            var filePath = string.Join("/", dirInfo, fileName);
            await WriteFileAsync(filePath, shellScript.Lines, shellScript.NewLine, cancellationToken);
        }
    }
}