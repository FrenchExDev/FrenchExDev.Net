using FrenchExDev.Net.CSharp.Object.Builder2;

namespace FrenchExDev.Net.HttpClient.Testing;

/// <summary>
/// Provides a mock implementation of the IHttpClient interface for simulating HTTP requests and responses in unit
/// tests.
/// </summary>
/// <remarks>FakeHttpClient allows developers to specify a sequence of HTTP response messages that are returned in
/// order for each simulated request. This enables predictable and controlled testing of components that depend on HTTP
/// client behavior without making actual network calls. The class is intended for use in test scenarios and is not
/// suitable for production environments.</remarks>
public class FakeHttpClient : IHttpClient
{
    /// <summary>
    /// Stores call count
    /// </summary>
    private int _callCount = 0;

    /// <summary>
    /// Stores response
    /// </summary>
    private List<HttpResponseMessage> _responses = [];

    /// <summary>
    /// Sets the HTTP response message to be used for subsequent operations.
    /// </summary>
    /// <param name="message">The HTTP response message to assign. Cannot be null.</param>
    public FakeHttpClient Response(HttpResponseMessage message)
    {
        _responses.Add(message);
        return this;
    }

    /// <summary>
    /// Initializes a new instance of the FakeGetHttpClient class with the specified HTTP response message.
    /// </summary>
    /// <remarks>This constructor is typically used in unit tests to provide a predetermined HTTP
    /// response for simulating HTTP client behavior.</remarks>
    /// <param name="response">The HTTP response message to be returned by the client. Cannot be null.</param>
    public FakeHttpClient(HttpResponseMessage[] responses)
    {
        _responses = responses.ToList();
    }

    /// <summary>
    /// Initializes a new instance of the FakeGetHttpClient class with a predefined sequence of HTTP responses.
    /// </summary>
    /// <remarks>Use this constructor to provide a set of responses that will be returned in order when
    /// simulating HTTP GET requests. This is useful for unit testing scenarios where predictable HTTP responses are
    /// required.</remarks>
    /// <param name="responses">The list of HTTP response messages to be returned by the client. Cannot be null.</param>
    public FakeHttpClient(List<HttpResponseMessage> responses)
    {
        _responses = responses;
    }

    /// <summary>
    /// Returns the current HTTP response message and increments the call count.
    /// </summary>
    /// <param name="url"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public Task<HttpResponseMessage> GetAsync(string url, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_responses[_callCount++] ?? throw new InvalidOperationException());
    }
}

/// <summary>
/// Provides a builder for creating and configuring instances of <see cref="IHttpClient"/> for testing or mocking
/// HTTP GET client behavior.
/// </summary>
/// <remarks>Use this builder to specify HTTP response details such as status code and content before constructing
/// a fake HTTP client. This type is intended for scenarios where controlled HTTP responses are required, such as unit
/// testing components that depend on HTTP GET operations.</remarks>
public class FakeHttpClientBuilder : IBuilder<IHttpClient>
{
    public Guid Id => throw new NotImplementedException();

    /// <summary>
    /// Stores the build result once the object has been constructed.
    /// </summary>
    private IResult? _result;
    /// <summary>
    /// Gets the result of the build operation.
    /// </summary>
    /// <remarks>Accessing this property before the build process has completed will throw an exception. The
    /// returned result provides details about the outcome of the build, including success status and any associated
    /// errors.</remarks>
    public IResult Result => _result ?? throw new InvalidOperationException(nameof(_result));

    public BuildStatus BuildStatus => throw new NotImplementedException();

    public ValidationStatus ValidationStatus => throw new NotImplementedException();

    /// <summary>
    /// Stores the list of HTTP response builders to configure the fake HTTP client.
    /// </summary>
    private BuilderList<HttpResponseMessage, HttpResponseMessageBuilder> _builders = [];

    /// <summary>
    /// Adds a custom HTTP response configuration using the specified builder action.
    /// </summary>
    /// <remarks>Use this method to specify multiple custom HTTP responses by calling it repeatedly with
    /// different builder actions. The configured responses will be used in subsequent HTTP client operations.</remarks>
    /// <param name="builder">An action that configures the <see cref="HttpResponseMessageBuilder"/> to define the desired HTTP response.
    /// Cannot be null.</param>
    /// <returns>The current <see cref="FakeHttpClientBuilder"/> instance to allow method chaining.</returns>
    public FakeHttpClientBuilder Response(Action<HttpResponseMessageBuilder> builder)
    {
        var b = new HttpResponseMessageBuilder();
        builder(b);
        _builders.Add(b);
        return this;
    }

    /// <summary>
    /// Builds and returns the result of the HTTP client construction process.
    /// </summary>
    /// <param name="visitedCollector">An optional dictionary used to track visited objects during the build process. If provided, it helps prevent
    /// redundant processing of objects that have already been handled.</param>
    /// <returns>An object that represents the result of the build operation, containing the constructed HTTP client instance.</returns>
    public IResult Build(VisitedObjectDictionary? visitedCollector = null)
    {
        _result = new SuccessResult<IHttpClient>(new FakeHttpClient(_builders.BuildSuccess()));
        return Result;
    }

    /// </inheritdoc/>
    public Reference<IHttpClient> Reference()
    {
        throw new NotImplementedException();
    }

    /// </inheritdoc/>
    public void OnBuilt(Action<IHttpClient> hook)
    {
        throw new NotImplementedException();
    }

    public void Validate(VisitedObjectDictionary visitedCollector, FailuresDictionary failures)
    {
        throw new NotImplementedException();
    }

    public void Existing(IHttpClient instance)
    {
        throw new NotImplementedException();
    }

    IBuilder<IHttpClient> IBuilder<IHttpClient>.Existing(IHttpClient instance)
    {
        throw new NotImplementedException();
    }
}
