#region Licensing

// Copyright Stéphane Erard
// All rights reserved

#endregion

namespace FrenchexDev.VirtualBox.Net;

/// <summary>
/// Represents filters used to search for VirtualBox version information, specifying criteria such as an exact version
/// match.
/// </summary>
/// <param name="ExactVersion">The exact VirtualBox version to match during the search. If specified, only results matching this version will be
/// included.</param>
public sealed record VirtualBoxVersionInformationSearchingFilters(string ExactVersion);