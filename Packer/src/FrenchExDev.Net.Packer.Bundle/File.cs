#region Licensing

// Copyright Stéphane Erard
// For licensing, please contact stephane.erard@gmail

#endregion

namespace FrenchExDev.Net.Packer.Bundle;

public class File : IFile
{
    public List<string> Lines { get; } = new();
    public string NewLine { get; private set; } = "\n";

    public string Name { get; init; }
    public string Extension { get; init; }
    public string Path { get; init; }

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