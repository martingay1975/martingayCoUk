using System.Text;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Strava.NET.Model {

  /// <summary>
  /// An enumeration of the types an activity may have.
  /// </summary>
  [DataContract]
  public class ActivityType1 {

    /// <summary>
    /// Get the string presentation of the object
    /// </summary>
    /// <returns>String presentation of the object</returns>
    public override string ToString()  {
      var sb = new StringBuilder();
      sb.Append("class ActivityType {\n");
      sb.Append("}\n");
      return sb.ToString();
    }

    /// <summary>
    /// Get the JSON string presentation of the object
    /// </summary>
    /// <returns>JSON string presentation of the object</returns>
    public string ToJson() {
      return JsonConvert.SerializeObject(this, Formatting.Indented);
    }

}
    [DataContract]
    public enum ActivityType
    {
        Run,
        Ride,
        Swim,
        Kayaking,
        Walk,
        Hike,
        VirtualRide,
        Workout,
        Surfing,
        Snowboard
    }
}