using System;

namespace HeatMapExe
{
    class Program
    {
        static void Main(string[] args)
        {
            var a = new HeatmapData.MartinsRoutes();
            a.GetRoutes("5d9ea0425d96c12495d43d7669cdf5a84dff8c73", @"c:\temp\strava");
        }
    }
}
