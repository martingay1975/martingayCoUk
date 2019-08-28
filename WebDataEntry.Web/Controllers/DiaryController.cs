using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Routing;
using WebDataEntry.Web.Application;
using WebDataEntry.Web.Models;


namespace WebDataEntry.Web.Controllers
{
	[RoutePrefix(@"api/diary")]
	public class DiaryController : ApiController
	{
		private readonly IDiaryRepository _diaryRepository;

		public DiaryController()
		{
			_diaryRepository = new DiaryRepositoryFactory().Create();
		}

		[HttpGet]
		[Route("", Name="GetDiary")]
		public IEnumerable<Entry> GetEntries([FromUri] DateEntry dateEntry)
		{
			var entries = _diaryRepository.GetEntries(dateEntry);
			if (entries.Count == 0)
			{
				// may be the case we want to add the first entry of the year. Maybe there are validly no entries in the year, therefore go back one year.
				dateEntry.Year = -1;
				entries = _diaryRepository.GetEntries(dateEntry);
				if (entries.Count == 0)
				{
					throw new HttpResponseException(HttpStatusCode.NotFound);
				}
			}

			return entries;
		}

		[HttpDelete]
		[Route("")]
		public HttpResponseMessage DeleteEntries([FromUri] DateEntry dateEntry)
		{
			_diaryRepository.DeleteEntries(dateEntry);
			return new HttpResponseMessage(HttpStatusCode.NoContent);
		}

		[HttpPost]
		[ValidationFilter]
		[Route("")]
		public HttpResponseMessage CreateEntry([FromBody] Entry entry)
		{
			// create the entry in the backed.
			_diaryRepository.Create(entry);

			// pass back the uri to the newly create function.
			var response = Request.CreateResponse<Entry>(HttpStatusCode.Created, entry);
			var uri = Url.Link("GetDiary", entry.DateEntry);
			response.Headers.Location = new Uri(uri);
			return response;
		}

		[HttpPut]
		[ValidationFilter]
		[Route("")]
		public void UpdateEntry([FromBody] UpdateEntryRequest updateEntryRequest)
		{
			try
			{
				_diaryRepository.Update(updateEntryRequest);
			}
			catch (InvalidOperationException)
			{
				throw new HttpResponseException(HttpStatusCode.NotFound);
			}
		}
	}
}


