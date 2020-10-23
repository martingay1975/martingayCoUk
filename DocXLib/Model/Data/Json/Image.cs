
using System.Runtime.Serialization;

namespace DocXLib.Model.Data.Json
{
    [DataContract]
    public class Image
    {
        [DataMember(Name = "src")]
        public string Source { get; set; }

        [DataMember(Name = "description")]
        public string Description { get; set; }
    }
}
