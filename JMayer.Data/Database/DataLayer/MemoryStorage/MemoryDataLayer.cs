using JMayer.Data.Data;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;

namespace JMayer.Data.Database.DataLayer.MemoryStorage;

/// <summary>
/// The class manages CRUD interactions with a list memory storage.
/// </summary>
/// <typeparam name="T">A DataObject which represents data in the collection/table.</typeparam>
/// <remarks>
/// This uses an 64-integer identity (auto-increments) ID so the DataObject.Integer64ID will be
/// used by this and any outside interactions with the data layer must use DataObject.Integer64ID. 
/// Also, the underlying data storage is a List so this shouldn't be used with very large datasets.
/// </remarks>
public class MemoryDataLayer<T> : IStandardCRUDDataLayer<T> where T : DataObject, new()
{
    /// <summary>
    /// The memory data storage for this database.
    /// </summary>
    private readonly List<T> _dataStorage = [];

    /// <summary>
    /// The property gets the data storage for this database.
    /// </summary>
    protected List<T> DataStorage
    {
        get => _dataStorage;
    }

    /// <summary>
    /// The lock used when accessing the data storage.
    /// </summary>
    private readonly object _dataStorageLock = new();

    /// <summary>
    /// The property gets the lock used when accessing the data storage.
    /// </summary>
    protected object DataStorageLock 
    { 
        get => _dataStorageLock; 
    }

    /// <summary>
    /// The identity of the last created record.
    /// </summary>
    private long _identity = 1;

    /// <summary>
    /// The property gets the identity of the last created record.
    /// </summary>
    protected long Identity
    {
        get => _identity;
    }

    /// <summary>
    /// A event for when a data object is created in the data layer.
    /// </summary>
    public event EventHandler<CreatedEventArgs>? Created;

    /// <summary>
    /// A event for when a data object is deleted in the data layer.
    /// </summary>
    public event EventHandler<DeletedEventArgs>? Deleted;

    /// <summary>
    /// A event for when a data object is updated in the data layer.
    /// </summary>
    public event EventHandler<UpdatedEventArgs>? Updated;

    /// <summary>
    /// The method returns the total count of data objects in a collection/table.
    /// </summary>
    /// <param name="cancellationToken">A token used for task cancellations.</param>
    /// <returns>A count.</returns>
    public async virtual Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        int count = 0;

        lock (DataStorageLock)
        {
            count = DataStorage.Count;
        }

        return await Task.FromResult(count);
    }

    /// <summary>
    /// The method returns a count of data objects in a collection/table based on a where predicate.
    /// </summary>
    /// <param name="wherePredicate">The where predicate to use against the collection/table.</param>
    /// <param name="cancellationToken">A token used for task cancellations.</param>
    /// <exception cref="ArgumentNullException">Thrown if the wherePredicate parameter is null.</exception>
    /// <returns>A count.</returns>
    public async virtual Task<int> CountAsync(Expression<Func<T, bool>> wherePredicate, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(wherePredicate);
        List<T> dataObjects = QueryData(wherePredicate);
        return await Task.FromResult(dataObjects.Count);
    }

    /// <summary>
    /// The method creates a data object in the table or collection.
    /// </summary>
    /// <param name="dataObject">The data object to create.</param>
    /// <param name="cancellationToken">A token used for task cancellations.</param>
    /// <exception cref="ArgumentNullException">Thrown if the dataObject parameter is null.</exception>
    /// <exception cref="DataObjectValidationException">Thrown if the data object fails validation.</exception>
    /// <returns>The created data object.</returns>
    public async virtual Task<T> CreateAsync(T dataObject, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(dataObject);

        List<ValidationResult> validationResults = await ValidateAsync(dataObject, cancellationToken);

        if (validationResults.Count > 0)
        {
            throw new DataObjectValidationException(dataObject, validationResults);
        }

        lock (DataStorageLock)
        {
            PrepForCreate(dataObject);
            DataStorage.Add(dataObject);
            IncrementIdentity();
            dataObject = CreateCopy(dataObject); //Create a copy so its independent of the data storage.
        }

        OnCreated(new CreatedEventArgs([dataObject]));

        return await Task.FromResult(dataObject);
    }

    /// <summary>
    /// The method creates multiple data objects in the collection/table.
    /// </summary>
    /// <param name="dataObjects">The data objects to create.</param>
    /// <param name="cancellationToken">A token used for task cancellations.</param>
    /// <exception cref="ArgumentNullException">Thrown if the dataObjects parameter is null.</exception>
    /// <exception cref="DataObjectValidationException">Thrown if the data object fails validation.</exception>
    /// <returns>The created data object.</returns>
    public async virtual Task<List<T>> CreateAsync(List<T> dataObjects, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(dataObjects);

        //Confirm validation because this wants all or none created.
        foreach (T dataObject in dataObjects)
        {
            List<ValidationResult> validationResults = await ValidateAsync(dataObject, cancellationToken);

            if (validationResults.Count > 0)
            {
                throw new DataObjectValidationException(dataObject, validationResults);
            }
        }

        List<T> returnDataObjects = [];

        lock (DataStorageLock)
        {
            foreach (T dataObject in dataObjects)
            {
                PrepForCreate(dataObject);
                DataStorage.Add(dataObject);
                IncrementIdentity();

                T returnDataObject = CreateCopy(dataObject);
                returnDataObjects.Add(returnDataObject); //Create a copy so its independent of the data storage.
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

    /// <summary>
    /// The method deletes a data object in the table or collection.
    /// </summary>
    /// <param name="dataObject">The data object to delete.</param>
    /// <param name="cancellationToken">A token used for task cancellations.</param>
    /// <exception cref="ArgumentNullException">Thrown if the dataObject parameter is null.</exception>
    /// <returns>A Task object for the async.</returns>
    public async virtual Task DeleteAsync(T dataObject, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(dataObject);

        lock (DataStorageLock)
        {
            T? databaseDataObject = DataStorage.FirstOrDefault(obj => obj.Integer64ID == dataObject.Integer64ID);

            if (databaseDataObject != null)
            {
                _ = DataStorage.Remove(databaseDataObject);
                OnDeleted(new DeletedEventArgs([databaseDataObject]));
            }
        }
    }

    /// <summary>
    /// The method deletes multiple data objects in the collection/table.
    /// </summary>
    /// <param name="dataObjects">The data objects to delete.</param>
    /// <param name="cancellationToken">A token used for task cancellations.</param>
    /// <exception cref="ArgumentNullException">Thrown if the dataObjects parameter is null.</exception>
    /// <returns>A Task object for the async.</returns>
    public async virtual Task DeleteAsync(List<T> dataObjects, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(dataObjects);

        List<long> ids = dataObjects.Select(obj => obj.Integer64ID ?? 0).ToList();
        List<T> databaseDataObjects = await GetAllAsync(obj => ids.Any(id => id == obj.Integer64ID));

        lock (DataStorageLock)
        {
            _ = DataStorage.RemoveAll(obj => ids.Any(id => id == obj.Integer64ID));
        }

        OnDeleted(new DeletedEventArgs([.. databaseDataObjects]));
    }

    /// <summary>
    /// The method deletes multiple data objects in the collection/table.
    /// </summary>
    /// <param name="wherePredicate">The where predicate to use when deleting records.</param>
    /// <param name="cancellationToken">A token used for task cancellations.</param>
    /// <exception cref="ArgumentNullException">Thrown if the wherePredicate parameter is null.</exception>
    /// <returns>A Task object for the async.</returns>
    public async virtual Task DeleteAsync(Expression<Func<T, bool>> wherePredicate, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(wherePredicate);

        List<T> dataObjects = await GetAllAsync(wherePredicate, cancellationToken);
        List<long> ids = dataObjects.Select(obj => obj.Integer64ID ?? 0).ToList();

        lock (DataStorageLock)
        {
            _ = DataStorage.RemoveAll(obj => ids.Any(id => id == obj.Integer64ID));
        }

        OnDeleted(new DeletedEventArgs([.. dataObjects]));
    }

    /// <summary>
    /// The method returns if data objects exists in the collection/table based on a where predicate.
    /// </summary>
    /// <param name="wherePredicate">The where predicate to use against the collection/table.</param>
    /// <param name="cancellationToken">A token used for task cancellations.</param>
    /// <exception cref="ArgumentNullException">Thrown if the wherePredicate parameter is null.</exception>
    /// <returns>True means the data objects exists based on the expression; false means none do.</returns>
    public async virtual Task<bool> ExistAsync(Expression<Func<T, bool>> wherePredicate, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(wherePredicate);
        bool result = QueryData(wherePredicate).FirstOrDefault() != null;
        return await Task.FromResult(result);
    }

    /// <summary>
    /// The method returns all the data objects for the table or collection.
    /// </summary>
    /// <param name="cancellationToken">A token used for task cancellations.</param>
    /// <returns>A list of DataObjects.</returns>
    public async virtual Task<List<T>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        List<T> dataObjects = QueryData();
        return await Task.FromResult(dataObjects);
    }

    /// <summary>
    /// The method returns all the data objects for the collection/table based on a where predicate.
    /// </summary>
    /// <param name="wherePredicate">The where predicate to use against the collection/table.</param>
    /// <param name="cancellationToken">A token used for task cancellations.</param>
    /// <exception cref="ArgumentNullException">Thrown if the wherePredicate parameter is null.</exception>
    /// <returns>A list of DataObjects.</returns>
    public async virtual Task<List<T>> GetAllAsync(Expression<Func<T, bool>> wherePredicate, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(wherePredicate);
        List<T> dataObjects = QueryData(wherePredicate);
        return await Task.FromResult(dataObjects);
    }

    /// <summary>
    /// The method returns all the data objects for the collection/table with an order.
    /// </summary>
    /// <param name="orderByPredicate">The order predicate to use against the collection/table.</param>
    /// <param name="descending">False means the data is ordered ascending; true means the data is ordered descending.</param>
    /// <param name="cancellationToken">A token used for task cancellations.</param>
    /// <exception cref="ArgumentNullException">Thrown if the orderByPredicate parameter is null.</exception>
    /// <returns>A list of DataObjects.</returns>
    public async Task<List<T>> GetAllAsync(Expression<Func<T, object>> orderByPredicate, bool descending = false, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(orderByPredicate);
        List<T> dataObjects = QueryData(null, orderByPredicate, descending);
        return await Task.FromResult(dataObjects);
    }

    /// <summary>
    /// The method returns all the data objects for the collection/table based on a where predicate with an order.
    /// </summary>
    /// <param name="wherePredicate">The where predicate to use against the collection/table.</param>
    /// <param name="orderByPredicate">The order predicate to use against the collection/table.</param>
    /// <param name="descending">False means the data is ordered ascending; true means the data is ordered descending.</param>
    /// <exception cref="ArgumentNullException">Thrown if the wherePredicate or orderByPredicate parameter is null.</exception>
    /// <param name="cancellationToken">A token used for task cancellations.</param>
    /// <returns>A list of DataObjects.</returns>
    public async virtual Task<List<T>> GetAllAsync(Expression<Func<T, bool>> wherePredicate, Expression<Func<T, object>> orderByPredicate, bool descending = false, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(wherePredicate);
        ArgumentNullException.ThrowIfNull(orderByPredicate);
        List<T> dataObjects = QueryData(wherePredicate, orderByPredicate, descending);
        return await Task.FromResult(dataObjects);
    }

    /// <summary>
    /// The method returns the first data object in the collection/table.
    /// </summary>
    /// <param name="cancellationToken">A token used for task cancellations.</param>
    /// <returns>A DataObject.</returns>
    public async virtual Task<T?> GetSingleAsync(CancellationToken cancellationToken = default)
    {
        T? dataObject = QueryData().FirstOrDefault();
        return await Task.FromResult(dataObject);
    }

    /// <summary>
    /// The method returns a data object in the collection/table based on a where predicate.
    /// </summary>
    /// <param name="wherePredicate">The where predicate to use against the collection/table.</param>
    /// <param name="cancellationToken">A token used for task cancellations.</param>
    /// <exception cref="ArgumentNullException">Thrown if the wherePredicate parameter is null.</exception>
    /// <returns>A DataObject.</returns>
    public async virtual Task<T?> GetSingleAsync(Expression<Func<T, bool>> wherePredicate, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(wherePredicate);
        T? dataObject = QueryData(wherePredicate).FirstOrDefault();
        return await Task.FromResult(dataObject);
    }

    /// <summary>
    /// The method increments the identity.
    /// </summary>
    protected void IncrementIdentity()
    {
        _identity += 1;
    }

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
    }

    /// <summary>
    /// The method preps the data object for an update operation.
    /// </summary>
    /// <param name="dataObject">The data object that needs to be preped.</param>
    protected virtual void PrepForUpdate(T dataObject) { }

    /// <summary>
    /// The method queries the data.
    /// </summary>
    /// <param name="wherePredicate">The where predicate to use against the collection/table.</param>
    /// <param name="orderByPredicate">The order predicate to use against the collection/table.</param>
    /// <param name="descending">False means the data is ordered ascending; true means the data is ordered descending.</param>
    /// <returns>A list of DataObjects.</returns>
    protected List<T> QueryData(Expression<Func<T, bool>>? wherePredicate = null, Expression<Func<T, object>>? orderByPredicate = null, bool descending = false)
    {
        List<T> dataObjects;

        lock (DataStorageLock)
        {
            IEnumerable<T> dataObjectEnumerable = DataStorage.AsEnumerable();

            if (wherePredicate != null)
            {
                dataObjectEnumerable = dataObjectEnumerable.Where(wherePredicate.Compile());
            }

            if (orderByPredicate != null)
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

            dataObjects = new(dataObjectEnumerable.Select(CreateCopy));
        }

        return dataObjects;
    }

    /// <summary>
    /// The method updates a data object in the table or collection.
    /// </summary>
    /// <param name="dataObject">The data object to update.</param>
    /// <param name="cancellationToken">A token used for task cancellations.</param>
    /// <exception cref="ArgumentNullException">Thrown if the dataObject parameter is null.</exception>
    /// <exception cref="DataObjectValidationException">Thrown if the data object fails validation.</exception>
    /// <exception cref="IDNotFoundException">Thrown if the data object's ID is not found in the collection/table.</exception>
    /// <returns>The latest data object.</returns>
    public async virtual Task<T> UpdateAsync(T dataObject, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(dataObject);

        List<ValidationResult> validationResults = await ValidateAsync(dataObject, cancellationToken);

        if (validationResults.Count > 0)
        {
            throw new DataObjectValidationException(dataObject, validationResults);
        }

        lock (DataStorageLock)
        {
            T? databaseDataObject = DataStorage.FirstOrDefault(obj => obj.Integer64ID == dataObject.Integer64ID);

            if (databaseDataObject == null)
            {
                throw new IDNotFoundException(dataObject.Integer64ID.ToString());
            }

            PrepForUpdate(dataObject);
            databaseDataObject.MapProperties(dataObject);
            dataObject = CreateCopy(databaseDataObject);
        }

        OnUpdated(new UpdatedEventArgs([dataObject]));

        return await Task.FromResult(dataObject);
    }

    /// <summary>
    /// The method updates multiple data objects in the collection/table.
    /// </summary>
    /// <param name="dataObjects">The data objects to update.</param>
    /// <param name="cancellationToken">A token used for task cancellations.</param>
    /// <exception cref="ArgumentNullException">Thrown if the dataObjects parameter is null.</exception>
    /// <exception cref="DataObjectValidationException">Thrown if any data object fails validation.</exception>
    /// <exception cref="IDNotFoundException">Thrown if any data objects' ID is not found in the collection/table.</exception>
    /// <returns>The latest data object.</returns>
    public async virtual Task<List<T>> UpdateAsync(List<T> dataObjects, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(dataObjects);

        //Confirm validation because this wants all or none updated.
        foreach (T dataObject in dataObjects)
        {
            List<ValidationResult> validationResults = await ValidateAsync(dataObject, cancellationToken);

            if (validationResults.Count > 0)
            {
                throw new DataObjectValidationException(dataObject, validationResults);
            }
        }

        List<T> returnDataObjects = [];

        lock (DataStorageLock)
        {
            List<T> databaseDataObjects = [];

            foreach (T dataObject in dataObjects)
            {
                T? databaseDataObject = DataStorage.FirstOrDefault(obj => obj.Integer64ID == dataObject.Integer64ID);

                if (databaseDataObject == null)
                {
                    throw new IDNotFoundException(dataObject.Integer64ID.ToString());
                }

                databaseDataObjects.Add(databaseDataObject);
            }

            for (int index = 0; index < dataObjects.Count; ++index)
            {
                PrepForUpdate(dataObjects[index]);
                databaseDataObjects[index].MapProperties(dataObjects[index]);

                T returnDataObject = CreateCopy(databaseDataObjects[index]);
                returnDataObjects.Add(returnDataObject);
            }
        }

        OnUpdated(new UpdatedEventArgs([.. returnDataObjects]));

        return await Task.FromResult(returnDataObjects);
    }

    /// <summary>
    /// The method validates a data object.
    /// </summary>
    /// <param name="dataObject">The data object to validate.</param>
    /// <param name="cancellationToken">A token used for task cancellations.</param>
    /// <exception cref="ArgumentNullException">Thrown if the dataObject parameter is null.</exception>
    /// <returns>The validation result.</returns>
    /// <remarks>
    /// //This only validates the data annotations on the object but subclasses can override this method to 
    /// add custom rules that validate against the records in the database.
    /// </remarks>
    public async virtual Task<List<ValidationResult>> ValidateAsync(T dataObject, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(dataObject);
        List<ValidationResult> validationResults = dataObject.Validate();
        return await Task.FromResult(validationResults);
    }
}
