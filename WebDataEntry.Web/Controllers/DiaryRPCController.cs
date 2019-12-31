using HeatmapData;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using WebDataEntry.Web.Application;
using WebDataEntry.Web.Models;
using WebDataEntry.Web.Models.Data;

namespace WebDataEntry.Web.Controllers
{
    [RoutePrefix(@"api/strava")]
    public class StravaController : ApiController
    {
        [Route("{authorizationCode}")]
        [HttpGet]
        public void Get([FromUri] string authorizationCode)
        {
            MartinsRoutes.Start(authorizationCode);
        }
    }


    [RoutePrefix(@"api/rpc")]
	public class DiaryRpcController : ApiController
	{
		private readonly IDiaryRepository _diaryRepository;
		private readonly IConfiguration _configuration;

		public DiaryRpcController()
		{
			_diaryRepository = new DiaryRepositoryFactory().Create();
			_configuration = new ConfigurationFactory().Create();
		}

		[Route("DownloadDatabase")]
		[HttpGet]
		public void DownloadDatabase()
		{
			_diaryRepository.DownloadDatabase();
			new DiaryRepositoryFactory().Reset();
		}

		[Route("UploadDatabase")]
		[HttpGet]
		public HttpResponseMessage UploadDatabase()
		{
			try
			{
				_diaryRepository.Save();
				_diaryRepository.UploadDatabase();
				return this.Request.CreateResponse(HttpStatusCode.OK);
			}
			catch (Exception e)
			{
				return this.Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
			}
		}

		[Route("SaveFormats")]
		[HttpPost]
		public HttpResponseMessage SaveFormats([FromBody] SaveRequest saveRequest)
		{
			try
			{
				_diaryRepository.SaveFormats(saveRequest);
				return this.Request.CreateResponse(HttpStatusCode.OK);
			}
			catch (Exception e)
			{
				return this.Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
			}
		}

		[Route("TestValidDiary")]
		[HttpGet]
		public void TestValidDiary()
		{
			_diaryRepository.TestValidDiary();
		}

		[Route("Save")]
		[HttpGet]
		public void Save()
		{
			_diaryRepository.Save();
		}

		[Route("GetPeople")]
		[HttpGet]
		public IEnumerable<Person> GetPeople()
		{
			return _diaryRepository.GetPeople();
		}

		[Route("GetLocations")]
		[HttpGet]
		public IEnumerable<Location> GetLocations()
		{
			return _diaryRepository.GetLocations();
		}

		[HttpPost]
		[Route("UploadImages")]
		public async Task<List<string>> UploadImages()
		{
			if (Request.Content.IsMimeMultipartContent())
			{
				ImageUploadStreamProvider streamProvider = GetStreamProvider();

				// read all the data before continuing
				await Request.Content.ReadAsMultipartAsync(streamProvider);

				foreach (var relativeFilePath in streamProvider.RelativeFilePaths)
				{
					FtpUploadAsync(relativeFilePath);
				}
                
				return streamProvider.RelativeFilePaths;
			}

			throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotAcceptable, "Invalid Request!"));
			
		}

		private ImageUploadStreamProvider GetStreamProvider()
		{
			var directoryInfo = new DirectoryInfo(Path.Combine(_configuration.BasePath, "temp"));
			if (!directoryInfo.Exists)
			{
				directoryInfo.Create();
			}

			return new ImageUploadStreamProvider(directoryInfo.FullName);
		}

		private void FtpUploadAsync(string relativeSource)
		{
			using (var sftpBatch = new SFtpBatch(_configuration))
			{
				sftpBatch.Upload(relativeSource);
			}
		}
	}
}
