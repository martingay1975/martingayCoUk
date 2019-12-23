using Strava.NET.Model;
using System;
using System.Collections.Generic;
using System.IO;

namespace HeatmapData
{
    public class GpxFileSystem
    {

        public Dictionary<string, List<long>> FileSystem { get; }
        string folderPath;

        public GpxFileSystem(string folderPath)
        {
            FileSystem = new Dictionary<string, List<long>>();
            foreach (var activityType in Enum.GetNames(typeof(ActivityType)))
            {
                FileSystem.Add(activityType, new List<long>());
            }

             this.folderPath = folderPath;
        }

        public string GetFileSystemPath()
        {
            return Path.Combine(folderPath, "FileSystem.json");
        }

        public string GetActivityFilePath(SummaryActivity activity)
        {
            return this.GetActivityFilePath(activity.Type, activity.Id.Value.ToString());
        }

        public string GetActivityFilePath(ActivityType activityType, string activityId)
        {
            return Path.Combine(folderPath, activityType.ToString(), $"{activityId}.json");
        }

        public void Add(ActivityType activityType, long value)
        {
            FileSystem[activityType.ToString()].Add(value);
        }
    }
}
