
using System.Runtime.Serialization;

namespace DiaryDatabase.Model.Data.Json
{
    [DataContract]
    public class Info 
    {
        [DataMember(Name="content", IsRequired = true)]
        public string Content { get; set; }

		[DataMember(Name="images")]
		public bool Images { get; set; }
    }
}