using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Strava.NET.Model;
using System;
using System.Runtime.Serialization;

namespace HeatmapData
{
    [DataContract]
    public class HeatMapJson
    {
        [DataMember]
        public string Polyline { get; set; }

        //public LatLngStream Latlng { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        [JsonConverter(typeof(StringEnumConverter))]
        public ActivityType ActivityType { get; set; }

        [DataMember]
        public DateTime? StartDateTime { get; set; }
    }
}
