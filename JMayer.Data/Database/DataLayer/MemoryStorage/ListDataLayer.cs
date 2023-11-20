using JMayer.Data.Data;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;

namespace JMayer.Data.Database.DataLayer.MemoryStorage
{
    /// <summary>
    /// The class manages CRUD interactions with a list memory storage.
    /// </summary>
    /// <typeparam name="T">A DataObject which represents data in the collection/table.</typeparam>
    /// <remarks>
    /// The underlying data storage is a list so this shouldn't be used with very large datasets.
    /// </remarks>
    public class ListDataLayer<T> : IDataLayer<T> where T : DataObject, new()
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
        /// <param name="cancellationToken">A token used for task cancellations.</param>
        /// <returns>A count.</returns>
        public async virtual Task<int> CountAsync(CancellationToken cancellationToken = default)
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
        /// <param name="cancellationToken">A token used for task cancellations.</param>
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
        /// <returns>The created data object.</returns>
        public async virtual Task<T> CreateAsync(T dataObject, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(dataObject);

            lock (_dataStorageLock)
            {
                PrepForCreate(dataObject);
                _dataStorage.Add(dataObject);
                _identity += 1;
                dataObject = CreateCopy(dataObject); //Create a copy so its independent of the data storage.
            }

            return await Task.FromResult(dataObject);
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
        /// <returns>A Task object for the async.</returns>
        public async virtual Task DeleteAsync(T dataObject, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(dataObject);

            T? latestDataObject = await GetSingleAsync(obj => obj.Key == dataObject.Key, cancellationToken);

            if (latestDataObject != null)
            {
                lock (_dataStorageLock)
                {
                    _ = _dataStorage.Remove(latestDataObject);
                }
            }
        }

        /// <summary>
        /// The method returns if the key exists in the collection/table.
        /// </summary>
        /// <param name="key">The key to search for.</param>
        /// <param name="cancellationToken">A token used for task cancellations.</param>
        /// <returns>True means the key exists; false means it does not.</returns>
        public async virtual Task<bool> ExistAsync(string key, CancellationToken cancellationToken = default)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(key);
            bool result = QueryData(obj => obj.Key == key).FirstOrDefault() != null;
            return await Task.FromResult(result);
        }

        /// <summary>
        /// The method returns if data objects exists in the collection/table based on a where predicate.
        /// </summary>
        /// <param name="wherePredicate">The where predicate to use against the collection/table.</param>
        /// <param name="cancellationToken">A token used for task cancellations.</param>
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
        /// <returns>A list of DataObjects.</returns>
        public async virtual Task<List<T>> GetAllAsync(Expression<Func<T, bool>> wherePredicate, CancellationToken cancellationToken = default)
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
        /// <param name="cancellationToken">A token used for task cancellations.</param>
        /// <returns>A list of DataObjects.</returns>
        public async virtual Task<List<T>> GetAllAsync(Expression<Func<T, bool>> wherePredicate, Expression<Func<T, bool>> orderByPredicate, bool descending = false, CancellationToken cancellationToken = default)
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
        /// <returns>A DataObject.</returns>
        public async virtual Task<T?> GetSingleAsync(Expression<Func<T, bool>> wherePredicate, CancellationToken cancellationToken = default)
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
        protected virtual void PrepForCreate(T dataObject)
        {
            dataObject.Key = _identity.ToString();
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

                dataObjects = new(dataObjectEnumerable.Select(CreateCopy));
            }

            return dataObjects;
        }

        /// <summary>
        /// The method updates a data object in the table or collection.
        /// </summary>
        /// <param name="dataObject">The data object to update.</param>
        /// <param name="cancellationToken">A token used for task cancellations.</param>
        /// <returns>The latest data object.</returns>
        public async virtual Task<T> UpdateAsync(T dataObject, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(dataObject);

            T? databaseDataObject = await GetSingleAsync(obj => obj.Key == dataObject.Key, cancellationToken);

            if (databaseDataObject == null)
            {
#warning I feel like a more specific exception should be thrown.
                throw new NullReferenceException($"Failed to find the {dataObject.Key} key in the data storage; could not update the data object.");
            }

            lock (_dataStorageLock)
            {
                PrepForUpdate(dataObject);
                databaseDataObject.MapProperties(dataObject);
            }

            return databaseDataObject;
        }

        /// <summary>
        /// The method validates a data object.
        /// </summary>
        /// <param name="dataObject">The data object to validate.</param>
        /// <param name="cancellationToken">A token used for task cancellations.</param>
        /// <returns>The validation result.</returns>
        public async virtual Task<List<ValidationResult>> ValidateAsync(T dataObject, CancellationToken cancellationToken = default)
        {
            //First, validate against the data annotations on the object.
            List<ValidationResult> validationResults = ValidateDataAnnotations(dataObject);

            //Now, validate against the database for with any custom rules.
            if (!string.IsNullOrWhiteSpace(dataObject.Key) && await ExistAsync(dataObject.Key, cancellationToken) == false)
            {
                validationResults.Add(new ValidationResult($"The {dataObject.Key} key does not exist.", new List<string>() { nameof(dataObject.Key) }));
            }

            return validationResults;
        }

        /// <summary>
        /// The method validates the data annotations on the data object.
        /// </summary>
        /// <param name="dataObject">The data object to validate.</param>
        /// <returns>A list of validation results.</returns>
        protected static List<ValidationResult> ValidateDataAnnotations(T dataObject)
        {
            List<ValidationResult> validationResults = [];
            _ = Validator.TryValidateObject(dataObject, new ValidationContext(dataObject), validationResults);
            return validationResults;
        }
    }
}
