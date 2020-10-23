using System.Runtime.Serialization;
namespace DocXLib.Model.Data.Json
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