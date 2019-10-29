using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Xml;
using DiaryDatabase.Presenter;
using WebDataEntry.Web.Application;
using WebDataEntry.Web.Models.Data;

namespace WebDataEntry.Web.Models
{
	public class DiaryRepository : IDiaryRepository
	{
		private readonly List<string> relativePaths;
		private readonly IDiarySerialization _diarySerialization;
		private readonly IConfiguration configuration;
		private readonly IDiaryAssembler diaryAssembler;

		public DiaryRepository(IDiarySerialization _diarySerialization, IConfiguration configuration, IDiaryAssembler diaryAssembler)
		{
			this._diarySerialization = _diarySerialization;
			this.configuration = configuration;
			this.diaryAssembler = diaryAssembler;
			this.relativePaths = new List<string> { 
				"res/xml/diary.xml", 
				"res/json/whoops.json",
				"res/json/2019.json",
				"res/json/2018.json",
				"res/json/2017.json",
				"res/json/2016.json",
				"res/json/2015.json",
				"res/json/2014.json",
				"res/json/2013.json",
				"res/json/2012.json",
				"res/json/2011.json",
				"res/json/2010.json",
				"res/json/2009.json",
				"res/json/2008.json",
				"res/json/2007.json",
				"res/json/2006.json",
				"res/json/2005.json",
				"res/json/2004.json",
				"res/json/2003.json",
				"res/json/2002.json",
				"res/json/2001.json",
				"res/json/2000.json",
				"res/json/1999.json",
				"res/json/1998.json",
				"res/json/1997.json",
				"res/json/1996.json",
				"res/json/1995.json",
				"res/json/old.json",
				"res/json/noInfo-all.json",
				"res/json/all.json",
				"res/json/siteOptions.json"};
		}

		public void DownloadDatabase()
		{
			using (var sftpClient = new SFtpBatch(this.configuration))
			{
				this.relativePaths.ForEach(path => sftpClient.Download(path, Path.Combine(this.configuration.BasePath, path)));
			}
				
			// the diary that was loaded is not out of date, therefore reset
			_diary = null;
			GetDiary();
		}

		private bool IsHashChanged(string relativePath)
		{
			var key = relativePath.Split('/').Last();

			if (this._savedHashes == null)
			{
				return false;
			}

			if (this._loadedHashes.ContainsKey(key) && this._savedHashes.ContainsKey(key))
			{
				return this._loadedHashes[key] != this._savedHashes[key];
			}

			return true;
		}

		public void UploadDatabase()
		{
			try
			{
				var devEnvionment = new GoogleDriveDevEnvironment(this.configuration);

				using (var sftpClient = new SFtpBatch(this.configuration))
				{
					var context = this;

					this.relativePaths.ForEach(relativePath =>
					{
						var hasChanged = context.IsHashChanged(relativePath);

						// only upload files that have changed.
						if (hasChanged)
						{
							sftpClient.Upload(relativePath);
						}
					});
				}

				// update the current saved state.
				this._loadedHashes = this._savedHashes;

				// also copy to Google Drive so the dev environment is in-sync.
                try
                {
                    this.relativePaths.ForEach(devEnvionment.Copy);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
			}
			catch (Exception e)
			{
				Trace.TraceError("Failed to Upload Database." + e.Message);
			}
		}

		private Dictionary<string, int> _loadedHashes;
		private Dictionary<string, int> _savedHashes;

		private Diary _diary;
		private Diary GetDiary()
		{
			if (_diary == null)
			{
				var model = this._diarySerialization.LoadXml(this.configuration.DiaryXmlFilePath);
				_diary = this.diaryAssembler.Map(model);
				Sort();
				SaveListOfImages(model);
				this._loadedHashes = SaveFormatsPrivate(new SaveRequest() { AllJson = true, LatestEntriesJson = true, OldEntriesJson = true, WhoopsJson = true });
			}

			return _diary;
		}

		public void TestValidDiary()
		{
			var modelDiary = this.diaryAssembler.Map(GetDiary());
			this._diarySerialization.TestValidDiary(this.configuration.DiaryXmlFilePath + ".commit", modelDiary);
		}

		public void Save()
		{
			var modelDiary = this.diaryAssembler.Map(GetDiary());
			this._diarySerialization.SaveAndValidateXml(this.configuration.DiaryXmlFilePath, modelDiary);
		}

		private void SaveListOfImages(DiaryDatabase.Model.Data.Xml.Diary modelDiary)
		{
			var images1 = modelDiary.Entries.Where(entry => entry.Info != null && entry.Info.Images != null);

			var images2 = images1.SelectMany(entry => entry.Info.Images);
			var images = images2.Select(image => image.Path);
			var imageList = string.Join(",", images);
			var imageListFilePath = Path.Combine(this.configuration.BasePath, "imagelist.csv");
			File.WriteAllText(imageListFilePath, imageList);
		}

		public void SaveFormats(SaveRequest saveRequest)
		{
			this._savedHashes = SaveFormatsPrivate(saveRequest);
		}

		private Dictionary<string, int> SaveFormatsPrivate(SaveRequest saveRequest)
		{
			var modelDiary = this.diaryAssembler.Map(GetDiary());
			var hashes = new Dictionary<string, int>();

			// Save the whoops entries
			if (saveRequest.WhoopsJson)
			{
				var whoopsDiary = this.CreateWhoops(modelDiary);
				this.SaveJson(whoopsDiary, "whoops.json", hashes);
			}

			// Saves all the entries
			if (saveRequest.AllJson)
			{
				var allDiary = this.CreateAll(modelDiary);
				this.SaveJson(allDiary, "all.json", hashes);
			}

			// Save the latest 10 entries
			if (saveRequest.LatestEntriesJson)
			{
				var last10Diary = this.CreateLast(10, modelDiary);
				this.SaveJson(last10Diary, "lastTen.json", hashes);
			}

			// Create diary json files for each year
			const int START_YEAR = 1995;
			for (var year = START_YEAR; year <= DateTime.Now.Year; year++)
			{
				var yearDiary = CreateYear(year, modelDiary);
				this.SaveJson(yearDiary, string.Format("{0}.json", year), hashes);
			}

			if (saveRequest.OldEntriesJson)
			{
				var oldEntriesDiary = CreateOldEntries(START_YEAR, modelDiary);
				this.SaveJson(oldEntriesDiary, "old.json", hashes);
			}

			// create titles
			var noInfoDiary = this.CreateNoInfo(modelDiary);
			this.SaveJson(noInfoDiary, "noInfo-all.json", hashes);

			// create siteOptions.json
			this.SaveSiteOptions(hashes);

			return hashes;
		}

		private void SaveJson(DiaryDatabase.Model.Data.Xml.Diary diary, string filename, Dictionary<string, int> hashes)
		{
			var outPath = Path.Combine(this.configuration.JsonDirectoryPath, filename);
			var hash = _diarySerialization.SaveJsonDiary(outPath, diary, true);
			hashes[filename] = hash;
		}

		private void SaveSiteOptions(Dictionary<string, int> hashes)
		{
			var outPath = Path.Combine(this.configuration.JsonDirectoryPath, "siteOptions.json");
			var jsonSerializer = new DataContractJsonSerializer(typeof(SiteOptions));
			var siteOptions = new SiteOptions();
			siteOptions.Hashes = hashes;
			using (var fileWriter = new StreamWriter(outPath))
			{
				jsonSerializer.WriteObject(fileWriter.BaseStream, siteOptions);
			}
		}

		private DiaryDatabase.Model.Data.Xml.Diary CreateOldEntries(int beforeYear, DiaryDatabase.Model.Data.Xml.Diary modelDiary)
		{
			var oldEntries = new DiaryDatabase.Model.Data.Xml.Diary(modelDiary);
			oldEntries.Entries = oldEntries.Entries.OrderByDescending(entry => entry.DateEntry.Value).Where(entry => entry.DateEntry.Year < beforeYear).ToList();
			return this.PostProcessEntries(oldEntries);
		}

		private DiaryDatabase.Model.Data.Xml.Diary CreateYear(int year, DiaryDatabase.Model.Data.Xml.Diary modelDiary)
		{
			var latestEntries = new DiaryDatabase.Model.Data.Xml.Diary(modelDiary);
			latestEntries.Entries = latestEntries.Entries.OrderByDescending(entry => entry.DateEntry.Value).Where(entry => entry.DateEntry.Year == year).ToList();
			return this.PostProcessEntries(latestEntries);
		}

		private DiaryDatabase.Model.Data.Xml.Diary CreateLast(int nLatestNEntries, DiaryDatabase.Model.Data.Xml.Diary modelDiary)
		{
			var latestEntries = new DiaryDatabase.Model.Data.Xml.Diary(modelDiary);
			latestEntries.Entries = latestEntries.Entries.OrderByDescending(entry => entry.DateEntry.Value).Take(nLatestNEntries).ToList();
			return this.PostProcessEntries(latestEntries);
		}

		private DiaryDatabase.Model.Data.Xml.Diary CreateAll(DiaryDatabase.Model.Data.Xml.Diary modelDiary)
		{
			var latestEntries = new DiaryDatabase.Model.Data.Xml.Diary(modelDiary);
			latestEntries.Entries = latestEntries.Entries.OrderByDescending(entry => entry.DateEntry.Value).ToList();
			return this.PostProcessEntries(latestEntries);
		}

		private DiaryDatabase.Model.Data.Xml.Diary PostProcessEntries(DiaryDatabase.Model.Data.Xml.Diary latestEntries)
		{
			latestEntries.Entries.ForEach(entry =>
			{
				if (entry.Info != null && entry.Info.Content != null)
				{
					entry.Info.Content = this.ConvertImagesToHtml(entry.Info.Content);
				}
			});
			latestEntries.Locations = null;
			latestEntries.People = null;
			return latestEntries;
		}

		private DiaryDatabase.Model.Data.Xml.Diary CreateNoInfo(DiaryDatabase.Model.Data.Xml.Diary modelDiary)
		{
			var noInfoDiary = new DiaryDatabase.Model.Data.Xml.Diary(modelDiary);
			foreach (var entry in noInfoDiary.Entries)
			{
				entry.Info = null;
			}

			return noInfoDiary;
		}

		private DiaryDatabase.Model.Data.Xml.Diary CreateWhoops(DiaryDatabase.Model.Data.Xml.Diary modelDiary)
		{
			var whoopsDiary = new DiaryDatabase.Model.Data.Xml.Diary(modelDiary);

			// only get those entries with a whoops in it.
			whoopsDiary.Entries = whoopsDiary.Entries.Where(entry => entry.Info != null && entry.Info.OriginalContent != null && entry.Info.OriginalContent.Contains("</woops>")).ToList();
			whoopsDiary.People = null;
			whoopsDiary.Locations = null;

			// remove the info context not contained in the whoops elements.
			foreach (var entry in whoopsDiary.Entries)
			{
				int searchFromIndex = 0;
				var node = this.GetNextNode("woops", entry, ref searchFromIndex, searchFromIndex);
				if (node != null)
				{
					entry.Info.Content = this.ConvertImagesToHtml(node.InnerXml);
					entry.Title.Value = node.Attributes["woopstitle"].Value;
					entry.Title.Name = node.Attributes["rating"].Value;
					entry.Locations = null;
					entry.People = null;
					entry.First = null;
				}
			}

			return whoopsDiary;
		}

		private List<Entry> GetEntries()
		{
			return GetDiary().Entries;
		}

		public ICollection<Person> GetPeople()
		{
			return GetDiary().People;
		}

		public ICollection<Location> GetLocations()
		{
			return GetDiary().Locations;
		}

		public ICollection<Entry> GetEntries(DateEntry dateEntry)
		{
			return GetEntries().Where(entry => entry.DateEntry.Equals(dateEntry)).ToList();
		}

		public int DeleteEntries(DateEntry dateEntry)
		{
			var deletedCount = GetEntries().RemoveAll(entry => entry.DateEntry.Equals(dateEntry));
			return deletedCount;
		}

		public void Create(Entry entry)
		{
			GetEntries().Add(entry);
			Sort();
		}

		private void Sort()
		{
			var entryComparer = new EntryComparer();

			_diary.Entries.Sort(entryComparer);
			_diary.Locations = _diary.Locations.OrderBy(location => location.Name).ToList();
		}

		public void Update(UpdateEntryRequest updateEntryRequest)
		{
			//if (DeleteEntries(updateEntryRequest.OriginalDate) == 0)
			//	throw new InvalidOperationException("No record to update");

			DeleteEntries(updateEntryRequest.OriginalDate);
			DeleteEntries(updateEntryRequest.Entry.DateEntry);
			Create(updateEntryRequest.Entry);
		}

		private XmlDocument ConvertToXml(string stringXml)
		{
			var xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(string.Format("<root>{0}</root>", stringXml));
			return xmlDocument;
		}

		private string ConvertImagesToHtml(string content)
		{
			// <image forweb="50"><src>images/years/1994_08_XX-01-RadioControlledPlane.jpg</src><caption>Plane before the crash</caption>
			var ret = this.ReplaceNode("image", content, xmlNode => 
				{
					var sourceNode = xmlNode.SelectSingleNode("src");
					var imageSource = sourceNode.InnerText;
					var captionNode = xmlNode.SelectSingleNode("caption");
					var caption = captionNode.InnerText;

					return string.Format("<img src='{0}' class='diaryImage' alt='{1}' /><p class='diaryImageCaption'>{1}</p>", imageSource, caption);
				}
			);

			return ret;
		}

		private XmlNode GetNextNode(string nodeToFind, DiaryDatabase.Model.Data.Xml.Entry entry, ref int indexFound, int currentIndex = 0)
		{
			if (entry.Info == null)
			{
				return null;
			}

			var woopsStartIndex = entry.Info.OriginalContent.IndexOf("<" + nodeToFind, currentIndex, StringComparison.Ordinal);
			if (woopsStartIndex == -1)
			{
				return null;
			}

			string endTagName = string.Format("</{0}>", nodeToFind);
			var woopsEndIndex = entry.Info.OriginalContent.IndexOf(endTagName, StringComparison.Ordinal) + endTagName.Length;
			var content = entry.Info.OriginalContent.Substring(woopsStartIndex, woopsEndIndex - woopsStartIndex);

			var xmlDocument = this.ConvertToXml(content);
			var xPath = string.Format(@"root//{0}", nodeToFind);
			indexFound = woopsEndIndex;
			return xmlDocument.SelectSingleNode(xPath);
		}


		private string ReplaceNode(string nodeToFind, string sourceString, Func<XmlNode, string> replaceFunc)
		{
			var currentReturnString = sourceString;
			var nodeStartIndex = currentReturnString.IndexOf("<" + nodeToFind, StringComparison.Ordinal);

			while (nodeStartIndex != -1)
			{
				
				string endTagName = string.Format("</{0}>", nodeToFind);
				var nodeEndIndex = currentReturnString.IndexOf(endTagName, StringComparison.Ordinal) + endTagName.Length;
				var content = currentReturnString.Substring(nodeStartIndex, nodeEndIndex - nodeStartIndex);

				var xmlDocument = this.ConvertToXml(content);
				var xPath = string.Format(@"root//{0}", nodeToFind);
				var xmlNode = xmlDocument.SelectSingleNode(xPath);

				var replaceString = replaceFunc(xmlNode);

				var ret = new StringBuilder(currentReturnString.Length);
				ret.Append(currentReturnString.Substring(0, nodeStartIndex));
				ret.Append(replaceString);
				ret.Append(currentReturnString.Substring(nodeEndIndex));

				currentReturnString = ret.ToString();

				nodeStartIndex = currentReturnString.IndexOf("<" + nodeToFind, StringComparison.Ordinal);
			}

			return currentReturnString;
		}

	}
}