namespace FrenchExDev.Net.CSharp.Object.Builder2.Tests;

public class TaggedItemBuilder : AbstractBuilder<TaggedItem>
{
    public List<string>? Tags { get; set; }
    public List<string>? NullableTags { get; set; }

    protected override TaggedItem Instantiate() => new() { Tags = Tags ?? [] };

    protected override void ValidateInternal(VisitedObjectDictionary visitedCollector, IFailureCollector failures)
    {
        AssertNotEmptyOrWhitespace(Tags, nameof(Tags), failures, n => new StringIsEmptyOrWhitespaceException(n));
        AssertNotEmptyOrWhitespace(NullableTags, nameof(NullableTags), failures, n => new StringIsEmptyOrWhitespaceException(n));
        AssertNotNull(Tags, nameof(Tags), failures, n => new ArgumentNullException(n));
        AssertNotNullNotEmptyCollection(Tags, nameof(Tags), failures, n => new StringIsEmptyOrWhitespaceException(n));
    }

    public TaggedItemBuilder WithTags(List<string> tags) { Tags = tags; return this; }
    public TaggedItemBuilder WithNullableTags(List<string>? tags) { NullableTags = tags; return this; }
}
