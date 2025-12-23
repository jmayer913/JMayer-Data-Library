using JMayer.Data.Data;
using JMayer.Data.Data.Query;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;

namespace JMayer.Data.Database.DataLayer.MemoryStorage;

/// <summary>
/// The class manages CRUD interactions with a list memory storage for data objects.
/// </summary>
/// <typeparam name="T">A DataObject which represents data in the collection/table.</typeparam>
/// <remarks>
/// <para>
/// This uses an 64-integer identity (auto-increments) ID so the DataObject.Integer64ID will be
/// used by this and any outside interactions with the data layer must use DataObject.Integer64ID. 
/// Also, the underlying data storage is a List so this shouldn't be used with very large datasets.
/// </para>
/// <para>
/// The create and update operations are dependent on the DataObject.MapProperties() method; a copy 
/// is created so the data object passed in and returned are a separate entity from the data object stored
/// in memory. What this means is in your child class, you must override the DataObject.MapProperties() 
/// and add mappings for the properties in your data object.
/// </para>
/// </remarks>
public class StandardCRUDDataLayer<T> : IStandardCRUDDataLayer<T> where T : DataObject, new()
{
    /// <summary>
    /// The memory data storage for this database.
    /// </summary>
    private readonly List<T> _dataStorage = [];

    /// <summary>
    /// The property gets the data storage for this database.
    /// </summary>
    protected List<T> DataStorage => _dataStorage;

    /// <summary>
    /// The lock used when accessing the data storage.
    /// </summary>
    private readonly Lock _dataStorageLock = new();

    /// <summary>
    /// The property gets the lock used when accessing the data storage.
    /// </summary>
    protected Lock DataStorageLock => _dataStorageLock; 

    /// <summary>
    /// The identity of the last created record.
    /// </summary>
    private long _identity = 1;

    /// <summary>
    /// The property gets the identity of the last created record.
    /// </summary>
    protected long Identity => _identity;

    /// <inheritdoc/>
    public event EventHandler<CreatedEventArgs>? Created;

    /// <inheritdoc/>
    public event EventHandler<DeletedEventArgs>? Deleted;

    /// <inheritdoc/>
    public bool IsOldDataObjectDetectionEnabled { get; init; }

    /// <inheritdoc/>
    public bool IsUniqueNameRequired { get; init; }

    /// <inheritdoc/>
    public event EventHandler<UpdatedEventArgs>? Updated;

    /// <summary>
    /// The method returns if there's no update conflict.
    /// </summary>
    /// <param name="databaseDataObject">The data object in the database.</param>
    /// <param name="userDataObject">The data object updated by the user.</param>
    /// <returns>True means the data object can be updated; false means there's a conflict.</returns>
    private bool AllowToUpdate(T databaseDataObject, T userDataObject)
    {
        //If both are not null, recreate the LastEditedOn but without the millseconds, microseconds & nanoseconds and then, compare to two.
        if (databaseDataObject.LastEditedOn is not null && userDataObject.LastEditedOn is not null)
        {
            DateTime databaseDataObjectLastEditedOn = new(databaseDataObject.LastEditedOn.Value.Year, databaseDataObject.LastEditedOn.Value.Month, databaseDataObject.LastEditedOn.Value.Day, databaseDataObject.LastEditedOn.Value.Hour, databaseDataObject.LastEditedOn.Value.Minute, databaseDataObject.LastEditedOn.Value.Second);
            DateTime userDataObjectLastEditedOn = new(userDataObject.LastEditedOn.Value.Year, userDataObject.LastEditedOn.Value.Month, userDataObject.LastEditedOn.Value.Day, userDataObject.LastEditedOn.Value.Hour, userDataObject.LastEditedOn.Value.Minute, userDataObject.LastEditedOn.Value.Second);

            return databaseDataObjectLastEditedOn == userDataObjectLastEditedOn;
        }
        //If both null then no edits have occurred so allow the update.
        else if (databaseDataObject.LastEditedOn is null && userDataObject.LastEditedOn is null)
        {
            return true;
        }
        //If one is null and the other not null then edits have occurred so do not allow an update.
        else
        {
            return false;
        }
    }

    /// <summary>
    /// The method converts the user editable data objects to list view objects.
    /// </summary>
    /// <param name="dataObjects">The data objects to convert.</param>
    /// <returns>A list of ListView objects.</returns>
    protected virtual List<ListView> ConvertToListView(List<T> dataObjects) => dataObjects.ConvertAll(obj => new ListView(obj));

    /// <inheritdoc/>
    public async virtual Task<long> CountAsync(CancellationToken cancellationToken = default)
    {
        long count;

        lock (DataStorageLock)
        {
            count = DataStorage.Count;
        }

        return await Task.FromResult(count);
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException">Thrown if the wherePredicate parameter is null.</exception>
    public async virtual Task<long> CountAsync(Expression<Func<T, bool>> wherePredicate, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(wherePredicate);
        List<T> dataObjects = QueryData(wherePredicate);
        return await Task.FromResult(dataObjects.Count);
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException">Thrown if the dataObject parameter is null.</exception>
    /// <exception cref="DataObjectValidationException">Thrown if the data object fails validation.</exception>
    public async virtual Task<T> CreateAsync(T dataObject, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(dataObject);
        return (await CreateAsync([dataObject], cancellationToken)).First();
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException">Thrown if the dataObjects parameter is null.</exception>
    /// <exception cref="DataObjectValidationException">Thrown if the data object fails validation.</exception>
    public async virtual Task<List<T>> CreateAsync(List<T> dataObjects, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(dataObjects);

        //Confirm validation because this wants all or none created.
        await ValidateParamaterDataObjectsAsync(dataObjects, cancellationToken);

        List<T> returnDataObjects = [];

        lock (DataStorageLock)
        {
            foreach (T dataObject in dataObjects)
            {
                //Create a copy so the object passed in is independent of what is stored.
                T storageDataObject = CreateCopy(dataObject);
                
                //Prep and insert the data object into storage.
                PrepForCreate(storageDataObject);
                DataStorage.Add(storageDataObject);
                IncrementIdentity();

                //Create a copy so its independent of the data storage.
                T returnDataObject = CreateCopy(storageDataObject);
                returnDataObjects.Add(returnDataObject);
            }
        }

        OnCreated(new CreatedEventArgs([.. returnDataObjects]));

        return await Task.FromResult(returnDataObjects);
    }

    /// <summary>
    /// The method returns a copy of the data object.
    /// </summary>
    /// <param name="dataObject">The data object to copy.</param>
    /// <returns>A copied data object.</returns>
    /// <remarks>
    /// Because this is memory storage and classes are reference types, a copy
    /// must be created so the memory storage is independent of any data manipulation
    /// done by the user. Basically, it forces the user to call UpdateAsync() 
    /// to update data in the memory storage.
    /// </remarks>
    protected static T CreateCopy(T dataObject)
    {
        T copyDataObject = new();
        copyDataObject.MapProperties(dataObject);
        return copyDataObject;
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException">Thrown if the dataObject parameter is null.</exception>
    public async virtual Task DeleteAsync(T dataObject, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(dataObject);

        T? databaseDataObject = null;

        lock (DataStorageLock)
        {
            databaseDataObject = DataStorage.FirstOrDefault(obj => obj.Integer64ID == dataObject.Integer64ID);

            if (databaseDataObject is not null)
            {
                _ = DataStorage.Remove(databaseDataObject);
            }
        }

        if (databaseDataObject is not null)
        {
            OnDeleted(new DeletedEventArgs([databaseDataObject]));
        }

        await Task.CompletedTask;
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException">Thrown if the dataObjects parameter is null.</exception>
    public async virtual Task DeleteAsync(List<T> dataObjects, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(dataObjects);

        List<long> ids = [.. dataObjects.Select(obj => obj.Integer64ID)];
        List<T> databaseDataObjects = await GetAllAsync(obj => ids.Any(id => id == obj.Integer64ID), cancellationToken: cancellationToken);

        lock (DataStorageLock)
        {
            _ = DataStorage.RemoveAll(obj => ids.Any(id => id == obj.Integer64ID));
        }

        OnDeleted(new DeletedEventArgs([.. databaseDataObjects]));
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException">Thrown if the wherePredicate parameter is null.</exception>
    public async virtual Task DeleteAsync(Expression<Func<T, bool>> wherePredicate, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(wherePredicate);

        List<T> dataObjects = await GetAllAsync(wherePredicate, cancellationToken: cancellationToken);
        List<long> ids = [.. dataObjects.Select(obj => obj.Integer64ID)];

        lock (DataStorageLock)
        {
            _ = DataStorage.RemoveAll(obj => ids.Any(id => id == obj.Integer64ID));
        }

        OnDeleted(new DeletedEventArgs([.. dataObjects]));
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException">Thrown if the wherePredicate parameter is null.</exception>
    public async virtual Task<bool> ExistAsync(Expression<Func<T, bool>> wherePredicate, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(wherePredicate);
        bool result = QueryData(wherePredicate).FirstOrDefault() is not null;
        return await Task.FromResult(result);
    }

    /// <inheritdoc/>
    public async virtual Task<List<T>> GetAllAsync(Expression<Func<T, bool>>? wherePredicate = default, Expression<Func<T, object>>? orderByPredicate = default, bool descending = default, CancellationToken cancellationToken = default)
    {
        List<T> dataObjects = QueryData(wherePredicate, orderByPredicate, descending);
        return await Task.FromResult(dataObjects);
    }

    /// <inheritdoc/>
    public async virtual Task<List<ListView>> GetAllListViewAsync(Expression<Func<T, bool>>? wherePredicate = default, Expression<Func<T, object>>? orderByPredicate = default, bool descending = default, CancellationToken cancellationToken = default)
    {
        List<T> dataObjects = QueryData(wherePredicate, orderByPredicate, descending);
        List<ListView> dataObjectListViews = ConvertToListView(dataObjects);
        return await Task.FromResult(dataObjectListViews);
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException">Thrown if the queryDefinition parameter is null.</exception>
    public async Task<PagedList<T>> GetPageAsync(QueryDefinition queryDefinition, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(queryDefinition);
        PagedList<T> pagedDataObjects = QueryData(queryDefinition);
        return await Task.FromResult(pagedDataObjects);
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException">Thrown if the queryDefinition parameter is null.</exception>
    public async Task<PagedList<ListView>> GetPageListViewAsync(QueryDefinition queryDefinition, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(queryDefinition);
        PagedList<T> pagedDataObjects = QueryData(queryDefinition);
        PagedList<ListView> pagedListViews = new()
        {
            DataObjects = ConvertToListView(pagedDataObjects.DataObjects),
            TotalRecords = pagedDataObjects.TotalRecords,
        };
        return await Task.FromResult(pagedListViews);
    }

    /// <inheritdoc/>
    public async virtual Task<T?> GetSingleAsync(Expression<Func<T, bool>>? wherePredicate = default, CancellationToken cancellationToken = default)
    {
        T? dataObject = QueryData(wherePredicate).FirstOrDefault();
        return await Task.FromResult(dataObject);
    }

    /// <summary>
    /// The method increments the identity.
    /// </summary>
    protected void IncrementIdentity() => _identity += 1;

    /// <summary>
    /// The method calls the Created event so any registered handler can react to the event.
    /// </summary>
    /// <param name="e">The arguments associated with the event.</param>
    protected virtual void OnCreated(CreatedEventArgs e) => Created?.Invoke(this, e);

    /// <summary>
    /// The method calls the Deleted event so any registered handler can react to the event.
    /// </summary>
    /// <param name="e">The arguments associated with the event.</param>
    protected virtual void OnDeleted(DeletedEventArgs e) => Deleted?.Invoke(this, e);

    /// <summary>
    /// The method calls the Updated event so any registered handler can react to the event.
    /// </summary>
    /// <param name="e">The arguments associated with the event.</param>
    protected virtual void OnUpdated(UpdatedEventArgs e) => Updated?.Invoke(this, e);

    /// <summary>
    /// The method preps the data object for a create operation.
    /// </summary>
    /// <param name="dataObject">The data object that needs to be preped.</param>
    /// <returns>The data object.</returns>
    protected virtual void PrepForCreate(T dataObject)
    {
        dataObject.Integer64ID = Identity;
        dataObject.CreatedOn = DateTime.Now;
    }

    /// <summary>
    /// The method preps the data object for an update operation.
    /// </summary>
    /// <param name="dataObject">The data object that needs to be preped.</param>
    protected virtual void PrepForUpdate(T dataObject) => dataObject.LastEditedOn = DateTime.Now;

    /// <summary>
    /// The method queries the data.
    /// </summary>
    /// <param name="queryDefinition">Defines how the data should be queried; includes filtering, paging and sorting.</param>
    /// <returns>A list of DataObjects.</returns>
    protected PagedList<T> QueryData(QueryDefinition queryDefinition)
    {
        PagedList<T> pagedDataObjects = new();

        lock (DataStorageLock)
        {
            IEnumerable<T> dataObjectEnumerable = DataStorage.AsEnumerable();

            foreach (FilterDefinition filterDefinition in queryDefinition.FilterDefinitions)
            {
                dataObjectEnumerable = dataObjectEnumerable.Where(filterDefinition.ToExpression<T>().Compile());
            }

            foreach (SortDefinition sortDefinition in queryDefinition.SortDefinitions)
            {
                if (sortDefinition.Descending)
                {
                    dataObjectEnumerable = dataObjectEnumerable.OrderByDescending(sortDefinition.ToExpression<T>().Compile());
                }
                else
                {
                    dataObjectEnumerable = dataObjectEnumerable.OrderBy(sortDefinition.ToExpression<T>().Compile());
                }
            }

            pagedDataObjects.TotalRecords = dataObjectEnumerable.Count();

            if (queryDefinition.Take > 0)
            {
                dataObjectEnumerable = dataObjectEnumerable.Skip(queryDefinition.Skip * queryDefinition.Take);
                dataObjectEnumerable = dataObjectEnumerable.Take(queryDefinition.Take);
            }

            //Create a copy so its independent of the data storage.
            pagedDataObjects.DataObjects = [.. dataObjectEnumerable.Select(CreateCopy)];
        }

        return pagedDataObjects;
    }

    /// <summary>
    /// The method queries the data.
    /// </summary>
    /// <param name="wherePredicate">The where predicate to use against the collection/table.</param>
    /// <param name="orderByPredicate">The order predicate to use against the collection/table.</param>
    /// <param name="descending">False means the data is ordered ascending; true means the data is ordered descending.</param>
    /// <returns>A list of DataObjects.</returns>
    protected List<T> QueryData(Expression<Func<T, bool>>? wherePredicate = default, Expression<Func<T, object>>? orderByPredicate = default, bool descending = default)
    {
        List<T> dataObjects;

        lock (DataStorageLock)
        {
            IEnumerable<T> dataObjectEnumerable = DataStorage.AsEnumerable();

            if (wherePredicate is not null)
            {
                dataObjectEnumerable = dataObjectEnumerable.Where(wherePredicate.Compile());
            }

            if (orderByPredicate is not null)
            {
                if (descending)
                {
                    dataObjectEnumerable = dataObjectEnumerable.OrderByDescending(orderByPredicate.Compile());
                }
                else
                {
                    dataObjectEnumerable = dataObjectEnumerable.OrderBy(orderByPredicate.Compile());
                }
            }

            //Create a copy so its independent of the data storage.
            dataObjects = [.. dataObjectEnumerable.Select(CreateCopy)];
        }

        return dataObjects;
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException">Thrown if the dataObject parameter is null.</exception>
    /// <exception cref="DataObjectValidationException">Thrown if the data object fails validation.</exception>
    /// <exception cref="DataObjectIDNotFoundException">Thrown if the data object's ID is not found in the collection/table.</exception>
    public async virtual Task<T> UpdateAsync(T dataObject, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(dataObject);
        return (await UpdateAsync([dataObject], cancellationToken)).First();
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException">Thrown if the dataObjects parameter is null.</exception>
    /// <exception cref="DataObjectValidationException">Thrown if any data object fails validation.</exception>
    /// <exception cref="DataObjectIDNotFoundException">Thrown if any data objects' ID is not found in the collection/table.</exception>
    public async virtual Task<List<T>> UpdateAsync(List<T> dataObjects, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(dataObjects);

        //Confirm validation because this wants all or none updated.
        await ValidateParamaterDataObjectsAsync(dataObjects, cancellationToken);

        List<T> returnDataObjects = [];

        lock (DataStorageLock)
        {
            List<T> databaseDataObjects = [];

            foreach (T dataObject in dataObjects)
            {
                T? databaseDataObject = DataStorage.FirstOrDefault(obj => obj.Integer64ID == dataObject.Integer64ID) 
                    ?? throw new DataObjectIDNotFoundException(dataObject.Integer64ID.ToString());

                if (IsOldDataObjectDetectionEnabled && AllowToUpdate(databaseDataObject, dataObject) is false)
                {
                    throw new DataObjectUpdateConflictException($"Failed to update because the data object was updated by {databaseDataObject.LastEditedBy ?? databaseDataObject.LastEditedByInteger64ID?.ToString() ?? databaseDataObject.LastEditedByStringID ?? string.Empty} on {databaseDataObject.LastEditedOn}.");
                }

                databaseDataObjects.Add(databaseDataObject);
            }

            for (int index = 0; index < dataObjects.Count; ++index)
            {
                //Prep and update the data object in storage.
                PrepForUpdate(dataObjects[index]);
                databaseDataObjects[index].MapProperties(dataObjects[index]);

                //Create a copy so its independent of the data storage.
                T returnDataObject = CreateCopy(databaseDataObjects[index]);
                returnDataObjects.Add(returnDataObject);
            }
        }

        OnUpdated(new UpdatedEventArgs([.. returnDataObjects]));

        return await Task.FromResult(returnDataObjects);
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException">Thrown if the dataObject parameter is null.</exception>
    public async virtual Task<List<ValidationResult>> ValidateAsync(T dataObject, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(dataObject);
        List<ValidationResult> validationResults = dataObject.Validate();

        if (IsUniqueNameRequired && await ExistAsync(obj => obj.Integer64ID != dataObject.Integer64ID && obj.Name == dataObject.Name, cancellationToken) is true)
        {
            validationResults.Add(new ValidationResult($"The {dataObject.Name} name already exists in the data store.", [nameof(dataObject.Name)]));
        }

        return await Task.FromResult(validationResults);
    }

    /// <summary>
    /// The method determines if the parameter data objects pass validation. Failure throws a ParameterDuplicateNameException or DataObjectValidationException.
    /// </summary>
    /// <param name="dataObjects">The data objects to check.</param>
    /// <param name="cancellationToken">A token used for task cancellations.</param>
    /// <returns>A Task for the async.</returns>
    /// <exception cref="DataObjectValidationException">Thrown if a parameter data object fails validation (data annotations and custom validation rules).</exception>
    /// <exception cref="ParameterDuplicateNameException">Thrown if parameter data objects fail name uniqueness with each other; the IsUniqueNameRequired property needs to be set to true.</exception>
    protected async Task ValidateParamaterDataObjectsAsync(List<T> dataObjects, CancellationToken cancellationToken = default)
    {
        //Confirm the names of the passed in data objects have unique names
        //with each other when uniqueness is enforced by the data layer. To do
        //this use distinct on the names and compare the counts. If the counts
        //do not match that means there was duplicate names.
        if (IsUniqueNameRequired && dataObjects.Count > 1)
        {
            int uniquenessCount = dataObjects.Select(obj => obj.Name).Distinct().Count();

            if (dataObjects.Count != uniquenessCount)
            {
                throw new ParameterDuplicateNameException();
            }
        }

        //Confirm data annotation validation and custom validation.
        foreach (T dataObject in dataObjects)
        {
            List<ValidationResult> validationResults = await ValidateAsync(dataObject, cancellationToken);

            if (validationResults.Count > 0)
            {
                throw new DataObjectValidationException(dataObject, validationResults);
            }
        }
    }
}
