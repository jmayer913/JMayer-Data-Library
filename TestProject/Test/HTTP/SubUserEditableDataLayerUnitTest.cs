﻿using JMayer.Data.Data;
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
    /// The method confirms if a a null or empty ID is passed to the SubUserEditableDataLayer.GetAllAsync(), an exception is thrown.
    /// </summary>
    /// <returns>A Task object for the async.</returns>
    [Fact]
    public async Task GetAllAsyncThrowsArgumentException() => await Assert.ThrowsAsync<ArgumentException>(() => new SimpleSubUserEditableDataLayer().GetAllAsync(string.Empty));

    /// <summary>
    /// The method confirms the SubUserEditableDataLayer.GetAllAsync() works as intended.
    /// </summary>
    /// <returns>A Task object for the async.</returns>
    [Theory]
    [InlineData(HttpStatusCode.OK)]
    [InlineData(HttpStatusCode.NoContent)]
    [InlineData(HttpStatusCode.Unauthorized)]
    public async Task GetAllAsyncWorks(HttpStatusCode httpStatusCode)
    {
        List<SimpleSubUserEditableDataObject> respondingDataObjects =
        [
            new SimpleSubUserEditableDataObject()
            {
                Integer64ID = 1,
                OwnerInteger64ID = 1,
                Value = 10,
            },
            new SimpleSubUserEditableDataObject()
            {
                Integer64ID = 2,
                OwnerInteger64ID = 1,
                Value = 20,
            },
        ];

        HttpClient httpClient = new MockHttpMessageHandler()
            .WithRoute($"api/{nameof(SimpleSubUserEditableDataObject)}/All")
            .WithRouteParameters(["1"])
            .RespondingHttpStatusCode(httpStatusCode)
            .RespondingJsonContent(respondingDataObjects)
            .Build();

        SimpleSubUserEditableDataLayer dataLayer = new(httpClient);
        List<SimpleSubUserEditableDataObject>? returnedDataObjects = await dataLayer.GetAllAsync(1);

        //With positive, confirm json data objects were returned.
        if (httpStatusCode == HttpStatusCode.OK)
        {
            Assert.True
            (
                returnedDataObjects != null //Must have responded with json.
                && returnedDataObjects.Count == respondingDataObjects.Count //Must have parsed the json correctly.
                && returnedDataObjects[0].Integer64ID == respondingDataObjects[0].Integer64ID && returnedDataObjects[0].OwnerInteger64ID == respondingDataObjects[0].OwnerInteger64ID && returnedDataObjects[0].Value == respondingDataObjects[0].Value //Must have parsed the json correctly.
                && returnedDataObjects[1].Integer64ID == respondingDataObjects[1].Integer64ID && returnedDataObjects[1].OwnerInteger64ID == respondingDataObjects[1].OwnerInteger64ID && returnedDataObjects[1].Value == respondingDataObjects[1].Value //Must have parsed the json correctly.
            );
        }
        //With negative, confirm no json data objects were returned.
        else
        {
            Assert.True(returnedDataObjects != null && returnedDataObjects.Count == 0);
        }
    }

    /// <summary>
    /// The method confirms if a null or empty ID is passed to the SubUserEditableDataLayer.GetAllListViewAsync(), an exception is thrown.
    /// </summary>
    /// <returns>A Task object for the async.</returns>
    [Fact]
    public async Task GetAllListViewAsyncThrowsArgumentException() => await Assert.ThrowsAsync<ArgumentException>(() => new SimpleSubUserEditableDataLayer().GetAllListViewAsync(string.Empty));

    /// <summary>
    /// The method confirms the SubUserEditableDataLayer.GetAllListViewAsync() works as intended.
    /// </summary>
    /// <returns>A Task object for the async.</returns>
    [Theory]
    [InlineData(HttpStatusCode.OK)]
    [InlineData(HttpStatusCode.NoContent)]
    [InlineData(HttpStatusCode.Unauthorized)]
    public async Task GetAllListViewAsyncWorks(HttpStatusCode httpStatusCode)
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
            .WithRoute($"api/{nameof(SimpleSubUserEditableDataObject)}/All/ListView")
            .WithRouteParameters(["1"])
            .RespondingHttpStatusCode(httpStatusCode)
            .RespondingJsonContent(respondingDataObjects)
            .Build();

        SimpleSubUserEditableDataLayer dataLayer = new(httpClient);
        List<ListView>? returnedDataObjects = await dataLayer.GetAllListViewAsync(1);

        //With positive, confirm json data objects were returned.
        if (httpStatusCode == HttpStatusCode.OK)
        {
            Assert.True
            (
                returnedDataObjects != null //Must have responded with json.
                && returnedDataObjects.Count == respondingDataObjects.Count //Must have parsed the json correctly.
                && returnedDataObjects[0].Integer64ID == respondingDataObjects[0].Integer64ID && returnedDataObjects[0].Name == respondingDataObjects[0].Name //Must have parsed the json correctly.
                && returnedDataObjects[1].Integer64ID == respondingDataObjects[1].Integer64ID && returnedDataObjects[1].Name == respondingDataObjects[1].Name //Must have parsed the json correctly.
            );
        }
        //With negative, confirm no json data objects were returned.
        else
        {
            Assert.True(returnedDataObjects != null && returnedDataObjects.Count == 0);
        }
    }

    /// <summary>
    /// The method confirms if a null or empty ID is passed to the SubUserEditableDataLayer.GetPageAsync(), an exception is thrown.
    /// </summary>
    /// <returns>A Task object for the async.</returns>
    [Fact]
    public async Task GetPageAsyncThrowsArgumentException() => await Assert.ThrowsAsync<ArgumentException>(() => new SimpleSubUserEditableDataLayer().GetPageAsync(string.Empty, null));

    /// <summary>
    /// The method confirms if a null query definition is passed to the SubUserEditableDataLayer.GetPageAsync(), an exception is thrown.
    /// </summary>
    /// <returns>A Task object for the async.</returns>
    [Fact]
    public async Task GetPageAsyncThrowsArgumentNullException() => await Assert.ThrowsAsync<ArgumentNullException>(() => new SimpleSubUserEditableDataLayer().GetPageAsync(1, null));

    /// <summary>
    /// The method confirms the SubUserEditableDataLayer.GetPageAsync() works as intended.
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
                    OwnerInteger64ID = 1,
                    Value = 1,
                },
                new SimpleSubUserEditableDataObject()
                {
                    Integer64ID = 10,
                    OwnerInteger64ID = 1,
                    Value = 10,
                },
                new SimpleSubUserEditableDataObject()
                {
                    Integer64ID = 100,
                    OwnerInteger64ID = 1,
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
            .WithRouteParameters(["1"])
            .WithQueryString(queryString)
            .RespondingHttpStatusCode(httpStatusCode)
            .RespondingJsonContent(respondingPage)
            .Build();

        SimpleSubUserEditableDataLayer dataLayer = new(httpClient);
        PagedList<SimpleSubUserEditableDataObject>? returnedPage = await dataLayer.GetPageAsync(1, queryDefinition);

        //With positive, confirm json data objects were returned.
        if (httpStatusCode == HttpStatusCode.OK)
        {
            Assert.True
            (
                returnedPage != null //Must have responded with json.
                && returnedPage.DataObjects.Count == respondingPage.DataObjects.Count //Must have parsed the json correctly.
                && returnedPage.TotalRecords == respondingPage.TotalRecords //Must have parsed the json correctly.
                && returnedPage.DataObjects[0].Integer64ID == respondingPage.DataObjects[0].Integer64ID && returnedPage.DataObjects[0].OwnerInteger64ID == respondingPage.DataObjects[0].OwnerInteger64ID && returnedPage.DataObjects[0].Value == respondingPage.DataObjects[0].Value //Must have parsed the json correctly.
                && returnedPage.DataObjects[1].Integer64ID == respondingPage.DataObjects[1].Integer64ID && returnedPage.DataObjects[1].OwnerInteger64ID == respondingPage.DataObjects[1].OwnerInteger64ID && returnedPage.DataObjects[1].Value == respondingPage.DataObjects[1].Value //Must have parsed the json correctly.
                && returnedPage.DataObjects[2].Integer64ID == respondingPage.DataObjects[2].Integer64ID && returnedPage.DataObjects[2].OwnerInteger64ID == respondingPage.DataObjects[2].OwnerInteger64ID && returnedPage.DataObjects[2].Value == respondingPage.DataObjects[2].Value //Must have parsed the json correctly.
            );
        }
        //With negative, confirm no json data objects were returned.
        else
        {
            Assert.True(returnedPage != null && returnedPage.DataObjects.Count == 0 && returnedPage.TotalRecords == 0);
        }
    }

    /// <summary>
    /// The method confirms if a null or empty ID is passed to the SubUserEditableDataLayer.GetPageListViewAsync(), an exception is thrown.
    /// </summary>
    /// <returns>A Task object for the async.</returns>
    [Fact]
    public async Task GetPageListViewAsyncThrowsArgumentException() => await Assert.ThrowsAsync<ArgumentException>(() => new SimpleSubUserEditableDataLayer().GetPageListViewAsync(string.Empty, null));

    /// <summary>
    /// The method confirms if a null query definition is passed to the SubUserEditableDataLayer.GetPageListViewAsync(), an exception is thrown.
    /// </summary>
    /// <returns>A Task object for the async.</returns>
    [Fact]
    public async Task GetPageListViewAsyncThrowsArgumentNullException() => await Assert.ThrowsAsync<ArgumentNullException>(() => new SimpleSubUserEditableDataLayer().GetPageListViewAsync(1, null));

    /// <summary>
    /// The method confirms the SubUserEditableDataLayer.GetPageListViewAsync() works as intended.
    /// </summary>
    /// <returns>A Task object for the async.</returns>
    [Theory]
    [InlineData(HttpStatusCode.OK)]
    [InlineData(HttpStatusCode.NoContent)]
    [InlineData(HttpStatusCode.Unauthorized)]
    public async Task GetPageListViewAsyncWorks(HttpStatusCode httpStatusCode)
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
            .WithRouteParameters(["1"])
            .WithQueryString(queryString)
            .RespondingHttpStatusCode(httpStatusCode)
            .RespondingJsonContent(respondingPage)
            .Build();

        SimpleSubUserEditableDataLayer dataLayer = new(httpClient);
        PagedList<ListView>? returnedPage = await dataLayer.GetPageListViewAsync(1, queryDefinition);

        //With positive, confirm json data objects were returned.
        if (httpStatusCode == HttpStatusCode.OK)
        {
            Assert.True
            (
                returnedPage != null //Must have responded with json.
                && returnedPage.DataObjects.Count == respondingPage.DataObjects.Count //Must have parsed the json correctly.
                && returnedPage.DataObjects[0].Integer64ID == respondingPage.DataObjects[0].Integer64ID && returnedPage.DataObjects[0].Name == respondingPage.DataObjects[0].Name //Must have parsed the json correctly.
                && returnedPage.DataObjects[1].Integer64ID == respondingPage.DataObjects[1].Integer64ID && returnedPage.DataObjects[1].Name == respondingPage.DataObjects[1].Name //Must have parsed the json correctly.
                && returnedPage.DataObjects[2].Integer64ID == respondingPage.DataObjects[2].Integer64ID && returnedPage.DataObjects[2].Name == respondingPage.DataObjects[2].Name //Must have parsed the json correctly.
            );
        }
        //With negative, confirm no json data objects were returned.
        else
        {
            Assert.True(returnedPage != null && returnedPage.DataObjects.Count == 0 && returnedPage.TotalRecords == 0);
        }
    }
}
