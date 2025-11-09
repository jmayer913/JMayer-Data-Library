using JMayer.Data.Data;
using JMayer.Data.Data.Query;
using JMayer.Data.Repository;
using System.ComponentModel.DataAnnotations;

namespace JMayer.Data.Service;

/// <summary>
/// The class manages CRUD interactions with a repository.
/// </summary>
/// <typeparam name="T">A DataObject which represents data for the service interactions.</typeparam>
/// <typeparam name="U">An IStandardCRUDRepository which the service uses to interact with data in the repository.</typeparam>
public class StandardCRUDService<T, U> : IStandardCRUDService<T, U>
    where T : DataObject
    where U : IStandardCRUDRepository<T>
{
    /// <inheritdoc/>
    public bool IsOldDataDetectionEnabled { get; init; }

    /// <inheritdoc/>
    public bool IsUniqueNameRequired { get; init; }

    /// <inheritdoc/>
    public bool IsValidationEnabled { get; init; } = true;

    /// <summary>
    /// The property gets the repository used to access the data.
    /// </summary>
    protected U Repository { get; private init; }

    /// <summary>
    /// The dependency injection constructor.
    /// </summary>
    /// <param name="Repository">The repository used to access the data.</param>
    public StandardCRUDService(U Repository) => this.Repository = Repository;

    /// <summary>
    /// The method returns if there's no update conflict.
    /// </summary>
    /// <param name="databaseDataObject">The data object in the database.</param>
    /// <param name="userDataObject">The data object updated by the user.</param>
    /// <returns>True means the data object can be updated; false means there's a conflict.</returns>
    private static bool AllowToUpdate(T databaseDataObject, T userDataObject)
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

    /// <inheritdoc/>
    public async Task<long> CountAsync(CancellationToken cancellationToken = default)
        => await Repository.CountAsync(cancellationToken);

    /// <inheritdoc/>
    public async Task<OperationResult> CreateAsync(T dataObject, CancellationToken cancellationToken = default)
    {
        if (IsValidationEnabled)
        {
            List<ValidationResult> validationResults = await ValidateAsync(dataObject, cancellationToken);

            if (validationResults.Count > 0)
            {
                return new OperationResult(validationResults);
            }
        }

        dataObject = await Repository.CreateAsync(dataObject, cancellationToken);

        return new OperationResult(dataObject);
    }

    /// <inheritdoc/>
    public async Task<OperationResult> DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        T? dataObject = await GetSingleAsync(id, cancellationToken);

        if (dataObject is null)
        {
#warning Cleanup the message.
            return new OperationResult("Not Found.");
        }

        await Repository.DeleteAsync(dataObject, cancellationToken);

        return new OperationResult(dataObject);
    }

    /// <inheritdoc/>
    public async Task<OperationResult> DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        T? dataObject = await GetSingleAsync(id, cancellationToken);

        if (dataObject is null)
        {
#warning Cleanup the message.
            return new OperationResult("Not Found.");
        }

        await Repository.DeleteAsync(dataObject, cancellationToken);

        return new OperationResult(dataObject);
    }

    /// <inheritdoc/>
    public async Task<List<T>?> GetAllAsync(CancellationToken cancellationToken = default)
        => await Repository.GetAllAsync(cancellationToken);

    /// <inheritdoc/>
    public Task<List<ListView>?> GetAllListViewAsync(CancellationToken cancellationToken = default)
        => Repository.GetAllListViewAsync(cancellationToken);

    /// <inheritdoc/>
    public async Task<PagedList<T>?> GetPageAsync(QueryDefinition queryDefinition, CancellationToken cancellationToken = default)
        => await Repository.GetPageAsync(queryDefinition, cancellationToken);

    /// <inheritdoc/>
    public async Task<PagedList<ListView>?> GetPageListViewAsync(QueryDefinition queryDefinition, CancellationToken cancellationToken = default)
        => await Repository.GetPageListViewAsync(queryDefinition, cancellationToken);

    /// <inheritdoc/>
    public async Task<T?> GetSingleAsync(CancellationToken cancellationToken = default)
        => await Repository.GetSingleAsync(cancellationToken);

    /// <inheritdoc/>
    public async Task<T?> GetSingleAsync(long id, CancellationToken cancellationToken = default)
        => await Repository.GetSingleAsync(id, cancellationToken);

    /// <inheritdoc/>
    public async Task<T?> GetSingleAsync(string id, CancellationToken cancellationToken = default)
        => await Repository.GetSingleAsync(id, cancellationToken);

    /// <inheritdoc/>
    public async Task<OperationResult> UpdateAsync(T dataObject, CancellationToken cancellationToken = default)
    {
        if (IsValidationEnabled)
        {
            List<ValidationResult> validationResults = await ValidateAsync(dataObject, cancellationToken);

            if (validationResults.Count > 0)
            {
                return new OperationResult(validationResults);
            }
        }

        if (IsOldDataDetectionEnabled)
        {
            T? databaseDataObject;

            if (dataObject.StringID is not null)
            {
                databaseDataObject = await GetSingleAsync(dataObject.StringID, cancellationToken);
            }
            else
            {
                databaseDataObject = await GetSingleAsync(dataObject.Integer64ID, cancellationToken);
            }

            if (databaseDataObject is null)
            {
#warning Cleanup the message.
                return new OperationResult("Not Found.");
            }

            if (AllowToUpdate(databaseDataObject, dataObject) is false)
            {
#warning Cleanup the message.
                return new OperationResult("Data Conflict");
            }
        }

        dataObject = await Repository.UpdateAsync(dataObject, cancellationToken);

        return new OperationResult(dataObject);
    }

    /// <summary>
    /// The method does data annotation checks on the data object and name uniqueness if enabled.
    /// </summary>
    /// <param name="dataObject">The data object to check.</param>
    /// <param name="cancellationToken">A token used for task cancellations.</param>
    /// <exception cref="ArgumentNullException">Thrown if the dataObject parameter is null.</exception>
    protected async virtual Task<List<ValidationResult>> ValidateAsync(T dataObject, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(dataObject);
        List<ValidationResult> validationResults = dataObject.Validate();

        if (IsUniqueNameRequired && await Repository.NameExistsAsync(dataObject, cancellationToken) is true)
        {
            validationResults.Add(new ValidationResult($"The {dataObject.Name} name already exists in the repository.", [nameof(dataObject.Name)]));
        }

        return await Task.FromResult(validationResults);
    }
}
