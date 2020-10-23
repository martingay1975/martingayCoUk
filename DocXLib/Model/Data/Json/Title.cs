using System.Runtime.Serialization;

namespace DocXLib.Model.Data.Json
{
    [DataContract]
    public class Title
    {
        [DataMember(Name = "value", IsRequired = true)]
        public string Value { get; set; }

        [DataMember(Name = "name", EmitDefaultValue = false)]
        public string Name { get; set; }
    }

}