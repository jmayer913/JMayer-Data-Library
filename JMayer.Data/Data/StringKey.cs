//Obvious this needs to inherit from a base class so it can be plug and play.
//The bigger question is how do I make this work in searches.

namespace JMayer.Data.Data
{
    public class StringKey
    {
        public string? Key { get; set; }

        public StringKey() { }

        public StringKey(string? key)
        {
            Key = key;
        }


    }
}
