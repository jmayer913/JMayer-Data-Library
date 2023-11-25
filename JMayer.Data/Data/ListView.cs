namespace JMayer.Data.Data
{
    /// <summary>
    /// The class represents simple name & ID data to be listed in UI.
    /// </summary>
    public sealed class ListView
    {
        /// <summary>
        /// The property gets/sets the ID for the data object as an 32-bit integer.
        /// </summary>
        /// <remarks>
        /// This wraps around the Integer64ID property.
        /// </remarks>
        public int? Integer32ID
        {
            get => (int?)Integer64ID;
            set => Integer64ID = value;
        }

        /// <summary>
        /// The property gets/sets the ID for the data object as an 64-bit integer.
        /// </summary>
        public long? Integer64ID { get; set; }

        /// <summary>
        /// The property gets/sets the ID for the data object as a string.
        /// </summary>
        public string? StringID { get; set; }

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
            Integer64ID = copy.Integer64ID;
            StringID = copy.StringID;
            Name = copy.Name;
        }

        /// <summary>
        /// A copy constructor.
        /// </summary>
        /// <param name="copy">The copy.</param>
        public ListView(ConfigurationDataObject copy)
        {
            ArgumentNullException.ThrowIfNull(copy);
            Integer64ID = copy.Integer64ID;
            StringID = copy.StringID;
            Name = copy.Name;
        }
    }
}
