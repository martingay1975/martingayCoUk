using Newtonsoft.Json;
using RestSharp;
using System;
using System.Runtime.Serialization;

namespace HeatMapExe
{
    class Program
    {
        // Click Authorize, then from the result, grab the code= part of the array.
        const string AuthorizationCode = "85afe5302c182180fb3c925a7799552bb4839096";

        static void Main(string[] args)
        {
            // Run this in the client
            // http://www.strava.com/oauth/authorize?client_id=9912&response_type=code&redirect_uri=http://localhost/exchange_token&approval_prompt=force&scope=profile:read_all,profile:write,activity:write,activity:read_all

            var client = new RestClient($"https://www.strava.com/oauth/token?client_id=9912&client_secret=64dc88eaf43bfa3d3b0f4f624e5b7aeefd1059c6&code={AuthorizationCode}&grant_type=authorization_code");
            client.Timeout = -1;

            var request = new RestRequest(Method.POST);

            var response = client.Execute(request);
            AccessTokenResponse accessTokenResponse = JsonConvert.DeserializeObject<AccessTokenResponse>(response.Content);


            var martinsRoutes = new HeatmapData.MartinsRoutes();
            martinsRoutes.GetRoutes(accessTokenResponse.AccessToken, @"c:\temp\strava");
        }

        [DataContract]
        public class AccessTokenResponse
        {
            [DataMember]
            [JsonProperty(PropertyName = "access_token")]
            public string AccessToken { get; set; }
        }
    }
}
