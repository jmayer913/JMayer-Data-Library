using JMayer.Data.Data;
using JMayer.Data.Database.DataLayer;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
using TestProject.Data;
using TestProject.Database;

namespace TestProject.Test
{
    /// <summary>
    /// The class manages tests for the ConfigurationListDataLayer object.
    /// </summary>
    /// <remarks>
    /// The tests are against a SimpleConfigurationListDataLayer object which inherits from 
    /// the ConfigurationListDataLayer and the SimpleConfigurationListDataLayer doesn't override 
    /// any of the base methods because of this, we're testing the methods in the 
    /// ConfigurationListDataLayer class.
    /// 
    /// ConfigurationListDataLayer class inherits from the ListDataLayer. Because of this,
    /// only new and overriden methods in the ConfigurationListDataLayer are tested because
    /// the ListDataLayerUnitTest already tests the ListDataLayer.
    /// 
    /// All tests associated with UpdateAsync() in the ListDataLayerUnitTest are included
    /// here because ConfigurationListDataLayer overrides UpdateAsync() and it doesn't call
    /// the base.
    /// </remarks>
    public class ConfigurationListDataLayerUnitTest
    {
        /// <summary>
        /// The method confirms ConfigurationListDataLayer.CreateAsync() works as intended.
        /// </summary>
        /// <returns>A Task object for the async.</returns>
        /// <remarks>
        /// The CreateAsync() does the following:
        /// 
        /// 1. Preps the data object (ID and created on are set).
        /// 2. Data object is added to the data store.
        /// 3. Internal identity is increment.
        /// 4. A copy of the data object is returned.
        /// </remarks>
        [Fact]
        public async Task CreateAsyncWorks()
        {
            SimpleConfigurationDataObject originalDataObject = new() { Name = "A Name" };
            SimpleConfigurationListDataLayer dataLayer = new();

            SimpleConfigurationDataObject firstReturnedCopiedDataObject = await dataLayer.CreateAsync(originalDataObject);
            SimpleConfigurationDataObject secondReturnedDataObject = await dataLayer.CreateAsync(originalDataObject);
            int count = await dataLayer.CountAsync();

            Assert.True
            (
                firstReturnedCopiedDataObject != null && secondReturnedDataObject != null //An object must have been returned.
                && originalDataObject != firstReturnedCopiedDataObject && originalDataObject != secondReturnedDataObject //Returned object must be a copy.
                && firstReturnedCopiedDataObject.Integer32ID > 0 && secondReturnedDataObject.Integer32ID > 0 //ID must have been set.
                && firstReturnedCopiedDataObject.CreatedOn > DateTime.MinValue && secondReturnedDataObject.CreatedOn > DateTime.MinValue //CreatedOn must have been set.
                && firstReturnedCopiedDataObject.Integer32ID != secondReturnedDataObject.Integer32ID && secondReturnedDataObject.Integer32ID - firstReturnedCopiedDataObject.Integer32ID == 1 //Internal identity must be incremented.
                && count == 2 //Data object was added to the data store.
            );
        }

        /// <summary>
        /// The method confirms if a null data object is passed to the ConfigurationListDataLayer.GetAllListViewAsync(), an exception is thrown.
        /// </summary>
        [Fact]
        public void GetAllListViewAsyncThrowsArgumentNullException()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => new SimpleConfigurationListDataLayer().GetAllListViewAsync(null));
            Assert.ThrowsAsync<ArgumentNullException>(() => new SimpleConfigurationListDataLayer().GetAllListViewAsync(null, false));
            Assert.ThrowsAsync<ArgumentNullException>(() => new SimpleConfigurationListDataLayer().GetAllListViewAsync(obj => obj.Value == 0, null));
        }

        /// <summary>
        /// The method confirms ConfigurationListDataLayer.GetAllListViewAsync() works as intended.
        /// </summary>
        /// <returns>A Task object for the async.</returns>
        [Fact]
        public async Task GetAllListViewAsyncWorks()
        {
            SimpleConfigurationListDataLayer dataLayer = new();
            _ = await dataLayer.CreateAsync(new SimpleConfigurationDataObject() { Name = "A Name" });
            List<ListView> listViews = await dataLayer.GetAllListViewAsync();
            Assert.True(listViews.Count == 1);
        }

        /// <summary>
        /// The method confirms ConfigurationListDataLayer.GetAllListViewAsync() works as intended for the where predicate.
        /// </summary>
        /// <returns>A Task object for the async.</returns>
        [Fact]
        public async Task GetAllAsyncWherePredicateWorks()
        {
            SimpleConfigurationListDataLayer dataLayer = new();

            _ = await dataLayer.CreateAsync(new SimpleConfigurationDataObject() { Name = "10", Value = 10 });
            _ = await dataLayer.CreateAsync(new SimpleConfigurationDataObject() { Name = "20", Value = 20 });
            _ = await dataLayer.CreateAsync(new SimpleConfigurationDataObject() { Name = "30", Value = 30 });
            _ = await dataLayer.CreateAsync(new SimpleConfigurationDataObject() { Name = "40", Value = 40 });
            _ = await dataLayer.CreateAsync(new SimpleConfigurationDataObject() { Name = "50", Value = 50 });
            _ = await dataLayer.CreateAsync(new SimpleConfigurationDataObject() { Name = "60", Value = 60 });
            _ = await dataLayer.CreateAsync(new SimpleConfigurationDataObject() { Name = "70", Value = 70 });
            _ = await dataLayer.CreateAsync(new SimpleConfigurationDataObject() { Name = "80", Value = 80 });
            _ = await dataLayer.CreateAsync(new SimpleConfigurationDataObject() { Name = "90", Value = 90 });
            _ = await dataLayer.CreateAsync(new SimpleConfigurationDataObject() { Name = "100", Value = 100 });

            List<ListView> lessThan40DataObjects = await dataLayer.GetAllListViewAsync(obj => obj.Value < 40);
            List<ListView> greaterThan80DataObjects = await dataLayer.GetAllListViewAsync(obj => obj.Value > 80);
            List<ListView> failedToFindDataObjects = await dataLayer.GetAllListViewAsync(obj => obj.Value == 1);

            Assert.True(lessThan40DataObjects.Count == 3 && greaterThan80DataObjects.Count == 2 && failedToFindDataObjects.Count == 0);
        }

        /// <summary>
        /// The method confirms ConfigurationListDataLayer.GetAllListViewAsync() works as intended for the order predicate.
        /// </summary>
        /// <returns>A Task object for the async.</returns>
        [Fact]
        public async Task GetAllAsyncOrderPredicateWorks()
        {
            SimpleConfigurationListDataLayer dataLayer = new();

            _ = await dataLayer.CreateAsync(new SimpleConfigurationDataObject() { Name = "10", Value = 10 });
            _ = await dataLayer.CreateAsync(new SimpleConfigurationDataObject() { Name = "20", Value = 20 });
            _ = await dataLayer.CreateAsync(new SimpleConfigurationDataObject() { Name = "30", Value = 30 });
            _ = await dataLayer.CreateAsync(new SimpleConfigurationDataObject() { Name = "40", Value = 40 });
            _ = await dataLayer.CreateAsync(new SimpleConfigurationDataObject() { Name = "50", Value = 50 });
            _ = await dataLayer.CreateAsync(new SimpleConfigurationDataObject() { Name = "60", Value = 60 });
            _ = await dataLayer.CreateAsync(new SimpleConfigurationDataObject() { Name = "70", Value = 70 });
            _ = await dataLayer.CreateAsync(new SimpleConfigurationDataObject() { Name = "80", Value = 80 });
            _ = await dataLayer.CreateAsync(new SimpleConfigurationDataObject() { Name = "90", Value = 90 });
            _ = await dataLayer.CreateAsync(new SimpleConfigurationDataObject() { Name = "100", Value = 100 });

            List<ListView> ascOrderByDataObjects = await dataLayer.GetAllListViewAsync(obj => obj.Value);
            List<ListView> descOrderByDataObjects = await dataLayer.GetAllListViewAsync(obj => obj.Value, true);

            Assert.True(ascOrderByDataObjects.First().Name == "10" && descOrderByDataObjects.First().Name == "100");
        }

        /// <summary>
        /// The method confirms ConfigurationListDataLayer.GetAllListViewAsync() works as intended for the where and order predicate.
        /// </summary>
        /// <returns>A Task object for the async.</returns>
        [Fact]
        public async Task GetAllAsyncWherePredicateOrderPredicateWorks()
        {
            SimpleConfigurationListDataLayer dataLayer = new();

            _ = await dataLayer.CreateAsync(new SimpleConfigurationDataObject() { Name = "10", Value = 10 });
            _ = await dataLayer.CreateAsync(new SimpleConfigurationDataObject() { Name = "20", Value = 20 });
            _ = await dataLayer.CreateAsync(new SimpleConfigurationDataObject() { Name = "30", Value = 30 });
            _ = await dataLayer.CreateAsync(new SimpleConfigurationDataObject() { Name = "40", Value = 40 });
            _ = await dataLayer.CreateAsync(new SimpleConfigurationDataObject() { Name = "50", Value = 50 });
            _ = await dataLayer.CreateAsync(new SimpleConfigurationDataObject() { Name = "60", Value = 60 });
            _ = await dataLayer.CreateAsync(new SimpleConfigurationDataObject() { Name = "70", Value = 70 });
            _ = await dataLayer.CreateAsync(new SimpleConfigurationDataObject() { Name = "80", Value = 80 });
            _ = await dataLayer.CreateAsync(new SimpleConfigurationDataObject() { Name = "90", Value = 90 });
            _ = await dataLayer.CreateAsync(new SimpleConfigurationDataObject() { Name = "100", Value = 100 });

            List<ListView> whereAscOrderByDataObjects = await dataLayer.GetAllListViewAsync(obj => obj.Value > 30, obj => obj.Value);
            List<ListView> whereDescOrderByDataObjects = await dataLayer.GetAllListViewAsync(obj => obj.Value < 60, obj => obj.Value, true);

            Assert.True(whereAscOrderByDataObjects.First().Name == "40" && whereDescOrderByDataObjects.First().Name == "50");
        }

        /// <summary>
        /// The method confirms the ListDataLayer.Updated event fires when ListDataLayer.UpdateAsync() is called.
        /// </summary>
        [Fact]
        public void UpdateAsyncFiresUpdatedEvent()
        {
            SimpleConfigurationListDataLayer dataLayer = new();
            Assert.RaisesAsync<DeletedEventArgs>
            (
                handler => dataLayer.Deleted += handler, 
                handler => dataLayer.Deleted -= handler, 
                async Task () =>
                {
                    SimpleConfigurationDataObject dataObject = await dataLayer.CreateAsync(new SimpleConfigurationDataObject() { Name = "A Name" });
                    dataObject.Value = 9999;
                    _ = await dataLayer.UpdateAsync(dataObject);
                }
            );
        }

        /// <summary>
        /// The method confirms if a null data object is passed to the ConfigurationListDataLayer.UpdateAsync(), an exception is thrown.
        /// </summary>
        [Fact]
        public void UpdateAsyncThrowsArgumentNullException() => Assert.ThrowsAsync<ArgumentNullException>(() => new SimpleListDataLayer().UpdateAsync(null));

        /// <summary>
        /// The method confirms if an invalid data object is passed to the ListDataLayer.UpdateAsync(), an exception is thrown.
        /// </summary>
        [Fact]
        public void UpdateAsyncThrowsDataObjectValidationException()
        {
            Assert.ThrowsAsync<DataObjectValidationException>(async Task () =>
            {
                SimpleConfigurationListDataLayer dataLayer = new();
                SimpleConfigurationDataObject dataObject = await dataLayer.CreateAsync(new SimpleConfigurationDataObject() { Name = "A Name" });

                dataObject.Name = null;
                _ = await dataLayer.UpdateAsync(dataObject);
            });
        }

        /// <summary>
        /// The method confirms if a non-existing ID is passed to the ConfigurationListDataLayer.UpdateAsync(), an exception is thrown.
        /// </summary>
        [Fact]
        public void UpdateAsyncThrowsIDNotFoundException() => Assert.ThrowsAsync<IDNotFoundException>(() => new SimpleListDataLayer().UpdateAsync(new SimpleDataObject() { Integer32ID = 99 }));

        /// <summary>
        /// The method confirms if old data is being updated in ConfigurationListDataLayer.UpdateAsync(), an exception is thrown.
        /// </summary>
        [Fact]
        public void UpdateAsyncThrowsUpdateConflictException()
        {
            Assert.ThrowsAsync<DataObjectUpdateConflictException>(async Task () =>
            {
                SimpleConfigurationListDataLayer dataLayer = new();
                SimpleConfigurationDataObject originalDataObject = await dataLayer.CreateAsync(new SimpleConfigurationDataObject() { Name = "A Name" });
                
                _ = await dataLayer.UpdateAsync(new SimpleConfigurationDataObject(originalDataObject)
                {
                    Value = 10,
                });

                originalDataObject.Value = 20;
                _ = await dataLayer.UpdateAsync(originalDataObject);
            });
        }

        /// <summary>
        /// The method confirms ConfigurationListDataLayer.UpdateAsync() works as intended.
        /// </summary>
        /// <returns>A Task object for the async.</returns>
        /// <remarks>
        /// The UpdateAsync() does the following:
        /// 
        /// 1. Finds the data object using the ID.
        /// 2. Confirms if the data isn't old.
        /// 3. Preps the data object (sets last edited on).
        /// 4. Data object is update in the data store.
        /// 5. A copy of the data object is returned.
        /// </remarks>
        [Fact]
        public async Task UpdateAsyncWorks()
        {
            SimpleConfigurationListDataLayer dataLayer = new();
            SimpleConfigurationDataObject originalDataObject = await dataLayer.CreateAsync(new SimpleConfigurationDataObject() { Name = "A Name" });

            originalDataObject.Value = 10;
            SimpleConfigurationDataObject returnedCopiedDataObject = await dataLayer.UpdateAsync(originalDataObject);
            SimpleConfigurationDataObject? confirmedDataObject = await dataLayer.GetSingleAsync();

            Assert.True
            (
                returnedCopiedDataObject != null //An object must have been returned.
                && originalDataObject != returnedCopiedDataObject //Returned object must be a copy.
                && originalDataObject.Value == confirmedDataObject?.Value //Confirm the data was updated.
                && returnedCopiedDataObject.LastEditedOn != null //LastEditedOn must have been set.
            );
        }

        /// <summary>
        /// The method confirms if a null data object is passed to the ConfigurationListDataLayer.ValidateAsync(), an exception is thrown.
        /// </summary>
        [Fact]
        public void ValidateAsyncThrowsArgumentNullException() => Assert.ThrowsAsync<ArgumentNullException>(() => new SimpleListDataLayer().ValidateAsync(null));

        /// <summary>
        /// The method confirms ConfigurationListDataLayer.ValidateAsync() works as intended.
        /// </summary>
        /// <returns>A Task object for the async.</returns>
        /// <remarks>
        /// The ValidateAsync() does the following:
        /// 
        /// 1. Validate the data annotations on the object. (SimpleConfigurationDataObject.Value has a Range data annotation.)
        /// 2. Validate against any custom rules. (ConfigurationListDataLayer needs the key to exists if the data object has one and the name must unique.)
        /// 3. The results is returned.
        /// </remarks>
        [Fact]
        public async Task ValidateAsyncWorks()
        {
            SimpleConfigurationListDataLayer dataLayer = new();
            SimpleConfigurationDataObject dataObject = await dataLayer.CreateAsync(new SimpleConfigurationDataObject() { Name = "A Name" });
            _ = await dataLayer.CreateAsync(new SimpleConfigurationDataObject() { Name = "A Name 2" });

            dataObject.Name = "Different Name";
            dataObject.Value = 10;
            List<ValidationResult> validationResults = await dataLayer.ValidateAsync(dataObject);
            bool dataAnnotationValid = validationResults.Count == 0;

            dataObject.Value = 9999;
            validationResults = await dataLayer.ValidateAsync(dataObject);
            bool dataAnnotationNotValided = validationResults.Count != 0;

            dataObject.Value = 0;
            validationResults = await dataLayer.ValidateAsync(dataObject);
            bool keyExistsValid = validationResults.Count == 0;

            dataObject.Integer32ID = 9999;
            validationResults = await dataLayer.ValidateAsync(dataObject);
            bool keyExistsNotValid = validationResults.Count != 0;

            dataObject.Integer32ID = 1;
            validationResults = await dataLayer.ValidateAsync(dataObject);
            bool nameValid = validationResults.Count == 0;

            dataObject.Integer32ID = 2;
            dataObject.Name = "A Name";
            validationResults = await dataLayer.ValidateAsync(dataObject);
            bool nameNotValid = validationResults.Count != 0;

            Assert.True(dataAnnotationValid && dataAnnotationNotValided && keyExistsValid && keyExistsNotValid && nameValid && nameNotValid);
        }
    }
}
