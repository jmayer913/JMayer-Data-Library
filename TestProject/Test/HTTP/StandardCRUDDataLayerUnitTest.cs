using JMayer.Data.Data;
using JMayer.Data.Data.Query;
using JMayer.Data.HTTP.DataLayer;
using JMayer.Data.HTTP.Details;
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
    /// The constant for the default id.
    /// </summary>
    private const long DefaultId = 1;

    /// <summary>
    /// The constant for the default value.
    /// </summary>
    private const int DefaultValue = 10;

    /// <summary>
    /// The method verifies the base address can have a path.
    /// </summary>
    /// <param name="httpStatusCode">The HTTP status code to test against.</param>
    /// <returns>A Task object for the async.</returns>
    [Theory]
    [InlineData(HttpStatusCode.OK)]
    [InlineData(HttpStatusCode.NoContent)]
    [InlineData(HttpStatusCode.Unauthorized)]
    public async Task VerifyBaseAddressWithPath(HttpStatusCode httpStatusCode)
    {
        List<SimpleDataObject> respondingDataObjects =
        [
            new SimpleDataObject()
            {
                Integer64ID = DefaultId,
                Value = DefaultValue,
            },
            new SimpleDataObject()
            {
                Integer64ID = 2,
                Value = 10,
            },
        ];

        HttpClient httpClient = new MockHttpMessageHandler()
            .WithBaseAddress(new Uri("http://localhost/Some/Extra/Path"))
            .WithRoute($"api/{nameof(SimpleDataObject)}/All")
            .RespondingHttpStatusCode(httpStatusCode)
            .RespondingJsonContent(respondingDataObjects)
            .Build();

        SimpleDataLayer dataLayer = new(httpClient);
        List<SimpleDataObject>? returnedDataObjects = await dataLayer.GetAllAsync();

        //With positive, confirm json data objects were returned.
        if (httpStatusCode is HttpStatusCode.OK)
        {
            Assert.NotNull(returnedDataObjects); //Must have responded with json.
            Assert.Equal(respondingDataObjects.Count, returnedDataObjects.Count); //Must have parsed the json correctly.

            for (int index = 0; index < returnedDataObjects.Count; index++)
            {
                Assert.Equal(respondingDataObjects[index].Integer64ID, returnedDataObjects[index].Integer64ID); //Must have parsed the json correctly.
                Assert.Equal(respondingDataObjects[index].Value, returnedDataObjects[index].Value); //Must have parsed the json correctly.
            }
        }
        //With negative, confirm no json data objects were returned.
        else
        {
            Assert.NotNull(returnedDataObjects);
            Assert.Empty(returnedDataObjects);
        }
    }

    /// <summary>
    /// The method verifies the DataLayer.CountAsync() request and response based on the status code.
    /// </summary>
    /// <param name="httpStatusCode">The HTTP status code to test against.</param>
    /// <returns>A Task object for the async.</returns>
    [Theory]
    [InlineData(HttpStatusCode.OK)]
    [InlineData(HttpStatusCode.NoContent)]
    [InlineData(HttpStatusCode.Unauthorized)]
    public async Task VerifyCount(HttpStatusCode httpStatusCode)
    {
        long respondingCount = 10;

        HttpClient httpClient = new MockHttpMessageHandler()
            .WithRoute($"api/{nameof(SimpleDataObject)}/Count")
            .RespondingHttpStatusCode(httpStatusCode)
            .RespondingStringContent(respondingCount)
            .Build();

        SimpleDataLayer dataLayer = new(httpClient);
        long returnedCount = await dataLayer.CountAsync();

        //With positive compared against the expected response.
        if (httpStatusCode is HttpStatusCode.OK)
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
    /// The method verifies the StandardCRUDDataLayer.CreateAsync() request and response based on the status code.
    /// </summary>
    /// <param name="httpStatusCode">The status to test against.</param>
    /// <returns>A Task object for the async.</returns>
    [Theory]
    [InlineData(HttpStatusCode.OK)]
    [InlineData(HttpStatusCode.BadRequest)]
    [InlineData(HttpStatusCode.InternalServerError)]
    [InlineData(HttpStatusCode.NoContent)]
    [InlineData(HttpStatusCode.Unauthorized)]
    public async Task VerifyCreate(HttpStatusCode httpStatusCode)
    {
        //The data object sent by the request will have no ID but the data object returned in the response will.
        SimpleDataObject requestDataObject = new()
        {
            Value = DefaultValue,
        };
        SimpleDataObject respondingDataObject = new(requestDataObject)
        {
            Integer64ID = DefaultId,
        };
        
        ProblemDetails responding500ProblemDetails = new(detail: $"Failed to create the {typeof(SimpleDataObject).Name} record because of an error on the server.");
        ValidationProblemDetails responding400ProblemDetails = new()
        {
            Detail = "One or more validation errors occurred.",
            Errors = 
            {
                { nameof(SimpleDataObject.Name), ["The name is not unique."] }
            }
        };

        HttpClient httpClient;

        if (httpStatusCode is HttpStatusCode.BadRequest)
        {
            httpClient = new MockHttpMessageHandler()
                .WithJsonContent(requestDataObject)
                .WithMethod(HttpMethod.Post)
                .WithRoute($"api/{nameof(SimpleDataObject)}")
                .RespondingHttpStatusCode(httpStatusCode)
                .RespondingJsonContent(responding400ProblemDetails)
                .Build();
        }
        else if (httpStatusCode is HttpStatusCode.InternalServerError)
        {
            httpClient = new MockHttpMessageHandler()
                .WithJsonContent(requestDataObject)
                .WithMethod(HttpMethod.Post)
                .WithRoute($"api/{nameof(SimpleDataObject)}")
                .RespondingHttpStatusCode(httpStatusCode)
                .RespondingJsonContent(responding500ProblemDetails)
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

        //With positive, compared against the expected data object response.
        if (httpStatusCode is HttpStatusCode.OK)
        {
            Assert.True(operationResult.IsSuccessStatusCode, "The IsSuccessStatusCode should have been true."); //Confirm a positive response.
            Assert.Null(operationResult.ProblemDetails); //Confirm there is no problem details.
            Assert.Empty(operationResult.ValidationErrors); //Confirm the server side validation result wan't the response.
            Assert.IsType<SimpleDataObject>(operationResult.DataObject); //Confirm a data object was returned.
            Assert.Equal(respondingDataObject.Integer64ID, ((SimpleDataObject)operationResult.DataObject).Integer64ID); //Confirm responding data object match data object in the operational result.
            Assert.Equal(respondingDataObject.Value, ((SimpleDataObject)operationResult.DataObject).Value); //Confirm responding data object match data object in the operational result.
        }
        //With validation errors, confirm the negative response and the expected validation error response.
        else if (httpStatusCode is HttpStatusCode.BadRequest)
        {
            Assert.False(operationResult.IsSuccessStatusCode, "The IsSuccessStatusCode should have been false."); //Confirm the negative response.
            Assert.Equal(httpStatusCode, operationResult.StatusCode); //Confirm the negative response.
            Assert.Null(operationResult.DataObject); //Confirm a data object wasn't the response
            Assert.Equal(responding400ProblemDetails.Detail, operationResult.ProblemDetails); //Confirm the details match.
            Assert.Contains(operationResult.ValidationErrors, obj => obj.Key == nameof(SimpleDataObject.Name)); //Confirm there's an error for the Name property.
            Assert.Single(operationResult.ValidationErrors[nameof(SimpleDataObject.Name)]); //Confirm the Name property has one error.
            Assert.Equal(responding400ProblemDetails.Errors[nameof(SimpleDataObject.Name)][0], operationResult.ValidationErrors[nameof(SimpleDataObject.Name)][0]); //Confirm json serialization/deserialization worked.
        }
        //With an internal server error, confirm only the status and problem details was set.
        else if (httpStatusCode is HttpStatusCode.InternalServerError)
        {
            Assert.False(operationResult.IsSuccessStatusCode, "The IsSuccessStatusCode should have been false."); //Confirm the negative response.
            Assert.Equal(httpStatusCode, operationResult.StatusCode); //Confirm the negative response.
            Assert.Null(operationResult.DataObject); //Confirm a data object wasn't the response
            Assert.Equal(responding500ProblemDetails.Detail, operationResult.ProblemDetails); //Confirm the details match.
            Assert.Empty(operationResult.ValidationErrors); //Confirm there are no validation errors.
        }
        //With other negative responses, confirm only the status was set.
        else
        {
            Assert.False(operationResult.IsSuccessStatusCode, "The IsSuccessStatusCode should have been false."); //Confirm the negative response.
            Assert.Equal(httpStatusCode, operationResult.StatusCode); //Confirm the negative response.
            Assert.Null(operationResult.DataObject); //Confirm a data object wasn't the response
            Assert.Null(operationResult.ProblemDetails); //Confirm there is no problem details.
            Assert.Empty(operationResult.ValidationErrors); //Confirm the server side validation result wan't the response.
        }
    }

    /// <summary>
    /// The method verifies if a null data object is passed to the StandardCRUDDataLayer.CreateAsync(), an exception is thrown.
    /// </summary>
    /// <returns>A Task object for the async.</returns>
    [Fact]
    public async Task VerifyCreateThrowsArgumentNullException() => await Assert.ThrowsAsync<ArgumentNullException>(() => new SimpleDataLayer().CreateAsync(null!));

    /// <summary>
    /// The method verifies if StandardCRUDDataLayer.CreateAsync() receives an empty string in the response, an exception is thrown.
    /// </summary>
    /// <param name="httpStatusCode">The status to test against.</param>
    /// <returns>A Task object for the async.</returns>
    /// <remarks>
    /// Only 200 and 400 codes attempts to parse json so only do an InlineData with them.
    /// </remarks>
    [Theory]
    [InlineData(HttpStatusCode.OK)]
    [InlineData(HttpStatusCode.BadRequest)]
    public async Task VerifyCreateThrowsJsonException(HttpStatusCode httpStatusCode)
    {
        SimpleDataObject requestDataObject = new()
        {
            Integer64ID = DefaultId,
            Value = DefaultValue,
        };

        HttpClient httpClient = new MockHttpMessageHandler()
            .WithJsonContent(requestDataObject)
            .WithMethod(HttpMethod.Post)
            .WithRoute($"api/{nameof(SimpleDataObject)}")
            .RespondingHttpStatusCode(httpStatusCode)
            .Build();

        await Assert.ThrowsAsync<JsonException>(() => new SimpleDataLayer(httpClient).CreateAsync(requestDataObject));
    }

    /// <summary>
    /// The method verifies the StandardCRUDDataLayer.DeleteAsync() request and response based on the status code.
    /// </summary>
    /// <param name="httpStatusCode">The HTTP status code to test against.</param>
    /// <returns>A Task object for the async.</returns>
    [Theory]
    [InlineData(HttpStatusCode.OK)]
    [InlineData(HttpStatusCode.Conflict)]
    [InlineData(HttpStatusCode.InternalServerError)]
    [InlineData(HttpStatusCode.NoContent)]
    [InlineData(HttpStatusCode.NotFound)]
    [InlineData(HttpStatusCode.Unauthorized)]
    public async Task VerifyDelete(HttpStatusCode httpStatusCode)
    {
        long id = DefaultId;
        ProblemDetails responding404ProblemDetails = new(detail: $"The {typeof(SimpleDataObject).Name} record was not found; please refresh the page because another user may have deleted it.");
        ProblemDetails responding409ProblemDetails = new(detail: $"The {typeof(SimpleDataObject).Name} record has a dependency that prevents it from being deleted; the dependency needs to be deleted first.");
        ProblemDetails responding500ProblemDetails = new(detail: $"Failed to delete the {typeof(SimpleDataObject).Name} record because of an error on the server.");

        HttpClient httpClient;

        if (httpStatusCode is HttpStatusCode.Conflict)
        {
            httpClient = new MockHttpMessageHandler()
                .WithMethod(HttpMethod.Delete)
                .WithRoute($"api/{nameof(SimpleDataObject)}")
                .WithRouteParameters([id.ToString()])
                .RespondingHttpStatusCode(httpStatusCode)
                .RespondingJsonContent(responding409ProblemDetails)
                .Build();
        }
        else if (httpStatusCode is HttpStatusCode.InternalServerError)
        {
            httpClient = new MockHttpMessageHandler()
                .WithMethod(HttpMethod.Delete)
                .WithRoute($"api/{nameof(SimpleDataObject)}")
                .WithRouteParameters([id.ToString()])
                .RespondingHttpStatusCode(httpStatusCode)
                .RespondingJsonContent(responding500ProblemDetails)
                .Build();
        }
        else if (httpStatusCode is HttpStatusCode.NotFound)
        {
            httpClient = new MockHttpMessageHandler()
                .WithMethod(HttpMethod.Delete)
                .WithRoute($"api/{nameof(SimpleDataObject)}")
                .WithRouteParameters([id.ToString()])
                .RespondingHttpStatusCode(httpStatusCode)
                .RespondingJsonContent(responding404ProblemDetails)
                .Build();
        }
        else
        {
            httpClient = new MockHttpMessageHandler()
                .WithMethod(HttpMethod.Delete)
                .WithRoute($"api/{nameof(SimpleDataObject)}")
                .WithRouteParameters([id.ToString()])
                .RespondingHttpStatusCode(httpStatusCode)
                .Build();
        }

        SimpleDataLayer dataLayer = new(httpClient);
        OperationResult operationResult = await dataLayer.DeleteAsync(new SimpleDataObject() { Integer64ID = id });

        //With positive, compared against the expected response.
        if (httpStatusCode is HttpStatusCode.OK)
        {
            Assert.True(operationResult.IsSuccessStatusCode, "The IsSuccessStatusCode should have been true."); //Confirm a positive response.
            Assert.Null(operationResult.DataObject);
            Assert.Null(operationResult.ProblemDetails);
            Assert.Empty(operationResult.ValidationErrors);
        }
        //With a conflict, confirm only the status and problem details was set.
        else if (httpStatusCode is HttpStatusCode.Conflict)
        {
            Assert.False(operationResult.IsSuccessStatusCode, "The IsSuccessStatusCode should have been false."); //Confirm the negative response.
            Assert.Equal(httpStatusCode, operationResult.StatusCode); //Confirm the negative response.
            Assert.Null(operationResult.DataObject); //Confirm a data object wasn't the response
            Assert.Equal(responding409ProblemDetails.Detail, operationResult.ProblemDetails); //Confirm the details match.
            Assert.Empty(operationResult.ValidationErrors); //Confirm there are no validation errors.
        }
        //With an internal server error, confirm only the status and problem details was set.
        else if (httpStatusCode is HttpStatusCode.InternalServerError)
        {
            Assert.False(operationResult.IsSuccessStatusCode, "The IsSuccessStatusCode should have been false."); //Confirm the negative response.
            Assert.Equal(httpStatusCode, operationResult.StatusCode); //Confirm the negative response.
            Assert.Null(operationResult.DataObject); //Confirm a data object wasn't the response
            Assert.Equal(responding500ProblemDetails.Detail, operationResult.ProblemDetails); //Confirm the details match.
            Assert.Empty(operationResult.ValidationErrors); //Confirm there are no validation errors.
        }
        //With an internal server error, confirm only the status and problem details was set.
        else if (httpStatusCode is HttpStatusCode.NotFound)
        {
            Assert.False(operationResult.IsSuccessStatusCode, "The IsSuccessStatusCode should have been false."); //Confirm the negative response.
            Assert.Equal(httpStatusCode, operationResult.StatusCode); //Confirm the negative response.
            Assert.Null(operationResult.DataObject); //Confirm a data object wasn't the response
            Assert.Equal(responding404ProblemDetails.Detail, operationResult.ProblemDetails); //Confirm the details match.
            Assert.Empty(operationResult.ValidationErrors); //Confirm there are no validation errors.
        }
        //With other negative responses, confirm only the status was set.
        else
        {
            Assert.False(operationResult.IsSuccessStatusCode, "The IsSuccessStatusCode should have been false."); //Confirm the negative response.
            Assert.Equal(httpStatusCode, operationResult.StatusCode);
            Assert.Null(operationResult.DataObject);
            Assert.Null(operationResult.ProblemDetails);
            Assert.Empty(operationResult.ValidationErrors);
        }
    }

    /// <summary>
    /// The method verifies if a null data object is passed to the StandardCRUDDataLayer.DeleteAsync(), an exception is thrown.
    /// </summary>
    /// <returns>A Task object for the async.</returns>
    [Fact]
    public async Task VerifyDeleteThrowsArgumentNullException() => await Assert.ThrowsAsync<ArgumentNullException>(() => new SimpleDataLayer().DeleteAsync(null!));

    /// <summary>
    /// The method verifies the StandardCRUDDataLayer.GetAllAsync() request and response based on the status code.
    /// </summary>
    /// <param name="httpStatusCode">The HTTP status code to test against.</param>
    /// <returns>A Task object for the async.</returns>
    [Theory]
    [InlineData(HttpStatusCode.OK)]
    [InlineData(HttpStatusCode.NoContent)]
    [InlineData(HttpStatusCode.Unauthorized)]
    public async Task VerifyGetAll(HttpStatusCode httpStatusCode)
    {
        List<SimpleDataObject> respondingDataObjects =
        [
            new SimpleDataObject()
            {
                Integer64ID = DefaultId,
                Value = DefaultValue,
            },
            new SimpleDataObject()
            {
                Integer64ID = 2,
                Value = 10,
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
        if (httpStatusCode is HttpStatusCode.OK)
        {
            Assert.NotNull(returnedDataObjects); //Must have responded with json.
            Assert.Equal(respondingDataObjects.Count, returnedDataObjects.Count); //Must have parsed the json correctly.

            for (int index = 0; index < returnedDataObjects.Count; index++)
            {
                Assert.Equal(respondingDataObjects[index].Integer64ID, returnedDataObjects[index].Integer64ID); //Must have parsed the json correctly.
                Assert.Equal(respondingDataObjects[index].Value, returnedDataObjects[index].Value); //Must have parsed the json correctly.
            }
        }
        //With negative, confirm no json data objects were returned.
        else
        {
            Assert.NotNull(returnedDataObjects);
            Assert.Empty(returnedDataObjects);
        }
    }

    /// <summary>
    /// The method confirms the StandardCRUDDataLayer.GetAllListViewAsync() request and response based on the status code.
    /// </summary>
    /// <returns>A Task object for the async.</returns>
    [Theory]
    [InlineData(HttpStatusCode.OK)]
    [InlineData(HttpStatusCode.NoContent)]
    [InlineData(HttpStatusCode.Unauthorized)]
    public async Task VerifyGetAllListView(HttpStatusCode httpStatusCode)
    {
        List<ListView> respondingDataObjects =
        [
            new ListView()
            {
                Integer64ID = 1,
                Name = "10",
            },
            new ListView()
            {
                Integer64ID = 2,
                Name = "20",
            },
        ];

        HttpClient httpClient = new MockHttpMessageHandler()
            .WithRoute($"api/{nameof(SimpleDataObject)}/All/ListView")
            .RespondingHttpStatusCode(httpStatusCode)
            .RespondingJsonContent(respondingDataObjects)
            .Build();

        SimpleDataLayer dataLayer = new(httpClient);
        List<ListView>? returnedDataObjects = await dataLayer.GetAllListViewAsync();

        //With positive, confirm json data objects were returned.
        if (httpStatusCode is HttpStatusCode.OK)
        {
            Assert.NotNull(returnedDataObjects); //Must have responded with json.
            Assert.Equal(respondingDataObjects.Count, returnedDataObjects.Count); //Must have parsed the json correctly.

            for (int index = 0; index < returnedDataObjects.Count; index++)
            {
                Assert.Equal(respondingDataObjects[index].Integer64ID, returnedDataObjects[index].Integer64ID); //Must have parsed the json correctly.
                Assert.Equal(respondingDataObjects[index].Name, returnedDataObjects[index].Name); //Must have parsed the json correctly.
            }
        }
        //With negative, confirm no json data objects were returned.
        else
        {
            Assert.NotNull(returnedDataObjects);
            Assert.Empty(returnedDataObjects);
        }
    }

    /// <summary>
    /// The method verifies the StandardCRUDDataLayer.GetPageAsync() request and response based on the status code.
    /// </summary>
    /// <param name="httpStatusCode">The HTTP status code to test against.</param>
    /// <returns>A Task object for the async.</returns>
    [Theory]
    [InlineData(HttpStatusCode.OK)]
    [InlineData(HttpStatusCode.NoContent)]
    [InlineData(HttpStatusCode.Unauthorized)]
    public async Task VerifyGetPage(HttpStatusCode httpStatusCode)
    {
        QueryDefinition queryDefinition = new()
        {
            FilterDefinitions =
            [
                new FilterDefinition()
                {
                    FilterOn = nameof(SimpleDataObject.Value),
                    Operator = FilterDefinition.StringContainsOperator,
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
        if (httpStatusCode is HttpStatusCode.OK)
        {
            Assert.NotNull(returnedPage); //Must have responded with json.
            Assert.Equal(respondingPage.DataObjects.Count, returnedPage.DataObjects.Count); //Must have parsed the json correctly.
            Assert.Equal(respondingPage.TotalRecords, returnedPage.TotalRecords); //Must have parsed the json correctly.

            for (int index = 0; index < returnedPage.DataObjects.Count; index++)
            {
                Assert.Equal(respondingPage.DataObjects[index].Integer64ID, returnedPage.DataObjects[index].Integer64ID); //Must have parsed the json correctly.
                Assert.Equal(respondingPage.DataObjects[index].Value, returnedPage.DataObjects[index].Value); //Must have parsed the json correctly.
            }
        }
        //With negative, confirm no json data objects were returned.
        else
        {
            Assert.NotNull(returnedPage);
            Assert.Empty(returnedPage.DataObjects);
            Assert.Equal(0, returnedPage.TotalRecords);
        }
    }

    /// <summary>
    /// The method confirms the StandardCRUDDataLayer.GetPageListViewAsync() request and response based on the status code.
    /// </summary>
    /// <returns>A Task object for the async.</returns>
    [Theory]
    [InlineData(HttpStatusCode.OK)]
    [InlineData(HttpStatusCode.NoContent)]
    [InlineData(HttpStatusCode.Unauthorized)]
    public async Task VerifyGetPageListView(HttpStatusCode httpStatusCode)
    {
        QueryDefinition queryDefinition = new()
        {
            FilterDefinitions =
            [
                new FilterDefinition()
                {
                    FilterOn = nameof(SimpleDataObject.Value),
                    Operator = FilterDefinition.StringContainsOperator,
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

        PagedList<ListView> respondingPage = new()
        {
            DataObjects =
            [
                new ListView()
                {
                    Integer64ID = 1,
                    Name = "1",
                },
                new ListView()
                {
                    Integer64ID = 10,
                    Name = "10",
                },
                new ListView()
                {
                    Integer64ID = 100,
                    Name = "100",
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
            .WithRoute($"api/{nameof(SimpleDataObject)}/Page/ListView")
            .WithQueryString(queryString)
            .RespondingHttpStatusCode(httpStatusCode)
            .RespondingJsonContent(respondingPage)
            .Build();

        SimpleDataLayer dataLayer = new(httpClient);
        PagedList<ListView>? returnedPage = await dataLayer.GetPageListViewAsync(queryDefinition);

        //With positive, confirm json data objects were returned.
        if (httpStatusCode is HttpStatusCode.OK)
        {
            Assert.NotNull(returnedPage); //Must have responded with json.
            Assert.Equal(respondingPage.DataObjects.Count, returnedPage.DataObjects.Count); //Must have parsed the json correctly.

            for (int index = 0; index < returnedPage.DataObjects.Count; index++)
            {
                Assert.Equal(respondingPage.DataObjects[index].Integer64ID, returnedPage.DataObjects[index].Integer64ID); //Must have parsed the json correctly.
                Assert.Equal(respondingPage.DataObjects[index].Name, returnedPage.DataObjects[index].Name); //Must have parsed the json correctly.
            }
        }
        //With negative, confirm no json data objects were returned.
        else
        {
            Assert.NotNull(returnedPage);
            Assert.Empty(returnedPage.DataObjects);
            Assert.Equal(0, returnedPage.TotalRecords);
        }
    }

    /// <summary>
    /// The method verifies if a null query definition is passed to the StandardCRUDDataLayer.GetPageAsync(), an exception is thrown.
    /// </summary>
    /// <returns>A Task object for the async.</returns>
    [Fact]
    public async Task VerifyGetPageThrowsArgumentNullException() => await Assert.ThrowsAsync<ArgumentNullException>(() => new SimpleDataLayer().GetPageAsync(null!));

    /// <summary>
    /// The method confirms if a null query definition is passed to the StandardCRUDDataLayer.GetPageListViewAsync(), an exception is thrown.
    /// </summary>
    /// <returns>A Task object for the async.</returns>
    [Fact]
    public async Task VerifyGetPageListViewThrowsArgumentNullException() => await Assert.ThrowsAsync<ArgumentNullException>(() => new SimpleDataLayer().GetPageListViewAsync(null!));

    /// <summary>
    /// The method verifies the StandardCRUDDataLayer.GetSingleAsync() request and response based on the status code.
    /// </summary>
    /// <param name="httpStatusCode">The HTTP status code to test against.</param>
    /// <returns>A Task object for the async.</returns>
    [Theory]
    [InlineData(HttpStatusCode.OK)]
    [InlineData(HttpStatusCode.NoContent)]
    [InlineData(HttpStatusCode.Unauthorized)]
    public async Task VerifyGetSingle(HttpStatusCode httpStatusCode)
    {
        SimpleDataObject respondingDataObject = new()
        {
            Integer64ID = DefaultId,
            Value = DefaultValue,
        };

        HttpClient httpClient = new MockHttpMessageHandler()
            .WithRoute($"/api/{nameof(SimpleDataObject)}/Single")
            .RespondingHttpStatusCode(httpStatusCode)
            .RespondingJsonContent(respondingDataObject)
            .Build();

        SimpleDataLayer dataLayer = new(httpClient);
        SimpleDataObject? returnedDataObject = await dataLayer.GetSingleAsync();

        //With positive, confirm json data object was returned.
        if (httpStatusCode is HttpStatusCode.OK)
        {
            Assert.NotNull(returnedDataObject); //Must have responded with json.
            Assert.Equal(respondingDataObject.Integer64ID, returnedDataObject.Integer64ID); //Must have parsed the json correctly.
            Assert.Equal(respondingDataObject.Value, returnedDataObject.Value); //Must have parsed the json correctly.
        }
        //With negative, confirm no json data object was returned.
        else
        {
            Assert.Null(returnedDataObject);
        }
    }

    /// <summary>
    /// The method verifies the StandardCRUDDataLayer.GetSingleAsync() request and response based on the status code.
    /// </summary>
    /// <param name="httpStatusCode">The HTTP status code to test against.</param>
    /// <returns>A Task object for the async.</returns>
    [Theory]
    [InlineData(HttpStatusCode.OK)]
    [InlineData(HttpStatusCode.NoContent)]
    [InlineData(HttpStatusCode.Unauthorized)]
    public async Task VerifyGetSingleWithID(HttpStatusCode httpStatusCode)
    {
        long id = DefaultId;
        SimpleDataObject respondingDataObject = new()
        {
            Integer64ID = id,
            Value = DefaultValue,
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
        if (httpStatusCode is HttpStatusCode.OK)
        {
            Assert.NotNull(returnedDataObject); //Must have responded with json.
            Assert.Equal(respondingDataObject.Integer64ID, returnedDataObject.Integer64ID); //Confirmed json serialization/deserialation worked.
            Assert.Equal(respondingDataObject.Value, returnedDataObject.Value); //Confirmed json serialization/deserialation worked.
        }
        //With negative, confirm no json data object was returned.
        else
        {
            Assert.Null(returnedDataObject);
        }
    }

    /// <summary>
    /// The method verifies the StandardCRUDDataLayer.UpdateAsync() request and response based on the status code.
    /// </summary>
    /// <param name="httpStatusCode">The status to test against.</param>
    /// <returns>A Task object for the async.</returns>
    [Theory]
    [InlineData(HttpStatusCode.OK)]
    [InlineData(HttpStatusCode.Conflict)]
    [InlineData(HttpStatusCode.BadRequest)]
    [InlineData(HttpStatusCode.InternalServerError)]
    [InlineData(HttpStatusCode.NoContent)]
    [InlineData(HttpStatusCode.NotFound)]
    [InlineData(HttpStatusCode.Unauthorized)]
    public async Task VerifyUpdate(HttpStatusCode httpStatusCode)
    {
        //Nothing changes between the request and responding data objects.
        SimpleDataObject requestDataObject = new()
        {
            Integer64ID = DefaultId,
            Value = DefaultValue,
        };
        SimpleDataObject respondingDataObject = new(requestDataObject);

        NotFoundDetails responding404ProblemDetails = new(detail: $"The {typeof(SimpleDataObject).Name} record was not found; please refresh the page because another user may have deleted it.");
        ConflictDetails responding409ProblemDetails = new(detail: $"The submitted {typeof(SimpleDataObject).Name} data was detected to be out of date; please refresh the page and try again.");
        ProblemDetails responding500ProblemDetails = new(detail: $"Failed to update the {typeof(SimpleDataObject).Name} record because of an error on the server.");
        ValidationProblemDetails responding400ProblemDetails = new()
        {
            Errors =
            {
                { nameof(SimpleDataObject.Name), ["The name is not unique."] }
            }
        };

        HttpClient httpClient;

        if (httpStatusCode == HttpStatusCode.BadRequest)
        {
            httpClient = new MockHttpMessageHandler()
                .WithJsonContent(requestDataObject)
                .WithMethod(HttpMethod.Put)
                .WithRoute($"api/{nameof(SimpleDataObject)}")
                .RespondingHttpStatusCode(httpStatusCode)
                .RespondingJsonContent(responding400ProblemDetails)
                .Build();
        }
        else if (httpStatusCode is HttpStatusCode.Conflict)
        {
            httpClient = new MockHttpMessageHandler()
                .WithJsonContent(requestDataObject)
                .WithMethod(HttpMethod.Put)
                .WithRoute($"api/{nameof(SimpleDataObject)}")
                .RespondingHttpStatusCode(httpStatusCode)
                .RespondingJsonContent(responding409ProblemDetails)
                .Build();
        }
        else if (httpStatusCode is HttpStatusCode.NotFound)
        {
            httpClient = new MockHttpMessageHandler()
                .WithJsonContent(requestDataObject)
                .WithMethod(HttpMethod.Put)
                .WithRoute($"api/{nameof(SimpleDataObject)}")
                .RespondingHttpStatusCode(httpStatusCode)
                .RespondingJsonContent(responding404ProblemDetails)
                .Build();
        }
        else if (httpStatusCode is HttpStatusCode.InternalServerError)
        {
            httpClient = new MockHttpMessageHandler()
                .WithJsonContent(requestDataObject)
                .WithMethod(HttpMethod.Put)
                .WithRoute($"api/{nameof(SimpleDataObject)}")
                .RespondingHttpStatusCode(httpStatusCode)
                .RespondingJsonContent(responding500ProblemDetails)
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

        //With positive, compared against the expected response.
        if (httpStatusCode is HttpStatusCode.OK)
        {
            Assert.True(operationResult.IsSuccessStatusCode, "The IsSuccessStatusCode should have been true."); //Confirm a positive response.
            Assert.Null(operationResult.ProblemDetails); //Confirm there is no problem details.
            Assert.Empty(operationResult.ValidationErrors); //Confirm the server side validation result wan't the response.
            Assert.IsType<SimpleDataObject>(operationResult.DataObject); //Confirm a data object was returned.
            Assert.Equal(respondingDataObject.Integer64ID, ((SimpleDataObject)operationResult.DataObject).Integer64ID); //Confirm responding data object match data object in the operational result.
            Assert.Equal(respondingDataObject.Value, ((SimpleDataObject)operationResult.DataObject).Value); //Confirm responding data object match data object in the operational result.
        }
        //With validation errors, confirm the negative response and the expected validation error response.
        else if (httpStatusCode is HttpStatusCode.BadRequest)
        {
            Assert.False(operationResult.IsSuccessStatusCode, "The IsSuccessStatusCode should have been false."); //Confirm the negative response.
            Assert.Equal(httpStatusCode, operationResult.StatusCode); //Confirm the negative response.
            Assert.Null(operationResult.DataObject); //Confirm a data object wasn't the response
            Assert.Equal(responding400ProblemDetails.Detail, operationResult.ProblemDetails); //Confirm the details match.
            Assert.Contains(operationResult.ValidationErrors, obj => obj.Key == nameof(SimpleDataObject.Name)); //Confirm there's an error for the Name property.
            Assert.Single(operationResult.ValidationErrors[nameof(SimpleDataObject.Name)]); //Confirm the Name property has one error.
            Assert.Equal(responding400ProblemDetails.Errors[nameof(SimpleDataObject.Name)][0], operationResult.ValidationErrors[nameof(SimpleDataObject.Name)][0]); //Confirm json serialization/deserialization worked.
        }
        //With a conflict, confirm only the status and problem details was set.
        else if (httpStatusCode is HttpStatusCode.Conflict)
        {
            Assert.False(operationResult.IsSuccessStatusCode, "The IsSuccessStatusCode should have been false."); //Confirm the negative response.
            Assert.Equal(httpStatusCode, operationResult.StatusCode); //Confirm the negative response.
            Assert.Null(operationResult.DataObject); //Confirm a data object wasn't the response
            Assert.Equal(responding409ProblemDetails.Detail, operationResult.ProblemDetails); //Confirm the details match.
            Assert.Empty(operationResult.ValidationErrors); //Confirm there are no validation errors.
        }
        //With a not found, confirm only the status and problem details was set.
        else if (httpStatusCode is HttpStatusCode.NotFound)
        {
            Assert.False(operationResult.IsSuccessStatusCode, "The IsSuccessStatusCode should have been false."); //Confirm the negative response.
            Assert.Equal(httpStatusCode, operationResult.StatusCode); //Confirm the negative response.
            Assert.Null(operationResult.DataObject); //Confirm a data object wasn't the response
            Assert.Equal(responding404ProblemDetails.Detail, operationResult.ProblemDetails); //Confirm the details match.
            Assert.Empty(operationResult.ValidationErrors); //Confirm there are no validation errors.
        }
        //With an internal server error, confirm only the status and problem details was set.
        else if (httpStatusCode is HttpStatusCode.InternalServerError)
        {
            Assert.False(operationResult.IsSuccessStatusCode, "The IsSuccessStatusCode should have been false."); //Confirm the negative response.
            Assert.Equal(httpStatusCode, operationResult.StatusCode); //Confirm the negative response.
            Assert.Null(operationResult.DataObject); //Confirm a data object wasn't the response
            Assert.Equal(responding500ProblemDetails.Detail, operationResult.ProblemDetails); //Confirm the details match.
            Assert.Empty(operationResult.ValidationErrors); //Confirm there are no validation errors.
        }
        //With other negative responses, confirm only the status was set.
        else
        {
            Assert.False(operationResult.IsSuccessStatusCode); //Confirm the negative response.
            Assert.Equal(httpStatusCode, operationResult.StatusCode); //Confirm the negative response.
            Assert.Null(operationResult.DataObject); //Confirm a data object wasn't the response
            Assert.Empty(operationResult.ValidationErrors); //Confirm the server side validation result wan't the response.
        }
    }

    /// <summary>
    /// The method verifies if a null data object is passed to the StandardCRUDDataLayer.UpdateAsync(), an exception is thrown.
    /// </summary>
    /// <returns>A Task object for the async.</returns>
    [Fact]
    public async Task VerifyUpdateThrowsArgumentNullException() => await Assert.ThrowsAsync<ArgumentNullException>(() => new SimpleDataLayer().UpdateAsync(null!));

    /// <summary>
    /// The method verifies if StandardCRUDDataLayer.UpdateAsync() receives an empty string in the response, an exception is thrown.
    /// </summary>
    /// <param name="httpStatusCode">The status to test against.</param>
    /// <returns>A Task object for the async.</returns>
    /// <remarks>
    /// Only 200 and 400 codes attempts to parse json so only do an InlineData with them.
    /// </remarks>
    [Theory]
    [InlineData(HttpStatusCode.OK)]
    [InlineData(HttpStatusCode.BadRequest)]
    public async Task VerifyUpdateThrowsJsonException(HttpStatusCode httpStatusCode)
    {
        SimpleDataObject requestDataObject = new()
        {
            Integer64ID = DefaultId,
            Value = DefaultValue,
        };

        HttpClient httpClient = new MockHttpMessageHandler()
            .WithJsonContent(requestDataObject)
            .WithMethod(HttpMethod.Put)
            .WithRoute($"api/{nameof(SimpleDataObject)}")
            .RespondingHttpStatusCode(httpStatusCode)
            .Build();

        await Assert.ThrowsAsync<JsonException>(() => new SimpleDataLayer(httpClient).UpdateAsync(requestDataObject));
    }
}
