using System.Runtime.Serialization;

namespace Cornball.Web.Services
{
    [DataContract(Name = "DataItem", Namespace = "http://lantisen.stodell.se/")]
    public class DataItem
    {
        public DataItem(string name, int value)
        {
            Name = name;
            Value = value;
        }

        [DataMember]
        public string Name { get; private set; }

        [DataMember]
        public int Value { get; private set; }
    }
}