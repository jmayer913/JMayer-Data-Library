using JMayer.Data.Database.DataLayer;
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
    /// the SimpleListDataLayer doesn't override any of the base methods. Because of this, we're testing 
    /// the methods in the ListDataLayer class.
    /// </remarks>
    public class ListDataLayerUnitTest
    {
        /// <summary>
        /// The method confirms if a null data object is passed to the ListDataLayer.CountAsync(), an exception is thrown.
        /// </summary>
        [Fact]
        public async Task CountAsyncThrowsArgumentNullException() => await Assert.ThrowsAsync<ArgumentNullException>(() => new SimpleListDataLayer().CountAsync(null));

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
            
            int oneCount = await dataLayer.CountAsync(obj => obj.Integer64ID == 1);
            int zeroCount = await dataLayer.CountAsync(obj => obj.Integer64ID == 99);

            Assert.True(oneCount == 1 && zeroCount == 0);
        }

        /// <summary>
        /// The method confirms ListDataLayer.CreateAsync() works as intended.
        /// </summary>
        /// <returns>A Task object for the async.</returns>
        /// <remarks>
        /// The CreateAsync() does the following:
        /// 
        /// 1. Preps the data object (ID is set).
        /// 2. Data object is added to the data store.
        /// 3. Internal identity is increment.
        /// 4. A copy of the data object is returned.
        /// </remarks>
        [Fact]
        public async Task CreateAsyncDataObjectWorks()
        {
            SimpleListDataLayer dataLayer = new();

            SimpleDataObject originalDataObject = new();
            SimpleDataObject firstReturnedCopiedDataObject = await dataLayer.CreateAsync(originalDataObject);
            SimpleDataObject secondReturnedDataObject = await dataLayer.CreateAsync(originalDataObject);
            int count = await dataLayer.CountAsync();

            Assert.True
            (
                firstReturnedCopiedDataObject != null && secondReturnedDataObject != null //An object must have been returned.
                && originalDataObject != firstReturnedCopiedDataObject && originalDataObject != secondReturnedDataObject //Returned object must be a copy.
                && firstReturnedCopiedDataObject.Integer64ID > 0 && secondReturnedDataObject.Integer64ID > 0 //ID must have been set.
                && firstReturnedCopiedDataObject.Integer64ID != secondReturnedDataObject.Integer64ID && secondReturnedDataObject.Integer64ID - firstReturnedCopiedDataObject.Integer64ID == 1 //Internal identity must be incremented.
                && count == 2 //Data object was added to the data store.
            );
        }

        /// <summary>
        /// The method confirms the ListDataLayer.Created event fires when ListDataLayer.CreateAsync() is called.
        /// </summary>
        [Fact]
        public async Task CreateAsyncFiresCreatedEvent()
        {
            SimpleListDataLayer dataLayer = new();
            await Assert.RaisesAsync<CreatedEventArgs>
            (
                handler => dataLayer.Created += handler,
                handler => dataLayer.Created -= handler,
                () => dataLayer.CreateAsync(new SimpleDataObject())
            );
            await Assert.RaisesAsync<CreatedEventArgs>
            (
                handler => dataLayer.Created += handler,
                handler => dataLayer.Created -= handler,
                () => dataLayer.CreateAsync([new SimpleDataObject(), new SimpleDataObject()])
            );
        }

        /// <summary>
        /// The method confirms ListDataLayer.CreateAsync() works as intended.
        /// </summary>
        /// <returns>A Task object for the async.</returns>
        /// <remarks>
        /// The CreateAsync() does the following:
        /// 
        /// 1. Preps the data object (ID is set).
        /// 2. Data object is added to the data store.
        /// 3. Internal identity is increment.
        /// 4. A copy of the data object is returned.
        /// </remarks>
        [Fact]
        public async Task CreateAsyncListWorks()
        {
            SimpleListDataLayer dataLayer = new();

            SimpleDataObject firstOriginalDataObject = new();
            SimpleDataObject secondOriginalDataObject = new();
            List<SimpleDataObject> returnedDataObjects = await dataLayer.CreateAsync([firstOriginalDataObject, secondOriginalDataObject]);
            int count = await dataLayer.CountAsync();

            Assert.True
            (
                returnedDataObjects != null && returnedDataObjects.Count == 2 //An object must have been returned and the same amount added must be returned.
                && firstOriginalDataObject != returnedDataObjects[0] && firstOriginalDataObject != returnedDataObjects[1] //Returned objects must be a copy.
                && returnedDataObjects[0].Integer64ID > 0 && returnedDataObjects[1].Integer64ID > 0 //ID must have been set.
                && returnedDataObjects[0].Integer64ID != returnedDataObjects[1].Integer64ID && returnedDataObjects[1].Integer64ID - returnedDataObjects[0].Integer64ID == 1 //Internal identity must be incremented.
                && count == 2 //Data objects were added to the data store.
            );
        }

        /// <summary>
        /// The method confirms if a null data object is passed to the ListDataLayer.CreateAsync(), an exception is thrown.
        /// </summary>
        [Fact]
        public async Task CreateAsyncThrowsArgumentNullException()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => new SimpleListDataLayer().CreateAsync((SimpleDataObject)null));
            await Assert.ThrowsAsync<ArgumentNullException>(() => new SimpleListDataLayer().CreateAsync((List<SimpleDataObject>)null));
        }

        /// <summary>
        /// The method confirms if an invalid data object is passed to the ListDataLayer.CreateAsync(), an exception is thrown.
        /// </summary>
        [Fact]
        public async Task CreateAsyncThrowsDataObjectValidationException()
        {
            await Assert.ThrowsAsync<DataObjectValidationException>(() => new SimpleListDataLayer().CreateAsync(new SimpleDataObject() { Value = 9999 }));
            await Assert.ThrowsAsync<DataObjectValidationException>(() => new SimpleListDataLayer().CreateAsync([ new SimpleDataObject() { Value = 1 }, new SimpleDataObject() { Value = 9999 } ]));
        }

        /// <summary>
        /// The method confirms ListDataLayer.DeleteAsync() works as intended.
        /// </summary>
        /// <returns>A Task object for the async.</returns>
        /// <remarks>
        /// The DeleteAsync() does the following:
        /// 
        /// 1. Finds the data object using the ID.
        /// 2. Data object is deleted from the data store.
        /// </remarks>
        [Fact]
        public async Task DeleteAsyncDataObjectWorks()
        {
            SimpleListDataLayer dataLayer = new();

            SimpleDataObject dataObject = await dataLayer.CreateAsync(new SimpleDataObject());
            await dataLayer.DeleteAsync(dataObject);
            int count = await dataLayer.CountAsync();

            Assert.Equal(0, count);
        }

        /// <summary>
        /// The method confirms the ListDataLayer.Deleted event fires when ListDataLayer.DeleteAsync() is called.
        /// </summary>
        [Fact]
        public async Task DeleteAsyncFiresDeletedEvent()
        {
            SimpleListDataLayer dataLayer = new();
            await Assert.RaisesAsync<DeletedEventArgs>
            (
                handler => dataLayer.Deleted += handler,
                handler => dataLayer.Deleted -= handler,
                async Task () =>
                {
                    SimpleDataObject dataObject = await dataLayer.CreateAsync(new SimpleDataObject());
                    await dataLayer.DeleteAsync(dataObject);
                }
            );
            await Assert.RaisesAsync<DeletedEventArgs>
            (
                handler => dataLayer.Deleted += handler,
                handler => dataLayer.Deleted -= handler,
                async Task () =>
                {
                    List<SimpleDataObject> dataObjects = await dataLayer.CreateAsync([new SimpleDataObject(), new SimpleDataObject()]);
                    await dataLayer.DeleteAsync(dataObjects);
                }
            );
            await Assert.RaisesAsync<DeletedEventArgs>
            (
                handler => dataLayer.Deleted += handler,
                handler => dataLayer.Deleted -= handler,
                async Task () =>
                {
                    _ = await dataLayer.CreateAsync([new SimpleDataObject(), new SimpleDataObject(), new SimpleDataObject(), new SimpleDataObject()]);
                    await dataLayer.DeleteAsync(obj => obj.Integer64ID > 1);
                }
            );
        }

        /// <summary>
        /// The method confirms ListDataLayer.DeleteAsync() works as intended.
        /// </summary>
        /// <returns>A Task object for the async.</returns>
        /// <remarks>
        /// The DeleteAsync() does the following:
        /// 
        /// 1. Finds the data objects using the ID.
        /// 2. Data objects are deleted from the data store.
        /// </remarks>
        [Fact]
        public async Task DeleteAsyncListWorks()
        {
            SimpleListDataLayer dataLayer = new();
            List<SimpleDataObject> dataObjects = [];

            dataObjects.Add(await dataLayer.CreateAsync(new SimpleDataObject()));
            dataObjects.Add(await dataLayer.CreateAsync(new SimpleDataObject()));
            dataObjects.Add(await dataLayer.CreateAsync(new SimpleDataObject()));
            dataObjects.Add(await dataLayer.CreateAsync(new SimpleDataObject()));

            await dataLayer.DeleteAsync(dataObjects);
            int count = await dataLayer.CountAsync();

            Assert.Equal(0, count);
        }

        /// <summary>
        /// The method confirms if a null data object is passed to the ListDataLayer.DeleteAsync(), an exception is thrown.
        /// </summary>
        [Fact]
        public async Task DeleteAsyncThrowsArgumentNullException()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => new SimpleListDataLayer().DeleteAsync((SimpleDataObject)null));
            await Assert.ThrowsAsync<ArgumentNullException>(() => new SimpleListDataLayer().DeleteAsync((List<SimpleDataObject>)null));
            await Assert.ThrowsAsync<ArgumentNullException>(() => new SimpleListDataLayer().DeleteAsync((Expression<Func<SimpleDataObject, bool>>)null));
        }

        /// <summary>
        /// The method confirms ListDataLayer.DeleteAsync() works as intended.
        /// </summary>
        /// <returns>A Task object for the async.</returns>
        /// <remarks>
        /// The DeleteAsync() does the following:
        /// 
        /// 1. Finds the data objects using the where predicate.
        /// 2. Data objects are deleted from the data store.
        /// </remarks>
        [Fact]
        public async Task DeleteAsyncWherePredicateWorks()
        {
            SimpleListDataLayer dataLayer = new();

            _ = await dataLayer.CreateAsync(new SimpleDataObject());
            _ = await dataLayer.CreateAsync(new SimpleDataObject());
            _ = await dataLayer.CreateAsync(new SimpleDataObject());
            _ = await dataLayer.CreateAsync(new SimpleDataObject());

            await dataLayer.DeleteAsync(obj => obj.Integer64ID > 2);
            int count = await dataLayer.CountAsync();

            Assert.Equal(2, count);
        }

        /// <summary>
        /// The method confirms if a null data object is passed to the ListDataLayer.ExistAsync(), an exception is thrown.
        /// </summary>
        [Fact]
        public async Task ExistAsyncThrowsArgumentNullException() => await Assert.ThrowsAsync<ArgumentNullException>(() => new SimpleListDataLayer().ExistAsync(null));

        /// <summary>
        /// The method confirms ListDataLayer.ExistAsync() works as intended for the where predicate.
        /// </summary>
        /// <returns>A Task object for the async.</returns>
        [Fact]
        public async Task ExistAsyncWherePredicateWorks()
        {
            SimpleListDataLayer dataLayer = new();
            _ = await dataLayer.CreateAsync(new SimpleDataObject());

            bool found = await dataLayer.ExistAsync(obj => obj.Integer64ID == 1);
            bool notFound = await dataLayer.ExistAsync(obj => obj.Integer64ID == 99) == false;

            Assert.True(found && notFound);
        }

        /// <summary>
        /// The method confirms if a null data object is passed to the ListDataLayer.GetAllAsync(), an exception is thrown.
        /// </summary>
        [Fact]
        public async Task GetAllAsyncThrowsArgumentNullException()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => new SimpleListDataLayer().GetAllAsync(null));
            await Assert.ThrowsAsync<ArgumentNullException>(() => new SimpleListDataLayer().GetAllAsync(null, false));
            await Assert.ThrowsAsync<ArgumentNullException>(() => new SimpleListDataLayer().GetAllAsync(obj => obj.Value == 0, null));
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
        /// The method confirms ListDataLayer.UpdateAsync() works as intended.
        /// </summary>
        /// <returns>A Task object for the async.</returns>
        /// <remarks>
        /// The UpdateAsync() does the following:
        /// 
        /// 1. Finds the data object using the ID.
        /// 2. Preps the data object (does nothing).
        /// 3. Data object is update in the data store.
        /// 4. A copy of the data object is returned.
        /// </remarks>
        [Fact]
        public async Task UpdateAsyncDataObjectWorks()
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
        /// The method confirms the ListDataLayer.Updated event fires when ListDataLayer.UpdateAsync() is called.
        /// </summary>
        [Fact]
        public async Task UpdateAsyncFiresUpdatedEvent()
        {
            SimpleListDataLayer dataLayer = new();
            await Assert.RaisesAsync<UpdatedEventArgs>
            (
                handler => dataLayer.Updated += handler,
                handler => dataLayer.Updated -= handler,
                async Task () =>
                {
                    SimpleDataObject dataObject = await dataLayer.CreateAsync(new SimpleDataObject());
                    dataObject.Value = 10;
                    _ = await dataLayer.UpdateAsync(dataObject);
                }
            );
            await Assert.RaisesAsync<UpdatedEventArgs>
            (
                handler => dataLayer.Updated += handler,
                handler => dataLayer.Updated -= handler,
                async Task () =>
                {
                    List<SimpleDataObject> dataObjects = await dataLayer.CreateAsync([new SimpleDataObject(), new SimpleDataObject()]);

                    dataObjects[0].Value = 10;
                    dataObjects[1].Value = 20;

                    _ = await dataLayer.UpdateAsync(dataObjects);
                }
            );
        }

        /// <summary>
        /// The method confirms ListDataLayer.UpdateAsync() works as intended.
        /// </summary>
        /// <returns>A Task object for the async.</returns>
        /// <remarks>
        /// The UpdateAsync() does the following:
        /// 
        /// 1. Finds the data object using the ID.
        /// 2. Preps the data object (does nothing).
        /// 3. Data object is update in the data store.
        /// 4. A copy of the data object is returned.
        /// </remarks>
        [Fact]
        public async Task UpdateAsyncListWorks()
        {
            SimpleListDataLayer dataLayer = new();
            List<SimpleDataObject> originalDataObjects = await dataLayer.CreateAsync([new SimpleDataObject(), new SimpleDataObject()]);

            originalDataObjects[0].Value = 10;
            originalDataObjects[1].Value = 20;

            List<SimpleDataObject> returnedCopiedDataObjects = await dataLayer.UpdateAsync(originalDataObjects);
            List<SimpleDataObject> confirmedDataObjects = await dataLayer.GetAllAsync();

            Assert.True
            (
                returnedCopiedDataObjects != null && returnedCopiedDataObjects.Count == 2 //An object must have been returned and the same amount updated must be returned.
                && originalDataObjects[0] != returnedCopiedDataObjects[0] && originalDataObjects[1] != returnedCopiedDataObjects[1] //Returned objects must be a copy.
                && originalDataObjects[0].Value == confirmedDataObjects[0].Value && originalDataObjects[1].Value == confirmedDataObjects[1].Value //Confirm the data was updated.
            );
        }

        /// <summary>
        /// The method confirms if a null data object is passed to the ListDataLayer.UpdateAsync(), an exception is thrown.
        /// </summary>
        [Fact]
        public async Task UpdateAsyncThrowsArgumentNullException()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => new SimpleListDataLayer().UpdateAsync((SimpleDataObject)null));
            await Assert.ThrowsAsync<ArgumentNullException>(() => new SimpleListDataLayer().UpdateAsync((List<SimpleDataObject>)null));
        }

        /// <summary>
        /// The method confirms if an invalid data object is passed to the ListDataLayer.UpdateAsync(), an exception is thrown.
        /// </summary>
        [Fact]
        public async Task UpdateAsyncThrowsDataObjectValidationException()
        {
            await Assert.ThrowsAsync<DataObjectValidationException>(async Task () =>
            {
                SimpleListDataLayer dataLayer = new();
                SimpleDataObject dataObject = await dataLayer.CreateAsync(new SimpleDataObject());

                dataObject.Value = 9999;
                
                _ = await dataLayer.UpdateAsync(dataObject);
            });
            await Assert.ThrowsAsync<DataObjectValidationException>(async Task () =>
            {
                SimpleListDataLayer dataLayer = new();
                List<SimpleDataObject> dataObjects = await dataLayer.CreateAsync([new SimpleDataObject(), new SimpleDataObject()]);

                dataObjects[0].Value = 1;
                dataObjects[1].Value = 9999;
                
                _ = await dataLayer.UpdateAsync(dataObjects);
            });
        }

        /// <summary>
        /// The method confirms if a non-existing ID is passed to the ListDataLayer.UpdateAsync(), an exception is thrown.
        /// </summary>
        [Fact]
        public async Task UpdateAsyncThrowsIDNotFoundException()
        {
            await Assert.ThrowsAsync<IDNotFoundException>(() => new SimpleListDataLayer().UpdateAsync(new SimpleDataObject() { Integer64ID = 99 }));
            await Assert.ThrowsAsync<IDNotFoundException>(() => new SimpleListDataLayer().UpdateAsync([new SimpleDataObject() { Integer64ID = 99 }]));
        }

        /// <summary>
        /// The method confirms if a null data object is passed to the ListDataLayer.ValidateAsync(), an exception is thrown.
        /// </summary>
        [Fact]
        public async Task ValidateAsyncThrowsArgumentNullException() => await Assert.ThrowsAsync<ArgumentNullException>(() => new SimpleListDataLayer().ValidateAsync(null));

        /// <summary>
        /// The method confirms ListDataLayer.ValidateAsync() works as intended.
        /// </summary>
        /// <returns>A Task object for the async.</returns>
        /// <remarks>
        /// The ValidateAsync() does the following:
        /// 
        /// 1. Validate the data annotations on the object. (SimpleDataObject.Value has a Range data annotation.)
        /// 2. The results is returned.
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

            Assert.True(dataAnnotationValid && dataAnnotationNotValided);
        }
    }
}
