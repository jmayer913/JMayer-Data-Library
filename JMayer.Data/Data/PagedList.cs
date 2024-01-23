namespace JMayer.Data.Data;

/// <summary>
/// The class represents a paged list of data objects.
/// </summary>
/// <typeparam name="T">Any type of object can be stored in the paged list.</typeparam>
public class PagedList<T> 
    where T : class
{
    /// <summary>
    /// The property gets/sets the data objects for the page.
    /// </summary>
    public List<T> DataObjects { get; set; } = [];

    /// <summary>
    /// The property gets/sets the total records outside the paging.
    /// </summary>
    public int TotalRecords { get; set; }
}
