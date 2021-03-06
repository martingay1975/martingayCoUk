﻿// Notify's that Owin's startup class.

using Microsoft.Owin;
using Microsoft.Owin.Extensions;
using Microsoft.Owin.FileSystems;
using Microsoft.Owin.StaticFiles;
using Owin;
using WebDataEntry.Web;

[assembly: OwinStartup("Production", typeof(WebDataEntry.Owin.StartUp.WebEntryStartUp))]
namespace WebDataEntry.Owin.StartUp
{
	static class WebEntryStartUp
	{
		/// <summary>
		/// Entry point from framework. Here we start the ball rolling in terms of configuring Owin Middleware.
		/// </summary>
		public static void Configuration(IAppBuilder app)
		{
			var physicalFileSystem = new PhysicalFileSystem(@"..\..\..\WebDataEntry.Web");

			var options = new FileServerOptions
									{
										EnableDefaultFiles = true,
										FileSystem = physicalFileSystem,
									};
			options.StaticFileOptions.FileSystem = physicalFileSystem;
			options.StaticFileOptions.ServeUnknownFileTypes = true;
			options.DefaultFilesOptions.DefaultFileNames = new[] { "default.html" };
			app.UseFileServer(options);

			app.UseStageMarker(PipelineStage.MapHandler);

			var httpConfiguration = MvcWebApiConfiguration.Configure();
			app.UseWebApi(httpConfiguration);
		}
	}
}