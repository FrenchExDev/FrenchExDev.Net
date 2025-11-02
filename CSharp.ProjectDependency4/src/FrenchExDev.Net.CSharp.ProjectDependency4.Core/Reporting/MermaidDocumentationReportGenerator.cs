using System.Text;
using FrenchExDev.Net.CSharp.ProjectDependency4.Core.Analysis;
using FrenchExDev.Net.CSharp.ProjectDependency4.Core.Markdown;

namespace FrenchExDev.Net.CSharp.ProjectDependency4.Core.Reporting;

/// <summary>
/// Generates Mermaid markdown documentation for projects and their code elements.
/// </summary>
public class MermaidDocumentationReportGenerator : IReportGenerator<MermaidDocumentationResult>
{
    public string Name => "MermaidDocumentationReport";

    public MarkdownSection[] Generate(MermaidDocumentationResult result)
    {
        var sections = new List<MarkdownSection>();

        // Generate index section with project-level Mermaid graph
      var indexSection = GenerateIndexSection(result);
        sections.Add(indexSection);

     // Generate sections for each project
 foreach (var project in result.Projects.OrderBy(p => p.Name, StringComparer.OrdinalIgnoreCase))
    {
   var projectSection = GenerateProjectSection(project);
   sections.Add(projectSection);
        }

        return sections.ToArray();
    }

    private MarkdownSection GenerateIndexSection(MermaidDocumentationResult result)
    {
        var section = new MarkdownSection("Projects Overview");

  section.AddContent("This section provides an overview of all projects in the solution and their relationships.");

  // Generate project dependency graph
 var mermaid = new StringBuilder();
   mermaid.AppendLine("```mermaid");
 mermaid.AppendLine("graph TD");

        // Add all projects as nodes
        foreach (var project in result.Projects)
        {
     var projectId = SanitizeId(project.Name);
 mermaid.AppendLine($"  {projectId}[\"{project.Name}\"]");
  }

// Add edges for project references
        foreach (var project in result.Projects)
        {
     var fromId = SanitizeId(project.Name);
     foreach (var refPath in project.ProjectReferences)
    {
      var refName = System.IO.Path.GetFileNameWithoutExtension(refPath);
                var toId = SanitizeId(refName);
    mermaid.AppendLine($"    {fromId} --> {toId}");
            }
        }

        mermaid.AppendLine("```");
        section.AddContent(mermaid.ToString());

   // Add project summary table
        var table = new StringBuilder();
        table.AppendLine("| Project | Elements | References | Used By |");
        table.AppendLine("|---------|----------|------------|---------|");
        foreach (var project in result.Projects.OrderBy(p => p.Name, StringComparer.OrdinalIgnoreCase))
     {
         var elementCount = project.Elements.Count;
            var refCount = project.ProjectReferences.Count;
   var usedByCount = project.UsedByProjects.Count;
       table.AppendLine($"| {project.Name} | {elementCount} | {refCount} | {usedByCount} |");
        }
        section.AddContent(table.ToString());

        return section;
    }

    private MarkdownSection GenerateProjectSection(ProjectDocumentation project)
    {
      var section = new MarkdownSection($"Project: {project.Name}");

        // Add project overview
        section.AddContent($"**Path:** `{project.Path}`");
        section.AddContent($"**Public Elements:** {project.Elements.Count}");

     // Project dependencies graph
        if (project.ProjectReferences.Any() || project.UsedByProjects.Any())
  {
          var projectGraph = new StringBuilder();
    projectGraph.AppendLine("### Project Dependencies");
            projectGraph.AppendLine("```mermaid");
            projectGraph.AppendLine("graph LR");

   var projectId = SanitizeId(project.Name);
 projectGraph.AppendLine($"    {projectId}[\"{project.Name}\"]");

            // Show what this project uses
      foreach (var refPath in project.ProjectReferences)
      {
    var refName = System.IO.Path.GetFileNameWithoutExtension(refPath);
          var refId = SanitizeId(refName);
     projectGraph.AppendLine($"    {projectId} -->|uses| {refId}[\"{refName}\"]");
            }

      // Show what uses this project
            foreach (var usedBy in project.UsedByProjects)
  {
        var usedById = SanitizeId(usedBy);
        projectGraph.AppendLine($"    {usedById}[\"{usedBy}\"] -->|uses| {projectId}");
         }

        projectGraph.AppendLine("```");
    section.AddContent(projectGraph.ToString());
        }

  // Add elements subsection
      if (project.Elements.Any())
        {
            var elementsSubsection = new MarkdownSection("Code Elements");

        // Group elements by kind
   var elementsByKind = project.Elements.GroupBy(e => e.Kind).OrderBy(g => g.Key);

    foreach (var group in elementsByKind)
            {
         var kindSubsection = new MarkdownSection($"{char.ToUpper(group.Key[0])}{group.Key.Substring(1)}s");

                foreach (var element in group.OrderBy(e => e.Name, StringComparer.OrdinalIgnoreCase))
        {
         var elementSubsection = GenerateElementSection(element);
         kindSubsection.AddSubSection(elementSubsection);
    }

         elementsSubsection.AddSubSection(kindSubsection);
    }

    section.AddSubSection(elementsSubsection);
    }

        return section;
    }

    private MarkdownSection GenerateElementSection(CodeElementDocumentation element)
    {
   var section = new MarkdownSection($"{element.Name}");

        section.AddContent($"**Type:** `{element.Kind}`");
     section.AddContent($"**Full Name:** `{element.FullName}`");

        // Generate Mermaid graph for this element
 var hasRelations = element.Usings.Any() || element.Usages.Any();
        if (hasRelations)
        {
  var mermaid = new StringBuilder();
   mermaid.AppendLine("```mermaid");
    mermaid.AppendLine("graph TD");

            var elementId = SanitizeId(element.FullName);
            mermaid.AppendLine($"  {elementId}[\"{element.Name}\"]");
         mermaid.AppendLine($"    style {elementId} fill:#e1f5ff,stroke:#01579b,stroke-width:2px");

    // Add usings (dependencies)
    if (element.Usings.Any())
      {
    foreach (var usingType in element.Usings.Take(20)) // Limit to avoid overcrowding
              {
  var usingId = SanitizeId(usingType);
            var displayName = GetShortName(usingType);
         mermaid.AppendLine($"  {usingId}[\"{displayName}\"]");
      mermaid.AppendLine($"    {elementId} -->|uses| {usingId}");
 }
       if (element.Usings.Count > 20)
      {
     mermaid.AppendLine($"    more_usings[\"...and {element.Usings.Count - 20} more\"]");
     mermaid.AppendLine($"    {elementId} -.-> more_usings");
 }
   }

            // Add usages (dependents)
        if (element.Usages.Any())
   {
      foreach (var usageType in element.Usages.Take(20)) // Limit to avoid overcrowding
             {
   var usageId = SanitizeId(usageType);
        var displayName = GetShortName(usageType);
         mermaid.AppendLine($"    {usageId}[\"{displayName}\"]");
        mermaid.AppendLine($"    {usageId} -->|uses| {elementId}");
          }
  if (element.Usages.Count > 20)
       {
       mermaid.AppendLine($"    more_usages[\"...and {element.Usages.Count - 20} more\"]");
  mermaid.AppendLine($"    more_usages -.-> {elementId}");
      }
            }

 mermaid.AppendLine("```");
    section.AddContent(mermaid.ToString());
        }

        // Add lists of dependencies
        if (element.Usings.Any())
        {
            var usingsSection = new StringBuilder();
       usingsSection.AppendLine("**Dependencies (Uses):**");
         foreach (var usingType in element.Usings.OrderBy(u => u, StringComparer.OrdinalIgnoreCase))
    {
      usingsSection.AppendLine($"- `{usingType}`");
  }
 section.AddContent(usingsSection.ToString());
        }

        // Add lists of dependents
        if (element.Usages.Any())
        {
       var usagesSection = new StringBuilder();
          usagesSection.AppendLine("**Used By:**");
      foreach (var usageType in element.Usages.OrderBy(u => u, StringComparer.OrdinalIgnoreCase))
            {
       usagesSection.AppendLine($"- `{usageType}`");
        }
 section.AddContent(usagesSection.ToString());
        }

        return section;
    }

    private static string SanitizeId(string name)
    {
        // Remove special characters and replace with underscores
 var sanitized = new string(name.Where(ch => char.IsLetterOrDigit(ch) || ch == '_' || ch == '.').Select(ch => ch == '.' ? '_' : ch).ToArray());
     if (string.IsNullOrEmpty(sanitized) || char.IsDigit(sanitized[0]))
        {
     sanitized = "E_" + sanitized;
        }
        return sanitized;
    }

    private static string GetShortName(string fullName)
    {
        // Extract just the type name without namespace
        var lastDot = fullName.LastIndexOf('.');
        if (lastDot >= 0 && lastDot < fullName.Length - 1)
        {
     return fullName.Substring(lastDot + 1);
        }
        return fullName;
    }
}
