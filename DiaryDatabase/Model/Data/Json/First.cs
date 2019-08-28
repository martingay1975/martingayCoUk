using System.Runtime.Serialization;

namespace DiaryDatabase.Model.Data.Json
{
    [DataContract]
    public class First
    {
        [DataMember(Name="value", IsRequired = true)]
        public string Value { get; set; }

        [DataMember(Name = "name", EmitDefaultValue = false)]
        public string Name { get; set; }
    }
}