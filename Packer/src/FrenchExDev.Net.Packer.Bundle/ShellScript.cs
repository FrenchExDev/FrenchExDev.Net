#region Licensing

// Copyright Stéphane Erard
// For licensing, please contact stephane.erard@gmail

#endregion

namespace FrenchExDev.Net.Packer.Bundle;

public class ShellScript : IScript
{
    public required string Name { get; set; }
    public List<string> Lines { get; set; } = new();
    public string NewLine { get; set; } = "\n";
}