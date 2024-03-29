﻿using JMayer.Data.Data;
using JMayer.Data.Data.Query;
using JMayer.Data.Database.DataLayer;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using TestProject.Data;
using TestProject.Database;

namespace TestProject.Test.Database;

/// <summary>
/// The class manages tests for the MemoryDataLayer object.
/// </summary>
/// <remarks>
/// The tests are against a SimpleMemoryDataLayer object which inherits from the MemoryDataLayer and
/// the SimpleMemoryDataLayer doesn't override any of the base methods. Because of this, we're testing 
/// the methods in the MemoryDataLayer class.
/// </remarks>
public class MemoryDataLayerUnitTest
{
    /// <summary>
    /// The method confirms if a null data object is passed to the MemoryDataLayer.CountAsync(), an exception is thrown.
    /// </summary>
    [Fact]
    public async Task CountAsyncThrowsArgumentNullException() => await Assert.ThrowsAsync<ArgumentNullException>(() => new SimpleMemoryDataLayer().CountAsync(null));

    /// <summary>
    /// The method confirms the MemoryDataLayer.CountAsync() works as intended.
    /// </summary>
    /// <returns>A Task object for the async.</returns>
    [Fact]
    public async Task CountAsyncWorks()
    {
        SimpleMemoryDataLayer dataLayer = new();

        _ = await dataLayer.CreateAsync(new SimpleDataObject());
        int count = await dataLayer.CountAsync();

        Assert.Equal(1, count);
    }

    /// <summary>
    /// The method confirms the MemoryDataLayer.CountAsync() works as intended for the where predicate.
    /// </summary>
    /// <returns>A Task object for the async.</returns>
    [Fact]
    public async Task CountAsyncWherePredicateWorks()
    {
        SimpleMemoryDataLayer dataLayer = new();

        _ = await dataLayer.CreateAsync(new SimpleDataObject());
        _ = await dataLayer.CreateAsync(new SimpleDataObject());
        _ = await dataLayer.CreateAsync(new SimpleDataObject());
        _ = await dataLayer.CreateAsync(new SimpleDataObject());

        int oneCount = await dataLayer.CountAsync(obj => obj.Integer64ID == 1);
        int zeroCount = await dataLayer.CountAsync(obj => obj.Integer64ID == 99);

        Assert.True(oneCount == 1 && zeroCount == 0);
    }

    /// <summary>
    /// The method confirms MemoryDataLayer.CreateAsync() works as intended.
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
        SimpleMemoryDataLayer dataLayer = new();

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
    /// The method confirms the MemoryDataLayer.Created event fires when MemoryDataLayer.CreateAsync() is called.
    /// </summary>
    [Fact]
    public async Task CreateAsyncFiresCreatedEvent()
    {
        SimpleMemoryDataLayer dataLayer = new();
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
    /// The method confirms MemoryDataLayer.CreateAsync() works as intended.
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
        SimpleMemoryDataLayer dataLayer = new();

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
    /// The method confirms if a null data object is passed to the MemoryDataLayer.CreateAsync(), an exception is thrown.
    /// </summary>
    [Fact]
    public async Task CreateAsyncThrowsArgumentNullException()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() => new SimpleMemoryDataLayer().CreateAsync((SimpleDataObject)null));
        await Assert.ThrowsAsync<ArgumentNullException>(() => new SimpleMemoryDataLayer().CreateAsync((List<SimpleDataObject>)null));
    }

    /// <summary>
    /// The method confirms if an invalid data object is passed to the MemoryDataLayer.CreateAsync(), an exception is thrown.
    /// </summary>
    [Fact]
    public async Task CreateAsyncThrowsDataObjectValidationException()
    {
        await Assert.ThrowsAsync<DataObjectValidationException>(() => new SimpleMemoryDataLayer().CreateAsync(new SimpleDataObject() { Value = 9999 }));
        await Assert.ThrowsAsync<DataObjectValidationException>(() => new SimpleMemoryDataLayer().CreateAsync([new SimpleDataObject() { Value = 1 }, new SimpleDataObject() { Value = 9999 }]));
    }

    /// <summary>
    /// The method confirms MemoryDataLayer.DeleteAsync() works as intended.
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
        SimpleMemoryDataLayer dataLayer = new();

        SimpleDataObject dataObject = await dataLayer.CreateAsync(new SimpleDataObject());
        await dataLayer.DeleteAsync(dataObject);
        int count = await dataLayer.CountAsync();

        Assert.Equal(0, count);
    }

    /// <summary>
    /// The method confirms the MemoryDataLayer.Deleted event fires when MemoryDataLayer.DeleteAsync() is called.
    /// </summary>
    [Fact]
    public async Task DeleteAsyncFiresDeletedEvent()
    {
        SimpleMemoryDataLayer dataLayer = new();
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
    /// The method confirms MemoryDataLayer.DeleteAsync() works as intended.
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
        SimpleMemoryDataLayer dataLayer = new();
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
    /// The method confirms if a null data object is passed to the MemoryDataLayer.DeleteAsync(), an exception is thrown.
    /// </summary>
    [Fact]
    public async Task DeleteAsyncThrowsArgumentNullException()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() => new SimpleMemoryDataLayer().DeleteAsync((SimpleDataObject)null));
        await Assert.ThrowsAsync<ArgumentNullException>(() => new SimpleMemoryDataLayer().DeleteAsync((List<SimpleDataObject>)null));
        await Assert.ThrowsAsync<ArgumentNullException>(() => new SimpleMemoryDataLayer().DeleteAsync((Expression<Func<SimpleDataObject, bool>>)null));
    }

    /// <summary>
    /// The method confirms MemoryDataLayer.DeleteAsync() works as intended.
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
        SimpleMemoryDataLayer dataLayer = new();

        _ = await dataLayer.CreateAsync(new SimpleDataObject());
        _ = await dataLayer.CreateAsync(new SimpleDataObject());
        _ = await dataLayer.CreateAsync(new SimpleDataObject());
        _ = await dataLayer.CreateAsync(new SimpleDataObject());

        await dataLayer.DeleteAsync(obj => obj.Integer64ID > 2);
        int count = await dataLayer.CountAsync();

        Assert.Equal(2, count);
    }

    /// <summary>
    /// The method confirms if a null data object is passed to the MemoryDataLayer.ExistAsync(), an exception is thrown.
    /// </summary>
    [Fact]
    public async Task ExistAsyncThrowsArgumentNullException() => await Assert.ThrowsAsync<ArgumentNullException>(() => new SimpleMemoryDataLayer().ExistAsync(null));

    /// <summary>
    /// The method confirms MemoryDataLayer.ExistAsync() works as intended for the where predicate.
    /// </summary>
    /// <returns>A Task object for the async.</returns>
    [Fact]
    public async Task ExistAsyncWherePredicateWorks()
    {
        SimpleMemoryDataLayer dataLayer = new();
        _ = await dataLayer.CreateAsync(new SimpleDataObject());

        bool found = await dataLayer.ExistAsync(obj => obj.Integer64ID == 1);
        bool notFound = await dataLayer.ExistAsync(obj => obj.Integer64ID == 99) == false;

        Assert.True(found && notFound);
    }

    /// <summary>
    /// The method confirms MemoryDataLayer.GetAllAsync() works as intended.
    /// </summary>
    /// <returns>A Task object for the async.</returns>
    [Fact]
    public async Task GetAllAsyncWorks()
    {
        SimpleMemoryDataLayer dataLayer = new();
        _ = await dataLayer.CreateAsync(new SimpleDataObject());
        List<SimpleDataObject> dataObjects = await dataLayer.GetAllAsync();
        Assert.True(dataObjects.Count == 1);
    }

    /// <summary>
    /// The method confirms MemoryDataLayer.GetAllAsync() works as intended for the where predicate.
    /// </summary>
    /// <returns>A Task object for the async.</returns>
    [Fact]
    public async Task GetAllAsyncWherePredicateWorks()
    {
        SimpleMemoryDataLayer dataLayer = new();

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
    /// The method confirms MemoryDataLayer.GetAllAsync() works as intended for the order predicate.
    /// </summary>
    /// <returns>A Task object for the async.</returns>
    [Fact]
    public async Task GetAllAsyncOrderPredicateWorks()
    {
        SimpleMemoryDataLayer dataLayer = new();

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

        List<SimpleDataObject> ascOrderByDataObjects = await dataLayer.GetAllAsync(orderByPredicate: obj => obj.Value);
        List<SimpleDataObject> descOrderByDataObjects = await dataLayer.GetAllAsync(orderByPredicate: obj => obj.Value, descending: true);

        Assert.True(ascOrderByDataObjects.First().Value == 10 && descOrderByDataObjects.First().Value == 100);
    }

    /// <summary>
    /// The method confirms MemoryDataLayer.GetAllAsync() works as intended for the where and order predicate.
    /// </summary>
    /// <returns>A Task object for the async.</returns>
    [Fact]
    public async Task GetAllAsyncWherePredicateOrderPredicateWorks()
    {
        SimpleMemoryDataLayer dataLayer = new();

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
    /// The method confirms if a null data object is passed to the MemoryDataLayer.GetPageAsync(), an exception is thrown.
    /// </summary>
    [Fact]
    public async Task GetPageAsyncThrowsArgumentNullException() => await Assert.ThrowsAsync<ArgumentNullException>(() => new SimpleMemoryDataLayer().GetPageAsync(null));

    /// <summary>
    /// The method confirms MemoryDataLayer.GetPageAsync() works as intended.
    /// </summary>
    /// <returns>A Task object for the async.</returns>
    [Fact]
    public async Task GetPageAsyncWorks()
    {
        SimpleMemoryDataLayer dataLayer = new();
        List<SimpleDataObject> dataObjects = [];

        for (int index = 1; index <= 100; index++)
        {
            dataObjects.Add(new SimpleDataObject()
            {
                Value = index,
            });
        }

        await dataLayer.CreateAsync(dataObjects);

        QueryDefinition skipAndTakeQueryDefinition = new()
        {
            Skip = 1,
            Take = 20,
        };
        PagedList<SimpleDataObject> skipAndTakePage = await dataLayer.GetPageAsync(skipAndTakeQueryDefinition);


        QueryDefinition filterQueryDefinition = new();
        filterQueryDefinition.FilterDefinitions.Add(new FilterDefinition()
        {
            FilterOn = nameof(SimpleDataObject.Value),
            Operator = FilterDefinition.EqualsOperator,
            Value = "50",
        });
        PagedList<SimpleDataObject> filterPage = await dataLayer.GetPageAsync(filterQueryDefinition);

        QueryDefinition sortQueryDefinition = new();
        sortQueryDefinition.SortDefinitions.Add(new SortDefinition()
        {
            Descending = true,
            SortOn = nameof(SimpleDataObject.Value),
        });
        PagedList<SimpleDataObject> sortPage = await dataLayer.GetPageAsync(sortQueryDefinition);

        Assert.True
        (
            skipAndTakePage.TotalRecords == 100 && skipAndTakePage.DataObjects.Count == 20 && skipAndTakePage.DataObjects.First().Value == 21
            && filterPage.TotalRecords == 1 && filterPage.DataObjects.Count == 1 && filterPage.DataObjects.First().Value == 50
            && sortPage.TotalRecords == 100 && sortPage.DataObjects.Count == 100 && sortPage.DataObjects.First().Value == 100
        );
    }

    /// <summary>
    /// The method confirms MemoryDataLayer.GetSingleAsync() works as intended.
    /// </summary>
    /// <returns>A Task object for the async.</returns>
    [Fact]
    public async Task GetSingleAsyncWorks()
    {
        SimpleMemoryDataLayer dataLayer = new();

        bool notFound = await dataLayer.GetSingleAsync() == null;
        _ = await dataLayer.CreateAsync(new SimpleDataObject());
        bool found = await dataLayer.GetSingleAsync() != null;

        Assert.True(found && notFound);
    }

    /// <summary>
    /// The method confirms MemoryDataLayer.GetSingleAsync() works as intended for the wherepredicate.
    /// </summary>
    /// <returns>A Task object for the async.</returns>
    [Fact]
    public async Task GetSingleAsyncWherePredicateWorks()
    {
        SimpleMemoryDataLayer dataLayer = new();

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
    /// The method confirms MemoryDataLayer.UpdateAsync() works as intended.
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
        SimpleMemoryDataLayer dataLayer = new();
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
    /// The method confirms the MemoryDataLayer.Updated event fires when MemoryDataLayer.UpdateAsync() is called.
    /// </summary>
    [Fact]
    public async Task UpdateAsyncFiresUpdatedEvent()
    {
        SimpleMemoryDataLayer dataLayer = new();
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
    /// The method confirms MemoryDataLayer.UpdateAsync() works as intended.
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
        SimpleMemoryDataLayer dataLayer = new();
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
    /// The method confirms if a null data object is passed to the MemoryDataLayer.UpdateAsync(), an exception is thrown.
    /// </summary>
    [Fact]
    public async Task UpdateAsyncThrowsArgumentNullException()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() => new SimpleMemoryDataLayer().UpdateAsync((SimpleDataObject)null));
        await Assert.ThrowsAsync<ArgumentNullException>(() => new SimpleMemoryDataLayer().UpdateAsync((List<SimpleDataObject>)null));
    }

    /// <summary>
    /// The method confirms if an invalid data object is passed to the MemoryDataLayer.UpdateAsync(), an exception is thrown.
    /// </summary>
    [Fact]
    public async Task UpdateAsyncThrowsDataObjectValidationException()
    {
        await Assert.ThrowsAsync<DataObjectValidationException>(async Task () =>
        {
            SimpleMemoryDataLayer dataLayer = new();
            SimpleDataObject dataObject = await dataLayer.CreateAsync(new SimpleDataObject());

            dataObject.Value = 9999;

            _ = await dataLayer.UpdateAsync(dataObject);
        });
        await Assert.ThrowsAsync<DataObjectValidationException>(async Task () =>
        {
            SimpleMemoryDataLayer dataLayer = new();
            List<SimpleDataObject> dataObjects = await dataLayer.CreateAsync([new SimpleDataObject(), new SimpleDataObject()]);

            dataObjects[0].Value = 1;
            dataObjects[1].Value = 9999;

            _ = await dataLayer.UpdateAsync(dataObjects);
        });
    }

    /// <summary>
    /// The method confirms if a non-existing ID is passed to the MemoryDataLayer.UpdateAsync(), an exception is thrown.
    /// </summary>
    [Fact]
    public async Task UpdateAsyncThrowsIDNotFoundException()
    {
        await Assert.ThrowsAsync<IDNotFoundException>(() => new SimpleMemoryDataLayer().UpdateAsync(new SimpleDataObject() { Integer64ID = 99 }));
        await Assert.ThrowsAsync<IDNotFoundException>(() => new SimpleMemoryDataLayer().UpdateAsync([new SimpleDataObject() { Integer64ID = 99 }]));
    }

    /// <summary>
    /// The method confirms if a null data object is passed to the MemoryDataLayer.ValidateAsync(), an exception is thrown.
    /// </summary>
    [Fact]
    public async Task ValidateAsyncThrowsArgumentNullException() => await Assert.ThrowsAsync<ArgumentNullException>(() => new SimpleMemoryDataLayer().ValidateAsync(null));

    /// <summary>
    /// The method confirms MemoryDataLayer.ValidateAsync() works as intended.
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
        SimpleMemoryDataLayer dataLayer = new();
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
