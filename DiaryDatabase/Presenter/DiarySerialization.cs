using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml;
using DiaryDatabase.Helper;
using DiaryDatabase.Model.Data.Xml;
using log4net;

namespace DiaryDatabase.Presenter
{
	public class DiarySerialization : IDiarySerialization
	{
		public DiarySerialization()
		{
			this.log = LogManager.GetLogger(nameof(DiarySerialization));
		}

		private readonly ILog log;

		public Diary Diary { get; private set; }

		public void TestValidDiary(string outpath, Diary diary)
		{
			this.SaveXml(outpath, diary);
			var xmlFile = new XmlFileStreamSerialize { Filename = outpath };
			
			try
			{
				using (var fileReader = new StreamReader(xmlFile.Filename))
				{
					xmlFile.Deserialize(fileReader);
				}
			}
			catch (XmlException xmlEx)
			{
				var offendingLine = File.ReadLines(outpath).ElementAtOrDefault(xmlEx.LineNumber - 2);
				var message = string.Format("The diary content has invalid XML. {1}{0}{0}{2}{0}{3}",
												Environment.NewLine, 
												xmlEx.Message,
												offendingLine, xmlFile.Filename);

				throw new InvalidOperationException(message);
			}
		}

		public Diary LoadXml(string inputDiaryXmlFilename)
		{
			try
			{
				var xmlFile = new XmlFileStreamSerialize { Filename = inputDiaryXmlFilename };

                Debug.WriteLine($"Loading diary: {xmlFile.Filename}");
				using (var fileReader = new StreamReader(xmlFile.Filename))
				{
					this.Diary = xmlFile.Deserialize(fileReader);
					return Diary;
				}
            }
			catch (Exception ex)
			{
				log.FatalFormat("Error loading the xml from file {0}. {1}", inputDiaryXmlFilename, ex);
				if (ex.InnerException != null)
				{
					log.Fatal("InnerException:", ex.InnerException);	
				}
				throw;
			}
		}

		public void SaveJson(string outJsonDirPath)
		{
			// 1995 => Current Year
			for (var year = 1995; year <= DateTime.Now.Year; year++)
			{
				int year1 = year;
				var yearsEntries = this.Diary.Entries.Where(entry => entry.DateEntry.Year == year1).OrderBy(entry => entry.DateEntry.Value);
				var diary = new Diary {Entries = yearsEntries.ToList()};
				var hash = SaveJsonFile(year, outJsonDirPath, diary);
			}

			// Pre 1995
			var pre1995Entries = this.Diary.Entries.Where(entry => entry.DateEntry.Year < 1995).OrderBy(entry => entry.DateEntry.Value);
			var diaryPre = new Diary { Entries = pre1995Entries.ToList() };
			SaveJsonFile(1994, outJsonDirPath, diaryPre);
		}

		public void SaveAndValidateXml(string outpath, Diary diary)
		{
			this.TestValidDiary(outpath + ".tmp", diary);
			this.SaveXml(outpath, diary);
		}

		private void SaveXml(string outpath, Diary diary)
		{
			var xmlFileStreamSerialize = new XmlFileStreamSerialize();
			using (var fileStream = new StreamWriter(outpath))
			{
				xmlFileStreamSerialize.Serialize(fileStream, diary);
                Debug.WriteLine($"Saving to {outpath}");
			}
		}

		public int SaveJsonDiary(string filePath, Diary diary, bool copyOptions)
		{
			var jsonFile = new JsonFileSerialize { Filename = filePath, CopyOptions = copyOptions };
			using (var fileWriter = new StreamWriter(jsonFile.Filename))
			{
				return jsonFile.Serialize(fileWriter, diary);
			}
		}

		private int SaveJsonFile(int year, string outJsonDirPath, Diary diary)
		{
			var filePath = Path.Combine(outJsonDirPath, $"{year}.json");
			return this.SaveJsonDiary(filePath, diary, true);
		}
   }
}