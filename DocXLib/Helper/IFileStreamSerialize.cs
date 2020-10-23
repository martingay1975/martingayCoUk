using System.IO;
using DocXLib.Model.Data.Xml;

namespace DocXLib.Helper
{
    public interface IFileStreamSerialize
    {
        string Filename { get; set; }
        int Serialize(StreamWriter stream, Diary diary);
        Diary Deserialize(StreamReader stream);
    }
}