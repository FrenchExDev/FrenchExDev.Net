#region Licensing

// Copyright Stéphane Erard
// For licensing, please contact stephane.erard@gmail

#endregion

using FrenchExDev.Net.CSharp.Object.Builder2;

namespace FrenchExDev.Net.Packer.Bundle;

public class FileBuilder : AbstractBuilder<File>
{
    protected List<string> _lines { get; } = new();
    protected string? _newLine;

    protected string? _name;
    protected string? _extension;
    protected string? _path;

    public FileBuilder AddLine(string value)
    {
        _lines.Add(value);
        return this;
    }

    public FileBuilder AddLines(string[] value)
    {
        _lines.AddRange(value);
        return this;
    }

    public FileBuilder SetNewLine(string newLine)
    {
        _newLine = newLine;
        return this;
    }

    public FileBuilder AddLines(string lines, string eol)
    {
        _lines.AddRange(lines.Split(eol));
        return this;
    }

    public FileBuilder AppendLine(string value)
    {
        return AddLine(value);
    }

    public FileBuilder AddLines(List<string> value)
    {
        _lines.AddRange(value);
        return this;
    }

    protected override File Instantiate()
    {
        return new File(_lines, _newLine ?? "\n", _name ?? string.Empty, _extension ?? string.Empty, _path ?? string.Empty);
    }
}

public class File : IFile
{
    public File(List<string> lines, string newLine, string name, string extension, string path)
    {
        Lines = lines;
        NewLine = newLine;
        Name = name;
        Extension = extension;
        Path = path;
    }

    public List<string> Lines { get; } = new();
    public string NewLine { get; private set; } = "\n";

    public string Name { get; private set; }
    public string Extension { get; private set; }
    public string Path { get; private set; }

    public File AddLine(string value)
    {
        Lines.Add(value);
        return this;
    }

    public File AddLines(string[] value)
    {
        Lines.AddRange(value);
        return this;
    }

    public File SetNewLine(string newLine)
    {
        NewLine = newLine;
        return this;
    }

    public File AddLines(string lines, string eol)
    {
        Lines.AddRange(lines.Split(eol));
        return this;
    }

    public File AppendLine(string value)
    {
        return AddLine(value);
    }

    public File AddLines(List<string> value)
    {
        Lines.AddRange(value);
        return this;
    }
}