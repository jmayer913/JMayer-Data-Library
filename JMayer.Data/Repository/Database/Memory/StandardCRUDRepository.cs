using JMayer.Data.Data;
using JMayer.Data.Data.Query;
using JMayer.Data.Database.DataLayer;
using System.Linq.Expressions;

namespace JMayer.Data.Repository.Database.Memory;

#warning The database version also had Created, Deleted and Updated events so another layer could respond to the operation (like cascade delete or cascade update). Not sure where to put these.

#warning Right now, I'm experimenting with having an interface with linq methods. I like the idea of if the repository is a database then you can run linq queries against it.
#warning It would be better if the linq methods were extensions but that requires direct access to the database (in my case the list).
#warning I'm not sure if its worth the trouble trying to create a universal database interface.
#warning I only have experience with SQL Server with EF and mongodb so I'm not sure what I create will work for everything.
#warning It means I'm creating another layer to setup and manage.

#warning I should double check if I'm using the linq methods. If they're not really being used, maybe I should ditch them.
#warning I think only the sub controller uses them to create where predicates to query for the owner id but I feel like this should be on the repository level and the owner parameter is passed down to it.
#warning I need to double check my example projects and see if any of those utilize the linq methods.

/// <summary>
/// The class manages CRUD interactions with a list of data objects stored in memory.
/// </summary>
/// <typeparam name="T">A DataObject which represents data stored memory.</typeparam>
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
public class StandardCRUDRepository<T> : IStandardCRUDRepository<T>
    where T : DataObject, new()
{
    /// <inheritdoc/>
    public event EventHandler<CreatedEventArgs>? Created;

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

    /// <inheritdoc/>
    public event EventHandler<DeletedEventArgs>? Deleted;

    /// <summary>
    /// The identity of the last created record.
    /// </summary>
    private long _identity = 1;

    /// <summary>
    /// The property gets the identity of the last created record.
    /// </summary>
    protected long Identity => _identity;

    /// <inheritdoc/>
    public event EventHandler<UpdatedEventArgs>? Updated;

    /// <summary>
    /// The method converts the data objects to list view objects.
    /// </summary>
    /// <param name="dataObjects">The data objects to convert.</param>
    /// <returns>A list of ListView objects.</returns>
    protected virtual List<ListView> ConvertToListView(List<T> dataObjects) => dataObjects.ConvertAll(obj => new ListView(obj));

    /// <inheritdoc/>
    public virtual async Task<long> CountAsync(CancellationToken cancellationToken = default)
    {
        long count;

        lock (DataStorageLock)
        {
            count = DataStorage.Count;
        }

        return await Task.FromResult(count);
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException">Thrown if the dataObject parameter is null.</exception>
    public virtual async Task<T> CreateAsync(T dataObject, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(dataObject);
        return (await CreateAsync([dataObject], cancellationToken)).First();
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException">Thrown if the dataObjects parameter is null.</exception>
    public async Task<List<T>> CreateAsync(List<T> dataObjects, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(dataObjects);

        List<T> returnDataObjects = [];

        lock (DataStorageLock)
        {
            foreach (T dataObject in dataObjects)
            {
                T storageDataObject = CreateCopy(dataObject);
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
    public virtual Task DeleteAsync(T dataObject, CancellationToken cancellationToken = default)
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

        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException">Thrown if the dataObject parameter is null.</exception>
    public Task DeleteAsync(List<T> dataObjects, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(dataObjects);

        List<long> ids = [.. dataObjects.Select(obj => obj.Integer64ID)];
        
        //I'm probably bringing back the linq get methods.
        //List<T> databaseDataObjects = await GetAllAsync(obj => ids.Any(id => id == obj.Integer64ID), cancellationToken: cancellationToken);
        List<T> databaseDataObjects = QueryData(obj => ids.Any(id => id == obj.Integer64ID));

        lock (DataStorageLock)
        {
            _ = DataStorage.RemoveAll(obj => ids.Any(id => id == obj.Integer64ID));
        }

        OnDeleted(new DeletedEventArgs([.. databaseDataObjects]));

        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public virtual async Task<List<T>?> GetAllAsync(CancellationToken cancellationToken = default)
    {
        List<T> dataObjects = QueryData();
        return await Task.FromResult(dataObjects);
    }

    /// <inheritdoc/>
    public virtual async Task<List<ListView>?> GetAllListViewAsync(CancellationToken cancellationToken = default)
    {
        List<T> dataObjects = QueryData();
        List<ListView> dataObjectListViews = ConvertToListView(dataObjects);
        return await Task.FromResult(dataObjectListViews);
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException">Thrown if the queryDefinition parameter is null.</exception>
    public virtual async Task<PagedList<T>?> GetPageAsync(QueryDefinition queryDefinition, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(queryDefinition);
        PagedList<T> pagedDataObjects = QueryData(queryDefinition);
        return await Task.FromResult(pagedDataObjects);
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException">Thrown if the queryDefinition parameter is null.</exception>
    public virtual async Task<PagedList<ListView>?> GetPageListViewAsync(QueryDefinition queryDefinition, CancellationToken cancellationToken = default)
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
    public virtual async Task<T?> GetSingleAsync(CancellationToken cancellationToken = default)
    {
        T? dataObject = QueryData().FirstOrDefault();
        return await Task.FromResult(dataObject);
    }

    /// <inheritdoc/>
    public virtual async Task<T?> GetSingleAsync(long id, CancellationToken cancellationToken = default)
    {
        T? dataObject = QueryData(obj => obj.Integer64ID == id).FirstOrDefault();
        return await Task.FromResult(dataObject);
    }

    /// <inheritdoc/>
    public virtual async Task<T?> GetSingleAsync(string id, CancellationToken cancellationToken = default)
    {
        T? dataObject = QueryData(obj => obj.StringID == id).FirstOrDefault();
        return await Task.FromResult(dataObject);
    }

    /// <summary>
    /// The method increments the identity.
    /// </summary>
    protected void IncrementIdentity() => _identity += 1;

    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException">Thrown if the dataObject parameter is null.</exception>
    public virtual async Task<bool> NameExistsAsync(T dataObject, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(dataObject);

        bool result;

        //Exclude the string id because you don't want to find the data object being passed in.
        if (dataObject.StringID is not null)
        {
            result = QueryData(obj => obj.StringID != dataObject.StringID && obj.Name == dataObject.Name).FirstOrDefault() is not null;
        }
        //Exclude the integer id because you don't want to find the data object being passed in.
        else if (dataObject.Integer64ID > 0)
        {
            result = QueryData(obj => obj.Integer64ID != dataObject.Integer64ID && obj.Name == dataObject.Name).FirstOrDefault() is not null;
        }
        //Just check the name; more than likely, the data object is in the process of being created.
        else
        {
            result = QueryData(obj => obj.Name == dataObject.Name).FirstOrDefault() is not null;
        }

        return await Task.FromResult(result);
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
    public virtual async Task<T> UpdateAsync(T dataObject, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(dataObject);
        return (await UpdateAsync([dataObject], cancellationToken)).First();
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException">Thrown if the dataObjects parameter is null.</exception>
    public async Task<List<T>> UpdateAsync(List<T> dataObjects, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(dataObjects);

        List<T> returnDataObjects = [];

        lock (DataStorageLock)
        {
            List<T> databaseDataObjects = [];

            foreach (T dataObject in dataObjects)
            {
                T? databaseDataObject = DataStorage.FirstOrDefault(obj => obj.Integer64ID == dataObject.Integer64ID)
                    ?? throw new IDNotFoundException(dataObject.Integer64ID.ToString());

                databaseDataObjects.Add(databaseDataObject);
            }

            for (int index = 0; index < dataObjects.Count; ++index)
            {
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
}
