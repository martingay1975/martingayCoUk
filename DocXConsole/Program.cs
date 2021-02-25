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
                        for (var idx=0; idx < 6; idx ++)
                        {

                            var process = new System.Diagnostics.Process();
                            process.StartInfo = new System.Diagnostics.ProcessStartInfo(@"C:\git\martingayCoUk\DocXConsole\bin\Debug\DocXConsole.exe", idx.ToString());
                            process.Start();
                        }

                        Console.WriteLine("Waiting 1 minute...");
                        System.Threading.Thread.Sleep((int)TimeSpan.FromMinutes(2).TotalMilliseconds);

                        for (var idx = 6; idx < DocumentSlices.DocumentList.Count; idx++)
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
                Console.WriteLine(args[0]);
                Console.WriteLine(e.Message);
                Console.ReadKey();
            }
            finally
            {
                Console.WriteLine("Finished. Press any key to exit");
                //Console.ReadKey();
            }
        }
    }
}
