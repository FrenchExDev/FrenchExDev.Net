namespace FrenchExDev.Net.CSharp.ProjectDependency3.Markdown;

/// <summary>
/// Represents a section of a Markdown document, including its title, content lines, and any nested subsections.
/// </summary>
/// <remarks>Use this class to construct hierarchical Markdown documents programmatically. Each section can
/// contain multiple lines of content and an arbitrary number of nested subsections, allowing for flexible document
/// structures. The class is not thread-safe; concurrent modifications should be synchronized externally if
/// needed.</remarks>
public class MarkdownSection
{
    /// <summary>
    /// Gets the title associated with the current instance.
    /// </summary>
    public string Title { get; }

    /// <summary>
    /// Gets the collection of subsections contained within this section.
    /// </summary>
    public List<MarkdownSection> SubSections { get; } = new();

    /// <summary>
    /// Gets the collection of content items associated with this instance.
    /// </summary>
    public List<string> Content { get; } = new();

    /// <summary>
    /// Initializes a new instance of the MarkdownSection class with the specified section title.
    /// </summary>
    /// <param name="title">The title of the markdown section. Cannot be null.</param>
    public MarkdownSection(string title)
    {
        Title = title;
    }

    /// <summary>
    /// Appends a line of content to the current Markdown section.
    /// </summary>
    /// <remarks>This method enables fluent chaining by returning the same instance. If <paramref
    /// name="line"/> is null, an exception may be thrown.</remarks>
    /// <param name="line">The line of text to add to the section. Cannot be null.</param>
    /// <returns>The current <see cref="MarkdownSection"/> instance with the new content added.</returns>
    public MarkdownSection AddContent(string line)
    {
        Content.Add(line);
        return this;
    }

    /// <summary>
    /// Adds the specified subsection to the current Markdown section.
    /// </summary>
    /// <param name="section">The subsection to add. Cannot be null.</param>
    /// <returns>The current MarkdownSection instance, allowing for method chaining.</returns>
    public MarkdownSection AddSubSection(MarkdownSection section)
    {
        SubSections.Add(section);
        return this;
    }

    /// <summary>
    /// Generates a formatted string representation of the section and its content, using a Markdown-style header at the
    /// specified level.
    /// </summary>
    /// <remarks>Subsections are rendered recursively with incremented header levels. The output is suitable
    /// for Markdown rendering.</remarks>
    /// <param name="level">The header level to use for the section title. Must be between 1 and 6, inclusive; values outside this range are
    /// clamped.</param>
    /// <returns>A string containing the formatted section, including its title, content, and any subsections, with appropriate
    /// header levels.</returns>
    internal string Render(int level)
    {
        var header = new string('#', Math.Clamp(level, 1, 6)) + " " + Title;
        var parts = new List<string> { header };
        if (Content.Count > 0)
        {
            parts.Add(string.Join("\n", Content));
        }
        foreach (var sub in SubSections)
        {
            parts.Add(sub.Render(level + 1));
        }
        return string.Join("\n\n", parts);
    }
}
