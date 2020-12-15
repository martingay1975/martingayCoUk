using HeatmapData;
using System;
using System.Threading.Tasks;

namespace HeatMapExe
{
    class Program
    {
        // 1) Run this in the client
        // http://www.strava.com/oauth/authorize?client_id=9912&response_type=code&redirect_uri=http://localhost/exchange_token&approval_prompt=auto&scope=profile:read_all,profile:write,activity:write,activity:read_all

        // 2) Click Authorize, then from the result, grab the code= part of the array.
        //const string AuthorizationCode = "47cc32bb0f41420aad43e96d93ce8cfcf5645ec2";

        static async Task Main(string[] args)
        {
            try
            {
                var webDriver = new WebDriver();
                webDriver.AutorizationCodeObtained += WebDriver_AutorizationCodeObtained;
                await webDriver.StartAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            Console.ReadKey();
        }

        public static void WebDriver_AutorizationCodeObtained(object sender, AutorizationCodeObtainedEventArgs e)
        {
            MartinsRoutes.Start(e.Code);
        }
    }
}
