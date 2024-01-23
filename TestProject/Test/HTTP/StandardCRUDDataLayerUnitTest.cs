using JMayer.Data.Data;
using JMayer.Data.Data.Query;
using JMayer.Data.HTTP.DataLayer;
using JMayer.Data.HTTP.Handler;
using System.Net;
using System.Text.Json;
using TestProject.Data;
using TestProject.HTTP;

namespace TestProject.Test.HTTP;

/// <summary>
/// The class manages tests for the HTTP Standard CRUD DataLayer object.
/// </summary>
/// <remarks>
/// The tests are against a SimpleDataLayer object which inherits from the StandardCRUDDataLayer and
/// the SimpleDataLayer doesn't override any of the base methods. Because of this, we're testing 
/// the methods in the StandardCRUDDataLayer class.
/// </remarks>
public class StandardCRUDDataLayerUnitTest
{
    /// <summary>
    /// The method confirms the DataLayer.CountAsync() works as intended.
    /// </summary>
    /// <param name="httpStatusCode">The HTTP status code to test against.</param>
    /// <returns>A Task object for the async.</returns>
    [Theory]
    [InlineData(HttpStatusCode.OK)]
    [InlineData(HttpStatusCode.NoContent)]
    [InlineData(HttpStatusCode.Unauthorized)]
    public async Task CountAsyncWorks(HttpStatusCode httpStatusCode)
    {
        int respondingCount = 10;

        HttpClient httpClient = new MockHttpMessageHandler()
            .WithRoute($"api/{nameof(SimpleDataObject)}/Count")
            .RespondingHttpStatusCode(httpStatusCode)
            .RespondingStringContent(respondingCount)
            .Build();

        SimpleDataLayer dataLayer = new(httpClient);
        int returnedCount = await dataLayer.CountAsync();

        //With positive compared against the expected response.
        if (httpStatusCode == HttpStatusCode.OK)
        {
            Assert.Equal(respondingCount, returnedCount);
        }
        //With negative compared against 0.
        else
        {
            Assert.Equal(0, returnedCount);
        }
    }

    /// <summary>
    /// The method confirms if a null data object is passed to the StandardCRUDDataLayer.CreateAsync(), an exception is thrown.
    /// </summary>
    /// <returns>A Task object for the async.</returns>
    [Fact]
    public async Task CreateAsyncThrowsArgumentNullException() => await Assert.ThrowsAsync<ArgumentNullException>(() => new SimpleDataLayer().CreateAsync(null));

    /// <summary>
    /// The method confirms if StandardCRUDDataLayer.CreateAsync() receives an empty string in the response, an exception is thrown.
    /// </summary>
    /// <param name="httpStatusCode">The status to test against.</param>
    /// <returns>A Task object for the async.</returns>
    /// <remarks>
    /// Only 200 and 400 codes attempts to parse json so only do an InlineData with them.
    /// </remarks>
    [Theory]
    [InlineData(HttpStatusCode.OK)]
    [InlineData(HttpStatusCode.BadRequest)]
    public async Task CreateAsyncThrowsJsonException(HttpStatusCode httpStatusCode)
    {
        SimpleDataObject requestDataObject = new()
        {
            Integer64ID = 1,
            Value = 10,
        };

        HttpClient httpClient;
        httpClient = new MockHttpMessageHandler()
            .WithJsonContent(requestDataObject)
            .WithMethod(HttpMethod.Post)
            .WithRoute($"api/{nameof(SimpleDataObject)}")
            .RespondingHttpStatusCode(httpStatusCode)
            .Build();

        await Assert.ThrowsAsync<JsonException>(() => new SimpleDataLayer(httpClient).CreateAsync(requestDataObject));
    }

    /// <summary>
    /// The method confirms the StandardCRUDDataLayer.CreateAsync() works as intended.
    /// </summary>
    /// <param name="httpStatusCode">The status to test against.</param>
    /// <returns>A Task object for the async.</returns>
    [Theory]
    [InlineData(HttpStatusCode.OK)]
    [InlineData(HttpStatusCode.NoContent)]
    [InlineData(HttpStatusCode.Unauthorized)]
    [InlineData(HttpStatusCode.BadRequest)]
    public async Task CreateAsyncWorks(HttpStatusCode httpStatusCode)
    {
        //The data object sent by the request will have no ID but the data object returned in the response will.
        SimpleDataObject requestDataObject = new()
        {
            Value = 10,
        };
        SimpleDataObject respondingDataObject = new(requestDataObject)
        {
            Integer64ID = 1,
        };
        ServerSideValidationResult respondingValidationResult = new()
        {
            Errors = 
            [
                new ServerSideValidationError()
                {
                    ErrorMessage = "The value is not unique.",
                    PropertyName = nameof(SimpleDataObject.Value),
                }
            ]
        };

        HttpClient httpClient;

        if (httpStatusCode == HttpStatusCode.BadRequest)
        {
            httpClient = new MockHttpMessageHandler()
                .WithJsonContent(requestDataObject)
                .WithMethod(HttpMethod.Post)
                .WithRoute($"api/{nameof(SimpleDataObject)}")
                .RespondingHttpStatusCode(httpStatusCode)
                .RespondingJsonContent(respondingValidationResult)
                .Build();
        }
        else
        {
            httpClient = new MockHttpMessageHandler()
                .WithJsonContent(requestDataObject)
                .WithMethod(HttpMethod.Post)
                .WithRoute($"api/{nameof(SimpleDataObject)}")
                .RespondingHttpStatusCode(httpStatusCode)
                .RespondingJsonContent(respondingDataObject)
                .Build();
        }

        SimpleDataLayer dataLayer = new(httpClient);
        OperationResult operationResult = await dataLayer.CreateAsync(requestDataObject);

        //With positive compared against the expected data object response.
        if (httpStatusCode == HttpStatusCode.OK)
        {
            Assert.True
            (
                operationResult.IsSuccessStatusCode //Confirm a positive response.
                && operationResult.DataObject is SimpleDataObject returnedDataObject //Confirm a data object was returned.
                && returnedDataObject.Integer64ID == respondingDataObject.Integer64ID && returnedDataObject.Value == respondingDataObject.Value //Confirmed json serialization/deserialation worked.
            );
        }
        //With validation errors confirm the negative response and the expected validation error response.
        else if (httpStatusCode == HttpStatusCode.BadRequest)
        {
            Assert.True
            (
                !operationResult.IsSuccessStatusCode && operationResult.StatusCode == httpStatusCode //Confirm the negative response.
                && operationResult.DataObject == null //Confirm a data object wasn't the response
                && operationResult.ServerSideValidationResult != null && !operationResult.ServerSideValidationResult.IsSuccess //Confirm the validation result was the response and it wasn't successful.
                && operationResult.ServerSideValidationResult.Errors.Count == respondingValidationResult.Errors.Count //Confirm json serialization/deserialization worked.
                && operationResult.ServerSideValidationResult.Errors[0].ErrorMessage == respondingValidationResult.Errors[0].ErrorMessage && operationResult.ServerSideValidationResult.Errors[0].PropertyName == respondingValidationResult.Errors[0].PropertyName //Confirm json serialization/deserialization worked.
            );
        }
        //With other negatives confirm the negative response and no json data object was returned.
        else
        {
            Assert.True(!operationResult.IsSuccessStatusCode && operationResult.StatusCode == httpStatusCode && operationResult.DataObject == null && operationResult.ServerSideValidationResult == null);
        }
    }

    /// <summary>
    /// The method confirms if a null data object is passed to the StandardCRUDDataLayer.DeleteAsync(), an exception is thrown.
    /// </summary>
    /// <returns>A Task object for the async.</returns>
    [Fact]
    public async Task DeleteAsyncThrowsArgumentNullException() => await Assert.ThrowsAsync<ArgumentNullException>(() => new SimpleDataLayer().DeleteAsync(null));

    /// <summary>
    /// The method confirms the StandardCRUDDataLayer.DeleteAsync() works as intended.
    /// </summary>
    /// <returns>A Task object for the async.</returns>
    [Theory]
    [InlineData(HttpStatusCode.OK)]
    [InlineData(HttpStatusCode.NoContent)]
    [InlineData(HttpStatusCode.Unauthorized)]
    public async Task DeleteAsyncWorks(HttpStatusCode httpStatusCode)
    {
        long id = 1;

        HttpClient httpClient = new MockHttpMessageHandler()
            .WithMethod(HttpMethod.Delete)
            .WithRoute($"api/{nameof(SimpleDataObject)}")
            .WithRouteParameters([id.ToString()])
            .RespondingHttpStatusCode(httpStatusCode)
            .Build();

        SimpleDataLayer dataLayer = new(httpClient);
        OperationResult operationResult = await dataLayer.DeleteAsync(new SimpleDataObject() { Integer64ID = id });

        //Confirm there's only a status code.
        Assert.True(operationResult.StatusCode == httpStatusCode && operationResult.DataObject == null && operationResult.ServerSideValidationResult == null);
    }

    /// <summary>
    /// The method confirms the StandardCRUDDataLayer.GetAllAsync() works as intended.
    /// </summary>
    /// <returns>A Task object for the async.</returns>
    [Theory]
    [InlineData(HttpStatusCode.OK)]
    [InlineData(HttpStatusCode.NoContent)]
    [InlineData(HttpStatusCode.Unauthorized)]
    public async Task GetAllAsyncWorks(HttpStatusCode httpStatusCode)
    {
        List<SimpleDataObject> respondingDataObjects =
        [
            new SimpleDataObject()
            {
                Integer64ID = 1,
                Value = 10,
            },
            new SimpleDataObject()
            {
                Integer64ID = 2,
                Value = 20,
            },
        ];

        HttpClient httpClient = new MockHttpMessageHandler()
            .WithRoute($"api/{nameof(SimpleDataObject)}/All")
            .RespondingHttpStatusCode(httpStatusCode)
            .RespondingJsonContent(respondingDataObjects)
            .Build();

        SimpleDataLayer dataLayer = new(httpClient);
        List<SimpleDataObject>? returnedDataObjects = await dataLayer.GetAllAsync();

        //With positive, confirm json data objects were returned.
        if (httpStatusCode == HttpStatusCode.OK)
        {
            Assert.True
            (
                returnedDataObjects != null //Must have responded with json.
                && returnedDataObjects.Count == respondingDataObjects.Count //Must have parsed the json correctly.
                && returnedDataObjects[0].Integer64ID == respondingDataObjects[0].Integer64ID && returnedDataObjects[0].Value == respondingDataObjects[0].Value //Must have parsed the json correctly.
                && returnedDataObjects[1].Integer64ID == respondingDataObjects[1].Integer64ID && returnedDataObjects[1].Value == respondingDataObjects[1].Value //Must have parsed the json correctly.
            );
        }
        //With negative, confirm no json data objects were returned.
        else
        {
            Assert.True(returnedDataObjects != null && returnedDataObjects.Count == 0);
        }
    }

    /// <summary>
    /// The method confirms if a null data object is passed to the StandardCRUDDataLayer.GetPageAsync(), an exception is thrown.
    /// </summary>
    /// <returns>A Task object for the async.</returns>
    [Fact]
    public async Task GetPageAsyncThrowsArgumentNullException() => await Assert.ThrowsAsync<ArgumentNullException>(() => new SimpleDataLayer().GetPageAsync(null));

    /// <summary>
    /// The method confirms the StandardCRUDDataLayer.GetPageAsync() works as intended.
    /// </summary>
    /// <returns>A Task object for the async.</returns>
    [Theory]
    [InlineData(HttpStatusCode.OK)]
    [InlineData(HttpStatusCode.NoContent)]
    [InlineData(HttpStatusCode.Unauthorized)]
    public async Task GetPageAsyncWorks(HttpStatusCode httpStatusCode)
    {
        QueryDefinition queryDefinition = new()
        {
            FilterDefinitions =
            [
                new FilterDefinition() 
                { 
                    FilterOn = nameof(SimpleDataObject.Value),
                    Operator = FilterDefinition.ContainsOperator,
                    Value = "1",
                }
            ],
            Skip = 1,
            SortDefinitions = 
            [
                new SortDefinition()
                {
                    Descending = false,
                    SortOn = nameof(SimpleDataObject.Value),
                }
            ],
            Take = 20,
        };

        PagedList<SimpleDataObject> respondingPage = new()
        {
            DataObjects =
            [
                new SimpleDataObject()
                {
                    Integer64ID = 1,
                    Value = 1,
                },
                new SimpleDataObject()
                {
                    Integer64ID = 10,
                    Value = 10,
                },
                new SimpleDataObject()
                {
                    Integer64ID = 100,
                    Value = 100,
                },
            ],
            TotalRecords = 3,
        };

        Dictionary<string, string> queryString = [];
        queryString.Add(nameof(queryDefinition.Skip), queryDefinition.Skip.ToString());
        queryString.Add(nameof(queryDefinition.Take), queryDefinition.Take.ToString());
        queryString.Add($"{nameof(queryDefinition.FilterDefinitions)}[0].{nameof(FilterDefinition.FilterOn)}", queryDefinition.FilterDefinitions[0].FilterOn);
        queryString.Add($"{nameof(queryDefinition.FilterDefinitions)}[0].{nameof(FilterDefinition.Operator)}", queryDefinition.FilterDefinitions[0].Operator);
        queryString.Add($"{nameof(queryDefinition.FilterDefinitions)}[0].{nameof(FilterDefinition.Value)}", queryDefinition.FilterDefinitions[0].Value);
        queryString.Add($"{nameof(queryDefinition.SortDefinitions)}[0].{nameof(SortDefinition.Descending)}", queryDefinition.SortDefinitions[0].Descending.ToString());
        queryString.Add($"{nameof(queryDefinition.SortDefinitions)}[0].{nameof(SortDefinition.SortOn)}", queryDefinition.SortDefinitions[0].SortOn);

        HttpClient httpClient = new MockHttpMessageHandler()
            .WithRoute($"api/{nameof(SimpleDataObject)}/Page")
            .WithQueryString(queryString)
            .RespondingHttpStatusCode(httpStatusCode)
            .RespondingJsonContent(respondingPage)
            .Build();

        SimpleDataLayer dataLayer = new(httpClient);
        PagedList<SimpleDataObject>? returnedPage = await dataLayer.GetPageAsync(queryDefinition);

        //With positive, confirm json data objects were returned.
        if (httpStatusCode == HttpStatusCode.OK)
        {
            Assert.True
            (
                returnedPage != null //Must have responded with json.
                && returnedPage.DataObjects.Count == respondingPage.DataObjects.Count //Must have parsed the json correctly.
                && returnedPage.TotalRecords == respondingPage.TotalRecords //Must have parsed the json correctly.
                && returnedPage.DataObjects[0].Integer64ID == respondingPage.DataObjects[0].Integer64ID && returnedPage.DataObjects[0].Value == respondingPage.DataObjects[0].Value //Must have parsed the json correctly.
                && returnedPage.DataObjects[1].Integer64ID == respondingPage.DataObjects[1].Integer64ID && returnedPage.DataObjects[1].Value == respondingPage.DataObjects[1].Value //Must have parsed the json correctly.
                && returnedPage.DataObjects[2].Integer64ID == respondingPage.DataObjects[2].Integer64ID && returnedPage.DataObjects[2].Value == respondingPage.DataObjects[2].Value //Must have parsed the json correctly.
            );
        }
        //With negative, confirm no json data objects were returned.
        else
        {
            Assert.True(returnedPage != null && returnedPage.DataObjects.Count == 0 && returnedPage.TotalRecords == 0);
        }
    }

    /// <summary>
    /// The method confirms the StandardCRUDDataLayer.GetSingleAsync() works as intended.
    /// </summary>
    /// <returns>A Task object for the async.</returns>
    [Theory]
    [InlineData(HttpStatusCode.OK)]
    [InlineData(HttpStatusCode.NoContent)]
    [InlineData(HttpStatusCode.Unauthorized)]
    public async Task GetSingleAsyncIDWorks(HttpStatusCode httpStatusCode)
    {
        long id = 1;
        SimpleDataObject respondingDataObject = new()
        {
            Integer64ID = id,
            Value = 10,
        };

        HttpClient httpClient = new MockHttpMessageHandler()
            .WithRoute($"/api/{nameof(SimpleDataObject)}/Single")
            .WithRouteParameters([id.ToString()])
            .RespondingHttpStatusCode(httpStatusCode)
            .RespondingJsonContent(respondingDataObject)
            .Build();

        SimpleDataLayer dataLayer = new(httpClient);
        SimpleDataObject? returnedDataObject = await dataLayer.GetSingleAsync(id.ToString());

        //With positive, confirm json data object was returned.
        if (httpStatusCode == HttpStatusCode.OK)
        {
            Assert.True
            (
                returnedDataObject != null //Must have responded with json.
                && respondingDataObject.Integer64ID == returnedDataObject.Integer64ID //Confirmed json serialization/deserialation worked.
                && respondingDataObject.Value == returnedDataObject.Value //Confirmed json serialization/deserialation worked.
            );
        }
        //With negative, confirm no json data object was returned.
        else
        {
            Assert.True(returnedDataObject == null);
        }
    }

    /// <summary>
    /// The method confirms the StandardCRUDDataLayer.GetSingleAsync() works as intended.
    /// </summary>
    /// <returns>A Task object for the async.</returns>
    [Theory]
    [InlineData(HttpStatusCode.OK)]
    [InlineData(HttpStatusCode.NoContent)]
    [InlineData(HttpStatusCode.Unauthorized)]
    public async Task GetSingleAsyncWorks(HttpStatusCode httpStatusCode)
    {
        SimpleDataObject respondingDataObject = new()
        {
            Integer64ID = 1,
            Value = 10,
        };

        HttpClient httpClient = new MockHttpMessageHandler()
            .WithRoute($"/api/{nameof(SimpleDataObject)}/Single")
            .RespondingHttpStatusCode(httpStatusCode)
            .RespondingJsonContent(respondingDataObject)
            .Build();

        SimpleDataLayer dataLayer = new(httpClient);
        SimpleDataObject? returnedDataObject = await dataLayer.GetSingleAsync();

        //With positive, confirm json data object was returned.
        if (httpStatusCode == HttpStatusCode.OK)
        {
            Assert.True
            (
                returnedDataObject != null //Must have responded with json.
                && respondingDataObject.Integer64ID == returnedDataObject.Integer64ID //Must have parsed the json correctly.
                && respondingDataObject.Value == returnedDataObject.Value //Must have parsed the json correctly.
            );
        }
        //With negative, confirm no json data object was returned.
        else
        {
            Assert.True(returnedDataObject == null);
        }
    }

    /// <summary>
    /// The method confirms if a null data object is passed to the StandardCRUDDataLayer.UpdateAsync(), an exception is thrown.
    /// </summary>
    /// <returns>A Task object for the async.</returns>
    [Fact]
    public async Task UpdateAsyncThrowsArgumentNullException() => await Assert.ThrowsAsync<ArgumentNullException>(() => new SimpleDataLayer().UpdateAsync(null));

    /// <summary>
    /// The method confirms if StandardCRUDDataLayer.UpdateAsync() receives an empty string in the response, an exception is thrown.
    /// </summary>
    /// <param name="httpStatusCode">The status to test against.</param>
    /// <returns>A Task object for the async.</returns>
    /// <remarks>
    /// Only 200 and 400 codes attempts to parse json so only do an InlineData with them.
    /// </remarks>
    [Theory]
    [InlineData(HttpStatusCode.OK)]
    [InlineData(HttpStatusCode.BadRequest)]
    public async Task UpdateAsyncThrowsJsonException(HttpStatusCode httpStatusCode)
    {
        SimpleDataObject requestDataObject = new()
        {
            Integer64ID = 1,
            Value = 10,
        };

        HttpClient httpClient;
        httpClient = new MockHttpMessageHandler()
            .WithJsonContent(requestDataObject)
            .WithMethod(HttpMethod.Put)
            .WithRoute($"api/{nameof(SimpleDataObject)}")
            .RespondingHttpStatusCode(httpStatusCode)
            .Build();

        await Assert.ThrowsAsync<JsonException>(() => new SimpleDataLayer(httpClient).UpdateAsync(requestDataObject));
    }

    /// <summary>
    /// The method confirms the StandardCRUDDataLayer.UpdateAsync() works as intended.
    /// </summary>
    /// <param name="httpStatusCode">The status to test against.</param>
    /// <returns>A Task object for the async.</returns>
    [Theory]
    [InlineData(HttpStatusCode.OK)]
    [InlineData(HttpStatusCode.NoContent)]
    [InlineData(HttpStatusCode.Unauthorized)]
    [InlineData(HttpStatusCode.BadRequest)]
    public async Task UpdateAsyncWorks(HttpStatusCode httpStatusCode)
    {
        //Nothing changes between the request and responding data objects.
        SimpleDataObject requestDataObject = new()
        {
            Integer64ID = 1,
            Value = 10,
        };
        SimpleDataObject respondingDataObject = new(requestDataObject);
        ServerSideValidationResult respondingValidationResult = new()
        {
            Errors =
            [
                new ServerSideValidationError()
                {
                    ErrorMessage = "The value is not unique.",
                    PropertyName = nameof(SimpleDataObject.Value),
                }
            ]
        };

        HttpClient httpClient;

        if (httpStatusCode == HttpStatusCode.BadRequest)
        {
            httpClient = new MockHttpMessageHandler()
                .WithJsonContent(requestDataObject)
                .WithMethod(HttpMethod.Put)
                .WithRoute($"api/{nameof(SimpleDataObject)}")
                .RespondingHttpStatusCode(httpStatusCode)
                .RespondingJsonContent(respondingValidationResult)
                .Build();
        }
        else
        {
            httpClient = new MockHttpMessageHandler()
                .WithJsonContent(requestDataObject)
                .WithMethod(HttpMethod.Put)
                .WithRoute($"api/{nameof(SimpleDataObject)}")
                .RespondingHttpStatusCode(httpStatusCode)
                .RespondingJsonContent(respondingDataObject)
                .Build();
        }

        SimpleDataLayer dataLayer = new(httpClient);
        OperationResult operationResult = await dataLayer.UpdateAsync(requestDataObject);

        //With positive compared against the expected response.
        if (httpStatusCode == HttpStatusCode.OK)
        {
            Assert.True
            (
                operationResult.IsSuccessStatusCode //Confirm a positive response.
                && operationResult.DataObject is SimpleDataObject returnedDataObject //Confirm a data object was returned.
                && returnedDataObject.Integer64ID == respondingDataObject.Integer64ID && returnedDataObject.Value == respondingDataObject.Value //Confirmed json serialization/deserialation worked.
            );
        }
        //With validation errors confirm the negative response and the expected validation error response.
        else if (httpStatusCode == HttpStatusCode.BadRequest)
        {
            Assert.True
            (
                !operationResult.IsSuccessStatusCode && operationResult.StatusCode == httpStatusCode //Confirm the negative response.
                && operationResult.DataObject == null //Confirm a data object wasn't the response
                && operationResult.ServerSideValidationResult != null && !operationResult.ServerSideValidationResult.IsSuccess //Confirm the validation result was the response and it wasn't successful.
                && operationResult.ServerSideValidationResult.Errors.Count == respondingValidationResult.Errors.Count //Confirm json serialization/deserialization worked.
                && operationResult.ServerSideValidationResult.Errors[0].ErrorMessage == respondingValidationResult.Errors[0].ErrorMessage && operationResult.ServerSideValidationResult.Errors[0].PropertyName == respondingValidationResult.Errors[0].PropertyName //Confirm json serialization/deserialization worked.
            );
        }
        //With other negatives confirm the negative response and no json data object was returned.
        else
        {
            Assert.True(!operationResult.IsSuccessStatusCode && operationResult.StatusCode == httpStatusCode && operationResult.DataObject == null && operationResult.ServerSideValidationResult == null);
        }
    }

    /// <summary>
    /// The method confirms if a null data object is passed to the StandardCRUDDataLayer.ValidationAsync(), an exception is thrown.
    /// </summary>
    /// <returns>A Task object for the async.</returns>
    [Fact]
    public async Task ValidateAsyncThrowsArgumentNullException() => await Assert.ThrowsAsync<ArgumentNullException>(() => new SimpleDataLayer().ValidationAsync(null));

    /// <summary>
    /// The method confirms the StandardCRUDDataLayer.UpdateAsync() works as intended.
    /// </summary>
    /// <returns>A Task object for the async.</returns>
    [Theory]
    [InlineData(HttpStatusCode.OK)]
    [InlineData(HttpStatusCode.NoContent)]
    [InlineData(HttpStatusCode.Unauthorized)]
    public async Task ValidateAsyncWorks(HttpStatusCode httpStatusCode)
    {
        SimpleDataObject requestDataObject = new()
        {
            Integer64ID = 1,
            Value = 10,
        };
        ServerSideValidationResult respondingServerSideValidationResult = new();

        HttpClient httpClient = new MockHttpMessageHandler()
            .WithJsonContent(requestDataObject)
            .WithMethod(HttpMethod.Post)
            .WithRoute($"api/{nameof(SimpleDataObject)}/Validate")
            .RespondingHttpStatusCode(httpStatusCode)
            .RespondingJsonContent(respondingServerSideValidationResult)
            .Build();

        SimpleDataLayer dataLayer = new(httpClient);
        ServerSideValidationResult? returnedServerSideValidationResult = await dataLayer.ValidationAsync(requestDataObject);

        //With positive compared against the expected response.
        if (httpStatusCode == HttpStatusCode.OK)
        {
            Assert.True
            (
                returnedServerSideValidationResult != null
                && returnedServerSideValidationResult.IsSuccess == respondingServerSideValidationResult.IsSuccess
                && returnedServerSideValidationResult.Errors.Count == respondingServerSideValidationResult.Errors.Count
            );
        }
        //With negative confirm the negative response and no json data object was returned.
        else
        {
            Assert.True(returnedServerSideValidationResult == null);
        }
    }
}
