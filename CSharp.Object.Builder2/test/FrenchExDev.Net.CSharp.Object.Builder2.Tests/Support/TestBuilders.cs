using FrenchExDev.Net.CSharp.Object.Builder2;

namespace FrenchExDev.Net.CSharp.Object.Builder2.Tests;

// Consolidated test-only builders used across unit tests
public class SimpleBuilder : AbstractBuilder<string>
{
    private string? _value;
    public SimpleBuilder Value(string v) { _value = v; return this; }
    protected override string Instantiate() => _value ?? string.Empty;
}

public class FailBuilder : AbstractBuilder<string>
{
    protected override string Instantiate() => string.Empty;
    protected override void ValidateInternal(VisitedObjectDictionary visitedCollector, FailuresDictionary failures)
    {
        failures.Failure("x", new Failure("err"));
    }
}

public class TestBuilder : AbstractBuilder<string>
{
    private string? _v;
    public TestBuilder Value(string v) { _v = v; return this; }
    protected override string Instantiate() => _v ?? string.Empty;
}

public class BuilderForSuccess : AbstractBuilder<string>
{
    protected override string Instantiate() => "ok";
}

public class PublicBuilder : AbstractBuilder<string>
{
    private bool _invalid;
    private string? _value;
    public PublicBuilder() { }
    public PublicBuilder MakeInvalid() { _invalid = true; return this; }
    public PublicBuilder WithValue(string v) { _value = v; return this; }

    protected override string Instantiate() => _value ?? string.Empty;

    protected override void ValidateInternal(VisitedObjectDictionary visitedCollector, FailuresDictionary failures)
    {
        if (_invalid) failures.Failure("invalid", new Failure(new System.Exception("bad")));
    }

    public void CallAssertNotEmptyOrWhitespace_String(string? value, string name, FailuresDictionary failures, Func<string, Exception> builder)
        => AssertNotEmptyOrWhitespace(value, name, failures, builder);

    public void CallAssertNotEmptyOrWhitespace_List(List<string>? list, string name, FailuresDictionary failures, Func<string, Exception> builder)
        => AssertNotEmptyOrWhitespace(list, name, failures, builder);

    public void CallAssertNotNullNotEmptyCollection<TOther>(List<TOther>? list, string name, FailuresDictionary failures, Func<string, Exception> builder)
        => AssertNotNullNotEmptyCollection(list, name, failures, builder);

    public void CallAssert(Func<bool> predicate, string name, FailuresDictionary failures, Func<string, Exception> builder)
        => Assert(predicate, name, failures, builder);

    public void CallBuildList<TBuilder, TModel>(BuilderList<TModel, TBuilder> builders, VisitedObjectDictionary visited) where TBuilder : IBuilder<TModel>, new() where TModel : class
        => BuildList(builders, visited);

    public void CallValidateListInternal<TOtherClass, TOtherBuilder>(BuilderList<TOtherClass, TOtherBuilder> list, string propertyName, VisitedObjectDictionary visitedCollector, FailuresDictionary failures)
        where TOtherClass : class where TOtherBuilder : class, IBuilder<TOtherClass>, new()
        => ValidateListInternal(list, propertyName, visitedCollector, failures);
}

public class FailingBuilder : AbstractBuilder<string>
{
    protected override string Instantiate() => string.Empty;
    protected override void ValidateInternal(VisitedObjectDictionary visitedCollector, FailuresDictionary failures)
    {
        failures.Failure("x", new System.Exception("err"));
    }
}

// HelperBuilder used by Additional_Helpers_Tests - test-only
public class HelperBuilder : AbstractBuilder<object>
{
    private bool _invalid;
    public HelperBuilder MakeInvalid() { _invalid = true; return this; }
    protected override object Instantiate() => new object();
    protected override void ValidateInternal(VisitedObjectDictionary visitedCollector, FailuresDictionary failures)
    {
        if (_invalid) failures.Failure("bad", new Failure("err"));
    }

    public void CallAssertNotEmptyOrWhitespace_String(string? value, string name, FailuresDictionary failures, Func<string, Exception> builder)
        => AssertNotEmptyOrWhitespace(value, name, failures, builder);

    public void CallAssertNotEmptyOrWhitespace_List(List<string>? list, string name, FailuresDictionary failures, Func<string, Exception> builder)
        => AssertNotEmptyOrWhitespace(list, name, failures, builder);

    public void CallAssertNotNullOrEmptyOrWhitespace(string? value, string name, FailuresDictionary failures, Func<string, Exception> builder)
        => AssertNotNullOrEmptyOrWhitespace(value, name, failures, builder);

    public void CallAssertNotNull(object? value, string name, FailuresDictionary failures, Func<string, Exception> builder)
        => AssertNotNull(value, name, failures, builder);

    public void CallAssertNotNullNotEmptyCollection<T>(List<T>? list, string name, FailuresDictionary failures, Func<string, Exception> builder)
        => AssertNotNullNotEmptyCollection(list, name, failures, builder);

    public void CallAssert(Func<bool> predicate, string name, FailuresDictionary failures, Func<string, Exception> builder)
        => Assert(predicate, name, failures, builder);
}
