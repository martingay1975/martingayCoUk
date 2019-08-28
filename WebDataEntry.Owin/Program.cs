
using System;
using Microsoft.Owin.Hosting;
using WebDataEntry.Owin.StartUp;

namespace WebDataEntry.Owin
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			// Start OWIN host 

			try
			{
				using ((WebApp.Start(@"http://localhost:53000/", WebEntryStartUp.Configuration)))
				using ((WebApp.Start(@"http://localhost:53001/", MartinGayCoUkStartUp.Configuration)))
				using ((WebApp.Start(@"http://localhost:53002/", FrimleyFlyersStartUp.Configuration)))
				{
					Console.WriteLine("Running on DiaryEntry-53000, MartinGay Co Uk-53001, FrimleyFlyers-53002");
					Console.WriteLine("Press Enter key to quit.");
					Console.ReadLine();
				}
			}
			catch (Exception e)
			{
				Console.WriteLine($"{e.GetType().Name} - {e.Message}");
				Console.WriteLine("Try running netstat -ano to find if the port is already in use.");
				Console.ReadLine();
			}
		}
	}
}