using Newtonsoft.Json;
using Strava.NET.Api;
using Strava.NET.Client;
using Strava.NET.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace HeatmapData
{

    public class MartinsRoutes
    {
        public void GetRoutes(string accessToken, string folderPath)
        {
            Configuration.AccessToken = accessToken;
            var activitiesApi = new ActivitiesApi();
            var streamsApi = new StreamsApi();
            var gpxFileSystem = new GpxFileSystem();

            var files = Directory.GetFiles(folderPath);
            var exists = files.Select(Path.GetFileNameWithoutExtension).ToList();
            
            var activities = activitiesApi.GetAllLoggedInAthleteActivities();

            var routes = new Dictionary<DateTime?, StreamSet>();
            for (var activityIndex=0; activityIndex < activities.Count; activityIndex ++)
            {
                var activity = activities[activityIndex];
                try
                {
                    Debug.WriteLine($"       {activity.Id} - {activity.Name}. ({activityIndex + 1} of {activities.Count})");

                    if (exists.Contains(activity.Id.ToString()))
                    {
                        continue;
                    }

                    var latlng = streamsApi.GetActivityStreams(activity.Id, new List<string> { "latlng" }, true);

                    if (!activity.StartDate.HasValue)
                    {
                        throw new Exception("Start Date not set on activity");
                    }

                    var heatMapJson = new HeatMapJson()
                    {
                        ActivityType = activity.Type,
                        Latlng = latlng.Latlng,
                        Name = activity.Name,
                        StartDateTime = activity.StartDate
                    };

                    var json = JsonConvert.SerializeObject(heatMapJson, Formatting.Indented);
                    File.WriteAllText(Path.Combine(folderPath, activity.Type.ToString(), $"{activity.Id.Value.ToString()}.json"), json);
                    gpxFileSystem.Add(activity.Type, activity.Id.Value);
                    routes.Add(activity.StartDate, latlng);
                }
                catch (Exception e)
                {
                    Debug.Log($"ERROR: {activity.Id} - {activity.Name}. {e}");
                }
            }

            var jsonFS = JsonConvert.SerializeObject(gpxFileSystem.FileSystem, Formatting.Indented);
            File.WriteAllText(Path.Combine(folderPath, "FileSystem.json"), jsonFS);
        }
    }
}
