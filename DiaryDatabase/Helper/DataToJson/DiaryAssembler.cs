using System.Collections.Generic;
using DiaryDatabase.Model.Data.Xml;

namespace DiaryDatabase.Helper.DataToJson
{
	class DiaryAssembler
	{
		public static DiaryAssembler Instance = new DiaryAssembler();

		private static readonly EntryAssembler EntryAssembler = new EntryAssembler();

		public Model.Data.Json.Diary Copy(Diary source, bool copyOptions)
		{
			var destination = new Model.Data.Json.Diary();
			int hash = 0;

			destination.Entries = new List<Model.Data.Json.Entry>();
			foreach (var sourceEntry in source.Entries)
			{
				destination.Entries.Add(EntryAssembler.Copy(sourceEntry, copyOptions));
				hash = unchecked((hash * 17) + sourceEntry.Version + 1);
			}

			destination.Hash = hash;
			return destination;
		}
	}
}
