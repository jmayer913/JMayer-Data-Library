using System.Web;

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

    /// <summary>
    /// The method returns the object formatted for a query string.
    /// </summary>
    /// <returns>The object as a query string.</returns>
    public string ToQueryString()
    {
        string filterDefinitionQueryString = string.Empty;

        for (int index = 0; index < FilterDefinitions.Count; index++)
        {
            FilterDefinition filterDefinition = FilterDefinitions[index];
            filterDefinitionQueryString += $"&{nameof(FilterDefinitions)}[{index}].{nameof(FilterDefinition.FilterOn)}={HttpUtility.UrlEncode(filterDefinition.FilterOn)}&{nameof(FilterDefinitions)}[{index}].{nameof(FilterDefinition.Operator)}={HttpUtility.UrlEncode(filterDefinition.Operator)}&{nameof(FilterDefinitions)}[{index}].{nameof(FilterDefinition.Value)}={HttpUtility.UrlEncode(filterDefinition.Value)}";
        }

        string sortDefinitionQueryString = string.Empty;

        for (int index = 0; index < SortDefinitions.Count; index++)
        {
            SortDefinition sortDefinition = SortDefinitions[index];
            sortDefinitionQueryString += $"&{nameof(SortDefinitions)}[{index}].{nameof(SortDefinition.Descending)}={sortDefinition.Descending}&{nameof(SortDefinitions)}[{index}].{nameof(SortDefinition.SortOn)}={HttpUtility.UrlEncode(sortDefinition.SortOn)}";
        }

        return $"{nameof(Skip)}={Skip}&{nameof(Take)}={Take}{filterDefinitionQueryString}{sortDefinitionQueryString}";
    }
}
