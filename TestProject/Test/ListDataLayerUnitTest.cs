using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using TestProject.Data;
using TestProject.Database;

namespace TestProject.Test
{
    /// <summary>
    /// The class manages tests for the ListDataLayer object.
    /// </summary>
    /// <remarks>
    /// The tests are against a SimpleListDataLayer object which inherits from the ListDataLayer and
    /// the SimpleListDataLayer doesn't override any of the base methods because of this, we're testing 
    /// the methods in the ListDataLayer class.
    /// </remarks>
    public class ListDataLayerUnitTest
    {
        /// <summary>
        /// The method confirms if a null data object is passed to the ListDataLayer.CountAsync(), an exception is thrown.
        /// </summary>
        [Fact]
        public void CountAsyncThrowsArgumentNullException() => Assert.ThrowsAsync<ArgumentNullException>(() => new SimpleListDataLayer().CountAsync(null));

        /// <summary>
        /// The method confirms the ListDataLayer.CountAsync() works as intended.
        /// </summary>
        /// <returns>A Task object for the async.</returns>
        [Fact]
        public async Task CountAsyncWorks()
        {
            SimpleListDataLayer dataLayer = new();

            _ = await dataLayer.CreateAsync(new SimpleDataObject());
            int count = await dataLayer.CountAsync();

            Assert.Equal(1, count);
        }

        /// <summary>
        /// The method confirms the ListDataLayer.CountAsync() works as intended for the where predicate.
        /// </summary>
        /// <returns>A Task object for the async.</returns>
        [Fact]
        public async Task CountAsyncWherePredicateWorks()
        {
            SimpleListDataLayer dataLayer = new();

            _ = await dataLayer.CreateAsync(new SimpleDataObject());
            _ = await dataLayer.CreateAsync(new SimpleDataObject());
            _ = await dataLayer.CreateAsync(new SimpleDataObject());
            _ = await dataLayer.CreateAsync(new SimpleDataObject());
            
            int oneCount = await dataLayer.CountAsync(obj => obj.Key32 == 1);
            int zeroCount = await dataLayer.CountAsync(obj => obj.Key32 == 99);

            Assert.True(oneCount == 1 && zeroCount == 0);
        }

        /// <summary>
        /// The method confirms if a null data object is passed to the ListDataLayer.CreateAsync(), an exception is thrown.
        /// </summary>
        [Fact]
        public void CreateAsyncThrowsArgumentNullException() => Assert.ThrowsAnyAsync<ArgumentNullException>(() => new SimpleListDataLayer().CreateAsync(null));

        /// <summary>
        /// The method confirms ListDataLayer.CreateAsync() works as intended.
        /// </summary>
        /// <returns>A Task object for the async.</returns>
        /// <remarks>
        /// The CreateAsync() does the following:
        /// 
        /// 1. Preps the data object (key is set).
        /// 2. Data object is added to the data store.
        /// 3. Internal identity is increment.
        /// 4. A copy of the data object is returned.
        /// </remarks>
        [Fact]
        public async Task CreateAsyncWorks()
        {
            SimpleDataObject originalDataObject = new();
            SimpleListDataLayer dataLayer = new();

            SimpleDataObject firstReturnedCopiedDataObject = await dataLayer.CreateAsync(originalDataObject);
            SimpleDataObject secondReturnedDataObject = await dataLayer.CreateAsync(originalDataObject);
            int count = await dataLayer.CountAsync();
            
            Assert.True
            (
                firstReturnedCopiedDataObject != null && secondReturnedDataObject != null //An object must have been returned.
                && originalDataObject != firstReturnedCopiedDataObject && originalDataObject != secondReturnedDataObject //Returned object must be a copy.
                && firstReturnedCopiedDataObject.Key32 > 0 && secondReturnedDataObject.Key32 > 0 //Key must have been set.
                && firstReturnedCopiedDataObject.Key32 != secondReturnedDataObject.Key32 && secondReturnedDataObject.Key32 - firstReturnedCopiedDataObject.Key32 == 1 //Internal identity must be incremented.
                && count == 2 //Data object was added to the data store.
            );
        }

        /// <summary>
        /// The method confirms if a null data object is passed to the ListDataLayer.DeleteAsync(), an exception is thrown.
        /// </summary>
        [Fact]
        public void DeleteAsyncThrowsArgumentNullException() => Assert.ThrowsAnyAsync<ArgumentNullException>(() => new SimpleListDataLayer().DeleteAsync(null));

        /// <summary>
        /// The method confirms ListDataLayer.DeleteAsync() works as intended.
        /// </summary>
        /// <returns>A Task object for the async.</returns>
        /// <remarks>
        /// The DeleteAsync() does the following:
        /// 
        /// 1. Finds the data object using the key.
        /// 2. Data object is deleted from the data store.
        /// </remarks>
        [Fact]
        public async Task DeleteAsyncWorks()
        {
            SimpleListDataLayer dataLayer = new();

            SimpleDataObject dataObject = await dataLayer.CreateAsync(new SimpleDataObject());
            await dataLayer.DeleteAsync(dataObject);
            int count = await dataLayer.CountAsync();

            Assert.Equal(0, count);
        }

        /// <summary>
        /// The method confirms if a null data object is passed to the ListDataLayer.ExistAsync(), an exception is thrown.
        /// </summary>
        [Fact]
        public void ExistAsyncThrowsArgumentNullException() 
        { 
            Assert.ThrowsAnyAsync<ArgumentException>(() => new SimpleListDataLayer().ExistAsync((string)null));
            Assert.ThrowsAnyAsync<ArgumentNullException>(() => new SimpleListDataLayer().ExistAsync((Expression<Func<SimpleDataObject, bool>>)null));
        }

        /// <summary>
        /// The method confirms ListDataLayer.ExistAsync() works as intended.
        /// </summary>
        /// <returns>A Task object for the async.</returns>
        [Fact]
        public async Task ExistAsyncKeyWorks()
        {
            SimpleListDataLayer dataLayer = new();
            _ = await dataLayer.CreateAsync(new SimpleDataObject());

            bool found = await dataLayer.ExistAsync("1");
            bool notFound = await dataLayer.ExistAsync("2") == false;

            Assert.True(found && notFound);
        }

        /// <summary>
        /// The method confirms ListDataLayer.ExistAsync() works as intended for the where predicate.
        /// </summary>
        /// <returns>A Task object for the async.</returns>
        [Fact]
        public async Task ExistAsyncWherePredicateWorks()
        {
            SimpleListDataLayer dataLayer = new();
            _ = await dataLayer.CreateAsync(new SimpleDataObject());

            bool found = await dataLayer.ExistAsync(obj => obj.Key32 == 1);
            bool notFound = await dataLayer.ExistAsync(obj => obj.Key32 == 2) == false;

            Assert.True(found && notFound);
        }

        /// <summary>
        /// The method confirms if a null data object is passed to the ListDataLayer.GetAllAsync(), an exception is thrown.
        /// </summary>
        [Fact]
        public void GetAllAsyncThrowsArgumentNullException()
        {
            Assert.ThrowsAnyAsync<ArgumentNullException>(() => new SimpleListDataLayer().GetAllAsync(null));
            Assert.ThrowsAnyAsync<ArgumentNullException>(() => new SimpleListDataLayer().GetAllAsync(null, false));
            Assert.ThrowsAnyAsync<ArgumentNullException>(() => new SimpleListDataLayer().GetAllAsync(obj => obj.Value == 0, null));
        }

        /// <summary>
        /// The method confirms ListDataLayer.GetAllAsync() works as intended.
        /// </summary>
        /// <returns>A Task object for the async.</returns>
        [Fact]
        public async Task GetAllAsyncWorks()
        {
            SimpleListDataLayer dataLayer = new();
            _ = await dataLayer.CreateAsync(new SimpleDataObject());
            List<SimpleDataObject> dataObjects = await dataLayer.GetAllAsync();
            Assert.True(dataObjects.Count == 1);
        }

        /// <summary>
        /// The method confirms ListDataLayer.GetAllAsync() works as intended for the where predicate.
        /// </summary>
        /// <returns>A Task object for the async.</returns>
        [Fact]
        public async Task GetAllAsyncWherePredicateWorks()
        {
            SimpleListDataLayer dataLayer = new();

            _ = await dataLayer.CreateAsync(new SimpleDataObject() { Value = 10 });
            _ = await dataLayer.CreateAsync(new SimpleDataObject() { Value = 20 });
            _ = await dataLayer.CreateAsync(new SimpleDataObject() { Value = 30 });
            _ = await dataLayer.CreateAsync(new SimpleDataObject() { Value = 40 });
            _ = await dataLayer.CreateAsync(new SimpleDataObject() { Value = 50 });
            _ = await dataLayer.CreateAsync(new SimpleDataObject() { Value = 60 });
            _ = await dataLayer.CreateAsync(new SimpleDataObject() { Value = 70 });
            _ = await dataLayer.CreateAsync(new SimpleDataObject() { Value = 80 });
            _ = await dataLayer.CreateAsync(new SimpleDataObject() { Value = 90 });
            _ = await dataLayer.CreateAsync(new SimpleDataObject() { Value = 100 });

            List<SimpleDataObject> lessThan40DataObjects = await dataLayer.GetAllAsync(obj => obj.Value < 40);
            List<SimpleDataObject> greaterThan80DataObjects = await dataLayer.GetAllAsync(obj => obj.Value > 80);
            List<SimpleDataObject> failedToFindDataObjects = await dataLayer.GetAllAsync(obj => obj.Value == 1);

            Assert.True(lessThan40DataObjects.Count == 3 && greaterThan80DataObjects.Count == 2 && failedToFindDataObjects.Count == 0);
        }

        /// <summary>
        /// The method confirms ListDataLayer.GetAllAsync() works as intended for the order predicate.
        /// </summary>
        /// <returns>A Task object for the async.</returns>
        [Fact]
        public async Task GetAllAsyncOrderPredicateWorks()
        {
            SimpleListDataLayer dataLayer = new();

            _ = await dataLayer.CreateAsync(new SimpleDataObject() { Value = 10 });
            _ = await dataLayer.CreateAsync(new SimpleDataObject() { Value = 20 });
            _ = await dataLayer.CreateAsync(new SimpleDataObject() { Value = 30 });
            _ = await dataLayer.CreateAsync(new SimpleDataObject() { Value = 40 });
            _ = await dataLayer.CreateAsync(new SimpleDataObject() { Value = 50 });
            _ = await dataLayer.CreateAsync(new SimpleDataObject() { Value = 60 });
            _ = await dataLayer.CreateAsync(new SimpleDataObject() { Value = 70 });
            _ = await dataLayer.CreateAsync(new SimpleDataObject() { Value = 80 });
            _ = await dataLayer.CreateAsync(new SimpleDataObject() { Value = 90 });
            _ = await dataLayer.CreateAsync(new SimpleDataObject() { Value = 100 });

            List<SimpleDataObject> ascOrderByDataObjects = await dataLayer.GetAllAsync(obj => obj.Value);
            List<SimpleDataObject> descOrderByDataObjects = await dataLayer.GetAllAsync(obj => obj.Value, true);

            Assert.True(ascOrderByDataObjects.First().Value == 10 && descOrderByDataObjects.First().Value == 100);
        }

        /// <summary>
        /// The method confirms ListDataLayer.GetAllAsync() works as intended for the where and order predicate.
        /// </summary>
        /// <returns>A Task object for the async.</returns>
        [Fact]
        public async Task GetAllAsyncWherePredicateOrderPredicateWorks()
        {
            SimpleListDataLayer dataLayer = new();

            _ = await dataLayer.CreateAsync(new SimpleDataObject() { Value = 10 });
            _ = await dataLayer.CreateAsync(new SimpleDataObject() { Value = 20 });
            _ = await dataLayer.CreateAsync(new SimpleDataObject() { Value = 30 });
            _ = await dataLayer.CreateAsync(new SimpleDataObject() { Value = 40 });
            _ = await dataLayer.CreateAsync(new SimpleDataObject() { Value = 50 });
            _ = await dataLayer.CreateAsync(new SimpleDataObject() { Value = 60 });
            _ = await dataLayer.CreateAsync(new SimpleDataObject() { Value = 70 });
            _ = await dataLayer.CreateAsync(new SimpleDataObject() { Value = 80 });
            _ = await dataLayer.CreateAsync(new SimpleDataObject() { Value = 90 });
            _ = await dataLayer.CreateAsync(new SimpleDataObject() { Value = 100 });

            List<SimpleDataObject> whereAscOrderByDataObjects = await dataLayer.GetAllAsync(obj => obj.Value > 30, obj => obj.Value);
            List<SimpleDataObject> whereDescOrderByDataObjects = await dataLayer.GetAllAsync(obj => obj.Value < 60, obj => obj.Value, true);

            Assert.True(whereAscOrderByDataObjects.First().Value == 40 && whereDescOrderByDataObjects.First().Value == 50);
        }

        /// <summary>
        /// The method confirms ListDataLayer.GetSingleAsync() works as intended.
        /// </summary>
        /// <returns>A Task object for the async.</returns>
        [Fact]
        public async Task GetSingleAsyncWorks()
        {
            SimpleListDataLayer dataLayer = new();

            bool notFound = await dataLayer.GetSingleAsync() == null;
            _ = await dataLayer.CreateAsync(new SimpleDataObject());
            bool found = await dataLayer.GetSingleAsync() != null;

            Assert.True(found && notFound);
        }

        /// <summary>
        /// The method confirms ListDataLayer.GetSingleAsync() works as intended for the wherepredicate.
        /// </summary>
        /// <returns>A Task object for the async.</returns>
        [Fact]
        public async Task GetSingleAsyncWherePredicateWorks()
        {
            SimpleListDataLayer dataLayer = new();

            _ = await dataLayer.CreateAsync(new SimpleDataObject() { Value = 10 });
            _ = await dataLayer.CreateAsync(new SimpleDataObject() { Value = 20 });
            _ = await dataLayer.CreateAsync(new SimpleDataObject() { Value = 30 });
            _ = await dataLayer.CreateAsync(new SimpleDataObject() { Value = 40 });
            _ = await dataLayer.CreateAsync(new SimpleDataObject() { Value = 50 });
            _ = await dataLayer.CreateAsync(new SimpleDataObject() { Value = 60 });
            _ = await dataLayer.CreateAsync(new SimpleDataObject() { Value = 70 });
            _ = await dataLayer.CreateAsync(new SimpleDataObject() { Value = 80 });
            _ = await dataLayer.CreateAsync(new SimpleDataObject() { Value = 90 });
            _ = await dataLayer.CreateAsync(new SimpleDataObject() { Value = 100 });

            bool found = await dataLayer.GetSingleAsync(obj => obj.Value == 70) != null;
            bool notFound = await dataLayer.GetSingleAsync(obj => obj.Value == 71) == null;

            Assert.True(found && notFound);
        }

        /// <summary>
        /// The method confirms if a null data object is passed to the ListDataLayer.UpdateAsync(), an exception is thrown.
        /// </summary>
        [Fact]
        public void UpdateAsyncThrowsArgumentNullException() => Assert.ThrowsAnyAsync<ArgumentNullException>(() => new SimpleListDataLayer().UpdateAsync(null));

        /// <summary>
        /// The method confirms if a non-existing key is passed to the ListDataLayer.UpdateAsync(), an exception is thrown.
        /// </summary>
        [Fact]
        public void UpdateAsyncThrowsKeyNotFoundException() => Assert.ThrowsAnyAsync<KeyNotFoundException>(() => new SimpleListDataLayer().UpdateAsync(new SimpleDataObject() { Key = "99" }));

        /// <summary>
        /// The method confirms ListDataLayer.UpdateAsync() works as intended.
        /// </summary>
        /// <returns>A Task object for the async.</returns>
        /// <remarks>
        /// The UpdateAsync() does the following:
        /// 
        /// 1. Finds the data object using the key.
        /// 2. Preps the data object (does nothing).
        /// 3. Data object is update in the data store.
        /// 4. A copy of the data object is returned.
        /// </remarks>
        [Fact]
        public async Task UpdateAsyncWorks()
        {
            SimpleListDataLayer dataLayer = new();
            SimpleDataObject originalDataObject = await dataLayer.CreateAsync(new SimpleDataObject());

            originalDataObject.Value = 10;
            SimpleDataObject returnedCopiedDataObject = await dataLayer.UpdateAsync(originalDataObject);
            SimpleDataObject? confirmedDataObject = await dataLayer.GetSingleAsync();

            Assert.True
            (
                returnedCopiedDataObject != null //An object must have been returned.
                && originalDataObject != returnedCopiedDataObject //Returned object must be a copy.
                && originalDataObject.Value == confirmedDataObject?.Value //Confirm the data was updated.
            );
        }

        /// <summary>
        /// The method confirms if a null data object is passed to the ListDataLayer.ValidateAsync(), an exception is thrown.
        /// </summary>
        [Fact]
        public void ValidateAsyncThrowsArgumentNullException() => Assert.ThrowsAnyAsync<ArgumentNullException>(() => new SimpleListDataLayer().ValidateAsync(null));

        /// <summary>
        /// The method confirms ListDataLayer.ValidateAsync() works as intended.
        /// </summary>
        /// <returns>A Task object for the async.</returns>
        /// <remarks>
        /// The ValidateAsync() does the following:
        /// 
        /// 1. Validate the data annotations on the object. (SimpleDataObject.Value has a Range data annotation.)
        /// 2. Validate against any custom rules. (ListDataLayer needs the key to exists if the data object has one.)
        /// 3. The results is returned.
        /// </remarks>
        [Fact]
        public async Task ValidateAsyncWorks()
        {
            SimpleListDataLayer dataLayer = new();
            SimpleDataObject dataObject = await dataLayer.CreateAsync(new SimpleDataObject());

            dataObject.Value = 10;
            List<ValidationResult> validationResults = await dataLayer.ValidateAsync(dataObject);
            bool dataAnnotationValid = validationResults.Count == 0;

            dataObject.Value = 9999;
            validationResults = await dataLayer.ValidateAsync(dataObject);
            bool dataAnnotationNotValided = validationResults.Count != 0;

            dataObject.Value = 0;
            validationResults = await dataLayer.ValidateAsync(dataObject);
            bool keyExistsValid = validationResults.Count == 0;

            dataObject.Key = "9999";
            validationResults = await dataLayer.ValidateAsync(dataObject);
            bool keyExistsNotValid = validationResults.Count != 0;

            Assert.True(dataAnnotationValid && dataAnnotationNotValided && keyExistsValid && keyExistsNotValid);
        }
    }
}
