using JMayer.Data.Data;
using JMayer.Data.Data.Query;
using JMayer.Data.Database.DataLayer;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using TestProject.Data;
using TestProject.Database;

namespace TestProject.Test.Database;

/// <summary>
/// The class manages tests for the StandardCRUDDataLayer object.
/// </summary>
/// <remarks>
/// The tests are against a SimpleStandardDataLayer object which inherits from the StandardCRUDDataLayer and
/// the SimpleStandardDataLayer doesn't override any of the base methods. Because of this, we're testing 
/// the methods in the StandardCRUDDataLayer class.
/// </remarks>
public class StandardCRUDDataLayerUnitTest
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
    /// The constant for the default id.
    /// </summary>
    private const long DefaultId = 1;

    /// <summary>
    /// The constant for the default value.
    /// </summary>
    private const int DefaultValue = 1;

    /// <summary>
    /// The constant for the maximum paging records.
    /// </summary>
    private const int MaxPagingRecords = 100;

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
    private static async Task PopulateDataObjectsDivisibleBy10Async(SimpleStandardDataLayer dataLayer)
    {
        for (int index = 1; index <= 10; index++)
        {
            _ = await dataLayer.CreateAsync(new SimpleDataObject() { Name = (10 * index).ToString(), Value = 10 * index });
        }
    }

    /// <summary>
    /// The method verifies the StandardCRUDDataLayer.CountAsync() will return a count.
    /// </summary>
    /// <returns>A Task object for the async.</returns>
    [Fact]
    public async Task VerifyCount()
    {
        SimpleStandardDataLayer dataLayer = new();

        _ = await dataLayer.CreateAsync(new SimpleDataObject() { Name = DefaultName });
        long count = await dataLayer.CountAsync();

        Assert.Equal(1, count);
    }

    /// <summary>
    /// The method verifies if a null data object is passed to the StandardCRUDDataLayer.CountAsync(), an exception is thrown.
    /// </summary>
    [Fact]
    public async Task VerifyCountThrowsArgumentNullException() => await Assert.ThrowsAsync<ArgumentNullException>(() => new SimpleStandardDataLayer().CountAsync(null));

    /// <summary>
    /// The method verifies the StandardCRUDDataLayer.CountAsync() will return a count for a where predicate.
    /// </summary>
    /// <returns>A Task object for the async.</returns>
    [Fact]
    public async Task VerifyCountWithWherePredicate()
    {
        SimpleStandardDataLayer dataLayer = new();

        await PopulateDataObjectsDivisibleBy10Async(dataLayer);

        long oneCount = await dataLayer.CountAsync(obj => obj.Integer64ID == DefaultId);
        long zeroCount = await dataLayer.CountAsync(obj => obj.Integer64ID == NotFoundId);

        Assert.Equal(1, oneCount);
        Assert.Equal(0, zeroCount);
    }

    /// <summary>
    /// The method verifies the StandardCRUDDataLayer.Created event fires when StandardCRUDDataLayer.CreateAsync() is called.
    /// </summary>
    [Fact]
    public async Task VerifyCreateFiresCreatedEvent()
    {
        SimpleStandardDataLayer dataLayer = new();
        await Assert.RaisesAsync<CreatedEventArgs>
        (
            handler => dataLayer.Created += handler,
            handler => dataLayer.Created -= handler,
            () => dataLayer.CreateAsync(new SimpleDataObject() { Name = DefaultName })
        );
        await Assert.RaisesAsync<CreatedEventArgs>
        (
            handler => dataLayer.Created += handler,
            handler => dataLayer.Created -= handler,
            () => dataLayer.CreateAsync([new SimpleDataObject() { Name = AnotherName }, new SimpleDataObject() { Name = DifferentName }])
        );
    }

    /// <summary>
    /// The method verifies if a null data object is passed to the StandardCRUDDataLayer.CreateAsync(), an exception is thrown.
    /// </summary>
    [Fact]
    public async Task VerifyCreateThrowsArgumentNullException()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() => new SimpleStandardDataLayer().CreateAsync((SimpleDataObject)null));
        await Assert.ThrowsAsync<ArgumentNullException>(() => new SimpleStandardDataLayer().CreateAsync((List<SimpleDataObject>)null));
    }

    /// <summary>
    /// The method verifies if an invalid data object is passed to the StandardCRUDDataLayer.CreateAsync(), an exception is thrown.
    /// </summary>
    [Fact]
    public async Task VerifyCreateThrowsDataObjectValidationException()
    {
        await Assert.ThrowsAsync<DataObjectValidationException>(() => new SimpleStandardDataLayer().CreateAsync(new SimpleDataObject() { Name = DefaultName, Value = InvalidValue }));
        await Assert.ThrowsAsync<DataObjectValidationException>(() => new SimpleStandardDataLayer().CreateAsync([new SimpleDataObject() { Name = DefaultName, Value = DefaultValue }, new SimpleDataObject() { Name = AnotherName, Value = InvalidValue }]));
    }

    /// <summary>
    /// The method verifies if the data objects passed to the StandardCRUDDataLayer.CreateAsync() and they have duplicate names, an exception is thrown.
    /// </summary>
    [Fact]
    public async Task VerifyCreateThrowsParameterDuplicateNameException()
    {
        await Assert.ThrowsAsync<ParameterDuplicateNameException>(async () =>
        {
            SimpleStandardDataLayer dataLayer = new()
            {
                IsUniqueNameRequired = true,
            };

            await dataLayer.CreateAsync([new SimpleDataObject() { Name = DefaultName, Value = DefaultValue }, new SimpleDataObject() { Name = DefaultName, Value = DefaultValue }]);
        });
    }

    /// <summary>
    /// The method verifies StandardCRUDDataLayer.CreateAsync() creates a list of data objects.
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
    public async Task VerifyCreateWithList()
    {
        SimpleDataObject firstOriginalDataObject = new() { Name = DefaultName };
        SimpleDataObject secondOriginalDataObject = new() { Name = AnotherName };
        SimpleStandardDataLayer dataLayer = new();

        //Create the data objects.
        List<SimpleDataObject> returnedDataObjects = await dataLayer.CreateAsync([firstOriginalDataObject, secondOriginalDataObject]);
        long count = await dataLayer.CountAsync();

        //Objects must have been returned and the amount added must be the same amount returned.
        Assert.NotNull(returnedDataObjects); 
        Assert.Equal(2, returnedDataObjects.Count);

        //Returned objects must be a copy.
        Assert.NotEqual(firstOriginalDataObject, returnedDataObjects[0]); 
        Assert.NotEqual(secondOriginalDataObject, returnedDataObjects[1]);

        //ID must have been set by the datalayer.
        Assert.NotEqual(0, returnedDataObjects[0].Integer64ID); 
        Assert.NotEqual(0, returnedDataObjects[1].Integer64ID);

        //Created on must have been set by the datalayer.
        Assert.NotEqual(DateTime.MinValue, returnedDataObjects[0].CreatedOn);
        Assert.NotEqual(DateTime.MinValue, returnedDataObjects[1].CreatedOn);

        //Datalayer's internal identity must be incremented.
        Assert.NotEqual(returnedDataObjects[0].Integer64ID, returnedDataObjects[1].Integer64ID); 
        Assert.Equal(1, returnedDataObjects[1].Integer64ID - returnedDataObjects[0].Integer64ID);

        //Data objects were added to the data store.
        Assert.Equal(2, count);
    }

    /// <summary>
    /// The method verifies StandardCRUDDataLayer.CreateAsync() creates a single data object.
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
    public async Task VerifyCreateWithSingle()
    {
        SimpleDataObject originalDataObject = new() { Name = DefaultName };
        SimpleStandardDataLayer dataLayer = new();

        //Create the data objects.
        SimpleDataObject firstReturnedDataObject = await dataLayer.CreateAsync(originalDataObject);
        SimpleDataObject secondReturnedDataObject = await dataLayer.CreateAsync(originalDataObject);
        long count = await dataLayer.CountAsync();

        //An object must have been returned.
        Assert.NotNull(firstReturnedDataObject); 
        Assert.NotNull(secondReturnedDataObject);

        //Returned object must be a copy.
        Assert.NotEqual(originalDataObject, firstReturnedDataObject); 
        Assert.NotEqual(originalDataObject, secondReturnedDataObject);

        //ID must have been set by the datalayer.
        Assert.NotEqual(0, firstReturnedDataObject.Integer64ID); 
        Assert.NotEqual(0, secondReturnedDataObject.Integer64ID);

        //Created on must have been set by the datalayer.
        Assert.NotEqual(DateTime.MinValue, firstReturnedDataObject.CreatedOn);
        Assert.NotEqual(DateTime.MinValue, secondReturnedDataObject.CreatedOn);

        //Datalayer's internal identity must be incremented.
        Assert.NotEqual(firstReturnedDataObject.Integer64ID, secondReturnedDataObject.Integer64ID); 
        Assert.Equal(1, secondReturnedDataObject.Integer64ID - firstReturnedDataObject.Integer64ID);

        //Data objects were added to the data store.
        Assert.Equal(2, count);
    }

    /// <summary>
    /// The method verifies the StandardCRUDDataLayer.Deleted event fires when StandardCRUDDataLayer.DeleteAsync() is called.
    /// </summary>
    [Fact]
    public async Task VerifyDeleteFiresDeletedEvent()
    {
        SimpleStandardDataLayer dataLayer = new();
        await Assert.RaisesAsync<DeletedEventArgs>
        (
            handler => dataLayer.Deleted += handler,
            handler => dataLayer.Deleted -= handler,
            async Task () =>
            {
                SimpleDataObject dataObject = await dataLayer.CreateAsync(new SimpleDataObject() { Name = DefaultName });
                await dataLayer.DeleteAsync(dataObject);
            }
        );
        await Assert.RaisesAsync<DeletedEventArgs>
        (
            handler => dataLayer.Deleted += handler,
            handler => dataLayer.Deleted -= handler,
            async Task () =>
            {
                List<SimpleDataObject> dataObjects = await dataLayer.CreateAsync([new SimpleDataObject() { Name = DefaultName }, new SimpleDataObject() { Name = AnotherName }]);
                await dataLayer.DeleteAsync(dataObjects);
            }
        );
        await Assert.RaisesAsync<DeletedEventArgs>
        (
            handler => dataLayer.Deleted += handler,
            handler => dataLayer.Deleted -= handler,
            async Task () =>
            {
                _ = await dataLayer.CreateAsync([new SimpleDataObject() { Name = DefaultName }, new SimpleDataObject() { Name = AnotherName }, new SimpleDataObject() { Name = DifferentName }]);
                await dataLayer.DeleteAsync(obj => obj.Integer64ID > 1);
            }
        );
    }

    /// <summary>
    /// The method verifies if a null data object is passed to the StandardCRUDDataLayer.DeleteAsync(), an exception is thrown.
    /// </summary>
    [Fact]
    public async Task VerifyDeleteThrowsArgumentNullException()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() => new SimpleStandardDataLayer().DeleteAsync((SimpleDataObject)null));
        await Assert.ThrowsAsync<ArgumentNullException>(() => new SimpleStandardDataLayer().DeleteAsync((List<SimpleDataObject>)null));
        await Assert.ThrowsAsync<ArgumentNullException>(() => new SimpleStandardDataLayer().DeleteAsync((Expression<Func<SimpleDataObject, bool>>)null));
    }

    /// <summary>
    /// The method verifies StandardCRUDDataLayer.DeleteAsync() deletes a list of data objects.
    /// </summary>
    /// <returns>A Task object for the async.</returns>
    /// <remarks>
    /// The DeleteAsync() does the following:
    /// 
    /// 1. Finds the data objects using the ID.
    /// 2. Data objects are deleted from the data store.
    /// </remarks>
    [Fact]
    public async Task VerifyDeleteWithList()
    {
        SimpleStandardDataLayer dataLayer = new();
        List<SimpleDataObject> dataObjects = [];

        dataObjects.Add(await dataLayer.CreateAsync(new SimpleDataObject() { Name = DefaultName }));
        dataObjects.Add(await dataLayer.CreateAsync(new SimpleDataObject() { Name = AnotherName }));
        dataObjects.Add(await dataLayer.CreateAsync(new SimpleDataObject() { Name = DifferentName }));

        await dataLayer.DeleteAsync(dataObjects);
        long count = await dataLayer.CountAsync();

        Assert.Equal(0, count);
    }

    /// <summary>
    /// The method verifies StandardCRUDDataLayer.DeleteAsync() deletes a single data object.
    /// </summary>
    /// <returns>A Task object for the async.</returns>
    /// <remarks>
    /// The DeleteAsync() does the following:
    /// 
    /// 1. Finds the data object using the ID.
    /// 2. Data object is deleted from the data store.
    /// </remarks>
    [Fact]
    public async Task VerifyDeleteWithSingle()
    {
        SimpleStandardDataLayer dataLayer = new();

        SimpleDataObject dataObject = await dataLayer.CreateAsync(new SimpleDataObject() { Name = DefaultName });
        await dataLayer.DeleteAsync(dataObject);
        long count = await dataLayer.CountAsync();

        Assert.Equal(0, count);
    }

    /// <summary>
    /// The method verifies StandardCRUDDataLayer.DeleteAsync() deletes data objects with a where predicate.
    /// </summary>
    /// <returns>A Task object for the async.</returns>
    /// <remarks>
    /// The DeleteAsync() does the following:
    /// 
    /// 1. Finds the data objects using the where predicate.
    /// 2. Data objects are deleted from the data store.
    /// </remarks>
    [Fact]
    public async Task VerifyDeleteWithWherePredicate()
    {
        SimpleStandardDataLayer dataLayer = new();

        _ = await dataLayer.CreateAsync(new SimpleDataObject() { Name = DefaultName });
        _ = await dataLayer.CreateAsync(new SimpleDataObject() { Name = AnotherName });
        _ = await dataLayer.CreateAsync(new SimpleDataObject() { Name = DifferentName });

        await dataLayer.DeleteAsync(obj => obj.Integer64ID > 2);
        long count = await dataLayer.CountAsync();

        Assert.Equal(2, count);
    }

    /// <summary>
    /// The method verifies if a null data object is passed to the StandardCRUDDataLayer.ExistAsync(), an exception is thrown.
    /// </summary>
    [Fact]
    public async Task VerifyExistThrowsArgumentNullException() => await Assert.ThrowsAsync<ArgumentNullException>(() => new SimpleStandardDataLayer().ExistAsync(null));

    /// <summary>
    /// The method verifies StandardCRUDDataLayer.ExistAsync() can determine if a data object exists or not.
    /// </summary>
    /// <returns>A Task object for the async.</returns>
    [Fact]
    public async Task VerifyExistWithWherePredicate()
    {
        SimpleStandardDataLayer dataLayer = new();
        _ = await dataLayer.CreateAsync(new SimpleDataObject() { Name = DefaultName });

        bool found = await dataLayer.ExistAsync(obj => obj.Integer64ID == DefaultId);
        bool notFound = await dataLayer.ExistAsync(obj => obj.Integer64ID == NotFoundId) == false;

        Assert.True(found, "The found condition should have been true.");
        Assert.True(notFound, "The not found condition should been true.");
    }

    /// <summary>
    /// The method verifies StandardCRUDDataLayer.GetAllAsync() returns all data objects.
    /// </summary>
    /// <returns>A Task object for the async.</returns>
    [Fact]
    public async Task VerifyGetAll()
    {
        SimpleStandardDataLayer dataLayer = new();
        _ = await dataLayer.CreateAsync(new SimpleDataObject() { Name = DefaultName });
        List<SimpleDataObject> dataObjects = await dataLayer.GetAllAsync();
        Assert.Single(dataObjects);
    }

    /// <summary>
    /// The method verifies StandardCRUDDataLayer.GetAllAsync() returns all the data object but in a specific order.
    /// </summary>
    /// <returns>A Task object for the async.</returns>
    [Fact]
    public async Task VerifyGetAllWithOrderPredicate()
    {
        SimpleStandardDataLayer dataLayer = new();

        await PopulateDataObjectsDivisibleBy10Async(dataLayer);

        List<SimpleDataObject> ascOrderByDataObjects = await dataLayer.GetAllAsync(orderByPredicate: obj => obj.Value);
        List<SimpleDataObject> descOrderByDataObjects = await dataLayer.GetAllAsync(orderByPredicate: obj => obj.Value, descending: true);

        Assert.Equal(10, ascOrderByDataObjects.First().Value);
        Assert.Equal(100, descOrderByDataObjects.First().Value);
    }

    /// <summary>
    /// The method verifies StandardCRUDDataLayer.GetAllAsync() returns the data objects based on a where predicate and in a specific order.
    /// </summary>
    /// <returns>A Task object for the async.</returns>
    [Fact]
    public async Task VerifyGetAllWithWhereOrderPredicate()
    {
        SimpleStandardDataLayer dataLayer = new();

        await PopulateDataObjectsDivisibleBy10Async(dataLayer);

        List<SimpleDataObject> whereAscOrderByDataObjects = await dataLayer.GetAllAsync(obj => obj.Value > 30, obj => obj.Value);
        List<SimpleDataObject> whereDescOrderByDataObjects = await dataLayer.GetAllAsync(obj => obj.Value < 60, obj => obj.Value, true);

        Assert.Equal(40, whereAscOrderByDataObjects.First().Value);
        Assert.Equal(50, whereDescOrderByDataObjects.First().Value);
    }

    /// <summary>
    /// The method verifies StandardCRUDDataLayer.GetAllAsync() returns the data objects based on a where predicate.
    /// </summary>
    /// <returns>A Task object for the async.</returns>
    [Fact]
    public async Task VerifyGetAllWithWherePredicate()
    {
        SimpleStandardDataLayer dataLayer = new();

        await PopulateDataObjectsDivisibleBy10Async(dataLayer);

        List<SimpleDataObject> lessThan40DataObjects = await dataLayer.GetAllAsync(obj => obj.Value < 40);
        List<SimpleDataObject> greaterThan80DataObjects = await dataLayer.GetAllAsync(obj => obj.Value > 80);
        List<SimpleDataObject> failedToFindDataObjects = await dataLayer.GetAllAsync(obj => obj.Value == 1);

        Assert.Equal(3, lessThan40DataObjects.Count);
        Assert.Equal(2, greaterThan80DataObjects.Count);
        Assert.Empty(failedToFindDataObjects);
    }

    /// <summary>
    /// The method verifies StandardCRUDDataLayer.GetAllListViewAsync() returns all data objects as a list view.
    /// </summary>
    /// <returns>A Task object for the async.</returns>
    [Fact]
    public async Task VerifyGetAllListView()
    {
        SimpleStandardDataLayer dataLayer = new();
        _ = await dataLayer.CreateAsync(new SimpleDataObject() { Name = DefaultName });
        List<ListView> listViews = await dataLayer.GetAllListViewAsync();
        Assert.Single(listViews);
    }

    /// <summary>
    /// The method verifies StandardCRUDDataLayer.GetAllListViewAsync() returns all data objects as a list view but with a specific order.
    /// </summary>
    /// <returns>A Task object for the async.</returns>
    [Fact]
    public async Task VerifyGetAllListViewWithOrderPredicate()
    {
        SimpleStandardDataLayer dataLayer = new();

        await PopulateDataObjectsDivisibleBy10Async(dataLayer);

        List<ListView> ascOrderByDataObjects = await dataLayer.GetAllListViewAsync(orderByPredicate: obj => obj.Value);
        List<ListView> descOrderByDataObjects = await dataLayer.GetAllListViewAsync(orderByPredicate: obj => obj.Value, descending: true);

        Assert.Equal("10", ascOrderByDataObjects.First().Name);
        Assert.Equal("100", descOrderByDataObjects.First().Name);
    }

    /// <summary>
    /// The method verifies StandardCRUDDataLayer.GetAllListViewAsync() returns the data objects as a list view based on a where predicate and with a specific order.
    /// </summary>
    /// <returns>A Task object for the async.</returns>
    [Fact]
    public async Task VerifyGetAllListViewWithWhereOrderPredicate()
    {
        SimpleStandardDataLayer dataLayer = new();

        await PopulateDataObjectsDivisibleBy10Async(dataLayer);

        List<ListView> whereAscOrderByDataObjects = await dataLayer.GetAllListViewAsync(obj => obj.Value > 30, obj => obj.Value);
        List<ListView> whereDescOrderByDataObjects = await dataLayer.GetAllListViewAsync(obj => obj.Value < 60, obj => obj.Value, true);

        Assert.Equal("40", whereAscOrderByDataObjects.First().Name);
        Assert.Equal("50", whereDescOrderByDataObjects.First().Name);
    }

    /// <summary>
    /// The method verifies StandardCRUDDataLayer.GetAllListViewAsync() returns the data objects as a list view based on a where predicate.
    /// </summary>
    /// <returns>A Task object for the async.</returns>
    [Fact]
    public async Task VerifyGetAllListViewWithWherePredicate()
    {
        SimpleStandardDataLayer dataLayer = new();

        await PopulateDataObjectsDivisibleBy10Async(dataLayer);

        List<ListView> lessThan40DataObjects = await dataLayer.GetAllListViewAsync(obj => obj.Value < 40);
        List<ListView> greaterThan80DataObjects = await dataLayer.GetAllListViewAsync(obj => obj.Value > 80);
        List<ListView> failedToFindDataObjects = await dataLayer.GetAllListViewAsync(obj => obj.Value == 1);

        Assert.Equal(3, lessThan40DataObjects.Count);
        Assert.Equal(2, greaterThan80DataObjects.Count);
        Assert.Empty(failedToFindDataObjects);
    }

    /// <summary>
    /// The method verifies StandardCRUDDataLayer.GetPageAsync() returns a page of data objects.
    /// </summary>
    /// <returns>A Task object for the async.</returns>
    [Fact]
    public async Task VerifyGetPage()
    {
        SimpleStandardDataLayer dataLayer = new();
        List<SimpleDataObject> dataObjects = [];

        for (int index = 1; index <= MaxPagingRecords; index++)
        {
            dataObjects.Add(new SimpleDataObject()
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

#warning This should be broken into multiple tests because its trying to test too many things.

        //Verify the skip and take.
        Assert.Equal(100, skipAndTakePage.TotalRecords);
        Assert.Equal(20, skipAndTakePage.DataObjects.Count);
        Assert.Equal(21, skipAndTakePage.DataObjects.First().Value);

        //Verify the filtering.
        Assert.Equal(1, filterPage.TotalRecords);
        Assert.Single(filterPage.DataObjects);
        Assert.Equal(50, filterPage.DataObjects.First().Value);

        //Verify the sorting.
        Assert.Equal(100, sortPage.TotalRecords);
        Assert.Equal(100, sortPage.DataObjects.Count);
        Assert.Equal(100, sortPage.DataObjects.First().Value);
    }

    /// <summary>
    /// The method verifies StandardCRUDDataLayer.GetPageListViewAsync() returns a page of data objects as a list view.
    /// </summary>
    /// <returns>A Task object for the async.</returns>
    [Fact]
    public async Task VerifyGetPageListView()
    {
        SimpleStandardDataLayer dataLayer = new();
        List<SimpleDataObject> dataObjects = [];

        for (int index = 1; index <= 100; index++)
        {
            dataObjects.Add(new SimpleDataObject()
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
            FilterOn = nameof(SimpleDataObject.Value),
            Operator = FilterDefinition.EqualsOperator,
            Value = "50",
        });
        PagedList<ListView> filterPage = await dataLayer.GetPageListViewAsync(filterQueryDefinition);

        QueryDefinition sortQueryDefinition = new();
        sortQueryDefinition.SortDefinitions.Add(new SortDefinition()
        {
            Descending = true,
            SortOn = nameof(SimpleDataObject.Value),
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
    /// The method verifies if a null data object is passed to the StandardCRUDDataLayer.GetPageListViewAsync(), an exception is thrown.
    /// </summary>
    [Fact]
    public async Task VerifyGetPageListViewThrowsArgumentNullException() => await Assert.ThrowsAsync<ArgumentNullException>(() => new SimpleStandardDataLayer().GetPageListViewAsync(null));

    /// <summary>
    /// The method verifies if a null data object is passed to the StandardCRUDDataLayer.GetPageAsync(), an exception is thrown.
    /// </summary>
    [Fact]
    public async Task VerifyGetPageThrowsArgumentNullException() => await Assert.ThrowsAsync<ArgumentNullException>(() => new SimpleStandardDataLayer().GetPageAsync(null));

    /// <summary>
    /// The method verifies StandardCRUDDataLayer.GetSingleAsync() returns the first data object.
    /// </summary>
    /// <returns>A Task object for the async.</returns>
    [Fact]
    public async Task VerifyGetSingle()
    {
        SimpleStandardDataLayer dataLayer = new();

        bool notFound = await dataLayer.GetSingleAsync() == null;
        _ = await dataLayer.CreateAsync(new SimpleDataObject() { Name = DefaultName });
        bool found = await dataLayer.GetSingleAsync() != null;

        Assert.True(found, "The found should have been true.");
        Assert.True(notFound, "The not found should have been true.");
    }

    /// <summary>
    /// The method verifies StandardCRUDDataLayer.GetSingleAsync() returns a single data object based on a where predicate.
    /// </summary>
    /// <returns>A Task object for the async.</returns>
    [Fact]
    public async Task VerifyGetSingleWithWherePredicate()
    {
        SimpleStandardDataLayer dataLayer = new();

        await PopulateDataObjectsDivisibleBy10Async(dataLayer);

        bool found = await dataLayer.GetSingleAsync(obj => obj.Value == 70) != null;
        bool notFound = await dataLayer.GetSingleAsync(obj => obj.Value == 71) == null;

        Assert.True(found, "The found condition should have been true.");
        Assert.True(notFound, "The not found condition should been true.");
    }

    /// <summary>
    /// The method verifies when IsOldDataObjectDetectionEnabled is false no exception is thrown.
    /// </summary>
    /// <returns>A Task for the async.</returns>
    /// <remarks>
    /// This is relying on when IsOldDataObjectDetectionEnabled is true and there's a conflict, an exception
    /// is thrown. Xunit fails the test when an unhandle exception occurs so this passes when no exception is thrown.
    /// </remarks>
    [Fact]
    public async Task VerifyUpdateDisableOldDataObjectDetectionThrowsNoException()
    {
        SimpleStandardDataLayer dataLayer = new()
        {
            IsOldDataObjectDetectionEnabled = false,
        };
        SimpleDataObject originalDataObject = await dataLayer.CreateAsync(new SimpleDataObject() { Name = DefaultName });

        _ = await dataLayer.UpdateAsync(new SimpleDataObject(originalDataObject)
        {
            Value = DefaultValue,
        });

        originalDataObject.Value = DefaultValue + 1;
        _ = await dataLayer.UpdateAsync(originalDataObject);
    }

    /// <summary>
    /// The method verifies when IsLessPreciseTimestampComparisonEnabled is true and LastEditedOn only has a time difference in the milliseconds or below no exception is thrown.
    /// </summary>
    /// <returns>A Task for the async.</returns>
    /// <remarks>
    /// This is relying on when a conflict is detected, an exception is thrown. Xunit fails the test 
    /// when an unhandle exception occurs so this passes when no exception is thrown.
    /// </remarks>
    [Fact]
    public async Task VerifyUpdateEnableLessPreciseTimestampComparisonThrowsNoException()
    {
#warning For the VerifyUpdateEnableLessPreciseTimestampComparisonThrowsNoException() test, I set LastEditedOn property to DateTime.Now without milliseconds. Create doesn't clear the LastEditedOn property but maybe it should because the object has just been added to the memory storage and the LastEditedOn property should be null.

        SimpleStandardDataLayer dataLayer = new()
        {
            IsLessPreciseTimestampComparisonEnabled = true,
        };
        SimpleDataObject originalDataObject = await dataLayer.CreateAsync(new SimpleDataObject() { Name = DefaultName, LastEditedOn = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second) });

        originalDataObject.Value = DefaultValue + 1;

        if (originalDataObject.LastEditedOn is not null)
        {
            originalDataObject.LastEditedOn = originalDataObject.LastEditedOn.Value.AddMilliseconds(1);
        }

        _ = await dataLayer.UpdateAsync(originalDataObject);
    }

    /// <summary>
    /// The method verifies the StandardCRUDDataLayer.Updated event fires when StandardCRUDDataLayer.UpdateAsync() is called.
    /// </summary>
    [Fact]
    public async Task VerifyUpdateFiresUpdatedEvent()
    {
        SimpleStandardDataLayer dataLayer = new();
        await Assert.RaisesAsync<UpdatedEventArgs>
        (
            handler => dataLayer.Updated += handler,
            handler => dataLayer.Updated -= handler,
            async Task () =>
            {
                SimpleDataObject dataObject = await dataLayer.CreateAsync(new SimpleDataObject() { Name = DefaultName });
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
                List<SimpleDataObject> dataObjects = await dataLayer.CreateAsync([new SimpleDataObject() { Name = AnotherName }, new SimpleDataObject() { Name = DifferentName }]);

                dataObjects[0].Value = 10;
                dataObjects[1].Value = 20;

                _ = await dataLayer.UpdateAsync(dataObjects);
            }
        );
    }

    /// <summary>
    /// The method verifies if a null data object is passed to the StandardCRUDDataLayer.UpdateAsync(), an exception is thrown.
    /// </summary>
    [Fact]
    public async Task VerifyUpdateThrowsArgumentNullException()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() => new SimpleStandardDataLayer().UpdateAsync((SimpleDataObject)null));
        await Assert.ThrowsAsync<ArgumentNullException>(() => new SimpleStandardDataLayer().UpdateAsync((List<SimpleDataObject>)null));
    }

    /// <summary>
    /// The method verifies if an invalid data object is passed to the StandardCRUDDataLayer.UpdateAsync(), an exception is thrown.
    /// </summary>
    [Fact]
    public async Task VerifyUpdateThrowsDataObjectValidationException()
    {
        await Assert.ThrowsAsync<DataObjectValidationException>(async Task () =>
        {
            SimpleStandardDataLayer dataLayer = new();
            SimpleDataObject dataObject = await dataLayer.CreateAsync(new SimpleDataObject() { Name = DefaultName });

            dataObject.Value = InvalidValue;

            _ = await dataLayer.UpdateAsync(dataObject);
        });
        await Assert.ThrowsAsync<DataObjectValidationException>(async Task () =>
        {
            SimpleStandardDataLayer dataLayer = new();
            List<SimpleDataObject> dataObjects = await dataLayer.CreateAsync([new SimpleDataObject() { Name = DefaultName }, new SimpleDataObject() { Name = AnotherName }]);

            dataObjects[0].Value = DefaultValue;
            dataObjects[1].Value = InvalidValue;

            _ = await dataLayer.UpdateAsync(dataObjects);
        });
        await Assert.ThrowsAsync<DataObjectValidationException>(async Task () =>
        {
            SimpleStandardDataLayer dataLayer = new()
            {
                IsUniqueNameRequired = true,
            };
            List<SimpleDataObject> dataObjects = await dataLayer.CreateAsync([new SimpleDataObject() { Name = DefaultName }, new SimpleDataObject() { Name = AnotherName }]);

            dataObjects[0].Name = AnotherName;
            _ = await dataLayer.UpdateAsync(dataObjects[0]);
        });
    }

    /// <summary>
    /// The method verifies if a non-existing ID is passed to the StandardCRUDDataLayer.UpdateAsync(), an exception is thrown.
    /// </summary>
    [Fact]
    public async Task VerifyUpdateAsyncIDNotFoundException()
    {
        await Assert.ThrowsAsync<IDNotFoundException>(() => new SimpleStandardDataLayer().UpdateAsync(new SimpleDataObject() { Integer64ID = NotFoundId, Name = DefaultName }));
        await Assert.ThrowsAsync<IDNotFoundException>(() => new SimpleStandardDataLayer().UpdateAsync([new SimpleDataObject() { Integer64ID = NotFoundId, Name = DefaultName }]));
    }

    /// <summary>
    /// The method verifies if the data objects passed to the StandardCRUDDataLayer.UpdateAsync() and they have duplicate names, an exception is thrown.
    /// </summary>
    [Fact]
    public async Task VerifyUpdateThrowsParameterDuplicateNameException()
    {
        await Assert.ThrowsAsync<ParameterDuplicateNameException>(async () =>
        {
            SimpleStandardDataLayer dataLayer = new()
            {
                IsUniqueNameRequired = true,
            };

            await dataLayer.CreateAsync([new SimpleDataObject() { Name = DefaultName, Value = DefaultValue }, new SimpleDataObject() { Name = DefaultName, Value = DefaultValue }]);
        });
    }

    /// <summary>
    /// The method verifies if old data is being updated in StandardCRUDDataLayer.UpdateAsync(), an exception is thrown.
    /// </summary>
    [Fact]
    public async Task VerifyUpdateThrowsUpdateConflictException()
    {
        await Assert.ThrowsAsync<DataObjectUpdateConflictException>(async Task () =>
        {
            SimpleStandardDataLayer dataLayer = new();
            SimpleDataObject originalDataObject = await dataLayer.CreateAsync(new SimpleDataObject() { Name = DefaultName });

            _ = await dataLayer.UpdateAsync(new SimpleDataObject(originalDataObject)
            {
                Value = DefaultValue,
            });

            originalDataObject.Value = DefaultValue + 1;
            _ = await dataLayer.UpdateAsync(originalDataObject);
        });
        await Assert.ThrowsAsync<DataObjectUpdateConflictException>(async Task () =>
        {
            SimpleStandardDataLayer dataLayer = new();
            List<SimpleDataObject> originalDataObjects = await dataLayer.CreateAsync([new SimpleDataObject() { Name = DefaultName }, new SimpleDataObject() { Name = AnotherName }]);

            _ = await dataLayer.UpdateAsync([new SimpleDataObject(originalDataObjects[0]) { Value = DefaultValue }, new SimpleDataObject(originalDataObjects[1]) { Value = DefaultValue + 1 }]);

            originalDataObjects[0].Value = DefaultValue + 1;
            originalDataObjects[1].Value = DefaultValue + 2;

            _ = await dataLayer.UpdateAsync(originalDataObjects);
        });
    }

    /// <summary>
    /// The method verifies StandardCRUDDataLayer.UpdateAsync() updates a list of data objects in the data store.
    /// </summary>
    /// <returns>A Task object for the async.</returns>
    /// <remarks>
    /// The UpdateAsync() does the following:
    /// 
    /// 1. Finds the data object using the ID.
    /// 2. Preps the data object (does nothing).
    /// 3. Preps the data object (sets last edited on).
    /// 4. Data object is update in the data store.
    /// 5. A copy of the data object is returned.
    /// </remarks>
    [Fact]
    public async Task VerifyUpdateWithList()
    {
        SimpleStandardDataLayer dataLayer = new();
        List<SimpleDataObject> originalDataObjects = await dataLayer.CreateAsync([new SimpleDataObject() { Name = DefaultName }, new SimpleDataObject() { Name = AnotherName }]);

        originalDataObjects[0].Value = 10;
        originalDataObjects[1].Value = 20;

        List<SimpleDataObject> returnedCopiedDataObjects = await dataLayer.UpdateAsync(originalDataObjects);
        List<SimpleDataObject> confirmedDataObjects = await dataLayer.GetAllAsync();

        //An object must have been returned and the same amount updated must be returned.
        Assert.NotNull(returnedCopiedDataObjects);
        Assert.Equal(2, returnedCopiedDataObjects.Count);

        //Returned objects must be a copy.
        Assert.NotEqual(originalDataObjects[0], returnedCopiedDataObjects[0]);
        Assert.NotEqual(originalDataObjects[1], returnedCopiedDataObjects[1]);

        //LastEditedOn must have been set.
        Assert.NotNull(returnedCopiedDataObjects[0].LastEditedOn);
        Assert.NotNull(returnedCopiedDataObjects[1].LastEditedOn);

        //Confirm the data was updated.
        Assert.Equal(originalDataObjects[0].Value, confirmedDataObjects[0].Value);
        Assert.Equal(originalDataObjects[1].Value, confirmedDataObjects[1].Value);
    }

    /// <summary>
    /// The method verifies StandardCRUDDataLayer.UpdateAsync() updates a data object in the data store.
    /// </summary>
    /// <returns>A Task object for the async.</returns>
    /// <remarks>
    /// The UpdateAsync() does the following:
    /// 
    /// 1. Finds the data object using the ID.
    /// 2. Preps the data object (does nothing).
    /// 3. Preps the data object (sets last edited on).
    /// 4. Data object is update in the data store.
    /// 5. A copy of the data object is returned.
    /// </remarks>
    [Fact]
    public async Task VerifyUpdateWithSingle()
    {
        SimpleStandardDataLayer dataLayer = new();
        SimpleDataObject originalDataObject = await dataLayer.CreateAsync(new SimpleDataObject() { Name = DefaultName });

        originalDataObject.Value = DefaultValue;
        SimpleDataObject returnedCopiedDataObject = await dataLayer.UpdateAsync(originalDataObject);
        SimpleDataObject? confirmedDataObject = await dataLayer.GetSingleAsync();

        Assert.NotNull(returnedCopiedDataObject); //An object must have been returned.
        Assert.NotEqual(originalDataObject, returnedCopiedDataObject); //Returned object must be a copy.
        Assert.Equal(originalDataObject.Value, confirmedDataObject?.Value); //Confirm the data was updated.
        Assert.NotNull(returnedCopiedDataObject.LastEditedOn); //LastEditedOn must have been set.
    }

    /// <summary>
    /// The method verifies StandardCRUDDataLayer.ValidateAsync() validates the data object correctly.
    /// </summary>
    /// <returns>A Task object for the async.</returns>
    /// <remarks>
    /// The ValidateAsync() does the following:
    /// 
    /// 1. Validate the data annotations on the object. (SimpleDataObject.Value has a Range data annotation.)
    /// 2. The results is returned.
    /// </remarks>
    [Fact]
    public async Task VerifyValidate()
    {
        SimpleStandardDataLayer dataLayer = new();
        SimpleDataObject dataObject = await dataLayer.CreateAsync(new SimpleDataObject() { Name = DefaultName });

        dataObject.Value = DefaultValue;
        List<ValidationResult> validationResults = await dataLayer.ValidateAsync(dataObject);
        bool dataAnnotationValid = validationResults.Count == 0;

        dataObject.Value = InvalidValue;
        validationResults = await dataLayer.ValidateAsync(dataObject);
        bool dataAnnotationNotValided = validationResults.Count != 0;

        Assert.True(dataAnnotationValid, "The data annotation valid should have been true.");
        Assert.True(dataAnnotationNotValided, "The data annotation not valid should have been true.");
    }

    /// <summary>
    /// The method verifies StandardCRUDDataLayer.ValidateAsync() validates the data object correctly when the IsUniqueNameRequired property set to true.
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
    public async Task VerifyValidateUniqueName()
    {
        SimpleStandardDataLayer dataLayer = new()
        {
            IsUniqueNameRequired = true,
        };
        SimpleDataObject dataObject = await dataLayer.CreateAsync(new SimpleDataObject() { Name = DefaultName });
        _ = await dataLayer.CreateAsync(new SimpleDataObject() { Name = AnotherName });

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
    /// The method verifies if a null data object is passed to the StandardCRUDDataLayer.ValidateAsync(), an exception is thrown.
    /// </summary>
    [Fact]
    public async Task VerifyValidateThrowsArgumentNullException() => await Assert.ThrowsAsync<ArgumentNullException>(() => new SimpleStandardDataLayer().ValidateAsync(null));
}
