using System;
using System.IO;
using System.Xml.Serialization;
using DiaryDatabase.Model.Data.Xml;

namespace DiaryDatabase.Helper
{
	public class XmlFileStreamSerialize : IFileStreamSerialize
	{
		private static readonly XmlSerializer XmlSerializer = new XmlSerializer(typeof (Diary));
		private XmlSerializerNamespaces _xmlSerializerNamespaces;

		private XmlSerializerNamespaces Namespaces
		{
			get
			{
				if (_xmlSerializerNamespaces == null)
				{
					_xmlSerializerNamespaces = new XmlSerializerNamespaces();
					_xmlSerializerNamespaces.Add("", "");
				}
				return _xmlSerializerNamespaces;
			}
		}

		public string Filename { get; set; }

		public int Serialize(StreamWriter stream, Diary value)
		{
			XmlSerializer.Serialize(stream, value, Namespaces);
			return 0;
		}

		public Diary Deserialize(StreamReader stream)
		{
			try
			{
				var diary = (Diary)XmlSerializer.Deserialize(stream);
				return diary;
			}
			catch (InvalidOperationException ex)
			{
				if (ex.InnerException != null)
				{
					throw ex.InnerException;
				}
				
				throw;
			}
		}
	}
}