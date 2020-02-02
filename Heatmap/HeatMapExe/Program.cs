using HeatmapData;

namespace HeatMapExe
{
    class Program
    {
        // 1) Run this in the client
        // http://www.strava.com/oauth/authorize?client_id=9912&response_type=code&redirect_uri=http://localhost/exchange_token&approval_prompt=auto&scope=profile:read_all,profile:write,activity:write,activity:read_all

        // 2) Click Authorize, then from the result, grab the code= part of the array.
        const string AuthorizationCode = "e25cf06c4da714a34493db6447922f25a30c4dbc";

        // 3) Once run, goto C:\temp\strava. Upload the latest file timestamps via FileZilla. Remember to do the FileSystem.json in the root as well.

        static void Main(string[] args)
        {
            MartinsRoutes.Start(AuthorizationCode);
        }
    }
}
