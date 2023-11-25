using System.ComponentModel.DataAnnotations;

namespace JMayer.Data.Data
{
    /// <summary>
    /// The class represents user configuration data in the database.
    /// </summary>
    public class ConfigurationDataObject : DataObject
    {
        /// <summary>
        /// The property gets/sets when the configuration was created.
        /// </summary>
        public DateTime CreatedOn { get; set; }

        /// <summary>
        /// The property gets/sets the description of the configuration.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// The property gets/sets who last edited the configuration.
        /// </summary>
        public string? LastEditedBy { get; set; }

        /// <summary>
        /// The property gets/sets the ID for who last edited the configuration.
        /// </summary>
        public string? LastEditedByID { get; set; }

        /// <summary>
        /// The property gets/sets the last time the configuration was edited.
        /// </summary>
        public DateTime? LastEditedOn { get; set; }

        /// <summary>
        /// The property gets/sets the name of the configuration.
        /// </summary>
        [Required]
        public string? Name { get; set; }

        /// <summary>
        /// The default constructor.
        /// </summary>
        public ConfigurationDataObject() { }

        /// <summary>
        /// The copy constructor.
        /// </summary>
        /// <param name="copy">The copy.</param>
        public ConfigurationDataObject(ConfigurationDataObject copy) => MapProperties(copy);

        /// <summary>
        /// The method maps a ConfigurationDataObject to this object.
        /// </summary>
        /// <param name="dataObject">The data object.</param>
        public override void MapProperties(DataObject dataObject)
        {
            base.MapProperties(dataObject);

            if (dataObject is ConfigurationDataObject configurationDataObject)
            {
                CreatedOn = configurationDataObject.CreatedOn;
                Description = configurationDataObject.Description;
                LastEditedBy = configurationDataObject.LastEditedBy;
                LastEditedByID = configurationDataObject.LastEditedByID;
                LastEditedOn = configurationDataObject.LastEditedOn;
                Name = configurationDataObject.Name;
            }
        }
    }
}
