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
/// The tests are against a SimpleUserEditableDataLayer object which inherits from 
/// the UserEditableMemoryDataLayer and the SimpleUserEditableDataLayer doesn't override 
/// any of the base methods. Because of this, we're testing the methods in the 
/// UserEditableMemoryDataLayer class.
/// 
/// UserEditableMemoryDataLayer class inherits from the MemoryDataLayer. Because of this,
/// only new and overriden methods in the UserEditableMemoryDataLayer are tested because
/// the StandardCRUDDataLayerUnitTest already tests the MemoryDataLayer.
/// 
/// All tests associated with UpdateAsync() in the StandardCRUDDataLayerUnitTest are included
/// here because UserEditableMemoryDataLayer overrides UpdateAsync() and it doesn't call
/// the base.
/// </remarks>
public class UserEditableDataLayerUnitTest
{
    /// <summary>
    /// The constant for another name.
    /// </summary>
    private const string AnotherName = "Another Name";

    /// <summary>
    /// The constant for the default name.
    /// </summary>
    private const string DefaultName = "A Name";

    /// <summary>
    /// The constant for a different name.
    /// </summary>
    private const string DifferentName = "Different Name";

    /// <summary>
    /// The constant for the default value.
    /// </summary>
    private const int DefaultValue = 1;

    /// <summary>
    /// The constant for the not found id.
    /// </summary>
    private const long NotFoundId = 99;

    /// <summary>
    /// The constant for an invalid value.
    /// </summary>
    private const int InvalidValue = 9999;

    /// <summary>
    /// The method populates data objects in a datalayer with values that are divisible by 10.
    /// </summary>
    /// <param name="dataLayer">The data layer to populate</param>
    /// <returns>A Task object for the async.</returns>
    private static async Task PopulateDataObjectsDivisibleBy10Async(SimpleUserEditableDataLayer dataLayer)
    {
        for (int index = 1; index <= 10; index++)
        {
            _ = await dataLayer.CreateAsync(new SimpleUserEditableDataObject() { Name = (10 * index).ToString(), Value = 10 * index });
        }
    }

    /// <summary>
    /// The method verifies UserEditableMemoryDataLayer.CreateAsync() creates a single data object.
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
    public async Task VerifyCreate()
    {
        SimpleUserEditableDataObject originalDataObject = new() { Name = DefaultName };
        SimpleUserEditableDataLayer dataLayer = new();

        SimpleUserEditableDataObject firstReturnedCopiedDataObject = await dataLayer.CreateAsync(originalDataObject);
        SimpleUserEditableDataObject secondReturnedDataObject = await dataLayer.CreateAsync(originalDataObject);
        long count = await dataLayer.CountAsync();

        //An object must have been returned.
        Assert.NotNull(firstReturnedCopiedDataObject);
        Assert.NotNull(secondReturnedDataObject);

        //Returned object must be a copy.
        Assert.NotEqual(originalDataObject, firstReturnedCopiedDataObject);
        Assert.NotEqual(originalDataObject, secondReturnedDataObject);

        //ID must have been set by the datalayer.
        Assert.NotEqual(0, firstReturnedCopiedDataObject.Integer64ID);
        Assert.NotEqual(0, secondReturnedDataObject.Integer64ID);

        //Created on must have been set by the datalayer.
        Assert.NotEqual(DateTime.MinValue, firstReturnedCopiedDataObject.CreatedOn);
        Assert.NotEqual(DateTime.MinValue, secondReturnedDataObject.CreatedOn);

        //Datalayer's internal identity must be incremented.
        Assert.NotEqual(firstReturnedCopiedDataObject.Integer64ID, secondReturnedDataObject.Integer64ID);
        Assert.Equal(1, secondReturnedDataObject.Integer64ID - firstReturnedCopiedDataObject.Integer64ID);

        //Data objects were added to the data store.
        Assert.Equal(2, count);
    }

    /// <summary>
    /// The method verifies UserEditableMemoryDataLayer.GetAllListViewAsync() returns all data objects as a list view.
    /// </summary>
    /// <returns>A Task object for the async.</returns>
    [Fact]
    public async Task VerifyGetAllListView()
    {
        SimpleUserEditableDataLayer dataLayer = new();
        _ = await dataLayer.CreateAsync(new SimpleUserEditableDataObject() { Name = DefaultName });
        List <ListView> listViews = await dataLayer.GetAllListViewAsync();
        Assert.Single(listViews);
    }

    /// <summary>
    /// The method verifies UserEditableMemoryDataLayer.GetAllListViewAsync() returns all data objects as a list view but with a specific order.
    /// </summary>
    /// <returns>A Task object for the async.</returns>
    [Fact]
    public async Task VerifyGetAllListViewWithOrderPredicate()
    {
        SimpleUserEditableDataLayer dataLayer = new();

        await PopulateDataObjectsDivisibleBy10Async(dataLayer);

        List<ListView> ascOrderByDataObjects = await dataLayer.GetAllListViewAsync(orderByPredicate: obj => obj.Value);
        List<ListView> descOrderByDataObjects = await dataLayer.GetAllListViewAsync(orderByPredicate: obj => obj.Value, descending: true);

        Assert.Equal("10", ascOrderByDataObjects.First().Name);
        Assert.Equal("100", descOrderByDataObjects.First().Name);
    }

    /// <summary>
    /// The method verifies UserEditableMemoryDataLayer.GetAllListViewAsync() returns the data objects as a list view based on a where predicate and with a specific order.
    /// </summary>
    /// <returns>A Task object for the async.</returns>
    [Fact]
    public async Task VerifyGetAllListViewWithWhereOrderPredicate()
    {
        SimpleUserEditableDataLayer dataLayer = new();

        await PopulateDataObjectsDivisibleBy10Async(dataLayer);

        List<ListView> whereAscOrderByDataObjects = await dataLayer.GetAllListViewAsync(obj => obj.Value > 30, obj => obj.Value);
        List<ListView> whereDescOrderByDataObjects = await dataLayer.GetAllListViewAsync(obj => obj.Value < 60, obj => obj.Value, true);

        Assert.Equal("40", whereAscOrderByDataObjects.First().Name);
        Assert.Equal("50", whereDescOrderByDataObjects.First().Name);
    }

    /// <summary>
    /// The method verifies UserEditableMemoryDataLayer.GetAllListViewAsync() returns the data objects as a list view based on a where predicate.
    /// </summary>
    /// <returns>A Task object for the async.</returns>
    [Fact]
    public async Task VerifyGetAllListViewWithWherePredicate()
    {
        SimpleUserEditableDataLayer dataLayer = new();

        await PopulateDataObjectsDivisibleBy10Async(dataLayer);

        List<ListView> lessThan40DataObjects = await dataLayer.GetAllListViewAsync(obj => obj.Value < 40);
        List<ListView> greaterThan80DataObjects = await dataLayer.GetAllListViewAsync(obj => obj.Value > 80);
        List<ListView> failedToFindDataObjects = await dataLayer.GetAllListViewAsync(obj => obj.Value == 1);

        Assert.Equal(3, lessThan40DataObjects.Count);
        Assert.Equal(2, greaterThan80DataObjects.Count);
        Assert.Empty(failedToFindDataObjects);
    }

    /// <summary>
    /// The method verifies UserEditableMemoryDataLayer.GetPageListViewAsync() returns a page of data objects as a list view.
    /// </summary>
    /// <returns>A Task object for the async.</returns>
    [Fact]
    public async Task VerifyGetPageListView()
    {
        SimpleUserEditableDataLayer dataLayer = new();
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

#warning This should be broken into multiple tests because its trying to test too many things.

        //Verify the skip and take.
        Assert.Equal(100, skipAndTakePage.TotalRecords);
        Assert.Equal(20, skipAndTakePage.DataObjects.Count);
        Assert.Equal("21", skipAndTakePage.DataObjects.First().Name);

        //Verify the filtering.
        Assert.Equal(1, filterPage.TotalRecords);
        Assert.Single(filterPage.DataObjects);
        Assert.Equal("50", filterPage.DataObjects.First().Name);

        //Verify the sorting.
        Assert.Equal(100, sortPage.TotalRecords);
        Assert.Equal(100, sortPage.DataObjects.Count);
        Assert.Equal("100", sortPage.DataObjects.First().Name);
    }

    /// <summary>
    /// The method verifies if a null data object is passed to the UserEditableMemoryDataLayer.GetPageListViewAsync(), an exception is thrown.
    /// </summary>
    [Fact]
    public async Task VerifyGetPageListViewThrowsArgumentNullException() => await Assert.ThrowsAsync<ArgumentNullException>(() => new SimpleUserEditableDataLayer().GetPageListViewAsync(null));

    /// <summary>
    /// The method verifies the UserEditableMemoryDataLayer.Updated event fires when UserEditableMemoryDataLayer.UpdateAsync() is called.
    /// </summary>
    [Fact]
    public async Task VerifyUpdateFiresUpdatedEvent()
    {
        SimpleUserEditableDataLayer dataLayer = new();
        await Assert.RaisesAsync<UpdatedEventArgs>
        (
            handler => dataLayer.Updated += handler,
            handler => dataLayer.Updated -= handler,
            async Task () =>
            {
                SimpleUserEditableDataObject dataObject = await dataLayer.CreateAsync(new SimpleUserEditableDataObject() { Name = DefaultName });
                dataObject.Value = DefaultValue;
                _ = await dataLayer.UpdateAsync(dataObject);
            }
        );
        await Assert.RaisesAsync<UpdatedEventArgs>
        (
            handler => dataLayer.Updated += handler,
            handler => dataLayer.Updated -= handler,
            async Task () =>
            {
                List<SimpleUserEditableDataObject> dataObjects = await dataLayer.CreateAsync([new SimpleUserEditableDataObject() { Name = AnotherName }, new SimpleUserEditableDataObject() { Name = DifferentName }]);

                dataObjects[0].Value = DefaultValue;
                dataObjects[1].Value = DefaultValue + 1;

                _ = await dataLayer.UpdateAsync(dataObjects);
            }
        );
    }

    /// <summary>
    /// The method verifies if a null data object is passed to the UserEditableMemoryDataLayer.UpdateAsync(), an exception is thrown.
    /// </summary>
    [Fact]
    public async Task VerifyUpdateThrowsArgumentNullException()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() => new SimpleUserEditableDataLayer().UpdateAsync((SimpleUserEditableDataObject?)null));
        await Assert.ThrowsAsync<ArgumentNullException>(() => new SimpleUserEditableDataLayer().UpdateAsync((List<SimpleUserEditableDataObject>)null));
    }

    /// <summary>
    /// The method verifies if an invalid data object is passed to the UserEditableMemoryDataLayer.UpdateAsync(), an exception is thrown.
    /// </summary>
    [Fact]
    public async Task VerifyUpdateThrowsDataObjectValidationException()
    {
        await Assert.ThrowsAsync<DataObjectValidationException>(async Task () =>
        {
            SimpleUserEditableDataLayer dataLayer = new();
            SimpleUserEditableDataObject dataObject = await dataLayer.CreateAsync(new SimpleUserEditableDataObject() { Name = DefaultName });

            dataObject.Name = null;
            _ = await dataLayer.UpdateAsync(dataObject);
        });
        await Assert.ThrowsAsync<DataObjectValidationException>(async Task () =>
        {
            SimpleUserEditableDataLayer dataLayer = new();
            List<SimpleUserEditableDataObject> dataObjects = await dataLayer.CreateAsync([new SimpleUserEditableDataObject() { Name = DefaultName }, new SimpleUserEditableDataObject() { Name = AnotherName }]);

            dataObjects[0].Value = DefaultValue;
            dataObjects[1].Value = InvalidValue;

            _ = await dataLayer.UpdateAsync(dataObjects);
        });
    }

    /// <summary>
    /// The method verifies if a non-existing ID is passed to the UserEditableMemoryDataLayer.UpdateAsync(), an exception is thrown.
    /// </summary>
    [Fact]
    public async Task VerifyUpdateThrowsIDNotFoundException()
    {
        await Assert.ThrowsAsync<IDNotFoundException>(() => new SimpleUserEditableDataLayer().UpdateAsync(new SimpleUserEditableDataObject() { Integer64ID = NotFoundId, Name = DefaultName }));
        await Assert.ThrowsAsync<IDNotFoundException>(() => new SimpleUserEditableDataLayer().UpdateAsync([new SimpleUserEditableDataObject() { Integer64ID = NotFoundId, Name = DefaultName }, new SimpleUserEditableDataObject() { Integer64ID = NotFoundId + 1, Name = AnotherName }]));
    }

    /// <summary>
    /// The method verifies if old data is being updated in UserEditableMemoryDataLayer.UpdateAsync(), an exception is thrown.
    /// </summary>
    [Fact]
    public async Task VerifyUpdateThrowsUpdateConflictException()
    {
        await Assert.ThrowsAsync<DataObjectUpdateConflictException>(async Task () =>
        {
            SimpleUserEditableDataLayer dataLayer = new();
            SimpleUserEditableDataObject originalDataObject = await dataLayer.CreateAsync(new SimpleUserEditableDataObject() { Name = DefaultName });

            _ = await dataLayer.UpdateAsync(new SimpleUserEditableDataObject(originalDataObject)
            {
                Value = DefaultValue,
            });

            originalDataObject.Value = DefaultValue + 1;
            _ = await dataLayer.UpdateAsync(originalDataObject);
        });
        await Assert.ThrowsAsync<DataObjectUpdateConflictException>(async Task () =>
        {
            SimpleUserEditableDataLayer dataLayer = new();
            List<SimpleUserEditableDataObject> originalDataObjects = await dataLayer.CreateAsync([new SimpleUserEditableDataObject() { Name = DefaultName }, new SimpleUserEditableDataObject() { Name = AnotherName }]);

            _ = await dataLayer.UpdateAsync([new SimpleUserEditableDataObject(originalDataObjects[0]) { Value = DefaultValue }, new SimpleUserEditableDataObject(originalDataObjects[1]) { Value = DefaultValue + 1 }]);

            originalDataObjects[0].Value = DefaultValue + 1;
            originalDataObjects[1].Value = DefaultValue + 2;

            _ = await dataLayer.UpdateAsync(originalDataObjects);
        });
    }

    /// <summary>
    /// The method verifies UserEditableMemoryDataLayer.UpdateAsync() updates a list of data objects in the data store.
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
    public async Task VerifyUpdateWithList()
    {
        SimpleUserEditableDataLayer dataLayer = new();
        List<SimpleUserEditableDataObject> originalDataObjects = await dataLayer.CreateAsync([new SimpleUserEditableDataObject() { Name = DefaultName }, new SimpleUserEditableDataObject() { Name = AnotherName }]);

        originalDataObjects[0].Value = DefaultValue;
        originalDataObjects[1].Value = DefaultValue + 1;

        List<SimpleUserEditableDataObject> returnedCopiedDataObjects = await dataLayer.UpdateAsync(originalDataObjects);
        List<SimpleUserEditableDataObject> confirmedDataObjects = await dataLayer.GetAllAsync();

        //An object must have been returned and the same amount updated must be returned.
        Assert.NotNull(returnedCopiedDataObjects);
        Assert.Equal(2, returnedCopiedDataObjects.Count);

        //Returned objects must be a copy.
        Assert.NotEqual(originalDataObjects[0], returnedCopiedDataObjects[0]);
        Assert.NotEqual(originalDataObjects[1], returnedCopiedDataObjects[1]);

        //LastEditedOn must have been set.
        Assert.NotNull(returnedCopiedDataObjects[0].LastEditedOn); //LastEditedOn must have been set.
        Assert.NotNull(returnedCopiedDataObjects[1].LastEditedOn); //LastEditedOn must have been set.

        //Confirm the data was updated.
        Assert.Equal(originalDataObjects[0].Value, confirmedDataObjects[0].Value);
        Assert.Equal(originalDataObjects[1].Value, confirmedDataObjects[1].Value);
    }

    /// <summary>
    /// The method verifies UserEditableMemoryDataLayer.UpdateAsync() updates a data object in the data store.
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
    public async Task VerifyUpdateWithSingle()
    {
        SimpleUserEditableDataLayer dataLayer = new();
        SimpleUserEditableDataObject originalDataObject = await dataLayer.CreateAsync(new SimpleUserEditableDataObject() { Name = DefaultName });

        originalDataObject.Value = DefaultValue;
        SimpleUserEditableDataObject returnedCopiedDataObject = await dataLayer.UpdateAsync(originalDataObject);
        SimpleUserEditableDataObject? confirmedDataObject = await dataLayer.GetSingleAsync();

        Assert.NotNull(returnedCopiedDataObject); //An object must have been returned.
        Assert.NotEqual(originalDataObject, returnedCopiedDataObject); //Returned object must be a copy.
        Assert.Equal(originalDataObject.Value, confirmedDataObject?.Value); //Confirm the data was updated.
        Assert.NotNull(returnedCopiedDataObject.LastEditedOn); //LastEditedOn must have been set.
    }

    /// <summary>
    /// The method verifies UserEditableMemoryDataLayer.ValidateAsync() validates the data object correctly.
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
    public async Task VerifyValidate()
    {
        SimpleUserEditableDataLayer dataLayer = new();
        SimpleUserEditableDataObject dataObject = await dataLayer.CreateAsync(new SimpleUserEditableDataObject() { Name = DefaultName });
        _ = await dataLayer.CreateAsync(new SimpleUserEditableDataObject() { Name = AnotherName });

        dataObject.Name = DifferentName;
        dataObject.Value = DefaultValue;
        List<ValidationResult> validationResults = await dataLayer.ValidateAsync(dataObject);
        bool dataAnnotationValid = validationResults.Count == 0;

        dataObject.Value = InvalidValue;
        validationResults = await dataLayer.ValidateAsync(dataObject);
        bool dataAnnotationNotValided = validationResults.Count != 0;

        dataObject.Value = 0;
        validationResults = await dataLayer.ValidateAsync(dataObject);
        bool nameValid = validationResults.Count == 0;

        dataObject.Integer64ID = 2;
        dataObject.Name = DefaultName;
        validationResults = await dataLayer.ValidateAsync(dataObject);
        bool nameNotValid = validationResults.Count != 0;

        Assert.True(dataAnnotationValid, "The data annotation valid should have been true.");
        Assert.True(dataAnnotationNotValided, "The data annotation not valid should have been true.");
        Assert.True(nameValid, "The name valid should have been true.");
        Assert.True(nameNotValid, "The name not valid should have been true.");
    }

    /// <summary>
    /// The method verifies if a null data object is passed to the UserEditableMemoryDataLayer.ValidateAsync(), an exception is thrown.
    /// </summary>
    [Fact]
    public async Task VerifyValidateThrowsArgumentNullException() => await Assert.ThrowsAsync<ArgumentNullException>(() => new SimpleUserEditableDataLayer().ValidateAsync(null));
}
