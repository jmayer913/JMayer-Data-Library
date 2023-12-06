using JMayer.Data.Data;
using System.ComponentModel.DataAnnotations;

namespace TestProject.Data;

/// <summary>
/// The represents a very simple configuration data object to be used for testing.
/// </summary>
public class SimpleConfigurationDataObject : ConfigurationDataObject
{
    /// <summary>
    /// The property gets/sets a value associated with the simple data object.
    /// </summary>
    [Range(0, 100)]
    public int Value { get; set; }

    /// <summary>
    /// The default constructor.
    /// </summary>
    public SimpleConfigurationDataObject() { }

    /// <summary>
    /// The copy constructor.
    /// </summary>
    /// <param name="copy">The copy.</param>
    public SimpleConfigurationDataObject(SimpleConfigurationDataObject copy) => MapProperties(copy);

    /// <summary>
    /// The method maps a SimpleDataObject to this object.
    /// </summary>
    /// <param name="dataObject">The data object.</param>
    public override void MapProperties(DataObject dataObject)
    {
        base.MapProperties(dataObject);

        if (dataObject is SimpleConfigurationDataObject simpleConfigurationDataObject)
        {
            Value = simpleConfigurationDataObject.Value;
        }
    }
}
