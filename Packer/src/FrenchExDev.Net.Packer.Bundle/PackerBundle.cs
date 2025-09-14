#region Licensing

// Copyright Stéphane Erard
// For licensing, please contact stephane.erard@gmail

#endregion

namespace FrenchExDev.Net.Packer.Bundle;

public class PackerBundle
{
    public required PackerFile PackerFile { get; set; }
    public required HttpDirectory HttpDirectory { get; set; }
    public required VagrantDirectory VagrantDirectory { get; set; }
    public required List<string> Directories { get; set; }
    public required Dictionary<string, IScript> Scripts { get; set; }
    public required List<string> Plugins { get; set; }
}