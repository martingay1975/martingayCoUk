using Newtonsoft.Json;
using RestSharp;
using Strava.NET.Api;
using Strava.NET.Client;
using Strava.NET.main.CsharpDotNet2.Strava.NET.Model;
using System;
using System.Diagnostics;
using System.IO;

namespace HeatmapData
{
    public static class MartinsRoutes
    {
        public static void Start(string authorizationCode)
        {
            var client = new RestClient($"https://www.strava.com/oauth/token?client_id=9912&client_secret=64dc88eaf43bfa3d3b0f4f624e5b7aeefd1059c6&code={authorizationCode}&grant_type=authorization_code");
            client.Timeout = -1;

            var request = new RestRequest(Method.POST);

            var response = client.Execute(request);
            var accessTokenResponse = JsonConvert.DeserializeObject<AccessTokenResponse>(response.Content);

            GetRoutes(accessTokenResponse.AccessToken, @"c:\temp\strava");
        }
        private static void GetRoutes(string accessToken, string folderPath)
        {
            Debug.WriteLine($"Access Token: {accessToken}, folderPath: {folderPath}");

            Configuration.AccessToken = accessToken;
            var activitiesApi = new ActivitiesApi();
            var streamsApi = new StreamsApi();
            var gpxFileSystem = new GpxFileSystem(folderPath);
           
            // Get all the activities for the logged in user
            var activities = activitiesApi.GetAllLoggedInAthleteActivities();
            var polyLineCount = 0;

            for (var activityIndex=0; activityIndex < activities.Count; activityIndex ++)
            {
                var activity = activities[activityIndex];
                try
                {
                    var fileExists = File.Exists(gpxFileSystem.GetActivityFilePath(activity));
                    Debug.WriteLine($"       {activity.Id} - {activity.Name}. ({activityIndex + 1} of {activities.Count}) - {(fileExists? "EXISTS" : "FETCH")}");

                    if (fileExists)
                    {
                        gpxFileSystem.Add(activity.Type, activity.Id.Value);
                        continue;
                    }

                    //var latlng = streamsApi.GetActivityStreams(activity.Id, new List<string> { "latlng" }, true);

                    if (!activity.StartDate.HasValue)
                    {
                        throw new Exception("Start Date not set on activity");
                    }

                    var heatMapJson = new HeatMapJson()
                    {
                        ActivityType = activity.Type,
                        Polyline = activity.Map.Polyline ?? activity.Map.SummaryPolyline,
                        Name = activity.Name,
                        StartDateTime = activity.StartDate
                    };

                    if (activity.Map.Polyline != null)
                    {
                        polyLineCount++;
                    }

                    var json = JsonConvert.SerializeObject(heatMapJson);
                    
                    var activityJsonPath = gpxFileSystem.GetActivityFilePath(activity);
                    File.WriteAllText(activityJsonPath, json);

                    gpxFileSystem.Add(activity.Type, activity.Id.Value);
                }
                catch (Exception e)
                {
                    Debug.WriteLine($"ERROR: {activity.Id} - {activity.Name}. {e}");
                }
            }

            Debug.WriteLine("Polyline: " + polyLineCount);

            var jsonFS = JsonConvert.SerializeObject(gpxFileSystem.FileSystem, Formatting.Indented);
            File.WriteAllText(gpxFileSystem.GetFileSystemPath(), jsonFS);
        }
    }
}
