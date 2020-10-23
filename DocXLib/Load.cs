using System.Diagnostics;
using System.IO;
using DocXLib.Helper;
using DocXLib.Model.Data.Xml;

namespace DocXLib
{
    public static class Load
    {
        public static Diary LoadXml(string inputDiaryXmlFilename)
        {
            var xmlFile = new XmlFileStreamSerialize { Filename = inputDiaryXmlFilename };

            Debug.WriteLine($"Loading diary: {xmlFile.Filename}");
            using (var fileReader = new StreamReader(xmlFile.Filename))
            {
                return xmlFile.Deserialize(fileReader);
            }
        }
	}
}
