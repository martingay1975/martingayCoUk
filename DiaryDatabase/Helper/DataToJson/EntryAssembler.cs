using System;
using System.Collections.Generic;
using DiaryDatabase.Model.Data.Json;
using Entry = DiaryDatabase.Model.Data.Xml.Entry;

namespace DiaryDatabase.Helper.DataToJson
{
    class EntryAssembler
    {
		public static int CountOccurences(string source, string substring)
		{
			int n = 0;
			int occurrences = 0;

			if (substring != "")
			{
				while ((n = source.IndexOf(substring, n, StringComparison.Ordinal)) != -1)
				{
					n += substring.Length;
					++occurrences;
				}
			}

			return occurrences;
		}

        private static readonly DateAssembler DateAssembler = new DateAssembler();
        private static readonly FirstAssembler FirstAssembler = new FirstAssembler();
        private static readonly TitleAssembler TitleAssembler = new TitleAssembler();
        private static readonly InfoAssembler InfoAssembler = new InfoAssembler();
        private static readonly ImageAssembler ImageAssembler = new ImageAssembler();

        public Model.Data.Json.Entry Copy(Entry source, bool copyOptions)
        {
            var destination = new Model.Data.Json.Entry
                                  {
                                      Date = DateAssembler.Copy(source.DateEntry),
                                      First = FirstAssembler.Copy(source.First),
                                      Title = TitleAssembler.Copy(source.Title),
                                      Info = copyOptions ? InfoAssembler.Copy(source.Info) : null,
									  Version = source.Version
                                  };

			if (source.Info != null)
			{
				if (source.Info.Content != null)
				{
					var imagesOccurrences = CountOccurences(source.Info.OriginalContent, "<image");
					//var imagesOccurrences = CountOccurences(source.Info.Content, "class='diaryImage'");
					destination.Images = imagesOccurrences;

				}
				
				//if (source.Info.Images != null)
				//{
				//	destination.Images = new List<Image>(source.Info.Images.Count);
				//	foreach (var sourceImage in source.Info.Images)
				//	{
				//		destination.Images.Add(ImageAssembler.Copy(sourceImage));
				//	}
				//}
			}
            return destination;
        }
    }
}