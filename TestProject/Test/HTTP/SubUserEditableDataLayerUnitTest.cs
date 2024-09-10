using JMayer.Data.Data;
using JMayer.Data.Data.Query;
using JMayer.Data.HTTP.Handler;
using System.Net;
using TestProject.Data;
using TestProject.HTTP;

namespace TestProject.Test.HTTP;

/// <summary>
/// The class manages tests for the HTTP Sub User Editable DataLayer object.
/// </summary>
/// <remarks>
/// The tests are against a SimpleSubUserEditableDataLayer object which inherits from the SubUserEditableDataLayer and
/// the SimpleSubUserEditableDataLayer doesn't override any of the base methods. Because of this, we're testing 
/// the methods in the SubUserEditableDataLayer class.
/// 
/// SubUserEditableDataLayer class inherits from the UserEditableDataLayer. Because of this,
/// only new and overriden methods in the SubUserEditableDataLayer are tested because
/// the UserEditableDataLayerUnitTest already tests the UserEditableDataLayer.
/// </remarks>
public class SubUserEditableDataLayerUnitTest
{
    /// <summary>
    /// The constant for the default owner id.
    /// </summary>
    private const long DefaultOwnerId = 1;

    /// <summary>
    /// The constant for the first record id.
    /// </summary>
    private const long FirstRecordId = 1;

    /// <summary>
    /// The constant for the first record value.
    /// </summary>
    private const int FirstRecordValue = 10;

    /// <summary>
    /// The constant for the second record id.
    /// </summary>
    private const int SecondRecordId = 2;

    /// <summary>
    /// The constant for the second record value.
    /// </summary>
    private const int SecondRecordValue = 20;

    /// <summary>
    /// The method verifies the SubUserEditableDataLayer.GetAllAsync() request and response based on the status code.
    /// </summary>
    /// <param name="httpStatusCode">The HTTP status code to test against.</param>
    /// <returns>A Task object for the async.</returns>
    [Theory]
    [InlineData(HttpStatusCode.OK)]
    [InlineData(HttpStatusCode.NoContent)]
    [InlineData(HttpStatusCode.Unauthorized)]
    public async Task VerifyGetAll(HttpStatusCode httpStatusCode)
    {
        List<SimpleSubUserEditableDataObject> respondingDataObjects =
        [
            new SimpleSubUserEditableDataObject()
            {
                Integer64ID = FirstRecordId,
                OwnerInteger64ID = DefaultOwnerId,
                Value = FirstRecordValue,
            },
            new SimpleSubUserEditableDataObject()
            {
                Integer64ID = 2,
                OwnerInteger64ID = DefaultOwnerId,
                Value = 20,
            },
        ];

        HttpClient httpClient = new MockHttpMessageHandler()
            .WithRoute($"api/{nameof(SimpleSubUserEditableDataObject)}/All")
            .WithRouteParameters([DefaultOwnerId.ToString()])
            .RespondingHttpStatusCode(httpStatusCode)
            .RespondingJsonContent(respondingDataObjects)
            .Build();

        SimpleSubUserEditableDataLayer dataLayer = new(httpClient);
        List<SimpleSubUserEditableDataObject>? returnedDataObjects = await dataLayer.GetAllAsync(DefaultOwnerId);

        //With positive, confirm json data objects were returned.
        if (httpStatusCode == HttpStatusCode.OK)
        {
            Assert.NotNull(returnedDataObjects); //Must have responded with json.
            Assert.Equal(respondingDataObjects.Count, returnedDataObjects.Count); //Must have parsed the json correctly.

            for (int index = 0; index < returnedDataObjects.Count; index++)
            {
                Assert.Equal(respondingDataObjects[index].Integer64ID, returnedDataObjects[index].Integer64ID); //Must have parsed the json correctly.
                Assert.Equal(respondingDataObjects[index].OwnerInteger64ID, returnedDataObjects[index].OwnerInteger64ID); //Must have parsed the json correctly.
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
    /// The method verifies the SubUserEditableDataLayer.GetAllListViewAsync() request and response based on the status code.
    /// </summary>
    /// <param name="httpStatusCode">The HTTP status code to test against.</param>
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
                Integer64ID = FirstRecordId,
                Name = FirstRecordValue.ToString(),
            },
            new ListView()
            {
                Integer64ID = SecondRecordId,
                Name = SecondRecordValue.ToString(),
            },
        ];

        HttpClient httpClient = new MockHttpMessageHandler()
            .WithRoute($"api/{nameof(SimpleSubUserEditableDataObject)}/All/ListView")
            .WithRouteParameters(["1"])
            .RespondingHttpStatusCode(httpStatusCode)
            .RespondingJsonContent(respondingDataObjects)
            .Build();

        SimpleSubUserEditableDataLayer dataLayer = new(httpClient);
        List<ListView>? returnedDataObjects = await dataLayer.GetAllListViewAsync(DefaultOwnerId);

        //With positive, confirm json data objects were returned.
        if (httpStatusCode == HttpStatusCode.OK)
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
    /// The method verifies if a null or empty ID is passed to the SubUserEditableDataLayer.GetAllListViewAsync(), an exception is thrown.
    /// </summary>
    /// <returns>A Task object for the async.</returns>
    [Fact]
    public async Task VerifyGetAllListViewThrowsArgumentException() => await Assert.ThrowsAsync<ArgumentException>(() => new SimpleSubUserEditableDataLayer().GetAllListViewAsync(string.Empty));

    /// <summary>
    /// The method verifies if a a null or empty ID is passed to the SubUserEditableDataLayer.GetAllAsync(), an exception is thrown.
    /// </summary>
    /// <returns>A Task object for the async.</returns>
    [Fact]
    public async Task VerifyGetAllThrowsArgumentException() => await Assert.ThrowsAsync<ArgumentException>(() => new SimpleSubUserEditableDataLayer().GetAllAsync(string.Empty));

    /// <summary>
    /// The method verifies the SubUserEditableDataLayer.GetPageAsync() request and response based on the status code.
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

        PagedList<SimpleSubUserEditableDataObject> respondingPage = new()
        {
            DataObjects =
            [
                new SimpleSubUserEditableDataObject()
                {
                    Integer64ID = 1,
                    OwnerInteger64ID = DefaultOwnerId,
                    Value = 1,
                },
                new SimpleSubUserEditableDataObject()
                {
                    Integer64ID = 10,
                    OwnerInteger64ID = DefaultOwnerId,
                    Value = 10,
                },
                new SimpleSubUserEditableDataObject()
                {
                    Integer64ID = 100,
                    OwnerInteger64ID = DefaultOwnerId,
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
            .WithRoute($"api/{nameof(SimpleSubUserEditableDataObject)}/Page")
            .WithRouteParameters([DefaultOwnerId.ToString()])
            .WithQueryString(queryString)
            .RespondingHttpStatusCode(httpStatusCode)
            .RespondingJsonContent(respondingPage)
            .Build();

        SimpleSubUserEditableDataLayer dataLayer = new(httpClient);
        PagedList<SimpleSubUserEditableDataObject>? returnedPage = await dataLayer.GetPageAsync(DefaultOwnerId, queryDefinition);

        //With positive, confirm json data objects were returned.
        if (httpStatusCode == HttpStatusCode.OK)
        {
            Assert.NotNull(returnedPage); //Must have responded with json.
            Assert.Equal(respondingPage.DataObjects.Count, returnedPage.DataObjects.Count); //Must have parsed the json correctly.
            Assert.Equal(respondingPage.TotalRecords, returnedPage.TotalRecords); //Must have parsed the json correctly.

            for (int index = 0; index < returnedPage.DataObjects.Count; index++)
            {
                Assert.Equal(respondingPage.DataObjects[index].Integer64ID, returnedPage.DataObjects[index].Integer64ID); //Must have parsed the json correctly.
                Assert.Equal(respondingPage.DataObjects[index].OwnerInteger64ID, returnedPage.DataObjects[index].OwnerInteger64ID); //Must have parsed the json correctly.
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
    /// The method verifies if a null or empty ID is passed to the SubUserEditableDataLayer.GetPageAsync(), an exception is thrown.
    /// </summary>
    /// <returns>A Task object for the async.</returns>
    [Fact]
    public async Task VerifyGetPageThrowsArgumentException() => await Assert.ThrowsAsync<ArgumentException>(() => new SimpleSubUserEditableDataLayer().GetPageAsync(string.Empty, null));

    /// <summary>
    /// The method verifies if a null query definition is passed to the SubUserEditableDataLayer.GetPageAsync(), an exception is thrown.
    /// </summary>
    /// <returns>A Task object for the async.</returns>
    [Fact]
    public async Task VerifyGetPageThrowsArgumentNullException() => await Assert.ThrowsAsync<ArgumentNullException>(() => new SimpleSubUserEditableDataLayer().GetPageAsync(1, null));

    /// <summary>
    /// The method verifies the SubUserEditableDataLayer.GetPageListViewAsync() request and response based on the status code.
    /// </summary>
    /// <param name="httpStatusCode">The HTTP status code to test against.</param>
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
                    FilterOn = nameof(SimpleUserEditableDataObject.Value),
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
                    SortOn = nameof(SimpleUserEditableDataObject.Value),
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
            .WithRoute($"api/{nameof(SimpleSubUserEditableDataObject)}/Page/ListView")
            .WithRouteParameters([DefaultOwnerId.ToString()])
            .WithQueryString(queryString)
            .RespondingHttpStatusCode(httpStatusCode)
            .RespondingJsonContent(respondingPage)
            .Build();

        SimpleSubUserEditableDataLayer dataLayer = new(httpClient);
        PagedList<ListView>? returnedPage = await dataLayer.GetPageListViewAsync(DefaultOwnerId, queryDefinition);

        //With positive, confirm json data objects were returned.
        if (httpStatusCode == HttpStatusCode.OK)
        {
            Assert.NotNull(returnedPage); //Must have parsed the json correctly.
            Assert.Equal(respondingPage.DataObjects.Count, returnedPage.DataObjects.Count); //Must have parsed the json correctly.
            Assert.Equal(respondingPage.TotalRecords, returnedPage.TotalRecords); //Must have parsed the json correctly.

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
    /// The method verifies if a null or empty ID is passed to the SubUserEditableDataLayer.GetPageListViewAsync(), an exception is thrown.
    /// </summary>
    /// <returns>A Task object for the async.</returns>
    [Fact]
    public async Task VerifyGetPageListViewThrowsArgumentException() => await Assert.ThrowsAsync<ArgumentException>(() => new SimpleSubUserEditableDataLayer().GetPageListViewAsync(string.Empty, null));

    /// <summary>
    /// The method verifies if a null query definition is passed to the SubUserEditableDataLayer.GetPageListViewAsync(), an exception is thrown.
    /// </summary>
    /// <returns>A Task object for the async.</returns>
    [Fact]
    public async Task VerifyGetPageListViewThrowsArgumentNullException() => await Assert.ThrowsAsync<ArgumentNullException>(() => new SimpleSubUserEditableDataLayer().GetPageListViewAsync(1, null));
}
