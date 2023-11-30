using JMayer.Data.Data;
using System.Net;
using System.Text.Json;

namespace JMayer.Data.HTTP.Handler;

/// <summary>
/// The class mocks a HTTP message handler.
/// </summary>
/// <remarks>
/// This will create mocked HTTP responses based on how
/// you configured it using the fluent style.
/// </remarks>
public class MockHttpMessageHandler : HttpMessageHandler
{
    /// <summary>
    /// The based address for the request.
    /// </summary>
    private Uri _baseAddress = new("http://localhost/");

    /// <summary>
    /// The content to be expected in the request.
    /// </summary>
    private string? _requestContent;

    /// <summary>
    /// The method to be expected in the request.
    /// </summary>
    private HttpMethod _method = HttpMethod.Get;

    /// <summary>
    /// The route to be expected in the request.
    /// </summary>
    private string _route = "/";

    /// <summary>
    /// The HTTP status code to respond with.
    /// </summary>
    private HttpStatusCode _responseStatusCode = HttpStatusCode.OK;

    /// <summary>
    /// The string content to respond with.
    /// </summary>
    private StringContent _responseStringContent = new(string.Empty);

    /// <summary>
    /// The method returns the HTTP client to be used in the unit test.
    /// </summary>
    /// <returns>A HttpClient object.</returns>
    public HttpClient Build()
    {
        return new HttpClient(this)
        {
            BaseAddress = _baseAddress,
        };
    }

    /// <summary>
    /// The method sets the HTTP status code to respond with.
    /// </summary>
    /// <param name="statusCode">The HTTP status code to respond with.</param>
    /// <returns>Itself.</returns>
    public MockHttpMessageHandler RespondingHttpStatusCode(HttpStatusCode statusCode)
    {
        _responseStatusCode = statusCode;
        return this;
    }

    /// <summary>
    /// The method sets the Json content to repond with.
    /// </summary>
    /// <typeparam name="T">A DataObject must be serialized.</typeparam>
    /// <param name="dataObject">The data object to be serialized into Json content.</param>
    /// <returns>Itself.</returns>
    public MockHttpMessageHandler RespondingJsonContent<T>(T dataObject) where T : DataObject
    {
        string stringContent = JsonSerializer.Serialize(dataObject);
        _responseStringContent = new StringContent(stringContent);
        return this;
    }

    /// <summary>
    /// The method sets the Json content to repond with.
    /// </summary>
    /// <typeparam name="T">A DataObject must be serialized.</typeparam>
    /// <param name="dataObjects">The data objects to be serialized into Json content.</param>
    /// <returns>Itself.</returns>
    public MockHttpMessageHandler RespondingJsonContent<T>(List<T> dataObjects) where T : DataObject
    {
        string stringContent = JsonSerializer.Serialize(dataObjects);
        _responseStringContent = new StringContent(stringContent);
        return this;
    }

    /// <summary>
    /// The method mocks the respond that would occur when sending a HTTP response.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <param name="cancellationToken">A token used for task cancellations.</param>
    /// <returns>The HTTP response message.</returns>
    protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
#warning I'll need to determine the correct responses for the negative checks.

        if (request.RequestUri != new Uri(_baseAddress, _route))
        {
            return await Task.FromResult(new HttpResponseMessage(HttpStatusCode.NotFound));
        }

        if (request.Method != _method)
        {
            return await Task.FromResult(new HttpResponseMessage(HttpStatusCode.NotFound));
        }

        if (request.Content == null && _requestContent != null)
        {
            return await Task.FromResult(new HttpResponseMessage(HttpStatusCode.BadRequest));
        }

        if (request.Content != null)
        {
            string requestContent = await request.Content.ReadAsStringAsync(cancellationToken);

            if (requestContent != _requestContent)
            {
                return await Task.FromResult(new HttpResponseMessage(HttpStatusCode.BadRequest));
            }
        }

        HttpResponseMessage httpResponseMessage = new(_responseStatusCode)
        {
            Content = _responseStringContent,
        };

        return await Task.FromResult(httpResponseMessage);
    }

    /// <summary>
    /// The method sets the base address for the HttpClient to be built.
    /// </summary>
    /// <param name="baseAddress">The base address to be used by Build().</param>
    /// <returns>Itself.</returns>
    public MockHttpMessageHandler WithBaseAddress(Uri baseAddress)
    {
        _baseAddress = baseAddress;
        return this;
    }

    /// <summary>
    /// The method sets the content to be expected in the request.
    /// </summary>
    /// <typeparam name="T">A DataObject must be serialized.</typeparam>
    /// <param name="dataObject">The data object to be expected; will be serialized into json.</param>
    /// <returns>Itself.</returns>
    public MockHttpMessageHandler WithContent<T>(T dataObject) where T : DataObject
    {
        _requestContent = JsonSerializer.Serialize(dataObject);
        return this;
    }

    /// <summary>
    /// The method sets the content to be expected in the request.
    /// </summary>
    /// <typeparam name="T">A DataObject must be serialized.</typeparam>
    /// <param name="dataObjects">The data objects to be expected; will be serialized into json.</param>
    /// <returns>Itself.</returns>
    public MockHttpMessageHandler WithContent<T>(List<T> dataObjects) where T : DataObject
    {
        _requestContent = JsonSerializer.Serialize(dataObjects);
        return this;
    }

    /// <summary>
    /// The method sets the method to be expected in the request.
    /// </summary>
    /// <param name="method">The method to be expected.</param>
    /// <returns>Itself.</returns>
    public MockHttpMessageHandler WithMethod(HttpMethod method)
    {
        _method = method;
        return this;
    }

    /// <summary>
    /// The method sets the relative route to be expected in the request.
    /// </summary>
    /// <param name="route">The route to be expected.</param>
    /// <returns>Itself.</returns>
    public MockHttpMessageHandler WithRoute(string route)
    {
        _route = route;
        return this;
    }
}
