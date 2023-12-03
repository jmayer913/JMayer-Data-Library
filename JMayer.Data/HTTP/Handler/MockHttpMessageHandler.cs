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
    /// The parameters to be expected in the request.
    /// </summary>
    private List<string> _routeParameters = [];

    /// <summary>
    /// The query string to be expected in the request.
    /// </summary>
    private Dictionary<string, string> _queryString = [];

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
    /// The Json serializer options.
    /// </summary>
    private readonly JsonSerializerOptions _serializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

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
    /// The method returns the route parameters as a string.
    /// </summary>
    /// <returns>The route parameter string.</returns>
    private string GetRouteParameters(bool includeSlashAtStart)
    {
        string parametersAsRoute = string.Empty;

        for (int index = 0; index < _routeParameters.Count; ++index)
        {
            if (index > 0)
            {
                parametersAsRoute += "/";
            }

            parametersAsRoute += _routeParameters[index];
        }

        if (includeSlashAtStart && parametersAsRoute.Length > 0)
        {
            parametersAsRoute = $"/{parametersAsRoute}";
        }

        return parametersAsRoute;
    }

    /// <summary>
    /// The method returns the query string dictionary as a string.
    /// </summary>
    /// <returns>The query string.</returns>
    private string GetQueryString()
    {
        if (_queryString.Count == 0)
        {
            return string.Empty;
        }

        string queryString = "?";

        foreach (string key in _queryString.Keys)
        {
            if (key != _queryString.Keys.First())
            {
                queryString += "&";
            }

            queryString += $"{key}={_queryString[key]}";
        }

        return queryString;
    }

    /// <summary>
    /// The method sets the HTTP status code to respond with.
    /// </summary>
    /// <param name="statusCode">The HTTP status code to respond with.</param>
    /// <returns>Itself for the fluent style.</returns>
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
    /// <exception cref="ArgumentNullException">Thrown if the dataObject parameter is null.</exception>
    /// <returns>Itself for the fluent style.</returns>
    public MockHttpMessageHandler RespondingJsonContent<T>(T dataObject) where T : class
    {
        ArgumentNullException.ThrowIfNull(dataObject);
        string stringContent = JsonSerializer.Serialize(dataObject);
        _responseStringContent = new StringContent(stringContent);
        return this;
    }

    /// <summary>
    /// The method sets the Json content to repond with.
    /// </summary>
    /// <typeparam name="T">A DataObject must be serialized.</typeparam>
    /// <param name="dataObjects">The data objects to be serialized into Json content.</param>
    /// <exception cref="ArgumentNullException">Thrown if the dataObjects parameter is null.</exception>
    /// <returns>Itself for the fluent style.</returns>
    public MockHttpMessageHandler RespondingJsonContent<T>(List<T> dataObjects) where T : class
    {
        ArgumentNullException.ThrowIfNull(dataObjects);
        string stringContent = JsonSerializer.Serialize(dataObjects);
        _responseStringContent = new StringContent(stringContent);
        return this;
    }

    /// <summary>
    /// The method sets the string content to respond with.
    /// </summary>
    /// <typeparam name="T">Must be a primitive type; primitives are IConvertible.</typeparam>
    /// <param name="value">The string value to respond with; a non-string will be converted into a string.</param>
    /// <returns>Itself for the fluent style.</returns>
    public MockHttpMessageHandler RespondingStringContent<T>(T value) where T : IConvertible
    {
        ArgumentNullException.ThrowIfNull(value);
        string? stringContent = Convert.ToString(value);

#warning If the type converts to null, should an exception be thrown instead of a silent fail?

        if (stringContent != null)
        {
            _responseStringContent = new StringContent(stringContent);
        }

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
        if (request.RequestUri != new Uri(_baseAddress, $"{_route}{GetRouteParameters(!_route.EndsWith('/'))}{GetQueryString()}"))
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
    /// <exception cref="ArgumentNullException">Thrown if the baseAddress parameter is null.</exception>
    /// <returns>Itself for the fluent style.</returns>
    public MockHttpMessageHandler WithBaseAddress(Uri baseAddress)
    {
        ArgumentNullException.ThrowIfNull(baseAddress);
        _baseAddress = baseAddress;
        return this;
    }

    /// <summary>
    /// The method sets the json content to be expected in the request.
    /// </summary>
    /// <typeparam name="T">A DataObject must be serialized.</typeparam>
    /// <param name="dataObject">The data object to be expected; will be serialized into json.</param>
    /// <exception cref="ArgumentNullException">Thrown if the dataObjects parameter is null.</exception>
    /// <returns>Itself for the fluent style.</returns>
    public MockHttpMessageHandler WithJsonContent<T>(T dataObject) where T : class
    {
        ArgumentNullException.ThrowIfNull(dataObject);
        _requestContent = JsonSerializer.Serialize(dataObject, _serializerOptions);
        return this;
    }

    /// <summary>
    /// The method sets the json content to be expected in the request.
    /// </summary>
    /// <typeparam name="T">A DataObject must be serialized.</typeparam>
    /// <param name="dataObjects">The data objects to be expected; will be serialized into json.</param>
    /// <exception cref="ArgumentNullException">Thrown if the dataObjects parameter is null.</exception>
    /// <returns>Itself for the fluent style.</returns>
    public MockHttpMessageHandler WithJsonContent<T>(List<T> dataObjects) where T : class
    {
        ArgumentNullException.ThrowIfNull(dataObjects);
        _requestContent = JsonSerializer.Serialize(dataObjects, _serializerOptions);
        return this;
    }

    /// <summary>
    /// The method sets the string content to be expected in the request.
    /// </summary>
    /// <typeparam name="T">Must be a primitive type; primitives are IConvertible.</typeparam>
    /// <param name="value">The value to be expected; a non-string will be converted into a string.</param>
    /// <exception cref="ArgumentNullException">Thrown if the value parameter is null.</exception>
    /// <returns>Itself for the fluent style.</returns>
    public MockHttpMessageHandler WithStringContent<T>(T value) where T : IConvertible
    {
        ArgumentNullException.ThrowIfNull(value);
        _requestContent = Convert.ToString(value);
        return this;
    }

    /// <summary>
    /// The method sets the method to be expected in the request.
    /// </summary>
    /// <param name="method">The method to be expected.</param>
    /// <exception cref="ArgumentNullException">Thrown if the method parameter is null.</exception>
    /// <returns>Itself for the fluent style.</returns>
    public MockHttpMessageHandler WithMethod(HttpMethod method)
    {
        ArgumentNullException.ThrowIfNull(method);
        _method = method;
        return this;
    }

    /// <summary>
    /// The method sets the route parameters to be expected in the request.
    /// </summary>
    /// <param name="routeParameters">The route parameters to be expected.</param>
    /// <exception cref="ArgumentNullException">Thrown if the queryString parameter is null.</exception>
    /// <returns>Itself for the fluent style.</returns>
    /// <remarks>
    /// The parameters will be appended to the route and before the query string. It will 
    /// have the following format in the route:
    /// 
    /// parameter1/parameter2/.../parameterN
    /// </remarks>
    public MockHttpMessageHandler WithRouteParameters(List<string> routeParameters)
    {
        ArgumentNullException.ThrowIfNull(routeParameters);
        _routeParameters = routeParameters;
        return this;
    }

    /// <summary>
    /// The method sets the query string to be expected in the request.
    /// </summary>
    /// <param name="queryString">The query string to be expected.</param>
    /// <exception cref="ArgumentNullException">Thrown if the queryString parameter is null.</exception>
    /// <returns>Itself for the fluent style.</returns>
    public MockHttpMessageHandler WithQueryString(Dictionary<string, string> queryString)
    {
        ArgumentNullException.ThrowIfNull(queryString);
        _queryString = queryString;
        return this;
    }

    /// <summary>
    /// The method sets the relative route to be expected in the request.
    /// </summary>
    /// <param name="route">The route to be expected.</param>
    /// <exception cref="ArgumentException">Thrown if the route parameter is null or empty.</exception>
    /// <returns>Itself for the fluent style.</returns>
    public MockHttpMessageHandler WithRoute(string route)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(route);
        _route = route;
        return this;
    }
}
