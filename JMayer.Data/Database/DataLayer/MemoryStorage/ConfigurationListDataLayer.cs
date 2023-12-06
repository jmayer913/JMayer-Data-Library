using JMayer.Data.Data;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;

namespace JMayer.Data.Database.DataLayer.MemoryStorage;

/// <summary>
/// The class manages CRUD interactions with a list memory storage.
/// </summary>
/// <typeparam name="T">A ConfigurationDataObject which represents data in the collection/table.</typeparam>
/// <remarks>
/// This uses an 64-integer identity (auto-increments) ID so the DataObject.Integer64ID will be
/// used by this and any outside interactions with the data layer must use DataObject.Integer64ID. 
/// Also, the underlying data storage is a List so this shouldn't be used with very large datasets.
/// 
/// UpdateAsync() has conflict detection. Because the ConfigurationDataObject has a LastEditedOn property, 
/// update can now determine if the data object passed in is older than the record in memory. When this occurs, 
/// an exception is thrown with the idea the user will need to refresh the data to have the latest.
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
            Integer64ID = obj.Integer64ID,
            StringID = obj.StringID,
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
    /// <exception cref="ArgumentNullException">Thrown if the wherePredicate parameter is null.</exception>
    /// <returns>A list of DataObjects.</returns>
    public async virtual Task<List<ListView>> GetAllListViewAsync(Expression<Func<T, bool>> wherePredicate, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(wherePredicate);
        List<T> dataObjects = QueryData(wherePredicate);
        List<ListView> dataObjectListViews = ConvertToListView(dataObjects);
        return await Task.FromResult(dataObjectListViews);
    }

    /// <summary>
    /// The method returns all the data objects for the collection/table as a list view with an order.
    /// </summary>
    /// <param name="orderByPredicate">The order predicate to use against the collection/table.</param>
    /// <param name="descending">False means the data is ordered ascending; true means the data is ordered descending.</param>
    /// <param name="cancellationToken">A token used for task cancellations.</param>
    /// <exception cref="ArgumentNullException">Thrown if the orderByPredicate parameter is null.</exception>
    /// <returns>A list of DataObjects.</returns>
    public async Task<List<ListView>> GetAllListViewAsync(Expression<Func<T, object>> orderByPredicate, bool descending = false, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(orderByPredicate);
        List<T> dataObjects = QueryData(null, orderByPredicate, descending);
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
    /// <exception cref="ArgumentNullException">Thrown if the wherePredicate or orderByPredicate parameter is null.</exception>
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
    /// <exception cref="ArgumentNullException">Thrown if the dataObject parameter is null.</exception>
    /// <exception cref="DataObjectUpdateConflictException">Thrown if the data object is older than the record in the collection/table.</exception>
    /// <exception cref="DataObjectValidationException">Thrown if the data object fails validation.</exception>
    /// <exception cref="IDNotFoundException">Thrown if the data object's ID is not found in the collection/table.</exception>
    /// <returns>The latest data object.</returns>
    public async override Task<T> UpdateAsync(T dataObject, CancellationToken cancellationToken = default)
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

            if (!AllowToUpdate(databaseDataObject, dataObject))
            {
                throw new DataObjectUpdateConflictException($"Failed to update because the data object was updated by {databaseDataObject.LastEditedBy ?? databaseDataObject.LastEditedByID ?? string.Empty} on {databaseDataObject.LastEditedOn}.");
            }
            else
            {
                PrepForUpdate(dataObject);
                databaseDataObject.MapProperties(dataObject);
                dataObject = CreateCopy(databaseDataObject);
            }
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
    public async override Task<List<T>> UpdateAsync(List<T> dataObjects, CancellationToken cancellationToken = default)
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

            //Confirm the ID exists and the data object isn't old because this wants all or none updated.
            foreach (T dataObject in dataObjects)
            {
                T? databaseDataObject = DataStorage.FirstOrDefault(obj => obj.Integer64ID == dataObject.Integer64ID);

                if (databaseDataObject == null)
                {
                    throw new IDNotFoundException(dataObject.Integer64ID.ToString());
                }

                if (!AllowToUpdate(databaseDataObject, dataObject))
                {
                    throw new DataObjectUpdateConflictException($"Failed to update because the data object was updated by {databaseDataObject.LastEditedBy ?? databaseDataObject.LastEditedByID ?? string.Empty} on {databaseDataObject.LastEditedOn}.");
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
    public async override Task<List<ValidationResult>> ValidateAsync(T dataObject, CancellationToken cancellationToken = default)
    {
        List<ValidationResult> validationResults = await base.ValidateAsync(dataObject, cancellationToken);

        if (await ExistAsync(obj => obj.Integer64ID != dataObject.Integer64ID && obj.Name == dataObject.Name, cancellationToken) == true) 
        {
            validationResults.Add(new ValidationResult($"The {dataObject.Name} name already exists in the data store.", new List<string>() { nameof(dataObject.Integer64ID) }));
        }

        return validationResults;
    }
}
