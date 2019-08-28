using System.IO;
using DiaryDatabase.Model.Data.Xml;

namespace DiaryDatabase.Helper
{
    public interface IFileStreamSerialize
    {
        string Filename { get; set; }
        int Serialize(StreamWriter stream, Diary diary);
        Diary Deserialize(StreamReader stream);
    }
}