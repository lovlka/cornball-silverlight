using System.Runtime.Serialization;
using Newtonsoft.Json;

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
        [JsonProperty("name")]
        public string Name { get; private set; }

        [DataMember]
        [JsonProperty("value")]
        public int Value { get; private set; }
    }
}