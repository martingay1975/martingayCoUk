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

            var files = Directory.GetFiles(folderPath);
            var exists = files.Select(Path.GetFileNameWithoutExtension).ToList();
            
            var activities = activitiesApi.GetAllLoggedInAthleteActivities();

            var routes = new Dictionary<DateTime?, StreamSet>();
            for (var activityIndex=0; activityIndex < activities.Count; activityIndex ++)
            {
                var activity = activities[activityIndex];
                Debug.WriteLine($"Activity {activityIndex + 1} of {activities.Count} - {activity.Name}");

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
                File.WriteAllText(Path.Combine(folderPath, activity.Type.ToString(), $"{activity.Id.ToString()}.json"), json);

                routes.Add(activity.StartDate, latlng);
            }
        }
    }
}
