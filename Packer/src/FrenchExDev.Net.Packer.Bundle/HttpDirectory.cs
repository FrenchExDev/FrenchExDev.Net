#region Licensing

// Copyright Stéphane Erard
// For licensing, please contact stephane.erard@gmail

#endregion

namespace FrenchExDev.Net.Packer.Bundle;

public class HttpDirectory
{
    public Dictionary<string, IFile> Files { get; set; } = new();
}