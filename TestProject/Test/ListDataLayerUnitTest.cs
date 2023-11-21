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
        /// The method confirms if a null data object is passed to the ListDataLayer.UpdateAsync(), an exception is thrown.
        /// </summary>
        [Fact]
        public void UpdateAsyncThrowsArgumentNullException() => Assert.ThrowsAnyAsync<ArgumentNullException>(() => new SimpleListDataLayer().UpdateAsync(null));

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
#warning The simple data object only has the key so there's nothing to update. I'll need to add a new property so this test can work.
        }
    }
}
