namespace FrenchExDev.Net.CSharp.ProjectDependency5.Core.Analysis;

public class GraphModel
{
 public List<GraphNode> Nodes { get; } = new();
 public List<GraphLink> Links { get; } = new();
}

public class GraphNode
{
 public required string Id { get; init; }
 public required string Name { get; init; }
 public required string Kind { get; init; } // Project, Namespace, Class, Interface, Enum, Record, Struct
 public string? ParentId { get; init; } // for hierarchy
}

public class GraphLink
{
 public required string SourceId { get; init; }
 public required string TargetId { get; init; }
 public required string Kind { get; init; } // Reference, Usage, Inherits, Implements, Contains
}
