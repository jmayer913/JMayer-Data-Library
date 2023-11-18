namespace JMayer.Data.Data
{
    /// <summary>
    /// The class represents simple name data to be listed in UI.
    /// </summary>
    public class ListView
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
        /// The property gets/sets the name of the configuration.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// The default constructor.
        /// </summary>
        public ListView() { }

        /// <summary>
        /// A copy constructor.
        /// </summary>
        /// <param name="copy">The copy.</param>
        public ListView(ListView copy)
        {
            ArgumentNullException.ThrowIfNull(copy);
            Key = copy.Key;
            Name = copy.Name;
        }

        /// <summary>
        /// A copy constructor.
        /// </summary>
        /// <param name="copy">The copy.</param>
        public ListView(ConfigurationDataObject copy)
        {
            ArgumentNullException.ThrowIfNull(copy);
            Key = copy.Key;
            Name = copy.Name;
        }
    }
}
