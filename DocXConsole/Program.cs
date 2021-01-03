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
                        for (var idx=0; idx < Start.DocumentSlices.Count; idx ++)
                        {

                            var process = new System.Diagnostics.Process();
                            process.StartInfo = new System.Diagnostics.ProcessStartInfo(@"C:\git\martingayCoUk\DocXConsole\bin\Debug\DocXConsole.exe", idx.ToString());
                            process.Start();
                        }

                        return;
                    }
                    Start.Run(int.Parse(args[0]));
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
            }
            finally
            {
                Console.WriteLine("Finished. Press any key to exit");
                Console.ReadKey();
            }
        }
    }
}
