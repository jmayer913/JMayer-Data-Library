using JMayer.Data.Data;
using System.ComponentModel.DataAnnotations;

namespace TestProject.Data;

/// <summary>
/// The represents a very simple data object to be used for testing.
/// </summary>
internal class SimpleDataObject : DataObject
{
    /// <inheritdoc/>
    /// <remarks>Overridden to enforce required data annotation.</remarks>
    [Required]
    public override string? Name { get => base.Name; set => base.Name = value; }

    /// <summary>
    /// The property gets/sets a value associated with the simple data object.
    /// </summary>
    [Range(0, 100)]
    public int Value { get; set; }

    /// <inheritdoc/>
    public SimpleDataObject() : base() { }

    /// <inheritdoc/>
    public SimpleDataObject(SimpleDataObject copy) : base(copy) { }

    /// <inheritdoc/>
    public override void MapProperties(DataObject dataObject)
    {
        base.MapProperties(dataObject);

        if (dataObject is SimpleDataObject simpleDataObject)
        {
            Value = simpleDataObject.Value;
        }
    }
}
