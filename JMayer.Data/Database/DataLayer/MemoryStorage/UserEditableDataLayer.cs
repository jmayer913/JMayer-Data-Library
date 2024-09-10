using JMayer.Data.Data;
using JMayer.Data.Data.Query;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;

namespace JMayer.Data.Database.DataLayer.MemoryStorage;

/// <summary>
/// The class manages CRUD interactions with a list memory storage.
/// </summary>
/// <typeparam name="T">A UserEditableDataObject which represents data in the collection/table.</typeparam>
/// <remarks>
/// <para>
/// This uses an 64-integer identity (auto-increments) ID so the DataObject.Integer64ID will be
/// used by this and any outside interactions with the data layer must use DataObject.Integer64ID. 
/// Also, the underlying data storage is a List so this shouldn't be used with very large datasets.
/// </para>
/// <para>
/// UpdateAsync() has conflict detection. Because the UserEditableDataObject has a LastEditedOn property, 
/// update can now determine if the data object passed in is older than the record in memory. When this occurs, 
/// an exception is thrown with the idea the user will need to refresh the data to have the latest and then
/// try editing it again.
/// </para>
/// </remarks>
public class UserEditableDataLayer<T> : StandardCRUDDataLayer<T>, IUserEditableDataLayer<T> where T : UserEditableDataObject, new()
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
    /// The method converts the user editable data objects to list view objects.
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

    /// <inheritdoc/>
    public async virtual Task<List<ListView>> GetAllListViewAsync(Expression<Func<T, bool>>? wherePredicate = default, Expression<Func<T, object>>? orderByPredicate = default, bool descending = default, CancellationToken cancellationToken = default)
    {
        List<T> dataObjects = QueryData(wherePredicate, orderByPredicate, descending);
        List<ListView> dataObjectListViews = ConvertToListView(dataObjects);
        return await Task.FromResult(dataObjectListViews);
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
    protected override void PrepForCreate(T dataObject)
    {
        base.PrepForCreate(dataObject);
        dataObject.CreatedOn = DateTime.Now;
    }

    /// <inheritdoc/>
    protected override void PrepForUpdate(T dataObject)
    {
        base.PrepForUpdate(dataObject);
        dataObject.LastEditedOn = DateTime.Now;
    }

    /// <inheritdoc/>
    /// <exception cref="DataObjectUpdateConflictException">Thrown if any updating data object is considered old.</exception>
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

    /// <inheritdoc/>
    public async override Task<List<ValidationResult>> ValidateAsync(T dataObject, CancellationToken cancellationToken = default)
    {
        List<ValidationResult> validationResults = await base.ValidateAsync(dataObject, cancellationToken);

        if (await ExistAsync(obj => obj.Integer64ID != dataObject.Integer64ID && obj.Name == dataObject.Name, cancellationToken) == true) 
        {
            validationResults.Add(new ValidationResult($"The {dataObject.Name} name already exists in the data store.", new List<string>() { nameof(dataObject.Name) }));
        }

        return validationResults;
    }
}
