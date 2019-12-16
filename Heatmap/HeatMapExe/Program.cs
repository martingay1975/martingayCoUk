using System;

namespace HeatMapExe
{
    class Program
    {
        static void Main(string[] args)
        {
            var a = new HeatmapData.MartinsRoutes();
            a.GetRoutes("afe3070b2b9fd65b7a0fa48b1bde5fd1a9b51f82", @"c:\temp\strava");
        }
    }
}
