#region Licensing

// Copyright Stéphane Erard
// For licensing, please contact stephane.erard@gmail

#endregion

namespace FrenchExDev.Net.Packer.Bundle;

public interface IFile
{
    string Name { get; }
    string Extension { get; }
    string Path { get; }
    string NewLine { get; }
}