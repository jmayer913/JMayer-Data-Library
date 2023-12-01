using JMayer.Data.HTTP.DataLayer;
using JMayer.Data.HTTP.Handler;
using System.Net;
using TestProject.Data;
using TestProject.HTTP;

namespace TestProject.Test.HTTP
{
    /// <summary>
    /// The class manages tests for the HTTP DataLayer object.
    /// </summary>
    /// <remarks>
    /// The tests are against a SimpleDataLayer object which inherits from the DataLayer and
    /// the SimpleDataLayer doesn't override any of the base methods. Because of this, we're testing 
    /// the methods in the DataLayer class.
    /// </remarks>
    public class DataLayerUnitTest
    {
        /// <summary>
        /// The method confirms the DataLayer.CountAsync() returns 0 when negative status code is returned.
        /// </summary>
        /// <returns>A Task object for the async.</returns>
        [Fact]
        public async Task CountAsyncNegativeStatusCode()
        {
            HttpClient httpClient = new MockHttpMessageHandler()
                .WithRoute($"api/{nameof(SimpleDataObject)}/Count")
                .RespondingHttpStatusCode(HttpStatusCode.Unauthorized)
                .RespondingStringContent(10)
                .Build();

            SimpleDataLayer dataLayer = new(httpClient);
            int returnedCount = await dataLayer.CountAsync();

            Assert.Equal(0, returnedCount);
        }

        /// <summary>
        /// The method confirms the DataLayer.CountAsync() returns 0 when a no content status code is returned.
        /// </summary>
        /// <returns>A Task object for the async.</returns>
        [Fact]
        public async Task CountAsyncNoContentStatusCode()
        {
            HttpClient httpClient = new MockHttpMessageHandler()
                .WithRoute($"api/{nameof(SimpleDataObject)}/Count")
                .RespondingHttpStatusCode(HttpStatusCode.NoContent)
                .RespondingStringContent(10)
                .Build();

            SimpleDataLayer dataLayer = new(httpClient);
            int returnedCount = await dataLayer.CountAsync();

            Assert.Equal(0, returnedCount);
        }

        /// <summary>
        /// The method confirms the DataLayer.CountAsync() works as intended.
        /// </summary>
        /// <returns>A Task object for the async.</returns>
        [Fact]
        public async Task CountAsyncWorks()
        {
            int respondingCount = 10;

            HttpClient httpClient = new MockHttpMessageHandler()
                .WithRoute($"api/{nameof(SimpleDataObject)}/Count")
                .RespondingStringContent(respondingCount)
                .Build();

            SimpleDataLayer dataLayer = new(httpClient);
            int returnedCount = await dataLayer.CountAsync();

            Assert.Equal(respondingCount, returnedCount);
        }

        /// <summary>
        /// The method confirms the DataLayer.DeleteAsync() works as intended.
        /// </summary>
        /// <returns>A Task object for the async.</returns>
        [Fact]
        public async Task DeleteAsyncWorks()
        {
            long id = 1;

            HttpClient httpClient = new MockHttpMessageHandler()
                .WithMethod(HttpMethod.Delete)
                .WithRoute($"api/{nameof(SimpleDataObject)}")
                .WithRouteParameters([id.ToString()])
                .Build();

            SimpleDataLayer dataLayer = new(httpClient);
            OperationResult operationResult = await dataLayer.DeleteAsync(new SimpleDataObject() { Integer64ID = id });

            Assert.True(operationResult.IsSuccessStatusCode &&  operationResult.StatusCode == HttpStatusCode.OK && operationResult.DataObject == null);
        }

        /// <summary>
        /// The method confirms the DataLayer.GetAllAsync() returns an empty list when negative status code is returned.
        /// </summary>
        /// <returns>A Task object for the async.</returns>
        [Fact]
        public async Task GetAllAsyncNegativeStatusCode()
        {
            HttpClient httpClient = new MockHttpMessageHandler()
                .WithRoute($"api/{nameof(SimpleDataObject)}/All")
                .RespondingHttpStatusCode(HttpStatusCode.Unauthorized)
                .RespondingJsonContent(new List<SimpleDataObject>()
                {
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
                })
                .Build();

            SimpleDataLayer dataLayer = new(httpClient);
            List<SimpleDataObject>? returnedDataObjects = await dataLayer.GetAllAsync();

            Assert.True(returnedDataObjects != null && returnedDataObjects.Count == 0);
        }

        /// <summary>
        /// The method confirms the DataLayer.GetAllAsync() returns an empty list when a no content status code is returned.
        /// </summary>
        /// <returns>A Task object for the async.</returns>
        [Fact]
        public async Task GetAllAsyncNoContentStatusCode()
        {
            HttpClient httpClient = new MockHttpMessageHandler()
                .WithRoute($"api/{nameof(SimpleDataObject)}/All")
                .RespondingHttpStatusCode(HttpStatusCode.NoContent)
                .RespondingJsonContent(new List<SimpleDataObject>()
                {
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
                })
                .Build();

            SimpleDataLayer dataLayer = new(httpClient);
            List<SimpleDataObject>? returnedDataObjects = await dataLayer.GetAllAsync();

            Assert.True(returnedDataObjects != null && returnedDataObjects.Count == 0);
        }

        /// <summary>
        /// The method confirms the DataLayer.GetAllAsync() works as intended.
        /// </summary>
        /// <returns>A Task object for the async.</returns>
        [Fact]
        public async Task GetAllAsyncWorks()
        {
            List<SimpleDataObject> respondingDataObjects = new()
            {
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
            };

            HttpClient httpClient = new MockHttpMessageHandler()
                .WithRoute($"api/{nameof(SimpleDataObject)}/All")
                .RespondingJsonContent(respondingDataObjects)
                .Build();

            SimpleDataLayer dataLayer = new(httpClient);
            List<SimpleDataObject>? returnedDataObjects = await dataLayer.GetAllAsync();

            Assert.True
            (
                returnedDataObjects != null //Must have responded with json.
                && returnedDataObjects.Count == respondingDataObjects.Count //Must have parsed the json correctly.
                && returnedDataObjects[0].Integer64ID == respondingDataObjects[0].Integer64ID && returnedDataObjects[0].Value == respondingDataObjects[0].Value //Must have parsed the json correctly.
                && returnedDataObjects[1].Integer64ID == respondingDataObjects[1].Integer64ID && returnedDataObjects[1].Value == respondingDataObjects[1].Value //Must have parsed the json correctly.
            );
        }

        /// <summary>
        /// The method confirms the DataLayer.GetSingleAsync() returns null when negative status code is returned.
        /// </summary>
        /// <returns>A Task object for the async.</returns>
        [Fact]
        public async Task GetSingleAsyncIDNegativeStatusCode()
        {
            long id = 1;

            HttpClient httpClient = new MockHttpMessageHandler()
                .WithRoute($"api/{nameof(SimpleDataObject)}/Single")
                .WithRouteParameters([id.ToString()])
                .RespondingHttpStatusCode(HttpStatusCode.Unauthorized)
                .RespondingJsonContent(new SimpleDataObject()
                {
                    Integer64ID = id,
                    Value = 10,
                })
                .Build();

            SimpleDataLayer dataLayer = new(httpClient);
            SimpleDataObject? returnedDataObject = await dataLayer.GetSingleAsync(id.ToString());

            Assert.True(returnedDataObject == null);
        }

        /// <summary>
        /// The method confirms the DataLayer.GetSingleAsync() returns null when a no content status code is returned.
        /// </summary>
        /// <returns>A Task object for the async.</returns>
        [Fact]
        public async Task GetSingleAsyncIDNoContentStatusCode()
        {
            long id = 1;

            HttpClient httpClient = new MockHttpMessageHandler()
                .WithRoute($"api/{nameof(SimpleDataObject)}/Single")
                .WithRouteParameters([id.ToString()])
                .RespondingHttpStatusCode(HttpStatusCode.NoContent)
                .RespondingJsonContent(new SimpleDataObject()
                {
                    Integer64ID = id,
                    Value = 10,
                })
                .Build();

            SimpleDataLayer dataLayer = new(httpClient);
            SimpleDataObject? returnedDataObject = await dataLayer.GetSingleAsync(id.ToString());

            Assert.True(returnedDataObject == null);
        }

        /// <summary>
        /// The method confirms the DataLayer.GetSingleAsync() works as intended.
        /// </summary>
        /// <returns>A Task object for the async.</returns>
        [Fact]
        public async Task GetSingleAsyncIDWorks()
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
                .RespondingJsonContent(respondingDataObject)
                .Build();

            SimpleDataLayer dataLayer = new(httpClient);
            SimpleDataObject? returnedDataObject = await dataLayer.GetSingleAsync(id.ToString());

            Assert.True
            (
                returnedDataObject != null //Must have responded with json.
                && respondingDataObject.Integer64ID == returnedDataObject.Integer64ID //Must have parsed the json correctly.
                && respondingDataObject.Value == returnedDataObject.Value //Must have parsed the json correctly.
            );
        }

        /// <summary>
        /// The method confirms the DataLayer.GetSingleAsync() returns null when negative status code is returned.
        /// </summary>
        /// <returns>A Task object for the async.</returns>
        [Fact]
        public async Task GetSingleAsyncNegativeStatusCode()
        {
            HttpClient httpClient = new MockHttpMessageHandler()
                .WithRoute($"api/{nameof(SimpleDataObject)}/Single")
                .RespondingHttpStatusCode(HttpStatusCode.Unauthorized)
                .RespondingJsonContent(new SimpleDataObject()
                {
                    Integer64ID = 1,
                    Value = 10,
                })
                .Build();

            SimpleDataLayer dataLayer = new(httpClient);
            SimpleDataObject? returnedDataObject = await dataLayer.GetSingleAsync();

            Assert.True(returnedDataObject == null);
        }

        /// <summary>
        /// The method confirms the DataLayer.GetSingleAsync() returns null when a no content status code is returned.
        /// </summary>
        /// <returns>A Task object for the async.</returns>
        [Fact]
        public async Task GetSingleAsyncNoContentStatusCode()
        {
            HttpClient httpClient = new MockHttpMessageHandler()
                .WithRoute($"api/{nameof(SimpleDataObject)}/Single")
                .RespondingHttpStatusCode(HttpStatusCode.NoContent)
                .RespondingJsonContent(new SimpleDataObject()
                {
                    Integer64ID = 1,
                    Value = 10,
                })
                .Build();

            SimpleDataLayer dataLayer = new(httpClient);
            SimpleDataObject? returnedDataObject = await dataLayer.GetSingleAsync();

            Assert.True(returnedDataObject == null);
        }

        /// <summary>
        /// The method confirms the DataLayer.GetSingleAsync() works as intended.
        /// </summary>
        /// <returns>A Task object for the async.</returns>
        [Fact]
        public async Task GetSingleAsyncWorks()
        {
            SimpleDataObject respondingDataObject = new()
            {
                Integer64ID = 1,
                Value = 10,
            };

            HttpClient httpClient = new MockHttpMessageHandler()
                .WithRoute($"/api/{nameof(SimpleDataObject)}/Single")
                .RespondingJsonContent(respondingDataObject)
                .Build();

            SimpleDataLayer dataLayer = new(httpClient);
            SimpleDataObject? returnedDataObject = await dataLayer.GetSingleAsync();

            Assert.True
            (
                returnedDataObject != null //Must have responded with json.
                && respondingDataObject.Integer64ID == returnedDataObject.Integer64ID //Must have parsed the json correctly.
                && respondingDataObject.Value == returnedDataObject.Value //Must have parsed the json correctly.
            );
        }
    }
}
