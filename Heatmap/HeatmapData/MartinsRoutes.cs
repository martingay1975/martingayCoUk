using Newtonsoft.Json;
using RestSharp;
using Strava.NET.Api;
using Strava.NET.Client;
using Strava.NET.main.CsharpDotNet2.Strava.NET.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using SFtp;

namespace HeatmapData
{
    public static class MartinsRoutes
    {
        public const string BasePath = @"c:\temp\strava";

        public static void Start(string authorizationCode)
        {
            var client = new RestClient($"https://www.strava.com/oauth/token?client_id=9912&client_secret=64dc88eaf43bfa3d3b0f4f624e5b7aeefd1059c6&code={authorizationCode}&grant_type=authorization_code");
            client.Timeout = -1;

            var request = new RestRequest(Method.POST);

            var response = client.Execute(request);
            var accessTokenResponse = JsonConvert.DeserializeObject<AccessTokenResponse>(response.Content);

            GetRoutes(accessTokenResponse.AccessToken, BasePath);
            Console.WriteLine("Uploading");
            UploadJsonToHost();
            Console.WriteLine("Finished");
        }
        private static void GetRoutes(string accessToken, string folderPath)
        {
            Console.WriteLine($"Access Token: {accessToken}, folderPath: {folderPath}");

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
                    Console.WriteLine($"       {activityIndex} {activity.Id} - {activity.Name}. ({activityIndex + 1} of {activities.Count}) - {(fileExists? "EXISTS" : "FETCH")}");

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
                    Console.WriteLine($"ERROR: {activity.Id} - {activity.Name}. {e}");
                }
            }

            Debug.WriteLine("Polyline: " + polyLineCount);

            var jsonFS = JsonConvert.SerializeObject(gpxFileSystem.FileSystem, Formatting.Indented);
            File.WriteAllText(gpxFileSystem.GetFileSystemPath(), jsonFS);
        }

        public static void UploadJsonToHost()
        {
            var martinGayCoUkHost = new MartinGayCoUkHost(BasePath, "/martingay/res/strava/");
            martinGayCoUkHost.Upload("FileSystem.json");

            var rootDirectoryInfo = new DirectoryInfo(BasePath);
            var jsonFiles = rootDirectoryInfo.GetFiles("*.json", SearchOption.AllDirectories);
            var jsonFilesList = new List<FileInfo>(jsonFiles);
            var relativeFilesNames = jsonFilesList.Select(activityFileInfo => martinGayCoUkHost.GetRelativePath(activityFileInfo.FullName));
            martinGayCoUkHost.UploadBatch(relativeFilesNames);
        }
    }
}
