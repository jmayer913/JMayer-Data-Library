using JMayer.Data.Database.DataLayer.MemoryStorage;
using TestProject.Data;

namespace TestProject.Database;

/// <summary>
/// The class manages CRUD interactions with a list memory storage for the simple configuration data object.
/// </summary>
internal class SimpleConfigurationListDataLayer : ConfigurationListDataLayer<SimpleConfigurationDataObject>
{
}
