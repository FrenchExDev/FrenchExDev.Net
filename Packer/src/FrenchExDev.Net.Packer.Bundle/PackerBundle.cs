#region Licensing

// Copyright Stéphane Erard
// For licensing, please contact stephane.erard@gmail

#endregion

namespace FrenchExDev.Net.Packer.Bundle;

public class PackerBundle
{
    /// <summary>
    /// The main Packer file containing builder, provisioner, post-processor, and variable configurations.
    /// </summary>
    public required PackerFile PackerFile { get; set; }

    /// <summary>
    /// Directory containing HTTP files to be served during the build process.
    /// </summary>
    public required HttpDirectory HttpDirectory { get; set; }

    /// <summary>
    /// Directory containing Vagrant-specific files, such as Vagrantfile and metadata.
    /// </summary>
    public required VagrantDirectory VagrantDirectory { get; set; }

    /// <summary>
    /// List of additional directories to be included in the bundle output.
    /// </summary>
    public required DirectoryList Directories { get; set; }

    /// <summary>
    /// Dictionary of script names and their corresponding script objects to be executed during provisioning.
    /// </summary>
    public required ScriptDictionary Scripts { get; set; }

    /// <summary>
    /// List of plugin names required for the build process.
    /// </summary>
    public required PluginList Plugins { get; set; }
}

/// <summary>
/// Represents a collection of directory path strings.
/// </summary>
/// <remarks>This class inherits all functionality from <see cref="List{string}"/>, allowing standard list
/// operations on directory paths. It is intended for scenarios where a list of directory paths needs to be managed or
/// passed between components.</remarks>
public class DirectoryList : List<string>
{
    public DirectoryList()
    {
    }

    public DirectoryList(IEnumerable<string> collection) : base(collection)
    {
    }
}

/// <summary>
/// Represents a collection of named scripts, accessible by their string keys.
/// </summary>
/// <remarks>This class extends <see cref="Dictionary{string, IScript}"/>, allowing scripts to be stored and
/// retrieved by name. It is commonly used to organize and manage multiple script instances within an application. All
/// standard dictionary operations are available.</remarks>
public class ScriptDictionary : Dictionary<string, IScript>
{
    public ScriptDictionary()
    {
    }

    public ScriptDictionary(IDictionary<string, IScript> dictionary) : base(dictionary)
    {
    }
}

/// <summary>
/// Represents a collection of plugin names as strings.
/// </summary>
/// <remarks>Use this class to manage a list of plugins by their names. Inherits all standard list operations from
/// <see cref="List{T}"/>.</remarks>
public class PluginList : List<string>
{
    public PluginList()
    {
    }

    public PluginList(IEnumerable<string> collection) : base(collection)
    {
    }
}