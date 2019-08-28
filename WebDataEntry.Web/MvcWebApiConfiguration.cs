using System.Web.Http;
using WebDataEntry.Web.Application;

namespace WebDataEntry.Web
{
	public static class MvcWebApiConfiguration
	{
		public static HttpConfiguration Configure()
		{
			AutoMapperSetup.Setup();

			var webApiConfiguration = new HttpConfiguration();
			webApiConfiguration.MapHttpAttributeRoutes();
			return webApiConfiguration;
		}
	}
}