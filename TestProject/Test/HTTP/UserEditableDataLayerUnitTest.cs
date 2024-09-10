using JMayer.Data.Data;
using JMayer.Data.Data.Query;
using JMayer.Data.HTTP.Handler;
using System.Net;
using TestProject.Data;
using TestProject.HTTP;

namespace TestProject.Test.HTTP;

/// <summary>
/// The class manages tests for the HTTP User Editable DataLayer object.
/// </summary>
/// <remarks>
/// The tests are against a SimpleUserEditableDataLayer object which inherits from the UserEditableDataLayer and
/// the SimpleUserEditableDataLayer doesn't override any of the base methods. Because of this, we're testing 
/// the methods in the UserEditableDataLayer class.
/// 
/// UserEditableDataLayer class inherits from the StandardCRUDDataLayer. Because of this,
/// only new and overriden methods in the UserEditableDataLayer are tested because
/// the StandardCRUDDataLayerUnitTest already tests the StandardCRUDDataLayer.
/// </remarks>
public class UserEditableDataLayerUnitTest
{
    /// <summary>
    /// The method confirms the UserEditableDataLayer.GetAllListViewAsync() request and response based on the status code.
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
            .WithRoute($"api/{nameof(SimpleUserEditableDataObject)}/All/ListView")
            .RespondingHttpStatusCode(httpStatusCode)
            .RespondingJsonContent(respondingDataObjects)
            .Build();

        SimpleUserEditableDataLayer dataLayer = new(httpClient);
        List<ListView>? returnedDataObjects = await dataLayer.GetAllListViewAsync();

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
    /// The method confirms the UserEditableDataLayer.GetPageListViewAsync() request and response based on the status code.
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
            .WithRoute($"api/{nameof(SimpleUserEditableDataObject)}/Page/ListView")
            .WithQueryString(queryString)
            .RespondingHttpStatusCode(httpStatusCode)
            .RespondingJsonContent(respondingPage)
            .Build();

        SimpleUserEditableDataLayer dataLayer = new(httpClient);
        PagedList<ListView>? returnedPage = await dataLayer.GetPageListViewAsync(queryDefinition);

        //With positive, confirm json data objects were returned.
        if (httpStatusCode == HttpStatusCode.OK)
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
    /// The method confirms if a null query definition is passed to the UserEditableDataLayer.GetPageListViewAsync(), an exception is thrown.
    /// </summary>
    /// <returns>A Task object for the async.</returns>
    [Fact]
    public async Task VerifyGetPageListViewThrowsArgumentNullException() => await Assert.ThrowsAsync<ArgumentNullException>(() => new SimpleUserEditableDataLayer().GetPageListViewAsync(null));
}
