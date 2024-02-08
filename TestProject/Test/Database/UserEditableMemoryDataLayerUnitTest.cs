using JMayer.Data.Data;
using JMayer.Data.Data.Query;
using JMayer.Data.Database.DataLayer;
using System.ComponentModel.DataAnnotations;
using TestProject.Data;
using TestProject.Database;

namespace TestProject.Test.Database;

/// <summary>
/// The class manages tests for the UserEditableMemoryDataLayer object.
/// </summary>
/// <remarks>
/// The tests are against a SimpleUserEditableMemoryListDataLayer object which inherits from 
/// the UserEditableMemoryDataLayer and the SimpleUserEditableMemoryListDataLayer doesn't override 
/// any of the base methods. Because of this, we're testing the methods in the 
/// UserEditableMemoryDataLayer class.
/// 
/// UserEditableMemoryDataLayer class inherits from the MemoryDataLayer. Because of this,
/// only new and overriden methods in the UserEditableMemoryDataLayer are tested because
/// the MemoryDataLayerUnitTest already tests the MemoryDataLayer.
/// 
/// All tests associated with UpdateAsync() in the MemoryDataLayerUnitTest are included
/// here because UserEditableMemoryDataLayer overrides UpdateAsync() and it doesn't call
/// the base.
/// </remarks>
public class UserEditableMemoryDataLayerUnitTest
{
    /// <summary>
    /// The method confirms UserEditableMemoryDataLayer.CreateAsync() works as intended.
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
        SimpleUserEditableDataObject originalDataObject = new() { Name = "A Name" };
        SimpleUserEditableMemoryDataLayer dataLayer = new();

        SimpleUserEditableDataObject firstReturnedCopiedDataObject = await dataLayer.CreateAsync(originalDataObject);
        SimpleUserEditableDataObject secondReturnedDataObject = await dataLayer.CreateAsync(originalDataObject);
        int count = await dataLayer.CountAsync();

        Assert.True
        (
            firstReturnedCopiedDataObject != null && secondReturnedDataObject != null //An object must have been returned.
            && originalDataObject != firstReturnedCopiedDataObject && originalDataObject != secondReturnedDataObject //Returned object must be a copy.
            && firstReturnedCopiedDataObject.Integer64ID > 0 && secondReturnedDataObject.Integer64ID > 0 //ID must have been set.
            && firstReturnedCopiedDataObject.CreatedOn > DateTime.MinValue && secondReturnedDataObject.CreatedOn > DateTime.MinValue //CreatedOn must have been set.
            && firstReturnedCopiedDataObject.Integer64ID != secondReturnedDataObject.Integer64ID && secondReturnedDataObject.Integer64ID - firstReturnedCopiedDataObject.Integer64ID == 1 //Internal identity must be incremented.
            && count == 2 //Data object was added to the data store.
        );
    }

    /// <summary>
    /// The method confirms UserEditableMemoryDataLayer.GetAllListViewAsync() works as intended.
    /// </summary>
    /// <returns>A Task object for the async.</returns>
    [Fact]
    public async Task GetAllListViewAsyncWorks()
    {
        SimpleUserEditableMemoryDataLayer dataLayer = new();
        _ = await dataLayer.CreateAsync(new SimpleUserEditableDataObject() { Name = "A Name" });
        List<ListView> listViews = await dataLayer.GetAllListViewAsync();
        Assert.True(listViews.Count == 1);
    }

    /// <summary>
    /// The method confirms UserEditableMemoryDataLayer.GetAllListViewAsync() works as intended for the where predicate.
    /// </summary>
    /// <returns>A Task object for the async.</returns>
    [Fact]
    public async Task GetAllAsyncWherePredicateWorks()
    {
        SimpleUserEditableMemoryDataLayer dataLayer = new();

        _ = await dataLayer.CreateAsync(new SimpleUserEditableDataObject() { Name = "10", Value = 10 });
        _ = await dataLayer.CreateAsync(new SimpleUserEditableDataObject() { Name = "20", Value = 20 });
        _ = await dataLayer.CreateAsync(new SimpleUserEditableDataObject() { Name = "30", Value = 30 });
        _ = await dataLayer.CreateAsync(new SimpleUserEditableDataObject() { Name = "40", Value = 40 });
        _ = await dataLayer.CreateAsync(new SimpleUserEditableDataObject() { Name = "50", Value = 50 });
        _ = await dataLayer.CreateAsync(new SimpleUserEditableDataObject() { Name = "60", Value = 60 });
        _ = await dataLayer.CreateAsync(new SimpleUserEditableDataObject() { Name = "70", Value = 70 });
        _ = await dataLayer.CreateAsync(new SimpleUserEditableDataObject() { Name = "80", Value = 80 });
        _ = await dataLayer.CreateAsync(new SimpleUserEditableDataObject() { Name = "90", Value = 90 });
        _ = await dataLayer.CreateAsync(new SimpleUserEditableDataObject() { Name = "100", Value = 100 });

        List<ListView> lessThan40DataObjects = await dataLayer.GetAllListViewAsync(obj => obj.Value < 40);
        List<ListView> greaterThan80DataObjects = await dataLayer.GetAllListViewAsync(obj => obj.Value > 80);
        List<ListView> failedToFindDataObjects = await dataLayer.GetAllListViewAsync(obj => obj.Value == 1);

        Assert.True(lessThan40DataObjects.Count == 3 && greaterThan80DataObjects.Count == 2 && failedToFindDataObjects.Count == 0);
    }

    /// <summary>
    /// The method confirms UserEditableMemoryDataLayer.GetAllListViewAsync() works as intended for the order predicate.
    /// </summary>
    /// <returns>A Task object for the async.</returns>
    [Fact]
    public async Task GetAllAsyncOrderPredicateWorks()
    {
        SimpleUserEditableMemoryDataLayer dataLayer = new();

        _ = await dataLayer.CreateAsync(new SimpleUserEditableDataObject() { Name = "10", Value = 10 });
        _ = await dataLayer.CreateAsync(new SimpleUserEditableDataObject() { Name = "20", Value = 20 });
        _ = await dataLayer.CreateAsync(new SimpleUserEditableDataObject() { Name = "30", Value = 30 });
        _ = await dataLayer.CreateAsync(new SimpleUserEditableDataObject() { Name = "40", Value = 40 });
        _ = await dataLayer.CreateAsync(new SimpleUserEditableDataObject() { Name = "50", Value = 50 });
        _ = await dataLayer.CreateAsync(new SimpleUserEditableDataObject() { Name = "60", Value = 60 });
        _ = await dataLayer.CreateAsync(new SimpleUserEditableDataObject() { Name = "70", Value = 70 });
        _ = await dataLayer.CreateAsync(new SimpleUserEditableDataObject() { Name = "80", Value = 80 });
        _ = await dataLayer.CreateAsync(new SimpleUserEditableDataObject() { Name = "90", Value = 90 });
        _ = await dataLayer.CreateAsync(new SimpleUserEditableDataObject() { Name = "100", Value = 100 });

        List<ListView> ascOrderByDataObjects = await dataLayer.GetAllListViewAsync(orderByPredicate: obj => obj.Value);
        List<ListView> descOrderByDataObjects = await dataLayer.GetAllListViewAsync(orderByPredicate: obj => obj.Value, descending: true);

        Assert.True(ascOrderByDataObjects.First().Name == "10" && descOrderByDataObjects.First().Name == "100");
    }

    /// <summary>
    /// The method confirms UserEditableMemoryDataLayer.GetAllListViewAsync() works as intended for the where and order predicate.
    /// </summary>
    /// <returns>A Task object for the async.</returns>
    [Fact]
    public async Task GetAllAsyncWherePredicateOrderPredicateWorks()
    {
        SimpleUserEditableMemoryDataLayer dataLayer = new();

        _ = await dataLayer.CreateAsync(new SimpleUserEditableDataObject() { Name = "10", Value = 10 });
        _ = await dataLayer.CreateAsync(new SimpleUserEditableDataObject() { Name = "20", Value = 20 });
        _ = await dataLayer.CreateAsync(new SimpleUserEditableDataObject() { Name = "30", Value = 30 });
        _ = await dataLayer.CreateAsync(new SimpleUserEditableDataObject() { Name = "40", Value = 40 });
        _ = await dataLayer.CreateAsync(new SimpleUserEditableDataObject() { Name = "50", Value = 50 });
        _ = await dataLayer.CreateAsync(new SimpleUserEditableDataObject() { Name = "60", Value = 60 });
        _ = await dataLayer.CreateAsync(new SimpleUserEditableDataObject() { Name = "70", Value = 70 });
        _ = await dataLayer.CreateAsync(new SimpleUserEditableDataObject() { Name = "80", Value = 80 });
        _ = await dataLayer.CreateAsync(new SimpleUserEditableDataObject() { Name = "90", Value = 90 });
        _ = await dataLayer.CreateAsync(new SimpleUserEditableDataObject() { Name = "100", Value = 100 });

        List<ListView> whereAscOrderByDataObjects = await dataLayer.GetAllListViewAsync(obj => obj.Value > 30, obj => obj.Value);
        List<ListView> whereDescOrderByDataObjects = await dataLayer.GetAllListViewAsync(obj => obj.Value < 60, obj => obj.Value, true);

        Assert.True(whereAscOrderByDataObjects.First().Name == "40" && whereDescOrderByDataObjects.First().Name == "50");
    }

    /// <summary>
    /// The method confirms if a null data object is passed to the UserEditableMemoryDataLayer.GetPageListViewAsync(), an exception is thrown.
    /// </summary>
    [Fact]
    public async Task GetPageAsyncThrowsArgumentNullException() => await Assert.ThrowsAsync<ArgumentNullException>(() => new SimpleUserEditableMemoryDataLayer().GetPageAsync(null));

    /// <summary>
    /// The method confirms UserEditableMemoryDataLayer.GetPageListViewAsync() works as intended.
    /// </summary>
    /// <returns>A Task object for the async.</returns>
    [Fact]
    public async Task GetPageListViewAsyncWorks()
    {
        SimpleUserEditableMemoryDataLayer dataLayer = new();
        List<SimpleUserEditableDataObject> dataObjects = [];

        for (int index = 1; index <= 100; index++)
        {
            dataObjects.Add(new SimpleUserEditableDataObject()
            {
                Name = index.ToString(),
                Value = index,
            });
        }

        await dataLayer.CreateAsync(dataObjects);

        QueryDefinition skipAndTakeQueryDefinition = new()
        {
            Skip = 1,
            Take = 20,
        };
        PagedList<ListView> skipAndTakePage = await dataLayer.GetPageListViewAsync(skipAndTakeQueryDefinition);


        QueryDefinition filterQueryDefinition = new();
        filterQueryDefinition.FilterDefinitions.Add(new FilterDefinition()
        {
            FilterOn = nameof(SimpleUserEditableDataObject.Value),
            Operator = FilterDefinition.EqualsOperator,
            Value = "50",
        });
        PagedList<ListView> filterPage = await dataLayer.GetPageListViewAsync(filterQueryDefinition);

        QueryDefinition sortQueryDefinition = new();
        sortQueryDefinition.SortDefinitions.Add(new SortDefinition()
        {
            Descending = true,
            SortOn = nameof(SimpleUserEditableDataObject.Value),
        });
        PagedList<ListView> sortPage = await dataLayer.GetPageListViewAsync(sortQueryDefinition);

        Assert.True
        (
            skipAndTakePage.TotalRecords == 100 && skipAndTakePage.DataObjects.Count == 20 && skipAndTakePage.DataObjects.First().Name == "21"
            && filterPage.TotalRecords == 1 && filterPage.DataObjects.Count == 1 && filterPage.DataObjects.First().Name == "50"
            && sortPage.TotalRecords == 100 && sortPage.DataObjects.Count == 100 && sortPage.DataObjects.First().Name == "100"
        );
    }

    /// <summary>
    /// The method confirms UserEditableMemoryDataLayer.UpdateAsync() works as intended.
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
    public async Task UpdateAsyncDataObjectWorks()
    {
        SimpleUserEditableMemoryDataLayer dataLayer = new();
        SimpleUserEditableDataObject originalDataObject = await dataLayer.CreateAsync(new SimpleUserEditableDataObject() { Name = "A Name" });

        originalDataObject.Value = 10;
        SimpleUserEditableDataObject returnedCopiedDataObject = await dataLayer.UpdateAsync(originalDataObject);
        SimpleUserEditableDataObject? confirmedDataObject = await dataLayer.GetSingleAsync();

        Assert.True
        (
            returnedCopiedDataObject != null //An object must have been returned.
            && originalDataObject != returnedCopiedDataObject //Returned object must be a copy.
            && originalDataObject.Value == confirmedDataObject?.Value //Confirm the data was updated.
            && returnedCopiedDataObject.LastEditedOn != null //LastEditedOn must have been set.
        );
    }

    /// <summary>
    /// The method confirms the UserEditableMemoryDataLayer.Updated event fires when UserEditableMemoryDataLayer.UpdateAsync() is called.
    /// </summary>
    [Fact]
    public async Task UpdateAsyncFiresUpdatedEvent()
    {
        SimpleUserEditableMemoryDataLayer dataLayer = new();
        await Assert.RaisesAsync<UpdatedEventArgs>
        (
            handler => dataLayer.Updated += handler,
            handler => dataLayer.Updated -= handler,
            async Task () =>
            {
                SimpleUserEditableDataObject dataObject = await dataLayer.CreateAsync(new SimpleUserEditableDataObject() { Name = "A Name" });
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
                List<SimpleUserEditableDataObject> dataObjects = await dataLayer.CreateAsync([new SimpleUserEditableDataObject() { Name = "Another Name" }, new SimpleUserEditableDataObject() { Name = " Yet Another Name" }]);

                dataObjects[0].Value = 10;
                dataObjects[1].Value = 20;

                _ = await dataLayer.UpdateAsync(dataObjects);
            }
        );
    }

    /// <summary>
    /// The method confirms UserEditableMemoryDataLayer.UpdateAsync() works as intended.
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
    public async Task UpdateAsyncListWorks()
    {
        SimpleUserEditableMemoryDataLayer dataLayer = new();
        List<SimpleUserEditableDataObject> originalDataObjects = await dataLayer.CreateAsync([new SimpleUserEditableDataObject() { Name = "A Name" }, new SimpleUserEditableDataObject() { Name = "Another Name" }]);

        originalDataObjects[0].Value = 10;
        originalDataObjects[1].Value = 20;

        List<SimpleUserEditableDataObject> returnedCopiedDataObjects = await dataLayer.UpdateAsync(originalDataObjects);
        List<SimpleUserEditableDataObject> confirmedDataObjects = await dataLayer.GetAllAsync();

        Assert.True
        (
            returnedCopiedDataObjects != null && returnedCopiedDataObjects.Count == 2 //An object must have been returned and the same amount updated must be returned.
            && originalDataObjects[0] != returnedCopiedDataObjects[0] && originalDataObjects[1] != returnedCopiedDataObjects[1] //Returned objects must be a copy.
            && originalDataObjects[0].Value == confirmedDataObjects[0].Value && originalDataObjects[1].Value == confirmedDataObjects[1].Value //Confirm the data was updated.
        );
    }

    /// <summary>
    /// The method confirms if a null data object is passed to the UserEditableMemoryDataLayer.UpdateAsync(), an exception is thrown.
    /// </summary>
    [Fact]
    public async Task UpdateAsyncThrowsArgumentNullException()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() => new SimpleUserEditableMemoryDataLayer().UpdateAsync((SimpleUserEditableDataObject?)null));
        await Assert.ThrowsAsync<ArgumentNullException>(() => new SimpleUserEditableMemoryDataLayer().UpdateAsync((List<SimpleUserEditableDataObject>)null));
    }

    /// <summary>
    /// The method confirms if an invalid data object is passed to the UserEditableMemoryDataLayer.UpdateAsync(), an exception is thrown.
    /// </summary>
    [Fact]
    public async Task UpdateAsyncThrowsDataObjectValidationException()
    {
        await Assert.ThrowsAsync<DataObjectValidationException>(async Task () =>
        {
            SimpleUserEditableMemoryDataLayer dataLayer = new();
            SimpleUserEditableDataObject dataObject = await dataLayer.CreateAsync(new SimpleUserEditableDataObject() { Name = "A Name" });

            dataObject.Name = null;
            _ = await dataLayer.UpdateAsync(dataObject);
        });
        await Assert.ThrowsAsync<DataObjectValidationException>(async Task () =>
        {
            SimpleUserEditableMemoryDataLayer dataLayer = new();
            List<SimpleUserEditableDataObject> dataObjects = await dataLayer.CreateAsync([new SimpleUserEditableDataObject() { Name = "A Name" }, new SimpleUserEditableDataObject() { Name = "Another Name" }]);

            dataObjects[0].Value = 1;
            dataObjects[1].Value = 9999;

            _ = await dataLayer.UpdateAsync(dataObjects);
        });
    }

    /// <summary>
    /// The method confirms if a non-existing ID is passed to the UserEditableMemoryDataLayer.UpdateAsync(), an exception is thrown.
    /// </summary>
    [Fact]
    public async Task UpdateAsyncThrowsIDNotFoundException()
    {
        await Assert.ThrowsAsync<IDNotFoundException>(() => new SimpleUserEditableMemoryDataLayer().UpdateAsync(new SimpleUserEditableDataObject() { Integer64ID = 99, Name = "A Name" }));
        await Assert.ThrowsAsync<IDNotFoundException>(() => new SimpleUserEditableMemoryDataLayer().UpdateAsync([new SimpleUserEditableDataObject() { Integer64ID = 99, Name = "A Name" }, new SimpleUserEditableDataObject() { Integer64ID = 100, Name = "Another Name" }]));
    }

    /// <summary>
    /// The method confirms if old data is being updated in UserEditableMemoryDataLayer.UpdateAsync(), an exception is thrown.
    /// </summary>
    [Fact]
    public async Task UpdateAsyncThrowsUpdateConflictException()
    {
        await Assert.ThrowsAsync<DataObjectUpdateConflictException>(async Task () =>
        {
            SimpleUserEditableMemoryDataLayer dataLayer = new();
            SimpleUserEditableDataObject originalDataObject = await dataLayer.CreateAsync(new SimpleUserEditableDataObject() { Name = "A Name" });

            _ = await dataLayer.UpdateAsync(new SimpleUserEditableDataObject(originalDataObject)
            {
                Value = 10,
            });

            originalDataObject.Value = 20;
            _ = await dataLayer.UpdateAsync(originalDataObject);
        });
        await Assert.ThrowsAsync<DataObjectUpdateConflictException>(async Task () =>
        {
            SimpleUserEditableMemoryDataLayer dataLayer = new();
            List<SimpleUserEditableDataObject> originalDataObjects = await dataLayer.CreateAsync([new SimpleUserEditableDataObject() { Name = "A Name" }, new SimpleUserEditableDataObject() { Name = "Another Name" }]);

            _ = await dataLayer.UpdateAsync([new SimpleUserEditableDataObject(originalDataObjects[0]) { Value = 10 }, new SimpleUserEditableDataObject(originalDataObjects[1]) { Value = 20 }]);

            originalDataObjects[0].Value = 20;
            originalDataObjects[1].Value = 30;

            _ = await dataLayer.UpdateAsync(originalDataObjects);
        });
    }

    /// <summary>
    /// The method confirms if a null data object is passed to the UserEditableMemoryDataLayer.ValidateAsync(), an exception is thrown.
    /// </summary>
    [Fact]
    public async Task ValidateAsyncThrowsArgumentNullException() => await Assert.ThrowsAsync<ArgumentNullException>(() => new SimpleUserEditableMemoryDataLayer().ValidateAsync(null));

    /// <summary>
    /// The method confirms UserEditableMemoryDataLayer.ValidateAsync() works as intended.
    /// </summary>
    /// <returns>A Task object for the async.</returns>
    /// <remarks>
    /// The ValidateAsync() does the following:
    /// 
    /// 1. Validate the data annotations on the object. (SimpleConfigurationDataObject.Value has a Range data annotation.)
    /// 2. Validate against any custom rules. (ConfigurationListDataLayer must have a unique name in the collection/table.)
    /// 3. The results is returned.
    /// </remarks>
    [Fact]
    public async Task ValidateAsyncWorks()
    {
        SimpleUserEditableMemoryDataLayer dataLayer = new();
        SimpleUserEditableDataObject dataObject = await dataLayer.CreateAsync(new SimpleUserEditableDataObject() { Name = "A Name" });
        _ = await dataLayer.CreateAsync(new SimpleUserEditableDataObject() { Name = "Another Name" });

        dataObject.Name = "Different Name";
        dataObject.Value = 10;
        List<ValidationResult> validationResults = await dataLayer.ValidateAsync(dataObject);
        bool dataAnnotationValid = validationResults.Count == 0;

        dataObject.Value = 9999;
        validationResults = await dataLayer.ValidateAsync(dataObject);
        bool dataAnnotationNotValided = validationResults.Count != 0;

        dataObject.Value = 0;
        validationResults = await dataLayer.ValidateAsync(dataObject);
        bool nameValid = validationResults.Count == 0;

        dataObject.Integer64ID = 2;
        dataObject.Name = "A Name";
        validationResults = await dataLayer.ValidateAsync(dataObject);
        bool nameNotValid = validationResults.Count != 0;

        Assert.True(dataAnnotationValid && dataAnnotationNotValided && nameValid && nameNotValid);
    }
}
