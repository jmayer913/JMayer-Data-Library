﻿using JMayer.Data.Data;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;

namespace JMayer.Data.Database.DataLayer.MemoryStorage
{
    /// <summary>
    /// The class manages CRUD interactions with a list memory storage.
    /// </summary>
    /// <typeparam name="T">A ConfigurationDataObject which represents data in the collection/table.</typeparam>
    /// <remarks>
    /// The underlying data storage is a list so this shouldn't be used with very large datasets.
    /// </remarks>
    public class ConfigurationListDataLayer<T> : ListDataLayer<T>, IConfigurationDataLayer<T> where T : ConfigurationDataObject, new()
    {
        /// <summary>
        /// The method returns if there's no update conflict.
        /// </summary>
        /// <param name="databaseDataObject">The data object in the database.</param>
        /// <param name="userDataObject">The data object updated by the user.</param>
        /// <returns>True means the data object can be updated; false means there's a conflict.</returns>
        private static bool AllowToUpdate(T databaseDataObject, T userDataObject)
        {
            return databaseDataObject.LastEditedOn == userDataObject.LastEditedOn;
        }

        /// <summary>
        /// The method converts the configuration data objects to list view objects.
        /// </summary>
        /// <param name="dataObjects">The data objects to convert.</param>
        /// <returns>A list of ListView objects.</returns>
        private static List<ListView> ConvertToListView(List<T> dataObjects)
        {
            return dataObjects.ConvertAll(obj => new ListView()
            {
                Key = obj.Key,
                Name = obj.Name,
            });
        }

        /// <summary>
        /// The method returns all the data objects for the table or collection as a list view.
        /// </summary>
        /// <param name="cancellationToken">A token used for task cancellations.</param>
        /// <returns>A list of DataObjects.</returns>
        public async virtual Task<List<ListView>> GetAllListViewAsync(CancellationToken cancellationToken = default)
        {
            List<T> dataObjects = QueryData();
            List<ListView> dataObjectListViews = ConvertToListView(dataObjects);
            return await Task.FromResult(dataObjectListViews);
        }

        /// <summary>
        /// The method returns all the data objects for the collection/table as a list view based on a where predicate.
        /// </summary>
        /// <param name="wherePredicate">The where predicate to use against the collection/table.</param>
        /// <param name="cancellationToken">A token used for task cancellations.</param>
        /// <returns>A list of DataObjects.</returns>
        public async virtual Task<List<ListView>> GetAllListViewAsync(Expression<Func<T, bool>> wherePredicate, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(wherePredicate);
            List<T> dataObjects = QueryData(wherePredicate);
            List<ListView> dataObjectListViews = ConvertToListView(dataObjects);
            return await Task.FromResult(dataObjectListViews);
        }

        /// <summary>
        /// The method returns all the data objects for the collection/table as a list view based on a where predicate with an order.
        /// </summary>
        /// <param name="wherePredicate">The where predicate to use against the collection/table.</param>
        /// <param name="orderByPredicate">The order predicate to use against the collection/table.</param>
        /// <param name="descending">False means the data is ordered ascending; true means the data is ordered descending.</param>
        /// <param name="cancellationToken">A token used for task cancellations.</param>
        /// <returns>A list of DataObjects.</returns>
        public async virtual Task<List<ListView>> GetAllListViewAsync(Expression<Func<T, bool>> wherePredicate, Expression<Func<T, object>> orderByPredicate, bool descending = false, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(wherePredicate);
            ArgumentNullException.ThrowIfNull(orderByPredicate);
            List<T> dataObjects = QueryData(wherePredicate, orderByPredicate, descending);
            List<ListView> dataObjectListViews = ConvertToListView(dataObjects);
            return await Task.FromResult(dataObjectListViews);
        }

        /// <summary>
        /// The method preps the data object for creation.
        /// </summary>
        /// <param name="dataObject">The data object that needs to be preped.</param>
        protected override void PrepForCreate(T dataObject)
        {
            base.PrepForCreate(dataObject);
            dataObject.CreatedOn = DateTime.Now;
        }

        /// <summary>
        /// The method preps the data object for an update.
        /// </summary>
        /// <param name="dataObject">The data object that needs to be preped.</param>
        protected override void PrepForUpdate(T dataObject)
        {
            base.PrepForUpdate(dataObject);
            dataObject.LastEditedOn = DateTime.Now;
        }

        /// <summary>
        /// The method updates a data object in the table or collection.
        /// </summary>
        /// <param name="dataObject">The data object to update.</param>
        /// <param name="cancellationToken">A token used for task cancellations.</param>
        /// <returns>The latest data object.</returns>
        public async override Task<T> UpdateAsync(T dataObject, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(dataObject);

            T? databaseDataObject = await GetSingleAsync(obj => obj.Key == dataObject.Key, cancellationToken);

            if (databaseDataObject == null)
            {
#warning I feel like a more specific exception should be thrown.
                throw new NullReferenceException($"Failed to find the {dataObject.Key} key in the data storage; could not update the data object.");
            }

            if (!AllowToUpdate(databaseDataObject, dataObject))
            {
                throw new UpdateConflictException($"Failed to update because the data object was updated by {databaseDataObject.LastEditedBy ?? databaseDataObject.LastEditedByKey ?? string.Empty} on {databaseDataObject.LastEditedOn}.");
            }
            else
            {
                lock (GetDataStorageLock())
                {
                    PrepForUpdate(dataObject);
                    databaseDataObject.MapProperties(dataObject);
                }
            }
            
            return databaseDataObject;
        }

        /// <summary>
        /// The method validates a data object.
        /// </summary>
        /// <param name="dataObject">The data object to validate.</param>
        /// <param name="cancellationToken">A token used for task cancellations.</param>
        /// <returns>The validation result.</returns>
        public async override Task<List<ValidationResult>> ValidateAsync(T dataObject, CancellationToken cancellationToken = default)
        {
            List<ValidationResult> validationResults = await base.ValidateAsync(dataObject, cancellationToken);

            if (await ExistAsync(obj => obj.Name == dataObject.Name, cancellationToken) == true) 
            {
                validationResults.Add(new ValidationResult($"The {dataObject.Name} name already exists in the data store.", new List<string>() { nameof(dataObject.Key) }));
            }

            return validationResults;
        }
    }
}
