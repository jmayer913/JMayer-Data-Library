using JMayer.Data.HTTP.Handler;
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
        /// The method confirms the DataLayer.GetSingleAsync() works as intended.
        /// </summary>
        /// <returns>A Task object for the async.</returns>
        [Fact]
        public async Task GetSingleAsyncWorks()
        {
            SimpleDataObject dataObject = new()
            {
                Integer64ID = 1,
                Value = 10,
            };

            HttpClient httpClient = new MockHttpMessageHandler()
                .WithRoute($"/api/{nameof(SimpleDataObject)}/Single")
                .RespondingJsonContent(dataObject)
                .Build();

            SimpleDataLayer dataLayer = new(httpClient);
            SimpleDataObject? returnedDataObject = await dataLayer.GetSingleAsync();

            Assert.True
            (
                returnedDataObject != null //Must have responded with json.
                && dataObject.Integer64ID == returnedDataObject.Integer64ID //Must have parsed the json correctly.
                && dataObject.Value == returnedDataObject.Value //Must have parsed the json correctly.
            );
        }
    }
}
