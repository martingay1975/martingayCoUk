using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Strava.NET.main.CsharpDotNet2.Strava.NET.Model
{
    [DataContract]
    public class AccessTokenResponse
    {
        [DataMember]
        [JsonProperty(PropertyName = "access_token")]
        public string AccessToken { get; set; }
    }
}
