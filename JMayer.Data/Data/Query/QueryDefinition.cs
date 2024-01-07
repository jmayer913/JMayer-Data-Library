namespace JMayer.Data.Data.Query;

/// <summary>
/// The class represents the query definition to be used when querying data.
/// </summary>
public class QueryDefinition
{
    /// <summary>
    /// The property gets/sets the filter definitions for the query.
    /// </summary>
    public List<FilterDefinition> FilterDefinitions { get; set; } = [];

    /// <summary>
    /// The property gets/sets how many sets of records should be skipped.
    /// </summary>
    public int Skip { get; set; }
    
    /// <summary>
    /// The property gets/sets the sort definitions for the query.
    /// </summary>
    public List<SortDefinition> SortDefinitions { get; set; } = [];

    /// <summary>
    /// The propery gets/sets how many records to take.
    /// </summary>
    public int Take { get; set; } = TakeAll;

    /// <summary>
    /// The constant for all records to be taken.
    /// </summary>
    public const int TakeAll = -1;
}
