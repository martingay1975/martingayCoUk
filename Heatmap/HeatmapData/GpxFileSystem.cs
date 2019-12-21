using Strava.NET.Model;
using System;
using System.Collections.Generic;

namespace HeatmapData
{
    public class GpxFileSystem
    {
        public Dictionary<string, List<long>> FileSystem { get; }

        public GpxFileSystem()
        {
            FileSystem = new Dictionary<string, List<long>>();
            foreach (var activityType in Enum.GetNames(typeof (ActivityType)))
            {
                FileSystem.Add(activityType, new List<long>());
            }
        }

        public void Add(ActivityType activityType, long value)
        {
            FileSystem[activityType.ToString()].Add(value);
        }
    }
}
