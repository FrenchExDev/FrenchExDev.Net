#region Licensing

// Copyright Stéphâne Erard
// For licensing please contact <stephane.erard@gmail.com>

#endregion

#region Usings

#endregion

namespace FrenchExDev.Net.Alpine.Version;

/// <summary>
/// Represents a collection of <see cref="AlpineVersionArchFlavorRecord"/> objects.
/// </summary>
/// <remarks>This class extends <see cref="List{T}"/> to provide a strongly-typed list specifically for  <see
/// cref="AlpineVersionArchFlavorRecord"/> instances. It can be used to manage and manipulate  records of Alpine version
/// architecture and flavor combinations.</remarks>
public class AlpineVersionList : List<AlpineVersionArchFlavorRecord>
{
    public AlpineVersionList()
    {
    }

    public AlpineVersionList(IEnumerable<AlpineVersionArchFlavorRecord> collection) : base(collection)
    {
    }
}