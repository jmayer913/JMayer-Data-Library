using JMayer.Data.Data;
using System.Linq.Expressions;

#warning Because this is memory based and objects are reference types, I wonder if I should be returning copies so the user can't update data directly and instead they must use the UpdateAsync().

namespace JMayer.Data.Database.DataLayer.MemoryStorage
{
    /// <summary>
    /// The class manages CRUD interactions with a list memory storage.
    /// </summary>
    /// <typeparam name="T">A DataObject which represents data in the collection/table.</typeparam>
    /// <remarks>
    /// The underlying data storage is a list so this shouldn't be used with very large datasets.
    /// </remarks>
    public class ListDataLayer<T> : IDataLayer<T> where T : DataObject
    {
        /// <summary>
        /// The memory data storage for this database.
        /// </summary>
        private readonly List<T> _dataStorage = [];

        /// <summary>
        /// The lock used when accessing the data storage.
        /// </summary>
        private readonly object _dataStorageLock = new();

        /// <summary>
        /// The last identity.
        /// </summary>
        private long _identity = 1;

        /// <summary>
        /// The method returns the total count of data objects in a collection/table.
        /// </summary>
        /// <returns>A count.</returns>
        public async virtual Task<int> CountAsync()
        {
            int count = 0;

            lock (_dataStorageLock)
            {
                count = _dataStorage.Count;
            }

            return await Task.FromResult(count);
        }

        /// <summary>
        /// The method returns a count of data objects in a collection/table based on a where predicate.
        /// </summary>
        /// <param name="wherePredicate">The where predicate to use against the collection/table.</param>
        /// <returns>A count.</returns>
        public async virtual Task<int> CountAsync(Expression<Func<T, bool>> wherePredicate)
        {
            ArgumentNullException.ThrowIfNull(wherePredicate);
            List<T> dataObjects = QueryData(wherePredicate);
            return await Task.FromResult(dataObjects.Count);
        }

        /// <summary>
        /// The method creates a data object in the table or collection.
        /// </summary>
        /// <param name="dataObject">The data object to create.</param>
        /// <returns>The created data object.</returns>
        public async virtual Task<T> CreateAsync(T dataObject)
        {
            ArgumentNullException.ThrowIfNull(dataObject);

            lock (_dataStorageLock)
            {
                dataObject = PrepForCreate(dataObject);
                _dataStorage.Add(dataObject);
                _identity += 1;
            }

            return await Task.FromResult(dataObject);
        }

        /// <summary>
        /// The method deletes a data object in the table or collection.
        /// </summary>
        /// <param name="dataObject">The data object to delete.</param>
        /// <returns>A Task object for the async.</returns>
        public async virtual Task DeleteAsync(T dataObject)
        {
            ArgumentNullException.ThrowIfNull(dataObject);

            T? latestDataObject = await GetSingleAsync(obj => obj.Key == dataObject.Key);

            if (latestDataObject != null)
            {
                lock (_dataStorageLock)
                {
                    _ = _dataStorage.Remove(latestDataObject);
                }
            }
        }

        /// <summary>
        /// The method returns all the data objects for the table or collection.
        /// </summary>
        /// <returns>A list of DataObjects.</returns>
        public async virtual Task<List<T>> GetAllAsync()
        {
            List<T> dataObjects = QueryData();
            return await Task.FromResult(dataObjects);
        }

        /// <summary>
        /// The method returns all the data objects for the collection/table based on a where predicate.
        /// </summary>
        /// <param name="wherePredicate">The where predicate to use against the collection/table.</param>
        /// <returns>A list of DataObjects.</returns>
        public async virtual Task<List<T>> GetAllAsync(Expression<Func<T, bool>> wherePredicate)
        {
            ArgumentNullException.ThrowIfNull(wherePredicate);
            List<T> dataObjects = QueryData(wherePredicate);
            return await Task.FromResult(dataObjects);
        }

        /// <summary>
        /// The method returns all the data objects for the collection/table based on a where predicate with an order.
        /// </summary>
        /// <param name="wherePredicate">The where predicate to use against the collection/table.</param>
        /// <param name="orderByPredicate">The order predicate to use against the collection/table.</param>
        /// <param name="descending">False means the data is ordered ascending; true means the data is ordered descending.</param>
        /// <returns>A list of DataObjects.</returns>
        public async virtual Task<List<T>> GetAllAsync(Expression<Func<T, bool>> wherePredicate, Expression<Func<T, bool>> orderByPredicate, bool descending = false)
        {
            ArgumentNullException.ThrowIfNull(wherePredicate);
            ArgumentNullException.ThrowIfNull(orderByPredicate);
            List<T> dataObjects = QueryData(wherePredicate, orderByPredicate, descending);
            return await Task.FromResult(dataObjects);
        }

        /// <summary>
        /// The method returns the lock object.
        /// </summary>
        /// <returns>The lock object.</returns>
        protected object GetDataStorageLock() => _dataStorageLock;

        /// <summary>
        /// The method returns the first data object in the collection/table.
        /// </summary>
        /// <returns>A DataObject.</returns>
        public async virtual Task<T?> GetSingleAsync()
        {
            T? dataObject;

            lock (_dataStorageLock)
            {
                dataObject = _dataStorage.FirstOrDefault();
            }

            return await Task.FromResult(dataObject);
        }

        /// <summary>
        /// The method returns a data object in the collection/table based on a where predicate.
        /// </summary>
        /// <param name="wherePredicate">The where predicate to use against the collection/table.</param>
        /// <returns>A DataObject.</returns>
        public async virtual Task<T?> GetSingleAsync(Expression<Func<T, bool>> wherePredicate)
        {
            ArgumentNullException.ThrowIfNull(wherePredicate);
            T? dataObject = QueryData(wherePredicate).FirstOrDefault();
            return await Task.FromResult(dataObject);
        }

        /// <summary>
        /// The method preps the data object for a create operation.
        /// </summary>
        /// <param name="dataObject">The data object that needs to be preped.</param>
        /// <returns>The data object.</returns>
        protected virtual T PrepForCreate(T dataObject)
        {
            dataObject.Key = _identity.ToString();
            return dataObject;
        }

        /// <summary>
        /// The method preps the data object for an update operation.
        /// </summary>
        /// <param name="dataObject">The data object that needs to be preped.</param>
        /// <returns>The data object.</returns>
        protected virtual T PrepForUpdate(T dataObject)
        {
            return dataObject;
        }

        /// <summary>
        /// The method queries the data.
        /// </summary>
        /// <param name="wherePredicate">The where predicate to use against the collection/table.</param>
        /// <param name="orderByPredicate">The order predicate to use against the collection/table.</param>
        /// <param name="descending">False means the data is ordered ascending; true means the data is ordered descending.</param>
        /// <returns>A list of DataObjects.</returns>
        protected List<T> QueryData(Expression<Func<T, bool>>? wherePredicate = null, Expression<Func<T, bool>>? orderByPredicate = null, bool descending = false)
        {
            List<T> dataObjects;

            lock (_dataStorageLock)
            {
                IEnumerable<T> dataObjectEnumerable = _dataStorage.AsEnumerable();

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

                dataObjects = new(dataObjectEnumerable);
            }

            return dataObjects;
        }

        /// <summary>
        /// The method updates a data object in the table or collection.
        /// </summary>
        /// <param name="dataObject">The data object to update.</param>
        /// <returns>The latest data object.</returns>
        public async virtual Task<T> UpdateAsync(T dataObject)
        {
            ArgumentNullException.ThrowIfNull(dataObject);

            T? databaseDataObject = await GetSingleAsync(obj => obj.Key == dataObject.Key);

            if (databaseDataObject == null)
            {
                throw new NullReferenceException($"Failed to find the {dataObject.Key} key in the data storage; could not update the data object.");
            }

            lock (_dataStorageLock)
            {
                dataObject = PrepForUpdate(dataObject);
                databaseDataObject.MapProperties(dataObject);
            }

            return databaseDataObject;
        }
    }
}
