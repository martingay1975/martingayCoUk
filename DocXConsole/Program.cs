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
                            var process = new System.Diagnostics.Process();
                            process.StartInfo = new System.Diagnostics.ProcessStartInfo(@"C:\git\martingayCoUk\DocXConsole\bin\Debug\DocXConsole.exe", idx.ToString());
                            process.Start();
                        }

                        Console.WriteLine("Waiting 2.5 minutes...");
                        System.Threading.Thread.Sleep((int)TimeSpan.FromSeconds(150).TotalMilliseconds);

                        for (var idx = 6; idx < DocumentSlices.DocumentList.Count; idx++)
                        {

                            var process = new System.Diagnostics.Process();
                            process.StartInfo = new System.Diagnostics.ProcessStartInfo(@"C:\git\martingayCoUk\DocXConsole\bin\Debug\DocXConsole.exe", idx.ToString());
                            process.Start();
                        }
                    }
                    else if (args[0] == "book1")
                    {
                        for (var idx = 0; idx < 3; idx++)
                        {
                            var process = new System.Diagnostics.Process();
                            process.StartInfo = new System.Diagnostics.ProcessStartInfo(@"C:\git\martingayCoUk\DocXConsole\bin\Debug\DocXConsole.exe", idx.ToString());
                            process.Start();
                        }
                    }
                    else if (args[0] == "book2")
                    {
                        for (var idx = 3; idx < 8; idx++)
                        {
                            var process = new System.Diagnostics.Process();
                            process.StartInfo = new System.Diagnostics.ProcessStartInfo(@"C:\git\martingayCoUk\DocXConsole\bin\Debug\DocXConsole.exe", idx.ToString());
                            process.Start();
                        }
                    }
                    else if (args[0] == "book3")
                    {
                        for (var idx = 8; idx < 13; idx++)
                        {
                            var process = new System.Diagnostics.Process();
                            process.StartInfo = new System.Diagnostics.ProcessStartInfo(@"C:\git\martingayCoUk\DocXConsole\bin\Debug\DocXConsole.exe", idx.ToString());
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
