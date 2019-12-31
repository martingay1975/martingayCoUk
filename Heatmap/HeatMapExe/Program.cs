using Newtonsoft.Json;
using RestSharp;
using System;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace HeatMapExe
{
    class Program
    {
        // Click Authorize, then from the result, grab the code= part of the array.
        const string AuthorizationCode = "9ed4daef55ce5f4a8c1db502a7ddef653ff76460";

        static void Main(string[] args)
        {
            // Run this in the client
            // http://www.strava.com/oauth/authorize?client_id=9912&response_type=code&redirect_uri=http://localhost/exchange_token&approval_prompt=auto&scope=profile:read_all,profile:write,activity:write,activity:read_all

            //var initialRequestUrl = "http://www.strava.com/oauth/authorize?client_id=9912&response_type=code&redirect_uri=http://localhost/exchange_token&approval_prompt=auto&scope=profile:read_all,profile:write,activity:write,activity:read_all";

            //var process = new Process();
            //process.StartInfo.WorkingDirectory = @"C:\Program Files(x86)\Google\Chrome\Application";
            //process.StartInfo.Arguments = initialRequestUrl;
            //process.StartInfo.FileName = "chrome.exe";
            //process.Start();
        }
    }
}
