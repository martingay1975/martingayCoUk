using System;
using System.IO;

namespace WebDataEntry.Web.Application
{
	public class GoogleDriveDevEnvironment
	{
		private readonly IConfiguration configuration;
		
		public GoogleDriveDevEnvironment(IConfiguration configuration)
		{
			this.configuration = configuration;
		}

		public void Copy(string relativePath)
		{
			relativePath = relativePath.Replace("/", @"\");
			var sourcePath = Path.Combine(this.configuration.BasePath, relativePath);
			var destinationPath = Path.Combine(this.BasePath, relativePath);
			File.Copy(sourcePath, destinationPath, true);
		}

		private string BasePath
		{
            // Dirty hack. But gets around the different ways that the identity of the worker process runs under.
			get
			{
                string username = string.Empty;
				switch (Environment.MachineName)
				{
					case "SLOPPC":
						username = "daddy";
						break;
					case "SLOP-WIN10":
						username = "Slop";
						break;
					case "DP-DEV-MG":
						username = "mgay";
						break;
					default:
						throw new NotSupportedException("What is the user on this machine?");
				}

				return string.Format(@"C:\Users\{0}\Google Drive\Work\Code\Web\BlogSite\martingay_co_uk", username);
			}
		}
	}
}