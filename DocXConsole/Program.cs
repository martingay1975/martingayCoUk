using System;
using DocXLib;

namespace DocXConsole
{
    static class Program
    {
        static void Main(string[] args)
        {
            try
            {
                if (args.Length > 0)
                {
                    if (args[0] == "runall")
                    {
                        for (var idx = 0; idx < 6; idx++)
                        {
                            var process = new System.Diagnostics.Process
                            {
                                StartInfo = new System.Diagnostics.ProcessStartInfo(
                                    @"C:\git\martingayCoUk\DocXConsole\bin\Debug\DocXConsole.exe", idx.ToString())
                            };
                            process.Start();
                        }

                        var waitTime = TimeSpan.FromSeconds(80);
                        Console.WriteLine($"Waiting {waitTime.TotalSeconds} seconds");
                        System.Threading.Thread.Sleep((int)waitTime.TotalMilliseconds);

                        for (var idx = 6; idx < DocumentSlices.DocumentList.Count; idx++)
                        {
                            var process = new System.Diagnostics.Process
                            {
                                StartInfo = new System.Diagnostics.ProcessStartInfo(
                                    @"C:\git\martingayCoUk\DocXConsole\bin\Debug\DocXConsole.exe", idx.ToString())
                            };
                            process.Start();
                        }
                    }
                    else if (args[0] == "book1")
                    {
                        for (var idx = 0; idx < 6; idx++)
                        {
                            var process = new System.Diagnostics.Process
                            {
                                StartInfo = new System.Diagnostics.ProcessStartInfo(
                                    @"C:\git\martingayCoUk\DocXConsole\bin\Debug\DocXConsole.exe", idx.ToString())
                            };
                            process.Start();
                        }
                    }
                    else if (args[0] == "book2")
                    {
                        for (var idx = 6; idx < 14; idx++)
                        {
                            var process = new System.Diagnostics.Process
                            {
                                StartInfo = new System.Diagnostics.ProcessStartInfo(
                                    @"C:\git\martingayCoUk\DocXConsole\bin\Debug\DocXConsole.exe", idx.ToString())
                            };
                            process.Start();
                        }
                    }
                    else
                    {
                        Start.Run(int.Parse(args[0]));
                    }
                }
                else
                {
                    Start.Run();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Ran with: '{args[0]}'");
                Console.WriteLine(e.Message);
                Console.ReadKey();
            }
            finally
            {
                Console.WriteLine($"Finished '{args[0]}'. Press any key to exit");
                //Console.ReadKey();
            }
        }
    }
}
