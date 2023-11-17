namespace JMayer.Data.Data
{
    /// <summary>
    /// The class represents generic data in the database.
    /// </summary>
    public class DataObject
    {
        /// <summary>
        /// The property gets/sets the key for the data object.
        /// </summary>
        public string? Key { get; set; }

        /// <summary>
        /// The property gets the key for the data object as an 32-bit integer.
        /// </summary>
        /// <remarks>
        /// If the key cannot be converted into an integer, 0 is returned.
        /// </remarks>
        public int Key32
        {
            get
            {
                _ = int.TryParse(Key, out int value);
                return value;
            }
        }

        /// <summary>
        /// The property gets the key for the data object as an 64-bit integer.
        /// </summary>
        /// <remarks>
        /// If the key cannot be converted into an integer, 0 is returned.
        /// </remarks>
        public long Key64
        {
            get 
            {
                _ = long.TryParse(Key, out long value);
                return value;
            }
        }

        /// <summary>
        /// The default constructor.
        /// </summary>
        public DataObject() { }

        /// <summary>
        /// The copy constructor.
        /// </summary>
        /// <param name="copy">The copy.</param>
        public DataObject(DataObject copy) => MapProperties(copy);

        /// <summary>
        /// The method maps a DataObject to this object.
        /// </summary>
        /// <param name="dataObject">The data object.</param>
        public virtual void MapProperties(DataObject dataObject)
        {
            ArgumentNullException.ThrowIfNull(dataObject);
            Key = dataObject.Key;
        }
    }
}
