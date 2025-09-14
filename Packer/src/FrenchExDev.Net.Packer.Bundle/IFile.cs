#region Licensing

// Copyright Stéphane Erard
// For licensing, please contact stephane.erard@gmail

#endregion

namespace FrenchExDev.Net.Packer.Bundle;

public interface IFile
{
    string Name { get; init; }
    string Extension { get; init; }
    string Path { get; init; }
    string NewLine { get; }
    File SetNewLine(string newLine);
    File AddLine(string value);
    File AddLines(string[] value);
}