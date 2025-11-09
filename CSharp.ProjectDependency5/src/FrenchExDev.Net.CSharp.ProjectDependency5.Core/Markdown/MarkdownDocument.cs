namespace FrenchExDev.Net.CSharp.ProjectDependency5.Core.Markdown;

public class MarkdownDocument
{
    public List<MarkdownSection> Sections { get; } = new();

    public MarkdownDocument AddSection(MarkdownSection section)
    {
        Sections.Add(section);
        return this;
    }

    public string Render()
    {
        var toc = BuildToc();
        var body = string.Join("\n\n", Sections.Select(s => s.Render(1)));
        return string.Join("\n\n", new[] { toc, body }.Where(s => !string.IsNullOrWhiteSpace(s)));
    }

    private string BuildToc()
    {
        if (Sections.Count == 0) return string.Empty;

        var lines = new List<string>
        {
            "## Table of Contents"
        };

        var slugCounts = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        AppendTocForSections(Sections, 0, lines, slugCounts);

        return string.Join("\n", lines);
    }

    private static void AppendTocForSections(IEnumerable<MarkdownSection> sections, int depth, List<string> lines, Dictionary<string, int> slugCounts)
    {
        foreach (var section in sections)
        {
            var slug = GetUniqueSlug(section.Title, slugCounts);
            var indent = new string(' ', depth * 2);
            lines.Add($"{indent}- [{section.Title}](#{slug})");

            // Recurse through subsections until leaves
            if (section.SubSections.Count > 0)
            {
                AppendTocForSections(section.SubSections, depth + 1, lines, slugCounts);
            }
        }
    }

    private static string GetUniqueSlug(string title, Dictionary<string, int> slugCounts)
    {
        var baseSlug = Slugify(title);
        if (!slugCounts.TryGetValue(baseSlug, out var count))
        {
            slugCounts[baseSlug] = 1;
            return baseSlug;
        }
        else
        {
            count++;
            slugCounts[baseSlug] = count;
            return $"{baseSlug}-{count - 1}"; // mimic GitHub style duplicates: -1, -2, ...
        }
    }

    private static string Slugify(string title)
    {
        // Lowercase, keep letters/digits/space/hyphen/underscore, spaces -> hyphen, collapse hyphens, trim hyphens
        var lower = title.Trim().ToLowerInvariant();
        var filtered = new string(lower.Select(ch => char.IsLetterOrDigit(ch) || ch == ' ' || ch == '-' || ch == '_' ? ch : '\0').Where(ch => ch != '\0').ToArray());
        var withHyphens = string.Join('-', filtered.Split(' ', StringSplitOptions.RemoveEmptyEntries));
        while (withHyphens.Contains("--")) withHyphens = withHyphens.Replace("--", "-");
        return withHyphens.Trim('-');
    }
}
